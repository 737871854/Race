/*
 * Copyright (c) 
 * 
 * 文件名称：   GameMode.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/13 9:14:46
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public partial class GameMode : SingletonBehaviour<GameMode>, IComparer<AIKinematics>
{
    public enum InputType
    {
        External = 0,
        Builtin = 1,
    } 

    public enum SelectMode
    {
        SinglePlayer = 0,
        Multiplayer,
    }

    public enum Step
    {
        None = -1,
        Step0,
        Step1,
        Step2,
        Step3,
    }

    public class Data
    {
        private int coin;

        public int Coin            
        { 
            get { return coin; }
            set 
            { 
                coin = value;
                GameMode.Instance.SendCoinMsg();
                GameMode.Instance.WattingTime = GameConfig.GAME_CONFIG_WAITTING_TIME;
            } 
        }
    }

    public static bool _oldSystemActive = false;

    #region Private Members
    private Data _data                          = new Data();
    private InputType _inputType;                                                   // 游戏操作方法
    private SelectMode _selectMode;                                                 // 悬着游戏单人模式或联机模式
    private GameObject _environmentSceneGo;                                         // 当前场景特性信息挂在根节点
    private string _enviScenename;                                                  // 当前场景名字
    private RunMode _runMode;                                                       // 游戏状态
    private Transform _root;                                                        // 根节点
    private bool _readGo;
    private Step _step                          = Step.Step0;
    // 已经扣币 防止多次扣币

    private int _chooseCharacterID;

    // 猫头鹰和狼嚎
    private float _randWolf                     = 40;
    private float _randWol                      = 50;
    private AirPlaneBehaviour _airPlane;
    private List<AIKinematics> _AIList          = new List<AIKinematics>();
    private Dictionary<int, int> _idDisDic      = new Dictionary<int, int>();       // 存放id, distance
    private Dictionary<int, int> _idScoreDic    = new Dictionary<int, int>();       // 存放id, score
    private Dictionary<int, int> _orderIdDic    = new Dictionary<int, int>();       // 存放order，id

    List<KeyValuePair<int, int>> lst            = new List<KeyValuePair<int, int>>();
    private List<int> _DistanceOrderList        = new List<int>();                          // 存放玩家id, 依据行驶距离从大到小
    private List<int> _ScoreOrderList           = new List<int>();                          // 存放玩家id，依据得分从高到低

    private float _continueTime                 = 10;
    private float _remainTime                   = 80;
    private float _totalTime                    = 80;
    private int _observedID;
    private PlayerKinematics _observedPlayer;
    private bool _startFire;

    private int _movieID;
    private float _waittingTime = 30;

    #endregion

    #region Public Members
    #endregion

    #region Public Properties
    public GameObject EnviSceneGO       { get { return GameObject.Find("Root_" + _enviScenename).gameObject; } }
    public string EnviSceneName         { set { _enviScenename = value; } }
    public RunMode Mode                 { get { return _runMode; } }
    public SelectMode ModeSelected      { get { return _selectMode; } set { _selectMode = value; } }
    public Transform Root               { get { return transform; } }
    public PlayerKinematics PlayerRef   { get { return _player; } set { _player = value; } }
    public Data ModeData                { get { return _data; } }
    public int ChooseCharacterID        { get { return _chooseCharacterID; } set { _chooseCharacterID = value; } }
    public AirPlaneBehaviour Airplane   { get { if (null != _airPlane) return _airPlane.GetComponent<AirPlaneBehaviour>(); return null; } set { _airPlane = value; } }
    public List<AIKinematics> AIList    { get {return _AIList;}}
    public List<int> DistanceOrderList  { get { return _DistanceOrderList; } }
    public List<int> ScoreOrderList     { get { return _ScoreOrderList; } }
    public bool ReadyGo                 { get { return _readGo; } set { _readGo = value; } }
    public Dictionary<int, int> IDDISDic   { get { return _idDisDic; } }
    public Dictionary<int, int> IDScoreDic { get { return _idScoreDic; } }
    public Step StepStage                  { get { return _step; } set { _step = value; } }
    public float ContinueTime              { get { return _continueTime; } }
    public float RemainTime                { get { return _remainTime; } }
    public float TotalTime                 { get { return _totalTime; } }
    public int ObservedID                  { get { return _observedID; }}
    public PlayerKinematics ObservedPlayer { get { return _observedPlayer; }}
    public float WattingTime               { set { _waittingTime = value; } }
    public int MovieID                     { get { return _movieID; } }
    #endregion


    #region Public Function
    public void GameInputType (InputType type)
    {
        _inputType = type;
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    public void ModeStart()
    {
        _startFire      = false;
        _readGo         = false;
        _player         = null;
        _step           = Step.Step0;
        _observedID     = -1;
        _observedPlayer = null;
        _waittingTime   = GameConfig.GAME_CONFIG_WAITTING_TIME;
        _remainTime     = GameConfig.GAME_CONFIG_STEP_TIME[0];
        _totalTime      = GameConfig.GAME_CONFIG_STEP_TIME[0];
        _selectMode     = SelectMode.SinglePlayer;
        ClearAIList();
        UIManager.Instance.CloseUIAll();
        LobbyManager.Instance.StopGame();
        LobbyManager.Instance.Reset();
        GameMode.Instance.ChangeMode(RunMode.Waiting);
    }

    public bool CanPlay()
    {
        return _data.Coin >= GameConfig.GAME_CONFIG_PER_USE_COIN;
    }

    /// <summary>
    /// 是否可以开始游戏
    /// </summary>
    public void ChangePlay()
    {
        if (Mode == RunMode.Waiting || Mode == RunMode.Movie)
            ChangeMode(RunMode.Select);
        if (Mode == RunMode.Continue)
            ChangeMode(RunMode.Racing);           
        _data.Coin -= GameConfig.GAME_CONFIG_PER_USE_COIN;
        SettingManager.Instance.LogCoins(GameConfig.GAME_CONFIG_PER_USE_COIN);
        SettingManager.Instance.Save();
    }

    public void ClearAIList()
    {
        _AIList.Clear();
        _orderIdDic.Clear();
    }

    /// <summary>
    /// 添加ＮＰＣ列表
    /// </summary>
    /// <param name="ai"></param>
    public void AddAiToList(AIKinematics ai)
    {
        _AIList.Add(ai);
    }

    public void NexStepState()
    {
        if (StepStage == Step.Step3)
            return;

        float addTime = 0;
        if (LobbyManager.Instance.LobbyPlayer.LifeValue > GameConfig.GAME_CONFIG_CHECK_TIME[(int)StepStage] + _remainTime)
            addTime = GameConfig.GAME_CONFIG_CHECK_TIME[(int)StepStage];
        else
            addTime = LobbyManager.Instance.LobbyPlayer.LifeValue - _remainTime;

        Message message     = new Message(MessageType.Message_Add_Time, this);
        message["index"]    = (int)StepStage;
        message.Send();

        _remainTime += addTime;
        _totalTime  += addTime;

        switch (StepStage)
        {
            case Step.Step0:
                StepStage = Step.Step1;
                break;
            case Step.Step1:
                StepStage = Step.Step2;
                break;
            case Step.Step2:
                StepStage = Step.Step3;
                break;
        }

        SoundController.Instance.PlaySoundCheckPoint();
    }

    /// <summary>
    /// 第一名是是否是玩家（false为NPC）
    /// </summary>
    /// <returns></returns>
    public bool TheFirstIsCurrentPlayer()
    {
        if (_DistanceOrderList[0] != GameMode.Instance.PlayerRef.ID)
            return false;

        return true;
    }

    /// <summary>
    /// 获取玩家数量
    /// </summary>
    /// <returns></returns>
    public int PlayerCount()
    {
        return AIList.Count + LobbyManager.Instance.PKList.Count;
    }

    /// <summary>
    /// 通过玩家id，获取玩家当前位置
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Vector3 GetPosByID(int id)
    {
        if (_selectMode == SelectMode.SinglePlayer)
        {
            if (id == _player.ID)
                return _player.transform.position;
            else
            {
                for (int i = 0; i < _AIList.Count; ++i)
                {
                    if (_AIList[i].ID == id)
                        return _AIList[i].transform.position;
                }
            }
        }
        else
        {
            for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
            {
                if (LobbyManager.Instance.PKList[i].ID == id)
                    return LobbyManager.Instance.PKList[i].transform.position;
            }
        }
        return Vector3.zero;
    }

    public void OpenStartFireByOrder(int order)
    {
        if (_selectMode == SelectMode.SinglePlayer)
        {
            if (order == _player.UniqueOrder)
                _player.ActivateStartFire();
            else
            {
                for (int i = 0; i < _AIList.Count; ++i)
                {
                    if (_AIList[i].UniqueOrder == order)
                        _AIList[i].ActivateStartFire();
                }
            }
        }
        else
        {
            for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
            {
                if (LobbyManager.Instance.PKList[i].UniqueOrder == order)
                    LobbyManager.Instance.PKList[i].ActivateStartFire();
            }
        }
    }

    /// <summary>
    /// 获取车辆对象通过ID
    /// </summary>
    /// <returns></returns>
    public GameObject GetObjBySelectedID(int id)
    {
        for (int i = 0; i < AIList.Count; ++i)
        {
            if (id == AIList[i].ID)
                return AIList[i].gameObject;
        }

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (id == LobbyManager.Instance.PKList[i].ID)
                return LobbyManager.Instance.PKList[i].gameObject;
        }

        return null;
    }

    /// <summary>
    /// 依据玩家ID给，给玩家积分
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    public void AddScore(int id, int value)
    {
        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (id == LobbyManager.Instance.PKList[i].ID && LobbyManager.Instance.PKList[i].isLocalPlayer)
            {
                Message message = new Message(MessageType.Message_Show_Score, this);
                message["score"] = value;
                message.Send();
                return;
            }
        }

        for (int i = 0; i < AIList.Count; ++i)
        {
            if (id == AIList[i].ID)
            {
                AIList[i].AddScore(value);
                return;
            }
        }
    }


    /// <summary>
    /// 依据玩家ID，获取玩家积分
    /// </summary>
    /// <param name="id"></param>
    public int GetScoreByID(int id)
    {
        if (_idScoreDic.ContainsKey(id))
            return _idScoreDic[id];

        Debuger.Log(id + "Not find");
        return 0;
    }

    /// <summary>
    /// 通过ID获取行驶距离
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetDisByID(int id)
    {
        if (_idDisDic.ContainsKey(id))
            return _idDisDic[id];

        Debuger.Log(id + "Not find");
        return 0;
    }

    /// <summary>
    ///  通过排名获取角色id
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public int GetIDByRank(int rank)
    {
        return _ScoreOrderList[rank];
    }

    /// <summary>
    /// 通过ID查询分数排名
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetRankByID(int id)
    {
        for (int i = 0; i < _ScoreOrderList.Count; ++i )
        {
            if (_ScoreOrderList[i] == id)
                return i;
        }

        return 0;
    }

    /// <summary>
    /// 通过排序入场排序获取角色id
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public int GetIDByOrder(int order)
    {
        if (_orderIdDic.ContainsKey(order))
            return _orderIdDic[order];

        return -1;
    }

    public void AddIDAndOrderToDic(int order, int id)
    {
        if (_orderIdDic.ContainsKey(order))
            return;

        _orderIdDic.Add(order, id);
    }


    /// <summary>
    /// 通过得分排名获取ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetIDByScoreOrder(int id)
    {
        if (ScoreOrderList.Count <= id)
            return -1;

        return ScoreOrderList[id];
    }

    /// <summary>
    /// 通过排名获取分数
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    public int GetScoreByRank(int rank)
    {
        int id = GetIDByRank(rank);

        return GetScoreByID(id);
    }


    /// <summary>
    /// 获取任意玩家同本地玩家距离的比较值
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    public float CompareDisWithLocal(int id, out Vector3 pos)
    {
        pos = GetPosByID(id);
        return pos.z - _player.transform.position.z;
    }

    /// <summary>
    /// 本地玩家
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsLocalPlayer(int id)
    {
        return id == _player.ID;
    }

    /// <summary>
    /// 最后一名是玩家
    /// </summary>
    /// <returns></returns>
    public bool TheLastIsPlayer()
    {
        if (_DistanceOrderList[_DistanceOrderList.Count - 1] != GameMode.Instance.PlayerRef.ID)
            return false;
        return true;
    }

    /// <summary>
    /// 获取行驶在最前面车辆的行驶路程
    /// </summary>
    /// <returns></returns>
    public float GetForwardDistance()
    {
        int id = DistanceOrderList[0];
        for (int i = 0; i < AIList.Count; ++i)
        {
            if (id == AIList[i].ID)
                return AIList[i].Distance;
        }

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (id == LobbyManager.Instance.PKList[i].ID)
                return LobbyManager.Instance.PKList[i].Distance;
        }

        return 0;
    }

    public Transform GetForwardTransform()
    {
        int id = DistanceOrderList[0];
         for (int i = 0; i < AIList.Count; ++i)
        {
            if (id == AIList[i].ID)
                return AIList[i].transform;
        }

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (id == LobbyManager.Instance.PKList[i].ID)
                return LobbyManager.Instance.PKList[i].transform;
        }

        return null;
    }

    /// <summary>
    /// 太靠后,排除已经Dead的车辆
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool BackforwardDestroy(Vector3 pos, float checkDis, bool isTraffic = true)
    {
        int id = DistanceOrderList[DistanceOrderList.Count - 1];
        if (IsAI(id))
            return GetObjBySelectedID(id).transform.position.z - pos.z > checkDis;

        PlayerKinematics pk = null;
        pk = LobbyManager.Instance.GetPlayerByID(id);
        if (!pk.IsDead)
            return pk.transform.position.z - pos.z > checkDis;

        id = DistanceOrderList[DistanceOrderList.Count - 2];
        if (IsAI(id))
            return GetObjBySelectedID(id).transform.position.z - pos.z > checkDis;

        pk = LobbyManager.Instance.GetPlayerByID(id);
        return pk.transform.position.z - pos.z > checkDis;
    }

    /// <summary>
    /// 太靠前
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="checkDis"></param>
    /// <returns></returns>
    public bool ForwardDestroy(Vector3 pos, float checkDis)
    {
        int id = DistanceOrderList[0];
        return pos.z - GetObjBySelectedID(id).transform.position.z > checkDis;
    }

    /// <summary>
    /// 判断指定id玩家是否是NPC
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool IsAI(int id)
    {
        for (int i = 0; i < _AIList.Count; ++i)
        {
            if (_AIList[i].ID == id)
                return true;
        }

        return false;
    }

    /// <summary>
    /// 获取行驶距离第一名的位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetForwardPos()
    {
        int id = DistanceOrderList[0];
        return GetObjBySelectedID(id).transform.position;
    }

    /// <summary>
    /// 通过唯一ID获取对于AI
    /// </summary>
    /// <param name="unique"></param>
    /// <returns></returns>
    public AIKinematics GetAIByUnique(int unique)
    {
        for (int i = 0; i < _AIList.Count; ++i)
        {
            if (unique == _AIList[i].UniqueOrder)
                return _AIList[i];
        }
        return null;
    }

    public AIKinematics GetAIByID(int id)
    {
        for (int i = 0; i < _AIList.Count; ++i)
        {
            if (id == _AIList[i].ID)
                return _AIList[i];
        }
        return null;
    }

    /// <summary>
    /// 第一名的速率
    /// </summary>
    /// <returns></returns>
    public float GetForwardPlayerOrAISpeedFactor()
    {
        int id = DistanceOrderList[0];
        for (int i = 0; i < AIList.Count; ++i)
        {
            if (id == AIList[i].ID)
                return AIList[i].SpeedFactor;
        }

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (id == LobbyManager.Instance.PKList[i].ID)
                return LobbyManager.Instance.PKList[i].SpeedFactor;
        }

        return 0;
    }

    ///// <summary>
    ///// 选车列表中是否包含当前ID
    ///// </summary>
    ///// <returns></returns>
    //public bool HasSelected(int id)
    //{
    //    for (int i = 0; i < LobbyManager.Instance.SelectedList.Count; ++i)
    //    {
    //        if (id == LobbyManager.Instance.SelectedList[i])
    //            return true;
    //    }
    //    return false;
    //}

    /// <summary>
    /// 获取后面紧随玩家图想
    /// </summary>
    public int GetFollowCard()
    {
        int index = 0;
        for (int i = 0; i < _DistanceOrderList.Count; ++i)
        {
            if (_DistanceOrderList[i] == _player.ID)
            {
                index = i;
                break;
            }
        }

        if (index == _DistanceOrderList.Count - 1)
            return -1;
        else
        {
            if (_player.Distance - _idDisDic[_DistanceOrderList[index + 1]] < 50 && _player.Distance - _idDisDic[_DistanceOrderList[index + 1]] > 0)
                return _DistanceOrderList[index + 1];
        }

        return -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void ChangeObserver(int id)
    {
        if (_observedID == id)
            return;
        _observedID = id;
        ObservedRigidbody();
    }

    /// <summary>
    /// 获取被观察者的刚体组件
    /// </summary>
    /// <returns></returns>
    public void ObservedRigidbody()
    {
        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (ObservedID == LobbyManager.Instance.PKList[i].ID)
                _observedPlayer = LobbyManager.Instance.PKList[i];
        }
    }

    /// <summary>
    /// 被观察者是否劫持卡车
    /// </summary>
    /// <returns></returns>
    public Hold ObservedHoldState()
    {
        for (int i = 0; i < AIList.Count; ++i)
        {
            if (ObservedID == AIList[i].ID)
                return AIList[i].HoldState;
        }

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (ObservedID == LobbyManager.Instance.PKList[i].ID)
                return LobbyManager.Instance.PKList[i].HoldState;
        }

        return Hold.None;
    }

    /// <summary>
    /// 观察者改变
    /// </summary>
    public void ObservedChange()
    {
        if (AllDead())
        {
            ChangeMode(RunMode.SceneEnd);
            return;
        }

        int index   = GetIndexInDistanceList(ObservedID);
        int id      = 0;

        for (int i = index + 1; i < DistanceOrderList.Count; ++i)
        {
            id = DistanceOrderList[i];
            if (LobbyManager.Instance.GetPlayerByID(id) == null || LobbyManager.Instance.GetPlayerByID(id).IsDead || LobbyManager.Instance.GetPlayerByID(id).IsContinue)
                continue;
            ChangeObserver(id);
            return;
        }

        for (int i = 0; i < index; ++i)
        {
            id = DistanceOrderList[i];
            if (LobbyManager.Instance.GetPlayerByID(id) == null || LobbyManager.Instance.GetPlayerByID(id).IsDead || LobbyManager.Instance.GetPlayerByID(id).IsContinue)
                continue;
            ChangeObserver(id);
            return;
        }
    }

    /// <summary>
    /// 返回最前面一位的ID
    /// </summary>
    /// <returns></returns>
    public int GetForwardID()
    {
        if (DistanceOrderList.Count == 0)
            return -1;

        return DistanceOrderList[0];
    }

    /// <summary>
    /// 所有玩家死亡
    /// </summary>
    /// <returns></returns>
    public bool AllDead()
    {
        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
        {
            if (!LobbyManager.Instance.PKList[i].IsDead)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 除了本角色意外，其他玩家均死亡
    /// </summary>
    /// <returns></returns>
    public bool OtherAllDead()
    {
        for (int i = 0; i < LobbyManager.Instance.PKList.Count;++i )
        {
            if (LobbyManager.Instance.PKList[i].ID != PlayerRef.ID)
            {
                if(!LobbyManager.Instance.PKList[i].IsDead)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 游戏状态控制
    /// </summary>
    /// <param name="newMode"></param>
    public void ChangeMode(RunMode newMode)
    {
        if (_runMode == newMode)
            return;

        switch (newMode)
        {
            case RunMode.Waiting:
                SceneManager.Instance.ChangeSceneDirect(EnumSceneType.LoginScene, EnumUIType.PanelStart);
                break;
            case RunMode.Setting:
                Message message = new Message(MessageType.EVNET_SETTING_CONFIRM, this);
                message.Send();
                break;
            case RunMode.Movie:
                ++_movieID;
                _movieID %= 3;
                SceneManager.Instance.ChangeSceneDirect(EnumSceneType.MovieScene, EnumUIType.PanelMovie);
                break;
            case RunMode.Select:
                if (Mode == RunMode.Movie)
                    SceneManager.Instance.ChangeSceneDirect(EnumSceneType.LoginScene, EnumUIType.PanelSelect);
                break;
            case RunMode.ReadyToRace:
                EnvirManager.Instance.Init();
                break;
            case RunMode.Racing:
                if (SpawnManager.Instance != null && _runMode == RunMode.ReadyToRace)
                    SpawnManager.Instance.OnStartGame();
                if (_runMode == RunMode.Continue)
                {
                    NexStepState();
                    _player.Continue(false);
                    message         = new Message(MessageType.Message_Mode_Continue, this);
                    message["show"] = false;
                    message.Send();
                }
                else
                    ActivateNormalEffect();
                break;
            case RunMode.SceneEnd:
                if (_runMode == RunMode.Continue)
                {
                    message         = new Message(MessageType.Message_Mode_Continue, this);
                    message["show"] = false;
                    message.Send();
                }
                message = new Message(MessageType.Message_Game_End, this);
                message.Send();

                SoundController.Instance.StopEngineMusic();
                SoundController.Instance.StopSceneGameMusic();
                SoundController.Instance.PlayPersonSoundGameOver();
                if (_airPlane != null)
                    SoundController.Instance.StopBossMusic();

                break;
            case RunMode.Continue:
                _continueTime   = GameConfig.GAME_CONFIG_CONTINUE_TIME;
                message         = new Message(MessageType.Message_Mode_Continue, this);
                message["show"] = true;
                message.Send();
                _player.Continue(true);
                break;
            case RunMode.Dead:
                _player.Dead();
                message = new Message(MessageType.Message_Mode_Dead, this);
                message.Send();
                break;
        }

        _runMode = newMode;
    }

    /// <summary>
    /// 开启联机模式
    /// </summary>
    public void UNetGameBegine()
    {
        if (_selectMode == SelectMode.SinglePlayer)
        {
            if (GameConfig.GAME_CONFIG_MODE_TEST)
            {
                LobbyManager.Instance.TestStartClient();
                GameMode.Instance.ModeSelected = SelectMode.Multiplayer;
            }else
                LobbyManager.Instance.OnSingleGame();
        }
        else
            LobbyManager.Instance.OnMulityGame();
    }

    /// <summary>
    /// 没有人加入联机，则返回到单机和联机模式选择界面中,并做相应的数据复原
    /// </summary>
    public void ReturnModeSelect()
    {
        LobbyManager.Instance.StopGame();
        _selectMode = SelectMode.Multiplayer;
        LobbyManager.Instance.Reset();
    }

    //public void AddScore(int id, int value)
    //{
    //    TriggerAddScore(id, value);
    //    if (id != _player.ID)
    //        return;
    //    Message message     = new Message(MessageType.Message_Show_Score, this);
    //    message["score"]    = value;
    //    message.Send();
    //}
   

    #endregion

    public override void Awake()
    {
        base.Awake();
        
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Fix_Frame, UpdateFixFrame);
        MessageCenter.Instance.AddListener(MessageType.Message_View_Is_Open, OnUIPanelOpen);
    }

    public override void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Fix_Frame, UpdateFixFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_View_Is_Open, OnUIPanelOpen);
        base.OnDestroy();
    }
  
    #region Private
    /// <summary>
    /// 更新字典数据
    /// </summary>
    private void CopyDataToDic()
    {
        _idDisDic.Clear();
        _idScoreDic.Clear();

        lst.Clear();
        lst = new List<KeyValuePair<int, int>>(_orderIdDic);

        for (int i = 0; i < lst.Count; ++i)
        {
            for (int j = 0; j < AIList.Count; ++j)
            {
                if (lst[i].Value == AIList[j].ID)
                {
                    _idDisDic.Add(lst[i].Value, (int)AIList[j].Distance);
                    _idScoreDic.Add(lst[i].Value, (int)AIList[j].Score);
                    break;
                }
            }

            for (int j = 0; j < LobbyManager.Instance.PKList.Count; ++j)
            {
                if (lst[i].Value == LobbyManager.Instance.PKList[j].ID)
                {
                    _idDisDic.Add(lst[i].Value, (int)LobbyManager.Instance.PKList[j].Distance);
                    _idScoreDic.Add(lst[i].Value, (int)LobbyManager.Instance.PKList[j].Score);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 排序
    /// </summary>
    private void SortDic()
    {
        lst.Clear();
        lst = new List<KeyValuePair<int, int>>(_idDisDic);
        lst.Sort(Compare);
        _DistanceOrderList.Clear();

        for (int i = 0; i < lst.Count; ++i)
            _DistanceOrderList.Add(lst[i].Key);

        lst.Clear();
        lst = new List<KeyValuePair<int, int>>(_idScoreDic);
        lst.Sort(Compare);
        _ScoreOrderList.Clear();

        for (int i = 0; i < lst.Count; ++i)
            _ScoreOrderList.Add(lst[i].Key);
    }

    private int GetIndexInDistanceList(int obID)
    {
        for (int i = 0; i < DistanceOrderList.Count; ++i)
        {
            if (DistanceOrderList[i] == ObservedID)
                return i;
        }
        return -1;
    }

    #endregion


    #region Message
    private void OnUIPanelOpen(Message message)
    {
        SendCoinMsg();
    }

    private void UpdateFixFrame(Message message)
    {
        if (Mode == RunMode.Waiting)
        {
            if (_waittingTime > 0)
                _waittingTime -= Time.fixedDeltaTime;
            else
                ChangeMode(RunMode.Movie);
        }

        if (!_startFire && Mode == RunMode.ReadyToRace && (int)LobbyManager.Instance.LobbyPlayer.ReadyTime == 11)
            StartCoroutine(OpenStartFire());

        if (Mode >= RunMode.ReadyToRace)
        {
            CopyDataToDic();
            SortDic();
        }
        else
            return;

        if (Mode == RunMode.Racing || Mode == RunMode.Continue || Mode == RunMode.Dead)
        {                 
            if (Mode == RunMode.Racing)
                UpdateCheckPoint();

            if (Mode == RunMode.Continue)
            {
                _continueTime -= Time.fixedDeltaTime;
                if (_continueTime <= 0)
                {
                    if (OtherAllDead())
                        ChangeMode(RunMode.SceneEnd);
                    else
                        ChangeMode(RunMode.Dead);
                }
            }

            if (LobbyManager.Instance.LobbyPlayer.LifeValue <= 0)
                ChangeMode(RunMode.SceneEnd);

            PlayeWolfAndWolSound();
        }
        else if (Mode == RunMode.GameOver)
        {
            LobbyManager.Instance.GoToLobbyScene();
            UIManager.Instance.OpenUICloseOthers(EnumUIType.PanelStart, null, EnumUIType.PanelMain);
            ModeStart();
        }
    }

    private void UpdatePreFrame(Message message)
    {
        switch (_inputType)
        {
            case InputType.Builtin:
                IOManager.Instance.UpdateInputEvent();
                break;
            case InputType.External:
                IOManager.Instance.UpdateIOEvent();
                break;
        }

        UpdatePlayerInput();

        if (Mode > RunMode.ReadyToRace)
            if (AllDead())
                ChangeMode(RunMode.SceneEnd);
    }

    #endregion
   

    public int Compare(KeyValuePair<int, int> x, KeyValuePair<int, int> y)
    {
        if (x.Value.CompareTo(y.Value) != 0)
            return x.Value.CompareTo(y.Value) * -1;
        else
            return x.Key.CompareTo(y.Key);
    }

    public int Compare(AIKinematics x, AIKinematics y)
    {
        throw new System.NotImplementedException();
    }

    //void OnGUI()
    //{
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 20;
    //    if (null != PlayerRef)
    //    {
    //        GUI.Label(new Rect(250, 250, 100, 100), "检查点： " + GameConfig.GAME_CONIFIG_CHECK_POINT[0] + "," + GameConfig.GAME_CONIFIG_CHECK_POINT[1] + "," + GameConfig.GAME_CONIFIG_CHECK_POINT[2], style);
    //        //GUI.Label(new Rect(250, 300, 100, 100), SamplePoolManager.Instance.BonusList.Count.ToString(), style);
    //        //GUI.Label(new Rect(250, 350, 100, 100), SamplePoolManager.Instance.OpponentList.Count.ToString(), style);
    //        //GUI.Label(new Rect(300, 200, 100, 100), _player._syncDelta.ToString(), style);
    //    }
    //}
}
