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


public class PropBehaviour : MonoBehaviour
{
    #region Public Members
    public SamplePoolManager.ObjectType _type;
    #endregion

    #region
    private int _id;

    //地雷或导弹
    private bool _active;
    private bool _isReady;

    private int _idMagic;

    // 引擎
    private bool _isLeftEngine;
    private bool _isRightEngine;
    private bool _stopEngine;

    // 机枪
    private bool _isLeftGun;
    private bool _isRightGun;
    private bool _stopGun;
    public GameObject _firePoint;

    // 子弹
    private float _checkDis = 15 * 2;
    private float _speed;
    private Vector3 _spawnPoint;

    private bool _isAI;
    private PlayerKinematics _kin;
    private AIKinematics _ai;

    // 没用的导弹
    private float _life;
     
    private Rigidbody rb;
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (!_isReady)
        {
            return;
        }

        // 子弹
        if (_type == SamplePoolManager.ObjectType.BonusBullet)
        {
            if (other.tag.Equals(GameTag.Trucks) || other.tag.Equals(GameTag.Traffic))
            {
                ToDestroy();
                other.GetComponent<OpponentKinematics>().OnOpponentDestroyed(OpponentKinematics.ExplodType.Bullet, _id);
            }
        }
    }

    private void ToDestroy()
    {
        SamplePoolManager.Instance.NotifyDestroyingParent(gameObject, GetComponent<SpawnableObjectType>()._type);
    }

    private bool CanDestroy(float checkDis)
    {
        if (GameMode.Instance.BackforwardDestroy(transform.position, checkDis, false))
        {
            ToDestroy();
            return true;
        }

        return false;
    }

    #region Public Function

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="lane"></param>
    public void ResetBonus(int lane = 0, Vector3 pos = default(Vector3), Vector3 dir = default(Vector3), int id = -1, int order = -1, bool isAi = false)
    {
        _isAI = isAi;
        Transform lenTran = null;
        Transform renTran = null;
        Transform lguTran = null;
        Transform rguTran = null;
        if (!_isAI && -1 != id)
        {
            _kin    = LobbyManager.Instance.GetPlayerByID(id);
            lenTran = _kin._leftEngine.transform;
            renTran = _kin._rightEngine.transform;
            lguTran = _kin._leftGun.transform;
            rguTran = _kin._rightGun.transform;
        }
        if (_isAI && -1 != id)
        {
            _ai     = GameMode.Instance.GetAIByID(id);
            lenTran = _ai._leftEngine.transform;
            renTran = _ai._rightEngine.transform;
            lguTran = _ai._leftGun.transform;
            rguTran = _ai._rightGun.transform;
        }

        switch(_type)
        {
            case SamplePoolManager.ObjectType.BonusEngine:
                _active     = false;
                _isReady    = false;
                _stopEngine = false;
                if (0 == lane)
                {
                    _isLeftEngine   = true;
                    _isRightEngine  = false;
                    transform.position = new Vector3(lenTran.position.x, lenTran.position.y, lenTran.position.z - 2);
                    transform.SetParent(lenTran);
                }
                else
                {
                    _isLeftEngine   = false;
                    _isRightEngine  = true;
                    transform.position = new Vector3(renTran.position.x, renTran.position.y, renTran.position.z - 2);
                    transform.SetParent(renTran);
                }
                GameObject go = transform.Find("Effect_Point").gameObject;
                SamplePoolManager.Instance.RequestEffect(go.transform, Vector3.zero, SamplePoolManager.ObjectType.Effect_SpeedEngine_Red, true);
                break;
            case SamplePoolManager.ObjectType.BonusGun:
                _active     = false;
                _isReady    = false;
                _stopGun    = false;
                if (0 == lane)
                {
                    _isLeftGun          = true;
                    _isRightGun         = false;
                    transform.position  = new Vector3(lguTran.position.x, lguTran.position.y, lguTran.position.z - 2);
                    transform.SetParent(lguTran);
                }
                else
                {
                    _isLeftGun          = false;
                    _isRightGun         = true;
                    transform.position  = new Vector3(rguTran.position.x, rguTran.position.y, rguTran.position.z - 2);
                    transform.SetParent(rguTran);
                }
                break;
            case SamplePoolManager.ObjectType.BonusBullet:
                _id                 = id;
                _isReady            = true;
                rb                  = GetComponent<Rigidbody>();
                _speed              = 100;
                _spawnPoint         = pos;
                transform.position  = _spawnPoint;
                if (dir != default(Vector3))
                {
                    rb.velocity = dir * _speed;
                }
                else
                {
                    rb.velocity = new Vector3(0, 0, _speed);
                }
                break;
            case SamplePoolManager.ObjectType.Free_Guided:
                if (EnvirManager.Instance.CurrentEnvirType == EnvirType.Dester_Scene)
                {
                    transform.Find("Effect_Free_Guided0").gameObject.SetActive(false);
                    transform.Find("Effect_Free_Guided1").gameObject.SetActive(true);
                }
                else
                {
                    transform.Find("Effect_Free_Guided0").gameObject.SetActive(true);
                    transform.Find("Effect_Free_Guided1").gameObject.SetActive(false);
                }
                rb      = GetComponent<Rigidbody>();
                _speed  = 100;
                dir     = (new Vector3(0, pos.y, pos.z + 100) - pos).normalized;
                _life   = 3;
                transform.position = pos;
                rb.velocity = dir * _speed;
                break;
        }
    }

    /// <summary>
    /// 熄灭引擎
    /// </summary>
    public void StopEngine()
    {
        _stopEngine = true;
        transform.SetParent(null);
    }

    /// <summary>
    /// 停止机枪发射
    /// </summary>
    public void StopGun()
    {
        _stopGun = true;
        transform.SetParent(null);
    }

    public void FixedUpdate()
    {
        #region 引擎
        if (_type == SamplePoolManager.ObjectType.BonusEngine)
        {
            if (_stopEngine)
            {
                Vector3 pos = Vector3.zero;
                if (_isAI)
                {
                    pos = _ai.transform.position;
                }
                else
                {
                    pos = _kin.transform.position;
                }
                transform.position += Vector3.back * Time.fixedDeltaTime * 20;
                if (pos.z - transform.position.z > 5)
                {
                    GameObject go = transform.Find("Effect_Point").gameObject;
                    SamplePoolManager.Instance.NotifyDestroyingEffect(go);
                    ToDestroy();
                }
                return;
            }

            if (_active && !_isReady)
            {
                _isReady = true;
                if (_isAI)
                {
                    _ai.OnActiveEngine();
                    _ai.ActivateTurbo(8);
                }
                else
                {
                    if (_kin.isLocalPlayer)
                    {
                        _kin.CmdEngine();
                        _kin.CmdActivateTurbo(8);
                    }
                }
            }

            float distance = 0;
            distance = Vector3.Distance(transform.localPosition, Vector3.zero);

            if (distance < 0.1f)
            {
                _active = true;
            }
            else
            {
                Vector3 dir = Vector3.zero - transform.localPosition;
                float factor = _isAI ? _ai.SpeedFactor : _kin.SpeedFactor;
                float speed = 4 * factor;
                speed = speed > 2 ? speed : 2;
                transform.localPosition += dir.normalized * Time.fixedDeltaTime * speed;
            }
            transform.localEulerAngles = Vector3.zero;

            return;
        }
        #endregion

        #region 机枪
        if (_type == SamplePoolManager.ObjectType.BonusGun)
        {
            if (_stopGun)
            {
                Vector3 pos = Vector3.zero;
                if (_isAI)
                {
                    pos = _ai.transform.position;
                }
                else
                {
                    pos = _kin.transform.position;
                }
                transform.position += Vector3.back * Time.fixedDeltaTime * 60;
                if (pos.z - transform.position.z > 5)
                {
                    ToDestroy();
                }
                return;
            }

            if (_active && !_isReady)
            {
                _isReady = true;
                if (_isAI)
                {
                    _ai.OnActiveGun();
                }
                else
                {
                    _kin.CmdGun();
                }
                StartCoroutine(EndGun());
            }

            float distance = 0;
            distance = Vector3.Distance(transform.localPosition, Vector3.zero);

            if (distance < 0.1f)
            {
                _active = true;
            }
            else
            {
                Vector3 dir = Vector3.zero - transform.localPosition;
                float factor = _isAI ? _ai.SpeedFactor : _kin.SpeedFactor;
                float speed = 4 * factor;
                speed = speed > 2 ? speed : 2;
                transform.localPosition += dir.normalized * Time.fixedDeltaTime * speed;
            }
            transform.localEulerAngles = Vector3.zero;

            return;
        }
        #endregion

        #region 子弹
        if (_type == SamplePoolManager.ObjectType.BonusBullet)
        {
            if (rb.position.z - _spawnPoint.z >= _checkDis || _spawnPoint.z - rb.position.z >= _checkDis)
            {
                ToDestroy();
            }

        }
        #endregion

        #region 没用导弹
        if (_type == SamplePoolManager.ObjectType.Free_Guided)
        {
            if (_life > 0)
            {
                _life -= Time.fixedDeltaTime;
            }
            else
            {
                ToDestroy();
            }
        }
        #endregion
      
    }
    #endregion


    IEnumerator EndGun()
    {
        yield return new WaitForSeconds(8);
        _stopGun = true;
    }
}
