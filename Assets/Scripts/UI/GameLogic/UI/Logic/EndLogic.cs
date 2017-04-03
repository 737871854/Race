/*
 * Copyright (c) 
 * 
 * 文件名称：   EndLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/9 17:49:42
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;

public class EndLogic : BaseUI
{
    private EndView view;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelMain;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    private int _distanceRemain;
    private int _comboRemain;
    private int _caoCheRemain;
    private int rank = -1;              // 当前玩家名次
    private int uid = -1;

    private int _chooseId;

    protected override void OnStart()
    {
        view            = new EndView();
        view.Init(transform);
        SoundController.Instance.StartEndMusic();
        InitUI();

        base.OnStart();
    }

    private void InitUI()
    {
        view.effect0.SetActive(true);
        uid = GameMode.Instance.PlayerRef.ID;
        {
            rank = GameMode.Instance.GetRankByID(uid);

            int playerCount = GameMode.Instance.PlayerCount();
            SetItemInfo(playerCount);
            StartCoroutine(ItemMove(playerCount));
        }
    }

    /// <summary>
    /// 设置各个玩家信息
    /// </summary>
    private void SetItemInfo(int count)
    {
        // 设置显示玩家的数量和当前玩家状态
        for (int i = 0; i < view.itemList.Count; ++i)
        {
            if (i >= count)
            {
                view.itemList[i].itemRoot.SetActive(false);
            }
            else
            {
                view.itemList[i].itemRoot.SetActive(true);
            }

            if (rank == i)
            {
                view.itemList[i].yellow.gameObject.SetActive(true);
                view.itemList[i].head.transform.localScale = Vector3.one * 1.2f;
            }
        }

        for (int i = 0; i < count; ++i )
        {
            int id = GameMode.Instance.GetIDByRank(i);
            view.itemList[i].head.sprite = view.module.head1List[id];

            float score = GameMode.Instance.GetScoreByID(id);
            int value0 = (int)(score / 1000);
            int value1 = (int)(score - 1000 * value0) / 100;
            int value2 = (int)(score - 1000 * value0 - 100 * value1) / 10;
            int value3 = (int)(score - 1000 * value0 - 100 * value1 - 10 * value2);
            view.itemList[i].distanceList[0].sprite = view.module.endList[value0];
            view.itemList[i].distanceList[1].sprite = view.module.endList[value1];
            view.itemList[i].distanceList[2].sprite = view.module.endList[value2];
            view.itemList[i].distanceList[3].sprite = view.module.endList[value3];
        }
    }

    IEnumerator ItemMove(int count)
    { 
        int num = 0;
        while (num < count)
        {
            view.itemList[num].itemRoot.transform.DOMoveX(0, 0.3f);
            ++num;
            yield return new WaitForSeconds(0.2f);
        }
        StartCoroutine(ChangeScene());
    }


    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(5);
        SoundController.Instance.StopEndMusic();
        SamplePoolManager.Instance.ClearaAll();
        GameMode.Instance.ModeStart();
    }
}
