using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Need.Mx;

public class LoadingBar : BaseUI
{
    private LoadingBarModule loadingBarmodule;
    #region UI
    // 进度条
    private Image sliderProgress;
    // 要显示的进度条数字
    private List<Image> progressList = null;
    #endregion

    public LoadingCompletedHandle LoadingComplete;

    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelLoading;
    }

    // Use this for initialization
    void LoadingStart()
    {
        InitUI();

        loadingBarmodule = ModuleManager.Instance.Get<LoadingBarModule>();
        loadingBarmodule.Load();
    }

    void InitUI()
    {
        Image image;
        progressList = new List<Image>();
        sliderProgress = transform.Find("Slider/SliderProgress").GetComponent<Image>();
        for (int i = 0; i < 3; i++)
        {
            image = transform.Find("ProgressInfo/Image_Number" + i.ToString()).GetComponent<Image>();
            progressList.Add(image);
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }


    public void Load(EnumSceneType _sceneType)
    {
        LoadingStart();
        CoroutineController.Instance.StartCoroutine(StartLoad(_sceneType));
    }

    private IEnumerator StartLoad(EnumSceneType _sceneType)
    {
        int displayProgress = 0;
        int toProgress = 0;
        string sceneName = SceneManager.Instance.GetSceneName(_sceneType);
        AsyncOperation op = Application.LoadLevelAsync(sceneName);
        op.allowSceneActivation = false;
        if (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
        if (null != LoadingComplete)
        {
            EventArgs e = new EventArgs();
            LoadingComplete(this, e);
            SceneManager.Instance.CurrentScene = ModuleManager.Instance.Get(sceneName) as BaseScene;
            SceneManager.Instance.CurrentScene.InitScene();
        }
    }


    private void SetLoadingPercentage(int displayProgress)
    {
        sliderProgress.fillAmount = (int)displayProgress / 100.0f;

        int[] value = new int[3];
        int level = 100;
        int count = 0;
        for (int index = 0; index < 3; ++index)
        {
            int number = (displayProgress / level) % 10;
            value[count++] = number;
            level /= 10;
        }

        if (count == 1)
        {
            progressList[0].sprite = loadingBarmodule.numberList[value[0]];
        }
        else if (count == 2)
        {
            progressList[0].sprite = loadingBarmodule.numberList[value[0]];
            progressList[1].sprite = loadingBarmodule.numberList[value[1]];
        }
        else
        {
            progressList[0].sprite = loadingBarmodule.numberList[value[0]];
            progressList[1].sprite = loadingBarmodule.numberList[value[1]];
            progressList[2].sprite = loadingBarmodule.numberList[value[2]];
        }

    }

}
