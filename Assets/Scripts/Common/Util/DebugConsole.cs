/**
 * Copyright (c) 2013,Need Corp. ltd
 * All rights reserved.
 * 
 * 文件名称：DebugConsole.cs
 * 简    述：用于运行中调试，直接输出消息到OnGUI上。
 * 创建标识：Terry.2013/3/30
 */
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;
using System.IO;
using System.Text;

public class DebugConsole : SingletonBehaviour<DebugConsole>
{
    private List<string> linesText;
    private List<string> debugInfos;

    /// <summary>
    /// 判断是否打开Debug显示。
    /// </summary>
    public bool isClose = true;
    /// <summary>
    /// 垂直属性值
    /// </summary>
    private float verticalValue = 0;
    /// <summary>
    /// 水平属性值
    /// </summary>
    //private float horizontalValue = 0.0f;

    Vector2 scrollPos;

    /// <summary>
    /// 当前时间流速是否为 1
    /// </summary>
    private bool IsTimeOne = true;
    // Use this for initialization
    void Start()
    {
    }

    public void Init()
    {
        isClose = true;
        debugInfos = new List<string>();
        linesText = new List<string>();
        AddDebugInfo("Debug输出开始:---");

        StoreNetInfo = new List<byte>();
        Log.Print("streamingAssetsPath:" + Application.streamingAssetsPath);
        Log.Print("dataPath:" + Application.dataPath);
    }
    int enterIndex = 0;
    // Update is called once per frame
    void Update()
    {
    }
     
    //测试主角技能使用变量;
    private string m_skillID = "";

    /// <summary>
    /// 最大行数,Terry
    /// </summary>
    private const int MAX_PRIVATE = 10;
    void OnGUI()
    {
        //是否是开发版本;
        //if (GlobalDef.IsDev == false)
        //{
        //    return;
        //}
        if (isClose)
        {
            if (GUI.Button(new Rect(205, 0, 60, 22), "Debug"))
            {
                isClose = false;
            }
            return;
        }

        if (GUI.Button(new Rect(195, 0, 60, 22), "CloseDb"))
        {
            isClose = true;
        }

        if (GUI.Button(new Rect(260, 0, 50, 22), "Clear"))
        {
            ClearDebugInfo();
        }

        if (GUI.Button(new Rect(430, 0, 75, 22), "SaveFile"))
        {
            SaveLogFile();
        }

        //切换场景;
        m_skillID = GUI.TextField(new Rect(90, 0, 100, 22), m_skillID);
        #region 播放技能动作;
        if (GUI.Button(new Rect(15, 0, 70, 22), "PlaySkill"))
        {
            if (m_skillID == "")
                return;
            int skillID;
            if (int.TryParse(m_skillID, out skillID) == true)
            {
                AddDebugInfo("DebugConsole Not found SkillID:" + skillID);
            }
        }
        #endregion

        #region Slow pause
        if (IsTimeOne)
        {
            if (GUI.Button(new Rect(315, 0, 50, 22), "Slow"))
            {
                Time.timeScale = 0.1f;
                IsTimeOne = false;
            }

            if (GUI.Button(new Rect(370, 0, 50, 22), "Pause"))
            {
                //Debug.LogWarning("____Press Pause____");
                Time.timeScale = 0.0f;
                IsTimeOne = false;
            }

        }
        else
        {
            if (GUI.Button(new Rect(420, 0, 50, 22), "Resume"))
            {
                Time.timeScale = 1.0f;
                IsTimeOne = true;
            }
        }

        #endregion
        //输出Debug信息;
        int maxLine = linesText.Count;
        int startIndex = 0;
        if (maxLine > MAX_PRIVATE)
        {
            verticalValue = GUI.VerticalSlider(new Rect(0, 20, 30, 200), verticalValue, 0, maxLine - MAX_PRIVATE);
            startIndex = (int)verticalValue;
        }
        for (int i = 0; i < MAX_PRIVATE; i++)
        {
            if (startIndex + i < maxLine)
            {
                GUI.Label(new Rect(10, 20 + 21 * i, Screen.width, 20), linesText[startIndex + i]);
            }
            else
            {
                break;
            }
        }
    }

    private string m_LastString = "";
    /// <summary>
    /// 添加输出到本Debug面板
    /// </summary>
    public void AddDebugInfo(string str)
    {
        //str += "\n" + StackTraceUtility.ExtractStackTrace();
        if (m_LastString == str)
        {// 这里可以考虑加上重复的标志; Terry
            return;
        }
        string text = enterIndex + ":" + str;
        enterIndex++;

        debugInfos.Add(text);
        m_LastString = str;

        //NGUIText.WrapText(text, out text);
        //**//text = UITool.FONT.WrapText(text, Screen.width / 14, 0, false, UIFont.SymbolStyle.None);
        string[] strArr = text.Split('\n');
        linesText.AddRange(strArr);
        //if (debugInfos.Count > 50000)
        if (debugInfos.Count > 500)
        {
            debugInfos.RemoveAt(0);
        }
        verticalValue = linesText.Count - MAX_PRIVATE;
    }

    public void ClearDebugInfo()
    {
        linesText.Clear();
    }

    #region Get DebugInfo
    const string DEBUG_INFO = "Debug_Info";
    /// <summary>
    /// 服务器指定的场景唯一ID
    /// </summary>
    const string DEBUG_SCENE_ID = "Debug_Scene_ID";
    /// <summary>
    /// 崩溃角色的职业
    /// </summary>
    const string DEBUG_HERO_JOB = "Debug_Hero_Job";
    /// <summary>
    /// 在发送完成上次奔溃的信息之后，重设信息，这样才能更好的保存当次崩溃的情况。
    /// </summary>
    public void ResetExceptionInfo()
    {
        PlayerPrefs.SetString(DEBUG_INFO, "No Debug Info");
        PlayerPrefs.SetInt(DEBUG_SCENE_ID, -1);
        PlayerPrefs.SetInt(DEBUG_HERO_JOB, 0);
    }

    public void SetExceptionPref(string di)
    {
        PlayerPrefs.SetString(DEBUG_INFO, di);
        {
            PlayerPrefs.SetInt(DEBUG_HERO_JOB, 0);
        }
    }

    /// <summary>
    /// 在启动时可以返回上次登录的服务器id,没有返回-1;
    /// </summary>
    public int GetLastServerZoneId()
    {
        return -1;
    }

    public int GetLastSceneID()
    {
        if (PlayerPrefs.HasKey(DEBUG_SCENE_ID))
        {
            return PlayerPrefs.GetInt(DEBUG_SCENE_ID);
        }
        else
            return -1;
    }

    public int GetLastHeroJob()
    {
        if (PlayerPrefs.HasKey(DEBUG_HERO_JOB))
        {
            return PlayerPrefs.GetInt(DEBUG_HERO_JOB);
        }
        else
            return 0;
    }

    public string GetInfo()
    {
        if (PlayerPrefs.HasKey(DEBUG_INFO))
        {
            return PlayerPrefs.GetString(DEBUG_INFO);
        }
        else
            return "No Debug Info";
    }

    #endregion
    /// <summary>
    /// 保存Debug输出到文本文件;
    /// </summary>
    public void SaveLogFile()
    {
        string filePath = "DebugLog.txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        //1. 创建流;
        Stream stream = File.Open(filePath, FileMode.CreateNew, FileAccess.Write);
        StreamWriter codeWriter = new StreamWriter(stream, Encoding.UTF8);

        for (int i = 0; i < linesText.Count; ++i)
        {
            codeWriter.Write(linesText[i] + "\r\n");
        }

        codeWriter.Flush();
        codeWriter.Close();
        stream.Close();
    }

    /// <summary>
    /// 存储socket连接中接收到的2进制信息。
    /// </summary>
    private List<byte> StoreNetInfo;

    /// <summary>
    /// 添加接受到的网络信息
    /// </summary>
    /// <param name="netbuf"></param>
    public void AddNetBuf(byte[] netbuf)
    {
        StoreNetInfo.AddRange(netbuf);
    }

    public void SaveNetFile()
    {
        string filePath = "NetLog_" + DateTime.Now.ToString("HH_mm_ss") + ".txt";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        File.WriteAllBytes(filePath, StoreNetInfo.ToArray());
    }

}
