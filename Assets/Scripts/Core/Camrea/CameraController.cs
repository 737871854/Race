/*
 * Copyright (c) 
 * 
 * 文件名称：   CameraController.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2017/1/14 12:28:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public Animator animator;

    private bool playAnimator = false;
    private float observeTime;

    void Start()
    {
        observeTime = GameConfig.GAME_CONFIG_OBSERVE_TIME;
        MessageCenter.Instance.AddListener(MessageType.Message_Change_Observe, OnChangeObserve);
    }

    void OnDestroy()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Change_Observe, OnChangeObserve);
    }

    void Update()
    {
        if (GameMode.Instance.Mode == RunMode.ReadyToRace && LobbyManager.Instance.LobbyPlayer.ReadyTime >= 3 && !playAnimator)
        {
            playAnimator = true;
            StartCoroutine(OtherCamera());
            GameMode.Instance.ChangeObserver(GameMode.Instance.PlayerRef.ID);
        }

        if (GameMode.Instance.Mode == RunMode.Dead)
        {
            if (observeTime > 0)
                observeTime -= Time.deltaTime;
            else
            {
                GameMode.Instance.ObservedChange();
                observeTime = GameConfig.GAME_CONFIG_OBSERVE_TIME;
            }
        }
    }

    /// <summary>
    /// 开启观察模式
    /// </summary>
    /// <param name="message"></param>
    private void OnChangeObserve(Message message)
    {
        observeTime = 0;
    }

    
    IEnumerator OtherCamera()
    {
        camera.gameObject.SetActive(false);
        animator.gameObject.SetActive(true);
        yield return new WaitForSeconds(GameConfig.GAME_CONFIG_READY_TIME - 3);
        GameMode.Instance.ReadyGo                       = true;
        camera.gameObject.transform.position            = new Vector3(0, 5, 11);
        camera.gameObject.transform.localEulerAngles    = new Vector3(31, 0, 0);
        camera.gameObject.SetActive(true);
        camera.gameObject.GetComponent<FollowCamera>().enabled = true;
        animator.gameObject.SetActive(false);
    }
}
