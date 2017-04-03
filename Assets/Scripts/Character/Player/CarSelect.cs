/*
 * Copyright (c) 
 * 
 * 文件名称：   CarSelect.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/14 8:58:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class CarSelect : MonoBehaviour
{
    public enum State
    {
        None = -1,
        idle,
        transformLeopard,
        transformCar,
    }

    public int _id;
    public State _state;
    public Animator _animator0;
    public Animator _animator1;
    public Material _material0;
    public Material _material1;
    public GameObject _effect;

    private float _waitingTimer = 2;
    private bool _reset;

    void Start()
    {
        _material0.shader = Shader.Find("Q5/Unity/Unlit/Texture Alpha Control");
        _material1.shader = Shader.Find("Q5/Unity/Unlit/Texture Alpha Control2");
        _material0.SetFloat("_Alpha", 1);
        _material1.SetFloat("_Alpha", 0);
    }

    void OnEnable()
    {
        _animator0.speed = 0;
        MessageCenter.Instance.AddListener(MessageType.Message_Car_Selected, OnCarSelect);
    }

    void  OnDisable()
    {
         MessageCenter.Instance.RemoveListener(MessageType.Message_Car_Selected, OnCarSelect);
    }

    private void OnCarSelect(Message message)
    {
        int selectId = (int)message["id"];
        if (_id == selectId)
        {
            _state = State.transformLeopard;
            SoundController.Instance.StartLeopardSelect();
        }
        else
        {
            _state          = State.idle;
            _reset          = false;
            _waitingTimer   = 2.0f;
            _animator0.gameObject.SetActive(true);
            _material0.SetFloat("_Alpha", 1);
            _material1.SetFloat("_Alpha", 0);
            _animator1.gameObject.SetActive(false);
            _animator0.Play("idle", -1, 0);
            _effect.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        switch (_state)
        {
            case State.None:
                _state = State.idle;
                break;
            case State.idle:
                 ChangeToRun();
                break;
            case State.transformLeopard:
                ChangeToLeopard();
                break;
            case State.transformCar:
                ChangeToCar();
                break;
        }

        if (_reset)
        {
            if (_waitingTimer > 0)
            {
                _waitingTimer -= Time.fixedDeltaTime;
            }
            else
            {
                _reset = false;
                _state = State.transformCar;
            }
        }
    }

    void ChangeToRun()
    {
        AnimatorStateInfo info = _animator0.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.idle.ToString()))
        {
            if (_animator0.gameObject.activeSelf)
            {
                _animator0.speed = 0;
                _animator0.SetInteger("State", (int)State.idle);
            }
            if (_animator1.gameObject.activeSelf)
            {
                _animator1.SetInteger("State", (int)State.idle);
            }
        }
    }

    void ChangeToLeopard()
    {
        _animator1.gameObject.SetActive(true);
        AnimatorStateInfo info = _animator1.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.transformLeopard.ToString()))
        {
            _animator0.speed = 1;
            _animator0.SetInteger("State", (int)_state);
            _animator1.SetInteger("State", (int)_state);
            _effect.gameObject.SetActive(true);
        }
        else
        {
            // 车由不透明边透明
            if (info.normalizedTime> 0.4f && info.normalizedTime <= 0.5f)
            {
                _material0.SetFloat("_Alpha", 10 * (0.5f - info.normalizedTime));
            }else if (info.normalizedTime > 0.5f)
            {
                _animator0.gameObject.SetActive(false);
            }

            // 豹子由透明边不透明
            if (info.normalizedTime > 0.188f && info.normalizedTime < 0.3056f)
            {
                _material1.SetFloat("_Alpha", 8.6f * (info.normalizedTime - 0.188f));
            }

            if (info.normalizedTime > 0.999f)
            {
                _state = State.idle;
                _waitingTimer = 2;
                _reset = true;
            }
        }
    }

    void ChangeToCar()
    {
        _animator0.gameObject.SetActive(true);
        AnimatorStateInfo info = _animator0.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(State.transformCar.ToString()))
        {
            _animator0.speed = 1;
            _animator0.SetInteger("State", (int)State.transformCar);
            _animator1.SetInteger("State", (int)State.transformCar);
        }
        else
        {
            if (info.normalizedTime > 0.4f && info.normalizedTime < 0.99f)
            {
                // 车由透明边不透明
                _material0.SetFloat("_Alpha", 1);
                //// 豹子由不透明变透明
                _material1.SetFloat("_Alpha", 1 - 3.0f * (info.normalizedTime - 0.1f));
            }
            else if (info.normalizedTime >= 0.99f)
            {
                _state = State.idle;
                _animator1.gameObject.SetActive(false);
            }

        }
    }
}
