/*
 * Copyright (c) 
 * 
 * 文件名称：   SmokeShow.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/3/18 15:50:06
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class SmokeShow : MonoBehaviour
{
    public ParticleSystem effect;
    private float showTime;
    // Use this for initialization
    // Update is called once per frame
    void Update()
    {
        if (showTime > 0)
        {
            showTime -= Time.deltaTime;
        }
        else
        {
            showTime = 50;
            effect.Play(true);
        }
    }
}
