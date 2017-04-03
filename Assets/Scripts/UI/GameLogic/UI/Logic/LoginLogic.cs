/*
 * Copyright (c) 
 * 
 * 文件名称：   LoginLogic.cs
 * 
 * 简    介:    Login界面逻辑处理
 * 
 * 创建标识：   Mike 2016/7/28 17:54:50
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using Need.Mx;

public class LoginLogic : BaseUI
{
    private LoginView view;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelStart;
    }

    protected override void OnStart()
    {
        view = new LoginView();
        view.Init(transform);

        MessageCenter.Instance.AddListener(MessageType.Message_Refresh_Coin, OnRefreshUICoin);
        MessageCenter.Instance.AddListener(MessageType.Message_Key_Game_Start, OnGameStart);
        MessageCenter.Instance.AddListener(MessageType.EVNET_SETTING_CONFIRM, OnOpenSetting);
        
        SoundController.Instance.StartSceneIdleMusic();       
        // LoginLogic界面打开初始化完成后，刷新一次数据
        Message message = new Message(MessageType.Message_View_Is_Open, this);
        message.Send();
        base.OnStart();
    }
  
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Refresh_Coin, OnRefreshUICoin);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Key_Game_Start, OnGameStart);
        MessageCenter.Instance.RemoveListener(MessageType.EVNET_SETTING_CONFIRM, OnOpenSetting);
        SoundController.Instance.StopSceneIdleMusic();
        base.OnRelease();
    }

    /// <summary>
    /// 刷新界面投币数
    /// </summary>
    /// <param name="message"></param>
    private void OnRefreshUICoin(Message message)
    {
        int coin = (int)message["coin"];

        int value0 = (int)((coin / 10) % 10);
        int value1 = (int)((coin / 1) % 10);

        view.coinList[0].sprite = view.loginModule.coinNumList[value0];
        view.coinList[1].sprite = view.loginModule.coinNumList[value1];
        view.coinList[2].sprite = view.loginModule.coinNumList[GameConfig.GAME_CONFIG_PER_USE_COIN];

        view.coinList[0].rectTransform.sizeDelta = view.loginModule.coinNumList[value0].rect.size;
        view.coinList[1].rectTransform.sizeDelta = view.loginModule.coinNumList[value1].rect.size;
        view.coinList[2].rectTransform.sizeDelta = view.loginModule.coinNumList[GameConfig.GAME_CONFIG_PER_USE_COIN].rect.size;

        if (GameMode.Instance.Mode == RunMode.Waiting)
        {
            int remain = GameConfig.GAME_CONFIG_PER_USE_COIN > coin ? (GameConfig.GAME_CONFIG_PER_USE_COIN - coin) : 0;
            view.imagePlease.sprite = view.loginModule.pleaseList[remain];
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Append(view.coinList[0].transform.DOScale(Vector3.one * 1.5f, 0.1f));
        sequence.Join(view.coinList[1].transform.DOScale(Vector3.one * 1.5f, 0.1f));
        sequence.Append(view.coinList[0].transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view.coinList[1].transform.DOScale(Vector3.one, 0.1f));
    }

    private void OnGameStart(Message message)
    {
        StartCoroutine(ChangeScene());
    }

    private void OnOpenSetting(Message message)
    {
        SceneManager.Instance.ChangeSceneDirect(EnumSceneType.SettingScene, EnumUIType.PanelSetting);
        UIManager.Instance.CloseUI(EnumUIType.PanelStart);
    }

    IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(1);
        UIManager.Instance.OpenUICloseOthers(EnumUIType.PanelSelect, null, EnumUIType.PanelStart);
    }
}
