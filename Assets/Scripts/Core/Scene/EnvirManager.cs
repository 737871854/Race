/*
 * Copyright (c) 
 * 
 * 文件名称：   EnvirManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/8/5 15:40:00
 * 
 * 修改描述：   2016.8/12
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public class EnvirManager : SingletonBehaviour<EnvirManager>
{
    private EnvirType _type = EnvirType.None;
    private bool _isOverStreet = true;
    private int _index;
    private AddEnvirItem _envirItem;
    private float _Z;
    private float _halfLength;

    private float curDis;
    private float DV;

    #region Public Properties
    public EnvirType CurrentEnvirType { get { return _type; } set { _type = value; } }
    public List<Vector3> startPosList;
    public List<int> initLane;
    public AddEnvirItem EnvirItem { get { return _envirItem; } }
    #endregion

    public override void Awake()
    {
        base.Awake();
    }  

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    #region ---------------------------Update---------------------------------
    public void Init()
    {
        curDis = 0;
        _isOverStreet = true;
        _index = 0;
        _halfLength = 0;
        _envirItem = null;
        GameObject go = GameObject.FindGameObjectWithTag(GameTag.EnvirItemTag);
        _envirItem = go.GetComponent<AddEnvirItem>();

        // 计算两条赛道互相切换时的间隔
        _halfLength = 0.5f * (_envirItem.StreetLength[0] + _envirItem.StreetLength[1]);
        _Z = _envirItem.StreetLength[0] + _envirItem.StreetLength[1] * 0.5f;

        AddEffectAtPositon(new Vector3(0, 0, 16));
    }

    public void AddEffectAtPositon(Vector3 pos)
    {
        if (CurrentEnvirType == EnvirType.Hallowmas_Scene)
        {
            GameObject go = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.Scene_SpeedUp);
            go.transform.position = pos;
            go.transform.localScale = Vector3.one;
        }
    }

    public void UpdatePreFrame()
    {
        if (null == _envirItem)
        {
            return;
        }

        if (null != GameMode.Instance.ObservedPlayer)
        {
            if (GameMode.Instance.ObservedID == GameMode.Instance.PlayerRef.ID)
            {
                if (GameMode.Instance.ObservedPlayer.transform.position.z - 16 >= _Z)
                {
                    _envirItem.StreetList[_index % 2].transform.position = new Vector3(_envirItem.StreetList[_index % 2].transform.position.x, _envirItem.StreetList[_index % 2].transform.position.y, _envirItem.StreetList[_index % 2].transform.position.z + _halfLength * 2);
                    _Z += _halfLength;
                    ++_index;
                }
            }
            else
            {
                _envirItem.StreetList[0].transform.position = GameMode.Instance.ObservedPlayer.Envir0Pos;
                _envirItem.StreetList[1].transform.position = GameMode.Instance.ObservedPlayer.Envir1Pos;
            }
        }
    }
    #endregion
}
