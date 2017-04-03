/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingLogic.cs
 * 
 * 简    介:    
 * 
 * 创建标识：   Mike 2016/9/4 10:47:18
 * 
 * 修改描述：
 * 
 */


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Need.Mx;

public class SettingLogic : BaseUI
{
    protected SettingView view;
    private SettingConfig.Page curPage; //当前选中页
    private SettingConfig.HomePageItem curHomeItem;//
    private SettingConfig.SettingPageItem curSettingItem;
    private SettingConfig.AccountPageItem curAccountItem;
    private SettingConfig.DataResetItem curDataResetItem;
    private SettingConfig.ComdOfButA curButA;

    //  Home_Page主页
    private int toggle_homeNumber;
    private int Index_homePageSwicth;
    // Setting_Page设置页
    private int toggle_settingdNumber;
    private int Index_settingPageSwitch;

    //Account_Page 查账页
    private int toggle_accountNumber;
    private int Index_accountPageSwitch;
    //BusinessRecord_Page 营业记录页

    //TotalRecord_Page总记录页

    //DataReset_Page数据清零页
    private int toggle_dataResetNumber;
    private int Index_dataResetPageSwitch;

    private int comdSwitch;         //指示按钮A和按钮B功能切换
    private int set_coin;           //设置每次消耗游戏币数
    private int set_volume;         //设置音量
    private int set_laguage;        //设置语言
    private int set_ticket;         //设置多少分出1票
    private int clear_coin;


    public override EnumUIType GetUIType()
    {
        return EnumUIType.PanelSetting;
    }

    protected override void OnAwake()
    {
        MessageCenter.Instance.AddListener(MessageType.EVNET_SETTING_CONFIRM, ComdButtonA);
        MessageCenter.Instance.AddListener(MessageType.EVNET_SETTING_SELECT, ComdButtonB);
        base.OnAwake();
    }

    protected override void OnRelease()
    {
        MessageCenter.Instance.RemoveListener(MessageType.EVNET_SETTING_CONFIRM, ComdButtonA);
        MessageCenter.Instance.RemoveListener(MessageType.EVNET_SETTING_SELECT, ComdButtonB);
        base.OnRelease();
    }

    protected override void OnStart()
    {
        view = new SettingView();
        view.Init(transform);
        toggle_homeNumber = view.homePage.homeToggleList.Count;
        toggle_settingdNumber = view.settingPage.settingToggleList.Count; ;
        toggle_accountNumber = view.accountPage.accountToggleList.Count;
        toggle_dataResetNumber = view.dataRestPage.datarestToggleList.Count;

        SetLanguage();

        base.OnStart();
    }

    //设置后台语言
    private void SetLanguage()
    {
        int language = SettingManager.Instance.GameLanguage;
        //Home_Page
        {
            for (int i = 0; i < view.homePage.homeTextList.Count; i++)
            {
                view.homePage.homeTextList[i].text = SettingConfig.homeName[language, i];
            }
        }
        //Setting_Page
        {
            for (int i = 0; i < view.settingPage.settingTextList.Count; i++)
            {
                view.settingPage.settingTextList[i].text = SettingConfig.setName[language, i];
            }
            view.settingPage.Text_title.text = SettingConfig.setName[language, view.settingPage.settingTextList.Count];
            if (language == 0)
            {
                view.settingPage.image_rateCoin[0].overrideSprite = view.imageFree.image_free_english.overrideSprite;
                for (int i = 0; i < view.settingPage.Text_clear.Length; i++)
                {
                    view.settingPage.Text_clear[i].text = SettingConfig.clearcoin[0, i];
                }
                for (int i = 0; i < view.settingPage.Text_ticket.Length;i++ )
                {
                    view.settingPage.Text_ticket[i].text = SettingConfig.gameDifficulty[0, i];
                }
            }
            else
            {
                view.settingPage.image_rateCoin[0].overrideSprite = view.imageFree.image_free_chinese.overrideSprite;
                for (int i = 0; i < view.settingPage.Text_clear.Length; i++)
                {
                    view.settingPage.Text_clear[i].text = SettingConfig.clearcoin[1, i];
                }
                for (int i = 0; i < view.settingPage.Text_ticket.Length; i++)
                {
                    view.settingPage.Text_ticket[i].text = SettingConfig.gameDifficulty[1, i];
                }
            }

        }
        //Account_Page
        {
            for (int i = 0; i < view.accountPage.accountTextList.Count; i++)
            {
                view.accountPage.accountTextList[i].text = SettingConfig.accountName[language, i];
            }
            view.accountPage.Text_title.text = SettingConfig.accountName[language, view.accountPage.accountTextList.Count];
        }
        //Business_Page
        {
            for (int i = 0; i < view.businnessPage.titleList.Count; i++)
            {
                view.businnessPage.titleList[i].text = SettingConfig.businessRecordName[language, i];
            }
        }
        //DataRest_Page
        {
            for (int i = 0; i < view.dataRestPage.datarestTextList.Count; i++)
            {
                view.dataRestPage.datarestTextList[i].text = SettingConfig.dataResetName[language, i];
            }
            view.dataRestPage.Text_title.text = SettingConfig.dataResetName[language, view.dataRestPage.datarestTextList.Count];
        }
        //Total_Page
        {
            for (int i = 0; i < view.totalPage.titleList.Count; i++)
            {
                view.totalPage.titleList[i].text = SettingConfig.totalRecordName[language, i];
            }
        }
    }

    //查询当前页
    private void FindPage()
    {
        if (view.Home_Page.activeSelf)
        {
            curPage = SettingConfig.Page.HomePage;
        }
        if (view.Setting_Page.activeSelf)
        {
            curPage = SettingConfig.Page.SettingPage;
        }
        if (view.Account_Page.activeSelf)
        {
            curPage = SettingConfig.Page.AccountPage;
        }
        if (view.BusinessRecords_Page.activeSelf)
        {
            curPage = SettingConfig.Page.BusinessRecord_Page;
        }
        if (view.TotalRecord_Page.activeSelf)
        {
            curPage = SettingConfig.Page.TotalRecord_Page;
        }
        if (view.DataReset_Page.activeSelf)
        {
            curPage = SettingConfig.Page.DataReset_Page;
        }
    }

    #region---------------------------按钮A_Start---------------------------------
    private void ComdButtonA(Message message)
    {
        FindPage();
        //主页操作
        if (curPage == SettingConfig.Page.HomePage)
        {
            if (curHomeItem == SettingConfig.HomePageItem.Set)  //进入SettingPage
            {
                view.Home_Page.SetActive(false);
                view.Setting_Page.SetActive(true);
                SetSetPageInfo();
                comdSwitch = 0;
            }
            if (curHomeItem == SettingConfig.HomePageItem.Account)//进入AccountPage
            {
                view.Home_Page.SetActive(false);
                view.Account_Page.SetActive(true);
            }
            if (curHomeItem == SettingConfig.HomePageItem.Exit)//退出后台
            {
                SettingManager.Instance.Save();
                SettingManager.Instance.Init();
                GameMode.Instance.ChangeMode(RunMode.Waiting);
            }
        }
        //设置页操作
        if (curPage == SettingConfig.Page.SettingPage)
        {
            ++comdSwitch;
            comdSwitch %= 2;
            if (curSettingItem == SettingConfig.SettingPageItem.Money       ||
                curSettingItem == SettingConfig.SettingPageItem.Volume      ||
                curSettingItem == SettingConfig.SettingPageItem.Lanuage     ||
                curSettingItem == SettingConfig.SettingPageItem.OutTicket   ||
                curSettingItem == SettingConfig.SettingPageItem.ClearCoin)
            {
                if (comdSwitch == 1)
                {
                    curButA = SettingConfig.ComdOfButA.Sure;
                    view.settingPage.settingImageList[(int)curSettingItem].color = new Color(1, 0, 0);
                }
                else
                {
                    curButA = SettingConfig.ComdOfButA.Enter;
                    view.settingPage.settingImageList[(int)curSettingItem].color = new Color(1, 1, 1);
                }
            }

            //if (curSettingItem == SettingConfig.SettingPageItem.ClearCoin)
            //{
            //    if (comdSwitch == 1)
            //    {
            //        curButA = SettingConfig.ComdOfButA.Sure;
            //        view.settingPage.Text_clear[comdSwitch].color = new Color(1, 0, 0);
            //    }
            //    else
            //    {
            //        curButA = SettingConfig.ComdOfButA.Enter;
            //        view.settingPage.Text_clear[comdSwitch].color = new Color(1, 1, 1);
            //    }
            //}

            if (curButA == SettingConfig.ComdOfButA.Enter)
            {
                switch (curSettingItem)
                {
                    case SettingConfig.SettingPageItem.Money:
                        SettingManager.Instance.GameRate = set_coin;
                        break;
                    case SettingConfig.SettingPageItem.Volume:
                        SettingManager.Instance.GameVolume = set_volume;
                        break;
                    case SettingConfig.SettingPageItem.Lanuage:
                        SettingManager.Instance.GameLanguage = set_laguage;
                        SetLanguage();
                        break;
                    case SettingConfig.SettingPageItem.OutTicket:
                        SettingManager.Instance.TicketModel = set_ticket;
                        break;
                    case SettingConfig.SettingPageItem.ClearCoin:
                        SettingManager.Instance.ClearCoin();
                        break;
                }
            }
            if (curSettingItem == SettingConfig.SettingPageItem.Exit)
            {
                view.Setting_Page.SetActive(false);
                view.Home_Page.SetActive(true);
            }
            //if (curSettingItem == SettingConfig.SettingPageItem.Calibration)
            //{
            //    view.Setting_Page.SetActive(false);
            //    view.Calibration_Page.SetActive(true);
            //}
        }
        //查账页操作
        if (curPage == SettingConfig.Page.AccountPage)
        {
            if (curAccountItem == SettingConfig.AccountPageItem.BusinessRecords)
            {
                view.Account_Page.SetActive(false);
                view.BusinessRecords_Page.SetActive(true);
                SetBusinessPageInfo();
            }
            if (curAccountItem == SettingConfig.AccountPageItem.TotalRecords)
            {
                view.Account_Page.SetActive(false);
                view.TotalRecord_Page.SetActive(true);
                SetTotalRecordInfo();
            }
            if (curAccountItem == SettingConfig.AccountPageItem.DataReset)
            {
                view.Account_Page.SetActive(false);
                view.DataReset_Page.SetActive(true);
            }
            if (curAccountItem == SettingConfig.AccountPageItem.Exit)
            {
                view.Account_Page.SetActive(false);
                view.Home_Page.SetActive(true);
            }
        }
        ////射击校验页操作
        //if(curPage == SettingConfig.Page.CalibrationPage)
        //{
        //    if(curCalibrationItem == SettingConfig.CalibrationPageItem.Player1)
        //    {
        //        view.Calibration_Page.SetActive(false);
        //        view.ShootingCalibration_Page.SetActive(true);
        //        IOManager.Instance.ShootCalibration(true);
        //    }
        //    if (curCalibrationItem == SettingConfig.CalibrationPageItem.Player2)
        //    {
        //        view.Calibration_Page.SetActive(false);
        //        view.ShootingCalibration_Page.SetActive(true);
        //        IOManager.Instance.ShootCalibration(true);
        //    }
        //    if (curCalibrationItem == SettingConfig.CalibrationPageItem.Player3)
        //    {
        //        view.Calibration_Page.SetActive(false);
        //        view.ShootingCalibration_Page.SetActive(true);
        //        IOManager.Instance.ShootCalibration(true);
        //    }
        //    if(curCalibrationItem == SettingConfig.CalibrationPageItem.Exit)
        //    {
        //        view.Calibration_Page.SetActive(false);
        //        view.Setting_Page.SetActive(true);
        //        comdSwitch = 0;
        //    }
        //    Index_shine = 0;
        //    Image img = view.shootingcalibrationpage.shootinglist[0] as Image;
        //    img.color = new Color(1, 0, 0);
        //}
        //查账记录页
        if (curPage == SettingConfig.Page.BusinessRecord_Page)
        {
            view.BusinessRecords_Page.SetActive(false);
            view.Account_Page.SetActive(true);
        }
        //总记录页
        if (curPage == SettingConfig.Page.TotalRecord_Page)
        {
            view.TotalRecord_Page.SetActive(false);
            view.Account_Page.SetActive(true);
        }
        //数据清零页
        if (curPage == SettingConfig.Page.DataReset_Page)
        {
            if (curDataResetItem == SettingConfig.DataResetItem.Yes)
            {
                SettingManager.Instance.ClearMonthInfo();
                SettingManager.Instance.ClearTotalRecord();
                view.DataReset_Page.SetActive(false);
                view.Account_Page.SetActive(true);
            }
            if (curDataResetItem == SettingConfig.DataResetItem.No)
            {
                view.DataReset_Page.SetActive(false);
                view.Account_Page.SetActive(true);
            }
        }
        ////射击校验页
        //if(curPage == SettingConfig.Page.ShootingCalibration_Page)
        //{
        //    calibrationVector2[Index_shine] = IOManager.Instance.GetRockerBar((int)curCalibrationItem);
        //    ++Index_shine;
        //    for (int i = 0; i < view.shootingcalibrationpage.shootinglist.Count;i++ )
        //    {
        //        Image img = view.shootingcalibrationpage.shootinglist[i] as Image;
        //        if(Index_shine == i)
        //        {                    
        //            img.color = new Color(1, 0, 0);
        //        }
        //        else
        //        {
        //            img.color = new Color(1, 1, 1);
        //        }
        //    }

        //    if (Index_shine >= view.shootingcalibrationpage.shootinglist.Count)
        //    {
        //        IOManager.Instance.ShootCalibration(false);
        //        view.ShootingCalibration_Page.SetActive(false);
        //        view.Calibration_Page.SetActive(true);
        //        ShootingCalibrationOp((int)curCalibrationItem);
        //    }

        //}
    }
    ////射击校验操作
    //private void ShootingCalibrationOp(int playerid)
    //{
    //    string[] strconfiges = new string[30];

    //    for (int i = 0, j = 0; j < 15;j++ )
    //    {
    //        strconfiges[i++] = "GAME_CONFIG_POINT_" + j + "X_PLAYER_" + playerid.ToString();
    //        strconfiges[i++] = "GAME_CONFIG_POINT_" + j + "Y_PLAYER_" + playerid.ToString();
    //    }

    //    for (int i = 0, j = 0; j < 15; j++)
    //    {
    //        PlayerPrefs.SetFloat(strconfiges[i++], calibrationVector2[j].x);
    //        PlayerPrefs.SetFloat(strconfiges[i++], calibrationVector2[j].y);
    //    }

    //    PlayerPrefs.Save();

    //    SettingManager.Instance.SetShootingCalibrationPointsPos((int)curCalibrationItem, calibrationVector2);
    //}
    private void SetSetPageInfo()
    {
        set_coin = SettingManager.Instance.GameRate;
        set_volume = SettingManager.Instance.GameVolume;
        set_laguage = SettingManager.Instance.GameLanguage;
        set_ticket = SettingManager.Instance.TicketModel;
        for (int i = 0; i < view.settingPage.image_rateCoin.Length; i++)
        {
            if (i == SettingManager.Instance.GameRate)
            {
                view.settingPage.image_rateCoin[i].color = new Color(1, 0, 0);
            }
            else
            {
                view.settingPage.image_rateCoin[i].color = new Color(1, 1, 1);

            }
        }
        for (int i = 0; i < view.settingPage.image_language.Length; i++)
        {
            if (SettingManager.Instance.GameLanguage == i)
            {
                view.settingPage.image_language[i].color = new Color(1, 0, 0);
            }
            else
            {
                view.settingPage.image_language[i].color = new Color(1, 1, 1);
            }
        }

        for (int i = 0; i < view.settingPage.Text_ticket.Length; i++ )
        {
            if (set_ticket == i)
            {
                view.settingPage.Text_ticket[i].color = new Color(1, 0, 0);
            }
            else
            {
                view.settingPage.Text_ticket[i].color = new Color(1, 1, 1);
            }
        }

        //for (int i = 0; i < view.settingPage.Text_clear.Length;i++ )
        //{
        //    if (clear_coin == i)
        //    {
        //        view.settingPage.Text_clear[i].color = new Color(1, 0, 0);
        //    }
        //    else
        //    {
        //        view.settingPage.Text_clear[i].color = new Color(1, 1, 1);
        //    }
        //}

        view.settingPage.slider_volume.value = (float)set_volume / SettingConfig.volume.Length;
        view.settingPage.Text_volume.text = SettingManager.Instance.GameVolume.ToString();
    }

    // 设置查账信息
    private void SetBusinessPageInfo()
    {
        int language = SettingManager.Instance.GameLanguage;
        for (int i = 0; i < SettingManager.Instance.GetMonthData().Count; i++)
        {
            view.businnessPage.monthList[i].text = SettingManager.Instance.GetMonthData(i)[0].ToString();
            view.businnessPage.coinsmonthList[i].text = SettingManager.Instance.GetMonthData(i)[1].ToString();
            view.businnessPage.gametimemonthList[i].text = HelperTool.DoTimeFormat(language, (int)SettingManager.Instance.GetMonthData(i)[2]);
            view.businnessPage.uptimemonthList[i].text = HelperTool.DoTimeFormat(language, (int)SettingManager.Instance.GetMonthData(i)[3]);
        }

        for (int i = 0; i < view.businnessPage.titleList.Count; i++)
        {
            view.businnessPage.titleList[i].text = SettingConfig.businessRecordName[language, i];
        }
    }

    // 设置总记录信息
    private void SetTotalRecordInfo()
    {
        int language = SettingManager.Instance.GameLanguage;
        for (int i = 0; i < SettingManager.Instance.TotalRecord().Length; i++)
        {
            if (i == 1 || i == 2)
            {
                view.totalPage.valueList[i].text = HelperTool.DoTimeFormat(language, (int)SettingManager.Instance.TotalRecord()[i]);
            }
            else
            {
                view.totalPage.valueList[i].text = SettingManager.Instance.TotalRecord()[i].ToString();
            }

        }

        for (int i = 0; i < view.totalPage.titleList.Count; i++)
        {
            view.totalPage.titleList[i].text = SettingConfig.totalRecordName[language, i].ToString();
        }
    }

    #endregion---------------------------按钮A_End---------------------------------


    #region---------------------按钮B_Start---------------------------------------------
    private void ComdButtonB(Message message)
    {
        FindPage();
        if (curPage == SettingConfig.Page.HomePage)
        {
            SwitchHomeItem();
        }
        if (curPage == SettingConfig.Page.SettingPage)
        {
            SwitchSettingItem();
        }
        if (curPage == SettingConfig.Page.AccountPage)
        {
            SwitchAccountItem();
        }
        if (curPage == SettingConfig.Page.DataReset_Page)
        {
            SwitchDataRestItem();
        }
    }

    private void SwitchHomeItem()
    {
        ++Index_homePageSwicth;
        Index_homePageSwicth %= toggle_homeNumber;
        int temp = 0;
        for (int i = 0; i < view.homePage.homeToggleList.Count; i++)
        {
            if (Index_homePageSwicth == temp)
            {
                view.homePage.homeToggleList[i].isOn = true;
                view.homePage.homeTextList[i].color = new Color(1, 0, 0);
                curHomeItem = (SettingConfig.HomePageItem)Index_homePageSwicth;
            }
            else
            {
                view.homePage.homeToggleList[i].isOn = false;
                view.homePage.homeTextList[i].color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }
    private void SwitchSettingItem()
    {
        if (curButA == SettingConfig.ComdOfButA.Enter)
        {
            ++Index_settingPageSwitch;
            Index_settingPageSwitch %= toggle_settingdNumber;
            int temp = 0;

            for (int i = 0; i < view.settingPage.settingToggleList.Count; i++)
            {
                if (Index_settingPageSwitch == temp)
                {
                    view.settingPage.settingToggleList[i].isOn = true;
                    view.settingPage.settingTextList[i].color = new Color(1, 0, 0);
                    curSettingItem = (SettingConfig.SettingPageItem)Index_settingPageSwitch;
                }
                else
                {
                    view.settingPage.settingToggleList[i].isOn = false;
                    view.settingPage.settingTextList[i].color = new Color(1, 1, 1);
                }
                ++temp;
            }
        }

        if (curButA == SettingConfig.ComdOfButA.Sure)//按钮A:按下可以锁定当前选项卡，以便按钮B进行数据调整
        {
            //调整比率
            if (curSettingItem == SettingConfig.SettingPageItem.Money)
            {
                ++set_coin;
                set_coin %= SettingConfig.money.Length;
                for (int i = 0; i < view.settingPage.image_rateCoin.Length; i++)
                {
                    if (i == set_coin)
                    {
                        view.settingPage.image_rateCoin[i].color = new Color(1, 0, 0);
                    }
                    else
                    {
                        view.settingPage.image_rateCoin[i].color = new Color(1, 1, 1);
                    }
                }
            }
            //调整声音
            if (curSettingItem == SettingConfig.SettingPageItem.Volume)
            {
                ++set_volume;
                set_volume %= (SettingConfig.volume.Length + 1);
                view.settingPage.Text_volume.text = set_volume.ToString();
                view.settingPage.slider_volume.value = (float)set_volume / SettingConfig.volume.Length;
            }
            //调整语言
            if (curSettingItem == SettingConfig.SettingPageItem.Lanuage)
            {
                ++set_laguage;
                set_laguage %= SettingConfig.language.Length;
                int temp = 0;

                for (int i = 0; i < view.settingPage.image_language.Length; i++)
                {
                    if (set_laguage == temp)
                    {
                        view.settingPage.image_language[i].color = new Color(1, 0, 0);
                    }
                    else
                    {
                        view.settingPage.image_language[i].color = new Color(1, 1, 1);
                    }
                    ++temp;
                }

            }

            if (curSettingItem == SettingConfig.SettingPageItem.OutTicket)
            {
                ++set_ticket;
                set_ticket %= view.settingPage.Text_ticket.Length;
                for (int i = 0; i < view.settingPage.Text_ticket.Length;i++ )
                {
                    if (set_ticket == i)
                    {
                        view.settingPage.Text_ticket[i].color = new Color(1, 0, 0);                       
                    }
                    else
                    {
                        view.settingPage.Text_ticket[i].color = new Color(1, 1, 1);
                    }
                }
            }
            // 清除投币
            if (curSettingItem == SettingConfig.SettingPageItem.ClearCoin)
            {
                ++clear_coin;
                clear_coin %= view.settingPage.Text_clear.Length;
                int temp = 0;

                for (int i = 0; i < view.settingPage.Text_clear.Length; i++)
                {
                    if (clear_coin == temp)
                    {
                        view.settingPage.Text_clear[i].color = new Color(1, 0, 0);
                    }
                    else
                    {
                        view.settingPage.Text_clear[i].color = new Color(1, 1, 1);
                    }
                    ++temp;
                }
            }
        }
    }

    private void SwitchAccountItem()
    {
        ++Index_accountPageSwitch;
        Index_accountPageSwitch %= toggle_accountNumber;
        int temp = 0;
        foreach (Toggle tg in view.accountPage.accountToggleList)
        {
            if (Index_accountPageSwitch == temp)
            {
                tg.isOn = true;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 0, 0);
                curAccountItem = (SettingConfig.AccountPageItem)Index_accountPageSwitch;
            }
            else
            {
                tg.isOn = false;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }
    private void SwitchDataRestItem()
    {
        ++Index_dataResetPageSwitch;
        Index_dataResetPageSwitch %= toggle_dataResetNumber;
        int temp = 0;
        foreach (Toggle tg in view.dataRestPage.datarestToggleList)
        {
            if (Index_dataResetPageSwitch == temp)
            {
                tg.isOn = true;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 0, 0);
                curDataResetItem = (SettingConfig.DataResetItem)Index_dataResetPageSwitch;
            }
            else
            {
                tg.isOn = false;
                tg.transform.Find("Label").GetComponent<Text>().color = new Color(1, 1, 1);
            }
            ++temp;
        }
    }
    #endregion---------------------按钮B_End---------------------------------------------
}
