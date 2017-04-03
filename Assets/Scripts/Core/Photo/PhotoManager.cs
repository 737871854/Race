/*
 * Copyright (c) 
 * 
 * 文件名称：   PhotoManager.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/11/10 9:05:47
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using Need.Mx;

public delegate void DelegateCameraIsOpen();
public delegate void DelegateCapture();

public class PhotoManager : SingletonBehaviour<PhotoManager>
{
    #region Private members
    // 接受返回的图片数据
    private WebCamTexture _wct;
    // 招聘
    private Texture2D _image;
    // 照片保存完成
    private bool _canSave;
    private bool _isOpen;
    #endregion

    #region Public proterties
    public Texture2D CaptureImage   { get { return _image; } }
    public bool CanSave             { get { return _canSave; } }
    public DelegateCameraIsOpen CameraIsOpenDeletate;
    public DelegateCapture CaptureScreenDelegate;
    #endregion

    #region unity callback
    void Start()
    {
        _canSave = false;
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
    }

    void Destroy()
    {
        if (null != _wct)
        {
            if (_wct.isPlaying)
            {
                _wct.Stop();
                _wct = null;
            }
        }
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
    }
    #endregion

    public void Rest()
    {
        _canSave = false;
    }

    // 开启摄像头
    public void OpenCamera()
    {
        if (null == _wct)
            StartCoroutine("OnOpenCamera");
        else if (_wct.isPlaying)
            return;
        else
            _wct.Play();
    }

    // 关闭摄像头
    public void CloseCamera()
    {
        StartCoroutine(OnCloseCamera());
    }

    /// <summary>
    /// 截图
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public void CaptureScreen()
    {
        StartCoroutine(Capture());
    }

    public void UpdatePreFrame(Message message)
    {
        if (null != _wct && _isOpen)
        {
            if (null == _image)
            {
                _image = new Texture2D(500, 380, TextureFormat.ARGB32, false, false);
            }
            _image.SetPixels(0, 0, 500, 380, _wct.GetPixels(0, 0, 500, 380));
            _image.Apply();
            _canSave = true;
            if (null != CameraIsOpenDeletate)
                CameraIsOpenDeletate();
        }
    }

    IEnumerator Capture()
    {
        yield return new  WaitForEndOfFrame();
        _image = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        _image.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        _image.Apply();
        if (null != CaptureScreenDelegate)
            CaptureScreenDelegate();
    }

    IEnumerator OnCloseCamera()
    {
        yield return new WaitForSeconds(1);
        if (null != _wct)
        {
            _wct.Stop();
            _wct = null;
            _isOpen = false;
        }
    }

    IEnumerator OnOpenCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length == 0)
                Debuger.Log("没有找到摄像头");
            else
            {
                string deviceName;
                deviceName  = devices[0].name;
                // 设置摄像机摄像区域
                _wct        = new WebCamTexture(deviceName, 620, 350, 60);
                _wct.Play();
                StartCoroutine(SetOpen());
            }
        }
        else
            Debug.Log("调用摄像头授权失败");
    }

    IEnumerator SetOpen()
    {
        yield return new WaitForSeconds(0.1f);
        _isOpen = true;
    }
}
