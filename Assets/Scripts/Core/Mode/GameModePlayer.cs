/*
 * Copyright (c) 
 * 
 * 文件名称：   GameModePlayer.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/14 9:38:34
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using Need.Mx;

public partial class GameMode : SingletonBehaviour<GameMode>
{

    #region Private Function
    private void UpdatePlayerInput()
    {
      
        // 任何时候
        if(IOManager.Instance.GetIsCoin(0))
        {
            _data.Coin += 1;
            SettingManager.Instance.AddCoin();
            SettingManager.Instance.Save();
            SoundController.Instance.PlaySoundInsertCoin();
            if (Mode == RunMode.Continue && CanPlay())
            {
                ChangePlay();
                TellPlayerSpeedUp();
            }
        }

        // 待机界面
        if (Mode == RunMode.Waiting)
        {
            if (IOManager.Instance.GetIsStart(0))
            {
                if (CanPlay())
                {
                    SoundController.Instance.PlaySoundSure();
                    ChangePlay();
                    Message message = new Message(MessageType.Message_Key_Game_Start, this);
                    message.Send();
                }
            }

            if (IOManager.Instance.GetIsPush1(0))
            {
                ChangeMode(RunMode.Setting);
            }

        }

        // 进入后台
        if (Mode == RunMode.Setting)
        {
            if (IOManager.Instance.GetIsPush1(0))
            {
                Message message = new Message(MessageType.EVNET_SETTING_CONFIRM, this);
                message.Send();
            }

            if (IOManager.Instance.GetIsPush2(0))
            {
                Message message = new Message(MessageType.EVNET_SETTING_SELECT, this);
                message.Send();
            }
        }

        // 进入选择界面
        if (Mode == RunMode.Select)
        {
            if (IOManager.Instance.GetIsRight(0))
            {
                SoundController.Instance.PlaySoundChange();

                Message message = new Message(MessageType.Message_Key_Game_Right, this);
                message.Send();
            }

            if (IOManager.Instance.GetIsLeft(0))
            {
                SoundController.Instance.PlaySoundChange();
                Message message = new Message(MessageType.Message_Key_Game_Left, this);
                message.Send();
            }

            if (IOManager.Instance.GetIsStart(0))
            {
                SoundController.Instance.PlaySoundSure();
                Message message = new Message(MessageType.Message_Key_Game_Start, this);
                message.Send();
            }
        }

        // 游戏开始倒计时
        if (Mode == RunMode.ReadyToRace)
        {

        }
        
        if (Mode == RunMode.SceneEnd)
        {
            if (IOManager.Instance.GetIsRight(0))
            {
                SoundController.Instance.PlaySoundChange();

                Message message = new Message(MessageType.Message_Key_Game_Right, this);
                message.Send();
            }

            if (IOManager.Instance.GetIsLeft(0))
            {
                SoundController.Instance.PlaySoundChange();
                Message message = new Message(MessageType.Message_Key_Game_Left, this);
                message.Send();
            }

            if (IOManager.Instance.GetIsStart(0))
            {
                SoundController.Instance.PlaySoundSure();
                Message message = new Message(MessageType.Message_Key_Game_Start, this);
                message.Send();
            }
        }

        if (Mode == RunMode.Continue)
        {
            if (IOManager.Instance.GetIsStart(0))
            {
                 if (CanPlay())
                 {
                     SoundController.Instance.PlaySoundSure();
                     ChangePlay();
                     TellPlayerSpeedUp();
                 }
            }
        }

        if (Mode == RunMode.Movie)
        {
            if (IOManager.Instance.GetIsStart(0))
            {
                if (CanPlay())
                {
                    SoundController.Instance.PlaySoundSure();
                    ChangePlay();
                }
            }
        }

        // 进入游戏
        if (Mode == RunMode.Racing)
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Message message = new Message(MessageType.Message_Combo, this);
                message["pos"]  = transform.position;
                message.Send();
            }

            if(IOManager.Instance.GetIsRight(0))
            {
                _player.IsRight = true;
            }
            else
            {
                _player.IsRight = false;
            }

            if (IOManager.Instance.GetIsLeft(0))
            {
                _player.IsLeft = true;
            }
            else
            {
                _player.IsLeft = false;
            }

            IOManager.Instance.ResetEvent(0, true);
        }
        else
        {
            IOManager.Instance.ResetEvent(0);
        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    GameMode.Instance.PlayerRef.CmdOnSpawnEngine();
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    GameMode.Instance.PlayerRef.CmdEngine();
        //    GameMode.Instance.PlayerRef.CmdActivateTurbo(8);
        //}

    }
    #endregion

}
