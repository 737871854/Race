/*
 * Copyright (c) 
 * 
 * 文件名称：   SamplePoolManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/29 19:18:06
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using Need.Mx;
using System;

public class SamplePoolManager : NetworkBehaviour
{

    public static SamplePoolManager _instance;

    public static SamplePoolManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [Serializable]
    public class PoolData
    {
        // 类型
        public ObjectType objType;
        // 该类型的所有预制体列表
        public List<GameObject> objPrefab;
        // 总量
        public int quantity;
    }

    public enum ObjectType
    {
        None = -1,
        BonusCheckpoint,
        BonusMagic,                 // 魔方
        BonusMoney,                 // 金币
        BonusShield,                // 金钟罩
        BonusTurbo,
        BonusTime,
        BonusMoneyBig,
        BonusMagent,
        BonusGuided,                // 攻击boss导弹
        BonusLandmine,              // 地雷
        BonusEngine,                // 引擎
        BonusGun,                   // 机枪
        BonusTransparent,           // 透明
        BonusSpeed,                 // 加速带
        BonusBullet,                // 子弹
        BonusTyphoon,               // 飓风
        BonusGuidedAttack,          // 攻击玩家导弹
        TrafficVehicle,             // 交通工具
        TruckNormal,
        TruckTurbo,
        TruckHold,
        TruckTransform,
        AirPlane,                   // Boss
        Free_Guided,
        
        #region 特效
        Explosion,
        Smoke,
        Sparks,
        CarExplosion,
        Effect_Landmine_Explosion,
        Effect_Boss,


        Effect_Car_Start,
        Effect_Car_Accle,
        Effect_Car_SlowDown,
        Effect_Missile_Explosion,
        Effect_Car_Hide,
        Effect_Car_Magent,
        Effect_Car_Shield,
        Effect_SpeedGuide,
        Effect_Truck,
        Effect_Truck_Unbeatable,
        Effect_Car_Unbeatable,
        Effect_Cheetah,
        Effect_Car_Transform,
        Effect_Vehide_Explosion,

        Effect_SpeedLine,
        Effect_SpeedWind,
        Effect_SpeedEngine_Red,
        Effect_SpeedEngine_Green,
        Effect_Magic,
        Effect_Crash,
        Effect_Gun_Connect,
        Effect_Side,
        Effect_Leopard_Acc,
        Effect_Side_Collision,
        Effect_Hold_Body,
        Effect_Boss_Smoke,
        Effect_Land,
        //Effect_Engine,
        // UI特效
        Effect_ShunShine,
        Effect_UI_Accle,
        #endregion

        Scene_SpeedUp,
        
        Count,
    }


    #region private members
    /// <summary>
    /// 正在被使用的的对象
    /// </summary>
    private Dictionary<ObjectType, List<GameObject>> _usingPoolDic = new Dictionary<ObjectType, List<GameObject>>();
    /// <summary>
    /// 未被使用的的对象
    /// </summary>
    private Dictionary<ObjectType, List<GameObject>> _freePoolDic = new Dictionary<ObjectType, List<GameObject>>();

    private List<OpponentKinematics> _opponentList = new List<OpponentKinematics>();

    private List<BonusBehaviour> _bonusList = new List<BonusBehaviour>();

    private List<OpponentKinematics> _truckList = new List<OpponentKinematics>();

    private bool _isInitilized;
    #endregion


    #region public members
    /// <summary>
    /// 存放游戏需要用到的对象的原始预制体
    /// </summary>
    public List<PoolData> _poolList;

    public Dictionary<ObjectType, List<GameObject>> UsingPool   { get { return _usingPoolDic; } }
    public Dictionary<ObjectType, List<GameObject>> FreePool    { get { return _freePoolDic; } }
    public List<OpponentKinematics> OpponentList                { get { return _opponentList; } }
    public List<BonusBehaviour> BonusList                       { get { return _bonusList; } }
    public bool IsInitilized                                    { get { return _isInitilized; } }
    public List<OpponentKinematics> TruckList                   { get { return _truckList; } }
    #endregion

    #region unity callback
    void Awake()
    {
        _instance = this;
    }

    #endregion


    #region private function
    /// <summary>
    /// 初始化对象池, 让_freePoolDic包含所有类型的对象
    /// </summary>
    public void InitilizePool()
    {      
        for (int i = 0; i < _poolList.Count; ++i )
        {
            // 客户端中的这些对象由服务器生成
            if (!isServer 
                &&(_poolList[i].objType == ObjectType.TrafficVehicle 
                || _poolList[i].objType == ObjectType.TruckHold 
                || _poolList[i].objType == ObjectType.TruckTurbo 
                || _poolList[i].objType == ObjectType.AirPlane 
                || _poolList[i].objType == ObjectType.BonusLandmine 
                || _poolList[i].objType == ObjectType.BonusGuided
                || _poolList[i].objType == ObjectType.BonusGuidedAttack
                || _poolList[i].objType == ObjectType.BonusMagic
                || _poolList[i].objType == ObjectType.BonusSpeed
                || _poolList[i].objType == ObjectType.BonusTyphoon
                || _poolList[i].objType == ObjectType.BonusCheckpoint))
            {
                continue;
            }
            FillPoolEachItem(_poolList[i]);
        }
        _isInitilized = true;
    }

    private void FillPoolEachItem(PoolData data)
    {
        if (!_freePoolDic.ContainsKey(data.objType))
        {
            _freePoolDic.Add(data.objType, new List<GameObject>());
        }
        GameObject obj;
        for (int i = 0; i < data.objPrefab.Count; ++i )
        {
            for (int j = 0; j < data.quantity; ++j)
            {
                obj = GameObject.Instantiate(data.objPrefab[i], Vector3.zero, Quaternion.identity) as GameObject;
                obj.name = data.objType.ToString() + "_" + _freePoolDic[data.objType].Count;
                obj.SetActive(false);
                _freePoolDic[data.objType].Add(obj);
            }
        }
    }

    /// <summary>
    /// 从_freePoolDic随机获取指定ObjectType类型的对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject GetRandomObject(ObjectType type)
    {
        int i = Util.Random(0, _freePoolDic[type].Count);
        GameObject obj = _freePoolDic[type][i];
        _freePoolDic[type].RemoveAt(i);
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 将从_freePoolDic取出的对象存入_usingPoolDic
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    private void ObjFromFreeToUsed(ObjectType type, GameObject obj)
    {
        if (!_usingPoolDic.ContainsKey(type))
         {
             _usingPoolDic.Add(type, new List<GameObject>());
         }
        _usingPoolDic[type].Add(obj);

        // 
        AddToOppOrBonusList(type, obj);
    }

    /// <summary>
    /// 创建新对象，存入_freePoolDic
    /// </summary>
    private void CreateObj(ObjectType type)
    {
        PoolData data = null;
        for (int i = 0; i < _poolList.Count; ++i )
        {
            if (type == _poolList[i].objType)
            {
                data = _poolList[i];
            }
        }

        if (null == data)
        {
            Debuger.Log("Error ObjectType!!");
            return;
        }

        int index = Util.Random(0, data.objPrefab.Count);

        GameObject obj = GameObject.Instantiate(data.objPrefab[index], Vector3.zero, Quaternion.identity) as GameObject;
        obj.name = type.ToString() + "_" + _freePoolDic[type].Count + _usingPoolDic[type].Count;
        obj.SetActive(false);
        _freePoolDic[type].Add(obj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lane"></param>
    /// <returns></returns>
    public bool IsSameLaneWithTruck(int lane)
    {
        for (int i = 0; i < _truckList.Count; ++i )
        {
           if (lane == _truckList[i].CurrentLane)
           {
               return true;
           }
        }
        return false;
    }

    /// <summary>
    /// 把当前不再使用的对象从_usingPoolDic移到_freePoolDic
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    private void FromUsingToFree(ObjectType type, GameObject obj)
    {
        _usingPoolDic[type].Remove(obj);
        obj.transform.parent = null;
        obj.SetActive(false);
        _freePoolDic[type].Add(obj);

        RemoveFromOppOrBonusList(type, obj);
    }

    public void AddToOppOrBonusList(ObjectType type, GameObject obj)
    {
      if(isServer)
      {
          AddOPPOrBnus(type, obj);
      }
      else
      {
          AddOPPOrBnus(type, obj);
      }
    }

    public void RemoveFromOppOrBonusList(ObjectType type, GameObject obj)
    {
        if (isServer)
        {
            RemoveOppOBonus(type, obj);
        }
        else
        {
            RemoveOppOBonus(type, obj);
        }
    }

    /// <summary>
    /// --------TODO  重复加入
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    private void AddOPPOrBnus(ObjectType type, GameObject obj)
    {
        if (type >= ObjectType.BonusCheckpoint && type <= ObjectType.BonusGuidedAttack)
        {
            if (null != obj.GetComponent<BonusBehaviour>())
            {
                if (!_bonusList.Contains(obj.GetComponent<BonusBehaviour>()))
                {
                    _bonusList.Add(obj.GetComponent<BonusBehaviour>());
                }
            }
        }

        if (type >= ObjectType.TrafficVehicle && type <= ObjectType.TruckTransform)
        {
            if (!_opponentList.Contains(obj.GetComponent<OpponentKinematics>()))
            {
                _opponentList.Add(obj.GetComponent<OpponentKinematics>());
            }

            if (type == ObjectType.TruckHold || type == ObjectType.TruckTurbo)
            {
                if(!_truckList.Contains(obj.GetComponent<OpponentKinematics>()))
                {
                    _truckList.Add(obj.GetComponent<OpponentKinematics>());
                }
            }
        }
    }

    private void RemoveOppOBonus(ObjectType type, GameObject obj)
    {
        if (type >= ObjectType.BonusCheckpoint && type <= ObjectType.BonusGuidedAttack)
        {
            if (null != obj.GetComponent<BonusBehaviour>())
            {
                _bonusList.Remove(obj.GetComponent<BonusBehaviour>());
            }
        }

        if (type >= ObjectType.TrafficVehicle && type <= ObjectType.TruckTransform)
        {
            _opponentList.Remove(obj.GetComponent<OpponentKinematics>());
            if (type == ObjectType.TruckHold || type == ObjectType.TruckTurbo)
            {
                _truckList.Remove(obj.GetComponent<OpponentKinematics>());
            }
        }
    }
    #endregion

    #region public function
    public bool IsATruck(ObjectType checkType)
    {
        return checkType == ObjectType.TruckNormal || checkType == ObjectType.TruckTurbo || checkType == ObjectType.TruckTransform || checkType == ObjectType.TruckHold;
    }

    public bool IsABonus(ObjectType checkType)
    {
        return checkType == ObjectType.BonusTurbo || checkType == ObjectType.BonusTime || checkType == ObjectType.BonusMoney || checkType == ObjectType.BonusMoneyBig || checkType == ObjectType.BonusMagent || checkType == ObjectType.BonusShield;
    }

    /// <summary>
    /// 在指定位置播放特效
    /// </summary>
    /// <param name="point"></param>
    /// <param name="type"></param>
    public void RequestEffectInPoint(Vector3 point, ObjectType type, bool net = false)
    {
        // 无空闲对象供使用
        if (_freePoolDic[type].Count == 0)
        {
            CreateObj(type);
        }

        GameObject effect = GetRandomObject(type);
        ObjFromFreeToUsed(type, effect);
        if (null == effect)
        {
            return;
        }

        effect.SetActive(true);
        effect.GetOrAddComponent<EffectBehaviour>().SpawnInPoint(point);
        effect.GetOrAddComponent<EffectBehaviour>().effectType = type;
        if (net)
        {
            CmdSpawnObj(effect);
        }
    }

    /// <summary>
    /// 在指定位置播放特效
    /// </summary>
    /// <param name="spawnTransform"></param>
    /// <param name="offset"></param>
    /// <param name="type"></param>
    /// <param name="parentToTransform"></param>
    public void RequestEffect(Transform spawnTransform, Vector3 offset, ObjectType type, bool parentToTransform)
    {
        // 无空闲对象供使用
        if (_freePoolDic[type].Count == 0)
        {
            CreateObj(type);
        }

        GameObject effect = GetRandomObject(type);
        ObjFromFreeToUsed(type, effect);
        if (null == effect)
        {
            return;
        }

        effect.SetActive(true);
        effect.GetOrAddComponent<EffectBehaviour>().Spawn(new EffectBehaviour.EffectSpawnParameters(spawnTransform, offset, parentToTransform));
        effect.GetOrAddComponent<EffectBehaviour>().effectType = type;
    }

    /// <summary>
    /// 从_freePoolDic取对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject RequestObject(ObjectType type)
    {
        // 无空闲对象供使用
        if (_freePoolDic[type].Count == 0)
        {
            CreateObj(type);
        }

        GameObject obj = null;
        obj = GetRandomObject(type);
        ObjFromFreeToUsed(type, obj);
        return obj;
    }

    /// <summary>
    /// 将当前不再使用的对象移到空闲池，并从所有客户端移除
    /// </summary>
    /// <param name="type"></param>
    /// <param name="obj"></param>
    public void NotifyDestroyingParent(GameObject obj, ObjectType type)
    {
        FromUsingToFree(type, obj);
        if (   type == ObjectType.BonusMagic 
            || type == ObjectType.BonusSpeed 
            || type == ObjectType.TrafficVehicle 
            || type == ObjectType.TruckHold 
            || type == ObjectType.TruckTurbo
            || type == ObjectType.BonusGuided 
            || type == ObjectType.BonusLandmine
            || type == ObjectType.BonusTyphoon
            || type == ObjectType.BonusGuidedAttack
            || type == ObjectType.BonusCheckpoint)

            CmdUnSpaenObj(obj);
    }


    public void EffectFinshed(GameObject effect)
    {
        ObjectType effectType = effect.GetComponent<EffectBehaviour>().effectType;
        NotifyDestroyingParent(effect, effectType);
    }

    public void NotifyDestroyingEffect(GameObject obj)
    {
        for (int i = 0; i < obj.transform.childCount;++i )
        {
            EffectBehaviour effectBh = obj.transform.GetChild(i).GetComponent<EffectBehaviour>();
            if (null != effectBh && effectBh.effectType >= ObjectType.CarExplosion && effectBh.effectType <= ObjectType.Effect_UI_Accle)
            {
                NotifyDestroyingParent(obj.transform.GetChild(i).gameObject, effectBh.effectType);
            }
        }
    }

    public void ClearaAll()
    {
        _usingPoolDic.Clear();
        _freePoolDic.Clear();
        _opponentList.Clear();
        _bonusList.Clear();
    }
    #endregion

    #region Command
    [Command]
    private void CmdSpawnObj(GameObject obj)
    {
        NetworkServer.Spawn(obj);
    }

    [Command]
    private void CmdUnSpaenObj(GameObject obj)
    {
        NetworkServer.UnSpawn(obj);
    }

    [Command]
    private void CmdDestroyObj(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }
    #endregion
}
