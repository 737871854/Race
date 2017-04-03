/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingManager.cs
 * 
 * 简    介:    SettingManager数据管理
 * 
 * 创建标识：  Mike 2016/7/28 17:54:50
 * 
 * 修改描述：    2016.9.3 PlayerPrefs转Json
 * 
 */

using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;
using JsonFx.Json;

public class SettingManager : SingletonBehaviour<SettingManager>
{
    private SettingConfigData po;

    private string _checkID         = string.Empty;
    private int _gameRate           = 3;                                // 游戏币率
    private int _gameVolume         = 10;                               // 游戏音量
    private int _gameLanguage       = 0;                                // 游戏语言
    private int _ticketModel        = 0;                                // 出票模式

    private int[] _hasCoin;                                             // 玩家剩余币数

    private List<float[]> _monthList = new List<float[]>();             // 月份信息

    private float[] _totalRecord;                                       // 总记录

    private int _gameTime;  // 游戏时间
    private int _upTime;    // 开机时间
    private int _gameCount; // 游戏次数
    private int _coin;      // 耗币
    private int _addCoin;   // 投币

    public void Init()
    {
        // 取出后台存储所有数据
        this.po = HelperTool.LoadJson(ResUpdateManager.Instance.GetFilePath(GameConfig.SETTING_COINFIG));
        if (null == this.po)
        {
            this.po = new SettingConfigData();
        }
        
        // CheckID
        this._checkID = po.CheckId;
    
        // 游戏币率
        this._gameRate = po.GameRate;

        // 游戏语言版本 0中文 1英文
        this._gameLanguage = po.GameLanguage;

        // 检查点模式
        this._ticketModel = po.TicketModel;

        // 游戏音量
        this._gameVolume = po.GameVolume;

        // 当前剩余币数
        this._hasCoin = po.Coin;

        // 月份信息
        this._monthList = po.MonthList;

        // 总记录
        this._totalRecord = po.TotalRecord;

        // 获取玩家剩余币数
        for (int i = 0; i < GameConfig.GAME_CONFIG_PLAYER_COUNT; i++)
        {
            //PlayerManager.Instance.GetPlayer().ChangeCoin(this._hasCoin[i]);
            GameMode.Instance.ModeData.Coin += this._hasCoin[i];
        }
 
        CheckIsNewMonth();

        GameConfig.ParsingGameConfig();
    }
      
   
    #region  -----------------后台提供借口-----------------------begin------------------
    // 校验ID
    public string CheckID
    {
        get { return _checkID; }
        set { _checkID = value; }
    }

    //获取游戏币率
    public int GameRate
    {
        get { return _gameRate; }
        set { _gameRate = value;}
    }
    //获取游戏音量
    public int GameVolume
    {
        get { return _gameVolume; }
        set { _gameVolume = value; }
    }
 
    //获取游戏语言
    public int GameLanguage
    {
        get { return _gameLanguage; }
        set { _gameLanguage = value; }
    }


    //获取1票需要多少分或出票模式
    public int TicketModel
    {
        get { return _ticketModel; }
        set { _ticketModel = value; }
    }
    
    // 总记录
    public float[] TotalRecord()
    {
        return this._totalRecord;
    }

    #endregion         -----------------后台提供借口-----------------end------------------------

    // 修改po的Coin值
    public void ClearCoin()
    {
        for (int i = 0; i < GameConfig.GAME_CONFIG_PLAYER_COUNT; ++i )
        {
            this._hasCoin[i] = 0;
        }
        GameMode.Instance.ModeData.Coin = 0;
    }

    // 清除月份信息
    public void ClearMonthInfo()
    {
        for (int i = 0; i < this._monthList.Count; i++ )
        {
            for (int j = 1; j < this._monthList[i].Length; j++ )
            {
                this._monthList[i][j] = 0;
            }
        }
    }

    // 清除总记录
    public void ClearTotalRecord()
    {
        for (int i = 0; i < this._totalRecord.Length; i++ )
        {
            this._totalRecord[i] = 0;
        }
    }


    public List<float[]> GetMonthData()
    {
        return this._monthList;
    }

    // 获取指定月份信息
    public float[] GetMonthData(int index)
    {
        return this._monthList[index];
    }

    // 游戏时间
    public void LogGameTimes(int value)
    {
        this._gameTime += value;
    }

    // 开机时间
    public void LogUpTime(int value)
    {
        this._upTime += value;
    }

    // 耗币
    public void LogCoins(int value)
    {
        this._coin += value;
    }

    // 增加游戏次数
    public void LogNumberOfGame(int value)
    {
        this._gameCount += value;
    }

    // 投币
    public void AddCoin()
    {
        this._addCoin += 1;
        this._hasCoin[0] += 1;
    }

   //----------------------------------------------

    // 保存后台信息
    public void Save()
    {
        CopyToPo();
        HelperTool.SaveJson(this.po, ResUpdateManager.Instance.GetFilePath(GameConfig.SETTING_COINFIG));
    }

    public void CopyToPo()
    {
        //// 玩家剩余币数
        //{
        //    for (int i = 0; i < GameConfig.GAME_CONFIG_PLAYER_COUNT; i++)
        //    {
        //        //this._hasCoin[i] = PlayerManager.Instance.GetPlayer().Coin;
        //        this._hasCoin[i] = GameMode.Instance.ModeData.Coin;
        //    }
        //}

        // 修改当期那月份信息
        {
            this._monthList[0][1] += this._coin;
            this._monthList[0][2] += this._gameTime;
            this._monthList[0][3] += this._upTime;
        }

        // 修改总记录
        {
            this._totalRecord[0] += this._gameCount;
            this._totalRecord[1] += this._gameTime;
            this._totalRecord[2] += this._upTime;
            this._totalRecord[3] += this._addCoin;
        }

        // 临时数据清除
        {
            this._gameCount = 0;
            this._gameTime = 0;
            this._upTime = 0;
            this._coin = 0;
            this._addCoin = 0;
        }

        // 自改po的值
        {
            this.po.CheckId = this._checkID;
            this.po.GameRate = this._gameRate;
            this.po.GameLanguage = this._gameLanguage;
            this.po.TicketModel = this._ticketModel;
            this.po.GameVolume = this._gameVolume;
            this.po.MonthList = this._monthList;
            this.po.Coin = this._hasCoin;

            this.po.TotalRecord = this._totalRecord;
            for (int i = 0; i < GameConfig.GAME_CONFIG_PLAYER_COUNT; i++)
            {
                //this.po.Coin[i] = PlayerManager.Instance.GetPlayer().Coin;
            }
        }
    }

    private void CheckIsNewMonth()
    {
        DateTime now = DateTime.Now;
        int month = now.Month;
        bool isNewMonth = true;
        for (int i = 0; i < this._monthList.Count; i++ )
        {
            if (month == this._monthList[i][0])
            {
                isNewMonth = false;
                break;
            }
        }

        if (!isNewMonth)
        {
            return;
        }

        this._monthList[2] = this._monthList[1];
        this._monthList[1] = this._monthList[0];
        for (int i = 0; i < this._monthList[0].Length; i++ )
        {
            this._monthList[0][i] = 0;
        }
        this._monthList[0][0] = month;
    }
  
}
