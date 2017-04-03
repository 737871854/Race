/*
 * Copyright (c) 
 * 
 * 文件名称：   SelectLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/14 17:24:33
 * 
 * 修改描述：
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;

public class SelectLogic : BaseUI, IComparer<int>
{
    private SelectView view;

    #region select map
    private float _addV = 0;
    private float _vK = 0;                      // 1右运动 -1左运动
    private float _currentV = 0;
    private float _vTotal = 0;
    private float _vT = 0;
    private float _animSpeed = 1.0f;            // 动画速度

    private bool _anim = false;                 // 动画状态

    private float _width = 1200;
    private float _maxScale = 1;

    private float _startValue = 0.1f;           // 开始坐标值
    private float _addValue = 0.4f;             // 间隔坐标值
    private float _vMin = 0.1f;                 // 小于_vMin达到最左
    private float _vMax = 0.9f;                 // 大于_vMax达到最右

    public AnimationCurve _positionCurve;       // 坐标曲线

    public AnimationCurve _scaleCurve;          // 缩放曲线

    public AnimationCurve _apaCurve;            // 透明曲线

    private int _selectCarID;                   // 选中玩家车辆ID

    private float _timeUp = 15;                 // 倒计时

    private class CameraInfo
    {
        public Vector3 position;
        public Vector3 localEulerAngle;
    }

    private List<SelectItem> _itemList;

    private SelectItem _current;

    private List<SelectItem> GotoFirstItems = new List<SelectItem>(), GotoLaserItems = new List<SelectItem>();

    #endregion


    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelSelect;
    }

    protected override void OnAwake()
    {
        MessageCenter.Instance.AddListener(MessageType.Message_Key_Game_Right, MoveRight);
        MessageCenter.Instance.AddListener(MessageType.Message_Key_Game_Left, MoveLeft);
        MessageCenter.Instance.AddListener(MessageType.Message_Key_Game_Start, SelectSure);
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.AddListener(MessageType.Message_Map_Is_Choosed, OnHostSelectMap);
        MessageCenter.Instance.AddListener(MessageType.Message_Select_Map, OnSelectMap);
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Key_Game_Right, MoveRight);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Key_Game_Left, MoveLeft);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Key_Game_Start, SelectSure);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Map_Is_Choosed, OnHostSelectMap);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Select_Map, OnSelectMap);
        SoundController.Instance.StopSelectMusic();
        base.OnRelease();
    }

    protected override void OnStart()
    {
        view = new SelectView();
        view.Init(transform);
        SoundController.Instance.StartSelectMusic();
        Reset();
        InitMap();      
        base.OnStart();
    }

    #region Private Function
    private void Reset()
    {
        _selectCarID                    = 2;        
        view._selectType                = SelectView.SelectType.Mode;

        for (int i = 0; i < view._carInfo.Count; ++i )
            view._carInfo[i].disabled = false;

    }

    /// <summary>
    /// 初始化Select Map
    /// </summary>
    private void InitMap()
    {
        Keyframe[] ks       = new Keyframe[5];
        ks[0]               = new Keyframe(0, -0.4f);
        ks[1]               = new Keyframe(0.1f, -0.4f);
        ks[1].inTangent     = 0.62561655f;
        ks[1].outTangent    = 0.62561655f;
        ks[2]               = new Keyframe(0.5f, 0.5f);
        ks[2].inTangent     = 1.24999809f;
        ks[2].outTangent    = 1.24999809f;
        ks[3]               = new Keyframe(0.9f, 1.4f);
        ks[3].inTangent     = 0.624381542f;
        ks[3].inTangent     = 0.624381542f;
        ks[4]               = new Keyframe(1, 1.4f);
        _positionCurve      = new AnimationCurve(ks);

        ks                  = new Keyframe[3];
        ks[0]               = new Keyframe(0, 0.9f);
        ks[1]               = new Keyframe(0.5f, 1);
        ks[2]               = new Keyframe(1, 0.9f);
        _scaleCurve         = new AnimationCurve(ks);

        ks                  = new Keyframe[7];
        ks[0]               = new Keyframe(0, 0.1f);
        ks[1]               = new Keyframe(0.1f, 0.5f);
        ks[1].inTangent     = 2.5361743f;
        ks[1].outTangent    = 2.5361743f;
        ks[2]               = new Keyframe(0.3f, 0.8f);
        ks[2].inTangent     = 2.87718964f;
        ks[2].outTangent    = 2.87718964f;
        ks[3]               = new Keyframe(0.5f, 1);
        ks[3].inTangent     = 0.104366615f;
        ks[3].outTangent    = 0.104366615f;
        ks[4]               = new Keyframe(0.7f, 0.8f);
        ks[4].inTangent     = -3.06029272f;
        ks[4].outTangent    = -3.06029272f;
        ks[5]               = new Keyframe(0.9f, 0.5f);
        ks[5].inTangent     = -2.54762077f;
        ks[5].outTangent    = -2.54762077f;
        ks[6]               = new Keyframe(1, 0.1f);
        _apaCurve           = new AnimationCurve(ks);

        _itemList           = new List<SelectItem>();
        for (int i = 0; i < view._selectMap.map.transform.childCount; i++)
        {
            Transform tran  = view._selectMap.map.transform.GetChild(i);
            SelectItem item = tran.GetComponent<SelectItem>();
            if (null != item)
            {
                _itemList.Add(item);
                item.Init(this);
                item.Drag(_startValue + i * _addValue);
                if (item.V - 0.5f < 0.05f)
                    _current = _itemList[i];
            }
        }

        _vMax       = _startValue + (view._selectMap.map.transform.childCount - 1) * _addValue;
        _current    = _itemList[1];

    }

    // 刷新车信息
    private void RefreshInfo()
    {
        ResetHead();

        Message message = new Message(MessageType.Message_Car_Selected, this);
        message["id"] = _selectCarID;
        message.Send();

        //LobbyManager.Instance.LobbyPlayer.CmdTellServerViewID(_selectCarID);

        for (int i = 0; i < view._carInfo.Count; ++i )
        {
            view._carInfo[i].obj.transform.position = view._carInfo[i].localPosition;
            view._carInfo[i].obj.transform.localScale = view._carInfo[i].localScale;
            view._carInfo[i].obj.transform.localEulerAngles = view._carInfo[i].localEulerAngles;
            if (i == 0)
                view._carInfo[i].car.transform.localEulerAngles = Vector3.up * 90;
            else
                view._carInfo[i].car.transform.localEulerAngles = Vector3.zero;
        }

    }

    /// <summary>
    /// 选右边的车
    /// </summary>
    private void CarMoveRight()
    {
        SelectView.CarInfo info = new SelectView.CarInfo();
        info.obj = view._carInfo[view._carInfo.Count - 1].obj;
        for (int i = view._carInfo.Count - 1; i > 0; --i )
            view._carInfo[i].obj = view._carInfo[i -1].obj;

        view._carInfo[0].obj = info.obj;
        if (--_selectCarID < 0)
            _selectCarID += view._carInfo.Count;
        if (view._carInfo[_selectCarID].disabled)
            CarMoveRight();
    }

    /// <summary>
    /// 选左边的车
    /// </summary>
    private void CarMoveLeft()
    {
        SelectView.CarInfo info = new SelectView.CarInfo();
        info.obj = view._carInfo[0].obj;
        for (int i = 0; i < view._carInfo.Count - 1; ++i )
            view._carInfo[i].obj = view._carInfo[i + 1].obj;

        view._carInfo[view._carInfo.Count - 1].obj = info.obj;
        ++_selectCarID;
        _selectCarID %= view._carInfo.Count;
        if (view._carInfo[_selectCarID].disabled)
            CarMoveLeft();
    }

    /// <summary>
    /// 选中车后，放大对应的角色头像
    /// </summary>
    private void ResetHead()
    {
        for (int i = 0; i < view._headList.Count; ++i)
        {
            if (view._headList[i].selected.gameObject.activeSelf)
                view._headList[i].root.transform.localScale = Vector3.one;

            if (i == _selectCarID)
            {
                view._headList[i].selected.gameObject.SetActive(true);
                view._headList[i].root.transform.localScale = Vector3.one * 1.1f;
            }
            else
                view._headList[i].selected.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 按右移选择按钮
    /// </summary>
    /// <param name="message"></param>
    private void MoveRight(Message message)
    {
        switch (view._selectType)
        {
            case SelectView.SelectType.Mode:
                SelectMulityMode();
                break;
            case SelectView.SelectType.Map:
                if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer && !LobbyManager.Instance.LocalIsHost())
                    return;
                AnimToEnd(-_addValue);
                LobbyManager.Instance.LobbyPlayer.RpcOnSelectMap(-_addValue);
                break;
            case SelectView.SelectType.Car:
                CarMoveRight();
                RefreshInfo();
                break;
        }
    }

    /// <summary>
    /// 按左移选择按钮
    /// </summary>
    /// <param name="message"></param>
    private void MoveLeft(Message message)
    {
        switch (view._selectType)
        {
            case SelectView.SelectType.Mode:
                SelectSingleMode();
                break;
            case SelectView.SelectType.Map:
                if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer && !LobbyManager.Instance.LocalIsHost())
                    return;
                AnimToEnd(_addValue);
                LobbyManager.Instance.LobbyPlayer.RpcOnSelectMap(_addValue);
                break;
            case SelectView.SelectType.Car:
                CarMoveLeft();
                RefreshInfo();
                break;
        }
    }

    /// <summary>
    /// 播放地图移动特效
    /// </summary>
    private void PlayMapMoveEffect()
    {
        for (int i = 0; i < _itemList.Count; ++i)
        {
            if (_current.type == _itemList[i].type)
            {
                _itemList[i].particle0.Play(true);
                _itemList[i].particle1.SetActive(true);
                _itemList[i].green.gameObject.SetActive(false);
                _itemList[i].yellow.gameObject.SetActive(true);
            }
            else
            {
                _itemList[i].particle1.SetActive(false);
                _itemList[i].green.gameObject.SetActive(true);
                _itemList[i].yellow.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 播放地图选择特效
    /// </summary>
    private void ShowMapSelectEffect()
    {
        for (int i = 0; i < _itemList.Count; ++i)
        {
            if (_current.type == _itemList[i].type)
                _itemList[i].selected.gameObject.SetActive(true);
            _itemList[i].self.enabled = false;
            _itemList[i].particle1.SetActive(false);
        }
        StartCoroutine(ShowCarView());
    }

   

    /// <summary>
    ///  单人模式
    /// </summary>
    private void SelectSingleMode()
    {
        view._selectModel.singleInfo.itemGreen.gameObject.SetActive(false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._selectModel.singleInfo.itemButton.transform.DOScale(Vector3.one * 1.1f, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemGreen.transform.DOScale(Vector3.one * 1.1f, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemButton.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemGreen.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemParticle0.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemParticle0.transform.DOScale(Vector3.one * 0.85f, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemCard.transform.DOScale(Vector3.one * 0.9f, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemCard.transform.DOScale(Vector3.one, 0.1f));
        view._selectModel.multiInfo.itemGreen.gameObject.SetActive(true);
        ShowSelected(true);
        GameMode.Instance.ModeSelected = GameMode.SelectMode.SinglePlayer;
    }

    /// <summary>
    /// 联机模式
    /// </summary>
    private void SelectMulityMode()
    {
        view._selectModel.multiInfo.itemGreen.gameObject.SetActive(false);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._selectModel.multiInfo.itemButton.transform.DOScale(Vector3.one * 1.1f, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemGreen.transform.DOScale(Vector3.one * 1.1f, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemButton.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemGreen.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemParticle0.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemParticle0.transform.DOScale(Vector3.one * 0.85f, 0.1f));
        sequence.Join(view._selectModel.multiInfo.itemCard.transform.DOScale(Vector3.one, 0.1f));
        sequence.Join(view._selectModel.singleInfo.itemCard.transform.DOScale(Vector3.one * 0.9f, 0.1f));
        view._selectModel.singleInfo.itemGreen.gameObject.SetActive(true);
        ShowSelected(false);
        GameMode.Instance.ModeSelected = GameMode.SelectMode.Multiplayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isSingle"></param>
    private void ShowSelected(bool isSingle)
    {
        if (isSingle)
        {
            view._selectModel.multiInfo.itemParticle0.SetActive(false);
            view._selectModel.multiInfo.itemBk.gameObject.SetActive(false);
            view._selectModel.singleInfo.itemParticle0.SetActive(true);
            view._selectModel.singleInfo.itemBk.gameObject.SetActive(true);
        }
        else
        {
            view._selectModel.multiInfo.itemParticle0.SetActive(true);
            view._selectModel.multiInfo.itemBk.gameObject.SetActive(true);
            view._selectModel.singleInfo.itemParticle0.SetActive(false);
            view._selectModel.singleInfo.itemBk.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 跳到地图选择界面
    /// </summary>
    private void GoToMap()
    {
        if (LobbyManager.Instance.LocalIsHost())
            LobbyManager.Instance.UpdateToMap(true);

        _timeUp             = 15;
        view._selectType    = SelectView.SelectType.Map;
        if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer)
            LobbyManager.Instance.SetMaxPlayerCount();
        view._selectModel.modelRoot.SetActive(false);
        view._selectMap.mapRoot.SetActive(true);
        StartCoroutine(ShowRotation(view._selectMap.animator));
    }

    /// <summary>
    /// 无人加入联机，则返回联机和单机选择界面
    /// </summary>
    private void ResetToMode()
    {
        GameMode.Instance.ReturnModeSelect();
        view._selectType = SelectView.SelectType.Mode;
        view._selectModel.waitting.SetActive(false);
        view._selectModel.singleInfo.itemRoot.SetActive(true);
        view._selectModel.multiInfo.itemRoot.SetActive(true);
    }

    /// <summary>
    /// 播放选择单人游戏特效
    /// </summary>
    private void PlaySingleModeParticle()
    {
        view._selectModel.singleParticle.SetActive(true);
        StartCoroutine(DisableModeParticle(view._selectModel.singleParticle));
    }

    /// <summary>
    /// 播放选择联机模式游戏特效
    /// </summary>
    /// <param name="active"></param>
    private void PlayMultiModeParticle()
    {
        view._selectModel.multiParticle.SetActive(true);
        StartCoroutine(DisableModeParticle(view._selectModel.multiParticle));
    }

    /// <summary>
    /// 选择联机或单机
    /// </summary>
    private void OnSelectMode()
    {
        GameMode.Instance.UNetGameBegine();
        switch (GameMode.Instance.ModeSelected)
        {
            case GameMode.SelectMode.SinglePlayer:
                SetSortOlder(view._selectModel.singleInfo.itemRoot, view._selectModel.multiInfo.itemRoot);
                StartCoroutine(ModelSure(true, true));
                break;
            case GameMode.SelectMode.Multiplayer:
                SetSortOlder(view._selectModel.multiInfo.itemRoot, view._selectModel.singleInfo.itemRoot);
                 //当前设备作为主机
                //if (LobbyManager.Instance.LocalIsHost())
                //    StartCoroutine(ModelSure(false, false));
                //else
                //    StartCoroutine(ModelSure(true, false));

                StartCoroutine(ModelSure(false, false));
                break;
        }
    }

    /// <summary>
    /// 选择地图
    /// </summary>
    private void OnSelectMap()
    {
        _timeUp = 30;

        if (LobbyManager.Instance.LocalIsHost() || GameMode.Instance.ModeSelected == GameMode.SelectMode.SinglePlayer)
            LobbyManager.Instance.UpdateGameDifficulty(SettingManager.Instance.TicketModel);

        //  联机，且不作为主机，无权选择地图
        if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer && !LobbyManager.Instance.LocalIsHost())
            return;
        else
        {
            EnvirManager.Instance.CurrentEnvirType = _current.type;
            ShowMapSelectEffect();
        }
    }

    /// <summary>
    /// 选车
    /// </summary>
    private void OnSelectCar()
    {       
        view._selectCar.SetActive(false);
        view.loading.gameObject.SetActive(true);
        //LobbyManager.Instance.SetupLocalPlayer(_selectCarID);
        LobbyManager.Instance.LobbyPlayer.CmdTellServerSelectedID(_selectCarID);
        GameMode.Instance.ChooseCharacterID = _selectCarID;
        SoundController.Instance.StopLeopardSelect();
        view._selectType = SelectView.SelectType.End;
        LobbyManager.Instance.Ready();
        //StartCoroutine(WaittingToGame());
    }

    /// <summary>
    /// 确认选择
    /// </summary>
    /// <param name="message"></param>
    private void SelectSure(Message message)
    {
       switch (view._selectType)
       {
           case SelectView.SelectType.Mode:
               OnSelectMode();
               break;
           case SelectView.SelectType.Wating:
               OnPlayerCountSure();
               break;
           case SelectView.SelectType.Map:
               OnSelectMap();
               break;
           case SelectView.SelectType.Car:
               OnSelectCar();
               break;
       }
    }

    /// <summary>
    /// 玩家人数确认
    /// </summary>
    private void OnPlayerCountSure()
    {
        if (!LobbyManager.Instance.LocalIsHost())
            return;

        if (LobbyManager.Instance.CanBegineGame())
            GoToMap();
    }

    /// <summary>
    /// 图层排序
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="backward"></param>
    private void SetSortOlder(GameObject forward, GameObject backward)
    {
        Canvas canvas0          = forward.GetOrAddComponent<Canvas>();
        canvas0.overrideSorting = true;
        canvas0.sortingOrder    = 5;

        Canvas canvas1          = forward.GetOrAddComponent<Canvas>();
        canvas1.overrideSorting = true;
        canvas1.sortingOrder    = 6;
    }


    /// <summary>
    /// 地图选择后
    /// </summary>
    /// <param name="message"></param>
    private void OnHostSelectMap(Message message)
    {
        view._selectMap.mapRoot.SetActive(false);
        view._uiCar.gameObject.SetActive(true);
        view._selectCar.gameObject.SetActive(true);
        view._selectType = SelectView.SelectType.Car;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void OnSelectMap(Message message)
    {
        float value = (float)message["value"];
        AnimToEnd(value);
    }

    private void AnimToEnd(float k)
    {
        if (_anim)
        {
            return;
        }

        _addV = k;
        if (_addV > 0)
            _vK = 1;
        else if (_addV < 0)
            _vK = -1;
        else
            return;
        _vTotal = 0;
        _anim = true;
    }

    private bool hasChoose;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    private void UpdatePreFrame(Message message)
    {
        UpdateWatingTime();

        UpdateMapTime();

        UpdateSelectedCar();

        UpdateSelectMap();

        if (view._selectType >= SelectView.SelectType.Car)
        {
            if (LobbyManager.Instance.LocalIsHost() || GameMode.Instance.ModeSelected == GameMode.SelectMode.SinglePlayer)
            {
                _timeUp -= Time.deltaTime;
                LobbyManager.Instance.UpdateLifeValue(_timeUp);
            }
            else
                _timeUp = LobbyManager.Instance.LobbyPlayer.LifeValue;

            if (view._selectType == SelectView.SelectType.Car)
            {
                if (_timeUp > 0)
                {
                    int value0 = (int)((_timeUp / 10) % 10);
                    int value1 = (int)((_timeUp - 10 * value0));
                    view._timeUpList[0].sprite = view.module._timeList[value0];
                    view._timeUpList[1].sprite = view.module._timeList[value1];
                }
                else
                {
                    if (hasChoose)
                        return;
                    hasChoose = true;
                    StartCoroutine(SureCar());
                }
            }
        }

        UpdateCarRotation();
       
    }

    IEnumerator SureCar()
    {
        yield return new WaitForSeconds(0.1f * LobbyManager.Instance.LobbyPlayer.Order);
        Sure();
    }

    private void Sure()
    {
        SoundController.Instance.PlaySoundSure();
        Message msg = new Message(MessageType.Message_Key_Game_Start, this);
        msg.Send();
    }

    /// <summary>
    /// 选车，车辆静止状态旋转
    /// </summary>
    private void UpdateCarRotation()
    {
        if (view._selectType != SelectView.SelectType.Car || view._carInfo[_selectCarID].carInfo._state != CarSelect.State.idle)
            return;

        view._carInfo[_selectCarID].car.transform.localEulerAngles -= Vector3.up * Time.deltaTime * 10;
    }

    private void UpdateSelectedCar()
    {
        for (int i = 0; i < view._carInfo.Count; ++i)
        {
            for (int j = 0; j < LobbyManager.Instance.SelectedList.Count; ++j)
            {
                if (i == LobbyManager.Instance.SelectedList[j])
                {
                    if (!view._carInfo[i].disabled)
                    {
                        view._carInfo[i].disabled       = true;
                        Material material               = new Material(Shader.Find("Q5/Proj/effect/UI Grey"));
                        view._headList[i].head.material = material;
                        if (i == _selectCarID && view._selectType != SelectView.SelectType.End)
                        {
                            if (LobbyManager.Instance.SelectedList.Count % 2 == 0)
                                CarMoveLeft();
                            else
                                CarMoveRight();
                            RefreshInfo();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 更新等待其他玩家加入游戏时间
    /// </summary>
    private void  UpdateWatingTime()
    {
        if (view._selectType != SelectView.SelectType.Wating)
            return;

        for (int i = 0; i < LobbyManager.Instance.JoinPlayerNum(); ++i )
            view._selectModel.joinList[i].gameObject.SetActive(true);

        if (LobbyManager.Instance.LocalIsHost())
        {
            _timeUp                         -= Time.deltaTime;
            LobbyManager.Instance.UpdateLifeValue(_timeUp);
        }
        else
            _timeUp = LobbyManager.Instance.LobbyPlayer.LifeValue;

        int value0 = (int)((_timeUp / 10) % 10);
        int value1 = (int)((_timeUp - 10 * value0));
        if (_timeUp < 10)
        {
            view._selectModel.timeList[0].gameObject.SetActive(false);
            view._selectModel.timeList[1].gameObject.SetActive(false);
            view._selectModel.timeList[2].gameObject.SetActive(true);
            view._selectModel.timeList[2].sprite = view.module._timeList[value1];
        }
        else
        {
            view._selectModel.timeList[0].gameObject.SetActive(true);
            view._selectModel.timeList[1].gameObject.SetActive(true);
            view._selectModel.timeList[2].gameObject.SetActive(false);
            view._selectModel.timeList[0].sprite = view.module._timeList[value0];
            view._selectModel.timeList[1].sprite = view.module._timeList[value1];
        }

        // 达到联机最大人数
        if (LobbyManager.Instance.PlayerCountEquals() || LobbyManager.Instance.LobbyPlayer.ToMap)
             GoToMap();

        // 无法满足联机人数， 返回
        if (_timeUp <= 0)
        {
            if (!LobbyManager.Instance.CanBegineGame())
                ResetToMode();
            else
                GoToMap();
        }
    }

    private void UpdateMapTime()
    {
        if (view._selectType != SelectView.SelectType.Map)
            return;

        if (LobbyManager.Instance.LocalIsHost() || GameMode.Instance.ModeSelected == GameMode.SelectMode.SinglePlayer)
        {
            _timeUp -= Time.deltaTime;
            LobbyManager.Instance.UpdateLifeValue(_timeUp);
        }
        else
        {
            if (GameMode.Instance.ModeSelected == GameMode.SelectMode.Multiplayer)
                _timeUp = LobbyManager.Instance.LobbyPlayer.LifeValue;
        }

        int value0 = (int)((_timeUp / 10) % 10);
        int value1 = (int)((_timeUp - 10 * value0));
        if (_timeUp < 10)
        {
            view._selectMap.timeList[0].gameObject.SetActive(false);
            view._selectMap.timeList[1].gameObject.SetActive(false);
            view._selectMap.timeList[2].gameObject.SetActive(true);
            view._selectMap.timeList[2].sprite = view.module._timeList[value1];
        }
        else
        {
            view._selectMap.timeList[0].gameObject.SetActive(true);
            view._selectMap.timeList[1].gameObject.SetActive(true);
            view._selectMap.timeList[2].gameObject.SetActive(false);
            view._selectMap.timeList[0].sprite = view.module._timeList[value0];
            view._selectMap.timeList[1].sprite = view.module._timeList[value1];
        }

        if (_timeUp <= 0)
            OnSelectMap();
    }

    /// <summary>
    /// 地图切换
    /// </summary>
    private void UpdateSelectMap()
    {
        if (_anim)
        {
            _currentV = Time.deltaTime * _animSpeed * _vK;
            _vT = _vTotal + _currentV;
            if (_vK > 0 && _vT >= _addV)
            {
                _anim = false;
                _currentV = _addV - _vTotal;
                PlayMapMoveEffect();
            }
            if (_vK < 0 && _vT <= _addV)
            {
                _anim = false;
                _currentV = _addV - _vTotal;
                PlayMapMoveEffect();
            }
            //==============
            for (int i = 0; i < _itemList.Count; i++)
            {
                _itemList[i].Drag(_currentV);
                if (_itemList[i].V - 0.5 < 0.05f)
                    _current = _itemList[i];
            }
            Check(_currentV);
            _vTotal = _vT;
        }
    }


    /// <summary>
    /// 检查是否有Item越界，如是则循环
    /// </summary>
    /// <param name="_v"></param>
    public void Check(float _v)
    {
        if (_v < 0)
        {//向左运动
            for (int i = 0; i < _itemList.Count; i++)
            {
                if (_itemList[i].V < (_vMin - _addValue / 2))
                    GotoLaserItems.Add(_itemList[i]);
            }
            if (GotoLaserItems.Count > 0)
            {
                for (int i = 0; i < GotoLaserItems.Count; i++)
                {
                    GotoLaserItems[i].V = _itemList[_itemList.Count - 1].V + _addValue;
                    _itemList.Remove(GotoLaserItems[i]);
                    _itemList.Add(GotoLaserItems[i]);
                }
                GotoLaserItems.Clear();
            }
        }
        else if (_v > 0)
        {//向右运动，需要把右边的放到前面来

            for (int i = _itemList.Count - 1; i > 0; i--)
            {
                if (_itemList[i].V >= _vMax)
                    GotoFirstItems.Add(_itemList[i]);
            }
            if (GotoFirstItems.Count > 0)
            {
                for (int i = 0; i < GotoFirstItems.Count; i++)
                {
                    GotoFirstItems[i].V = _itemList[0].V - _addValue;
                    _itemList.Remove(GotoFirstItems[i]);
                    _itemList.Insert(0, GotoFirstItems[i]);
                }
                GotoFirstItems.Clear();
            }
        }
    }
    #endregion


    #region Public Function
   
    public float GetApa(float v)
    {
        return _apaCurve.Evaluate(v);
    }

    public float GetPosition(float v)
    {
        return _positionCurve.Evaluate(v) * _width - 580;
    }

    public float GetScale(float v)
    {
        return _scaleCurve.Evaluate(v) * _maxScale;
    }

    #endregion

    IEnumerator ModelSure(bool toMap, bool single)
    {
        view._selectModel.singleInfo.itemRoot.SetActive(false);
        view._selectModel.multiInfo.itemRoot.SetActive(false);
        if (single)
            PlaySingleModeParticle();
        else
            PlayMultiModeParticle();
        yield return new WaitForSeconds(0.8f);
        if (toMap)
        {
            GoToMap();
        }
        else
        {
            StartCoroutine(GoToWaiting());
        }
    }

    IEnumerator DisableModeParticle(GameObject go)
    {
        yield return new WaitForSeconds(1);
        go.SetActive(false);
    }

    IEnumerator ShowRotation(Animator animator)
    {
        yield return new WaitForSeconds(1.5f);
        animator.enabled = true;
    }

    IEnumerator GoToWaiting()
    {
        yield return new WaitForSeconds(0.8f);
        view._selectModel.waitting.SetActive(true);
        view._selectModel.multiParticle.SetActive(false);
        view._selectType    = SelectView.SelectType.Wating;
        _timeUp             = 15;
        if (LobbyManager.Instance.LocalIsHost())
            LobbyManager.Instance.UpdateToMap(false);
    }

    IEnumerator ShowCarView()
    {
        yield return new WaitForSeconds(0.5f);
        view._selectMap.mapRoot.SetActive(false);
        view._uiCar.gameObject.SetActive(true);
        view._selectCar.gameObject.SetActive(true);
        view._selectType = SelectView.SelectType.Car;
        LobbyManager.Instance.LobbyPlayer.RpcOnMapSure(_current.type);
    }


    IEnumerator WaittingToGame()
    {
        yield return new WaitForSeconds(1);
        LobbyManager.Instance.Ready();
    }


    public int Compare(int x, int y)
    {
        return x < y ? 1 : -1;
    }
}
