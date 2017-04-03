/*
 * Copyright (c) 
 * 
 * 文件名称：   EndView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/9 17:49:52
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Need.Mx;

public class EndView
{
    public EndModule module;

    public GameObject rankObj;
    // 多人结算UI
    public GameObject rankMulti;
    public List<ItemInfo> itemList;
    public class ItemInfo
    {
        public GameObject itemRoot;
        public Image head;
        public List<Image> distanceList;
        public Image yellow;
    }

    public GameObject effect0;

    public void Init(Transform transform)
    {
        module = ModuleManager.Instance.Get<EndModule>();

        // 多人结算UI
        {
            rankMulti = transform.Find("Rank").gameObject;
            itemList = new List<ItemInfo>();
            for (int k = 0; k < 4;++k )
            {
                ItemInfo info               = new ItemInfo();
                info.distanceList           = new List<Image>();
                info.itemRoot               = rankMulti.transform.Find("RankInfo/Item" + k).gameObject;
                info.head                   = rankMulti.transform.Find("RankInfo/Item" + k + "/Head").GetComponent<Image>();
                info.yellow                 = rankMulti.transform.Find("RankInfo/Item" + k + "/Yellow").GetComponent<Image>();
                for (int j = 0; j < 4; ++j)
                {
                    info.distanceList.Add(rankMulti.transform.Find("RankInfo/Item" + k + "/Number" + j).GetComponent<Image>());
                }
                itemList.Add(info);
            }
        }

        effect0 = transform.Find("Rank/Background/paihangbang_ 1").gameObject;
    }

}
