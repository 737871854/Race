/*
 * Copyright (c) 
 * 
 * 文件名称：   LobbyManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/18 13:33:35
 * 
 * 修改描述：   大厅管理
 * 
 */


using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using Need.Mx;

public class LobbyManager : SingletonNetworkLobbyManager<LobbyManager> ,IComparer<PlayerKinematics>
{
    private delegate void QuitDelegate();
    private QuitDelegate QuitGame;
    private bool isStop;
    #region private members
    /// <summary>
    /// 本地大厅玩家
    /// </summary>
    private LobbyPlayer _lobbyPlayer;
    /// <summary>
    /// 本地是否为主机
    /// </summary>
    public bool _isHost;

    /// <summary>
    /// 玩家列表
    /// </summary>
    private List<PlayerKinematics> _pkList = new List<PlayerKinematics>();

    private List<LobbyPlayer> _lobbyPlayerList = new List<LobbyPlayer>();
    /// <summary>
    /// 保存已选车辆id
    /// </summary>
    private List<int> _selectedList = new List<int>();
    // order, selectedID
    private Dictionary<int, int> _viewDic = new Dictionary<int, int>();

    #endregion

    #region public members
    /// <summary>
    /// 存放所有玩家列表
    /// </summary>
    public List<GameObject> _playerList;
    #endregion

    #region public properties
    public LobbyPlayer LobbyPlayer          { get { return _lobbyPlayer; } set { _lobbyPlayer = value; } }
    public List<PlayerKinematics> PKList    { get { return _pkList; } }
    public bool IsStop                      { get { return isStop; } }
    public int PlayerCount                  { get { return _pkList.Count; } }
    public List<int> SelectedList           { get { return _selectedList; } }
    public int LobbyCount                   { get { return _lobbyPlayerList.Count; } }
    public List<LobbyPlayer> LobbyList      { get { return _lobbyPlayerList; } }
    public Dictionary<int, int> ViewDic     { get { return _viewDic; } }

    #endregion

    #region unity callback

    void OnApplicationQuit()
    {
        SocketManager.Instance.CloseConnectThread();
    }
    #endregion

    #region public function
    public void Reset()
    {
        _isHost     = false;
        isStop      = false;
        _lobbyPlayerList.Clear();
        _selectedList.Clear();
        //_viewList.Clear();
        _pkList.Clear();
        _lobbyPlayer                            = null;
        SocketManager.Instance.HostMesg         = null;
        SocketManager.Instance.LocalMesg.Host   = false;
    }

    /// <summary>
    /// 多人游戏
    /// </summary>
    public void OnMulityGame()
    {
        if (null == SocketManager.Instance.HostMesg)
        {
            SocketManager.Instance.LocalMesg.Host   = true;
            SocketManager.Instance.SendHostIPMesg();
            _isHost         = true;
            minPlayers      = 2;
            maxPlayers      = GameConfig.GAME_CONFIG_MULTI_PLAYER_COUNT;
            networkAddress  = SocketManager.Instance.LocalMesg.IP;
            StartHost();
        }
        else
        {
            _isHost             = false;
            networkAddress      = SocketManager.Instance.HostMesg.IP;
            minPlayers          = 2;
            maxPlayers          = GameConfig.GAME_CONFIG_MULTI_PLAYER_COUNT;
            StartClient();
        }
        SocketManager.Instance.HostMesg = null;
    }

    public void AddSelectedID(int id, int order)
    {
        _selectedList.Add(id);
        //_selectedOrder.Add(order);
    }

    //public void RefreshViewList()
    //{
    //    _viewList.Clear();
    //    for (int i = 0; i < LobbyManager.Instance._lobbyPlayerList.Count; ++i)
    //    {
    //        if (!_viewList.Contains(LobbyManager.Instance._lobbyPlayerList[i]._viewID))
    //            _viewList.Add(LobbyManager.Instance._lobbyPlayerList[i]._viewID);
    //    }

    //    _idOrdersDic.Clear();
    //    for (int i = 0; i < LobbyManager.Instance._lobbyPlayerList.Count; ++i )
    //    {
    //        if (!_idOrdersDic.ContainsKey(LobbyManager.Instance._lobbyPlayerList[i]._viewID))
    //        {
    //            _idOrdersDic.Add(LobbyManager.Instance._lobbyPlayerList[i]._viewID, new List<int>());
    //            _idOrdersDic[LobbyManager.Instance._lobbyPlayerList[i]._viewID].Add(LobbyManager.Instance._lobbyPlayerList[i].Order);
    //        }
    //        else
    //        {
    //            _idOrdersDic[LobbyManager.Instance._lobbyPlayerList[i]._viewID].Add(LobbyManager.Instance._lobbyPlayerList[i].Order);
    //        }
    //    }

    //    Sort();
    //}

    //private void Sort()
    //{
    //    foreach(KeyValuePair<int, List<int>> kvp in _idOrdersDic)
    //    {
    //        if (kvp.Value.Count > 1)
    //        {
    //            List<int> temp = kvp.Value.Clone();
    //            temp.Sort(CompareOrder);
    //            _idOrdersDic[kvp.Key] = temp.Clone();
    //        }
    //    }
    //}

    /// <summary>
    /// 获取加入联机人数
    /// </summary>
    /// <returns></returns>
    public int JoinPlayerNum()
    {
        int num = 0;
        for (int i = 0; i < maxPlayers; ++i)
        {
            if (lobbySlots.Length - 1 >= i && null != lobbySlots[i])
                ++num;
        }
        return num;
    }

    public void TestStartHost()
    {
        networkAddress = "192.168.1.210";
        StartHost();
    }

    public void TestStartClient()
    {
        networkAddress = SocketManager.Instance.LocalMesg.IP;
        minPlayers = 2;
        maxPlayers = GameConfig.GAME_CONFIG_MULTI_PLAYER_COUNT;
        StartClient();
    }

    /// <summary>
    /// 单人游戏
    /// </summary>
    public void OnSingleGame()
    {
        networkAddress  = SocketManager.Instance.LocalMesg.IP;
        minPlayers      = 1;
        maxPlayers      = 1;
        StartHost();
    }

    /// <summary>
    /// Ready
    /// </summary>
    public void Ready()
    {
        LobbyPlayer.SendReadyToBeginMessage();
    }

    /// <summary>
    /// 重新设置最大玩家数量
    /// </summary>
    /// <param name="value"></param>
    public void SetMaxPlayerCount()
    {
        maxPlayers = CurrentPlayerCount();
    }

    /// <summary>
    /// 当前设备作为主机
    /// </summary>
    /// <returns></returns>
    public bool LocalIsHost()
    {
        return _isHost;
    }

    /// <summary>
    /// 返回当前联机人数
    /// </summary>
    /// <returns></returns>
    private int CurrentPlayerCount()
    {
        for (int i = 0; i < maxPlayers; ++i)
        {
            if (lobbySlots.Length - 1 >= i && null == lobbySlots[i])
                return i + 1;
        }
        
        return 0;
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void StopGame()
    {
        if (null != QuitGame)
        {
            QuitGame();
            QuitGame = null;
            SocketManager.Instance.LocalMesg = null;
        }
    }

    /// <summary>
    /// 是否满足游戏连接开始需求的最小人数
    /// </summary>
    /// <returns></returns>
    public bool CanBegineGame()
    {
        for (int i = 0; i < minPlayers; ++i )
        {
            if (lobbySlots.Length - 1 >= i && null != lobbySlots[i] && i >= minPlayers - 1)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否满足最大人数
    /// </summary>
    /// <returns></returns>
    public bool PlayerCountEquals()
    {
        for (int i = 0; i < maxPlayers; ++i)
        {
            if (lobbySlots.Length - 1 >= i && null != lobbySlots[i] && i == maxPlayers - 1)
                return true;
        }
        return false;
    }

    ///// <summary>
    ///// 设置本地玩家所选车辆id
    ///// </summary>
    ///// <param name="index"></param>
    //public void SetupLocalPlayer(int index)
    //{
    //    LobbyPlayer.SetupLocalPlayer(index);
    //}

    /// <summary>
    /// 修改主场景
    /// </summary>
    /// <param name="name"></param>
    public void ChangePlayScene(string name)
    {
        playScene = name;
    }

    /// <summary>
    /// 返回大厅
    /// </summary>
    public void GoToLobbyScene()
    {
        ServerChangeScene(lobbyScene);
        isStop = true;
        if (_isHost)
            HostQuit();
        else
            ClientQuit();
    }

    public void AddPlayer(PlayerKinematics player)
    {
        _pkList.Add(player);
    }

    /// <summary>
    ///  依据玩家id标示获取玩家
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public PlayerKinematics GetPlayerByOrder(int order)
    {
        for (int i = 0; i < _pkList.Count; ++i )
        {
            if (_pkList[i].UniqueOrder == order)
                return _pkList[i];
        }
        return null;
    }

    public PlayerKinematics GetPlayerByID(int id)
    {
        for (int i = 0; i < _pkList.Count; ++i )
        {
            if (_pkList[i].ID == id)
                return _pkList[i];
        }

        return null;
    }

    /// <summary>
    /// 取得领先的玩家
    /// </summary>
    /// <returns></returns>
    public PlayerKinematics GetForwardPlayer()
    {
        if (_pkList.Count == 1)
            return _pkList[0]; 

        PlayerKinematics pk = _pkList[0];
        for (int i = 1; i < _pkList.Count; ++i)
        {
           if (pk.PlayerRigidbody.position.z < _pkList[i].PlayerRigidbody.position.z)
               pk = _pkList[i];
        }

        return pk;
    }

    /// <summary>
    /// 获取实时排名
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public int GetRankByDistance(int rank)
    {
        if (GameMode.Instance.Mode == RunMode.Racing)
            _pkList.Sort(Compare);

        for (int i = 0; i < _pkList.Count; ++i )
        {
            if (_pkList[i].UniqueOrder == rank)
                return i;
        }

        return -1;
    }

    public void UpdateLifeValue(float value)
    {
        for (int i = 0; i < LobbyList.Count; ++i )
        {
            LobbyList[i].RpcUpdateLifeValue(value);
        }
    }

    public void UpdateReadyValue(float value)
    {
        for (int i = 0; i < LobbyList.Count; ++i)
        {
            LobbyList[i].RpcUpdateReadyValue(value);
        }
    }

    public void UpdateToMap(bool value)
    {
        for (int i = 0; i < LobbyList.Count; ++i)
        {
            LobbyList[i].RpcToMap(value);
        }
    }

    public void UpdateGameDifficulty(int value)
    {
        for (int i = 0; i < LobbyList.Count; ++i )
        {
            LobbyList[i].RpcUpdateGameDifficulty(value);
        }
    }

    //public void UpdateViewDic()
    //{
    //    ViewDic.Clear();
    //    for (int i = 0; i < LobbyList.Count; ++i )
    //    {
    //        _viewDic.Add(LobbyList[i].Order, LobbyList[i].ViewID);
    //    }
    //}

    /// <summary>
    /// 依据玩家排名获取玩家对应的图像ID
    /// </summary>
    /// <returns></returns>
    public int GetCharacterIDByRank(int value)
    {
        PlayerKinematics pk = _pkList[value];
        int uid = pk.UniqueOrder;
        return GetCharacterIdByUID(uid);
    }

    /// <summary>
    /// 依据玩家唯一ID获取选取角色ID
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public int GetCharacterIdByUID(int uid)
    {
        for (int i = 0; i < _pkList.Count; ++i)
        {
            if (uid == _pkList[i].UniqueOrder)
                return _pkList[i].ID;
        }
        return -1;
    }

    /// <summary>
    /// 取得最后的玩家
    /// </summary>
    /// <returns></returns>
    public PlayerKinematics GetLastPlayer()
    {
        if (_pkList.Count == 1)
            return _pkList[0];

        PlayerKinematics pk = _pkList[0];
        for (int i = 1; i < _pkList.Count; ++i)
        {
            if (pk.PlayerRigidbody.position.z > _pkList[i].PlayerRigidbody.position.z)
                pk = _pkList[i];
        }

        return pk;
    }

    /// <summary>
    /// 所有玩家是否进入就位
    /// </summary>
    /// <returns></returns>
    public bool AllPlayerIsReady()
    {
        for (int i = 0; i < _lobbyPlayerList.Count; ++i)
        {
            if (!_lobbyPlayerList[i]._isReady)
                return false;
        }
        return true;
    }

    #region override network
    /// <summary>
    /// 游戏从大厅场景切换到主场景时，调用
    /// </summary>
    /// <param name="lobbyPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns></returns>
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        gamePlayer.GetComponent<PlayerKinematics>().ID = lobbyPlayer.GetComponent<LobbyPlayer>()._playerIndex;
        gamePlayer.GetComponent<PlayerKinematics>().UniqueOrder = lobbyPlayer.GetComponent<LobbyPlayer>()._order;
        return true;
    }

    /// <summary>
    /// 服务器大厅创建一个游戏玩家
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    /// <returns></returns>
    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        if (null == gamePlayerPrefab)
        {
            Debuger.Log("The gamePlayerPreafab is empty on the LobbyManager");
            return null;
        }

        if (null == gamePlayerPrefab.GetComponent<NetworkIdentity>())
        {
            Debuger.Log("The gamePlayerPrefab does not have a NetWorkIdentity");
            return null;
        }

        if (null == _playerList)
        {
            Debuger.Log("_playerList is null");
            return null;
        }

        GameObject player       = null;
        LobbyPlayer lobbyPlayer = lobbySlots[conn.connectionId] as LobbyPlayer;
        player                  = GameObject.Instantiate(_playerList[lobbyPlayer.PlayerIndex], Vector3.zero, Quaternion.identity) as GameObject;
        player.tag              = GameTag.PlayerTag;
        return player;
    }

    /// <summary>
    /// 客服端连接
    /// </summary>
    /// <param name="conn"></param>
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        if (!NetworkServer.active)
            QuitGame = ClientQuit;
    }

    /// <summary>
    /// 启动服务端
    /// </summary>
    public override void OnStartHost()
    {
        base.OnStartHost();
        QuitGame = HostQuit;
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        base.OnLobbyClientDisconnect(conn);
    }

    ///// <summary>
    ///// 客户端丢失连接
    ///// </summary>
    ///// <param name="conn"></param>
    //public override void OnClientDisconnect(NetworkConnection conn)
    //{
    //    base.OnClientDisconnect(conn);
    //    LobbyPlayer lobbyPlayer = lobbySlots[conn.connectionId] as LobbyPlayer;
    //    _lobbyPlayerList.Remove(LobbyPlayer);
    //    int removeIndex = 0;
    //    for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i )
    //    {
    //        if (LobbyManager.Instance.PKList[i].ID == lobbyPlayer._playerIndex)
    //            removeIndex = i;
    //    }

    //    LobbyManager.Instance.PKList.RemoveAt(removeIndex);
    //}

    /// <summary>
    /// 进游戏时，检测各个玩家是否都已经准备好，准备好之后直接开始游戏
    /// </summary>
    /// <param name="conn"></param>
    public override void OnLobbyServerPlayersReady()
    {
        for (int i = 0; i < lobbySlots.Length; ++i )
        {
            if (lobbySlots.Length - 1 >= i && null != lobbySlots[i]  && !lobbySlots[i].readyToBegin)
                return;
        }
        
        ServerChangeScene(playScene);
    }

    #endregion

    #endregion

    #region Private Function
    /// <summary>
    /// 客户端退出
    /// </summary>
    private void ClientQuit()
    {
        StopClient();
    }


    /// <summary>
    /// 主机退出
    /// </summary>
    private void HostQuit()
    {
        StopHost();
        SocketManager.Instance.LocalMesg = new IPHostMessage();
    }
    #endregion


    public int Compare(PlayerKinematics x, PlayerKinematics y)
    {
        if (null == x || null == y)
            return -1;

        return x.Score.CompareTo(y.Score) * -1;
    }
   
}
