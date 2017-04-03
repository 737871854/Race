/*
 * Copyright (c) 
 * 
 * 文件名称：   HoldTrigger.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/12/21 9:25:13
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class HoldTrigger : MonoBehaviour
{
    public TruckHold truckHold;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == GameTag.PlayerTag)
        {
            if (other.GetComponent<PlayerKinematics>().FState != PlayerKinematics.FlyState.None || other.GetComponent<PlayerKinematics>().TransparentActive || other.GetComponent<PlayerKinematics>().HoldState != Hold.None)
            {
                return;
            }
            truckHold.SendHold(other.GetComponent<PlayerKinematics>().UniqueOrder);
        }

        if (other.tag == GameTag.AI)
        {
            if (other.GetComponent<AIKinematics>().FState != AIKinematics.FlyState.None || other.GetComponent<AIKinematics>().TransparentActive || other.GetComponent<AIKinematics>().HoldState != Hold.None)
            {
                return;
            }
            truckHold.SendHold(other.GetComponent<AIKinematics>().UniqueOrder, true);
        }
    }
}
