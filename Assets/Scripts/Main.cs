/**
* 	Copyright (c) 2014 Need co.,Ltd
*	All rights reserved
*		
*	文件名称:	Main.cs	
*	简    介:	整个游戏的进入接口，继承于MonoBehaviour,挂接的GameObject不能销毁。
*	创建标识:	Terry 2015/08/11
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using CodeStage.AdvanecedFPSCounter;
using Need.Mx;

public delegate void delegateUpdate(float time, float deltaTime);
/// <summary>
/// 控制游戏程序的进入和退出
/// </summary>
public class Main : MonoBehaviour
{
    public static GameObject Container;
    public static NonStopTime NonStopTime;
    private static float curTime;
    private static float updateFix;
    private Message messagePre;
    private Message messageFix;
    private static float updateLog;

    void Awake()
    {
        InitUnity();
        InitGame();
    }

    void Start()
    {
        InitOption();
        InitUI();
    }

    void OnApplicationQuit()
    {
         
    }

    /// <summary>
    /// 初始化UnitEngine.Application的一些属性
    /// </summary>
    void InitUnity()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        //保证即使被切换到苹果后台也不会断线;即使这样设置也不能保证 苹果有后台
        Application.runInBackground = true;
        //注册log回调函数，主要为了错误时可以关闭Socket，不造成Untiy崩溃
        //Application.RegisterLogCallback(ProcessExceptionReport);
    }

    /// <summary>
    /// 核心内容，初始化网络NetWork,游戏配置数据初始化
    /// </summary>
    void InitGame()
    {
        Container = gameObject;
        DontDestroyOnLoad(Container);
        NonStopTime = Container.GetComponent<NonStopTime>();

        // 初始化动画插件(需要注意，这是全局的动画系统，全局设置时不会自动消耗
        // 所有需要主要在游戏切换的时候记得统一销毁
        DOTween.Init(true, true, LogBehaviour.Verbose).SetCapacity(1000,0);
    }

    void InitUI()
    {
        
    }
   
    /// <summary>
    /// 初始化配置值;
    /// </summary>
    void InitOption()
    {
        // 初始化输入设备
        IOManager.Instance.Init(GameConfig.GAME_CONFIG_PLAYER_COUNT);
        GameMode.Instance.GameInputType(GameMode.InputType.Builtin);

        // 初始化声音
        SoundManager.Instance.Init();
        SoundController.Instance.Init();
        InitMode();
    }

    /// <summary>
    /// 初始化当前模式状态;在这里注册全部的网络监听消息;
    /// </summary>
    void InitMode()
    {
        SettingManager.Instance.Init();
        updateFix = 0.0f;

        ModuleManager.Instance.RegisterAllModules();
        SceneManager.Instance.RegisterAllScene();

        GameMode.Instance.ModeStart();

    }

    void Update()
    {
        try
        {
            UpdatePreFrame();
            UpdateSave();
        }
        catch (Exception e)
        {
            Log.Print("Update Exception StackTrace : " + e.StackTrace);
            Debug.LogError("Update Exception StackTrace : " + e.StackTrace);
        }
    }

    void FixedUpdate()
    {
        try
        {
            UpdateFixFrame();
        }
        catch (Exception e)
        {
            Log.Print("FixedUpdate Exception StackTrace : " + e.StackTrace);
            Debug.LogError("FixedUpdate Exception StackTrace : " + e.StackTrace);
        }
    }

    void UpdateSave()
    {
        updateLog += Time.deltaTime;
        if (updateLog >= 2.0f)
        {
            if (SceneManager.Instance.ChangeSceneType != EnumSceneType.LoginScene && SceneManager.Instance.ChangeSceneType != EnumSceneType.SettingScene)
            {
                SettingManager.Instance.LogGameTimes(2);
            }
            SettingManager.Instance.LogUpTime(2);
            updateLog = 0.0f;
        }
    }

    void UpdatePreFrame()
    {
        messagePre = new Message(MessageType.Message_Update_Pre_Frame, this);
        MessageCenter.Instance.SendMessage(messagePre);
    }

    void UpdateFixFrame()
    {
        messageFix = new Message(MessageType.Message_Update_Fix_Frame, this);
        MessageCenter.Instance.SendMessage(messageFix);
    }

    /// <summary>
    /// 处理抛出的错误
    /// </summary>
    private void ProcessExceptionReport(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            Log.Print("ExceptionReport:" + condition);
        }
    }

    //修改为私有的，在注册函数里面管理一下;TODO:以下代码考虑之后删除，不用这种Register的方式来注册和移除;
    protected static List<delegateUpdate> updateList = new List<delegateUpdate>();
    //注册更新函数;
    public static void RegisterUpdateCallback(delegateUpdate callback)
    {
        if (updateList.Contains(callback))
        {
            Debug.LogWarning("Main.cs, RegisterUpdateCallback: Sorry, Main's updatelist has contains this item:" + callback.ToString());
            return;
        }
        updateList.Add(callback);
    }

    //反注册更新函数;	
    public static void UnRegisterUpdateCallback(delegateUpdate callback)
    {
        if (!updateList.Contains(callback))
        {
            Debug.LogWarning("Main.cs, UnRegisterUpdateCallback: Sorry, Main's updatelist has not contains this item:" + callback.ToString());
            return;
        }
        updateList.Remove(callback);
    }
   
}