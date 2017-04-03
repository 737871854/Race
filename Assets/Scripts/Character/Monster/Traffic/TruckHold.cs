/*
 * Copyright (c) 
 * 
 * 文件名称：   TruckHold.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/2 10:23:23
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using UnityEngine.Networking;
using System.Collections;

public class TruckHold : NetworkBehaviour
{
    [SyncVar(hook="SyncHold")]
    public int _hold = 10;
    void SyncHold(int value)
    {
        OnHold(value);
    }

    public Rigidbody _rb;
    public float _rotCoeff = 0.05f;
    public Animator _animator;
    public HoldTrigger _trigger;
    public Transform Effect_Body_Point;
    public Transform Effect_Left_Engine_Point;
    public Transform Effect_Right_Engine_Point;

    private PlayerKinematics _pk = null;
    private AIKinematics _ai = null;
    private bool _isAI;


    public bool IsAI
    {
        get
        {
            return _isAI;
        }
    }

    public enum State
    {
        run = 0,
        run1,
        run2,
    }

    public State _state = State.run;
    public bool OwnerPlayerIsHold
    {
        get 
        { 
           if (!_isAI)
           {
               if (null == _pk)
               {
                   return false;
               }
               else
               {
                   if (_pk.HoldState == Hold.Hold)
                   {
                       return true;
                   }
                   else
                   {
                       return false;
                   }
               }
           }
           else
           {
               if (null == _ai)
               {
                   return false;
               }
               else
               {
                   if (_ai.HoldState == Hold.Hold)
                   {
                       return true;
                   }
                   else
                   {
                       return false;
                   }
               }
           }
        
        } 
    }

    public PlayerKinematics OwnerPlayer
    {
        get 
        {
            if (null != _pk)
            {
                return _pk;
            }
            return null;
        }
    }

    public AIKinematics OwnerAI
    {
        get
        {
            if (null != _ai)
            {
                return _ai;
            }
            return null;
        }
    }

    public int SelectedCharacterID
    {
        get
        {
            if (OwnerPlayer != null)
                return OwnerPlayer.ID;
            if (OwnerAI != null)
                return OwnerAI.ID;
            return -1;
        }
    }

    public void SendHold(int order, bool isai = false)
    {
        _isAI = isai;
        OnHold(order);
    }

    void OnEnable()
    {
        _trigger.GetComponent<BoxCollider>().enabled = true;
    }

    void OnHold(int order)
    {
        // 接手Player
        _state = State.run1;
        GetComponent<OpponentKinematics>().enabled      = false;
        GetComponent<TruckBehaviour>().enabled          = false;
        GetComponent<BoxCollider>().isTrigger           = true;
        _trigger.GetComponent<BoxCollider>().enabled    = false;
        if (!_isAI)
        {
            _pk = LobbyManager.Instance.GetPlayerByOrder(order);
            _pk.OnHold();
            if (_pk.isLocalPlayer)
            {
                Message message = new Message(MessageType.Message_Trigger_Tips, this);
                message["tips"] = (int)EnumTips.Hold;
                message["pos"] = transform.position;
                message.Send();
            }
        }
        else
        {
            _ai = GameMode.Instance.GetAIByUnique(order);
            _ai.OnHold();
        }
      
        SamplePoolManager.Instance.RequestEffect(Effect_Body_Point, Vector3.zero, SamplePoolManager.ObjectType.Effect_Hold_Body, true);
        SoundController.Instance.PlaySoundHold();
    }

    void FixedUpdate()
    {
        if (!_isAI)
        {
            if (null != _pk && _pk.HoldState == Hold.Hold)
            {
                GetComponent<BoxCollider>().enabled = true;
                if (_pk.isLocalPlayer)
                {
                    _rb.velocity = _pk.PlayerRigidbody.velocity;
                    Vector3 rotTran = new Vector3(_rb.velocity.x * _rotCoeff, 0.0f, 1.0f);
                    _rb.MoveRotation(Quaternion.LookRotation(rotTran));
                }
                else
                {
                    _rb.rotation = _pk.PlayerRigidbody.rotation;
                }

            }
            else if (null != _pk && _pk.HoldState == Hold.ExitHold)
            {
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Body_Point.gameObject);
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Left_Engine_Point.gameObject);
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Right_Engine_Point.gameObject);
                _pk = null;
                GetComponent<OpponentKinematics>().enabled = true;
                GetComponent<OpponentKinematics>().OnOpponentDestroyed(OpponentKinematics.ExplodType.Truck, SelectedCharacterID);
            }
        }
        else
        {
            if (null != _ai && _ai.HoldState == Hold.Hold)
            {
                GetComponent<BoxCollider>().enabled = true;
                _rb.velocity = _ai.PlayerRigidbody.velocity;
                Vector3 rotTran = new Vector3(_rb.velocity.x * _rotCoeff, 0.0f, 1.0f);
                _rb.MoveRotation(Quaternion.LookRotation(rotTran));
            }
            else if (null != _ai && _ai.HoldState == Hold.ExitHold)
            {
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Body_Point.gameObject);
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Left_Engine_Point.gameObject);
                SamplePoolManager.Instance.NotifyDestroyingEffect(Effect_Right_Engine_Point.gameObject);
                _ai = null;
                GetComponent<OpponentKinematics>().enabled = true;
                GetComponent<OpponentKinematics>().OnOpponentDestroyed(OpponentKinematics.ExplodType.Truck, SelectedCharacterID);
            }
        }
       
    }

    void Update()
    {
        if (!_isAI)
        {
            if (null != _pk && _pk.HoldState == Hold.Hold)
            {
                transform.position = new Vector3(_pk.PlayerRigidbody.position.x, 0, _pk.PlayerRigidbody.position.z);
            }
        }
        else
        {
            if (null != _ai && _ai.HoldState == Hold.Hold)
            {
                transform.position = new Vector3(_ai.PlayerRigidbody.position.x, 0, _ai.PlayerRigidbody.position.z);
            }
        }

        switch(_state)
        {
            case State.run:
                ChangeToRun();
                break;
            case State.run1:
                ChangeToRun1();
                break;
            case State.run2:
                ChangeToRun2();
                break;
        }
    }

    void ChangeToRun()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("run"))
        {
            _animator.SetInteger("State", (int)_state);
        }
    }

    void ChangeToRun1()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("run1"))
        {
            _animator.SetInteger("State", (int)_state);
        }else if (info.normalizedTime >= 1.0f)
        {
            _state = State.run2;
            SamplePoolManager.Instance.RequestEffect(Effect_Left_Engine_Point, Vector3.zero, SamplePoolManager.ObjectType.Effect_SpeedEngine_Red, true);
            SamplePoolManager.Instance.RequestEffect(Effect_Right_Engine_Point, Vector3.zero, SamplePoolManager.ObjectType.Effect_SpeedEngine_Red, true);
        }
    }

    void ChangeToRun2()
    {
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName("run2"))
        {
            _animator.SetInteger("State", (int)_state);
        }
    }
}
