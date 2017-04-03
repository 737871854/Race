/*
 * Copyright (c) 
 * 
 * 文件名称：   SpawnManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/24 10:15:15
 * 
 * 修改描述：   场景对象管理类,和对象池一起负责场景对象的生成与移除
 *              
 *              有时间需要把该类里面很多功能放入GameMode模块，保证该类专注于场景对象的管理，而非其他功能
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager _instance;

    public static SpawnManager Instance
    {
        get
        {
            return _instance;
        }
    }

    #region private members
    // 用于存放将要Spawn的ObjectClas
    private List<SpawnableObjectClass> _wantToSpawn;
    private SpawnableObjectClass _spawnDelayed;
    // 用于存放Truck道具刷新点
    private List<float> _nextTruckSpawnPos;
    private float[] _freeLanes;
    // 是否可以刷新Truck道具
    private bool _truckIsComing;
    private int _laneChoosen;
    private int _nextTruckLane = -1;
    private int _isTruckOnScreen = 0;
    // 游戏是否开始
    private bool _started = false;
    // 最新刷新的Spawn时，player的distance
    private float _lastPositionSpawn;
    // 最新刷新的Spawn
    private OpponentKinematics _lastOpponentSpawn;
    private GameObject _lastHold;

    private int _vehicleTrafficIndex;
    private float[] _backupTrafficValues;
    // 道具刷新点控制参数
    private float _minStartMinSpawn;
    private float _minStartMaxSpawn;
    private float _minEndMinSpawn;
    private float _minEndMaxSpawn;

    private float _turboTrafficMultiplier   = 0.6f;
    private float _truckMultiplier          = 1.0f;
    private float _helicopterMultiplier     = 1.0f;
    private float _currentTruckMultiplier   = 1.0f;
    private float _backupTruckMultiplier    = 1.0f;

    private float _lifeValue;

    // Boss
    private GameObject _airPlane;
    private int _bossNum;
    private bool _isBossComing;                                                    // boss来袭

    // 魔方
    private bool _activeMagic;
    private float _interalTimer0;
    public List<int> _randTrigger;

    private int _numTrubo;
    private int _lastTruboLane;

    // 加速带
    private bool _activeSpeed;
    private float _interalTimer1;

    // 飓风
    private float _typhoonTimer = 35;
    private int _refreshNumber = 0;

    // 加速带
    private bool _hasSpawnCheckPoint;
    #endregion

    #region public members
    [HideInInspector]
    public int _objectDistanceForRemoving = 15;
    public List<SpawnableObjectClass> _objectsToSpawn;
    public List<AIKinematics> _AIList;
    #endregion

    #region public properties
    public int IsTruckOnScreen                          { get { return _isTruckOnScreen; }          set { _isTruckOnScreen = value; } }
    public float TurboTrafficMultiplier                 { get { return _turboTrafficMultiplier; }   set { _turboTrafficMultiplier = value; } }
    public AirPlaneBehaviour Airplane                   { get { if (null != _airPlane) return _airPlane.GetComponent<AirPlaneBehaviour>(); return null; } }
    public bool BossComing                              { get { return _isBossComing; }             set { _isBossComing = value; } }
    public float LifeValue                              { get { return _lifeValue; }                set { _lifeValue = value; } }
    #endregion

    #region Unity callback
    void Awake()
    {
        _instance                   = this;
       _nextTruckSpawnPos           = new List<float>();
       _wantToSpawn                 = new List<SpawnableObjectClass>();
       _freeLanes                   = new float[5] { -1.0f, -1.0f, -1.0f, -1.0f, -1.0f };

       UpdateTrafficDataByPlayerLevel();

       MessageCenter.Instance.AddListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
       MessageCenter.Instance.AddListener(MessageType.Message_Update_Fix_Frame, UpdateFixFrame);

     
    }
    // Use this for initialization

    void Destroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Fix_Frame, UpdateFixFrame);
    }

    #endregion

    #region Public Function
    /// <summary>
    /// 场景加载时，数据初始化
    /// </summary>
    public void Init()
    {
        _lifeValue          = GameConfig.GAME_CONFIG_MAX_LIFE_TIME;
        _interalTimer1      = 2;
        _bossNum            = 0;
        _started            = true;
        _lastPositionSpawn  = 8.0f;
        _lastOpponentSpawn  = null;
        _lastHold           = null;
        _nextTruckSpawnPos  = new List<float>();
        _nextTruckLane      = -1;
        _isBossComing       = false;
        _activeSpeed        = false;
        _typhoonTimer       = 35;
        _refreshNumber      = 0;
        _hasWarning         = false;
        _hasSpawnCheckPoint = false;
        _lifeValue          = GameConfig.GAME_CONFIG_MAX_LIFE_TIME;
    }

    /// <summary>
    /// 开始游戏时，数据初始化
    /// </summary>
    public void OnStartGame()
    {
        _randTrigger        = new List<int>();
        _activeMagic        = false;
        _activeMagic        = false;
        _interalTimer0      = 20.0f;
        for (int i = 0; i < 5; ++i )
        {
            _randTrigger.Add(i);
        }

        for (int i = 0; i < _objectsToSpawn.Count; ++i )
        {
            SpawnableObjectClass currObj = _objectsToSpawn[i];
            currObj.Reset();
        }

        for (int i = 0; i < _freeLanes.Length; ++i )
        {
            _freeLanes[i] = -1.0f;
        }

        _objectsToSpawn[_vehicleTrafficIndex]._startMinSpawnDistance    = _backupTrafficValues[0];
        _objectsToSpawn[_vehicleTrafficIndex]._startMaxSpawnDistance    = _backupTrafficValues[1];
        _objectsToSpawn[_vehicleTrafficIndex]._endMinSpawnDistance      = _backupTrafficValues[2];
        _objectsToSpawn[_vehicleTrafficIndex]._endMaxSpawnDistance      = _backupTrafficValues[3];
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void OnGameOver ()
    {
        _started = false;
        if (isServer)
            DestroyAllObjects();
    }

    /// <summary>
    /// 创建AI
    /// </summary>
    /// <param name="id"></param>
    public void CreateAI(int id)
    {
        AIKinematics ai = GameObject.Instantiate(_AIList[id]) as AIKinematics;
        ai.ID = id;
    }

    /// <summary>
    /// 开始游戏初始化车辆道具刷新
    /// </summary>
    /// <param name="startDistance"></param>
    /// <param name="quantity"></param>
    /// <param name="distanceBetween"></param>
    public void SpawnTrafficRandomly(float startDistance, int quantity, int distanceBetween)
    {
        if(!isServer)
            return;

        Vector3 startPositionOffset = Vector3.forward * startDistance;
        for (int i = 0; i < quantity; ++i )
            SpawnObject(new SpawnableObjectClass(SamplePoolManager.ObjectType.TrafficVehicle, 0, 0, 0, 0, 0), 1, 0.0f, -1, startPositionOffset + Vector3.forward * distanceBetween * i);
    }

    /// <summary>
    /// 判断当前车道是否被占用
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool LaneIsFree(int index)
    {
        bool truckLane = _truckIsComing && index == _nextTruckLane;
        return !truckLane;
    }
  
    /// <summary>
    /// 启动武器
    /// </summary>
    public void ActivateWeapon(SamplePoolManager.ObjectType type, int lane = 0)
    {
        if (!isServer)
            return;

        GameObject go           = SamplePoolManager.Instance.RequestObject(type);
        go.transform.rotation   = Quaternion.identity;
        if (type == SamplePoolManager.ObjectType.BonusGuidedAttack)
            go.GetComponent<BonusBehaviour>().InitData(lane, Vector3.zero, Vector3.zero, GameMode.Instance.GetForwardID());
        else
            go.GetComponent<BonusBehaviour>().InitData(lane);
        CmdSpawnObj(go);
    }

    private float _updateTimer          = 0.1f;
    private int _carsEvaluatedPerFrem   = 4;
    private SamplePoolManager.ObjectType _oppType;
    private OpponentKinematics _oppKin;
    private BonusBehaviour _bonBhv;
    private Rigidbody _oppRb;
    private GameObject _opponent;
    private bool _hasWarning;
    public void UpdatePreFrame(Message message)
    {
        if (!isServer)
            return;

        if (GameMode.Instance.Mode < RunMode.ReadyToRace || GameMode.Instance.Mode == RunMode.GameOver)
            return;

        //if (LobbyManager.Instance.LobbyPlayer.LifeValue < 4)
        if (LobbyManager.Instance.LobbyPlayer.LifeValue < 4)
            return;

        _updateTimer -= Time.deltaTime;
        if (_updateTimer < 0.0f)
        {
            _updateTimer = 0.01f;

            for (int i = 0; i < SamplePoolManager.Instance.OpponentList.Count; ++i )
            {
                _opponent                   = SamplePoolManager.Instance.OpponentList[i].gameObject;
                _oppRb                      = _opponent.GetComponent<Rigidbody>();
                _oppKin                     = SamplePoolManager.Instance.OpponentList[i];
                _oppType                    = _opponent.GetComponent<SpawnableObjectType>()._type;
                bool disableTooFar          = false;
                bool disableTooFarForward   = false;
                if (_oppType == SamplePoolManager.ObjectType.TrafficVehicle)
                {
                    // 如果TrafficVehicle被Player超越，或者TrafficVehicle被击飞，该TrafficVehicle可被移除场景
                    disableTooFar = (GameMode.Instance.BackforwardDestroy(_oppRb.position, 8) || _oppRb.position.y > 20.0f);

                    if (GameMode.Instance.GetForwardDistance() > 20.0f)
                        // 如果最前面的Player落后TrafficVehicle达到一定距离，则该TrafficVehicle也将被移除场景
                        disableTooFarForward = GameMode.Instance.ForwardDestroy(_oppKin.transform.position, 300);
                }
                else if (null != _oppRb)
                    // 非TrafficVehicle对象移除场景
                    disableTooFar = (GameMode.Instance.BackforwardDestroy(_oppRb.position, _objectDistanceForRemoving) || _oppRb.position.y > 20.0f);

                if (disableTooFar || disableTooFarForward)
                    // 使用对象池对目标对象移除
                    SamplePoolManager.Instance.NotifyDestroyingParent(_opponent, _oppType);
            }
        }

        // Manager free lanes
        int freeLanesLength = _freeLanes.Length;
        for (int i = 0; i < freeLanesLength; ++i)
        {
            if (_freeLanes[i] >= 0.0f)
                _freeLanes[i] -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 更新Opp道具
    /// </summary>
    void UpdateFixedOpp()
    {
        for (int i = 0; i < SamplePoolManager.Instance.OpponentList.Count; ++i)
            SamplePoolManager.Instance.OpponentList[i].UpdateFixedFrame();
    }

    /// <summary>
    /// 更新Bonus道具
    /// </summary>
    void UpdatePreBouns()
    {
        for (int i = 0; i < SamplePoolManager.Instance.BonusList.Count; ++i)
            SamplePoolManager.Instance.BonusList[i].UpdateFixedFrame();
    }

    private float _waitOther;
    public void UpdateFixFrame(Message message)
    {
        UpdateFixedOpp();
        UpdatePreBouns();

        if (!isServer)
            return;

        if (GameMode.Instance.Mode < RunMode.ReadyToRace)
        {
            _waitOther  = 0;
            return;
        }

        if (_waitOther < GameConfig.GAME_CONFIG_READY_TIME)
        {
            _waitOther += Time.fixedDeltaTime;
            LobbyManager.Instance.UpdateReadyValue(_waitOther);
        }
        else
            LobbyManager.Instance.UpdateReadyValue(GameConfig.GAME_CONFIG_READY_TIME);


        if (GameMode.Instance.Mode == RunMode.Racing || GameMode.Instance.Mode == RunMode.Continue || GameMode.Instance.Mode == RunMode.Dead)
        {
            _lifeValue -= Time.fixedDeltaTime;
            if (_lifeValue <= 0)
                _lifeValue = 0;
        }

        LobbyManager.Instance.UpdateLifeValue(_lifeValue);

        if (GameMode.Instance.Mode != RunMode.Racing && GameMode.Instance.Mode != RunMode.Dead && GameMode.Instance.Mode != RunMode.Continue)
            return;

        // 保证airplane只出现一次------------------------------------
        if (_lifeValue <= 48 && !_hasWarning)
        {
            _hasWarning = true;
            LobbyManager.Instance.LobbyPlayer.RpcOnBossCome();
        }
        if (_lifeValue <= 45)
        {
            if (_bossNum == 0)
            {
                SoundController.Instance.StartBossMusic(0);
                ActivateBadGuy(SamplePoolManager.ObjectType.AirPlane);
                _isBossComing = true;
                ++_bossNum;
            }

            if (_bossNum == 1 && !_isBossComing)
            {
                if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer)
                {
                    for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
                    {
                        LobbyManager.Instance.PKList[i].CmdActivateTurbo(_lifeValue - 1);
                        LobbyManager.Instance.PKList[i].CmdEngineEffect(true);
                    }
                }
                else
                {
                    GameMode.Instance.PlayerRef.CmdActivateTurbo(_lifeValue - 1);
                    GameMode.Instance.PlayerRef.CmdEngineEffect(true);
                    for (int i = 0; i < GameMode.Instance.AIList.Count; ++i )
                    {
                        GameMode.Instance.AIList[i].ActivateTurbo(_lifeValue - 1);
                        GameMode.Instance.AIList[i].ActiveateEngineEffect(true);
                    }
                }

                LobbyManager.Instance.LobbyPlayer.RpcOnSceneEffect(new Vector3(0, GameMode.Instance.GetForwardPos().y, GameMode.Instance.GetForwardPos().z + 100));
                //EnvirManager.Instance.AddEffectAtPositon(new Vector3(GameMode.Instance.GetForwardPos().x, GameMode.Instance.GetForwardPos().y, GameMode.Instance.GetForwardPos().z + 100));

                ++_bossNum;
            }
        }
        //------------------------------------

        // 刷魔方--------------------------------------
        if (_interalTimer0 > 0)
            _interalTimer0 -= Time.fixedDeltaTime;
        else
        {
            _interalTimer0 = 20.0f;
            _activeMagic = true;
        }

        if (_activeMagic && _lifeValue > 50)
        {
            // 魔方只刷新5次
            if (_randTrigger.Count > 0)
            {
                _activeMagic = false;
                int index = Util.Random(0, _randTrigger.Count);
                int rand = _randTrigger[index];
                _randTrigger.RemoveAt(index);
                SpawnMagic(rand);
            }
        }
        // ------------------------------------

        // ----------刷检查点----------------
        SpawnCheckPoint();
        // ----------------------

        // 刷加速带--------------------------------------
        if (_interalTimer1 > 0)
            _interalTimer1 -= Time.fixedDeltaTime;
        else
        {
            _interalTimer1 = Util.Random(5, 8);
            _activeSpeed = true;
        }
        if (_activeSpeed && _bossNum == 0)
        {
            _activeSpeed = false;
            SpawnSpeedRoad();
        }

        // 刷飓风
        if (_typhoonTimer > 0)
            _typhoonTimer -= Time.fixedDeltaTime;
        else
        {
            if (_refreshNumber < 4)
            {
                _typhoonTimer = 20;
                ++_refreshNumber;
                SpawnTyphoon();
            }
        }

        float currentPlayerDistance = GameMode.Instance.GetForwardDistance();

        _truckIsComing = _nextTruckSpawnPos.Count > 0 && (_nextTruckSpawnPos[0] - currentPlayerDistance) < 80.0f && (_nextTruckSpawnPos[0] - currentPlayerDistance) > 0.0f;
        if (_truckIsComing && _nextTruckLane < 0)
            _nextTruckLane = ChooseFreeLane(false, false);

        for (int i = 0; i < _objectsToSpawn.Count; ++i)
        {
            SpawnableObjectClass currObj = _objectsToSpawn[i];

            if (false && currObj._type != SamplePoolManager.ObjectType.TrafficVehicle)
                continue;

            // 是第一次使用用NextSpawnObjectDistance时去生成下个object出生点，Plaer行驶距离达到NextSpawnObjectDistance条件时去Spawn下个Object
            if (_started && (currObj.NextSpawnObjectDistance == -1.0f || currentPlayerDistance >= currObj.NextSpawnObjectDistance))
            {
                float ratio = 1.0f;
                float minDistance = currObj._startMinSpawnDistance + ratio * (currObj._endMinSpawnDistance - currObj._startMinSpawnDistance);
                float maxDistance = currObj._startMaxSpawnDistance + ratio * (currObj._endMaxSpawnDistance - currObj._startMaxSpawnDistance);
                if (currObj._type == SamplePoolManager.ObjectType.TrafficVehicle)
                {
                    minDistance *= _turboTrafficMultiplier;
                    maxDistance *= _turboTrafficMultiplier;
                }
                bool isTruck = SamplePoolManager.Instance.IsATruck(currObj._type);
                if (isTruck && _truckMultiplier != 1.0f && _helicopterMultiplier == 1.0f)
                {
                    minDistance *= _truckMultiplier;
                    maxDistance *= _truckMultiplier;
                }

                if (currObj._type == SamplePoolManager.ObjectType.TruckTurbo && _helicopterMultiplier != 1.0f)
                {
                    minDistance *= _helicopterMultiplier;
                    maxDistance *= _helicopterMultiplier;
                }

                // 满足条件，把将要Spawn到场景的object添加入Spawn列表里
                if (currObj.NextSpawnObjectDistance != -1.0f)
                     _wantToSpawn.Add(currObj);

                 currObj.NextSpawnObjectDistance += Random.Range(minDistance, maxDistance);

                if (isTruck)
                    _nextTruckSpawnPos.Add(currObj.NextSpawnObjectDistance);
            }
        }

        bool canSpawn = (currentPlayerDistance - _lastPositionSpawn) > 6.0f;
        if (null != _lastOpponentSpawn && canSpawn)
            // player接近最新刷出的道具时，刷新下一个
            canSpawn = (_lastOpponentSpawn.OpponentRigidbody.position.z - GameMode.Instance.GetForwardPos().z) < 200;

        bool alwaysSpawnFound = false;
        int wantToSpawnCount = _wantToSpawn.Count;
        if (wantToSpawnCount > 0)
        {
            int higherPriority = 999;
            int higherPriorityIdex = -1;
            for (int i = 0; i < wantToSpawnCount; ++i)
            {
                if (_wantToSpawn[i]._alwaysSpawn)
                {
                    alwaysSpawnFound = true;
                    if (canSpawn)
                    {
                        canSpawn = true;
                        higherPriority = _wantToSpawn[i]._priority;
                        higherPriorityIdex = i;
                    }
                    else
                        _spawnDelayed = _wantToSpawn[i];
                }
                else if (_wantToSpawn[i]._priority < higherPriority && canSpawn && !alwaysSpawnFound)
                {
                    higherPriority = _wantToSpawn[i]._priority;
                    higherPriorityIdex = i;
                }
            }

            if (higherPriorityIdex >= 0 && _spawnDelayed == null && !_isBossComing)
            {
                // 如果是刷金币，连刷5个，其他，则只刷1个
                int quantity = _wantToSpawn[higherPriorityIdex]._type == SamplePoolManager.ObjectType.BonusMoney ? 5 : 1;
                // 如果是刷金币，每个金币间隔距离为5
                float distanceBetween = _wantToSpawn[higherPriorityIdex]._type == SamplePoolManager.ObjectType.BonusMoney ? 5 : 0;
                Vector3 offset = Vector3.zero;
                if (_wantToSpawn[higherPriorityIdex]._type == SamplePoolManager.ObjectType.TruckTurbo)
                    offset = new Vector3(0, 0, 5);

                _lastOpponentSpawn  = SpawnObject(_wantToSpawn[higherPriorityIdex], quantity, distanceBetween, -1, offset);
                _lastPositionSpawn  = currentPlayerDistance;
                if (_wantToSpawn[higherPriorityIdex]._type == SamplePoolManager.ObjectType.TruckTurbo )
                {
                    if (_lifeValue > 70)
                    {
                        if (_lastOpponentSpawn != null)
                        {
                            _lastOpponentSpawn.GetComponent<TruckBehaviour>().CmdTellServerShowGuided();
                            _lastOpponentSpawn = SpawnObject(new SpawnableObjectClass(SamplePoolManager.ObjectType.TruckHold, 0, 0, 0, 0, 0), 1, 0, _lastTruboLane, new Vector3(0, 0, 35));
                            _lastHold = _lastOpponentSpawn.gameObject;
                        }
                    }
                }
            }

            _wantToSpawn.Clear();

            if (null != _spawnDelayed)
            {
                _wantToSpawn.Add(_spawnDelayed);
                _spawnDelayed = null;
            }
        }
    }
    #endregion

    #region Private Function
    /// <summary>
    /// 实时刷新车辆道具数据初始化
    /// </summary>
    private void UpdateTrafficDataByPlayerLevel()
    {
        for (int i = 0; i < _objectsToSpawn.Count; ++i )
        {
            if (_objectsToSpawn[i]._type == SamplePoolManager.ObjectType.TrafficVehicle)
            {
                _objectsToSpawn[i]._startMinSpawnDistance = 120;
                _objectsToSpawn[i]._startMaxSpawnDistance = 130;
                _objectsToSpawn[i]._endMinSpawnDistance = 120;
                _objectsToSpawn[i]._endMaxSpawnDistance = 130;

                _vehicleTrafficIndex = i;
                _backupTrafficValues = new float[4];
                _backupTrafficValues[0] = _objectsToSpawn[_vehicleTrafficIndex]._startMinSpawnDistance;
                _backupTrafficValues[1] = _objectsToSpawn[_vehicleTrafficIndex]._startMaxSpawnDistance;
                _backupTrafficValues[2] = _objectsToSpawn[_vehicleTrafficIndex]._endMinSpawnDistance;
                _backupTrafficValues[3] = _objectsToSpawn[_vehicleTrafficIndex]._endMaxSpawnDistance;
                _minStartMinSpawn   = _objectsToSpawn[i]._startMinSpawnDistance;
                _minStartMaxSpawn   = _objectsToSpawn[i]._startMaxSpawnDistance;
                _minEndMinSpawn     = _objectsToSpawn[i]._endMinSpawnDistance;
                _minEndMaxSpawn     = _objectsToSpawn[i]._endMaxSpawnDistance;
            }
        }
    }

    /// <summary>
    /// 启动Boss
    /// </summary>
    /// <param name="type"></param>
    private void ActivateBadGuy(SamplePoolManager.ObjectType type)
    {
        if (type == SamplePoolManager.ObjectType.AirPlane)
        {
            if (null == _airPlane)
                _airPlane = SamplePoolManager.Instance.RequestObject(type);

            _airPlane.SetActive(true);
            _airPlane.transform.rotation = Quaternion.identity;
            _airPlane.GetComponent<AirPlaneBehaviour>().Initialize();
            CmdSpawnObj(_airPlane);
        }
    }

    /// <summary>
    /// 刷新魔方
    /// </summary>
    /// <param name="type"></param>
    private void SpawnMagic(int rand)
    {
        for (int i = 0; i < 5; ++i)
        {
            GameObject magic = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusMagic);
            magic.SetActive(true);
            magic.transform.rotation = Quaternion.identity;
            magic.GetComponent<BonusBehaviour>().InitData(i, Vector3.zero, Vector3.zero, rand);
            CmdSpawnObj(magic);
        }
    }

    /// <summary>
    /// 刷检查点
    /// </summary>
    private void SpawnCheckPoint()
    {
        if (_hasSpawnCheckPoint)
            return;
        for (int i = 0; i < GameConfig.GAME_CONIFIG_CHECK_POINT.Length; ++i)
        {
            GameObject go = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusCheckpoint);
            go.GetComponent<BonusBehaviour>().InitData(0, Vector3.zero, Vector3.zero, i);
            CmdSpawnObj(go);
        }

        _hasSpawnCheckPoint = true;
    }

    /// <summary>
    /// 刷新加速带
    /// </summary>
    private void SpawnSpeedRoad()
    {
        if (_lifeValue < 55)
            return;

        int lane = 0;
        int rand = 0;
        for (int i = 0; i < 2; ++i)
        {
            GameObject road = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusSpeed);
            road.transform.rotation = Quaternion.identity;
            while (lane == rand)
            {
                rand = Util.Random(0, 10);
                rand %= 3;
            }
            lane = rand;
            road.GetComponent<BonusBehaviour>().InitData(rand, Vector3.zero, Vector3.zero);
            CmdSpawnObj(road);
        }
    }

    /// <summary>
    /// 刷新飓风
    /// </summary>
    private void SpawnTyphoon()
    {
        for (int i = 0; i < 2; ++i)
        {
            GameObject go = SamplePoolManager.Instance.RequestObject(SamplePoolManager.ObjectType.BonusTyphoon);
            go.SetActive(true);
            go.transform.rotation = Quaternion.identity;
            go.GetComponent<BonusBehaviour>().InitData(2 * i + 1, Vector3.zero, Vector3.zero);
            CmdSpawnObj(go);
        }
    }

    protected GameObject lastObjectSpawned;
    protected float lastObjectSpawnedInitPos;
    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="currObj"></param>
    /// <param name="quantity">数量</param>
    /// <param name="distanceBetween">间隔</param>
    /// <param name="forceLane"></param>
    /// <param name="positionOffset"></param>
    /// <returns></returns>
    private OpponentKinematics SpawnObject(SpawnableObjectClass currObj, int quantity = 0, float distanceBetween = 0.0f, int forceLane = -1, Vector3 positionOffset = default(Vector3))
    {
        GameObject instance = null;

        SamplePoolManager.ObjectType objType = currObj._type;
        float initPosition = GameMode.Instance.GetForwardPos().z;
        Vector3 pos         = new Vector3(0.0f, 0.0f, initPosition + 150.0f ) + positionOffset;
        while(true)
        {
            _laneChoosen = forceLane < 0 ? ChooseFreeLane() : forceLane;
           if (_lastHold != null && _lastHold.gameObject.activeSelf)
           {
               if (Mathf.Abs(pos.z - _lastHold.transform.position.z) < 10 && SamplePoolManager.Instance.IsSameLaneWithTruck(_laneChoosen))
                   continue;
               else
                   break;
           }
           else
               break;
        }
        
        if (forceLane < 0)
        {
            if ((SamplePoolManager.Instance.IsATruck(objType) && _laneChoosen < 0) || (SamplePoolManager.Instance.IsATruck(objType) && _nextTruckLane < 0))
                return null;
        }

        for (int i = 0; i < quantity; i++ )
        {
            instance = SamplePoolManager.Instance.RequestObject(objType);
            instance.transform.position = new Vector3(0.0f, 0.0f, initPosition + 150.0f + i * distanceBetween) + positionOffset;

            instance.transform.rotation = Quaternion.identity;

            if (SamplePoolManager.Instance.IsABonus(objType))
                instance.transform.position += Vector3.forward * 5.0f;

            lastObjectSpawned = instance;
            lastObjectSpawnedInitPos = initPosition;

            if (instance.GetComponent<SpawnableObjectType>() == null)
                instance.AddComponent<SpawnableObjectType>();

            instance.GetComponent<SpawnableObjectType>()._type = objType;

            OpponentKinematics opponentKinematics = instance.GetComponent<OpponentKinematics>();
            opponentKinematics.CurrentPos = instance.transform.position;
            if (opponentKinematics != null)
            {
                // 保证Truck和TrafficVehicle不在同一个车道
                if (SamplePoolManager.Instance.IsATruck(objType))
                {
                    ++IsTruckOnScreen;
                    if (objType == SamplePoolManager.ObjectType.TruckHold)
                        opponentKinematics.CurrentLane = _lastTruboLane;
                    else
                    {
                        opponentKinematics.CurrentLane = _nextTruckLane;
                        _lastTruboLane = _nextTruckLane;
                    }
                }else
                    opponentKinematics.CurrentLane = _laneChoosen;

                if (forceLane != -1)
                    opponentKinematics.CurrentLane = forceLane;
            }

            _freeLanes[_laneChoosen] = 0.5f;
         
            if (SamplePoolManager.Instance.IsATruck(objType))
            {
                opponentKinematics.IsTruck = true;
               
                _nextTruckSpawnPos.RemoveAt(0);
                if (_nextTruckSpawnPos.Count == 0)
                    _nextTruckLane = -1;
                else
                    _nextTruckLane = ChooseFreeLane(false, true);
            }

            CmdSpawnObj(instance);
        }

        return instance.GetComponent<OpponentKinematics>();
    }

    private bool IsInWrongDirection(int lane)
    {
        //if (lane > 2)
        //{
        //    return true;
        //}

        return false;
    }

    /// <summary>
    /// 为刷新的车辆选择一条车道
    /// </summary>
    /// <param name="isBadGuy"></param>
    /// <param name="isTruck"></param>
    /// <returns></returns>
    private int ChooseFreeLane(bool isBadGuy = false, bool isTruck = false)
    {
        int laneChoosen = -1;
        int minLane = 0;
        int maxLane = 4;
        do
        {
            laneChoosen =Random.Range(minLane, maxLane + 1);
        } while (_laneChoosen == laneChoosen || !LaneIsFree(laneChoosen));// || TruckWillSpawnShortlyHere(laneChoosen)

        return laneChoosen;
    }

    private void DestroyAllObjects()
    {
        //SamplePoolManager.Instance.NotifyDestroyingExplosion();
        //for (int i = _opponentsObj.Count - 1; i >= 0; --i)
        //{
        //    GameObject opponent = _opponentsObj[i];
        //    if (null != opponent)
        //    {
        //        SamplePoolManager.Instance.NotifyDestroyingParent(opponent, opponent.GetComponent<SpawnableObjectType>()._type);
        //        _opponentsObj.RemoveAt(i);
        //    }
        //}


        for (int i = 0; i < SamplePoolManager.Instance.OpponentList.Count;++i )
        {
            //SamplePoolManager.Instance.NotifyDestroyingParent()
        }

    }

    #endregion

    [Command]
    private void CmdSpawnObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }

    #region Message
   
    #endregion
}
