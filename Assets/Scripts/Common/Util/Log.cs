/**
* Copyright (c) 2014,Need 
* All rights reserved.
* 
* 文件名称：Log.cs
* 简    述：调试输出的方便函数，用于多人输出调试信息，方便除去自己不关注的调试信息。
* 创建标识：Terry  2012/11/12
*/
using System;
using UnityEngine;
using Need.Mx;

namespace Need.Mx
{
    /// <summary>
    /// Log静态方法类，方便每个前端人员进行调试信息的输出。
    /// 一般情况下请使用自己的名字进行信息输出，比如  Log.Lxt("测试信息输出") , 并将对应的判断条件lxt = true 。
    ///  也可以根据需要将多个判断条件赋值为true, 但日常使用时请不要把这个类对象上传，因为每个人的配置都不一样。
    /// </summary>
    public class Log
    {       
        private static bool hsz = true;

        //private static bool msgTime = true;
        //通用的记录. Terry 2012/11/12
        private static bool commonLog = true;

        public Log()
        {

        }

        /// <summary>
        /// 通用trace函数。一般请不要使用这个函数来进行记录，除非确认前端同事都需要相同调试信息。
        /// </summary>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public static void Trace(object msg)
        {
            if (commonLog)
                Debug.Log(msg);
        }

        public static void Print(object msg)
        {
            if (DebugConsole.Instance != null)
            {
                DebugConsole.Instance.AddDebugInfo((string)msg);
            }
        }

        public static void PrintAndDebug(object msg)
        {
            Print(msg);
            Trace(msg);
        }

        /// <summary>
        /// 记录当前的警告信息，并做为BugReport的信息存储下来。
        /// </summary>
        /// <param name="msg"></param>
        public static void ExceptionPrint(string msg)
        {
            if (DebugConsole.Instance != null)
            {
                DebugConsole.Instance.AddDebugInfo(msg);
                DebugConsole.Instance.SetExceptionPref(msg);
            }
        }
      
        public static void Hsz(object msg)
        {
            if (hsz)
            {
                Debuger.Log("hsz:" + msg);
            }
        }

    }
}