/*
 * Copyright (c) 
 * 
 * 文件名称：   TruckBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/11 10:25:23
 * 
 * 修改描述：   起跳平台逻辑功能
 * 
 */

using System;
using UnityEngine;
using System.Collections;
using Need.Mx;
using UnityEngine.Networking;

public class TruckBehaviour : NetworkBehaviour
{
    #region private members
    private float _distanceCheck = 3.5f;
    private bool _jumpStarted;
    #endregion

    #region public members
    public TruckType _truckType;
    public GameObject _guide;
	#endregion

    #region public properties

    void OnEnable()
    {
        _syncShowGuided = false;
        _jumpStarted    = false;
    }
  
    public enum TruckType
    {
        None = -1,
        Normal,
        Turbo,
        Transform,
        Hold,
    }

    public TruckType TypeOfTruck { get { return _truckType; } }

    #endregion


    [SyncVar(hook="OnShowGuided")]
    private bool _syncShowGuided;
    void OnShowGuided(bool value)
    {
        if (_guide != null)
            _guide.SetActive(value);
    }

    [Command]
    public void CmdTellServerShowGuided()
    {
        _syncShowGuided = true;
    }

    bool sameLane;
    float distanceZ;
    float distanceX;
    bool canCheckForPlayer;
    void Update()
    {
        PlayerKinematics kin = GameMode.Instance.PlayerRef;

        if (null == kin)
            return;

        if (kin.transform.position.y > 10.0f)
            return;

        sameLane = kin.CurrentLane == gameObject.GetComponent<OpponentKinematics>().CurrentLane;
        if (sameLane && transform.position.y == 0)
        {
            distanceZ         = gameObject.transform.position.z - GameMode.Instance.PlayerRef.transform.position.z;
            distanceX         = Mathf.Abs(gameObject.transform.position.x - GameMode.Instance.PlayerRef.transform.position.x);
            canCheckForPlayer  = (distanceZ > 0 && distanceZ < _distanceCheck && !_jumpStarted) && (distanceX < 0.3f);

            if (canCheckForPlayer)
            {
                if (kin.IsOnlane)
                {
                    _jumpStarted                                = true;
                    gameObject.GetComponent<Collider>().enabled = false;
                    kin.OnTruckEnter(gameObject);
                }
            }
            else if (distanceZ < 0.0f)
            {
                _jumpStarted                                = false;
                gameObject.GetComponent<Collider>().enabled = true;
            }
        }

        for (int i = 0; i < GameMode.Instance.AIList.Count; ++i )
        {
            sameLane = GameMode.Instance.AIList[i].CurrentLane == gameObject.GetComponent<OpponentKinematics>().CurrentLane;
            if (sameLane && transform.position.y == 0)
            {
                distanceZ = gameObject.transform.position.z - GameMode.Instance.AIList[i].transform.position.z;
                distanceX = Mathf.Abs(gameObject.transform.position.x - GameMode.Instance.AIList[i].transform.position.x);
                canCheckForPlayer = (distanceZ > 0 && distanceZ < _distanceCheck && !_jumpStarted) && (distanceX < 0.3f);

                if (canCheckForPlayer)
                {
                    if (GameMode.Instance.AIList[i].IsOnlane)
                    {
                        _jumpStarted = true;
                        gameObject.GetComponent<Collider>().enabled = false;
                        GameMode.Instance.AIList[i].OnTruckEnter(gameObject);
                    }
                }
                else if (distanceZ < 0.0f)
                {
                    _jumpStarted = false;
                    gameObject.GetComponent<Collider>().enabled = true;
                }
            }
        }
    }
}
