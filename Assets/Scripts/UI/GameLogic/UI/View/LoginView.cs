/*
 * Copyright (c) 
 * 
 * 文件名称：   LoginView.cs
 * 
 * 简    介:    Login界面UI数据存储
 * 
 * 创建标识：   Mike 2016/7/28 17:54:50
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Need.Mx;


public class LoginView
{
    public LoginModule loginModule;
   
    // 要显示的投币数
    public List<Image> coinList = null;

    public Image imagePlease;

    public void Init(Transform transform)
    {
        loginModule = ModuleManager.Instance.Get<LoginModule>();
        loginModule.Load();

        Image image = null;
        coinList    = new List<Image>();
        for (int i = 0; i < transform.Find("Coin/Num").transform.childCount; i++)
        {
            image = transform.Find("Coin/Num/Image_Number" + i.ToString()).GetComponent<Image>();
            coinList.Add(image);
        }

        imagePlease = transform.Find("Please/Number").GetComponent<Image>();
    }

}
