/*
 * Copyright (c) 
 * 
 * 文件名称：   Hallowmas_Scene.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/31 16:56:17
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections.Generic;
using Need.Mx;
using System;

public class Hallowmas_Scene : BaseScene
{
    public Hallowmas_Scene()
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
    }
}
