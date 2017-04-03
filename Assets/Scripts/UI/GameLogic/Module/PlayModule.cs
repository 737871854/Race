using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;

public class PlayModule : BaseModule
{
    // 通用数字精灵
    public List<Sprite> timeNumList = null;
    public List<Sprite> coinNumList = null;
    public List<Sprite> speedNumList = null;
    public List<Sprite> speedScaleNumList = null;
    public List<Sprite> distanceNumList = null;
    public List<Sprite> comboNumList = null;
    public List<Sprite> countDownList = null;
    public List<Sprite> magicList = null;
    public List<Sprite> pleaseList = null;
    public List<Sprite> rankNumList = null;
    public List<Sprite> orderNumList = null;
    public List<Sprite> addTimeList = null;
    public List<Sprite> head1List = null;
    public List<Sprite> head0List = null;
    public List<Sprite> greatList = null;

    public PlayModule()
    {
        this.AutoRegister = true;
    }

    protected override void OnLoad()
    {
        // 速度数字
        speedNumList = new List<Sprite>();
        GameObject spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteSpeed)) as GameObject;
        PrefabComponent pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        string name = string.Empty;
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_main_velocity_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                speedNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 时间数字
        timeNumList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteTimeNum)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_main_time_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                timeNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 加速倍数
        speedScaleNumList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteSpeedScale)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_prompt_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                speedScaleNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 投币数字
        coinNumList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteCoinNum)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "ui_coin_number" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                coinNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 运行距离数字
        distanceNumList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteEnd)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++)
        {
            name = "SpriteEnd" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                distanceNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 击飞次数数字
        comboNumList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteCombo)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
        for (int i = 0; i < pc.SpriteDic.Count; i++ )
        {
            name = "ui_combo_" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                comboNumList.Add(pc.SpriteDic[name]);
            }
        }

        // 倒计时
        countDownList = new List<Sprite>();
        spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteCountDown)) as GameObject;
        pc = spriteNumber.GetComponent<PrefabComponent>();
        pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++ )
        {
            name = "ui_main_count down" + i.ToString();
            if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
            {
                countDownList.Add(pc.SpriteDic[name]);
            }
        }
    
       magicList = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteProp)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "ui_prop_magic" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               magicList.Add(pc.SpriteDic[name]);
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

       rankNumList = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteRank)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "ui_head portrait_" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               rankNumList.Add(pc.SpriteDic[name]);
           }
       }

       orderNumList = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteOrder)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "SpriteOrder" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               orderNumList.Add(pc.SpriteDic[name]);
           }
       }

       addTimeList = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteAddTime)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "SpriteAddTime" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               addTimeList.Add(pc.SpriteDic[name]);
           }
       }

       head0List = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteHead0)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "ui_head_type0_" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               head0List.Add(pc.SpriteDic[name]);
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

       greatList = new List<Sprite>();
       spriteNumber = ResManager.Instance.Load(SpritePathDefine.GetPrefabPathByType(EnumSpriteType.SpriteGreat)) as GameObject;
       pc = spriteNumber.GetComponent<PrefabComponent>();
       pc.Init();
       for (int i = 0; i < pc.SpriteDic.Count; i++)
       {
           name = "SpriteGreat" + i.ToString();
           if (null != pc.SpriteDic && pc.SpriteDic.ContainsKey(name))
           {
               greatList.Add(pc.SpriteDic[name]);
           }
       }

        base.OnLoad();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }   

}
