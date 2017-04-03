/*
 * Copyright (c) 
 * 
 * 文件名称：   SelectView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/14 17:24:44
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Need.Mx;

public class SelectView
{
    public enum SelectType
    {
        None = -1,
        Mode,
        Wating,
        Map,
        Car,
        End,
    }

    public SelectModule module;

    // 单机联机
    public struct SelectModel
    {
        public GameObject modelRoot;
        public ItemModel multiInfo;
        public ItemModel singleInfo;
        public GameObject singleParticle;
        public GameObject multiParticle;
        public GameObject waitting;
        public List<Image> timeList;
        public List<Image> joinList;
    }
    public SelectModel _selectModel;

    public struct ItemModel
    {
        public GameObject itemRoot;
        public GameObject itemCard;
        public GameObject itemSelected;
        public Image itemButton;
        public Image itemGreen;
        public GameObject itemParticle0;
        public Image itemBk;
    }

    // 地图选择
    public struct SelectMap
    {
        public GameObject mapRoot;
        public GameObject map;
        public Animator animator;
        public List<Image> timeList;
    }
    public SelectMap _selectMap;

    // 车辆选择
    public GameObject _selectCar;
    public GameObject _uiCar;

    // 当前操作类型
    public SelectType _selectType = SelectType.None;

    public class HeadInfo
    {
        public GameObject root;
        public Image head;
        public Image selected;
    }
    public List<HeadInfo> _headList;

    // 3D车辆信息
    public class CarInfo
    {
        public GameObject obj;
        public GameObject car;
        public CarSelect carInfo;
        public Vector3 localPosition;
        public Vector3 localScale;
        public Vector3 localEulerAngles;
        public bool disabled;
    }
    public List<CarInfo> _carInfo;

    // 选车倒计时
    public List<Image> _timeUpList;

    // Waitting
    public Image loading;

    public void Init(Transform transform)
    {
        module = ModuleManager.Instance.Get<SelectModule>();
        module.Load();

        {
            _selectModel                                    = new SelectModel();
            _selectModel.multiInfo                          = new ItemModel();
            _selectModel.singleInfo                         = new ItemModel();
            _selectModel.modelRoot                          = transform.Find("SelectModel").gameObject;
            _selectModel.multiInfo.itemRoot                 = _selectModel.modelRoot.transform.Find("Multi").gameObject;
            _selectModel.multiInfo.itemCard                 = _selectModel.modelRoot.transform.Find("Multi/Card").gameObject;
            _selectModel.multiInfo.itemButton               = _selectModel.modelRoot.transform.Find("Multi/Image_Show").GetComponent<Image>();
            _selectModel.multiInfo.itemGreen                = _selectModel.modelRoot.transform.Find("Multi/Image_Green").GetComponent<Image>();
            _selectModel.multiInfo.itemParticle0            = _selectModel.modelRoot.transform.Find("Multi/Card/UI_show").gameObject;
            _selectModel.multiInfo.itemBk                   = _selectModel.modelRoot.transform.Find("Multi/Card/Background").GetComponent<Image>();

            _selectModel.singleInfo.itemRoot                = _selectModel.modelRoot.transform.Find("Single").gameObject;
            _selectModel.singleInfo.itemCard                = _selectModel.modelRoot.transform.Find("Single/Card").gameObject;
            _selectModel.singleInfo.itemButton              = _selectModel.modelRoot.transform.Find("Single/Image_Show").GetComponent<Image>();
            _selectModel.singleInfo.itemGreen               = _selectModel.modelRoot.transform.Find("Single/Image_Green").GetComponent<Image>();
            _selectModel.singleInfo.itemParticle0           = _selectModel.modelRoot.transform.Find("Single/Card/UI_show").gameObject;
            _selectModel.singleInfo.itemBk                  = _selectModel.modelRoot.transform.Find("Single/Card/Background").GetComponent<Image>();

            _selectModel.singleParticle                     = _selectModel.modelRoot.transform.Find("UI_Single").gameObject;
            _selectModel.multiParticle                      = _selectModel.modelRoot.transform.Find("UI_Multi").gameObject;
            _selectModel.waitting                           = _selectModel.modelRoot.transform.Find("Waitting").gameObject;
            _selectModel.timeList = new List<Image>();
            for (int i = 0; i < 3; ++i )
            {
                _selectModel.timeList.Add(_selectModel.waitting.transform.Find("TimeUp/Number" + i).GetComponent<Image>());
            }
            _selectModel.joinList = new List<Image>();
            for (int i = 0; i < 4; ++i )
            {
                _selectModel.joinList.Add(_selectModel.waitting.transform.Find("Head/Image" + i).GetComponent<Image>());
            }
        }

        {
            _selectMap                  = new SelectMap();
            _selectMap.mapRoot          = transform.Find("SelectMap").gameObject;
            _selectMap.map              = _selectMap.mapRoot.transform.Find("Map").gameObject;
            _selectMap.animator         = transform.Find("SelectMap/Down/Image1").GetComponent<Animator>();
            _selectMap.timeList         = new List<Image>();
            for (int i = 0; i < 3; ++i)
            {
                _selectMap.timeList.Add(_selectMap.mapRoot.transform.Find("TimeUp/Number" + i).GetComponent<Image>());
            }
        }
      
        
        _selectCar            = transform.Find("SelectCar").gameObject;
        _uiCar                = transform.Find("UICar").gameObject;
        loading               = _uiCar.transform.Find("Waitting").GetComponent<Image>();
       
        // 汽车
        {
            _carInfo = new List<CarInfo>();
            for (int i = 0; i < _selectCar.transform.childCount; ++i)
            {
                CarInfo info            = new CarInfo();
                info.obj                = _selectCar.transform.GetChild(i).gameObject;
                info.carInfo            = info.obj.GetComponent<CarSelect>();
                info.car                = info.obj.transform.Find("Player_Car" + i + "/Car").gameObject;
                info.localPosition      = _selectCar.transform.GetChild(i).position;
                info.localScale         = _selectCar.transform.GetChild(i).localScale;
                info.localEulerAngles   = _selectCar.transform.GetChild(i).localEulerAngles;
                info.disabled           = false;
                _carInfo.Add(info);
            }

            _headList = new List<HeadInfo>();
            for (int i = 0; i < 4; ++i )
            {
                HeadInfo headInfo   = new HeadInfo();
                headInfo.root       = transform.Find("UICar/Head/Head" + i.ToString()).gameObject;
                headInfo.head       = transform.Find("UICar/Head/Head" + i.ToString() + "/Head").GetComponent<Image>();
                headInfo.selected   = transform.Find("UICar/Head/Head" + i.ToString() + "/Selected").GetComponent<Image>();
                _headList.Add(headInfo);
            }

            _timeUpList = new List<Image>();
            for (int i = 0; i < 2; ++i)
            {
                _timeUpList.Add(transform.Find("UICar/TimeUp/Number" + i).GetComponent<Image>());
            }
        }
    }
}

