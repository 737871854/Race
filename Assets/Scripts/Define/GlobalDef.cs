/**
* Copyright (c) 2015,Need Corp. ltd;
* All rights reserved.
* 
* 文件名称：GlabalDef.cs
* 简    述：不需要和服务器保持一致的常量和枚举定义存放处。
* 创建标识：Terry  2015/08/10
* 修改标识：
* 修改描述：
*/

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Need.Mx
{
 
    /// <summary>
    /// 当前程序运行的模式
    /// </summary>
    public enum RunMode
    {
        None,
        Waiting,                // 投币界面
        Movie,                  
        Select,                 // 选择地图车辆场景
        Setting,                // 进入后台
        Loading,                // 加载中
        ReadyToRace,            // 预备状态
        Racing,                 // 进行状态
        SceneEnd,               // 当前场景结束
        GameOver,               // 游戏结束
        Continue,
        Dead,
    }
   
    public enum EnvirType
    {
        None = -1,
        Town_Scene,
        Hallowmas_Scene,
        Dester_Scene,
    }

    public enum Hold
    {
        None = -1,
        Hold,
        ExitHold,
    }

    /// <summary>
    /// Game tag.游戏中物体的标记;
    /// </summary>
    public class GameTag
    {
        public const string UntaggedTag     = "Untagged";
        public const string PlayerTag       = "Player";
        public const string MonsterTag      = "Monster";
        public const string DestroyTag      = "Destroy";
        public const string ParticleTag     = "Particle";
        public const string TerrainTag      = "Terrain";
        public const string StreetTag       = "Street";
        public const string BonusTag        = "Bonus";
        public const string EnvirItemTag    = "EnvirItem";
        public const string AirplaneTage    = "Airplane";
        public const string IgnoreRaycast   = "ignore Raycast";
        public const string Traffic         = "Traffic";
        public const string Trucks          = "Trucks";
        public const string Hold            = "Hold";
        public const string LandMine        = "LandMine";
        public const string Guided          = "Guided";
        public const string AI              = "AI";
        public const string NetWorkData     = "NetWorkData";
    }

    /// <summary>
    /// 层名的定义，可以通过层名直接获取层;
    /// </summary>
    public class SceneLayerMask
    {
        //默认;
        public const string Default = "Default";
        //透明;
        public const string TransparentFx = "TransparentFx";
        //用于忽略Raycast射线碰撞检测 Terry
        public const string IgnoreRaycast = "Ignore Raycast";
        //玩家
        public const string Player = "Player";
        //物体透明;
        //public const string ModelTranspant = "ModelTranspant";
        //UI
        public const string UI      = "UI";
        //地图;
        public const string Map     = "Environment";
    }

    public class MonsterName
    {
        public const string None = "None";
     
    }

    public class EffectType
    {
        public const string None  = "None";
     
    }

    public enum EnumTips
    {
        None,
        Avoid,
        OverTake,
        Comb,
        Hold,
        SpeedScale,
        Beat,
        Prefect,
        CheckTime,
    }
}

