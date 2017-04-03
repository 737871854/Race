/*
 * Copyright (c) 
 * 
 * 文件名称：   FollowCamera.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/22 9:29:28
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class FollowCamera : MonoBehaviour
{
    public enum State
    {
        None = -1,
        Begine,
        Follow,
        End,
    }

    public AnimationCurve x_Curve;
    public AnimationCurve y_Curve;
    public AnimationCurve z_Curve;


    private bool _shake;
    private float _colliderTime;
    private float _fixedDis = 5;
    private float _verifyValue;
    private float _shakeTime;
    private float _toalTime;
    private float _intensity = 1;
    private bool _landmine;

    private float _detalZ;
    private float _offset;
    private bool _hold;
    private Vector3 _normalRotate   = new Vector3(12, 0, 0);
    private Vector3 _holdRotate     = new Vector3(31, 0, 0);
    private Vector3 _holdHeight     = new Vector3(0, 6f, 0);
    private Vector3 _normalHeight   = new Vector3(0, 2.5f, 0);

    public State state = State.Begine;

    void OnEnable()
    {
        MessageCenter.Instance.AddListener(MessageType.Message_Game_End, OnEnd);
        MessageCenter.Instance.AddListener(MessageType.Message_Camera_Shake, OnShake);
    }

    void OnDisable()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Game_End, OnEnd);
        MessageCenter.Instance.AddListener(MessageType.Message_Camera_Shake, OnShake);
    }

    // 游戏开始
    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(new Vector3(0, 2.5f, 11), 2));
        sequence.Join(transform.DOLocalRotate(new Vector3(12, 0, 0), 2));
        sequence.OnComplete(OnBegineEnd);
    }

    private void OnBegineEnd()
    {
        state = State.Follow;
    }

    private void OnEnd(Message message)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOMoveY(5, 2));
        sequence.Join(transform.DORotate(new Vector3(35, 0, 0), 2));
    }

    private void OnShake(Message message)
    {
        _shake      = true;
        _shakeTime  = 0;
        _toalTime   = (float)message["time"];
        _intensity  = (float)message["intensity"];
        _landmine   = (bool)message["landmine"];
        if (null != message["opp"])
            _colliderTime = 1;
    }

    void Update()
    {
        if (_shake)
        {
            if (_shakeTime >= _toalTime)
            {
                _shake                  = false;
                _shakeTime              = 0;
                transform.position      = new Vector3(0, transform.position.y, transform.position.z);
                return;
            }
            transform.position = new Vector3(transform.position.x + x_Curve.Evaluate(_shakeTime) * _intensity * (_landmine == true ? 0 : 1), transform.position.y + y_Curve.Evaluate(_shakeTime) * _intensity, transform.position.z + z_Curve.Evaluate(_shakeTime) * _intensity * (_landmine == true ? 0 : 1));
            _shakeTime                  += Time.deltaTime;
        }

        if (_colliderTime > 0)
        {
            _colliderTime -= Time.deltaTime * 2;
            _verifyValue -= Time.deltaTime * 2;
        }
        else if (_verifyValue < 0)
            _verifyValue += Time.deltaTime;

    }
    
    void LateUpdate()
    {
        if (null == GameMode.Instance.PlayerRef)
            return;

        if ( null == GameMode.Instance.ObservedPlayer)
        {
            GameMode.Instance.ObservedRigidbody();
            return;
        }

        if (state == State.Follow)
        {
            _offset = _fixedDis - Mathf.Clamp(GameMode.Instance.ObservedPlayer.PlayerRigidbody.velocity.z * 0.11f, 0, 5) + _verifyValue;
           
            _detalZ = Mathf.Clamp(GameMode.Instance.ObservedPlayer.PlayerRigidbody.velocity.z * 0.1f + _offset, 0, 15);

            transform.position = new Vector3(transform.position.x, transform.position.y, GameMode.Instance.ObservedPlayer.PlayerRigidbody.position.z - _detalZ);

            if (GameMode.Instance.PlayerRef.HoldState == Hold.Hold)
            {
                if (Vector3.Distance(transform.transform.localEulerAngles, _holdRotate) > 0.1f)
                    transform.transform.localEulerAngles    += _normalRotate * Time.fixedDeltaTime;
                if (transform.position.y < _holdHeight.y)
                    transform.position                      += _normalHeight * Time.fixedDeltaTime;
            }
            else
            {
                if (Vector3.Distance(transform.transform.localEulerAngles, _normalRotate) > 0.1f)
                    transform.transform.localEulerAngles    -= _normalRotate * Time.fixedDeltaTime;
                if (transform.position.y > _normalHeight.y)
                    transform.position                      -= _normalHeight * Time.fixedDeltaTime;
            }
        }
    }
}
