/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingScene.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/9/4 10:40:27
 * 
 * 修改描述：
 * 
 */


using System;
using System.Collections.Generic;
using Need.Mx;

public class SettingScene : BaseScene
{
    public SettingScene ()
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
}
