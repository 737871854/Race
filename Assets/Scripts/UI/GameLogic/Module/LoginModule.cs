/*
 * Copyright (c) 
 * 
 * 文件名称：   LoginModule.cs
 * 
 * 简    介:    Login界面数据处理
 * 
 * 创建标识：  Mike 2016/7/28 17:54:50
 * 
 * 修改描述：
 * 
 */

using System;
using UnityEngine;
using System.Collections.Generic;
using Need.Mx;


public class LoginModule : BaseModule
{
    // 通用数字精灵
    public List<Sprite> coinNumList = null;
    public List<Sprite> pleaseList = null;
    

    public int Coin { get; private set; }

    public LoginModule ()
    {
        this.AutoRegister = true;
    }

    protected override void OnLoad()
    {
        // 投币数字
        coinNumList = new List<Sprite>();
        GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteCoinNum)) as GameObject;
        PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        string name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_coin_number" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                coinNumList.Add(pc.SpriteDic[name]);
            }
        }

        pleaseList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpritePlease)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "01_0000s_000" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                pleaseList.Add(pc.SpriteDic[name]);
            }
        }

        base.OnLoad();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }
}
