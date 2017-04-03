/*
 * Copyright (c) 
 * 
 * 文件名称：   ModeMission.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/13 15:44:39
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public partial class GameMode : SingletonBehaviour<GameMode>
{
    #region private menmbers
    private PlayerKinematics _player;           // 玩家的引用
    private PlayerKinematics _oldPlayer;
    private float _turboRatio = -1;

    #endregion

    #region public properties
    public bool IsTurboActive               { get { return _turboRatio >= 0.0f; } }
    #endregion

    #region Private Function
    void OnLevelWasLoaded(int level)
    {
        switch (EnvirManager.Instance.CurrentEnvirType)
        {
            case EnvirType.Hallowmas_Scene:
                break;
            case EnvirType.Town_Scene:
                break;
            case EnvirType.Dester_Scene:
                break;
        }
        
        RunMode newMode = _runMode;

        if (level ==4 || level == 5 || level == 6)
        {
            InitialGameDifficulty();

            UIManager.Instance.OpenUICloseOthers(EnumUIType.PanelMain, null, EnumUIType.PanelSelect);
            SamplePoolManager.Instance.InitilizePool();
            SpawnManager.Instance.Init();
            if (ModeSelected == SelectMode.SinglePlayer)
            {
                for (int i = 0; i < 4; ++i )
                {
                    if (i != ChooseCharacterID)
                        SpawnManager.Instance.CreateAI(i);
                }
            }
        }
        //else if (level == 8)
        //    StartCoroutine(LoadPlayScene());

        ChangeMode(newMode);
    }

   
    /// <summary>
    ///  随机播放狼嚎或猫头鹰叫
    /// </summary>
    private void PlayeWolfAndWolSound()
    {
        if (EnvirManager.Instance.CurrentEnvirType != EnvirType.Hallowmas_Scene)
            return;

        if (_randWolf > 0)
            _randWolf -= Time.fixedDeltaTime;
        else
        {
            _randWolf = Util.Random(40, 60);
            SoundController.Instance.PlaySoundWolf();
        }

        if (_randWol > 0)
            _randWol -= Time.fixedDeltaTime;
        else
        {
            _randWol = Util.Random(30, 60);
            SoundController.Instance.PlaySoundOwl();
        }
    }

    /// <summary>
    /// 初始化游戏难度
    /// </summary>
    private void InitialGameDifficulty()
    {
        for (int i = 0; i < GameConfig.GAME_CONIFIG_CHECK_POINT.Length; ++i)
        {
            GameConfig.GAME_CONIFIG_CHECK_POINT[i] = SettingConfig.CheckPoint[LobbyManager.Instance.LobbyPlayer.GameDifficulty, i];
        }
    }

    /// <summary>
    /// 开启普通特效
    /// </summary>
    private void ActivateNormalEffect()
    {
        for (int i = 0; i < AIList.Count; ++i )
            AIList[i].ActivateGOEffect(); ;

        for (int i = 0; i < LobbyManager.Instance.PKList.Count; ++i)
            LobbyManager.Instance.PKList[i].ActivateGOEffect();
    }
  
    /// <summary>
    /// 发送投币消息
    /// </summary>
    private void SendCoinMsg()
    {
        Message message = new Message(MessageType.Message_Refresh_Coin, this);
        message["coin"] = _data.Coin;
        message.Send();
    }

    /// <summary>
    /// 检查点
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    private bool CheckPlayerPass(Step step)
    {
       if (GameMode.Instance.PlayerRef.Distance < GameConfig.GAME_CONIFIG_CHECK_POINT[(int)step])
           return false;
       else
           return true;
    }

    IEnumerator OpenStartFire()
    {
        for (int i = 0; i < PlayerCount(); ++i )
        {
            OpenStartFireByOrder(i);
            yield return new WaitForSeconds(0.4f);
        }
    }

    /// <summary>
    /// 检查点
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool CheckAIPass(int index)
    {
        int step = AIList[index].CheckStep;
        if (AIList[index].Distance < GameConfig.GAME_CONIFIG_CHECK_POINT[step])
            return false;
        else
            return true;
    }

    /// <summary>
    /// 检查点
    /// </summary>
    private void UpdateCheckPoint()
    {
        bool pass = false;
        if (StepStage < Step.Step3)
        {
            pass = CheckPlayerPass(StepStage);
            if (_remainTime > 0)
            {
                _remainTime = TotalTime - GameConfig.GAME_CONFIG_MAX_LIFE_TIME + LobbyManager.Instance.LobbyPlayer.LifeValue;
                if (pass)
                {
                    //GameMode.Instance.AddScore(GameMode.Instance.PlayerRef.ID, (int)_remainTime);
                    NexStepState();
                }
            }
            else
            {
                _remainTime = 0;
                GameMode.Instance.ChangeMode(RunMode.Continue);
            }
        }
        else
        {
            if (_remainTime > 0)
                _remainTime = LobbyManager.Instance.LobbyPlayer.LifeValue;
            else
                _remainTime = 0;
        }

        for (int i = 0; i < AIList.Count; ++i)
        {
            if (AIList[i].CheckStep < 3)
            {
                pass = CheckAIPass(i);
                if (pass)
                {
                    int step = AIList[i].CheckStep;
                    int temp = (int)(GameConfig.GAME_CONFIG_STEP_TIME[step] - GameConfig.GAME_CONFIG_MAX_LIFE_TIME + LobbyManager.Instance.LobbyPlayer.LifeValue);
                    if (temp > 0)
                    {
                        //GameMode.Instance.AddScore(AIList[i].ID, temp);
                    }
                    AIList[i].NextCheckStep();
                }
            }
        }
    }

    /// <summary>
    /// 让玩家启动引擎效果
    /// </summary>
    private void TellPlayerSpeedUp()
    {
        if (_player != null)
        {
            _player.CmdEngine();
            _player.ActivateTurbo(8);
        }
    }

    #endregion

    //IEnumerator LoadPlayScene()
    //{
    //    yield return new WaitForEndOfFrame();
    //    switch (EnvirManager.Instance.CurrentEnvirType)
    //    {
    //        case EnvirType.Town_Scene:
    //            SceneManager.Instance.ChangeScene(EnumSceneType.Town_Scene, EnumUIType.PanelMain);
    //            break;
    //        case EnvirType.Dester_Scene:
    //            SceneManager.Instance.ChangeScene(EnumSceneType.Sea_Scene, EnumUIType.PanelMain);
    //            break;
    //        case EnvirType.Hallowmas_Scene:
    //            SceneManager.Instance.ChangeScene(EnumSceneType.Hallowmas_Scene, EnumUIType.PanelMain);
    //            break;
    //    }
    //}

    IEnumerator InitializePlayerPos()
    {
        yield return new WaitForEndOfFrame();
        GameObject obj = GameObject.Find("StartPostion").gameObject;
        Vector3[] startPos = new Vector3[obj.transform.childCount];

        for (int i = 0; i < obj.transform.childCount; ++i)
            startPos[i] = obj.transform.GetChild(i).transform.position;

        GameMode.Instance.PlayerRef.transform.position = startPos[LobbyManager.Instance.LobbyPlayer.Order];
    }

}
