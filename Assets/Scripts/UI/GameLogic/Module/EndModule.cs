/*
 * Copyright (c) 
 * 
 * 文件名称：   EndModule.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/8/11 17:28:33
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class EndModule : BaseModule
{
    //public List<Sprite> distanceList;
    public List<Sprite> accountList;
    public List<Sprite> rankList;
    public List<Sprite> endList;
    public List<Sprite> head1List;

    public EndModule()
    {
        this.AutoRegister = true;
    }

    protected override void OnLoad()
    {
        //// 速度数字
        //distanceList = new List<Sprite>();
        //GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteDistanceEnd)) as GameObject;
        //PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        //pc.Init();
        //string name = string.Empty;
        //for (int i = 0; i < pc.SpriteDic.Count; i++)
        //{
        //    name = "ui_settle accounts_distance_" + i.ToString();
        //    if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
        //    {
        //        distanceList.Add(pc.SpriteDic[name]);
        //    }
        //}

        accountList = new List<Sprite>();
        GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteAccount)) as GameObject;
        PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        string name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_settle accounts_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                accountList.Add(pc.SpriteDic[name]);
            }
        }

        rankList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteRankEnd)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_settle accounts_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                rankList.Add(pc.SpriteDic[name]);
            }
        }

        endList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteEnd)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "SpriteEnd" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                endList.Add(pc.SpriteDic[name]);
            }
        }

        head1List = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteHead1)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_head_type1_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                head1List.Add(pc.SpriteDic[name]);
            }
        }

        base.OnLoad();
    }

    protected override void OnRelease()
    {

        base.OnRelease();
    }
}
