//
// /**************************************************************************
//
// BattleScene.cs
//
// Author: make
//
//  
//
// Date: 16-6-20
//
// Description:Provide  functions  to connect Oracle
//
// Copyright (c) 2016 make
//
// **************************************************************************/

using System;
using System.Collections.Generic;
using Need.Mx;


public class StartGame : BaseScene
{
    public StartGame()
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
        //if (null != PlayerManager.Instance.GetPlayer())
        //{
        //    PlayerManager.Instance.GetPlayer().gameObject.SetActive(false);
        //}
        base.OnInitScene();
    }
}
