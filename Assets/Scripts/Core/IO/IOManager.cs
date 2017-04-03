/**
* Copyright (c) 2012,广州纷享游艺设备有限公司
* All rights reserved.
* 
* 文件名称：IOManager.cs
* 简    述：获取模拟器操作信息
* 创建标识：meij  2015/10/28
* 修改标识：meij  2015/11/10
* 修改描述：采用1个COM端口进行协议收发。
*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Need.Mx;

public class IOManager : SingletonBehaviour<IOManager>
{
    private bool[] isSetGameEnd;
    private int ioCount;
    private SerialIOHost serialIoHost;
    private IOEvent ie;
    private IOEvent[] ioEvent;                       //协议信息数组   每帧最多包含MAX_PLAYER_NUMBER个协议数,
    private bool[] playerGameBegine;
    private bool[] playerGameEnd;
    private bool isfindport;
    private bool cansearch = true;
    private float checkTime;
    private float ticktime = 1.0f;
    private float searchcount;
    private bool checkPass;
    private bool hasCheck;
    private bool connectDown;
    private float time;
    private bool ioCheckPass;

    // IO通信检测
    public bool[] IOCheck
    {
        get;
        set;
    }

    private bool[] isFirstTicket;
    private bool[] hasRespone;
    private float[] ticketTime;

    public bool IsFindPort()
    {
        return isfindport;
    }

    public bool IOCheckPass
    {
        get { return ioCheckPass; }
        set { ioCheckPass = value; }
    }

    private int connectCount = 0;
    public int GetConnectCount()
    {
        return connectCount;   
    }
    public void SetConnnectCount()
    {
        ++connectCount;
    }

    // 端口读取是否正常
    private float connectTime = 2;
    private bool isConnect = true;
    public bool IsConnect
    {
        get { return isConnect; }
        set { isConnect = value; }
    }

    private bool receive5A = false;
    public bool Receice5A
    {
        get { return receive5A; }
        set { receive5A = value; }
    }
    private string findPort = "空";
    public void SetFindPort(string port)
    {
        findPort = findPort + port;
    }
    public string GetFindProt()
    {
        return findPort;
    }
   

    private bool beginFind = false;
    public bool BeginFind
    {
        get { return beginFind; }
        set { beginFind = value; }
    }


    private byte[] byteHost = new byte[14];
    // 每帧中每条协议对应一个角色信息)
    #region ----------------------获取当前帧中，对应Player的协议信息-----------------------------------
    public byte[] ByteHost
    {
        get { return byteHost; }
        set { byteHost = value; }
    }

    private byte[] byteHostAA = new byte[14];
    public byte[] ByteHostAA
    {
        get { return byteHostAA; }
        set { byteHostAA = value; }
    }

    public float CheckTime
    {
        get { return checkTime; }
        set { checkTime = value; }
    }

    public  byte GetPlayerID(int i)
    {
        return ioEvent[i].ID;
    }

    public bool GetIsOutTicket(int i)
    {
        return ioEvent[i].IsOutTicket;
    }

    public bool GetIsCoin(int i)
    {
        return ioEvent[i].IsCoin;
    }

    public bool GetIsStart(int i)
    {
        return ioEvent[i].IsStart;
    }

    public bool GetIsPush1(int i)
    {
        return ioEvent[i].IsPush1;
    }

    public bool GetIsPush2(int i)
    {
        return ioEvent[i].IsPush2;
    }

    public bool GetIsLeft (int i)
    {
        return ioEvent[i].IsLeft;
    }

    public bool GetIsRight (int i)
    {
        return ioEvent[i].IsRight;
    }

    public bool CheckPass
    {
        set { checkPass = value; }
        get { return checkPass; }
    }

    public bool HasCheck
    {
        set { hasCheck = value; }
        get { return hasCheck; }
    }

    //重置指定角色操作数据
    public void ResetEvent(int i, bool isRacing = false)
    {
        ioEvent[i].IsCoin       = false;
        ioEvent[i].IsOutTicket  = false;
        ioEvent[i].IsPush1      = false;
        ioEvent[i].IsPush2      = false;
        ioEvent[i].IsStart      = false;
        if (!isRacing)
        {
            ioEvent[i].IsRight = false;
            ioEvent[i].IsLeft = false;
        }
       
    }
    #endregion

    #region ----------------设置下发协议信息-----------------------------
    //设置对应玩家是否游戏开始（硬件打开对应的水阀）
    public void SetPlayerGameBegine(int index, bool value)
    {
        playerGameBegine[index] = value;
    }
    //指定玩家游戏结束（关闭该玩家水阀）
    public void SetPlayerGameEnd(int index, bool value)
    {
        playerGameEnd[index] = value;
    }
   
    //整个游戏结束（关闭所有玩家水阀）
    public void SetGameEnd()
    {
        for (int i = 0; i < ioCount;i++ )
        {
            playerGameEnd[i] = true;
        }
    }

    //控制条件（为了只触发一次结束条件）
    public bool GetIsSetGameEnd(int index)
    {
        return isSetGameEnd[index];
    }
    public void SetIsSetGameEnd(int index, bool value)
    {
        isSetGameEnd[index] = value;
    }
    #endregion ---------------------------------------------
    public SerialIOHost GetSerialIoHost(int i)
    {
        return serialIoHost;
    }

    /// <summary>
    /// 应用程序退出操作
    /// </summary>
    public void Close()
    {
        if (serialIoHost != null)
        {
            for (int i = 0; i < GameConfig.GAME_CONFIG_PLAYER_COUNT; i++)
            {
                IOManager.Instance.SetPlayerGameBegine(i, false);
             }
            serialIoHost.Close();
            }    
    }

    public void ResetIO()
    {
        if (null != serialIoHost)
        {
            serialIoHost.Close();
            cansearch = true;
            isfindport = false;
            searchcount = 0;
            serialIoHost = null;
            serialIoHost = new SerialIOHost();
        }
    }

    //初始化操作，打开端口，初始化ioEvent数组
    public void Init(int portCount)
    {
        ioCount          = portCount;
        ioEvent          = new IOEvent[portCount];
        playerGameBegine = new bool[portCount];
        playerGameEnd    = new bool[portCount];
        isSetGameEnd     = new bool[portCount];
        checkPass        = true;
        hasCheck         = true;
        connectDown      = false;
        ioCheckPass      = false;

        isFirstTicket = new bool[portCount];
        ticketTime    = new float[portCount];
        hasRespone    = new bool[portCount];

        serialIoHost = new SerialIOHost();
        for (byte i = 0; i < ioCount; i++)
        {
            ioEvent[i]    = new IOEvent();
        }
             
    }

    private void ReadDatas()
    {

        if (!ioCheckPass)
        {
            byte data = serialIoHost.Read(5);
            bool[] check = new bool[7];
            check[0] = (data & 0x80) != 0;
            check[1] = (data & 0x40) != 0;
            check[2] = (data & 0x20) != 0;
            check[3] = (data & 0x10) != 0;
            check[4] = (data & 0x8) != 0;
            check[5] = (data & 0x4) != 0;
            check[6] = (data & 0x2) != 0;

            IOCheck = check;
        }
        else
        {
            int num = 0;
            for (byte i = 0; i < ioCount; i++)
            {
                byte data = serialIoHost.Read(num);
                ioEvent[i].ID = data;
                data = serialIoHost.Read(num + 1);

                ioEvent[i].IsCoin       = (data & 0x80) != 0;
                ioEvent[i].IsOutTicket  = (data & 0x40) != 0;
                ioEvent[i].IsPush1      = (data & 0x20) != 0;
                ioEvent[i].IsPush2      = (data & 0x10) != 0;
                ioEvent[i].IsStart      = (data & 0x8) != 0;
                ioEvent[i].IsRight      = (data & 0x4) != 0;
                ioEvent[i].IsLeft       = (data & 0x2) != 0;
                num += 4;
            }
        }
    }

    /// <summary>
    /// 每帧分别读取一次协议队列中的协议，并存入协议信息类对象数组ioEvent
    /// </summary>
    public void UpdateIOEvent()
    {
        #region id校验，重连
        //if (null != idString)
        //{
        //    SettingManager.Instance.CheckID = idString;
        //    SettingManager.Instance.Save();
        //    idString = null;
        //}

        #region MyRegion
        if (isfindport)
        {
            // 丢失连接
            if (!isConnect)
            {
                if (connectTime > 0)
                {
                    connectTime -= Time.deltaTime;
                }
                else
                {
                    isfindport = false;
                    connectTime = 2.0f;
                    IOManager.Instance.SetFindPort("空");
                    IOManager.Instance.ResetIO();
                }
            }
            else
            {
                connectTime = 2.0f;
            }
        }

        if (cansearch && !isfindport && searchcount < 50)
        {
            beginFind = true;

            if (!serialIoHost.Setup(14, 8, 10))
            {
                isfindport = false;
                Debug.Log("SerialIO setup failed!!");
            }
            else
            {
                isfindport = true;
                serialIoHost.OnReceiveFailed += () => { Debug.Log("Read Failed !!!"); };
                serialIoHost.OnReceiveSucceed += ReadDatas;
            }

            cansearch = false;
            ++searchcount;
        }

        if (!isfindport)
        {
            time += Main.NonStopTime.deltaTime;
        }

        if (time >= ticktime)
        {
            cansearch = true;
            time = 0.0f;
        }
        #endregion
        #endregion
        
        if(serialIoHost != null && serialIoHost.HadDevice())
        {
            serialIoHost.UpdateIOHost();
         
            //向下位机发协议
            int num = 0;
            for (byte i = 0; i < ioCount;i++ )
            {
                serialIoHost.Write(num, i);
                serialIoHost.Write(num + 1, 7, playerGameBegine[i]);
                serialIoHost.Write(num + 1, 6, playerGameEnd[i]);

                if (GameConfig.GAME_COINFIG_TEST)
                {
                    serialIoHost.Write(num + 1, 5, true);
                }
                else
                {
                    serialIoHost.Write(num + 1, 5, false);
                }

                num += 2;
            }
        }            
     } 

    /// <summary>
    /// 键盘控制
    /// </summary>
    public void UpdateInputEvent()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ioEvent[0].IsCoin = true;
        }
        
        if (Input.GetKeyDown(KeyCode.F3))
        {
            ioEvent[0].IsStart = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ioEvent[0].IsLeft = true;
        }else if (Input.GetKeyUp(KeyCode.A))
        {
            ioEvent[0].IsLeft = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ioEvent[0].IsRight = true;
        }else if (Input.GetKeyUp(KeyCode.D))
        {
            ioEvent[0].IsRight = false;
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            ioEvent[0].IsPush1 = true;
        }
        else if (Input.GetKeyUp(KeyCode.PageUp))
        {
            ioEvent[0].IsPush1 = false;
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            ioEvent[0].IsPush2 = true;
        }else if (Input.GetKeyUp(KeyCode.PageUp))
        {
            ioEvent[0].IsPush2 = false;
        }
    }
}

