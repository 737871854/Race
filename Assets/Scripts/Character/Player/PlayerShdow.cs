/*
 * Copyright (c) 
 * 
 * 文件名称：   PlayerShdow.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/30 8:43:52
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;

public class PlayerShdow : MonoBehaviour
{
    #region Public Member
    public GameObject _playerRef;
    public Vector3 offset_Town;
    public Vector3 offset_hallowmas;
    public Vector3 offset_dester;
    #endregion

    #region Private Member
    private Vector3 _baseOffset;
    private Vector3 _offSet;
    #endregion

    void Start()
    {
        _baseOffset = gameObject.transform.localPosition;
        switch(EnvirManager.Instance.CurrentEnvirType)
        {
            case Need.Mx.EnvirType.Town_Scene:
                _offSet = offset_Town;
                break;
            case Need.Mx.EnvirType.Hallowmas_Scene:
                _offSet = offset_hallowmas;
                break;
            case Need.Mx.EnvirType.Dester_Scene:
                _offSet = offset_dester;
                break;
        }
    }

    void Update()
    {
        transform.position              = new Vector3(_playerRef.transform.position.x, -0.1f, _playerRef.transform.position.z) + _offSet;
        gameObject.transform.rotation   = Quaternion.Euler(0.0f, _playerRef.transform.rotation.eulerAngles.y, 0.0f);
    }
}
