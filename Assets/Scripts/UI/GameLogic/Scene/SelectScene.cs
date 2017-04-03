/*
 * Copyright (c) 
 * 
 * 文件名称：   SelectScene.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/14 17:21:59
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System;

public class SelectScene : BaseScene
{
    public SelectScene()
    {
        this.AutoRegister = true;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    protected override void OnStateChanged(EnumObjectState newState, EnumObjectState oldState)
    {
        GC.Collect();
    }

    protected override void OnInitScene()
    {
        base.OnInitScene();
    }
}
