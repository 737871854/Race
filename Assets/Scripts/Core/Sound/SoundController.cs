using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Need.Mx;


public class SoundController : SingletonBehaviour<SoundController>
{

    [SerializeField]
    public SoundLibrary library;

    public SoundLibrary Library
    {
        get { return library; }
        set { library = value; }
    }

    static int soundIDCounter = 0;

    static int GetSoundID()
    {
        soundIDCounter++;
        return soundIDCounter;
    }

    private Dictionary<string, SoundEvent> eventsDict = new Dictionary<string, SoundEvent>();
    private int standByMusicId;
    private int standByMusicIndex;
    private int sceneMusicId;
    private int sceneEffectId;
    private int sceneTempId;
    private int resultMusicId;
    private int bossMusicId;
    private int bossComingId;
    private int engineId;
    private int gunId;
    private int leopardId;
    private int selectCarId;
    private int endingAccountId;

    // Use this for initialization
    public void Init()
    {
        sceneMusicId = -1;
        sceneTempId = 0;
        standByMusicId = -1;
        standByMusicIndex = 0;
        resultMusicId = -1;
        bossMusicId = 0;
        bossComingId = -1;
        engineId = 0;
        gunId = -1;
        leopardId = 0;
        selectCarId = -1;
        endingAccountId = -1;
        library.Init();
        InitData();
    }

    void InitData()
    {
        // Music
        // 待机背景音效
        AddSoundEvent("StartMusic", "name=Music_Idle_Start", "sound=Music_Idle_Start", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Idle_Stop", "sound=Music_Idle_Start", "fadeTime=0");

        // 选择界面背景音乐
        AddSoundEvent("StartMusic", "name=Music_Select_Role_Start", "sound=Music_Select_Role", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Select_Role_Stop", "sound=Music_Select_Role", "fadeTime=0");

        // 游戏背景音效  万圣节
        AddSoundEvent("StartMusic", "name=Music_Game_Background_1_Start", "sound=Music_Game_Background_1", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Game_Background_1_Stop", "sound=Music_Game_Background_1", "fadeTime=0");

        // 游戏背景音效 海底
        AddSoundEvent("StartMusic", "name=Music_Game_Background_2_Start", "sound=Music_Game_Background_2", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Game_Background_2_Stop", "sound=Music_Game_Background_2", "fadeTime=0");

        // 游戏背景音效 小镇
        AddSoundEvent("StartMusic", "name=Music_Game_Background_0_Start", "sound=Music_Game_Background_0", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Game_Background_0_Stop", "sound=Music_Game_Background_0", "fadeTime=0");

        // Boss背景音乐0
        AddSoundEvent("StartMusic", "name=Music_Boss_Step0_Start", "sound=Music_Boss_Step0", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Boss_Step0_Stop", "sound=Music_Boss_Step0", "fadeTime=0");

        // Boss背景音乐1
        AddSoundEvent("StartMusic", "name=Music_Boss_Step1_Start", "sound=Music_Boss_Step1", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Boss_Step1_Stop", "sound=Music_Boss_Step1", "fadeTime=0");

        // Boss警告
        AddSoundEvent("StartMusic", "name=Music_Boss_Warning_Start", "sound=Music_Boss_Warning", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Boss_Warning_Stop", "sound=Music_Boss_Warning", "fadeTime=0");

        // 引擎加速
        AddSoundEvent("StartMusic", "name=Music_Engine_2_Start", "sound=Music_Engine_Acc", "startEndVolume=0,0.6", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Engine_2_Stop", "sound=Music_Engine_Acc", "fadeTime=0");

        // 引擎准备
        AddSoundEvent("StartMusic", "name=Music_Engine_0_Start", "sound=Music_Engine_Ready", "startEndVolume=0,0.6", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Engine_0_Stop", "sound=Music_Engine_Ready", "fadeTime=0");

        // 引擎普通行驶
        AddSoundEvent("StartMusic", "name=Music_Engine_1_Start", "sound=Music_Engine_Run", "startEndVolume=0,0.1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Engine_1_Stop", "sound=Music_Engine_Run", "fadeTime=0");

        // 机枪
        AddSoundEvent("StartMusic", "name=Music_Gun_Fire_Start", "sound=Music_Gun_Fire", "startEndVolume=0,0.5", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Gun_Fire_Stop", "sound=Music_Gun_Fire", "fadeTime=0");

        // 豹子奔跑
        AddSoundEvent("StartMusic", "name=Music_Leopard_Run_Start", "sound=Music_Leopard_Run", "startEndVolume=0,0.6", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Leopard_Run_Stop", "sound=Music_Leopard_Run", "fadeTime=0");

        // 结算画面
        AddSoundEvent("StartMusic", "name=Music_Panel_End_Start", "sound=Music_Panel_End", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Panel_End_Stop", "sound=Music_Panel_End", "fadeTime=0");

        // 游戏准备
        AddSoundEvent("StartMusic", "name=Music_Ready_Start", "sound=Music_Ready", "startEndVolume=0,1", "fadeTime=1", "loop=true");
        AddSoundEvent("StopMusic", "name=Music_Ready_Stop", "sound=Music_Ready", "fadeTime=0");

        // 选择界面变豹子声音
        AddSoundEvent("StartMusic", "name=SFX_Voice_Sound_ChangeLeopard_Idle_Start", "sound=SFX_Voice_Sound_ChangeLeopard_Idle", "startEndVolume=0,1", "fadeTime=1", "loop=false");
        AddSoundEvent("StopMusic", "name=SFX_Voice_Sound_ChangeLeopard_Idle_Stop", "sound=SFX_Voice_Sound_ChangeLeopard_Idle", "fadeTime=0");

        // 结算数数
        AddSoundEvent("StartMusic", "name=Music_Account_Start", "sound=Music_Account", "startEndVolume=0,1", "fadeTime=1", "loop=false");
        AddSoundEvent("StopMusic", "name=Music_Account_Stop", "sound=Music_Account", "fadeTime=0");


        // Sound
        // 投币音效
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_InsertCoin", "sound=SFX_Voice_Sound_InsertCoin");
        // 拍照
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Photo", "sound=SFX_Voice_Sound_Photo");
        // Boss死亡
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Boss_Die", "sound=SFX_Voice_Sound_Boss_Die");
        // 切换选择时播放
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Change", "sound=SFX_Voice_Sound_Change");
      
        // 游戏中变豹子声音
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_ChangeToLeopard_Game", "sound=SFX_Voice_Sound_ChangeToLeopard_Game");
        // 导弹
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Guided", "sound=SFX_Voice_Sound_Guided");
        // 卡车
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Hold", "sound=SFX_Voice_Sound_Hold");
        // 地雷
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_LandMine", "sound=SFX_Voice_Sound_LandMine");
        // 狼嚎
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Wolf", "sound=SFX_Voice_Sound_Wolf");
        // 猫头鹰
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Owl", "sound=SFX_Voice_Sound_Owl");
        // 道具
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Prop", "sound=SFX_Voice_Sound_Prop");
        // 确定
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Sure", "sound=SFX_Voice_Sound_Sure");
        // 汽车碰撞
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Collision", "sound=SFX_Voice_Sound_Collision");
        // 没用的特效音效
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_NoUse", "sound=SFX_Voice_Sound_NoUse");
        // 导弹爆炸
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Guided_Explosion", "sound=SFX_Voice_Sound_Guided_Explosion");
      
        //过检查点
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_AddTime", "sound=SFX_Voice_Sound_AddTime");
        // 跳跃车
        AddSoundEvent("PlaySound", "name=SFX_Voice_Skip_Sound", "sound=SFX_Voice_Skip_Sound");
        // 完美躲避
        AddSoundEvent("PlaySound", "name=SFX_Voice_Dodge_Sound", "sound=SFX_Voice_Dodge_Sound");
        // 落地
        AddSoundEvent("PlaySound", "name=SFX_Voice_Fall_Ground_Sound", "sound=SFX_Voice_Fall_Ground_Sound");
        //// UFO吃光豆的音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_UfoEatProps01", "sound=SFX_Voice_UfoEatProps01");
        //// 光豆飞翔几十分板音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_LightBeanFly", "sound=SFX_Voice_LightBeanFly");
        //// 分数增加音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_BonusPoint", "sound=SFX_Voice_BonusPoint");
        //// UFO碰到道具时播放
        //AddSoundEvent("PlaySound", "name=SFX_Voice_UfoEatProps02", "sound=SFX_Voice_UfoEatProps02");
        //// UFO发射炮弹时音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_UfoFire", "sound=SFX_Voice_UfoFire");
        //// UFO碰撞时音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_UfoCollide01", "sound=SFX_Voice_UfoCollide01");
        //// 敌UFO发射光波音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_NpcUfoFire", "sound=SFX_Voice_NpcUfoFire");
        //// Boss导弹发射音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_BossFire", "sound=SFX_Voice_BossFire");
        //// 爆炸音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Bomb", "sound=SFX_Voice_Bomb");
        //// 雪柱喷涌音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_SnowColumn", "sound=SFX_Voice_SnowColumn");
        //// 石柱砸向路面的音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_StelaeCollide", "sound=SFX_Voice_StelaeCollide");
        //// 烟花弹音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Fireworks01", "sound=SFX_Voice_Fireworks01");
        //// 剩余10秒时提醒音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_CountDown", "sound=SFX_Voice_CountDown");

        //// 达到目的地音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Finishing", "sound=SFX_Voice_Finishing");
        //// 分数变化音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_StatisticalScore", "sound=SFX_Voice_StatisticalScore");
        //// 奖励勋章音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Medal", "sound=SFX_Voice_Medal");
        //// 礼花粒子音效
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Firework02", "sound=SFX_Voice_Firework02");

        // 中文
        // 游戏结束
        AddSoundEvent("PlaySound", "name=Voice_C_gameover", "sound=Voice_C_gameover");
        // 都让开
        AddSoundEvent("PlaySound", "name=Voice_C_Getaway", "sound=Voice_C_Getaway");
        // 冲冲
        AddSoundEvent("PlaySound", "name=Voice_C_Go", "sound=Voice_C_Go");
        // 赞
        AddSoundEvent("PlaySound", "name=Voice_C_Goodjob", "sound=Voice_C_Goodjob");
        // 狂赞
        AddSoundEvent("PlaySound", "name=Voice_C_Greatjob", "sound=Voice_C_Greatjob");
        // 隐身
        AddSoundEvent("PlaySound", "name=Voice_C_INVISIBLE", "sound=Voice_C_INVISIBLE");
        // 准备拍照
        AddSoundEvent("PlaySound", "name=Voice_C_Photo ready", "sound=Voice_C_Photo ready");
        // 获得前两名可以免费打印相片
        AddSoundEvent("PlaySound", "name=Voice_C_photograph01", "sound=Voice_C_photograph01");
        // 相片正在打印中
        AddSoundEvent("PlaySound", "name=Voice_C_photograph02", "sound=Voice_C_photograph02");
        // 游戏开始啦
        AddSoundEvent("PlaySound", "name=Voice_C_Start", "sound=Voice_C_Start");
        // 太棒了
        AddSoundEvent("PlaySound", "name=Voice_C_Wowgreat", "sound=Voice_C_Wowgreat");
        // 耶
        AddSoundEvent("PlaySound", "name=Voice_C_yeah", "sound=Voice_C_yeah");
        // 3
        AddSoundEvent("PlaySound", "name=Voice_C_3", "sound=Voice_C_3");
        // 2
        AddSoundEvent("PlaySound", "name=Voice_C_2", "sound=Voice_C_2");
        // 1
        AddSoundEvent("PlaySound", "name=Voice_C_1", "sound=Voice_C_1");
        // 游戏倒计时 14-4秒
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_14_4", "sound=SFX_Voice_Sound_14_4");
        // 游戏倒计时 3-0秒
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_3", "sound=SFX_Voice_Sound_3");
        // 拍照倒计时
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Photo_CountDown", "sound=SFX_Voice_Sound_Photo_CountDown");

        // 英文
        // 加速
        AddSoundEvent("PlaySound", "name=SFX_Voice_Sound_Ready_GO", "sound=SFX_Voice_Sound_Ready_GO");
        //// 小心
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Becareful_EN_01", "sound=SFX_Voice_Becareful_EN_01");
        //// 小心
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Becareful_EN_02", "sound=SFX_Voice_Becareful_EN_02");
        //// 小心
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Becareful_EN_03", "sound=SFX_Voice_Becareful_EN_03");
        //// GO
        //AddSoundEvent("PlaySound", "name=SFX_Voice_GO_EN", "sound=SFX_Voice_GO_EN");
        //// 奖章1
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Madal_EN_01", "sound=SFX_Voice_Madal_EN_01");
        //// 奖章2
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Madal_EN_02", "sound=SFX_Voice_Madal_EN_02");
        //// 奖章3
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Madal_EN_03", "sound=SFX_Voice_Madal_EN_03");
        //// 任务完成
        //AddSoundEvent("PlaySound", "name=SFX_Voice_MissionComplete_Person_EN", "sound=SFX_Voice_MissionComplete_Person_EN");
        //// 任务失败
        //AddSoundEvent("PlaySound", "name=SFX_Voice_MissionLoss_Person_EN", "sound=SFX_Voice_MissionLoss_Person_EN");
        //// Nice1
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Nice_EN_01", "sound=SFX_Voice_Nice_EN_01");
        //// Nice2
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Nice_EN_02", "sound=SFX_Voice_Nice_EN_02");
        //// Nice3
        //AddSoundEvent("PlaySound", "name=SFX_Voice_Nice_EN_03", "sound=SFX_Voice_Nice_EN_03");
        //// IDle
        //AddSoundEvent("PlaySound", "name=SFX_Voice_NotAttack_EN_01", "sound=SFX_Voice_NotAttack_EN_01");
        //// IDle
        //AddSoundEvent("PlaySound", "name=SFX_Voice_NotAttack_EN_02", "sound=SFX_Voice_NotAttack_EN_02");
        //// IDle
        //AddSoundEvent("PlaySound", "name=SFX_Voice_NotAttack_EN_03", "sound=SFX_Voice_NotAttack_EN_03");
        //// IDle
        //AddSoundEvent("PlaySound", "name=SFX_Voice_NotAttack_EN_04", "sound=SFX_Voice_NotAttack_EN_04");
        //// IDle
        //AddSoundEvent("PlaySound", "name=SFX_Voice_TimeUp_EN", "sound=SFX_Voice_TimeUp_EN");
       
    }

    public void ResetVolumeScale()
    {
        SoundEvent.volumeScale = (float)SettingManager.Instance.GameVolume / 10.0f;
    }

    void Update()
    {
        SoundEvent.CleanUpChannels();
    }

    public AudioChannel GetAudioChannel(int id)
    {
        return SoundEvent.GetAudioChannel(id);
    }

    public int FireEvent(string eventName, int id = -1, Transform parent = null)
    {
        if (eventsDict.ContainsKey(eventName))
        {
            SoundEvent e = eventsDict[eventName];

            id = e.FireEvent(id, parent);
        }
        else
        {
            Log.Hsz("Warning: Firing sound event that doesn't exist - " + eventName);
        }

        return id;
    }

    public void AddSoundEvent(string eventType, params string[] paramlist)
    {
        SoundEvent e = null;

        switch (eventType)
        {
            case "PlaySound":
                e = new PlaySFXEvent();
                break;
            case "StartSoundLoop":
                e = new StartSFXLoopEvent();
                break;
            case "StopSoundLoop":
                e = new StopSFXLoopEvent();
                break;
            case "StartMusic":
                e = new StartMusicEvent();
                break;
            case "StopMusic":
                e = new StopMusicEvent();
                break;
            case "FadeMusic":
                e = new FadeMusicEvent();
                break;
        }

        if (e != null)
        {
            e.Deserialize(paramlist);

            if (eventsDict.ContainsKey(e.name))
            {
                eventsDict[e.name].clip = library.GetSoundFromLibrary(e.soundName);
                eventsDict[e.name].library = library;
            }
            else
            {
                e.clip = library.GetSoundFromLibrary(e.soundName);
                e.library = library;
                eventsDict.Add(e.name, e);
            }
        }
    }

    // 待机背景音效
    public void StartSceneIdleMusic()
    {
        sceneMusicId = FireEvent("Music_Idle_Start");
    }
    public void StopSceneIdleMusic(Transform parent = null)
    {
        FireEvent("Music_Idle_Stop", sceneMusicId);
    }

    private int mainId = 3;
    // 游戏背景音效
    public void StartSceneGameMusic(int id)
    {
        mainId = id;
        sceneMusicId = FireEvent("Music_Game_Background_" + mainId + "_Start");
    }
    public void StopSceneGameMusic()
    {
        FireEvent("Music_Game_Background_" + mainId + "_Stop", sceneMusicId);
    }

    // 游戏选择界面背景音效
    public void StartSelectMusic()
    {
        sceneMusicId = FireEvent("Music_Select_Role_Start");
    }
    public void StopSelectMusic()
    {
        FireEvent("Music_Select_Role_Stop", sceneMusicId);
    }

    // 准备
    public void StartReadyMusic()
    {
        sceneMusicId = FireEvent("Music_Ready_Start");
    }
    public void StopReadyMusic()
    {
        FireEvent("Music_Ready_Stop", sceneMusicId);
    }

    private int _bossMusicID;
    // Boss背景音效
    public void StartBossMusic(int index)
    {
        _bossMusicID = index;
        bossMusicId = FireEvent("Music_Boss_Step" + _bossMusicID + "_Start");
    }
    public void StopBossMusic()
    {
        FireEvent("Music_Boss_Step" + _bossMusicID + "_Stop", bossMusicId);
    }

    // Boss警告
    public void StartBossWarningMusic()
    {
        bossComingId = FireEvent("Music_Boss_Warning_Start");
    }
    public void StopBossWarningMusic()
    {
        FireEvent("Music_Boss_Warning_Stop", bossComingId);
    }

    private int _engineId;
    // 引擎准备起跑 0 准备 1 正常 2加速
    public void StartEngineMusic(int index)
    {
        if (-1 != engineId)
        {
            StopEngineMusic();
        }
        _engineId = index;
        engineId = FireEvent("Music_Engine_" + _engineId + "_Start");
    }
    public void StopEngineMusic()
    {
        FireEvent("Music_Engine_" + _engineId + "_Stop", engineId);
    }

    // 引擎普通行驶

    // 机枪
    public void StartGunFireMusic()
    {
        gunId = FireEvent("Music_Gun_Fire_Start");
    }
    public void StopGunMusic()
    {
        FireEvent("Music_Gun_Fire_Stop", gunId);
    }

    // 豹子奔跑
    public void StartLeopardRunMusic()
    {
        leopardId = FireEvent("Music_Leopard_Run_Start");
    }
    public void StopLeopardRunMusic()
    {
        FireEvent("Music_Leopard_Run_Stop", leopardId);
    }

    // 结算画面
    public void StartEndMusic()
    {
        sceneMusicId = FireEvent("Music_Panel_End_Start");
    }
    public void StopEndMusic()
    {
        FireEvent("Music_Panel_End_Stop", sceneMusicId);
    }


    // 悬着界面变豹子
    public void StartLeopardSelect()
    {
        if (-1 != selectCarId)
        {
            StopLeopardSelect();
        }
        selectCarId = FireEvent("SFX_Voice_Sound_ChangeLeopard_Idle_Start");
    }
    public void StopLeopardSelect()
    {
        FireEvent("SFX_Voice_Sound_ChangeLeopard_Idle_Stop", selectCarId);
    }


    // 结算计数
    public void StartEndingAccount()
    {
        endingAccountId = FireEvent("Music_Account_Start");
    }
    public void StopEndingAccount()
    {
        FireEvent("Music_Account_Stop", endingAccountId);
    }

    // 投币
    public void PlaySoundInsertCoin()
    {
        FireEvent("SFX_Voice_Sound_InsertCoin");
    }

    // 拍照
    public void PlaySoundPhoto()
    {
        FireEvent("SFX_Voice_Sound_Photo");
    }

    // Boss死亡
    public void PlaySoundBossDie(Transform transform)
    {
        FireEvent("SFX_Voice_Sound_Boss_Die", -1, transform);
    }

    // 切换选择
    public void PlaySoundChange()
    {
        FireEvent("SFX_Voice_Sound_Change");
    }

    // 游戏中变豹子
    public void PlaySoundLeopardGame()
    {
        FireEvent("SFX_Voice_Sound_ChangeToLeopard_Game");
    }

    // 导弹
    public void PlaySoundGuided(Transform transform)
    {
        FireEvent("SFX_Voice_Sound_Guided", -1, transform);
    }

    // 导弹爆炸
    public void PlayerSoundGuidedExplosion(Transform transform)
    {
        FireEvent("SFX_Voice_Sound_Guided_Explosion", -1, transform);
    }

    //卡车
    public void PlaySoundHold()
    {
        FireEvent("SFX_Voice_Sound_Hold");
    }

    // 地雷
    public void PlaySoundLandmine()
    {
        FireEvent("SFX_Voice_Sound_LandMine");
    }

    // 狼嚎
    public void PlaySoundWolf()
    {
        FireEvent("SFX_Voice_Sound_Wolf");
    }

    // 猫头鹰
    public void PlaySoundOwl()
    {
        FireEvent("SFX_Voice_Sound_Owl");
    }

    // 吃道具
    public void PlaySoundProp()
    {
        FireEvent("SFX_Voice_Sound_Prop");
    }

    // 确定
    public void PlaySoundSure()
    {
        FireEvent("SFX_Voice_Sound_Sure");
    }

    // 汽车碰撞
    public void PlayerSoundCollision(Transform transform)
    {
        FireEvent("SFX_Voice_Sound_Collision", -1, transform);
    }

    // 过检查点
    public void PlaySoundCheckPoint()
    {
        FireEvent("SFX_Voice_Sound_AddTime");
    }

    // 跳跃车
    public void PlaySoundSkipSound()
    {
        FireEvent("SFX_Voice_Skip_Sound");
    }

    // 完美躲避
    public void PlaySoundDodgeSound()
    {
        FireEvent("SFX_Voice_Dodge_Sound");
    }

    // 落地
    public void PlaySoundFallGroundSound()
    {
        FireEvent("SFX_Voice_Fall_Ground_Sound");
    }

    // 没用的特效音效
    public void PlaySoundNoUse()
    {
        FireEvent("SFX_Voice_Sound_NoUse");
    }

    public void PlaySound14_4()
    {
        FireEvent("SFX_Voice_Sound_14_4");
    }

    public void PlaySound3()
    {
        FireEvent("SFX_Voice_Sound_3");
    }

    public void PlaySoundPhotoCoutDown()
    {
        FireEvent("SFX_Voice_Sound_Photo_CountDown");
    }

    // 语音 英文
    public void PlayPersonSoundReadyGo()
    {
        FireEvent("SFX_Voice_Sound_Ready_GO");
    }


    /// <summary>
    /// 游戏结束
    /// </summary>
    public void PlayPersonSoundGameOver()
    {
        FireEvent("Voice_C_gameover");
    }

    /// <summary>
    /// 冲冲冲
    /// </summary>
    public void PlayPersonSoundGOGOGO()
    {
        FireEvent("Voice_C_Go");
    }

    /// <summary>
    /// 都让开
    /// </summary>
    public void PlayPersonSoundGetAway()
    {
        FireEvent("Voice_C_Getaway");
    }

    /// <summary>
    /// 赞
    /// </summary>
    public void PlayPersonSoundGood()
    {
        FireEvent("Voice_C_Goodjob");
    }

    /// <summary>
    /// 狂赞
    /// </summary>
    public void PlayPersonSoundGreat()
    {
        FireEvent("Voice_C_Greatjob");
    }

    /// <summary>
    /// 隐身
    /// </summary>
    public void PlayPersonSoundHide()
    {
        FireEvent("Voice_C_INVISIBLE");
    }

    /// <summary>
    /// 准备拍照
    /// </summary>
    public void PlayPersonSoundPhotoReady()
    {
        FireEvent("Voice_C_Photo ready");
    }

    /// <summary>
    /// 前两名免费
    /// </summary>
    public void PlayPersonSoundPhoto1()
    {
        FireEvent("Voice_C_photograph01");
    }

    /// <summary>
    /// 相片正在打印中
    /// </summary>
    public void PlayPersonSoundPhoto2()
    {
        FireEvent("Voice_C_photograph02");
    }

    /// <summary>
    /// 游戏开始了
    /// </summary>
    public void PlayPersonSoundStart()
    {
        FireEvent("Voice_C_Start");
    }

    /// <summary>
    /// 哇
    /// </summary>
    public void PlayPersonSoundWow()
    {
        FireEvent("Voice_C_Wowgreat");
    }

    /// <summary>
    /// 耶
    /// </summary>
    public void PlayPersonSoundYeah()
    {
        FireEvent("Voice_C_yeah");
    }

    /// <summary>
    /// 拍照 321
    /// </summary>
    /// <param name="num"></param>
    public void PlayerPersonSound321(int num)
    {
        FireEvent("Voice_C_" + num);
    }

}
