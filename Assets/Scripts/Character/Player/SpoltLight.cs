/*
 * Copyright (c) 
 * 
 * 文件名称：   SpoltLight.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/1/16 18:13:17
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class SpoltLight : MonoBehaviour
{
    public PlayerKinematics _playerRef;

    #region Private Member
    private Vector3 _baseOffset;
    private Vector3 _offSet;
    #endregion

    void Start()
    {
        _offSet     = new Vector3(0.0f, 0.01f, 0.0f);
        _baseOffset = gameObject.transform.localPosition;
        switch (EnvirManager.Instance.CurrentEnvirType)
        {
            case Need.Mx.EnvirType.Hallowmas_Scene:
                if (_playerRef.isLocalPlayer)
                    gameObject.SetActive(true);
                else
                    gameObject.SetActive(false);
                break;
            case Need.Mx.EnvirType.Town_Scene:
            case Need.Mx.EnvirType.Dester_Scene:
                gameObject.SetActive(false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(_playerRef.transform.position.x, -0.06f, _playerRef.transform.position.z) + _offSet;
        gameObject.transform.rotation = Quaternion.identity;
    }
}
