/*
 * Copyright (c) 
 * 
 * 文件名称：   EffectGunConnectBehaviour.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/19 10:23:22
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class EffectGunConnectBehaviour : MonoBehaviour
{
    public GameObject Left_Light;
    public GameObject Right_Light;
    public GameObject Left_Ball;
    public GameObject Right_Ball;
    public GameObject Left_Point;
    public GameObject Right_Point;

    // Update is called once per frame
    void Update()
    {
        Left_Ball.transform.position = Left_Point.transform.position;
        Right_Ball.transform.position = Right_Point.transform.position;

        if (null != GameMode.Instance.PlayerRef)
        {
            GameMode.Instance.PlayerRef._leftGun.transform.position = Left_Point.transform.position + new Vector3(0, 0, 0.5f);
            GameMode.Instance.PlayerRef._rightGun.transform.position = Right_Point.transform.position + new Vector3(0, 0, 0.5f);
        }
    }
}
