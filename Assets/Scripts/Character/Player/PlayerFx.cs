/*
 * Copyright (c) 
 * 
 * 文件名称：   PlayerFx.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/28 10:56:11
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;
using System.Collections.Generic;

public class PlayerFx : MonoBehaviour
{

    #region Public Members
    public Transform Effect_Point_SpeedLine;
    public Transform Effect_Point_SpeedWind;
    public Transform Effect_Point_Engine_Right;
    public Transform Effect_Point_Engine_Left;
    public Transform Effect_Point_Magent;
    public Transform Effect_Point_Connect;
    public Transform Effect_Point_Side;
    public Transform Effect_Point_Leopard_Acc;
    public Transform Effect_Point_Middle_Engine;
    public Transform Effect_Point_Trial;
    public Transform Effect_Point_Eat_Prop;
    public Transform Effect_Point_Eat_Follow;
    public Transform Effect_Point_QiLiu;
    public Transform Effect_Point_Engine_SpeedUp;
    public Transform Effect_Point_LunTai;
    public Transform Effect_Ready_Go;
    public Transform Effect_Car_Change_To_Leopard;


    public bool IsAI;
    #endregion

    #region Protected Members
    protected PlayerKinematics kin;
    protected AIKinematics ai;

    #endregion

    // 没时间配表，直接上，如果不同车辆配置不同特效，还是需要配表
    void Start()
    {
        if (IsAI)
        {
            ai = GetComponent<AIKinematics>();
        }
        else
        {
            kin = GetComponent<PlayerKinematics>();
        }
     
    }

    public void CloseAllEffect()
    {
        Effect_Point_SpeedLine.parent.gameObject.SetActive(false);
    }

    /// <summary>
    ///  机枪连接特效
    /// </summary>
    /// <param name="active"></param>
    public void ActivaterGunFx(bool active)
    {
        Effect_Point_Connect.gameObject.SetActive(active);
    }

    /// <summary>
    /// 迟到道具播放一次
    /// </summary>
    public void ActivateTriggerProp()
    {
        Effect_Point_Eat_Prop.gameObject.SetActive(true);
        for (int i = 0; i < Effect_Point_Eat_Prop.childCount; ++i )
        {
            if (Effect_Point_Eat_Prop.GetChild(i).GetComponent<ParticleSystem>() != null)
            {
                Effect_Point_Eat_Prop.GetChild(i).GetComponent<ParticleSystem>().Play(true);
            }
        }
    }

    /// <summary>
    /// 播放吃道具后状态特效
    /// </summary>
    /// <param name="lastTime"></param>
    public void ActivatePropFollow(float lastTime)
    {
        Effect_Point_Eat_Follow.gameObject.SetActive(true);
        StartCoroutine(ClosePropFollow(lastTime));
    }

    /// <summary>
    /// 加速特效
    /// </summary>
    /// <param name="active"></param>
    public void ActivateTurboFx(bool active)
    {
        if (!IsAI && kin.TypeModel == PlayerKinematics.ModelType.Leopard || IsAI && ai.TypeModel == AIKinematics.ModelType.Leopard)
        {
            Effect_Point_SpeedWind.gameObject.SetActive(true);
            Effect_Point_Trial.gameObject.SetActive(false);
        }
        else
        {
            Effect_Point_SpeedWind.gameObject.SetActive(false);
            Effect_Point_Middle_Engine.gameObject.GetComponent<ParticleScaler>().particleScale = active ? 0.4f : 0.2f;
            if (active)
            {
                Effect_Point_Trial.gameObject.SetActive(true);
            }
            else
            {
                StartCoroutine(ShutDownTrial());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="active"></param>
    public void ActivateQiLiu(bool active)
    {
        Effect_Point_QiLiu.gameObject.SetActive(active);
    }

    public void ActivateLeopardEffect(bool active)
    {
        Effect_Point_Leopard_Acc.gameObject.SetActive(active);
    }

    /// <summary>
    /// 是否激活轮胎特效
    /// </summary>
    /// <param name="active"></param>
    public void ActivateLunTai(bool active)
    {
        Effect_Point_LunTai.gameObject.SetActive(active);
    }

    /// <summary>
    /// 引起加速特效
    /// </summary>
    /// <param name="active"></param>
    public void ActivateEngineSpeedUp(bool active)
    {
        Effect_Point_Engine_SpeedUp.gameObject.SetActive(active);
    }
 
    /// <summary>
    /// 开启小引擎特效
    /// </summary>
    /// <param name="active"></param>
    /// <param name="wait"></param>
    public void AcctivateNormalEndingeFx(bool active, int wait = 0)
    {
        Effect_Point_Engine_Right.gameObject.SetActive(active);
        Effect_Point_Engine_Left.gameObject.SetActive(active);
        Effect_Point_Middle_Engine.gameObject.SetActive(active);
        if (active)
        {
            StartCoroutine(ActiveSpeedLineFx(wait));
        }
        else
        {
            Effect_Point_SpeedLine.gameObject.SetActive(active);
        }
    }

    public void ActivateCarChangeLeopard(bool active)
    {
        Effect_Car_Change_To_Leopard.gameObject.SetActive(active);
    }

    /// <summary>
    /// 开启喷气特效
    /// </summary>
    public void ActiveteReadyGoFx(float t)
    {
        ActiveStartFire();
        StartCoroutine(CoroutineReadyGo(t));
    }

    public void ActiveStartFire()
    {
        Effect_Ready_Go.gameObject.SetActive(true);
        StartCoroutine(CloseStartFire());
    }
  
    /// <summary>
    /// 方向赤铁特效
    /// </summary>
    /// <param name="active"></param>
    public void ActivateMagentFx(bool active)
    {
        Effect_Point_Magent.gameObject.SetActive(active);
    }

    /// <summary>
    /// 空气墙
    /// </summary>
    /// <param name="active"></param>
    public void ActivateSideAir(bool active)
    {
        Effect_Point_Side.gameObject.SetActive(active);
    }

    void Update()
    {
        if (GameMode.Instance.PlayerRef == null)
        {
            return;
        }
        Effect_Point_Side.transform.position = new Vector3(0, 0, GameMode.Instance.PlayerRef.transform.position.z - 12);
        Effect_Point_Side.transform.rotation = Quaternion.identity;
    }

    IEnumerator ShutDownTrial()
    {
        yield return new WaitForSeconds(2);
        Effect_Point_Trial.gameObject.SetActive(false);
    }

    IEnumerator CoroutineReadyGo(float t)
    {
        yield return new WaitForSeconds(t);
        AcctivateNormalEndingeFx(true, 1);
    }

    IEnumerator ActiveSpeedLineFx(int wait)
    {
        yield return new WaitForSeconds(wait);
        Effect_Point_SpeedLine.gameObject.SetActive(true);
    }


    IEnumerator ClosePropFollow(float value)
    {
        yield return new WaitForSeconds(value);
        Effect_Point_Eat_Follow.gameObject.SetActive(false);
    }

    IEnumerator CloseStartFire()
    {
        yield return new WaitForSeconds(0.5f);
        Effect_Ready_Go.gameObject.SetActive(false);
    }
}
