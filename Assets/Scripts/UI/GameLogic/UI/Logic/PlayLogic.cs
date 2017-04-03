/*
 * Copyright (c) 
 * 
 * 文件名称：   PlayLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/10/21 17:33:14
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine.UI;

public class PlayLogic : BaseUI
{
    private PlayView view;
    private int _guidReadyGo = -1;
    private List<PlayView.ScoreItem> scoreList = new List<PlayView.ScoreItem>();
    private List<PlayView.ScoreItem> useScoreLists = new List<PlayView.ScoreItem>();
    private Dictionary<EnumTips, List<Dictionary<GameObject, float>>> tipsDic = new Dictionary<EnumTips, List<Dictionary<GameObject, float>>>();

    private Vector3 comboStartPos;
    private Color color0 = new Color(1, 1, 1, 0);
    private Color color1 = new Color(1, 1, 1, 1);

    private List<int> numList = new List<int>();
    #region override function

    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelMain;
    }

    protected override void OnAwake()
    {
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Fix_Frame, UpdateFixedFrame);
        MessageCenter.Instance.AddListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.AddListener(MessageType.Message_Boss_Come, OnBossCome);
        MessageCenter.Instance.AddListener(MessageType.Message_Magic, OnMagic);
        MessageCenter.Instance.AddListener(MessageType.Message_Speed_Accle, OnTurbo);
        MessageCenter.Instance.AddListener(MessageType.Message_Combo, OnCombo);
        MessageCenter.Instance.AddListener(MessageType.Message_Boss_On_Damage, OnBossDamage);
        MessageCenter.Instance.AddListener(MessageType.Message_Refresh_Coin, OnRefreshCoin);
        MessageCenter.Instance.AddListener(MessageType.Message_Show_Follow_Card, OnShowFollowCard);
        MessageCenter.Instance.AddListener(MessageType.Message_Game_End, OnEnd);
        MessageCenter.Instance.AddListener(MessageType.Message_Trigger_Tips, OnTips);
        MessageCenter.Instance.AddListener(MessageType.Message_Show_Score, OnShowScoe);
        MessageCenter.Instance.AddListener(MessageType.Message_Mode_Continue, OnContinue);
        MessageCenter.Instance.AddListener(MessageType.Message_Mode_Dead, OnDead);
        MessageCenter.Instance.AddListener(MessageType.Message_Add_Time, OnAddTime);
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Fix_Frame, UpdateFixedFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Update_Pre_Frame, UpdatePreFrame);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Boss_Come, OnBossCome);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Magic, OnMagic);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Speed_Accle, OnTurbo);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Combo, OnCombo);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Boss_On_Damage, OnBossDamage);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Refresh_Coin, OnRefreshCoin);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Show_Follow_Card, OnShowFollowCard);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Game_End, OnEnd);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Trigger_Tips, OnTips);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Show_Score, OnShowScoe);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Mode_Continue, OnContinue);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Mode_Dead, OnDead);
        MessageCenter.Instance.RemoveListener(MessageType.Message_Add_Time, OnAddTime);
        base.OnRelease();
    }

    protected override void OnStart()
    {
        view = new PlayView();
        view.Init(transform);
        Message message = new Message(MessageType.Message_View_Is_Open, this);
        message.Send();
        readyTime = -1;
        canScale = false;
        _isCombo = false;
        comboStartPos = view._origCombo.numRoot.transform.localPosition;
        //freeList.Add(view._origCombo);
        SoundController.Instance.StartEngineMusic(0);
        SoundController.Instance.StartReadyMusic();
        base.OnStart();
    }

    private bool hasReady;
    private void DoReady()
    {
        view._readyGo.SetActive(true);
        StartCoroutine(IEDoReady());
    }

    private int readyTime = -1;

    #endregion

    #region Message
    private void UpdatePreFrame(Message message)
    {
        if (LobbyManager.Instance.LobbyPlayer == null || GameMode.Instance.PlayerRef == null)
            return;

        if (LobbyManager.Instance.LobbyPlayer.ReadyTime == GameConfig.GAME_CONFIG_READY_TIME && !hasReady)
        {
            hasReady = true;
            DoReady();
        }

        if (LobbyManager.Instance.LobbyPlayer.ReadyTime < 3)
            InitMiniMap();
    }

    private float _time = 1.0f;
    private float _rand;
    private void UpdateFixedFrame(Message message)
    {
        if (GameMode.Instance.Mode < RunMode.ReadyToRace)
            return;

        int playerCount = GameMode.Instance.PlayerCount();

        // 入场次序
        if (GameMode.Instance.Mode >= RunMode.ReadyToRace)
            UpdateHeadInfo();

        // Continue
        if (view._continue.root.activeSelf)
        {
            view._continue.time.sprite = view.module.pleaseList[(int)GameMode.Instance.ContinueTime];
            // 续币倒计时
            UpdateContinue();
            if (GameMode.Instance.Mode == RunMode.SceneEnd)
                view._continue.root.SetActive(false);
        }

        if (GameMode.Instance.Mode < RunMode.Racing)
            return;

        // 防止加分频率太高，导致同步数据丢失
        int value = tempScore - GameMode.Instance.PlayerRef.Score;
        if (value != 0)
            GameMode.Instance.PlayerRef.AddScore(value);


        OnUpdateHitPoint();

        //更新时间和速度
        UpdateTimeAndSpeed();

        // 完美躲避
        UpdateAvoid();

        // 连击
        UpdateCombo();

        // 更新小地图
        UpdateMinimap();

        // 变身卡
        if (view._prop._transfrom.transform.parent.gameObject.activeSelf)
            view._prop._transfrom.transform.localEulerAngles += new Vector3(0, 0, 1);
    }

    /// <summary>
    ///  boss来袭
    /// </summary>
    int _guid = -1;
    private void OnBossCome(Message message)
    {
        _guid = TimerManager.Instance.AddTimerEvent(0, 3, 1, (_timeObj) =>
        {
            if (0 == _timeObj.CurTick)
            {
                view._bossCome.SetActive(true);
                view._boss.transform.parent.gameObject.SetActive(true);
                SoundController.Instance.StartBossWarningMusic();
            }

            if (3 == _timeObj.CurTick)
            {
                view._bossCome.SetActive(false);
                TimerManager.Instance.RemoveTimerEvent(_guid);
                SoundController.Instance.StopBossWarningMusic();
            }
        }, true);

    }

    /// <summary>
    /// 加速
    /// </summary>
    /// <param name="message"></param>
    private void OnTurbo(Message message)
    {
        bool flag = (bool)message["active"];
        int scale = (int)message["scale"];
        view._scaleObj.gameObject.SetActive(flag);
        view._effectAcc.SetActive(flag);
        view._speedScale.sprite = view.module.speedScaleNumList[scale];
    }

    /// <summary>
    /// 变身豹子, 机枪，引擎, 隐身
    /// </summary>
    /// <param name="message"></param>
    private void OnMagic(Message message)
    {
        bool flag = (bool)message["flag"];
        int index = (int)message["index"];
        view._prop._transfrom.transform.parent.gameObject.SetActive(flag);
        if (flag)
            view._prop._prop.sprite = view.module.magicList[index];
    }

    /// <summary>
    /// 提示信息 
    /// </summary>
    /// <param name="message"></param>
    private void OnTips(Message message)
    {
        int tips = (int)message["tips"];
        if (tips == (int)EnumTips.CheckTime)
            return;

        Vector3 pos = (Vector3)message["pos"];
        Canvas canvas = transform.GetComponent<Canvas>();
        Vector2 pos0 = Camera.main.WorldToScreenPoint(pos);
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos))
        {
            GameObject go = null;
            int flag = 0;
            if (CreateUIObj((EnumTips)tips, out go, out flag))
            {
                Dictionary<GameObject, float> dic = new Dictionary<GameObject, float>();
                dic.Add(go, 0.5f);
                FillTipsToDic((EnumTips)tips, dic);
            }
            else
                tipsDic[(EnumTips)tips][flag][go] = 0.5f;
            go.SetActive(true);
            go.transform.position = pos;
        }
    }

    /// <summary>
    /// 得分
    /// </summary>
    /// <param name="message"></param>
    private void OnShowScoe(Message message)
    {
        int score = (int)message["score"];
        NewScoreObj(score);
    }

    /// <summary>
    /// 继续倒计时
    /// </summary>
    /// <param name="message"></param>
    private void OnContinue(Message message)
    {
        bool show = (bool)message["show"];
        view._continue.root.SetActive(show);
        if (GameMode.Instance.CanPlay())
        {
            view._continue.Ccontinue.root.SetActive(true);
            view._continue.Ccoin.root.SetActive(false);
        }
        else
        {
            view._continue.Ccontinue.root.SetActive(false);
            view._continue.Ccoin.root.SetActive(true);
        }
    }

    /// <summary>
    /// 玩家死亡
    /// </summary>
    /// <param name="message"></param>
    private void OnDead(Message message)
    {
        view._continue.root.SetActive(false);
        PhotoManager.Instance.CaptureScreenDelegate += OnCaptureScreenEnd;
        PhotoManager.Instance.CaptureScreen();
    }

    private void OnCaptureScreenEnd()
    {
        PhotoManager.Instance.CaptureScreenDelegate -= OnCaptureScreenEnd;
        view._observe.screen.texture = PhotoManager.Instance.CaptureImage;
        view._observe.image.texture = PhotoManager.Instance.CaptureImage;
        view._observe.screen.gameObject.SetActive(true);
        StartCoroutine(ToObserveMode());
    }

    private int timeID;
    /// <summary>
    /// 播放加时动画
    /// </summary>
    /// <param name="message"></param>
    private void OnAddTime(Message message)
    {
        timeID = (int)message["index"];
        view._checkTime.sprite = view.module.addTimeList[timeID];
        view._checkTime.gameObject.SetActive(true);
        view._effectList[timeID].gameObject.SetActive(true);
        view._checkTime.rectTransform.DOLocalMoveY(400, 0.5f).OnComplete(OnAddTimeEnd);
    }

    private int total = 4;
    private void OnBossDamage(Message message)
    {
        view._boss.fillAmount = (float)message["ratio"];
        if (view._boss.fillAmount == 0)
            view._boss.transform.parent.gameObject.SetActive(false);
    }

    private void OnShowFollowCard(Message message)
    {
        int id = (int)message["id"];
        bool active = id == -1 ? false : true;
        view.follow.SetActive(active);
        if (active)
        {
            view.followHead.sprite = view.module.head1List[id];
            Vector3 pos = GameMode.Instance.GetPosByID(id);
            pos.x = 800 * (pos.x / 3.7f);
            pos.y = -524;
            pos.z = 0;
            view.follow.GetComponent<RectTransform>().localPosition = pos;
        }
    }

    private bool canScale;
    private void OnUpdateHitPoint()
    {
        if (null == GameMode.Instance.Airplane)
            return;


        Vector3 pos = GameMode.Instance.Airplane._attackPoint.transform.position;
        float factor = pos.z - GameMode.Instance.PlayerRef.transform.position.z;
        factor = 30 / factor;

        if (!view._locked.gameObject.activeSelf)
            view._locked.transform.DOScale(Vector3.one * (factor > 0.5f ? 0.5f : factor) * 0.5f, 0.5f).OnComplete(EndScale);

        if (factor < 1.5f && factor >= 0.1f)
        {
            view._locked.gameObject.SetActive(true);

            Canvas canvas = GetComponent<Canvas>();
            Vector2 pos0 = Camera.main.WorldToScreenPoint(pos);
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform, pos0, canvas.worldCamera, out pos))
                view._locked.transform.position = pos;
            if (canScale)
                view._locked.transform.localScale = Vector3.one * (factor > 0.5f ? 0.5f : factor) * 0.5f;
        }
        else if (factor < 0.1f)
            view._locked.gameObject.SetActive(false);
    }

    private void OnEnd(Message message)
    {
        view._gameOver.SetActive(true);
        StartCoroutine(GameOver());
    }

    /// <summary>
    /// 更新币数
    /// </summary>
    /// <param name="message"></param>
    private void OnRefreshCoin(Message message)
    {
        int coin = (int)message["coin"];

        int value0 = (int)((coin / 10) % 10);
        int value1 = (int)((coin / 1) % 10);

        view._coinList[0].sprite = view.module.coinNumList[value0];
        view._coinList[1].sprite = view.module.coinNumList[value1];
        view._coinList[2].sprite = view.module.coinNumList[GameConfig.GAME_CONFIG_PER_USE_COIN];

        view._coinList[0].rectTransform.sizeDelta = view.module.coinNumList[value0].rect.size;
        view._coinList[1].rectTransform.sizeDelta = view.module.coinNumList[value1].rect.size;
        view._coinList[2].rectTransform.sizeDelta = view.module.coinNumList[GameConfig.GAME_CONFIG_PER_USE_COIN].rect.size;
    }
    #endregion

    #region Private Function
    /// <summary>
    /// 倒计时准备开始游戏
    /// </summary>
    private void ReadyToRace(int time)
    {
        if (time < 0)
        {
            if (-1 == time)
                SoundController.Instance.PlayPersonSoundReadyGo();
            return;
        }

        view._imageCountDown.sprite = view.module.countDownList[time];
        view._imageCountDown.transform.localScale = Vector3.one;
        view._imageCountDown.rectTransform.sizeDelta = view.module.countDownList[time].rect.size;
        view._imageCountDown.color = color1;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._imageCountDown.transform.DOScale(Vector3.one * 3, 0.8f));
        sequence.Join(view._imageCountDown.DOColor(color0, 0.8f));
    }

    /// <summary>
    /// 初始化小地图
    /// </summary>
    private void InitMiniMap()
    {
        int playerCount = GameMode.Instance.PlayerCount();
        for (int i = 0; i < playerCount; ++i)
        {
            if (view.miniList[i].root.gameObject.activeSelf)
                continue;
            int id = GameMode.Instance.GetIDByOrder(i);
            if (id < 0)
                continue;
            if (id == GameMode.Instance.PlayerRef.ID)
                view.miniList[i].root.enabled = true;
            else
                view.miniList[i].root.enabled = false;
            view.miniList[i].head.sprite = view.module.head1List[id];
            view.miniList[i].id = id;
            view.miniList[i].root.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 更新头像信息
    /// </summary>
    /// <param name="myID"></param>
    /// <param name="playerCount"></param>
    private void UpdateHeadInfo()
    {
        view._head.head.sprite = view.module.head0List[GameMode.Instance.PlayerRef.ID];
        view._head.order.sprite = view.module.orderNumList[GameMode.Instance.PlayerRef.UniqueOrder];
    }

    private int tempScore;
    private void UpdateScoreInfo()
    {
        if (numList.Count > 0)
        {
            tempScore += numList[0];
            numList.RemoveAt(0);
        }

        int value0 = (int)((tempScore / 1000) % 10);
        int value1 = (int)(((tempScore - 1000 * value0) / 100) % 10);
        int value2 = (int)(((tempScore - 1000 * value0 - 100 * value1) / 10) % 10);
        int value3 = (int)((tempScore - 1000 * value0 - 100 * value1 - 10 * value2) % 10);
        view._head.distanceList[0].sprite = view.module.distanceNumList[value0];
        view._head.distanceList[1].sprite = view.module.distanceNumList[value1];
        view._head.distanceList[2].sprite = view.module.distanceNumList[value2];
        view._head.distanceList[3].sprite = view.module.distanceNumList[value3];


        int rank = GameMode.Instance.GetRankByID(GameMode.Instance.PlayerRef.ID);
        view.rank.sprite = view.module.rankNumList[rank];

        ScoreAnimation();
    }

    private bool _scoreAni;
    private void ScoreAnimation()
    {
        if (_scoreAni)
            return;
        _scoreAni = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._head.root.DOScale(Vector3.one * 1.3f, 0.1f));
        sequence.Append(view._head.root.DOScale(Vector3.one, 0.1f));
        sequence.OnComplete(OnScoreAniEnd);
    }

    private void OnScoreAniEnd()
    {
        _scoreAni = false;
    }

    /// <summary>
    /// 更新小地图坐标
    /// </summary>
    private void UpdateMinimap()
    {
        if (GameMode.Instance.Mode == RunMode.SceneEnd)
            return;

        for (int i = 0; i < GameMode.Instance.PlayerCount(); ++i)
        {
            int id = GameMode.Instance.GetIDByOrder(i);
            Vector3 pos;
            float dis = GameMode.Instance.CompareDisWithLocal(id, out pos);

            if (dis >= 110)
                view.miniList[i].upArrow.gameObject.SetActive(true);
            else if (dis <= -110)
                view.miniList[i].downArrow.gameObject.SetActive(true);
            else
            {
                pos = Util.GetMiniMapPos(dis, pos.x, GameMode.Instance.IsLocalPlayer(id));
                view.miniList[i].root.transform.localPosition = pos;
                view.miniList[i].downArrow.gameObject.SetActive(false);
                view.miniList[i].upArrow.gameObject.SetActive(false);
            }
        }

    }

    /// <summary>
    /// 检查点倒计时
    /// </summary>
    private void UpdateContinue()
    {
        int value = (int)GameMode.Instance.ContinueTime;
        view._continue.time.sprite = view.module.timeNumList[value];

        if (view._continue.Ccoin.root.activeSelf)
        {
            value = GameMode.Instance.ModeData.Coin;
            value = GameConfig.GAME_CONFIG_PER_USE_COIN - value;
            view._continue.Ccoin.coin.sprite = view.module.pleaseList[value];
        }
        else if (view._continue.Ccontinue.root.activeSelf)
        {
            value = GameMode.Instance.ModeData.Coin;
            int value0 = (value / 10) % 10;
            int value1 = (value - 10 * value0) % 10;
            view._continue.Ccontinue.coinList[0].sprite = view.module.pleaseList[value0];
            view._continue.Ccontinue.coinList[1].sprite = view.module.pleaseList[value1];
            view._continue.Ccontinue.pre.sprite = view.module.pleaseList[GameConfig.GAME_CONFIG_PER_USE_COIN];
        }
    }

    /// <summary>
    /// 设置时间颜色
    /// </summary>
    private bool _hasSetRed;
    private void SetTimeColor(Color color)
    {
        for (int i = 0; i < view._muniteList.Count; ++i)
            view._muniteList[i].color = color;
        for (int i = 0; i < view._secondList.Count; ++i)
            view._secondList[i].color = color;
        for (int i = 0; i < view._timeOtherList.Count; ++i)
            view._timeOtherList[i].color = color;
        for (int i = 0; i < view._muillsecondList.Count; ++i)
            view._muillsecondList[i].color = color;
    }

    /// <summary>
    /// 加时消失
    /// </summary>
    private void OnAddTimeEnd()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._timeRect.DOScale(Vector3.one * 1.5f, 0.1f));
        sequence.Append(view._timeRect.DOScale(Vector3.one, 0.1f));
        sequence.OnComplete(OnEndTime);
    }

    private void OnEndTime()
    {
        view._checkTime.DOColor(color0, 0.2f).OnComplete(OnEffectEnd);
    }

    private void OnEffectEnd()
    {
        view._checkTime.gameObject.SetActive(false);
        view._checkTime.color = color1;
        view._checkTime.rectTransform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 创建UI分数
    /// </summary>
    /// <param name="value"></param>
    private void NewScoreObj(int value)
    {
        PlayView.ScoreItem item = null;
        if (scoreList.Count > 0)
        {
            item = scoreList[0];
            scoreList.RemoveAt(0);
            useScoreLists.Add(item);
        }

        if (null == item)
        {
            item = view.NewItemScore();
            useScoreLists.Add(item);
        }

        GameMode.Instance.PlayerRef.AddScore(value);
        numList.Add(value);

        int value0 = 0;
        int value1 = 0;
        if (value > 9)
        {
            value0 = (value / 10);
            value1 = (value - 10 * value0);
            item.scoreList[0].sprite = view.module.pleaseList[value0];
            item.scoreList[1].gameObject.SetActive(true);
            item.scoreList[1].sprite = view.module.pleaseList[value1];
        }
        else
        {
            value0 = value;
            item.scoreList[0].sprite = view.module.pleaseList[value0];
            item.scoreList[1].gameObject.SetActive(false);
        }

        item.root.transform.localScale = Vector3.one * 0.5f;
        item.root.transform.localPosition = Vector3.zero;
        item.root.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(item.root.transform.DOScale(Vector3.one * 1.1f, 0.2f));
        sequence.Append(item.root.transform.DOScale(Vector3.one, 0.1f));
        sequence.Append(item.root.transform.DOLocalMoveY(200, 0.5f));
        sequence.Append(item.root.transform.DOLocalMove(new Vector3(-700, 342, 0), 0.2f));
        sequence.Append(item.image.DOColor(color0, 0.1f));
        sequence.Join(item.scoreList[0].DOColor(color0, 0.1f));
        sequence.Join(item.scoreList[1].DOColor(color0, 0.1f));
        sequence.OnComplete(OnScoreMoveEnd);
    }

    /// <summary>
    /// 分数UI移动
    /// </summary>
    private void OnScoreMoveEnd()
    {
        PlayView.ScoreItem item = null;
        item = useScoreLists[0];
        useScoreLists.RemoveAt(0);
        item.root.SetActive(false);
        item.image.color = color1;
        item.scoreList[0].color = color1;
        item.scoreList[1].color = color1;
        scoreList.Add(item);

        UpdateScoreInfo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tips"></param>
    /// <param name="dic"></param>
    private void FillTipsToDic(EnumTips tips, Dictionary<GameObject, float> dic)
    {
        if (!tipsDic.ContainsKey(tips))
        {
            List<Dictionary<GameObject, float>> lst = new List<Dictionary<GameObject, float>>();
            lst.Add(dic);
            tipsDic.Add(tips, lst);
        }
        else
            tipsDic[tips].Add(dic);
    }

    private int _numCombo;
    private float _coolTimer;
    private bool _isCombo;
    /// <summary>
    /// 连击
    /// </summary>
    /// <param name="message"></param>
    private void OnCombo(Message message)
    {
        if (!view._comboRoot.activeSelf)
            view._comboRoot.SetActive(true);

        ++_numCombo;
        if (GameMode.Instance.PlayerRef.MaxCombo < _numCombo)
            GameMode.Instance.PlayerRef.MaxCombo = _numCombo;
        if (_numCombo == 5)
            SoundController.Instance.PlayPersonSoundGood();
        else if (_numCombo == 10)
            SoundController.Instance.PlayPersonSoundGreat();
        else if (_numCombo == 20)
            SoundController.Instance.PlayPersonSoundGreat();

        _coolTimer = 2;
        ComboAnimation();
    }

    private void RestCombo()
    {
        _isCombo = false;
        view._origCombo.root.SetActive(false);
        view._origCombo.root.transform.localPosition = Vector3.zero;
        view._origCombo.root.transform.localScale = Vector3.one;
        view._origCombo.combo.color = color1;
        view._origCombo.combo.transform.localScale = Vector3.one;
        view._origCombo.numRoot.transform.localPosition = comboStartPos;
        for (int i = 0; i < view._origCombo.numList.Count; ++i)
        {
            view._origCombo.numList[i].color = color1;
        }
    }

    /// <summary>
    /// combo动画
    /// </summary>
    private void ComboAnimation()
    {
        if (_isCombo)
            return;

        _isCombo = true;
        view._origCombo.root.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._origCombo.combo.transform.DOScale(Vector3.one * 1.5f, 0.3f));
        sequence.Join(view._origCombo.numRoot.transform.DOLocalMoveY(comboStartPos.y + 50, 0.3f));

        sequence.Append(view._origCombo.combo.transform.DOScale(Vector3.one, 0.2f));

        sequence.Append(view._origCombo.combo.DOColor(color0, 0.1f));
        sequence.Join(view._origCombo.numList[0].DOColor(color0, 0.1f));
        sequence.Join(view._origCombo.numList[1].DOColor(color0, 0.1f));
        sequence.OnComplete(RestCombo);
    }

    /// <summary>
    /// 创建UI对象
    /// </summary>
    /// <param name="tips"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    private bool CreateUIObj(EnumTips tips, out GameObject go, out int flag)
    {
        if (tipsDic.ContainsKey(tips) && tipsDic[tips].Count > 0)
        {
            for (int i = 0; i < tipsDic[tips].Count; ++i)
            {
                foreach (KeyValuePair<GameObject, float> kvp in tipsDic[tips][i])
                {
                    if (kvp.Value <= 0)
                    {
                        go = kvp.Key;
                        flag = i;
                        return false;
                    }
                }
            }
        }

        flag = -1;
        go = GameObject.Instantiate(view.tipsDic[tips]);
        go.transform.SetParent(transform);
        go.transform.localScale = Vector3.one;
        return true;
    }

    private void EndScale()
    {
        canScale = true;
    }

    private void UpdateTimeAndSpeed()
    {
        if (GameMode.Instance.Mode == RunMode.SceneEnd)
            return;

        // 时间
        int value = (int)GameMode.Instance.RemainTime;

        if (value <= 15 && !_hasSetRed)
        {
            _hasSetRed = true;
            SetTimeColor(Color.red);
            StartCoroutine(PlayCountDownSound());
        }
        else if (value > 15)
        {
            _hasSetRed = false;
            SetTimeColor(Util.Value2Color(255, 252, 255, 34));
        }

        int value0 = (int)(value / 60) % 10;
        view._muniteList[1].sprite = view.module.timeNumList[value0];
        value0 = (int)(value - 60 * value0);
        int value1 = (int)(value0 / 10) % 10;
        int value2 = (int)(value0 - 10 * value1) % 10;
        view._secondList[0].sprite = view.module.timeNumList[value1];
        view._secondList[1].sprite = view.module.timeNumList[value2];

        value0 = (int)((GameMode.Instance.RemainTime - value) * 10);
        value1 = (int)(value0 / 10) % 10;
        value2 = (int)(value0 - 10 * value1) % 10;
        view._muillsecondList[0].sprite = view.module.timeNumList[value1];
        view._muillsecondList[1].sprite = view.module.timeNumList[value2];

        // 速度
        value = (int)(GameMode.Instance.PlayerRef.Speed * 5 + _rand);
        if (value > 100)
        {
            if (_time > 0)
                _time -= Time.fixedDeltaTime;
            else
            {
                _time = 1.0f;
                _rand = Util.Random(0, 6);
            }
        }
        value0 = (int)(value / 100);
        value1 = (int)((value - 100 * value0) / 10);
        value2 = (int)((value - 100 * value0 - 10 * value1));
        view._speedList[0].sprite = view.module.speedNumList[value0];
        view._speedList[1].sprite = view.module.speedNumList[value1];
        view._speedList[2].sprite = view.module.speedNumList[value2];
        view._imageSpeed.fillAmount = value / 500.0f;
    }

    private Dictionary<EnumTips, List<Dictionary<GameObject, float>>>.Enumerator enumerator;
    private void UpdateAvoid()
    {
        enumerator = tipsDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            for (int i = 0; i < enumerator.Current.Value.Count; ++i)
            {
                List<GameObject> list = new List<GameObject>(enumerator.Current.Value[i].Keys);
                if (list.Count > 0)
                {
                    for (int j = 0; j < list.Count; ++j)
                    {
                        if (enumerator.Current.Value[i][list[j]] > 0)
                            enumerator.Current.Value[i][list[j]] -= Time.fixedDeltaTime;
                        else
                        {
                            enumerator.Current.Value[i][list[j]] = 0;
                            list[j].SetActive(false);
                        }
                    }
                }
            }
        }
    }

    private void UpdateCombo()
    {
        if (_coolTimer > 0)
            _coolTimer -= Time.fixedDeltaTime;
        else
        {
            _coolTimer = 0;
            _numCombo = 0;
        }

        int value0 = (_numCombo / 10) % 10;
        int value1 = _numCombo - 10 * value0;
        if (_numCombo < 10)
        {
            view._origCombo.numList[1].gameObject.SetActive(false);
            view._origCombo.numList[0].sprite = view.module.comboNumList[value1];
        }
        else
        {
            view._origCombo.numList[1].gameObject.SetActive(true);
            view._origCombo.numList[0].sprite = view.module.comboNumList[value0];
            view._origCombo.numList[1].sprite = view.module.comboNumList[value1];
        }

        if (_numCombo < 5)
            view._origCombo.combo.sprite = view.module.greatList[0];
        else if (_numCombo >= 5 && _numCombo <= 10)
            view._origCombo.combo.sprite = view.module.greatList[1];
        else if (_numCombo > 10 && _numCombo < 20)
            view._origCombo.combo.sprite = view.module.greatList[2];
        else
            view._origCombo.combo.sprite = view.module.greatList[3];
    }

    private void OnScreenEnd()
    {
        view._observe.screen.texture = null;
        view._observe.screen.gameObject.SetActive(false);
        view._observe.image.gameObject.SetActive(true);
        view._observe.title.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._observe.image.rectTransform.DOLocalMove(new Vector3(470, -390, 0), 0.7f));
        sequence.Join(view._observe.image.rectTransform.DOScale(Vector3.one * 1.3f, 0.7f));
        sequence.OnComplete(OnFlip);
    }

    private void OnFlip()
    {
        view._observe.image.rectTransform.DOScale(Vector3.one, 0.3f);
    }
    #endregion

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2);

        view._gameOver.SetActive(false);
        view._observe.image.gameObject.SetActive(false);
        UIManager.Instance.OpenUI(EnumUIType.PanelEnd);
    }

    IEnumerator ToObserveMode()
    {
        yield return new WaitForSeconds(1f);
        Message message = new Message(MessageType.Message_Change_Observe, this);
        message.Send();
        yield return new WaitForSeconds(0.5f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(view._observe.screen.transform.DOScale(Vector3.zero, 1.5f));
        sequence.Join(view._observe.screen.transform.DOMove(view._observe.point.transform.position, 1.5f));
        sequence.OnComplete(OnScreenEnd);
    }

    IEnumerator PlayCountDownSound()
    {
        int num = 15;
        while (num > 2)
        {
            if ((int)GameMode.Instance.RemainTime > 15)
                yield break;

            if (num > 3)
                SoundController.Instance.PlaySound14_4();
            else
                SoundController.Instance.PlaySound3();
            --num;
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator IEDoReady()
    {
        while (readyTime < view.module.countDownList.Count)
        {
            ReadyToRace(readyTime);
            ++readyTime;
            yield return new WaitForSeconds(1);
        }

        view._readyGo.SetActive(false);
        GameMode.Instance.ChangeMode(RunMode.Racing);
        SoundController.Instance.StopReadyMusic();
        SoundController.Instance.StartSceneGameMusic((int)EnvirManager.Instance.CurrentEnvirType);
        SoundController.Instance.StartEngineMusic(1);
    }
}
