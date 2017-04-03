/*
 * Copyright (c) 
 * 
 * 文件名称：   BonusBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/24 15:37:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using UnityEngine.Networking;
using System.Collections.Generic;


// NetworkBehaviour 无法向MonoBehaviour一样使用泛型参数，
public class BonusBehaviour : NetworkBehaviour
{
    public int CurrentLane;
   
    [SyncVar]
    public int syncLane;
    [SyncVar]
    public Vector3 syncPos;
    [SyncVar]
    public Vector3 syncDir;
    [SyncVar]
    public int syncBonusID;
    [SyncVar]
    public int syncOrder;
    [SyncVar(hook = "OnDataOver")]
    public bool syncFlag;
    void OnDataOver(bool value)
    {
        ResetBonus(syncLane, syncPos, syncDir, syncBonusID, syncOrder);
    }

    [SyncVar]
    public int ID;


    [SyncVar(hook = "OnChangeLerpPos")]                  // 位置
    private Vector3 _syncPos;
    void OnChangeLerpPos(Vector3 value)
    {
        _syncPos = value;
    }

    [SyncVar(hook = "OnChangeVelocity")]
    private Vector3 _syncVelocity;
    void OnChangeVelocity(Vector3 value)
    {
        _syncVelocity = value;
    }

    [Command]
    private void CmdSendVelocity(Vector3 value)
    {
        _syncVelocity = value;
    }

    [Command]
    private void CmdSendPosToServer(Vector3 pos)
    {
        _syncPos = pos;
    }

    [Command]
    private void CmdSendID(int value)
    {
        ID = value;
    }

    #region Public Members
    public SamplePoolManager.ObjectType _type;
    public GameObject _effect0;
    public GameObject _effect1;

    public GameObject _magicMesh;
    #endregion

    #region
    private bool _active;
    [SyncVar(hook="OnIsReady")]
    private bool _isReady;
    void OnIsReady(bool value)
    {
        _isReady = value;
        if (_type == SamplePoolManager.ObjectType.BonusGuided)
        {
            _effect0.SetActive(true);
        }
    }

    [Command]
    private void CmdTellServerIsReady()
    {
        _isReady = true;
    }

    //地雷或导弹
    private Vector3 _targetPos;
    private float _distanceZ = 30.0f;
    private PlayerKinematics _kin;
    private AIKinematics _ai;
    #endregion
    void OnEnable()
    {
        if (isServer)
        {
            return;
        }
        SamplePoolManager.Instance.AddToOppOrBonusList(GetComponent<SpawnableObjectType>()._type, gameObject);
    }

    void OnDisable()
    {
        if (isServer)
        {
            return;
        }
        SamplePoolManager.Instance.RemoveFromOppOrBonusList(GetComponent<SpawnableObjectType>()._type, gameObject);
    }

    /// <summary>
    /// 攻击boss导弹
    /// </summary>
    /// <param name="other"></param>
    /// <param name="id"></param>
    private void OnTriggerGuided(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            _kin = other.GetComponent<PlayerKinematics>();
            _active = true;
            _effect0.SetActive(false);
            _effect1.SetActive(true);
            SoundController.Instance.PlaySoundNoUse();
            id = _kin.ID;
            if (isServer)
            {
                CmdSendID(id);
            }

            if (_kin.isLocalPlayer)
            {
                GameMode.Instance.AddScore(id, 3);
            }
        }

        if (other.CompareTag(GameTag.AI))
        {
            _active = true;
            _effect0.SetActive(false);
            _effect1.SetActive(true);
            SoundController.Instance.PlaySoundGuided(transform);
            id = other.GetComponent<AIKinematics>().ID;
            GameMode.Instance.AddScore(id, 3);
            CmdSendID(id);
        }

        if (other.CompareTag(GameTag.AirplaneTage))
        {
            gameObject.SetActive(false);
            SoundController.Instance.PlayerSoundGuidedExplosion(transform);
            if (isServer)
            {
                GameMode.Instance.Airplane.OnDamage();
            }

            if (!GameMode.Instance.IsAI(ID))
            {
                if (LobbyManager.Instance.GetPlayerByID(ID).isLocalPlayer)
                {
                    GameMode.Instance.AddScore(ID, 10);

                    Message message = new Message(MessageType.Message_Trigger_Tips, this);
                    message["tips"] = (int)EnumTips.Beat;
                    message["pos"] = LobbyManager.Instance.GetPlayerByID(ID).transform.position;
                    message.Send();
                }
            }

            SamplePoolManager.Instance.RequestEffectInPoint(transform.position, SamplePoolManager.ObjectType.Effect_Missile_Explosion);
        } 
    }

    /// <summary>
    /// 地雷
    /// </summary>
    private void OnTriggerLandmine(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            _kin = other.GetComponent<PlayerKinematics>();
            _kin.CmdTellServerLandMine();
            if (_kin.isLocalPlayer)
            {
                SoundController.Instance.PlaySoundLandmine();

                //Message message = new Message(MessageType.Message_Camera_Shake, this);
                //message["intensity"] = 3.0f;
                //message["time"] = 1.0f;
                //message["landmine"] = false;
                //message.Send();
            }
            if (isServer)
            {
                ToDestroy();
            }
            SamplePoolManager.Instance.RequestEffectInPoint(transform.position, SamplePoolManager.ObjectType.Effect_Landmine_Explosion);

            gameObject.SetActive(false);
        }

        if (other.CompareTag(GameTag.AI))
        {
            _ai = other.GetComponent<AIKinematics>();
            _ai.OnTriggerLandmine();
            ToDestroy();
            SamplePoolManager.Instance.RequestEffectInPoint(transform.position, SamplePoolManager.ObjectType.Effect_Landmine_Explosion);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 魔方
    /// </summary>
    /// <param name="other"></param>
    /// <param name="id"></param>
    private void OnTriggerMagic(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            if (isServer)
                ToDestroy();
            else
                return;

            _kin = other.GetComponent<PlayerKinematics>();
            id = _kin.ID;
            if (_kin.HoldState == Hold.Hold)
            {
                return;
            }
            if (_kin.isLocalPlayer)
            {
                SoundController.Instance.PlaySoundProp();
            }
            switch (syncBonusID)
            {
                case 0:
                    _kin.CmdCrash();
                    break;
                case 1:
                    _kin.CmdOnSpawnEngine();
                    break;
                case 2:
                    _kin.CmdOnSpawnGun();
                    break;
                case 3:
                    _kin.CmdTellServerLeopard();
                    break;
                case 4:
                    _kin.CmdTellServerTransparent();
                    break;
            }

             GameMode.Instance.AddScore(id, 3);
        }

        if (other.CompareTag(GameTag.AI))
        {
            ToDestroy();
            _ai = other.GetComponent<AIKinematics>();
            id = _ai.ID;
            if (_ai.HoldState == Hold.Hold)
            {
                return;
            }
            switch (syncBonusID)
            {
                case 0:
                    _ai.OnActiveCrash();
                    break;
                case 1:
                    _ai.OnSpawnEngine();
                    break;
                case 2:
                    _ai.OnSpawnGun();
                    break;
                case 3:
                    _ai.ChangeToLerop();
                    break;
                case 4:
                    _ai.Transparent();
                    break;
            }
            GameMode.Instance.AddScore(id, 3);
        }
    }

    /// <summary>
    /// 加速带
    /// </summary>
    /// <param name="other"></param>
    /// <param name="id"></param>
    private void OnTriggerSpeed(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            _kin = other.GetComponent<PlayerKinematics>();
            id = _kin.ID;
            if (_kin.isLocalPlayer && _kin.TypeModel == PlayerKinematics.ModelType.Car)
            {
                SoundController.Instance.StopEngineMusic();
                SoundController.Instance.StartEngineMusic(2);
                if (Util.Random(0, 11) < 3)
                {
                    SoundController.Instance.PlayPersonSoundGOGOGO();
                }
            }
            if (isServer)
            {
                _kin.CmdActivateTurbo(1);
            }

            if (_kin.isLocalPlayer)
            {
                GameMode.Instance.AddScore(id, 3);
            }
        }

        if (other.CompareTag(GameTag.AI))
        {
            _ai = other.GetComponent<AIKinematics>();
            _ai.ActivateTurbo(1);
            id = _ai.ID;
            GameMode.Instance.AddScore(id, 3);
        }
    }

    ///// <summary>
    ///// 检查点
    ///// </summary>
    ///// <param name="other"></param>
    ///// <param name="id"></param>
    //private void OnTriggerCheckPoint(Collider other, int id)
    //{
    //    if (other.tag.Equals(GameTag.PlayerTag))
    //    {
    //        if (isServer)
    //            ToDestroy();
    //        else
    //            return;
    //        _kin = other.GetComponent<PlayerKinematics>();
    //        _kin.CmdCheckPoint(syncBonusID);
    //    }

    //    if (other.tag.Equals(GameTag.AI))
    //    {
    //        _ai = other.GetComponent<AIKinematics>();
    //        _ai.OnCheckPoint(syncBonusID);
    //    }
    //}

    /// <summary>
    /// 攻击玩家导弹
    /// </summary>
    /// <param name="other"></param>
    /// <param name="id"></param>
    private void OnTriggerGuidedAttack(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            _kin = other.GetComponent<PlayerKinematics>();
            _kin.CmdTellServerLandMine(); // 效果如果不同，要换;
            if (isServer)
            {
                ToDestroy();
            }
        }

        if (other.CompareTag(GameTag.AI))
        {
            _ai = other.GetComponent<AIKinematics>();
            _ai.OnTriggerLandmine();
            ToDestroy();
        }
    }

    /// <summary>
    /// 飓风
    /// </summary>
    /// <param name="other"></param>
    /// <param name="id"></param>
    private void OnTriggerTyphoon(Collider other, int id)
    {
        if (other.CompareTag(GameTag.PlayerTag))
        {
            _kin = other.GetComponent<PlayerKinematics>();
            if (_kin.HoldState == Hold.Hold || _kin.IsJumping || _kin.IsLefting)
            {
                return;
            }
            if (isServer)
            {
                _kin.CmdTellServerTyphoon(transform.position);
                //GetComponent<BoxCollider>().enabled = false;
                //StartCoroutine(DelayToDestroy());
            }
        }

        if (other.CompareTag(GameTag.AI))
        {
            _ai = other.GetComponent<AIKinematics>();
            if (_ai.HoldState == Hold.Hold || _ai.IsJumping || _ai.IsLefting)
            {
                return;
            }
            _ai.ActiveTyphoon(transform.position);
            //GetComponent<BoxCollider>().enabled = false;
            //StartCoroutine(DelayToDestroy());
        }
    }


    private void ToDestroy()
    {
        SamplePoolManager.Instance.NotifyDestroyingParent(gameObject, GetComponent<SpawnableObjectType>()._type);
    }

    /// <summary>
    /// 检查是否可以销毁
    /// </summary>
    /// <param name="checkDis"></param>
    /// <returns></returns>
    private bool CanDestroy(float checkDis)
    {
        if (GameMode.Instance.BackforwardDestroy(transform.position, checkDis, false))
        {
            ToDestroy();
            return true;
        }

        return false;
    }


    /// <summary>
    /// 对位置坐插值
    /// </summary>
    private void LerpPosition()
    {
        transform.position = Vector3.Lerp(transform.position, _syncPos, 20 * Time.fixedDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isReady)
        {
            return;
        }

        int id = -1;
        switch(_type)
        {
            case SamplePoolManager.ObjectType.BonusGuided:
                OnTriggerGuided(other, id);
                break;
            case SamplePoolManager.ObjectType.BonusLandmine:
                OnTriggerLandmine(other, id);
                break;
            case SamplePoolManager.ObjectType.BonusMagic:
                OnTriggerMagic(other, id);
                break;
            case SamplePoolManager.ObjectType.BonusSpeed:
                OnTriggerSpeed(other, id);
                break;
            case SamplePoolManager.ObjectType.BonusTyphoon:
                OnTriggerTyphoon(other, id);
                break;
            case SamplePoolManager.ObjectType.BonusGuidedAttack:
                OnTriggerGuidedAttack(other, id);
                break;
            //case SamplePoolManager.ObjectType.BonusCheckpoint:
            //    OnTriggerCheckPoint(other, id);
            //    break;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameTag.PlayerTag) && _type == SamplePoolManager.ObjectType.BonusSpeed)
        {
            _kin = other.GetComponent<PlayerKinematics>();
            _kin.OnTurboActive(false);
            if (_kin.TypeModel == PlayerKinematics.ModelType.Car && _kin.isLocalPlayer)
            {
                SoundController.Instance.StopEngineMusic();
                SoundController.Instance.StartEngineMusic(1);
            }
        }

        if (other.CompareTag(GameTag.AI) && _type == SamplePoolManager.ObjectType.BonusSpeed)
        {
            _ai = other.GetComponent<AIKinematics>();
            _ai.OnTurboActive(false);
        }
    }

    /// <summary>
    /// 延迟销毁
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayToDestroy()
    {
        yield return new WaitForSeconds(3);
        if (isServer)
        {
            ToDestroy();
        }
    }

    #region Public Function
    /// <summary>
    /// 数据初始化
    /// </summary>
    /// <param name="lane"></param>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    /// <param name="id"></param>
    /// <param name="order"></param>
    public void InitData(int lane = 0, Vector3 pos = default(Vector3), Vector3 dir = default(Vector3), int id = 0, int order = -1)
    {
        CurrentLane     = lane;
        syncLane        = lane;
        syncPos         = pos;
        syncDir         = dir;
        syncBonusID     = id;
        syncOrder       = order;
        syncFlag        = !syncFlag;
    }

    /// <summary>
    /// 行为初始化
    /// </summary>
    /// <param name="lane"></param>
    public void ResetBonus(int lane = 0, Vector3 pos = default(Vector3), Vector3 dir = default(Vector3), int id = 0, int order = -1)
    {
        if (-1 != order)
        {
            _kin = LobbyManager.Instance.GetPlayerByOrder(order);
        }

        switch(_type)
        {
            case SamplePoolManager.ObjectType.BonusGuided:
            case SamplePoolManager.ObjectType.BonusLandmine:
                _active     = false;
                _isReady    = false;
                transform.position  = GameMode.Instance.Airplane._weaponPoint.transform.position;
                _targetPos          = new Vector3(GameConfig.GAME_COINFIG_LANES[syncLane], 0, GameMode.Instance.Airplane.transform.position.z + _distanceZ);
                break;
            case SamplePoolManager.ObjectType.BonusMagic:
                _isReady    = true;
                _active     = true;
                transform.position = new Vector3(GameConfig.GAME_COINFIG_LANES[syncLane], 0.5f, GameMode.Instance.GetForwardPos().z + 100.0f);
                _magicMesh.transform.localEulerAngles += Vector3.up * syncLane * 50;
                break;
            case SamplePoolManager.ObjectType.BonusSpeed:
                _isReady            = true;
                _active             = true;
                transform.position = new Vector3(GameConfig.GAME_COINFIG_LANES[2 * lane], 0, GameMode.Instance.GetForwardPos().z + 100.0f);
                break;
            case SamplePoolManager.ObjectType.BonusTyphoon:
                if (EnvirManager.Instance.CurrentEnvirType == EnvirType.Dester_Scene)
                {
                    transform.Find("longjuanfeng0").gameObject.SetActive(false);
                    transform.Find("longjuanfeng1").gameObject.SetActive(true);
                }
                else
                {
                    transform.Find("longjuanfeng0").gameObject.SetActive(true);
                    transform.Find("longjuanfeng1").gameObject.SetActive(false);
                }
                _isReady            = true;
                _active             = true;
                GetComponent<BoxCollider>().enabled = true;
                transform.position = new Vector3(GameConfig.GAME_COINFIG_LANES[lane], 2f, GameMode.Instance.GetForwardPos().z + 100.0f);
                break;
            case SamplePoolManager.ObjectType.BonusGuidedAttack:
                _active = false;
                transform.position = GameMode.Instance.Airplane._weaponPoint.transform.position;
                _targetPos = GameMode.Instance.GetForwardPos() + Vector3.forward * 20;
                break;
            case SamplePoolManager.ObjectType.BonusCheckpoint:
                transform.position = new Vector3(0, 2.08f, GameConfig.GAME_CONIFIG_CHECK_POINT[syncBonusID]);
                break;
        }
    }

    public void UpdateFixedFrame()
    {
        Vector3 dir = Vector3.zero;
        float dis = 0;

        if (_type == SamplePoolManager.ObjectType.BonusMagic)
            _magicMesh.transform.localEulerAngles += Vector3.up * Time.fixedDeltaTime * 50;

        if (isServer)
        {
            #region 地雷 导弹
            if (_type == SamplePoolManager.ObjectType.BonusGuided || _type == SamplePoolManager.ObjectType.BonusLandmine)
            {
                if (GameMode.Instance.Airplane == null)
                {
                    ToDestroy();
                    return;
                }

                if (_active)
                {
                    dir = GameMode.Instance.Airplane._attackPoint.transform.position - transform.position;
                    dis = Vector3.Distance(GameMode.Instance.Airplane.transform.position, transform.position);
                    Quaternion toRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, dis / 30.0f);
                    transform.position += dir.normalized * Time.fixedDeltaTime * 100;
                }

                if (CanDestroy(10))
                {
                    return;
                }

                dis = Vector3.Distance(transform.position, _targetPos);
                if (dis <= 1.0f)
                {
                    _isReady = true;
                    CmdTellServerIsReady();
                }

                if (_isReady)
                    return;
                transform.position += (_targetPos - transform.position).normalized * Time.fixedDeltaTime * 60;
                CmdSendPosToServer(transform.position);
                return;
            }
            #endregion

            #region 加速带 飓风 魔方
            if (_type == SamplePoolManager.ObjectType.BonusSpeed 
                || _type == SamplePoolManager.ObjectType.BonusTyphoon 
                || _type == SamplePoolManager.ObjectType.BonusMagic
                || _type == SamplePoolManager.ObjectType.BonusCheckpoint)
            {
                CanDestroy(20);
                CmdSendPosToServer(transform.position);
            }
            #endregion

            #region 攻击玩家导弹
            if (_type == SamplePoolManager.ObjectType.BonusGuidedAttack && !_active)
            {
                if (transform.position.y <= 0)
                {
                    _active = true;
                    StartCoroutine(DelayToDestroy());
                }
                else
                    transform.position += (_targetPos - transform.position).normalized * Time.fixedDeltaTime * 30;
                CmdSendPosToServer(transform.position);
                return;
            }
            #endregion
        }

        if (_type == SamplePoolManager.ObjectType.BonusGuided && _active)
        {
            if (GameMode.Instance.Airplane == null)
                return;
            dir       = GameMode.Instance.Airplane._attackPoint.transform.position - transform.position;
            dis       = Vector3.Distance(GameMode.Instance.Airplane.transform.position, transform.position);
            Quaternion toRotation   = Quaternion.LookRotation(dir);
            transform.rotation      = Quaternion.Lerp(transform.rotation, toRotation, dis / 30.0f);
            transform.position      += dir.normalized * Time.fixedDeltaTime * 100;
            return;
        } 
        
        //if (_type == SamplePoolManager.ObjectType.BonusGuidedAttack)
        //{
        //    if (transform.position.y <= 0)
        //    {
        //        gameObject.SetActive(false);
        //    }
        //    else
        //        transform.position += (_targetPos - transform.position).normalized * Time.fixedDeltaTime * 20;
        //}
       
        //if (!isServer)
        //    LerpPosition();
    }
    #endregion
}
