/*
 * Copyright (c) 
 * 
 * 文件名称：   LoadingBarModule.cs
 * 
 * 简    介:    LoadingBarModule类
 * 
 * 创建标识：   Mike 2016/7/28 17:57:59
 * 
 * 修改描述：
 * 
 */

using System;
using UnityEngine;
using System.Collections.Generic;
using Need.Mx;


public class LoadingBarModule : BaseModule
{
    // 通用数字精灵
    public List<Sprite> numberList = null;

    public LoadingBarModule()
    {
        this.AutoRegister = true;        
    }

    protected override void OnLoad()
    {
        //numberList = new List<Sprite>();
        //GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteLoadNum)) as GameObject;
        //PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        //pc.Init(); 
        //string name = string.Empty;
        //for (int i = 0; i < pc.SpriteDic.Count; i++)
        //{
        //    name = "ui_loading_number_" + i.ToString();
        //    if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
        //    {
        //        numberList.Add(pc.SpriteDic[name]);
        //    }
        //}
        base.OnLoad();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}
