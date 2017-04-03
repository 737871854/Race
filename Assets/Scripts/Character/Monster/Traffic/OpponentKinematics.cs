/*
 * Copyright (c) 
 * 
 * 文件名称：   OpponentKinematics.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/11 10:55:35
 * 
 * 修改描述：   所有地面行驶的道具车辆行为管理脚本
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using UnityEngine.Networking;
using System.Collections.Generic;

public class OpponentKinematics : NetworkBehaviour
{
    public enum State
    {
        none = -1,
        run,
        roll,
    }

    public enum ExplodType
    {
        None,
        Trigger,
        Collide,
        Bullet,
        Crash,
        Truck,
    }

    public static float _lateralCollisionTime = -1;

    #region private members
    public int _currentLane;
    private int _id;
    private Rigidbody _rb;
    private Vector3 _startPosition;
    private float _currentMaxSpeed;
    private float _backupSideAcc;
    private LayerMask _ignoreLayers     = 0;
    private int _forwardDirectionOnLane = 1;
    private float _lastCollisionTime    = -1;
    private float _backCollisionTime    = -1.0f;
    private float _lastFxCollisionTime  = -1.0f;
    private bool _isChangingLane;
    private bool _isChangingLaneCollision;
    private bool _isTruck;

    // change lane
    private bool _canChangelane     = true;
    private bool _beginArrowBlink   = false;
    private GameObject _arrowLeft;
    private GameObject _arrowRight;

    #endregion

    #region public members
    public Animator _animator;
    public State _state = State.run;
    public OpponentId _vehicleId;
    public BoxCollider _holdBoxCollider;
    public float _forwardAcc        = 5.0f;
    public float _maxSpeed          = 50.0f;
    public float _maxSpeedBackwards = 40.0f;
    public float _grip              = 16.0f;
    public float _sideAcc           = 50.0f;
    public float _sideAcc2          = 10.0f;
    public float _laneDelta         = 0.1f;
    public float _laneDelta2        = 0.05f;
    public float _rotCoeff          = 0.01f;
    public GameObject _Explosion;
    #endregion

    #region public properties
    public int CurrentLane                      { get { return _currentLane; } set { _syncLane = value; } }
    public Vector3 CurrentPos                   { set { _syncPos = value; } }
    public Rigidbody OpponentRigidbody          { get { return _rb; } }
    public bool IsTruck                         { get { return _isTruck; } set { _isTruck = value; } }
    public float CurrentDistanceFromPlayer      { get { return gameObject.transform.position.z - LobbyManager.Instance.GetForwardPlayer().transform.position.z; } }
    #endregion

    #region 同步
    //  
    [SyncVar(hook = "OnLane")]                  // 车道
    public int _syncLane;
    void OnLane(int lane)
    {
        _currentLane = lane;
        _wandering = false;
        OnReset();
    }

    [SyncVar(hook="OnWander")]
    public bool _wandering;
    void OnWander(bool value)
    {
        _wandering = value;
    }

    private List<Vector3> _synPosList = new List<Vector3>();
    // 非本地玩家，做位置插值用
    [SyncVar(hook = "OnChangeLerpPos")]                  // 位置
    private Vector3 _syncPos;
    void OnChangeLerpPos(Vector3 value)
    {
        _syncPos = value;
    }

    public List<Vector3> _syncVList = new List<Vector3>();
    [SyncVar(hook = "OnChangeLerpV")]
    public Vector3 _lerpV;
    void OnChangeLerpV(Vector3 value)
    {
        _lerpV = value;
    }

    private List<Quaternion> _rotationList = new List<Quaternion>();
    [SyncVar(hook = "OnChangeLerpRotate")]                // 角度
    public Quaternion _lerpRotation;
    void OnChangeLerpRotate(Quaternion value)
    {
      
    }
  
    [Command]
    private void CmdSendPosToServer(Vector3 pos)
    {
        _syncPos = pos;
    }

    [Command]
    private void CmdSendRotationToServer(Quaternion rotation)
    {
            _lerpRotation = rotation;
    }

    [Command]
    private void CmdSendVelocityToServer(Vector3 v)
    {
        _lerpV = v;
    }

    #endregion

    public enum OpponentId
    {
        None = -1,
        Traffic0,
        Traffic1,
        Traffic2,
        Traffic3,
        Traffic4,
        Traffic5,
        Traffic6,
        Traffic7,
        Traffic8,
        TrafficGeneric,
        TruckTurbo,
    }

    #region Unity Callbacks
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _ignoreLayers       |= (1 << LayerMask.NameToLayer(SceneLayerMask.IgnoreRaycast) | (1 << LayerMask.NameToLayer(SceneLayerMask.Player)));
        _currentMaxSpeed    = _maxSpeed;
        _startPosition      = _rb.position;
        _backupSideAcc      = _sideAcc;
    }

    void OnEnable()
    {
        _rb.velocity                            = Vector3.zero;
        _rb.rotation                            = Quaternion.identity;
        GetComponent<Collider>().enabled        = false;
        SamplePoolManager.Instance.AddToOppOrBonusList(GetComponent<SpawnableObjectType>()._type, gameObject);
    }

    void OnDisable()
    {
        SamplePoolManager.Instance.RemoveFromOppOrBonusList(GetComponent<SpawnableObjectType>()._type, gameObject);
    }

    // Use this for initialization
    void Start()
    {
        OnReset();
    }

    void OnCollisionEnter(Collision collision)
    {
        //// 触雷或导弹
        //if (collision.collider.gameObject.CompareTag(GameTag.LandMine) || collision.collider.gameObject.CompareTag(GameTag.Guided))
        //{
        //    CarExplode(20.0f, transform.position + transform.forward);
        //    return;
        //}

        SamplePoolManager.Instance.RequestEffectInPoint(transform.position, SamplePoolManager.ObjectType.Effect_Side_Collision, false);

        if (collision.collider.gameObject.CompareTag(GameTag.PlayerTag) && collision.collider.gameObject.GetComponent<PlayerKinematics>().isLocalPlayer 
            || (collision.collider.gameObject.CompareTag(GameTag.Hold) && !collision.collider.gameObject.GetComponent<OpponentKinematics>().enabled))
            SoundController.Instance.PlayerSoundCollision(collision.gameObject.transform);

        bool lateralCollision = Mathf.Abs(collision.contacts[0].normal.x) >= 0.9f ? true : false;
      
        bool collidedWithTruck = false;
        if (null != collision.collider.GetComponent<TruckBehaviour>())
            collidedWithTruck = true;

        // 碰到Truck
        if (!_isTruck && collidedWithTruck && !lateralCollision)
        {
            CarExplode(20.0f, transform.position + transform.forward);
            return;
        }

        bool collidedWithPlayer = collision.collider.gameObject.CompareTag(GameTag.PlayerTag);

        if (_lastCollisionTime != -1 && collidedWithPlayer)
            return;

        if (collidedWithPlayer && collision.collider.GetComponent<PlayerKinematics>().IsJumping && !IsTruck)
        {
            GetComponent<Collider>().isTrigger = true;
            OnPlayerEndJump(0.0f);
            return;
        }

        float now = GameMode.Instance.PlayerRef.TotalTime;

        bool extremeLanes = _currentLane == 0 || _currentLane == 4;

        if (collidedWithPlayer && now - _lateralCollisionTime > 2.0f)
            collidedWithPlayer = false;

        if (now - _lateralCollisionTime < 0.1f && _backCollisionTime > 0.0f && extremeLanes)
            return;

        if (collision.collider.gameObject.CompareTag(GameTag.PlayerTag))
        {
            if (lateralCollision)
            {
                // 侧面撞击
                //_rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, -_rb.velocity.z * 0.3f);
                CarExplode(20.0f, transform.position + transform.forward, ExplodType.Collide, collision.collider.gameObject.GetComponent<PlayerKinematics>().ID);
                if (collision.collider.GetComponent<PlayerKinematics>().isLocalPlayer)
                {
                    Message message = new Message(MessageType.Message_Combo, this);
                    message["pos"] = transform.position;
                    message.Send();
                }
                return;
            }
        }
        else if (collision.collider.gameObject.CompareTag(GameTag.AI))
        {
            if (lateralCollision)
            {
                // 侧面撞击
                //_rb.velocity = new Vector3(_rb.velocity.x, _rb.velocity.y, -_rb.velocity.z * 0.3f);
                CarExplode(20.0f, transform.position + transform.forward, ExplodType.Collide, collision.collider.gameObject.GetComponent<AIKinematics>().ID);
                return;
            }
        }

        OpponentKinematics oppKin   = collision.collider.GetComponent<OpponentKinematics>();
        bool collisionWithBadGuy    = oppKin != null;
        if (collisionWithBadGuy && !IsTruck)
        {
            CarExplode(20.0f, transform.position + transform.forward);
            return;
        }

        if (IsTruck)
        {
            if (lateralCollision)
            {
                if (null != collision.collider.GetComponent<PlayerKinematics>())
                    collision.collider.GetComponent<PlayerKinematics>().OnTruckLateralcollision();
                else
                    return;
            }
        }else if (lateralCollision && _backCollisionTime < 1.3f)
        {
            _backCollisionTime                  = -1;
            _lastCollisionTime                  = now;
            _sideAcc                            = _backupSideAcc;
            bool explodeForDoubleCollisionCheck = collidedWithPlayer && _lastCollisionTime - OpponentKinematics._lateralCollisionTime < 0.8f;

            if (extremeLanes || explodeForDoubleCollisionCheck || collisionWithBadGuy)
            {
                bool isGoingOnExtremeLane = _isChangingLaneCollision && extremeLanes && !explodeForDoubleCollisionCheck;
                if (!isGoingOnExtremeLane)
                {
                    CarExplode(20.0f, transform.position + transform.forward);
                }
            }else if (!_isChangingLaneCollision )
            {
                _lateralCollisionTime       = now;
                bool collisionOnLeftSide    = collision.contacts[0].normal.x > 0.0f;
                _lastCollisionTime          = -1.0f;
                float impulseFore           = collisionOnLeftSide ? 3000.0f : -3000.0f;
                _rb.AddForce(Vector3.right * impulseFore, ForceMode.Impulse);

                OnChangeLane(collisionOnLeftSide);
                if (collidedWithPlayer)
                {
                    OpponentKinematics._lateralCollisionTime    = now;
                }
            }
        }

        if (!IsTruck && !lateralCollision && _backCollisionTime < 0.0f)
        {
            if (IsInWrongDirection(CurrentLane) && collision.collider.gameObject.CompareTag(GameTag.PlayerTag))
            {
                CarExplode(20.0f, transform.position + Vector3.forward, ExplodType.Collide, collision.collider.gameObject.GetComponent<PlayerKinematics>().ID);
            }else 
            {
                _backCollisionTime  = 1.5f;
                _sideAcc            = 20.0f;
                float impulseForce  = Random.Range(0, 100) > 50 ? -2500.0f : 2500.0f;
                _rb.AddForce(Vector3.right * impulseForce, ForceMode.Impulse);
            }
        }

        bool backCollison   = collision.contacts[0].point.z < gameObject.transform.position.z ? true : false;
        Vector3 collOffset  = Vector3.zero;
        Vector3 carSize     = gameObject.GetComponent<BoxCollider>().size;
        if (lateralCollision)
        {
            Vector3 side    = collision.contacts[0].normal.x < 0.0f ? Vector3.right : Vector3.left;
            collOffset      = side * (carSize.x * 0.5f);
        }else if (backCollison)
        {
            collOffset      = Vector3.back * (carSize.z * 0.5f);
        }else if (!backCollison)
        {
            _rb.velocity    = new Vector3(_rb.velocity.x, _rb.velocity.y, _rb.velocity.z * 0.7f);
        }

        if (now - _lastFxCollisionTime > 0.5f && collOffset != Vector3.zero)
        {
            _lastFxCollisionTime = now;
        }
    }

    void OnTriggerEnter (Collider other)
    {
        if (null != gameObject.GetComponent<BonusBehaviour>())
            return;

        if (null != other.GetComponent<SpawnableObjectType>() && (other.GetComponent<SpawnableObjectType>()._type == SamplePoolManager.ObjectType.BonusSpeed
                                                                || other.GetComponent<SpawnableObjectType>()._type == SamplePoolManager.ObjectType.BonusMagic
                                                                ))
            return;

        //if (other.gameObject.CompareTag(GameTag.LandMine) || other.gameObject.CompareTag(GameTag.Guided))
        //{
        //    CarExplode(20.0f, transform.position + transform.forward);
        //    return;
        //}

        bool collidedWithPlayer = other.gameObject.CompareTag(GameTag.PlayerTag);
        collidedWithPlayer      = collidedWithPlayer ? !other.GetComponent<PlayerKinematics>().TransparentActive : collidedWithPlayer;

        bool collidedWithHold   = other.gameObject.CompareTag(GameTag.Hold);
        bool collidedWithAI     = other.gameObject.CompareTag(GameTag.AI);
        collidedWithAI          = collidedWithAI ? !other.GetComponent<AIKinematics>().TransparentActive : collidedWithAI;
       
        bool isUsedTruck = false;
        if (_isTruck)
        {
            if (other.gameObject.CompareTag(GameTag.PlayerTag))
                isUsedTruck = (other.gameObject.GetComponent<PlayerKinematics>().IsLefting || other.gameObject.GetComponent<PlayerKinematics>().IsJumping);
        }
        if (isUsedTruck)
            return;

        if (null == other.GetComponent<Rigidbody>())
            return;
       

        int id = -1;
        if (collidedWithAI)
            id = other.gameObject.GetComponent<AIKinematics>().ID;
        else if (collidedWithPlayer)
            id = other.gameObject.GetComponent<PlayerKinematics>().ID;
        else if (collidedWithHold)
            id = other.gameObject.GetComponent<TruckHold>().SelectedCharacterID;

        if (collidedWithPlayer || collidedWithAI || collidedWithHold)
        {
            if (null != _holdBoxCollider)
                _holdBoxCollider.enabled = false;
            float deltaX = _rb.position.x - other.GetComponent<Rigidbody>().position.x;
            _rb.velocity = new Vector3(Mathf.Sign(deltaX) * Random.Range(20.0f, 35.0f), 0.0f, -other.GetComponent<Rigidbody>().velocity.z * 0.5f);
            SoundController.Instance.PlayerSoundCollision(other.gameObject.transform);
            CarExplode(20.0f, (_rb.position + other.GetComponent<Rigidbody>().position) * 0.5f, ExplodType.Trigger, id);
        }

        if (collidedWithPlayer && other.gameObject.GetComponent<PlayerKinematics>().isLocalPlayer )
        {
            Message message = new Message(MessageType.Message_Combo, this);
            message["pos"]  = transform.position;
            message.Send();
        }else if (collidedWithHold)
        {
            if (!other.gameObject.GetComponent<OpponentKinematics>().enabled && other.gameObject.GetComponent<TruckHold>().OwnerPlayerIsHold)
            {
                if (other.gameObject.GetComponent<TruckHold>().OwnerPlayer != null && other.gameObject.GetComponent<TruckHold>().OwnerPlayer.isLocalPlayer)
                {
                    Message message = new Message(MessageType.Message_Combo, this);
                    message["pos"]  = transform.position;
                    message.Send();
                }
            }
        }
    }
    #endregion

    #region Private Function
   
    /// <summary>
    /// 是否逆行
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    private bool IsInWrongDirection(int lane)
    {
        return false;
    }

    private void CarExplode(float explosionForce = 20.0f, Vector3 explosionPosition = default(Vector3), ExplodType type = ExplodType.Collide, int id = -1)
    {
        if (_hasExplode)
            return;

        _hasExplode                         = true;
        _rb.AddExplosionForce(10, explosionPosition, 2.0f, 3.0f, ForceMode.VelocityChange);
        float forwardForce                  = 10.0f;
        _rb.AddForce((Vector3.back + Vector3.up) * forwardForce, ForceMode.VelocityChange);
        _rb.constraints                     = RigidbodyConstraints.None;
        ActiveExplosionEffect();
        GetComponent<Collider>().enabled    = false;
        _state                              = State.roll;

        int score = 0;
        switch(type)
        {
            case ExplodType.Collide:
                score = 1;
                break;
            case ExplodType.Bullet:
                score = 3;
                break;
            case ExplodType.Trigger:
            case ExplodType.Truck:
                score = 2;
                break;
        }

        if (score > 0)
            GameMode.Instance.AddScore(id, score);
    }

    private void ActiveExplosionEffect()
    {
        _Explosion.SetActive(true);
    }

    private void OnPlayerEndJump(float angle)
    {
        Vector3 vect    = new Vector3(0.0f, 2.0f, 3.0f);
        _rb.AddForceAtPosition(vect * 30.0f, _rb.position, ForceMode.VelocityChange);
        _rb.constraints = RigidbodyConstraints.None;

        // 加特效和声音
    }

    private void OnChangeLane(bool collDx)
    {
        _isChangingLaneCollision = true;
        if (collDx)
            ++_currentLane;
        else
            --_currentLane;
    }

    private void Update()
    {
        if (null == _animator)
            return;

        if (isServer)
            CmdSendPosToServer(_rb.position);
        else
        {
            if (!_hasExplode)
                LerpPostion();
        }

       switch(_state)
       {
           case State.run:
               ChangeToRun();
               break;
           case State.roll:
               ChangeToRoll();
               break;
       }
    }

    private void ChangeToRun()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.run.ToString()))
        {
            _animator.SetInteger("State", (int)_state);
            _animator.speed = 1;
        }
    }

    private void ChangeToRoll()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.roll.ToString()))
        {
            _animator.SetInteger("State", (int)_state);
            _animator.speed = 10;
        }
    }
    #endregion

    #region Public Function
    public void OnReset()
    {
        _forwardDirectionOnLane = 1;
        _currentMaxSpeed    = _maxSpeed;
        _hasExplode         = false;
        _state              = State.run;
        _wandering          = false;
        _id                 = -1;
        _rb.position        = new Vector3(GameConfig.GAME_COINFIG_LANES[_currentLane], _rb.position.y, _rb.position.z + _currentMaxSpeed * GameConfig.ToMetersPerSecs * Time.fixedDeltaTime);
        _rb.velocity        = new Vector3(0.0f, 0.0f, _forwardDirectionOnLane * _currentMaxSpeed * GameConfig.ToMetersPerSecs);
        _lastCollisionTime  = -1;
        _backCollisionTime  = -1;    
        _rb.useGravity      = false;
        _rb.constraints     = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        StartCoroutine(ActiveCollider());
    }

    IEnumerator ActiveCollider()
    {
        yield return new WaitForSeconds(1);
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Collider>().enabled   = true;
        if (null != _holdBoxCollider)
            _holdBoxCollider.enabled = true;
    }

    /// <summary>
    /// 被子弹击中效果
    /// </summary>
    public void OnOpponentDestroyed(ExplodType type, int id)
    {
        CarExplode(40.0f, transform.position + transform.forward, type, id);
    }

    /// <summary>
    /// 相应player扫射
    /// </summary>
    public void OnFiretruckHit(int id)
    {
        if (!_wandering)
        {
            Vector3 impulseVector;
            if (_currentLane < 2)
                impulseVector = Vector3.left * 20.0f;
            else if (_currentLane > 2)
                impulseVector = Vector3.right * 20.0f;
            else
                impulseVector = Util.Random(0, 100) > 50 ? Vector3.right * 20.0f : Vector3.left * 20.0f;
            
            _rb.velocity    = Vector3.zero;
            _rb.AddForceAtPosition(impulseVector, Vector3.zero, ForceMode.Impulse);
            _rb.AddForce(impulseVector, ForceMode.VelocityChange);
            _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _wandering      = true;
            _id = id;
        }
    }

    /// <summary>
    ///  被砸飞
    /// </summary>
    public void OnEndJump()
    {
        CarExplode();
    }

    private void LerpRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, _lerpRotation, 10 * Time.deltaTime);
    }

    private void LerpVelocity()
    {
        _rb.velocity = _lerpV;
    }

    private void LerpPostion()
    {       
        float factor = (-transform.position.z + _syncPos.z);
        if (Mathf.Abs(factor) > 0.05f)
            transform.position = new Vector3(_syncPos.x, _syncPos.y, transform.position.z + factor * Time.fixedDeltaTime);
        else
            transform.position = _syncPos;
    }

    private bool _hasExplode;
    Vector3 force;
    Vector3 vel;
    float targetX;
    float deltaToTarget;
    Vector3 rotTran;
    public void UpdateFixedFrame()
    {
        Vector3 pos = _rb.position;
        if (_wandering)
        {
            if (Mathf.Abs(pos.x) > 3.5f && !_hasExplode)
            {
                SoundController.Instance.PlayerSoundCollision(transform.transform);
                CarExplode(20.0f, transform.position + transform.forward, ExplodType.Crash, _id);
            }
            return;
        }

        force   = new Vector3(0.0f, 0.0f, 0.0f);
        vel     = _rb.velocity;
        if (GameMode.Instance.Mode == RunMode.SceneEnd && _rb.velocity.z > 0)
            _rb.velocity -= _rb.velocity * Time.fixedDeltaTime * 50;

        if (Mathf.Abs(vel.z) <= _currentMaxSpeed * GameConfig.ToMetersPerSecs)
            force.z = _forwardDirectionOnLane * _forwardAcc * 10.0f * Time.fixedDeltaTime;
        else if (Mathf.Abs(vel.z) > _currentMaxSpeed * GameConfig.ToMetersPerSecs + 0.2f)
            force.z = -_forwardDirectionOnLane * _forwardAcc * 10.0f * Time.fixedDeltaTime;

        targetX         = GameConfig.GAME_COINFIG_LANES[_currentLane];
        deltaToTarget   = targetX - pos.x;

        bool isOnLane = Mathf.Abs(deltaToTarget) < 0.1f;

        if (isOnLane && _isChangingLane)
            _isChangingLane = false;
        else if (isOnLane && _isChangingLaneCollision)
            _isChangingLaneCollision = false;

        {
            if (deltaToTarget > _laneDelta)
                force.x = _sideAcc * Time.fixedDeltaTime;
            else if (deltaToTarget < -_laneDelta)
                force.x = -_sideAcc * Time.fixedDeltaTime;
            else if (deltaToTarget > _laneDelta2)
                force.x = _sideAcc2 * Time.fixedDeltaTime;
            else if (deltaToTarget < -_laneDelta2)
                force.x = -_sideAcc2 * Time.fixedDeltaTime;
            force.x -= vel.x * _grip * Time.fixedDeltaTime;

            _rb.AddForce(force, ForceMode.VelocityChange);
        }

        rotTran = new Vector3(vel.x * _rotCoeff, 0.0f, _forwardDirectionOnLane * 1.0f);
        _rb.MoveRotation(Quaternion.LookRotation(rotTran));

        if (_backCollisionTime > 0.0f)
        {
            _backCollisionTime -= Time.fixedDeltaTime;
            if (_backCollisionTime < 0.0f)
                _sideAcc = _backupSideAcc;
        }

        if (isServer)
         
            CmdSendVelocityToServer(_rb.velocity);
        else
        {
            if (!_hasExplode)
                LerpVelocity();
        }
    }
    #endregion
}
