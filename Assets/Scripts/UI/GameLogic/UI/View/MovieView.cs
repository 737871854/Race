/*
 * Copyright (c) 
 * 
 * 文件名称：   MovieView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/3/21 16:56:16
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MovieView
{
    public RawImage rawImage;

    public AudioSource audioSource;

    public void Init(Transform transform)
    {
        rawImage        = transform.Find("RawImage").GetComponent<RawImage>();
        audioSource     = transform.GetComponent<AudioSource>();
    }
}
