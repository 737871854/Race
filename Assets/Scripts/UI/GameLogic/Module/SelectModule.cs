/*
 * Copyright (c) 
 * 
 * 文件名称：   SelectModule.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/14 17:28:06
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class SelectModule : BaseModule
{
    public List<Sprite> _timeList = null;
    //public List<Sprite> _pictureList = null;

    public SelectModule ()
    {
        this.AutoRegister = true;
    }

    protected override void OnLoad()
    {
        _timeList = new List<Sprite>();
        GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteTimeNum)) as GameObject;
        PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        string name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_main_time_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                _timeList.Add(pc.SpriteDic[name]);
            }
        }

        //_pictureList = new List<Sprite>();
        //spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteDistanceEnd)) as GameObject;
        //pc = spriteNumber.GetComponent<PrefabComponent>();
        //pc.Init();
        //name = string.Empty;
        //for (int i = 0; i < pc.SpriteDic.Count; i++)
        //{
        //    name = "ui_settle accounts_distance_" + i.ToString();
        //    if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
        //    {
        //        _pictureList.Add(pc.SpriteDic[name]);
        //    }
        //}

        base.OnLoad();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}
