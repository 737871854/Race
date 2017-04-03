/*
 * Copyright (c) 
 * 
 * 文件名称：   Town_Scene.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/31 16:55:03
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System;

public class Town_Scene : BaseScene
{
    public Town_Scene()
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
