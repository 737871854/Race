/*
 * Copyright (c) 
 * 
 * 文件名称：   MovieLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/3/21 16:56:31
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class MovieLogic : BaseUI
{
    private MovieTexture movTex;
    private bool isPlay;

    private MovieView view;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelMovie;
    }

    protected override void OnAwake()
    {
        MessageCenter.Instance.AddListener(MessageType.Message_Refresh_Coin, OnCoin);
    }

    protected override void OnRelease()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Refresh_Coin, OnCoin);
    }

    protected override void OnStart()
    {
        EnumMovieType type = (EnumMovieType)GameMode.Instance.MovieID;

        movTex  = ResManager.Instance.Load(MoviePathDefine.GetMoviePathByType(type)) as MovieTexture;
        view    = new MovieView();
        view.Init(transform);
        if (null == movTex)
            Debuger.Log("can not find EnumMovieType.Waitting");
        movTex.loop = false;
        isPlay = false;
        PlayMovie();
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (null == movTex)
            return;

        if (!movTex.isPlaying)
        {
            StopMovie();
        }
    }

    private void StopMovie()
    {
        movTex.Stop();
        view.audioSource.Stop();
        GameMode.Instance.ModeStart();
    }

    private void PlayMovie()
    {
        if (null == movTex)
            return;

        isPlay = true;

        movTex.Play();
        view.audioSource.clip = movTex.audioClip;
        view.audioSource.Play();
        view.rawImage.texture = movTex;
    }

    private void OnCoin(Message message)
    {
        StopMovie();
    }
}
