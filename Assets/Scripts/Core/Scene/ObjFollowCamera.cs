/*
 * Copyright (c) 
 * 
 * 文件名称：   MountainiFollow.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/1/13 9:13:04
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;

public class ObjFollowCamera : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject startLine;

    private float dis;
    
    // Use this for initialization
    void Start()
    {
        dis = transform.position.z - mainCamera.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z + dis);
        if (startLine.activeSelf)
        {
            if (GameMode.Instance.PlayerRef != null)
            {
                if (GameMode.Instance.PlayerRef.transform.position.z - startLine.transform.position.z > 100)
                {
                    startLine.SetActive(false);
                }
            }
        }
    }
}
