/*
 * Copyright (c) 
 * 
 * 文件名称：   AirPlaneBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/2 15:32:18
 * 
 * 修改描述：   Boos行为类
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using System.Collections.Generic;
using UnityEngine.Networking;

public class AirPlaneBehaviour : NetworkBehaviour
{
    enum State
    {
        Entering = 0,
        Following,
        Exiting,
        Inactive,
    }
    #region Private Function
    private State _state = State.Entering;
    [SyncVar(hook="OnHealChange")]
    private float _curHealth = 4;
    private float _maxHealth;

    private float _coolTimer = 1.0f;            // 启动武器冷却时间
    private int _count = 0;
    private float _distance;
    private bool _hasHurt;
    #endregion

    #region Public Properties
    public GameObject _weaponPoint;             // 武器挂点
    public GameObject _attackPoint;             // 收击点
    public GameObject _smokePoint;              // 冒烟特效挂点
    public AnimationCurve _xCurve;
    public AnimationCurve _yCurve;
    public AnimationCurve _xRotation;
    public AnimationCurve _yRotation;
    public AnimationCurve _zRotation;

    public float MaxHealth { get { return _maxHealth; } }
    #endregion

    private float _lerpPosSpeed = 30;
    private List<Vector3> _synPosList = new List<Vector3>();
    // 非本地玩家，做位置插值用
    [SyncVar(hook = "OnChangeLerpPos")]                  // 位置
    private Vector3 _syncPos;
    void OnChangeLerpPos(Vector3 value)
    {
       if (!isServer)
       {
           _synPosList.Add(value);
       }
    }

    private float _lerpRotSpeed = 10;
    private Quaternion _lastRot;
    private List<Quaternion> _syncRotList = new List<Quaternion>();
    [SyncVar(hook = "OnChangeLerpRot")]                // 角度
    private Quaternion _syncRot;
    void OnChangeLerpRot(Quaternion value)
    {
        if (!isServer)
        {
            _syncRotList.Add(value);
        }
    }

    [Command]
    private void CmdSendPosToServer(Vector3 pos)
    {
        _syncPos = pos;
    }

    [Command]
    private void CmdSendEulerToServer(Quaternion value)
    {
        _syncRot = value;
        _lastRot = value;
    }

    void Start()
    {
        GameMode.Instance.Airplane = this;
        _maxHealth = 4;
    }

    /// <summary>
    /// 血量变化
    /// </summary>
    /// <param name="value"></param>
    private void OnHealChange(float value)
    {
        if (value >= 0)
        {
            Message message     = new Message(MessageType.Message_Boss_On_Damage, this);
            message["ratio"]    = value / _maxHealth;
            message.Send();
        }

        if (value == 0)
        {
            _state = State.Exiting;
            SamplePoolManager.Instance.RequestEffectInPoint(transform.position, SamplePoolManager.ObjectType.Effect_Boss);
            SamplePoolManager.Instance.NotifyDestroyingEffect(_weaponPoint);
            SpawnManager.Instance.BossComing = false;
            SoundController.Instance.StopBossMusic();
            SoundController.Instance.PlaySoundBossDie(transform);
            GameMode.Instance.Airplane = null;
        }

        if (!_hasHurt)
        {
            _hasHurt = true;
            SamplePoolManager.Instance.RequestEffect(_smokePoint.transform, Vector3.zero, SamplePoolManager.ObjectType.Effect_Boss_Smoke, true);
        }
    }

    /// <summary>
    /// 对位置坐插值
    /// </summary>
    private void LerpPosition()
    {
       if (_synPosList.Count > 0)
       {
           transform.position = Vector3.Lerp(transform.position, _synPosList[0], _lerpPosSpeed * Time.fixedDeltaTime);
           if (Vector3.Distance(transform.position, _synPosList[0]) < 0.1f)
           {
               _synPosList.RemoveAt(0);
           }

           if (_synPosList.Count > 10)
           {
               _lerpPosSpeed = 70;
           }
           else
           {
               _lerpPosSpeed = 50;
           }
       }
    }

    private void LerpRot()
    {
        if (_syncRotList.Count > 0)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _syncRotList[0], _lerpRotSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, _syncRotList[0]) < 1)
            {
                _syncRotList.RemoveAt(0);
            }

            if (_syncRotList.Count > 10)
            {
                _lerpRotSpeed = 20;
            }
            else
            {
                _lerpRotSpeed = 10;
            }
        }
    }

    public void Initialize()
   {
       gameObject.transform.position    = new Vector3(_xCurve.Evaluate(0), _yCurve.Evaluate(0), GameMode.Instance.GetForwardTransform().transform.position.z + 40);
       transform.rotation               = Quaternion.identity;
       _hasHurt                         = false;
       gameObject.SetActive(true);
   }

    public void OnDamage()
    {
        CmdDamage();
    }

    [Command]
    void CmdDamage()
    {
        _curHealth = _curHealth - 1;
    }

    void Update()
    {
        switch (_state)
        {
            case State.Inactive:
                return;
            case State.Entering:
                UpdateEnteringState(Time.fixedDeltaTime);
                break;
            case State.Following:
                UpdateFollowingState(Time.fixedDeltaTime);
                break;
            case State.Exiting:
                UpdateExitingState(Time.fixedDeltaTime);
                break;
        }
    }

    private float _last;
    private Vector3 _euler = Vector3.zero;
    private float _z;
    private void UpdateEnteringState(float deltaTime)
    {
        _last                       += deltaTime;
        float factor                = GameMode.Instance.GetForwardPlayerOrAISpeedFactor();
        factor                      = factor > 1.5f ? factor : 1.5f;
        factor                      = factor < 0.5f ? 1 : factor;
        _z                          = GameMode.Instance.GetForwardTransform().transform.position.z + 40 + factor * deltaTime * 150;
        transform.position          = new Vector3(_xCurve.Evaluate(_last), _yCurve.Evaluate(_last), _z);
        transform.localEulerAngles  = new Vector3(_xRotation.Evaluate(_last), _yRotation.Evaluate(_last) * 30, _zRotation.Evaluate(_last) * 30);

        if (_last >= 6)
        {
            _state = State.Following;
            _distance = transform.position.z - GameMode.Instance.GetForwardTransform().transform.position.z;
            SoundController.Instance.StopBossMusic();
            SoundController.Instance.StartBossMusic(1);
        }
    }

    IEnumerator SwitchExitingState()
    {
        yield return new WaitForSeconds(8);
        _state = State.Exiting;
        SpawnManager.Instance.BossComing = false;
    }

    private float _floatingYangle = 0.0f;
    private float _smallSinXdeviation;
    private void UpdateFollowingState(float deltaTime)
    {
        _last += deltaTime;
        if (_last >= 15)
        {
            _last -= 9;
        }
        transform.position = new Vector3(_xCurve.Evaluate(_last), transform.position.y, GameMode.Instance.GetForwardTransform().transform.position.z + _distance);
        transform.localEulerAngles = new Vector3(0, 0, _zRotation.Evaluate(_last) * 30);
        // 是否启动武器
        if (_coolTimer > 0)
        {
            _coolTimer -= deltaTime;
        }
        else
        {
            _coolTimer = 5.0f;
            StartCoroutine(ActivateWeapon());           
        }

        //Message message = new Message(MessageType.Message_Update_Boss_HitPoint, this);
        //message["pos"]  = _attackPoint.transform.position;
        //message.Send();
    }

    private void UpdateExitingState(float deltaTime)
    {
        gameObject.SetActive(false);
    }

    IEnumerator ActivateWeapon()
    {
        int num = 0;
        int lane = 0;
        bool hasGuided = false;
        while (num < 4)
        {
            // 如果不是Following状态，则不再释放武器
            if (_state != State.Following)
            {
                yield break;
            }

            // 使武器呈S型散落下来
            if (0 == _count % 2)
            {
                lane = num % GameConfig.GAME_COINFIG_LANES.Length;
            }
            else
            {
                lane = GameConfig.GAME_COINFIG_LANES.Length - num % GameConfig.GAME_COINFIG_LANES.Length - 1;
            }

            // 设置放置导弹和地雷的概率
            if (hasGuided)
            {
                SpawnManager.Instance.ActivateWeapon(SamplePoolManager.ObjectType.BonusLandmine, lane);
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                if (Util.Random(0, 100) > 60)
                {
                    hasGuided = true;
                    SpawnManager.Instance.ActivateWeapon(SamplePoolManager.ObjectType.BonusGuided, lane);
                    yield return new WaitForSeconds(0.3f);
                }
                else
                {
                    SpawnManager.Instance.ActivateWeapon(SamplePoolManager.ObjectType.BonusLandmine, lane);
                    yield return new WaitForSeconds(0.3f);
                }
            }                      

            ++num;
        }
        //SpawnManager.Instance.ActivateWeapon(SamplePoolManager.ObjectType.BonusGuidedAttack);
        ++_count;
    }
}
