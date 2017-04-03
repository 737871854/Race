/*
 * Copyright (c) 
 * 
 * 文件名称：   LobbyPlayer.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/18 13:34:34
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Need.Mx;
using System.Collections.Generic;

public class LobbyPlayer : NetworkLobbyPlayer
{
   
    [SyncVar]
    public int _playerIndex;//判断选择的是第几个角色
  
    [SyncVar(hook = "OnChangeOrder")]
    public int _order;//第几个进入游戏的角色，根据进入的先后顺序安排不同的赛道
    void OnChangeOrder(int value)
    {
        _order = value;
    }

    [SyncVar(hook = "OnReady")]
    public bool _isReady;
    void OnReady(bool value)
    {
        _isReady = value;
    }

    [SyncVar]
    private float _lifeValue;

    [SyncVar]
    private float _readyTime;

    [SyncVar]
    private bool _toMap;

    // 同步游戏难度
    private int _gameDifficulty;

    [Command]
    public void CmdTellServerIsReady()
    {
        _isReady = true;
    }

    [SyncVar(hook = "CanPlayCameraAnimation")]
    public bool cameraAnimation;
    void CanPlayCameraAnimation(bool value)
    {
        cameraAnimation = value;
    }

    [Command]
    public void CmdTellServerCameraAnimation()
    {
        cameraAnimation = true;
    }

    public bool _host = false;//玩家是否是主机的标志

    public int Order        { get { return _order;} }
    public int PlayerIndex  { get { return _playerIndex; } }
    public float LifeValue  { get { return _lifeValue; } }
    public float ReadyTime  { get { return _readyTime; } }
    public bool ToMap       { get { return _toMap; } }
    public int GameDifficulty { get { return _gameDifficulty; } }

    void Awake()
    {
        LobbyManager.Instance.LobbyList.Add(this);
    }

    void Start()
    {
        _playerIndex = -1;
        _gameDifficulty = -1;
    }

    //开始时，在本地执行的方法//
    public override void OnStartAuthority()
    {
        LobbyManager.Instance.LobbyPlayer   = this;
    }

    //当进入游戏大厅
    public override void OnClientEnterLobby()
    {
        transform.SetParent(LobbyManager.Instance.transform);
        // 第一个进入游戏大厅的是主机
        if (LobbyManager.Instance.transform.childCount == 1)
            _host = true;
        _order = LobbyManager.Instance.transform.childCount + GameMode.Instance.AIList.Count - 1;
    }
    
    // 当所有玩家都准备好了之后，由主机选择具体的游戏场景
    [ClientRpc]
    public void RpcOnAllReady(EnumSceneType type)
    {
        switch(type)
        {
            case EnumSceneType.Sea_Scene:
                LobbyManager.Instance.playScene = EnumSceneType.Sea_Scene.ToString();
                break;
            case EnumSceneType.Hallowmas_Scene:
                LobbyManager.Instance.playScene = EnumSceneType.Hallowmas_Scene.ToString();
                break;
            case EnumSceneType.Town_Scene:
                LobbyManager.Instance.playScene = EnumSceneType.Town_Scene.ToString();
                break;
        }
    }

    // 当所有玩家都加入后, 进入到选择游戏场景界面
    [ClientRpc]
    public void RpcOnAllPlayersJoin()
    {

    }

    [ClientRpc]
    public void RpcUpdateGameDifficulty(int value)
    {
        _gameDifficulty = value;
    }

    [ClientRpc]
    public void RpcUpdateLifeValue(float value)
    {
        _lifeValue = value;
    }

    [ClientRpc]
    public void RpcUpdateReadyValue(float value)
    {
        _readyTime = value;
    }

    [ClientRpc]
    public void RpcToMap(bool value)
    {
        _toMap = value;
    }
  
    /// <summary>
    ///  确认地图
    /// </summary>
    /// <param name="type"></param>
    [ClientRpc]
    public void RpcOnMapSure(EnvirType type)
    {
        EnvirManager.Instance.CurrentEnvirType = type;
        Message message = new Message(MessageType.Message_Map_Is_Choosed, this);
        message.Send();
        LobbyManager.Instance.ChangePlayScene(type.ToString());
    }

    [ClientRpc]
    public void RpcOnSceneEffect(Vector3 pos)
    {
        EnvirManager.Instance.AddEffectAtPositon(pos);
    }

    /// <summary>
    /// Boss来袭UI事件
    /// </summary>
    [ClientRpc]
    public void RpcOnBossCome()
    {
        Message message = new Message(MessageType.Message_Boss_Come, this);
        message.Send();
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    /// <param name="offset"></param>
    [ClientRpc]
    public void RpcOnSelectMap(float value)
    {
        Message message = new Message(MessageType.Message_Select_Map, this);
        message["value"] = value;
        message.Send();
    }

    [ClientRpc]
    public void RpcOnSelectCar(int id)
    {
        _playerIndex = id;
        LobbyManager.Instance.AddSelectedID(_playerIndex, _order);
        LobbyManager.Instance.ViewDic.Remove(_order);
    }

    [Command]
    public void CmdTellServerSelectedID(int id)
    {
        RpcOnSelectCar(id);
    }
}
