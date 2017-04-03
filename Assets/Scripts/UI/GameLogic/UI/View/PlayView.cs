/*
 * Copyright (c) 
 * 
 * 文件名称：   PlayView.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/21 17:33:26
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Need.Mx;

public class PlayView
{
    public PlayModule module;

    public List<Image> _coinList;

    // 时间
    public List<Image> _muniteList;
    public List<Image> _secondList;
    public List<Image> _muillsecondList;
    public List<Image> _timeOtherList;
    public List<GameObject> _effectList;
    public RectTransform _timeRect;

    // 开始倒计时
    public GameObject _readyGo;
    public Image _imageCountDown;

    // 头像信息
    public struct Head
    {
        public RectTransform root;
        public Image order;
        public List<Image> distanceList;
        public Image head;
    }
    public Head _head;

    // Follow
    public GameObject follow;
    public Image followHead;

    // 道具卡
    public struct Prop
    {
        public Image _transfrom;
        public Image _prop;
    }
    public Prop _prop;

    // boss来袭
    public GameObject _bossCome;

    // 完美躲避
    public GameObject _avoidPrefect;

    // boss
    public Image _boss;
    public Image _locked;

    // 连击
    public GameObject _comboRoot;
    public class ItemCombo
    {
        public GameObject root;
        public List<Image> numList;
        public Image combo;
        public RectTransform numRoot;
        public bool canUse;
    }
    public ItemCombo _origCombo;

    // 速度
    public List<Image> _speedList;
    public Image _imageSpeed;

    // 躲避成功
    public GameObject _avoidSuc;

    // 反击成功
    public GameObject _beatBack;

    // 完美超车
    public GameObject _overTake;

    // 劫持卡车
    public GameObject _hold;

    // 加速倍数
    public GameObject _scaleObj;
    public Image _speedScale;

    // 检查点增加时间
    public Image _checkTime;

    // 得分
    public class ScoreItem
    {
        public GameObject root;
        public Image image;
        public List<Image> scoreList;
    }
    public ScoreItem itemScore = new ScoreItem();
    private Transform tipsRoot;

    // 小地图
    public class ItemMinimap
    {
        public Image root;
        public Image head;
        public int id;
        public Image upArrow;
        public Image downArrow;
    }
    public List<ItemMinimap> miniList = new List<ItemMinimap>();
    public Image rank;

    // 游戏结束
    public GameObject _gameOver;

    public GameObject _effectAcc;

    public Dictionary<EnumTips, GameObject> tipsDic = new Dictionary<EnumTips, GameObject>();

    // Continue
    public class CoinRoot
    {
        public GameObject root;
        public Image coin;
    }
    public class ContinueRoot
    {
        public GameObject root;
        public List<Image> coinList;
        public Image pre;
    }
    public class Continue
    {
        public GameObject root;
        public CoinRoot Ccoin;
        public ContinueRoot Ccontinue;
        public Image time;
    }
    public Continue _continue;

    // 截屏
    public class Observe
    {
        public RawImage screen;
        public RawImage image;
        public GameObject point;
        public Image title;
    }
    public Observe _observe;

    public void Init(Transform transform)
    {
        module = ModuleManager.Instance.Get<PlayModule>();

         
        _coinList = new List<Image>();
        {
            for (int i = 0; i < transform.Find("Coin/Num").gameObject.transform.childCount; ++i )
            {
                _coinList.Add(transform.Find("Coin/Num/Image_Number" + i).GetComponent<Image>());
            }
        }

        {
            _muniteList = new List<Image>();
            for (int i = 0; i < 2; ++i)
            {
                _muniteList.Add(transform.Find("Time/Muinte/Image_Number" + i).GetComponent<Image>());
            }

            _secondList = new List<Image>();
            for (int i = 0; i < 2; ++i )
            {
                _secondList.Add(transform.Find("Time/Second/Image_Number" + i).GetComponent<Image>());
            }
            _timeOtherList = new List<Image>();
            {
                _timeOtherList.Add(transform.Find("Time/Image").GetComponent<Image>());
                _timeOtherList.Add(transform.Find("Time/Image1").GetComponent<Image>());
                _timeOtherList.Add(transform.Find("Time/Image0").GetComponent<Image>());
            }
            _muillsecondList = new List<Image>();
            for (int i = 0; i < 2; ++i)
            {
                _muillsecondList.Add(transform.Find("Time/Millisecond/Image_Number" + i).GetComponent<Image>());
            }

            _readyGo        = transform.Find("Ready_GO").gameObject;
            _imageCountDown = _readyGo.transform.Find("Image_Number0").GetComponent<Image>();
            _timeRect       = transform.Find("Time").GetComponent<RectTransform>();
            _effectList     = new List<GameObject>();
            for (int i = 0; i < 3; ++i )
            {
                _effectList.Add(transform.Find("Time/Effect/Effect_time_" + i).gameObject);
            }
        }

        {
            _head               = new Head();
            _head.root          = transform.Find("Head/Meters").GetComponent<RectTransform>();
            _head.order         = transform.Find("Head/Id").GetComponent<Image>();
            _head.head          = transform.Find("Head/Head").GetComponent<Image>();
            _head.distanceList = new List<Image>();
            for (int i = 0; i < 4; ++i )
            {
                _head.distanceList.Add(transform.Find("Head/Meters/Number" + i).GetComponent<Image>());
            }
        }

        {
            _prop = new Prop();
            _prop._transfrom    = transform.Find("Transform/Background").GetComponent<Image>();
            _prop._prop         = transform.Find("Transform/Prop").GetComponent<Image>();

        }

        {
            _checkTime  = transform.Find("Tips/CheckTime").GetComponent<Image>();
            _bossCome   = transform.Find("BossCome").gameObject;
            _avoidSuc   = transform.Find("Tips/Avoid").gameObject;
            _avoidPrefect = transform.Find("Tips/Prefect").gameObject;
            _boss       = transform.Find("Boss/Image").GetComponent<Image>();
            _locked     = transform.Find("Boss/Locked").GetComponent<Image>();
            _beatBack   = transform.Find("Tips/Beat").gameObject;
            _overTake   = transform.Find("Tips/OverTake").gameObject;
            _hold       = transform.Find("Tips/Hold").gameObject;
            tipsDic.Add(EnumTips.Avoid, _avoidSuc);
            tipsDic.Add(EnumTips.Hold, _hold);
            tipsDic.Add(EnumTips.OverTake, _overTake);
            tipsDic.Add(EnumTips.Prefect, _avoidPrefect);
            tipsDic.Add(EnumTips.Beat, _beatBack);

            _comboRoot          = transform.Find("Combo").gameObject;
            _origCombo          = new ItemCombo();
            _origCombo.root     = transform.Find("Combo/Item").gameObject;
            _origCombo.numRoot  = _origCombo.root.transform.Find("Num").GetComponent<RectTransform>();
            _origCombo.combo    = _origCombo.root.transform.Find("Level").GetComponent<Image>();
            _origCombo.canUse   = true;
            _origCombo.numList  = new List<Image>();
            for (int i = 0; i < 2; ++i )
            {
                _origCombo.numList.Add(_origCombo.numRoot.transform.Find("Number" + i).GetComponent<Image>());
            }
        }

        {
            _speedList = new List<Image>();
            _imageSpeed = transform.Find("Speed/Image").GetComponent<Image>();
            GameObject go = transform.Find("Speed/Num").gameObject;
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                _speedList.Add(go.transform.GetChild(i).GetComponent<Image>());
            }

            _scaleObj   = transform.Find("Tips/SpeedScale").gameObject;
            _speedScale = _scaleObj.transform.Find("Image").GetComponent<Image>();
            tipsDic.Add(EnumTips.SpeedScale, _scaleObj);
        }

        {
            tipsRoot = transform.Find("Tips").transform;
            itemScore.root = transform.Find("Tips/Score").gameObject;
            itemScore.scoreList = new List<Image>();
            itemScore.image = itemScore.root.GetComponent<Image>();
            for (int i = 0; i < 2; ++i )
            {
                itemScore.scoreList.Add(transform.Find("Tips/Score/Number" + i).GetComponent<Image>());
            }
        }

        // MiniMap
        {
            for (int i = 0; i < 4; ++i )
            {
                ItemMinimap item = new ItemMinimap();
                item.root       = transform.Find("MiniMap/Player" + i).GetComponent<Image>();
                item.head       = item.root.transform.Find("Head").GetComponent<Image>();
                item.upArrow    = item.root.transform.Find("UpArrow").GetComponent<Image>();
                item.downArrow  = item.root.transform.Find("DownArrow").GetComponent<Image>();
                miniList.Add(item);
            }
            rank = transform.Find("MiniMap/Image").GetComponent<Image>();
        }

        // 继续
        {
            _continue = new Continue();
            _continue.Ccoin = new CoinRoot();
            _continue.Ccontinue = new ContinueRoot();
            _continue.root = transform.Find("Continue").gameObject;
            _continue.time = _continue.root.transform.Find("Time").GetComponent<Image>();

            _continue.Ccoin.root = _continue.root.transform.Find("Coin").gameObject;
            _continue.Ccoin.coin = _continue.Ccoin.root.transform.Find("Coin").GetComponent<Image>();

            _continue.Ccontinue.root = _continue.root.transform.Find("Continue").gameObject;
            _continue.Ccontinue.coinList = new List<Image>();
            for (int i = 0; i < 2; ++i)
            {
                _continue.Ccontinue.coinList.Add(_continue.Ccontinue.root.transform.Find("Coin/Number" + i).GetComponent<Image>());
            }
            _continue.Ccontinue.pre = _continue.Ccontinue.root.transform.Find("Pre").GetComponent<Image>();
        }

        // Observe
        {
            _observe            = new Observe();
            _observe.screen     = transform.Find("Observe/RawImage").GetComponent<RawImage>();
            _observe.image      = transform.Find("Observe/Image").GetComponent<RawImage>();
            _observe.point      = transform.Find("Observe/Point").gameObject;
            _observe.title      = transform.Find("Observe/Title").GetComponent<Image>();
        }

        _effectAcc = transform.Find("UI_Effect_ACC").gameObject;

        follow      = transform.Find("Follow").gameObject;
        followHead  = transform.Find("Follow/Image").GetComponent<Image>();
        _gameOver   = transform.Find("GameOver").gameObject;
    }
    

    public ScoreItem NewItemScore()
    {
        ScoreItem item = new ScoreItem();
        item.root = GameObject.Instantiate(itemScore.root) as GameObject;
        item.root.transform.SetParent(tipsRoot);
        item.image = item.root.GetComponent<Image>();
        item.scoreList = new List<Image>();
        for (int i = 0; i < 2; ++i )
        {
            item.scoreList.Add(item.root.transform.Find("Number" + i).GetComponent<Image>());
        }
        return item;
    }

    public ItemCombo NewCombo()
    {
        ItemCombo combo = new ItemCombo();
        combo.canUse    = true;
        combo.root      = GameObject.Instantiate(_origCombo.root) as GameObject;
        combo.root.transform.SetParent(_comboRoot.transform);
        combo.numRoot   = _origCombo.root.transform.Find("Num").GetComponent<RectTransform>();
        combo.combo     = _origCombo.root.transform.Find("Level").GetComponent<Image>();
        combo.root.transform.localScale     = Vector3.one;
        combo.root.transform.localPosition  = Vector3.zero;
        combo.numList   = new List<Image>();
        for (int i = 0; i < 2; ++i )
        {
            combo.numList.Add(_origCombo.numRoot.transform.Find("Number" + i).GetComponent<Image>());
        }

        return combo;
    }
}
