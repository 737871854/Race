/*
 * Copyright (c) 
 * 
 * 文件名称：   Video.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/3/21 14:12:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Video : MonoBehaviour
{
    private int usedID;
    private Vector3 localPos;
    public List<Camera> cameraList;
    // Use this for initialization
    void Start()
    {
        usedID = -1;
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        ++usedID;
    //        usedID %= (cameraList.Count + 1);
    //        switch (usedID)
    //        {
    //            case 0:
    //                cameraList[0].gameObject.SetActive(true);
    //                break;
    //            case 1:
    //                cameraList[1].gameObject.SetActive(true);
    //                cameraList[0].gameObject.SetActive(false);
    //                break;
    //            case 2:
    //                cameraList[2].gameObject.SetActive(true);
    //                cameraList[1].gameObject.SetActive(false);
    //                localPos = cameraList[2].transform.localPosition;
    //                cameraList[2].transform.SetParent(null);
    //                break;
    //            case 3:
    //                cameraList[2].gameObject.SetActive(false);
    //                cameraList[2].transform.SetParent(transform);
    //                cameraList[2].transform.localPosition = localPos;
    //                cameraList[3].gameObject.SetActive(true);
    //                localPos = cameraList[3].transform.localPosition;
    //                cameraList[3].transform.SetParent(null);
    //                break;
    //            case 4:
    //                cameraList[3].gameObject.SetActive(false);
    //                cameraList[3].transform.SetParent(transform);
    //                cameraList[3].transform.localPosition = localPos;
    //                break;
    //        }
    //    }
    //}
}
