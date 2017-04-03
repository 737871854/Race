/*
 * Copyright (c) 
 * 
 * 文件名称：   Debuger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/29 15:14:57
 * 
 * 修改描述：   通过一个bool参数,实现打印输出信息可控的Dubug的功能，
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public class Debuger
{
    public static bool _enableDebug = GameConfig.DebugMode;

    public static void Log(object message)
    {
        if (_enableDebug)
        {
            Debug.Log(message);
        }
    }

    public static void Log(object message, Object context)
    {
        if (_enableDebug)
        {
            Debug.Log(message, context);
        }
    }

    public static void LogError(object message)
    {
        if (_enableDebug)
        {
            Debug.LogError(_enableDebug);
        }
    }

    public static void LogError(object message, Object context)
    {
        if (_enableDebug)
        {
            Debug.LogError(message, context);
        }
    }

    public static void LogWarning(object message)
    {
        if (_enableDebug)
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogWarning(object message, Object context)
    {
        if (_enableDebug)
        {
            Debug.LogWarning(message, context);
        }
    }
}
