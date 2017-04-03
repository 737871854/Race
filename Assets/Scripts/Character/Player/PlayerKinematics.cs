
/*
 * Copyright (c) 
 * 
 * 文件名称：   Player.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/9 14:32:48
 * 
 * 修改描述：   角色车辆的行为逻辑处理脚本
 * 
 */


using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class PlayerKinematics : NetworkBehaviour
{
    public enum State
    {
        none = -1,
        run,
        transformLeopard,
        transformCar,
        dead,
    }


    public enum FlyState
    {
        None = -1,
        TakeOff,
        Flying,
        Landing,
    }

    public enum ModelType
    {
        None,
        Car,
        Leopard,
    }

    #region private menmbers
    private Hold _hold              = Hold.None;
    private State _state            = State.none;
    private FlyState _flyState      = FlyState.None;
    private ModelType _modelType    = ModelType.Car;
   
    private PlayerFx _playerFx;                         // 特效脚本
    private int _coin;                                  // 拥有币数
    private GameObject _cameraRef;                      // 相机引用
    private bool _isRunning;                            // 正在行驶
    private int _prevLane;                              // 之前所在航线
    private int _currentLane = 2;                       // 当前航线
    private float _maxSpeed = 100;                      // 最大速度
    private float _totalTime;                           // 行驶时间
    private int _rank;                                  // 排名
    private Vector3 _startDistance          = new Vector3(0, 0, 16);
    private bool _isOnLane;
    private int _isChangingLane             = -1;
    private float _grip                     = 16.0f;
    private bool _turboOn;                              // 启动加速效果
    private float _turboRatio;
    private float _turboSpeed               = 250;
    private bool _draftOn;
    private float _timeLostForHit           = -1.0f;
    private bool _wrongDirection            = false;
    private Vector3 _prevVel;

    private float _lastCollisionTime        = -1.0f;
    private float _avoidFrequentCollisionTime = -1.0f;

    // jump
    private TruckBehaviour _truckInUse;
    private bool _isLifting;                            // 车辆在平台上升过程
    private bool _isJumping;                            // 车辆起跳过程
    private float _liftAngle;                           // 车辆在平台上升角度
    private float _jumpDistance = 10;                   // 车辆跳跃距离
    private float _firingAngle = 45;                    // 起跳平台角度
    private float _graviryBase = 9.8f;                  // 重力
    private float _currentGravity = 9.8f;               // 当前所受重力
    private float _gravityMultiplier = 3.5f;            // 重力放大倍数
    private float _playerStartPositionY;                // 车辆起始位置Y值
    private Vector3 _jumpTarget;                        // 跳跃目标点
    private float _elapseTime;                          // 跳起受重力时间
    private float Vz;                                   // 车辆z轴方向速度
    private float Vy;                                   // 车辆y周方向速度
    private bool _isDescendingParabola;                 // 向下抛物运动
    private int _maxCombo;                              // 最高连击次数
    private int _caoChe;                                // 超车数
    private float _radiusExplosionEndJump = 15;         // 落地爆炸范围
    private bool _isFirstBounce = true;
    private float _jumpDistanceBounce   = 5.0f;
    private float _minSideAccRatio      = 1.5f;
    private float _maxSideAccRatio      = 1.5f;
    private float _startSideAccLane     = 400.0f;
    private float _endSideAccLane       = 4800.0f;
    private float _laneDelta            = 0.1f;
    private float _laneDelta2           = 0.05f;
    private float _sideAcc              = 100.0f;
    private float _sideAcc2             = 30.0f;
    private float _planeSideAcc         = 40.0f;
    private float _planeSideAcc2        = 30.0f;

    // Crash
    private LayerMask _ignoreLayers = 0;
    private bool _activeCrash       = false;
    private float _crashAngle0      = -30;
    private float _crashAngle1      = 30;
    private float _checkDis         = 8;
    private float _crashLastTimer;

    // Engine
    private List<PropBehaviour> _engineList = new List<PropBehaviour>();
    private float _engineLastTimer              = 8;
    private bool _activeEngine;

    // Gun
    private bool _activeGun;
    private List<PropBehaviour> _gunList = new List<PropBehaviour>();
    private float _gunlastTimer = 8;

    // Fly
    private float _verticalSpeed = 2.5f;
    private float _flyingHeight  = 1.5f;

    // Fire
    private float _fireRate  = 0.1f;
    private float _firetimer = 0.0f;

    // Transparent
    private bool _activeTransparent;
    private float _tranlastTimer = 8;

    // Leopard
    private float _leopardLastTimer = 8;
    private bool _leopardActive;

    // Hold
    private float _holdTimer = 8;

    // Draft
    private bool _isInDraft;
    private float _waitForDraftTimer;
    private Collider _lastHitCollider;
    private float _endDraftTimer;
    private int _landmineLane;

    // FreeGuided
    private float _freeGuidedTime;

    // Typhoon
    private float _typhoonTimer;
    private bool _isTyphoon;

    // Landmaine
    private bool _triggerLandmine;
    private float _landmineTime;
    private float _landmineHeight;

    private Rigidbody rb;                               // 玩家所挂刚体组件
    #endregion

    #region 同步数据

    private List<Vector3> _synPosList = new List<Vector3>();
    // 非本地玩家，做位置插值用
    [SyncVar(hook = "OnChangeLerpPos")]                  // 位置
    public Vector3 _syncPos;
    void OnChangeLerpPos(Vector3 value)
    {
        _syncPos = value;
        //if (!isLocalPlayer)
        //{
        //    _synPosList.Add(value);
        //}
    }

    [SyncVar]
    public float _syncDelta; 

    private List<Vector3> _syncVList = new List<Vector3>();
    [SyncVar(hook = "OnChangeLerpV")]
    public Vector3 _syncV;
    void OnChangeLerpV(Vector3 value)
    {
        _syncV = value;
    }

    [SyncVar]
    private Vector3 _syncEuler;

    [SyncVar]
    public int ID = -1;

    [SyncVar]
    public int UniqueOrder = -1;

    [SyncVar]
    private bool _inputLeft;

    [SyncVar]
    private bool _inputRight;

    [SyncVar]
    private float _syncDistance;

    [SyncVar]
    private int _syncScore;

    [SyncVar]
    private bool _syncIsUp;

    [SyncVar]
    private bool _syncIsDead;

    [SyncVar]
    private bool _syncIsContinue;

    [SyncVar]
    public Vector3 _syncPosEnvir0;

    [SyncVar]
    public Vector3 _syncPosEnvir1;

    [SyncVar(hook="OnActiveEngineEffect")]
    private bool _syncEngineEffect;

    private void OnActiveEngineEffect(bool value)
    {
        ActiveateEngineEffect(value);
    }

    [Command]
    void CmdTellServerDelta(float value)
    {
        _syncDelta = value;
    }

    [Command]
    void CmdTellServerEnvir0Pos(Vector3 value)
    {
        _syncPosEnvir0 = value;
    }

    [Command]
    void CmdTellServerEnvir1Pos(Vector3 value)
    {
        _syncPosEnvir1 = value;
    }

    [Command]
    void CmdTellServerIsUp(bool value)
    {
        _synPosList.Clear();
        _syncIsUp = value;
    }

    [Command]
    void CmdTellServerDistance(float value)
    {
        _syncDistance = value;
    }

    [Command]
    void CmdTellServerScore(int value)
    {
        _syncScore = value;
    }

    [Command]
    void CmdTellServerDead(bool value)
    {
        _syncIsDead = value;
    }

    [Command]
    void CmdTellServerContinue(bool value)
    {
        _syncIsContinue = value;
    }

    [SyncVar]
    public float _syncSpeedFactor;

    [Command]
    void CmdTellServerSpeedFactor(float value)
    {
        _syncSpeedFactor = value;
    }

    [Command]
    void CmdTellServerLeft(bool value)
    {
        _inputRight = value;
    }

    [Command]
    void CmdTellServerRight(bool value)
    {
        _inputLeft = value;
    }

    [SyncVar]
    private bool _goOneLaneDx;
    [Command]
    void CmdTellServerDx(bool value)
    {
        _goOneLaneDx = value;
    }

    [SyncVar]
    private bool _goOneLaneSx;
    [Command]
    private void CmdTellServerSx(bool value)
    {
        _goOneLaneSx = value;
    }

    [Command]
    private void CmdSendPosToServer(Vector3 pos)
    {
        _syncPos = pos;
    }

    [Command]
    private void CmdSendEulerToServer(Vector3 value)
    {
        _syncEuler = value;
    }

    [Command]
    private void CmdSendVelocityToServer(Vector3 v)
    {
        _syncV = v;
    }

    [SyncVar(hook = "OnTransparent")]
    private bool syncTransparent;
    void OnTransparent(bool value)
    {
        Transparent();
    }
    [Command]
    public void CmdTellServerTransparent()
    {
        syncTransparent = !syncTransparent;
    }

    [SyncVar(hook = "OnAcctivateTurbo")]
    private float syncLast;
    void OnAcctivateTurbo(float value)
    {
        ActivateTurbo(value);
        syncLast = -1;
    }
    [Command]
    public void CmdActivateTurbo(float last)
    {
        syncLast = last;
    }

    [SyncVar(hook = "OnToLeopard")]
    public bool syncLeopard;
    void OnToLeopard(bool value)
    {
        ChangeToLerop();
    }

    [Command]
    public void CmdTellServerLeopard()
    {
        syncLeopard = !syncLeopard;
    }

    [SyncVar(hook = "OnCrash")]
    private bool syncCrash;
    void OnCrash(bool value)
    {
        OnActiveCrash();
    }

    [Command]
    public void CmdEngineEffect(bool value)
    {
        _syncEngineEffect = value;
    }

    [Command]
    public void CmdCrash()
    {
        syncCrash = !syncCrash;       
    }

    [SyncVar(hook = "OnEngine")]
    private bool syncEngine;
    void OnEngine(bool value)
    {
        OnActiveEngine();
    }
    [Command]
    public void CmdEngine()
    {
        syncEngine = !syncEngine;
    }

    [SyncVar(hook = "OnGun")]
    private bool syncGun;
    void OnGun(bool value)
    {
        OnActiveGun();
    }

    [Command]
    public void CmdGun()
    {
        syncGun = !syncGun;
    }

    [SyncVar(hook = "OnSpawnGun")]
    private bool syncSpwanGun;
    void OnSpawnGun(bool value)
    {
        for (int i = 0; i < 2; ++i)
        {
            GameObject go   = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusGun);
            pb              = go.GetOrAddComponent<PropBehaviour>();
            pb.ResetBonus(i, Vector3.zero, Vector3.zero, ID, UniqueOrder);
            _gunList.Add(pb);
        }
    }

    [SyncVar(hook = "OnLanemine")]
    private bool syncLandmine;
    void OnLanemine(bool value)
    {
        _triggerLandmine = true;
        _landmineTime = 0.6f;
        _landmineHeight = 2;
        _lastHitCollider = null;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DORotate(new Vector3(720, 720, 0), 0.5f, RotateMode.FastBeyond360));
    }

    [Command]
    public void CmdTellServerLandMine()
    {
        syncLandmine = !syncLandmine;
    }

    [SyncVar(hook="OnNoUseEffect")]
    private bool syncNoUseEffect;
    void OnNoUseEffect(bool value)
    {
        SpawnFreeGuided();
    }

    [Command]
    void CmdOnNoUseEffect()
    {
        syncNoUseEffect = !syncNoUseEffect;
    }

    [Command]
    public void CmdOnSpawnGun()
    {
        syncSpwanGun = !syncSpwanGun;
    }

    [SyncVar(hook = "OnSpawnEngine")]
    private bool syncSpawnEngine;
    void OnSpawnEngine(bool value)
    {
        PropBehaviour prop = null;
        for (int i = 0; i < 2; ++i)
        {
            GameObject go   = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusEngine);
            prop            = go.GetOrAddComponent<PropBehaviour>();
            prop.ResetBonus(i, Vector3.zero, Vector3.zero, ID, UniqueOrder);
            _engineList.Add(prop);
        }
    }

    [Command]
    public void CmdOnSpawnEngine()
    {
        syncSpawnEngine = !syncSpawnEngine;
    }


    [SyncVar(hook = "OnActiveTyphoon")]
    private Vector3 syncTyphoon;
    void OnActiveTyphoon(Vector3 value)
    {
        syncTyphoon = value;
        ActiveTyphoon();
    }

    [Command]
    public void CmdTellServerTyphoon(Vector3 pos)
    {
        syncTyphoon = pos;
    }

    private float _lerpPosSpeed = 30;
    private float _lerpRotSpeed = 10;

    #endregion
   

    #region public menmbers
    public GameObject _shdow;
    public Character _character;                        // 角色脚本
    public Animator _animator0;
    public Animator _animator1;
    public Material _material0;
    public Material _material1;
    public SkinnedMeshRenderer _smr0;
    public SkinnedMeshRenderer _smr1;
    public GameObject _leftEngine;
    public GameObject _rightEngine;
    public GameObject _leftGun;
    public GameObject _rightGun;
    public GameObject _firePoint;
    public float _forwardAccMin = 15.0f;
    public float _forwardAccMax = 25.0f;
    public float _maxSpeedMin   = 150.0f;
    public float _maxSpeedMax   = 350.0f;
    public float _resistanceMin = 0.5f;
    public float _resistanceMax = 0.15f;
    public float _turboSpeedMin = 0.5f;
    public float _turboSpeedMax = 0.15f;

    public bool _analogicInput;
    public float _forwardAccTurbo   = 30;
    public float _forwardAcc        = 10.0f;
    public float _backWardAcc       = -30.0f;
    public float _limitMaxSpeed     = 420.0f;
    public float _rotCoeff          = 0.05f;
    #endregion

    #region 外部可调用
    public int Coin                     { get { return _coin; } set { _coin = value; } }
    public float TotalTime              { get { return _totalTime; } }
    public int CurrentLane              { get { return _currentLane; } }
    public bool IsOnlane                { get { return _isOnLane; } }
    public TruckBehaviour TruckTaken    { get { return _truckInUse; } }
    public bool ForceOneLaneDx          { set { _goOneLaneDx = value; } }
    public bool ForceOneLaneSx          { set { _goOneLaneSx = value; } }
    public bool IsRight                 { set { CmdTellServerRight(value); } }
    public bool IsLeft                  { set { CmdTellServerLeft(value); } }
    public float Distance               { get { return _syncDistance; } }
    public bool TurboOn                 { get { return _turboOn; } }
    public bool IsJumping               { get { return _isJumping; } }
    public bool IsLefting               { get { return _isLifting; } }
    public bool IsWrongDirection        { get { return _wrongDirection; } }
    public float SpeedFactor            { get { return _syncSpeedFactor; } }
    public int MaxCombo                 { get { return _maxCombo; } set { _maxCombo = value; } }
    public int CaoChe                   { get { return _caoChe; } }
    public float Speed                  { get { return PlayerRigidbody.velocity.z; } }
    public int Rank                     { get { return _rank; } }
    public FlyState FState              { get { return _flyState; } }
    public int Score                    { get { return _syncScore; } }
    public Hold HoldState               { get { return _hold; } }
    public ModelType TypeModel          { get { return _modelType; } set { _modelType = value; } }
    public bool IsDead                  { get { return _syncIsDead; } }
    public bool IsContinue              { get { return _syncIsContinue; } }
    public Vector3 Envir0Pos            { get { return _syncPosEnvir0; } }
    public Vector3 Envir1Pos            { get { return _syncPosEnvir1; } }
    public bool TransparentActive       { get { return _activeTransparent; } }
 
    public Rigidbody PlayerRigidbody
    {
        get
        {
            if (null == rb)
                rb = GetComponent<Rigidbody>();
            return rb;
        }
    }
    #endregion
    
    #region uninty callback

    public override void OnStartLocalPlayer()
    {
        // 数据拷贝
        if (null != GameMode.Instance.PlayerRef)
        {
            GameObject obj = GameMode.Instance.PlayerRef.transform.gameObject;
            GameMode.Instance.PlayerRef = null;
            Destroy(obj);
        }
    }

    void Start()
    {
        Init();

        if (isLocalPlayer)
        {
            GameMode.Instance.PlayerRef = this;
            GameMode.Instance.ChangeMode(RunMode.ReadyToRace);
            CmdSendPosToServer(transform.position);
            LobbyManager.Instance.LobbyPlayer.CmdTellServerIsReady();
        }

        LobbyManager.Instance.AddPlayer(this);
        GameMode.Instance.AddIDAndOrderToDic(UniqueOrder, ID);
    }

    private float _collisionTime;
    void OnCollisionEnter (Collision collision)
    {
        _oldHitCollider = null;
        _collisionTime = 1f;


        if (_lastCollisionTime != -1.0f)
            return;

        _totalTime = GameConfig.GAME_CONFIG_MAX_LIFE_TIME - LobbyManager.Instance.LobbyPlayer.LifeValue;
        float now   = _totalTime;

        if (_isJumping)
            return;

        if (now - _avoidFrequentCollisionTime < 0.5f)
            return;

        bool lateralCollision   = Mathf.Abs(collision.contacts[0].normal.x) >= 0.9f ? true : false;
        bool isPlayer           = collision.collider.CompareTag(GameTag.PlayerTag);
        bool isAI               = collision.collider.CompareTag(GameTag.AI);
        bool forwardCollision   = collision.contacts[0].normal.z <= -0.9f ? true : false;
        bool backwareCollision  = collision.contacts[0].normal.z >= 0.9f ? true : false;

        if (!_turboOn)
        {
            if (GameMode._oldSystemActive)
            {
                _lastCollisionTime          = now;
                PlayerRigidbody.velocity    = new Vector3(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.y, PlayerRigidbody.velocity.z * 0.7f);
                PlayerRigidbody.AddExplosionForce(5.0f, PlayerRigidbody.transform.position + PlayerRigidbody.transform.forward, 2.0f, 3.0f, ForceMode.VelocityChange);
                PlayerRigidbody.constraints = RigidbodyConstraints.None;
            }
            else
            {
                _avoidFrequentCollisionTime = now;
                if (lateralCollision && (isPlayer || isAI))
                {
                    PlayerRigidbody.velocity *= 0.5f;
                    _timeLostForHit = 0.54f;

                    bool goRight;
                    if (_currentLane == 0)
                        goRight = true;
                    else if (_currentLane == GameConfig.GAME_COINFIG_LANES.Length - 1)
                        goRight = false;
                    else
                        goRight = UnityEngine.Random.Range(1, 100) > 50;

                    float impulseForce  = goRight ? -2500 : 2500;
                    PlayerRigidbody.AddForce(Vector3.right * impulseForce, ForceMode.Impulse);
                    _goOneLaneDx        = goRight ? true : false;
                    _goOneLaneSx        = !_goOneLaneDx;
                }
                else if (forwardCollision)
                {
                    bool goRight;
                    if (_currentLane == 0)
                        goRight = true;
                    else if (_currentLane == 4)
                        goRight                 = false;
                    else goRight                = Util.Random(1, 100) > 50;
                    _goOneLaneDx                = goRight ? true : false;
                    _goOneLaneSx                = !_goOneLaneDx;
                    PlayerRigidbody.velocity    = new Vector3(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.y, 0.2f * PlayerRigidbody.velocity.z);
                    if (isLocalPlayer)
                    {
                        Message message         = new Message(MessageType.Message_Camera_Shake, this);
                        message["intensity"]    = 1.5f;
                        message["time"]         = 1.0f;
                        message["opp"]          = true;
                        message["landmine"]     = false;
                        message.Send();
                    }
                }else if (backwareCollision)
                {
                    PlayerRigidbody.velocity = new Vector3(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.y, 1.3f * PlayerRigidbody.velocity.z);
                }
            }
        }
    }

    void OnCollisionStay (Collision collision)
    {
        _triggerStay = true;
    }

    void OnTriggerEnter(Collider other)
    {
        _oldHitCollider = null;
        _collisionTime = 1f;
    }

    private bool _triggerStay;
    void OnTriggerStay(Collider other)
    {
        _triggerStay = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTag.Traffic) || other.CompareTag(GameTag.Trucks))
            GameMode.Instance.AddScore(ID, 3);

        _triggerStay = false;
    }
    #endregion


    #region Private Function

    /// <summary>
    /// 数据初始化
    /// </summary>
    private void Init()
    {
        _truckInUse         = null;
        _material0          = GameObject.Instantiate(_material0);
        _material1          = GameObject.Instantiate(_material1);
        _smr0.material      = _material0;
        _smr1.material      = _material1;
        ShaderToAlpha(_material0, 1);
        ShaderToAlpha2(_material1, 0);
        _turboRatio         = -1;
        _slipstreamDuration = 1;
        _syncDistance       = 0;
        _syncScore          = 0;
        _syncSpeedFactor    = 0;
        _syncIsDead         = false;
        _activeCrash        = false;
        _syncIsContinue     = false;
        _animator0.speed    = 0;
        _ignoreLayers       = 1 << LayerMask.NameToLayer(SceneLayerMask.IgnoreRaycast);
        _freeGuidedTime     = Util.Random(6, 12);
        _typhoonTimer       = 2;
        _isTyphoon          = false;
        _playerFx           = GetComponent<PlayerFx>();
        transform.name      = "Player_" + UniqueOrder + "_" + ID;
        transform.position  = EnvirManager.Instance.startPosList[UniqueOrder];
        _currentLane        = EnvirManager.Instance.initLane[UniqueOrder];
        _prevLane           = _currentLane;
    }

    #region 改变材质
    private void ShaderToStandard(Material material)
    {
        material.shader = Shader.Find("Standard");
    }

    private void ShaderToTransparent(Material material)
    {
        material.shader = Shader.Find("FenXiang/liangbian");
    }

    private void ShaderToAlpha2(Material material, float alpha)
    {
        material.shader = Shader.Find("Q5/Unity/Unlit/Texture Alpha Control2");
        material.SetFloat("_Alpha", alpha);
    }

    private void ShaderToAlpha(Material material, float alpha)
    {
        material.shader = Shader.Find("Q5/Unity/Unlit/Texture Alpha Control");
        material.SetFloat("_Alpha", alpha);
    }
    #endregion

    /// <summary>
    /// 是否开启碰撞检测
    /// </summary>
    /// <param name="active"></param>
    IEnumerator ActivateTrigger(bool active)
    {
        //if (active)
        //{
        //    GetComponent<Collider>().isTrigger  = true;
        //    //_collisionPositionTimer             = _collisionProtectionTime;
        //}else if (!_turboOn)
        //{
        //    //_collisionPositionTimer             = -1.0f;
        //    GetComponent<Collider>().isTrigger  = false;
        //}
        if (active)
            GetComponent<BoxCollider>().isTrigger = active;
        else
        {
            while(_triggerStay || _turboOn)
            {
                yield return new WaitForEndOfFrame();
            }
            GetComponent<BoxCollider>().isTrigger = active;
        }
    }

    /// <summary>
    /// 对位置坐插值
    /// </summary>
    private void LerpPosition()
    {
        if (_syncIsUp)
        {
            //_synPosList.Clear();
            transform.position = _syncPos;
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _syncPos + _syncDelta * Vector3.forward, Time.fixedDeltaTime);

        //_synPosList.Clear();
        //float factor = (-transform.position.z + _syncPos.z);
        //if (Mathf.Abs(factor) > 0.1f)
        //    transform.position = Vector3.Lerp(transform.position, _syncPos, Time.fixedDeltaTime * factor);
        //else
        //    transform.position = _syncPos;


        //if (_synPosList.Count > 0)
        //{
        //    float factor = _synPosList[0].z - transform.position.z;
        //    if (Mathf.Abs(factor) > 0.05f)
        //    {
        //        transform.position  = new Vector3(transform.position.x, transform.position.y, transform.position.z + 5 * factor * Time.fixedDeltaTime);
        //        _synPosList[0]      = new Vector3(_synPosList[0].x, _synPosList[0].y, transform.position.z);
        //        transform.position  = Vector3.Lerp(transform.position, _synPosList[0], _lerpPosSpeed * Time.fixedDeltaTime);
        //    }
        //    else
        //    {
        //        transform.position = new Vector3(transform.position.x, transform.position.y, _synPosList[0].z);
        //    }

        //    if (Vector3.Distance(transform.position, _synPosList[0]) < 0.5f)
        //    {
        //        _synPosList.RemoveAt(0);
        //    }

        //    if (_synPosList.Count > 2)
        //    {
        //        _lerpPosSpeed = 30;
        //    }
        //    else
        //    {
        //        _lerpPosSpeed = 20;
        //    }

        //    if (_synPosList.Count > 4)
        //    {
        //        _synPosList.RemoveAt(0);
        //    }
        //}
    }


    private void LerpEuler()
    {
        if (_isTyphoon || _triggerLandmine)
            return;
        PlayerRigidbody.transform.localEulerAngles = _syncEuler;
    }

    private void LerpVelocity()
    {
        PlayerRigidbody.velocity = _syncV;
    }

    /// <summary>
    /// 是否逆行
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    private bool IsInWrongDirection(int lane)
    {
        //if (lane > 2)
        //{
        //    return true;
        //}

        return false;
    }


    /// <summary>
    /// 改变角色状态
    /// </summary>
    /// <param name="state"></param>
    private void ChangeCharacterState(Character.State state)
    {
        if (null != _character)
            _character.state = state;
    }

    /// <summary>
    /// 
    /// </summary>
    IEnumerator FreeGuided()
    {
        int rand = Util.Random(1, 4);
        while(rand > 0)
        {
            SoundController.Instance.PlaySoundNoUse();
            GameObject go = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.Free_Guided);
            go.GetComponent<PropBehaviour>().ResetBonus(0, _firePoint.transform.position + (rand % 2 == 0 ? -1 : 1) * Vector3.right * 5);
            --rand;
            yield return new WaitForSeconds(1);
        }
    }

    /// <summary>
    /// 发射导弹
    /// </summary>
    private void SpawnFreeGuided()
    {
        StartCoroutine(FreeGuided());
    }

    private void RandFireFreeGuided()
    {
        if (_freeGuidedTime > 0)
            _freeGuidedTime -= Time.fixedDeltaTime;
        else
        {
            _freeGuidedTime = Util.Random(6, 12);
            CmdOnNoUseEffect();
        }
    }

    /// <summary>
    /// 触发飓风
    /// </summary>
    private void ActiveTyphoon()
    {
        _isTyphoon                          = true;
        _typhoonTimer                       = 2;
        _turboOn                            = false;
        PlayerRigidbody.velocity            = Vector3.zero;
        _playerFx.ActivateLunTai(true);
        StartCoroutine(ActivateTrigger(true));
        ActiveateEngineEffect(false);
        for (int i = 0; i < _gunList.Count; ++i)
        {
            _gunList[i].StopGun();
        }
        _gunList.Clear();

        for (int i = 0; i < _engineList.Count; ++i)
        {
            _engineList[i].StopEngine();
        }
        _engineList.Clear();
    }

    /// <summary>
    /// 玩家跟随
    /// </summary>
    private void UpdateFollowCard()
    {
        int card = GameMode.Instance.GetFollowCard();
        Message message = new Message(MessageType.Message_Show_Follow_Card, this);
        message["id"] = card;
        message.Send();
    }

    /// <summary>
    /// 更新飓风
    /// </summary>
    private void UpdateTyphoon()
    {
        if (_typhoonTimer > 0)
        {
            _typhoonTimer -= Time.fixedDeltaTime;
            transform.localEulerAngles -= Vector3.up * Time.fixedDeltaTime * 600 * (_typhoonTimer < 0.4f ? 0.4f : _typhoonTimer);
        }
        else
        {
            if (transform.localEulerAngles.y != 0)
                transform.localEulerAngles -= Vector3.up * Time.fixedDeltaTime * 600 * (_typhoonTimer < 0.4f ? 0.4f : _typhoonTimer);
            if (transform.localEulerAngles.y < 14)
                transform.localEulerAngles = Vector3.zero;

            if (transform.localEulerAngles == Vector3.zero)
            {
                _isTyphoon = false;
                _playerFx.ActivateLunTai(false);
                StartCoroutine(ActivateTrigger(false));
            }
        }
    }

    /// <summary>
    /// 更新触雷
    /// </summary>
    private void UpdateLandmine()
    {
        if (_landmineTime > 0)
        {
            _isFirstBounce = true;
            _landmineTime -= Time.fixedDeltaTime;
            if (transform.position.y < _landmineHeight)
                transform.position += Vector3.up * Time.fixedDeltaTime * 3;
        }
        else
        {
            if (transform.position.y > 0)
                transform.position -= Vector3.up * Time.fixedDeltaTime * 25;
            if (transform.position.y < 0)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);

            if (transform.position.y == 0 )
            {
                _triggerLandmine = false;
                _playerStartPositionY = 0.0f;
                EndJump(0.4f, 6, 0, true);
            }
        }
    }

    private Collider _oldHitCollider;
    /// <summary>
    /// 完美躲避 / 超车
    /// </summary>
    private void UpdateDraft()
    {
        if (!isLocalPlayer)
            return;

        if (_collisionTime > 0)
        {
            _oldHitCollider = null;
            _collisionTime  -= Time.deltaTime;
            return;
        }

        if (_turboOn || _activeCrash)
            return;

        RaycastHit hit;
        Vector3 startPos    = gameObject.transform.position + Vector3.forward * 3 - Vector3.up * 0.5f;
        Vector3 endPos      = gameObject.transform.position + Vector3.forward * 8 - Vector3.up * 0.5f;
        Physics.Raycast(startPos, endPos, out hit, 6, ~_ignoreLayers);
        if (hit.collider && null == _lastHitCollider)
        {
            if (hit.collider.gameObject.name == "ModelLandmine")
            {
                _lastHitCollider    = hit.collider;
                _landmineLane       = _currentLane;
            }
            else
                _lastHitCollider = null;
        }
        else if (null != _lastHitCollider && (hit.collider != _lastHitCollider || null == hit.collider))
        {
            // 躲避地雷成功
            if (_landmineLane != _currentLane)
            {
                GameMode.Instance.AddScore(ID, 3);
                Message message     = new Message(MessageType.Message_Trigger_Tips, this);
                message["pos"]      = _lastHitCollider.transform.position;
                message["tips"]     = (int)EnumTips.Avoid;
                message.Send();
                _landmineLane       = _currentLane;

                SoundController.Instance.PlaySoundDodgeSound();
            }

            _lastHitCollider = null;
        }

        RaycastHit hit2;
        startPos    = transform.position;
        startPos.y  = 0.5f;
        endPos      = new Vector3(startPos.x, 0.5f, startPos.z + 6);
        Ray ray     = new Ray(startPos, endPos - startPos);
        Physics.Raycast(ray, out hit2, 6, ~_ignoreLayers);
        if (hit2.collider == null && _oldHitCollider != null && transform.position.y <= 0.1f && _oldHitCollider.transform.position.y<= 0.1f)
        {
            AddCaoChe();
            Message message = new Message(MessageType.Message_Trigger_Tips, this);
            message["pos"]  = _oldHitCollider.transform.position;
            message["tips"] = (int)EnumTips.Prefect;
            message.Send();          
            _oldHitCollider = null;

            SoundController.Instance.PlaySoundDodgeSound();

        }else if (hit2.collider != null)
            _oldHitCollider = hit2.collider;

        if (transform.position.y > 0.1f)
            _oldHitCollider = null;
    }

    /// <summary>
    /// 车辆在平台上的上升过程，同时调整这头的朝向，当车的高度大于或者等于target的y值时，开始执行StartJump逻辑
    /// </summary>
    /// <param name="dt"></param>
    private void LiftingUpdate(float dt)
    {
        gameObject.transform.Translate(0, Vy * dt, Vz * dt);

        UpdateJumpingRotation(10.0f, dt);

        if (gameObject.transform.position.y >= _jumpTarget.y)
            StartJump(_jumpDistance);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="coeff"></param>
    /// <param name="dt"></param>
    private void UpdateJumpingRotation(float coeff, float dt)
    {
        Vector3 rotTan = new Vector3(0.0f, _liftAngle, 1.0f);
        PlayerRigidbody.MoveRotation(Quaternion.LookRotation(rotTan));
        _liftAngle += dt * coeff;
        _liftAngle = Mathf.Clamp(_liftAngle, 0.0f, 0.5f);
    }

    /// <summary>
    /// 做起跳的准备工作，并依据相应的起跳平台做相应的处理
    /// </summary>
    /// <param name="distanceZ"></param>
    private void StartJump(float distanceZ)
    {
        PlayerRigidbody.velocity = new Vector3(0.0f, PlayerRigidbody.velocity.y, PlayerRigidbody.velocity.z);
        InitializeJump(distanceZ);
        _isLifting      = false;
        _isJumping      = true;
        _elapseTime     = 0.0f;
    }

    /// <summary>
    /// 更新车辆跳起到落地过程
    /// </summary>
    /// <param name="dt"></param>
    private void JumpUpdate(float dt)
    {
        float verticalSpeed = (Vy - (_currentGravity * _elapseTime)) * dt;

        if (verticalSpeed < 0 && !_isDescendingParabola)
        {
            _isDescendingParabola = true;
            _currentGravity = _graviryBase * _gravityMultiplier;
        }

        gameObject.transform.Translate(0, verticalSpeed, Vz * dt);
        _elapseTime += dt;

        UpdateJumpingRotation(-0.5f, dt);

        if (gameObject.transform.position.y <= _jumpTarget.y)
        {
            EndJump();
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, _playerStartPositionY, gameObject.transform.position.z);
        }
    }

    /// <summary>
    /// 准备上平台
    /// </summary>
    private void StartLifting()
    {
        _isFirstBounce  = true;
        _isLifting      = true;
        _isJumping      = false;
        _currentGravity = _graviryBase;
        InitializeLifting();
    }

    /// <summary>
    /// 初始化上平台信息
    /// </summary>
    private void InitializeLifting()
    {
        _liftAngle = 0;

        float truckHeight = 1.0f;
        float truckLength = 1.0f;

        if (PlayerRigidbody.velocity.z < 70.0f)
        {
            PlayerRigidbody.velocity = new Vector3(PlayerRigidbody.velocity.x, PlayerRigidbody.velocity.y, 70.0f);
        }

        // 设置车z方向速度
        float playerForwardSpeed = Mathf.Max(PlayerRigidbody.velocity.z, _truckInUse.gameObject.GetComponent<Rigidbody>().velocity.z + 50.0f);
        // 计算车辆上升过程需要消耗的时间
        float lifeTime = truckLength / playerForwardSpeed * GameConfig.ToKmh;

        // 计算上升到的目标位置
        _jumpTarget = gameObject.transform.position + gameObject.transform.forward * _jumpDistance;
        _jumpTarget.y = truckHeight;

        Vz = truckLength / lifeTime;

        float L = truckLength / Mathf.Sin(90.0f - _firingAngle * Mathf.Deg2Rad);
        Vy = L / lifeTime;
    }

    /// <summary>
    /// 计算车辆着地点，以及起跳各个方向的速度
    /// </summary>
    /// <param name="distanceX"></param>
    private void InitializeJump(float distanceZ)
    {
        _currentGravity = _graviryBase;

        _jumpTarget = gameObject.transform.position + gameObject.transform.forward * distanceZ;
        _jumpTarget.y = _playerStartPositionY;

        float targetDistance = Vector3.Distance(gameObject.transform.position, _jumpTarget);

        float projectileVelocity = targetDistance / (Mathf.Sin(2 * _firingAngle * Mathf.Deg2Rad) / _graviryBase);

        Vz = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(_firingAngle * Mathf.Deg2Rad);
        Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(_firingAngle * Mathf.Deg2Rad) * 0.5f;
    }

    /// <summary>
    /// 结束跳跃过程
    /// </summary>
    private void EndJump(float time = 1, float intensity = 4, float distance = 6, bool landmine = false)
    {
        _isJumping = false;

        if (_isFirstBounce)
        {
            _currentGravity = _graviryBase;
            _isFirstBounce  = false;
            StartJump(_jumpDistanceBounce);
         
            // 播放落地特效 (需求变更，要求更随车)
            //SpawnManager.Instance.ActivateLandEffect(new Vector3(transform.position.x, 0, transform.position.z + distance));
            //Vector3 pos = new Vector3(transform.position.x, 0, transform.position.z + distance);
            //SamplePoolManager.Instance.RequestEffectInPoint(pos, SamplePoolManager.ObjectType.Effect_Land, true);
            if (_shdow.activeSelf)
                SamplePoolManager.Instance.RequestEffect(_shdow.transform, Vector3.zero, SamplePoolManager.ObjectType.Effect_Land, true);
            for (int i = 0; i < SamplePoolManager.Instance.OpponentList.Count; ++i )
            {
                OpponentKinematics ok = SamplePoolManager.Instance.OpponentList[i];
                float dis = (ok.transform.position - transform.position).magnitude;
                if (dis < _radiusExplosionEndJump)
                {
                    ok.OnEndJump();
                }
            }

            if (isLocalPlayer)
            {
                Message message = new Message(MessageType.Message_Camera_Shake, this);
                message["intensity"] = intensity;
                message["time"] = time;
                message["landmine"] = landmine;
                message.Send();

                SoundController.Instance.PlaySoundFallGroundSound();
            }
          
        }
        else
        {
            _isDescendingParabola = false;
            CmdTellServerIsUp(false);
        }
    }
    
    private void UpdateTurbo()
    {
        if (_turboRatio >= 0.0f)
        {
            // 加速倒计时
            _turboRatio -= Time.deltaTime * (1.0f / _turboDuration);
            //OnTurboUpdate(_turboRatio);

            if (_turboRatio < 0.5f)
            {
                SpawnManager.Instance.TurboTrafficMultiplier = 0.1f;
            }

            if (_turboRatio <= 0.0f)
            {
                _turboDuration = 0;
                _turboCounter = 0;
                _turboRatio = -1;
              
                OnTurboActive(false);
            }
        }

        if (_truckTurboRatio >= 0.0f)
        {
            _truckTurboRatio -= Time.deltaTime * (1.0f / _slipstreamDuration);
            if (_truckTurboRatio <= 0.0f)
                ActivateTruckTurbo(false);
        }
    }

    #region 玩家触碰道具后，被激活的功能
    private void UpdateFlyingState(float dt)
    {
        switch (_flyState)
        {
            case FlyState.None:
                return;
            case FlyState.TakeOff:
                UpdateTakeOff(dt);
                break;
            case FlyState.Flying:
                UpdateFlying(dt);
                break;
            case FlyState.Landing:
                UpdateLanding(dt);
                break;
        }
    }

    // 机枪
    private void UpdateGun(float dt)
    {
        if (!_activeGun)
            return;

        if (_gunlastTimer > 0)
            _gunlastTimer -= dt;
        else
        {
            _gunlastTimer = 8;
            _activeGun = false;
            _playerFx.ActivaterGunFx(false);
            for (int i = 0; i < _gunList.Count; ++i)
                _gunList[i].StopGun();

            if (isLocalPlayer)
            {
                SoundController.Instance.StopGunMusic();
                Message message = new Message(MessageType.Message_Magic, this);
                message["flag"] = false;
                message["index"] = -1;
                message.Send();
            }
        }

        Fire(dt, true);
    }

    /// <summary>
    /// 起飞
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateTakeOff(float dt)
    {
        float playerHeight  = PlayerRigidbody.transform.position.y;
        playerHeight        += _verticalSpeed * dt;
        if (playerHeight > _flyingHeight)
        {
            playerHeight    = _flyingHeight;
            _flyState       = FlyState.Flying;
        }
        PlayerRigidbody.transform.position = new Vector3(PlayerRigidbody.transform.position.x, playerHeight, PlayerRigidbody.transform.position.z);
    }

    private PropBehaviour pb;
    private void Fire(float dt, bool isGun)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos;
        float checkDis;
        // 机枪两边
        if (isGun )
        {
            startPos.y      = 0.5f;
            checkDis        = _checkDis * 1.2f;
            endPos          = new Vector3(startPos.x, 0.5f, startPos.z + checkDis);
            if (_firetimer <= 0)
            {
                for (int i = 0; i < _gunList.Count; ++i)
                {
                    GameObject go   = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusBullet);
                    pb              = go.GetComponent<PropBehaviour>();
                    pb.ResetBonus(0, _gunList[i]._firePoint.transform.position, Vector3.forward, ID, UniqueOrder);
                }
                _firetimer = _fireRate;
            }
            else
            {
                _firetimer -= dt;
            }

            ShotRaycast(isGun, startPos, endPos, checkDis);
        }
        //// 引擎射击
        //else
        //{
        //    startPos    = _firePoint.transform.position;
        //    checkDis    = _checkDis * 1.9f;
        //    endPos      = new Vector3(startPos.x, 0, startPos.z + checkDis);
        //    if (_firetimer <= 0)
        //    {
        //        GameObject go    = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusBullet);
        //        go.GetComponent<PropBehaviour>().ResetBonus(0, _firePoint.transform.position, (endPos - startPos).normalized, 0, UniqueOrder);
        //        _firetimer       = _fireRate;
        //    }
        //    else
        //    {
        //        _firetimer -= dt;
        //    }
        //}
    }

    /// <summary>
    /// 发射射线,射击
    /// </summary>
    private void ShotRaycast(bool isGun, Vector3 startPos, Vector3 endPos, float checkDis)
    {
        RaycastHit hit;
        Ray ray = new Ray(startPos, endPos - startPos);
        Physics.Raycast(ray, out hit, checkDis, ~_ignoreLayers);
        if (hit.collider)
        {
            OpponentKinematics oppKin = hit.collider.gameObject.GetComponent<OpponentKinematics>();
            if (null != oppKin)
                oppKin.OnOpponentDestroyed(OpponentKinematics.ExplodType.Bullet, ID);
        }
        else
        {
            Vector3 explosionPos = Vector3.forward * _checkDis;
            explosionPos.y = 0;
        }
    }

    private void ActivateTruckTurbo(bool active)
    {
        if (active)
        {
            if (_truckTurboRatio < 0.0f)
                _truckTurboRatio = 1.0f;
        }
        else
            _truckTurboRatio = -1.0f;
    }

    /// <summary>
    /// 飞行
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateFlying(float dt)
    {
        PlayerRigidbody.transform.position = new Vector3(PlayerRigidbody.transform.position.x, _flyingHeight, PlayerRigidbody.transform.position.z);

        Fire(dt, false);

        // Time up
        if (_engineLastTimer > 0)
            _engineLastTimer -= dt;
        else
        {
            _flyState = FlyState.Landing;
            for (int i = 0; i < _engineList.Count; ++i)
                _engineList[i].StopEngine();
            _engineList.Clear();

            if (isLocalPlayer)
            {
                Message message = new Message(MessageType.Message_Magic, this);
                message["flag"] = false;
                message["index"] = -1;
                message.Send();
            }
        }
    }

    /// <summary>
    /// 着陆
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateLanding(float dt)
    {
        float playerHeight = PlayerRigidbody.transform.position.y;
        playerHeight -= _verticalSpeed * dt;
        if (playerHeight < 0.0f)
        {
            playerHeight = 0.0f;
            _flyState = FlyState.None;
        }
        PlayerRigidbody.transform.position = new Vector3(PlayerRigidbody.transform.position.x, playerHeight, PlayerRigidbody.transform.position.z);
    }

    /// <summary>
    /// 引擎
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateEngine(float dt)
    {
        if (_activeEngine)
        {
            if (_engineLastTimer > 0)
                _engineLastTimer -= dt;
            else
            {
                _engineLastTimer = 8;
                _activeEngine = false;
                ActiveateEngineEffect(false);
                for (int i = 0; i < _engineList.Count; ++i)
                {
                    _engineList[i].StopEngine();
                }
                _engineList.Clear();

                if (isLocalPlayer)
                {
                    Message message = new Message(MessageType.Message_Magic, this);
                    message["flag"] = false;
                    message["index"] = -1;
                    message.Send();
                }
            }
        }
    }

    /// <summary>
    /// 透明
    /// </summary>
    /// <param name="dt"></param>
    private void UpdateTransparent(float dt)
    {
        if (_activeTransparent)
        {
            if (_tranlastTimer > 0)
                _tranlastTimer -= dt;
            else
            {
                _activeTransparent = false;
                _tranlastTimer = 8;
                EndTransparent();
            }
        }
    }

    #endregion
  
    #region 玩家动画状态处理
    private void UpdateState()
    {
        switch (_state)
        {
            case State.none:
                _animator0.speed = 4;
                _state = State.run;
                break;
            case State.run:
                ChangeToRun();
                break;
            case State.transformLeopard:
                ChangeToTransformLeopard();
                break;
            case State.transformCar:
                ChangeToTransformCar();
                break;
            case State.dead:
                ChangeToDead();
                break;
        }

        if (_leopardActive)
        {
            if (_leopardLastTimer > 0)
                _leopardLastTimer -= Time.deltaTime;
            else
            {
                _leopardLastTimer = 8;
                _leopardActive = false;

                if (isLocalPlayer)
                {
                    Message message = new Message(MessageType.Message_Magic, this);
                    message["flag"] = false;
                    message["index"] = -1;
                    message.Send();
                }
              
                ChangeToCar();
                if (isLocalPlayer)
                {
                    SoundController.Instance.StopLeopardRunMusic();
                    SoundController.Instance.StartEngineMusic(1);
                }
            }
        }
    }

    /// <summary>
    /// 切换正常行驶状态
    /// </summary>
    private void ChangeToRun()
    {
        _animator0.speed = PlayerRigidbody.velocity.z;
        AnimatorStateInfo info = _animator0.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.run.ToString()))
        {
            if (_animator0.gameObject.activeSelf)
            {                
                _animator0.SetInteger("State", (int)State.run);
            }
            if (_animator1.gameObject.activeSelf)
                _animator1.SetInteger("State", (int)State.run);
        }
    }

    /// <summary>
    /// 切换到猎豹状态
    /// </summary>
    private void ChangeToTransformLeopard()
    {
        _animator1.gameObject.SetActive(true);
        AnimatorStateInfo info = _animator0.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.transformLeopard.ToString()))
        {
            _animator0.speed = 1;
            _animator0.SetInteger("State", (int)State.transformLeopard);
            _animator1.SetInteger("State", (int)State.transformLeopard);
            _playerFx.ActivateCarChangeLeopard(true);
        }
        else
        {
            if (info.normalizedTime > 0.5f && info.normalizedTime < 0.99f)
            {
                _material0.SetFloat("_Alpha", 1 - 2 * (info.normalizedTime - 0.5f));
                _material1.SetFloat("_Alpha", 2 * (info.normalizedTime - 0.5f));
            }
            else if (info.normalizedTime >= 1)
            {
                _state = State.run;
                _animator0.gameObject.SetActive(false);
                _playerFx.ActivateCarChangeLeopard(false);
                _playerFx.ActivateLeopardEffect(true);
                if (isServer)
                    CmdActivateTurbo(8);
                if (isLocalPlayer)
                    SoundController.Instance.StartLeopardRunMusic();
                // 给的动作有位移问题，在此代码强制修复
                _animator1.gameObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    /// <summary>
    /// 切换到汽车形态
    /// </summary>
    private void ChangeToTransformCar()
    {
        _animator0.gameObject.SetActive(true);
        AnimatorStateInfo info = _animator0.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.transformCar.ToString()))
        {
            _animator0.speed = 1;
            _animator0.SetInteger("State", (int)State.transformCar);
            _animator1.SetInteger("State", (int)State.transformCar);
            _playerFx.ActivateLeopardEffect(false);
        }
        else
        {
            if (info.normalizedTime < 0.5f)
            {
                _material0.SetFloat("_Alpha", 2 * info.normalizedTime);
                _material1.SetFloat("_Alpha", 1 - 2 * info.normalizedTime);
            }else if (info.normalizedTime >= 0.5f && info.normalizedTime < 1)
                _character.isHide = false;
            else if (info.normalizedTime >= 1)
            {
                _state = State.run;
                _animator1.gameObject.SetActive(false);
                _playerFx.AcctivateNormalEndingeFx(true);

                // 给的动作有位移问题，在此代码强制修复
                _animator0.gameObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void ChangeToDead()
    {
        _animator0.speed = 0;
    }
    #endregion
   
    #endregion


    #region Public Function

    private float _turboDuration = 8;
    private int _turboCounter = 0;
    private float _truckTurboRatio = -1.0f;
    private float _slipstreamDuration;
   
    /// <summary>
    /// 开启普通特效
    /// </summary>
    public void ActivateGOEffect()
    {
        _playerFx.ActiveteReadyGoFx(0.3f);
    }

    /// <summary>
    /// 开启急促喷火引擎
    /// </summary>
    public void ActivateStartFire()
    {
        _playerFx.ActiveStartFire();
    }

    /// <summary>
    /// 启动加速功能
    /// </summary>
    public void ActivateTurbo(float last)
    {
        if (last == 1 && _modelType == ModelType.Car)
            _playerFx.ActivateQiLiu(true);

        if (_turboRatio * _turboDuration < last)
        {
            _turboDuration = last;
            _turboRatio = 1.0f;
            OnTurboActive(true);
        }

        if (_turboRatio <= 0)
        {
            if (_modelType == PlayerKinematics.ModelType.Car)
            {
                SoundController.Instance.StopEngineMusic();
                SoundController.Instance.StartEngineMusic(2);
            }
        }
    }

    ///// <summary>
    ///// 触雷,减速
    ///// </summary>
    //public void OnTriggerLandmine()
    //{
    //    _triggerLandmine    = true;
    //    _landmineTime       = 1;
    //    _lastHitCollider    = null;
    //}

    public void OnTruckLateralcollision()
    {
        _inputLeft = _inputRight = false;
        if (_currentLane != _prevLane)
            _currentLane = _prevLane;
        else if (_currentLane >= 2)
            _currentLane--;
        else
            _currentLane++;
        PlayerRigidbody.velocity += new Vector3(0, 0, -PlayerRigidbody.velocity.z);
    }

    /// <summary>
    /// 劫持
    /// </summary>
    public void OnHold()
    {
        _hold                   = Hold.Hold;
        _isFirstBounce          = false;
        _playerStartPositionY   = 0.9f;
        _jumpTarget             = new Vector3(_jumpTarget.x, _playerStartPositionY, _jumpTarget.z);
        _holdTimer              = 10;
        if (isServer)
            CmdActivateTurbo(_holdTimer);
    }

    /// <summary>
    /// 增加超车次数
    /// </summary>
    public void AddCaoChe()
    {
        ++_caoChe;
        GameMode.Instance.AddScore(ID, 5);
    }

    /// <summary>
    /// 透明穿墙
    /// </summary>
    private void Transparent()
    {
        _tranlastTimer                      = 8;
        _activeTransparent                  = true;
        _material0.shader                   = Shader.Find("FenXiang/liangbian");
        _shdow.SetActive(false);
        _material0.SetColor("_InnerColor", new Color(0.1f, 0.64f, 0.21f, 0.5f));
        _material0.SetColor("_RimColor", new Color(0.15f, 0.09f, 0.502f, 0.5f));
        ActivateTurbo(_tranlastTimer);
        _character.Transparent();
        _playerFx.ActivateTriggerProp();
        _playerFx.ActivatePropFollow(8);
        if(isLocalPlayer)
        {
            SoundController.Instance.PlayPersonSoundHide();

            Message message = new Message(MessageType.Message_Magic, this);
            message["flag"] = true;
            message["index"] = 4;
            message.Send();
        }
      
    }

    /// <summary>
    /// 结束透明
    /// </summary>
    private void EndTransparent()
    {
        _character.EndTransparent();
        ShaderToAlpha(_material0, 1);
        _shdow.SetActive(true);

        if (isLocalPlayer)
        {
            Message message = new Message(MessageType.Message_Magic, this);
            message["flag"] = false;
            message["index"] = -1;
            message.Send();
        }
    }


    /// <summary>
    /// 变豹子
    /// </summary>
    private void ChangeToLerop()
    {
        if (_leopardActive)
            return;

        _character.isHide   = true;
        _state              = State.transformLeopard;
        _leopardLastTimer   = 8;
        _leopardActive      = true;
        if (isLocalPlayer)
        {
            SoundController.Instance.StopEngineMusic();
            SoundController.Instance.PlayPersonSoundWow();
            Message message = new Message(MessageType.Message_Magic, this);
            message["flag"] = true;
            message["index"] = 0;
            message.Send();
        }
        _modelType = ModelType.Leopard;
        ShaderToAlpha(_material0, 1);
        ShaderToAlpha2(_material1, 0);
        _playerFx.AcctivateNormalEndingeFx(false);
        _playerFx.ActivateTriggerProp();
        _playerFx.ActivatePropFollow(8);
    }

    private void ChangeToCar()
    {
        ShaderToAlpha(_material0, 0);
        ShaderToAlpha2(_material1, 1);
        _state      = State.transformCar;
        _modelType  = ModelType.Car;
        if (_turboOn)
            _playerFx.ActivateTurboFx(true);
    }
   
    /// <summary>
    /// 逆向磁铁
    /// </summary>
    private void OnActiveCrash()
    {
        _activeCrash    = true;
        _crashLastTimer = 0;
        _playerFx.ActivateMagentFx(true);
        _playerFx.ActivateTriggerProp();
        _playerFx.ActivatePropFollow(5);
        if (isLocalPlayer)
        {
            SoundController.Instance.PlayPersonSoundGetAway();
            Message message = new Message(MessageType.Message_Magic, this);
            message["flag"] = true;
            message["index"] = 3;
            message.Send();
        }
    }

    /// <summary>
    /// 引擎
    /// </summary>
    private void OnActiveEngine()
    {
        _engineLastTimer    = 8;
        _activeEngine       = true;
        _playerFx.ActivateTriggerProp();
        ActiveateEngineEffect(true);
        _playerFx.ActivatePropFollow(8);
        if(isLocalPlayer)
        {
            SoundController.Instance.PlayPersonSoundYeah();
            Message message     = new Message(MessageType.Message_Magic, this);
            message["flag"]     = true;
            message["index"]    = 2;
            message.Send();
        }
    }
  
    /// <summary>
    /// 机枪
    /// </summary>
    private void OnActiveGun()
    {
        if (_activeGun)
            return;

        _activeGun      = true;
        _gunlastTimer   = 8;
        _playerFx.ActivaterGunFx(true);
        _playerFx.ActivateTriggerProp();
        _playerFx.ActivatePropFollow(8);
        if(isLocalPlayer)
        {
            SoundController.Instance.StartGunFireMusic();
            Message message     = new Message(MessageType.Message_Magic, this);
            message["flag"]     = true;
            message["index"]    = 1;
            message.Send();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    public void OnTurboActive(bool active)
    {
        if (!active)
            _playerFx.ActivateQiLiu(false);

        // 非加速带加速状态还未借宿
        if (!active && _turboRatio > 0 && _turboDuration != 1)
            return;

        if (isLocalPlayer)
        {
            if (!active)           
            {
                if (_modelType == PlayerKinematics.ModelType.Car)
                {
                    SoundController.Instance.StopEngineMusic();
                    SoundController.Instance.StartEngineMusic(1);
                }
            }
        }

        _turboOn = active;
        if (_turboOn)
        {
            _turboRatio = 1.0f;
            SpawnManager.Instance.TurboTrafficMultiplier = 0.1f;
        }
        else
        {
            _turboRatio = -1.0f;
            SpawnManager.Instance.TurboTrafficMultiplier = 0.6f;
        }

        _playerFx.ActivateTurboFx(active);
        StartCoroutine(ActivateTrigger(active));

        if (isLocalPlayer && SpeedFactor > 1)
        {
            Message message     = new Message(MessageType.Message_Speed_Accle, this);
            message["scale"]    = (int)Mathf.Ceil(SpeedFactor);
            message["active"]   = active;
            message.Send();
        }
    }

    //private void OnTurboUpdate(float ratio)
    //{
    //    _turboRatio = ratio;

    //    if (ratio <= 0.25f)
    //    {
    //        //SetTurboFxSize(ratio * 4.0f);
    //    }
    //}

    public void Update()
    {
        if(GameMode.Instance.Mode <= RunMode.ReadyToRace)
            return;

        // 本地玩家则发送位置到服务器
        if (isLocalPlayer)
        {
            CmdTellServerDelta(PlayerRigidbody.position.z - _syncPos.z);
            CmdSendPosToServer(PlayerRigidbody.position);
            CmdSendEulerToServer(PlayerRigidbody.transform.localEulerAngles);
        }
        else
        {
            LerpEuler();
            LerpPosition();
        }

        EnvirManager.Instance.UpdatePreFrame();
        
        if (IsInWrongDirection(_currentLane))
            _wrongDirection = true;
        else
            _wrongDirection = false;

        //if (_collisionPositionTimer >= 0.0f)
        //{
        //    _collisionPositionTimer -= Time.deltaTime;
        //    if (_collisionPositionTimer < 0.0f)
        //        OnCollisionProtectionActivation(false);
        //}

        // 启动Crash
        if (_activeCrash)
        {
            if (_crashLastTimer >= 8)
            {
                _activeCrash    = false;
                _crashLastTimer = 0;
                _playerFx.ActivateMagentFx(false);

                if (isLocalPlayer)
                {
                    Message message = new Message(MessageType.Message_Magic, this);
                    message["flag"] = false;
                    message["index"] = -1;
                    message.Send();
                }
            }else
                _crashLastTimer += Time.deltaTime;

            Vector3 startPos    = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z);
            _crashAngle0        += Time.deltaTime * 50;
            _crashAngle0        %= 360;
            if (_crashAngle0 > 30)
                _crashAngle0    -= 60;

            _crashAngle1        -= Time.deltaTime * 50;
            _crashAngle1        %= 360;
            if (_crashAngle1 < -30)
                _crashAngle1    += 60;

            Vector3 endPos0 = new Vector3(startPos.x + _checkDis * Mathf.Sin(_crashAngle0 * Mathf.Deg2Rad), startPos.y, startPos.z + _checkDis * Mathf.Cos(_crashAngle0 * Mathf.Deg2Rad));
            Vector3 endPos1 = new Vector3(startPos.x + _checkDis * Mathf.Sin(_crashAngle1 * Mathf.Deg2Rad), startPos.y, startPos.z + _checkDis * Mathf.Cos(_crashAngle1 * Mathf.Deg2Rad));
            Vector3 endPos2 = new Vector3(startPos.x, startPos.y, startPos.z + _checkDis);
            Ray ray0        = new Ray(startPos, endPos0 - startPos);
            Ray ray1        = new Ray(startPos, endPos1 - startPos);
            Ray ray2        = new Ray(startPos, endPos2 - startPos);
            //Debug.DrawRay(startPos, endPos0 - startPos, Color.red);
            //Debug.DrawRay(startPos, endPos1 - startPos, Color.blue);
            //Debug.DrawRay(startPos, endPos2 - startPos, Color.green);
            RaycastHit hit0;
            RaycastHit hit1;
            RaycastHit hit2;
            Physics.Raycast(ray0, out hit0, _checkDis, ~_ignoreLayers);
            Physics.Raycast(ray1, out hit1, _checkDis, ~_ignoreLayers);
            Physics.Raycast(ray2, out hit2, _checkDis, ~_ignoreLayers);
            if (null != hit0.collider)
            {
                if (hit0.collider.gameObject.GetComponent<OpponentKinematics>() != null)
                    hit0.collider.gameObject.GetComponent<OpponentKinematics>().OnFiretruckHit(ID);
            }
            if (null != hit1.collider)
            {
                if (hit1.collider.gameObject.GetComponent<OpponentKinematics>() != null)
                    hit1.collider.gameObject.GetComponent<OpponentKinematics>().OnFiretruckHit(ID);
            }
            if (null != hit2.collider)
            {
                if (hit2.collider.gameObject.GetComponent<OpponentKinematics>() != null)
                    hit2.collider.gameObject.GetComponent<OpponentKinematics>().OnFiretruckHit(ID);
            }
        }

        // 挟持
        if (_hold == Hold.Hold)
        {
            if (_holdTimer > 0)
                _holdTimer -= Time.deltaTime;
            else
            {
                _hold = Hold.ExitHold;
                StartCoroutine(ActivateTrigger(true));
            }
        }

        if (_hold == Hold.ExitHold)
        {
            if (_isOnLane && transform.position.y == 0)
            {
                _hold = Hold.None;
                StartCoroutine(ActivateTrigger(false));
            }
            UpdateLanding(Time.deltaTime * 3);
        }

        if (GameMode.Instance.Mode == RunMode.SceneEnd)
        {
            _playerFx.CloseAllEffect();
        }

        // 动作更新
        UpdateState();
      
        // 飞行
        UpdateFlyingState(Time.deltaTime);

        // 机枪
        UpdateGun(Time.deltaTime);

        // 透明
        UpdateTransparent(Time.deltaTime);

        UpdateEngine(Time.deltaTime);

        UpdateTurbo();

        UpdateDraft();
    }

    float targetSpeed;
    public void FixedUpdate()
    {
        if (GameMode.Instance.Mode < RunMode.Racing)
            return;

        if (_isTyphoon)
        {
            UpdateTyphoon();
            _inputLeft  = _inputRight = false;
            _maxSpeed   = 30;
        }else
            _maxSpeed = 100;

        if (_triggerLandmine)
        {
            UpdateLandmine();
            _inputLeft = _inputRight = false;
            return;
        }

        // 本地玩家则发送位置到服务器
        if (isLocalPlayer)
        {
            if (GameMode.Instance.Mode < RunMode.Racing)
                return;
                     
            if (GameMode.Instance.Mode == RunMode.SceneEnd)
            {
                if (_turboOn)
                    OnTurboActive(false);
                CmdEngineEffect(false);
                _maxSpeed   = 0;
                _inputLeft  = _inputRight = false;

                targetSpeed = _maxSpeed * GameConfig.ToMetersPerSecs;
                if (PlayerRigidbody.velocity.z < 0.4f * targetSpeed)
                    _playerFx.ActivateSideAir(false);
                else
                    _playerFx.ActivateSideAir(true);
            }

            if (GameMode.Instance.Mode == RunMode.Continue || GameMode.Instance.Mode == RunMode.Dead)
            {
                if (_turboOn)
                    OnTurboActive(false);
                _maxSpeed                       = 0;
                _inputLeft                      = _inputRight = false;
                if (PlayerRigidbody.velocity.z < 0.5f)
                    PlayerRigidbody.velocity    = Vector3.zero;
            }

            {
                _rank = LobbyManager.Instance.GetRankByDistance(UniqueOrder);

                // 手动控制换行
                float inputDirection = (_inputRight ? -1.0f : 0.0f) + (_inputLeft ? 1.0f : 0.0f);
                // 碰撞换行
                if (_goOneLaneDx || _goOneLaneSx)
                {
                    inputDirection  = (_goOneLaneDx ? 1.0f : 0.0f) + (_goOneLaneSx ? -1.0f : 0.0f);
                    _goOneLaneDx    = _goOneLaneSx = false;
                    CmdTellServerSx(_goOneLaneSx);
                    CmdTellServerDx(_goOneLaneDx);
                }

                if (_isLifting)
                {
                    CmdTellServerIsUp(true);
                    LiftingUpdate(Time.fixedDeltaTime);
                    inputDirection = 0.0f;
                }

                if (inputDirection == 1)
                    ChangeCharacterState(Character.State.right);
                else if (inputDirection == -1)
                    ChangeCharacterState(Character.State.left);
                else if (_character.state != Character.State.show && _character.state != Character.State.acc && _character.state != Character.State.over)
                    ChangeCharacterState(Character.State.normal);

                if (_isJumping)
                    JumpUpdate(Time.fixedDeltaTime);
                          
                UpdateFollowCard();
                RandFireFreeGuided();

                float targetX           = GameConfig.GAME_COINFIG_LANES[_currentLane];
                float deltalToTarget    = targetX - PlayerRigidbody.position.x;

                Vector3 force = Vector3.zero;

                // 换车道的方式
                if (_analogicInput)
                {
                    float lastLanePos   = GameConfig.GAME_COINFIG_LANES[4];
                    float firstlanePos  = GameConfig.GAME_COINFIG_LANES[0];

                    targetX = PlayerRigidbody.position.x;
                    if (inputDirection > 0)
                        targetX = lastLanePos;
                    else if (inputDirection < 0)
                        targetX = firstlanePos;

                    deltalToTarget = targetX - PlayerRigidbody.position.x;

                    if (deltalToTarget > _laneDelta)
                        force.x = _planeSideAcc * _minSideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget < -_laneDelta)
                        force.x = -_planeSideAcc * _minSideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget > _laneDelta2)
                        force.x = _planeSideAcc2 * _minSideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget < -_laneDelta2)
                        force.x = -_planeSideAcc2 * _minSideAccRatio * Time.fixedDeltaTime;

                    force.x -= PlayerRigidbody.velocity.x * _grip * Time.deltaTime;

                    if (PlayerRigidbody.position.x > lastLanePos)
                        PlayerRigidbody.position = new Vector3(lastLanePos, PlayerRigidbody.position.y, PlayerRigidbody.position.z);
                    else if (PlayerRigidbody.position.x < firstlanePos)
                        PlayerRigidbody.position = new Vector3(firstlanePos, PlayerRigidbody.position.y, PlayerRigidbody.position.z);
                }
                else
                {
                    _isOnLane       = Mathf.Abs(deltalToTarget) < 0.1f;
                    int newLane     = _currentLane;
                    // 计算新的赛道编号
                    if (inputDirection > 0.0f && _currentLane < GameConfig.GAME_COINFIG_LANES.Length - 1)
                        newLane++;
                    else if (inputDirection < 0.0f && _currentLane > 0)
                        newLane--;

                    // 
                    if (_isChangingLane < 0)
                    {
                        if (newLane != _currentLane)
                            _isChangingLane = _currentLane;
                    }
                    else
                    {
                        if (newLane == _currentLane)
                            _isChangingLane = -1;
                    }

                    if (_isOnLane)
                        _prevLane = _currentLane;

                    if (_currentLane == _prevLane && _isOnLane)
                        _currentLane = newLane;
                    else if (newLane == _prevLane)
                        _currentLane = newLane;

                    float sideAccRatio = _minSideAccRatio + Mathf.Clamp01((PlayerRigidbody.position.z - _startSideAccLane) / (_endSideAccLane - _startSideAccLane)) * (_maxSideAccRatio - _maxSideAccRatio);

                    if (deltalToTarget > _laneDelta)
                        force.x = _sideAcc * sideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget < -_laneDelta)
                        force.x = -_sideAcc * sideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget > _laneDelta2)
                        force.x = _sideAcc2 * sideAccRatio * Time.fixedDeltaTime;
                    else if (deltalToTarget < -_laneDelta2)
                        force.x = -_sideAcc2 * sideAccRatio * Time.fixedDeltaTime;

                    force.x -= PlayerRigidbody.velocity.x * _grip * Time.fixedDeltaTime;

                    if (_isJumping && _isFirstBounce)
                        force.x *= 0.1f;
                }

                targetSpeed = _maxSpeed * GameConfig.ToMetersPerSecs;

                if (PlayerRigidbody.velocity.z < 0.4f * targetSpeed)
                    _playerFx.ActivateSideAir(false);
                else
                    _playerFx.ActivateSideAir(true);

                if (_turboOn && _turboRatio >= 0.1f || _turboRatio >= 0.1f)
                    targetSpeed = _turboSpeed * GameConfig.ToMetersPerSecs;

                if (PlayerRigidbody.velocity.z <= targetSpeed)
                    force.z = (_turboOn ? _forwardAccTurbo : _forwardAcc) * Time.fixedDeltaTime;
                else if (PlayerRigidbody.velocity.z >= targetSpeed + 0.2f)
                    force.z = _backWardAcc * Time.fixedDeltaTime;

                if (_timeLostForHit >= 0.0f)
                {
                    _timeLostForHit -= Time.fixedDeltaTime;
                    force.z *= 0.1f;
                }

                PlayerRigidbody.AddForce(force, ForceMode.VelocityChange);

                _maxSpeed = Mathf.Min(_maxSpeed , _limitMaxSpeed);

                if (!_isLifting && !_isJumping && !_isTyphoon)
                {
                    Vector3 rotTran = new Vector3(PlayerRigidbody.velocity.x * _rotCoeff, 0.0f, 1.0f);
                    PlayerRigidbody.MoveRotation(Quaternion.LookRotation(rotTran));
                }

                float maxXVal = 8.0f;
                if (Mathf.Abs(PlayerRigidbody.transform.position.x) > maxXVal)
                {
                    float nextX = Mathf.Clamp(PlayerRigidbody.transform.position.x, -maxXVal, maxXVal);
                    PlayerRigidbody.transform.position = new Vector3(nextX, PlayerRigidbody.transform.position.y, PlayerRigidbody.transform.position.z);
                }

                _syncDistance = PlayerRigidbody.position.z - _startDistance.z;
                _syncDistance = _syncDistance < 0 ? 0 : _syncDistance;
                CmdTellServerDistance(_syncDistance);

            }
           
            _prevVel            = PlayerRigidbody.velocity;
            _syncSpeedFactor    = PlayerRigidbody.velocity.z / 20.0f;

            CmdSendVelocityToServer(_prevVel);
            CmdTellServerSpeedFactor(_syncSpeedFactor);

            CmdTellServerEnvir0Pos(EnvirManager.Instance.EnvirItem.StreetList[0].transform.position);
            CmdTellServerEnvir1Pos(EnvirManager.Instance.EnvirItem.StreetList[1].transform.position);
        }
        else
        {
            LerpVelocity();
        }
    }

    /// <summary>
    ///  重置游戏数据
    /// </summary>
    public void Reset(float lifeTime, float initSpeed, Vector3 initRBSpeed, Vector3 newStartDistance, int currLane)
    {
        PlayerRigidbody.velocity    = initRBSpeed;
        _currentLane                = currLane;
    }

    /// <summary>
    /// 加分
    /// </summary>
    /// <param name="value"></param>
    public void AddScore(int value)
    {
        CmdTellServerScore(_syncScore + value);
    }

    public void ActiveateEngineEffect(bool active)
    {
        _playerFx.ActivateEngineSpeedUp(active);
    }

    /// <summary>
    /// 当前角色死亡
    /// </summary>
    public void Dead()
    {
        _syncIsDead = true;
        CmdTellServerDead(true);
    }

    /// <summary>
    /// 当前角色进入续币
    /// </summary>
    /// <param name="value"></param>
    public void Continue(bool value)
    {
        _syncIsContinue = value;
        CmdTellServerContinue(value);
    }

    ///// <summary>
    ///// 启动观察相机
    ///// </summary>
    //public void ActiveObserveCamera()
    //{
    //    _observeCamera.gameObject.SetActive(true);
    //}

    /// <summary>
    /// 触发车辆时间入口
    /// </summary>
    /// <param name="truckGO"></param>
    public void OnTruckEnter(GameObject truckGO)
    {
        if (isLocalPlayer)
        {
            SoundController.Instance.PlaySoundSkipSound();
        }

        _truckInUse                 = truckGO.GetComponent<TruckBehaviour>();
        PlayerRigidbody.velocity    = new Vector3(0.0f, PlayerRigidbody.velocity.y, PlayerRigidbody.velocity.z);
        StartLifting();
        _playerStartPositionY       = 0.0f;
    }
    #endregion


    [Command]
    private void CmdSpawnObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }

}
