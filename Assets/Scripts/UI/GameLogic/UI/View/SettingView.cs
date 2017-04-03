/*
 * Copyright (c) 
 * 
 * 文件名称：   SettingView.cs
 * 
 * 简    介:    SettingView Setting视图
 * 
 * 创建标识：  Mike 2015/11/21 8:32:44
 * 
 * 修改描述： Mike 2016.9.3    数据存储方式由PlayerPrefs转为JSON
 * 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SettingView
{

    // Canvas--页面切换
    [HideInInspector]
    public GameObject Home_Page;
    [HideInInspector]
    public GameObject Setting_Page;
    [HideInInspector]
    public GameObject Account_Page;
    [HideInInspector]
    public GameObject BusinessRecords_Page;
    [HideInInspector]
    public GameObject TotalRecord_Page;
    [HideInInspector]
    public GameObject DataReset_Page;
    [HideInInspector]
    public GameObject Image_Free;

    //public HomePage homepage;
    //public SettingPage settingpage;
    //public CalibrationPage calibrationPage;
    //public AccountPage accountPage;
    //public BusinnessPage businnessPage;
    //public TotalPage totalPage;
    //public DataRestPage dataRestPage;
    //public ShootingCalibrationPage shootingCalibrationPage;
    //public ImageFree imageFree;
    //Home_Page
    public struct HomePage
    {
        public Toggle Toggle_set;
        public Toggle Toggle_account;
        public Toggle Toggle_exit;
        public List<Toggle> homeToggleList;
        public List<Text> homeTextList;
        public List<Image> homeImageList;
    }
    public HomePage homePage;
    //Setting_Page
    public struct SettingPage
    {
        public Toggle Toggle_rate;
        public Toggle Toggle_volume;
        public Toggle Toggle_language;
        public Toggle Toggle_ticket;
        public Toggle Toggle_exit;
        public Toggle Toggle_clear;
        public Image[] image_rateCoin;
        public Image[] image_language;
        public Image[] image_water;
        public Slider slider_volume;
        public Text Text_volume;
        public Text[] Text_ticket;
        public Text Text_title;
        public Text[] Text_clear;
        public List<Toggle> settingToggleList;
        public List<Text> settingTextList;
        public List<Image> settingImageList;
    }
    public SettingPage settingPage;

    //Account_Page
    public struct AccountPage
    {
        public Toggle Toggle_business;
        public Toggle Toggle_total;
        public Toggle Toggle_DataReset;
        public Toggle Toggle_exit;
        public Text Text_title;
        public List<Toggle> accountToggleList;
        public List<Text> accountTextList;
        public List<Image> accountImageList;
    }
    public AccountPage accountPage;

    //BusinessRecord_Page
    public struct BusinnessPage
    {
        public Text Text_title;
        public Text Text_tip;
        public Text Text_coinsTitle;
        public Text Text_gameTimeTitle;
        public Text Text_upTimeTitle;
        public Text Text_this_month_tip;
        public Text Text_one_month_ago_tip;
        public Text Text_two_month_ago_tip;

        public Text coins_month_one;
        public Text coins_month_two;
        public Text coins_month_three;
        public Text gameTime_month_one;
        public Text gameTime_month_two;
        public Text gameTime_month_three;
        public Text upTime_month_one;
        public Text upTime_month_two;
        public Text upTime_month_three;


        public Text month_one;
        public Text month_two;
        public Text month_three;

        public List<Text> titleList;
        public List<Text> monthList;
        public List<Text> coinsmonthList;
        public List<Text> gametimemonthList;
        public List<Text> uptimemonthList;
    }
    public BusinnessPage businnessPage;

    //TotalRecord_Page
    public struct TotalPage
    {
        public Text Text_title;
        public Text Text_gameTimesTips;
        public Text Text_totalGameTImesTips;
        public Text Text_upTime;
        public Text Text_totalCoin;

        public Text gameTimesValue;
        public Text totalGameTimesValue;
        public Text upTimeValue;
        public Text totalCoinValue;

        public List<Text> titleList;
        public List<Text> valueList;

    }
    public TotalPage totalPage;

    //DataRest_Page
    public struct DataRestPage
    {
        public Toggle Toggle_Yes;
        public Toggle Toggle_No;
        public Text Text_title;
        public List<Toggle> datarestToggleList;
        public List<Text> datarestTextList;
        public List<Image> datarestImageList;
    }
    public DataRestPage dataRestPage;

    public struct ImageFree
    {
        public Image image_free_chinese;
        public Image image_free_english;
    }
    public ImageFree imageFree;

    // Use this for initialization
    public void Init(Transform transform)
    {
        //切换页
        Home_Page = transform.FindChild("Home_Page").gameObject;
        Setting_Page = transform.FindChild("Setting_Page").gameObject;
        Account_Page = transform.FindChild("Account_Page").gameObject;
        BusinessRecords_Page = transform.FindChild("BusinessRecord_Page").gameObject;
        TotalRecord_Page = transform.FindChild("TotalRecord_Page").gameObject;
        DataReset_Page = transform.FindChild("DataReset_Page").gameObject;
        Image_Free = transform.FindChild("Image_Free").gameObject;
        //Home_Page
        {
            //homepage = new HomePage();
            homePage = new HomePage();
            homePage.homeToggleList = new List<Toggle>();
            homePage.homeTextList = new List<Text>();
            homePage.homeImageList = new List<Image>();
            homePage.Toggle_set = Home_Page.transform.Find("Set").GetComponent<Toggle>();
            homePage.Toggle_account = Home_Page.transform.Find("Account").GetComponent<Toggle>();
            homePage.Toggle_exit = Home_Page.transform.Find("Exit").GetComponent<Toggle>();

            homePage.homeToggleList.Add(homePage.Toggle_set);
            homePage.homeToggleList.Add(homePage.Toggle_account);
            homePage.homeToggleList.Add(homePage.Toggle_exit);
            for (int i = 0; i < homePage.homeToggleList.Count; i++)
            {
                homePage.homeTextList.Add(homePage.homeToggleList[i].transform.Find("Label").GetComponent<Text>());
                homePage.homeImageList.Add(homePage.homeToggleList[i].transform.Find("Background/Checkmark").GetComponent<Image>());
            }
        }

        //Setting_page
        {
            //settingpage = new SettingPage();
            settingPage = new SettingPage();
            settingPage.settingToggleList = new List<Toggle>();
            settingPage.settingTextList = new List<Text>();
            settingPage.settingImageList = new List<Image>();
            settingPage.Toggle_rate = Setting_Page.transform.Find("GameMoneyRate/Toggle").GetComponent<Toggle>();
            settingPage.Toggle_volume = Setting_Page.transform.Find("GameVolume/Toggle").GetComponent<Toggle>();
            settingPage.Toggle_language = Setting_Page.transform.Find("GameLanuage/Toggle").GetComponent<Toggle>();
            settingPage.Toggle_ticket = Setting_Page.transform.Find("GameOutTicket/Toggle").GetComponent<Toggle>();
            settingPage.Toggle_exit = Setting_Page.transform.Find("GameExit/Toggle").GetComponent<Toggle>();
            settingPage.Toggle_clear = Setting_Page.transform.Find("ClearCoin/Toggle").GetComponent<Toggle>();

            GameObject gm = Setting_Page.transform.Find("GameMoneyRate/RateCoin").gameObject;
            settingPage.image_rateCoin = new Image[7];
            settingPage.image_language = new Image[2];
            settingPage.image_water = new Image[2];
            for (int i = 0; i < settingPage.image_rateCoin.Length; i++)
            {
                settingPage.image_rateCoin[i] = gm.transform.GetChild(i).GetComponent<Image>();
            }
            gm = Setting_Page.transform.Find("GameLanuage/Language").gameObject;
            for (int i = 0; i < settingPage.image_language.Length; i++)
            {
                settingPage.image_language[i] = gm.transform.GetChild(i).GetComponent<Image>();
            }
          
            settingPage.slider_volume = Setting_Page.transform.Find("GameVolume/Volume_Slider").GetComponent<Slider>();
            settingPage.Text_volume = Setting_Page.transform.Find("GameVolume/TextValue").GetComponent<Text>();
            settingPage.Text_ticket = new Text[3];
            for (int i = 0; i < settingPage.Text_ticket.Length; i++ )
            {
                settingPage.Text_ticket[i] = Setting_Page.transform.Find("GameOutTicket/Modle" + i.ToString()).GetComponent<Text>();
            }
            settingPage.Text_title = Setting_Page.transform.Find("Title").GetComponent<Text>();

            settingPage.settingToggleList.Add(settingPage.Toggle_rate);
            settingPage.settingToggleList.Add(settingPage.Toggle_volume);
            settingPage.settingToggleList.Add(settingPage.Toggle_language);
            settingPage.settingToggleList.Add(settingPage.Toggle_ticket);
            settingPage.settingToggleList.Add(settingPage.Toggle_exit);
            settingPage.settingToggleList.Add(settingPage.Toggle_clear);

            settingPage.Text_clear = new Text[2];
            for (int i = 0; i < settingPage.Text_clear.Length;i++ )
            {
                settingPage.Text_clear[i] = Setting_Page.transform.Find("ClearCoin/Toggle/Value" + i.ToString()).GetComponent<Text>();
            }

            for (int i = 0; i < settingPage.settingToggleList.Count; i++)
            {
                settingPage.settingTextList.Add(settingPage.settingToggleList[i].transform.Find("Label").GetComponent<Text>());
                settingPage.settingImageList.Add(settingPage.settingToggleList[i].transform.Find("Background/Checkmark").GetComponent<Image>());
            }
        }
        //Calibration_Page

        //Account_Page
        {
            //accountPage = new AccountPage();
            accountPage = new AccountPage();
            accountPage.accountToggleList = new List<Toggle>();
            accountPage.accountTextList = new List<Text>();
            accountPage.accountImageList = new List<Image>();
            accountPage.Toggle_business = Account_Page.transform.Find("Business_Records").GetComponent<Toggle>();
            accountPage.Toggle_total = Account_Page.transform.Find("Total_Records").GetComponent<Toggle>();
            accountPage.Toggle_DataReset = Account_Page.transform.Find("Data_Reset").GetComponent<Toggle>();
            accountPage.Toggle_exit = Account_Page.transform.Find("Exit").GetComponent<Toggle>();
            accountPage.Text_title = Account_Page.transform.Find("Title").GetComponent<Text>();

            accountPage.accountToggleList.Add(accountPage.Toggle_business);
            accountPage.accountToggleList.Add(accountPage.Toggle_total);
            accountPage.accountToggleList.Add(accountPage.Toggle_DataReset);
            accountPage.accountToggleList.Add(accountPage.Toggle_exit);
            for (int i = 0; i < accountPage.accountToggleList.Count; i++)
            {
                accountPage.accountTextList.Add(accountPage.accountToggleList[i].transform.Find("Label").GetComponent<Text>());
                accountPage.accountImageList.Add(accountPage.accountToggleList[i].transform.Find("Background/Checkmark").GetComponent<Image>());
            }
        }

        //BusinessRecord_Page
        {
            //businnessPage = new BusinnessPage();
            businnessPage = new BusinnessPage();
            businnessPage.titleList = new List<Text>();
            businnessPage.monthList = new List<Text>();
            businnessPage.coinsmonthList = new List<Text>();
            businnessPage.gametimemonthList = new List<Text>();
            businnessPage.uptimemonthList = new List<Text>();
            businnessPage.Text_title = BusinessRecords_Page.transform.Find("BusinessTips/Title").GetComponent<Text>();
            businnessPage.Text_tip = BusinessRecords_Page.transform.Find("BusinessTips/Tip").GetComponent<Text>();
            businnessPage.Text_coinsTitle = BusinessRecords_Page.transform.Find("BusinessTips/Coins_title").GetComponent<Text>();
            businnessPage.Text_gameTimeTitle = BusinessRecords_Page.transform.Find("BusinessTips/GameTime_title").GetComponent<Text>();
            businnessPage.Text_upTimeTitle = BusinessRecords_Page.transform.Find("BusinessTips/Uptime_title").GetComponent<Text>();
            businnessPage.Text_this_month_tip = BusinessRecords_Page.transform.Find("BusinessTips/ThisMonth_Tip").GetComponent<Text>();
            businnessPage.Text_one_month_ago_tip = BusinessRecords_Page.transform.Find("BusinessTips/OneMonthAgo_Tip").GetComponent<Text>();
            businnessPage.Text_two_month_ago_tip = BusinessRecords_Page.transform.Find("BusinessTips/TwoMonthAgo_Tip").GetComponent<Text>();

            businnessPage.coins_month_one = BusinessRecords_Page.transform.Find("BusinessInfo/Coins_Month_One").GetComponent<Text>();
            businnessPage.coins_month_two = BusinessRecords_Page.transform.Find("BusinessInfo/Coins_Month_Two").GetComponent<Text>();
            businnessPage.coins_month_three = BusinessRecords_Page.transform.Find("BusinessInfo/Coins_Month_Three").GetComponent<Text>();

            businnessPage.gameTime_month_one = BusinessRecords_Page.transform.Find("BusinessInfo/GameTime_Month_One").GetComponent<Text>();
            businnessPage.gameTime_month_two = BusinessRecords_Page.transform.Find("BusinessInfo/GameTime_Month_Two").GetComponent<Text>();
            businnessPage.gameTime_month_three = BusinessRecords_Page.transform.Find("BusinessInfo/GameTime_Month_Three").GetComponent<Text>();

            businnessPage.upTime_month_one = BusinessRecords_Page.transform.Find("BusinessInfo/Uptime_Month_One").GetComponent<Text>();
            businnessPage.upTime_month_two = BusinessRecords_Page.transform.Find("BusinessInfo/Uptime_Month_Two").GetComponent<Text>();
            businnessPage.upTime_month_three = BusinessRecords_Page.transform.Find("BusinessInfo/Uptime_Month_Three").GetComponent<Text>();

            businnessPage.month_one = BusinessRecords_Page.transform.Find("Month/Month_One").GetComponent<Text>();
            businnessPage.month_two = BusinessRecords_Page.transform.Find("Month/Month_Two").GetComponent<Text>();
            businnessPage.month_three = BusinessRecords_Page.transform.Find("Month/Month_Three").GetComponent<Text>();

            businnessPage.titleList.Add(businnessPage.Text_title);
            businnessPage.titleList.Add(businnessPage.Text_tip);
            businnessPage.titleList.Add(businnessPage.Text_coinsTitle);
            businnessPage.titleList.Add(businnessPage.Text_gameTimeTitle);
            businnessPage.titleList.Add(businnessPage.Text_upTimeTitle);
            businnessPage.titleList.Add(businnessPage.Text_this_month_tip);
            businnessPage.titleList.Add(businnessPage.Text_one_month_ago_tip);
            businnessPage.titleList.Add(businnessPage.Text_two_month_ago_tip);

            businnessPage.monthList.Add(businnessPage.month_one);
            businnessPage.monthList.Add(businnessPage.month_two);
            businnessPage.monthList.Add(businnessPage.month_three);

            businnessPage.coinsmonthList.Add(businnessPage.coins_month_one);
            businnessPage.coinsmonthList.Add(businnessPage.coins_month_two);
            businnessPage.coinsmonthList.Add(businnessPage.coins_month_three);

            businnessPage.gametimemonthList.Add(businnessPage.gameTime_month_one);
            businnessPage.gametimemonthList.Add(businnessPage.gameTime_month_two);
            businnessPage.gametimemonthList.Add(businnessPage.gameTime_month_three);

            businnessPage.uptimemonthList.Add(businnessPage.upTime_month_one);
            businnessPage.uptimemonthList.Add(businnessPage.upTime_month_two);
            businnessPage.uptimemonthList.Add(businnessPage.upTime_month_three);
        }

        //TotalRecord_Page
        {
            //totalPage = new TotalPage();
            totalPage = new TotalPage();
            totalPage.titleList = new List<Text>();
            totalPage.valueList = new List<Text>();
            totalPage.Text_title = TotalRecord_Page.transform.Find("Tips/Title").GetComponent<Text>();
            totalPage.Text_gameTimesTips = TotalRecord_Page.transform.Find("Tips/GameTimesTips").GetComponent<Text>();
            totalPage.Text_totalGameTImesTips = TotalRecord_Page.transform.Find("Tips/TotalGameTImesTips").GetComponent<Text>();
            totalPage.Text_upTime = TotalRecord_Page.transform.Find("Tips/UpTime").GetComponent<Text>();
            totalPage.Text_totalCoin = TotalRecord_Page.transform.Find("Tips/TotalCoin").GetComponent<Text>();

            totalPage.gameTimesValue = TotalRecord_Page.transform.Find("Value/GameTimesTips").GetComponent<Text>();
            totalPage.totalGameTimesValue = TotalRecord_Page.transform.Find("Value/TotalGameTImesTips").GetComponent<Text>();
            totalPage.upTimeValue = TotalRecord_Page.transform.Find("Value/UpTime").GetComponent<Text>();
            totalPage.totalCoinValue = TotalRecord_Page.transform.Find("Value/TotalCoin").GetComponent<Text>();

            totalPage.titleList.Add(totalPage.Text_title);
            totalPage.titleList.Add(totalPage.Text_gameTimesTips);
            totalPage.titleList.Add(totalPage.Text_totalGameTImesTips);
            totalPage.titleList.Add(totalPage.Text_upTime);
            totalPage.titleList.Add(totalPage.Text_totalCoin);

            totalPage.valueList.Add(totalPage.gameTimesValue);
            totalPage.valueList.Add(totalPage.totalGameTimesValue);
            totalPage.valueList.Add(totalPage.upTimeValue);
            totalPage.valueList.Add(totalPage.totalCoinValue);
        }

        //DataRest_Page
        {
            //dataRestPage = new DataRestPage();
            dataRestPage = new DataRestPage();
            dataRestPage.datarestToggleList = new List<Toggle>();
            dataRestPage.datarestTextList = new List<Text>();
            dataRestPage.datarestImageList = new List<Image>();
            dataRestPage.Toggle_Yes = DataReset_Page.transform.Find("Yes").GetComponent<Toggle>();
            dataRestPage.Toggle_No = DataReset_Page.transform.Find("No").GetComponent<Toggle>();
            dataRestPage.Text_title = DataReset_Page.transform.Find("Title").GetComponent<Text>();

            dataRestPage.datarestToggleList.Add(dataRestPage.Toggle_Yes);
            dataRestPage.datarestToggleList.Add(dataRestPage.Toggle_No);
            for (int i = 0; i < dataRestPage.datarestToggleList.Count; i++)
            {
                dataRestPage.datarestTextList.Add(dataRestPage.datarestToggleList[i].transform.Find("Label").GetComponent<Text>());
                dataRestPage.datarestImageList.Add(dataRestPage.datarestToggleList[i].transform.Find("Background/Checkmark").GetComponent<Image>());
            }
        }
       
        //Image_free
        {
            //imageFree = new ImageFree();
            imageFree = new ImageFree();
            imageFree.image_free_english = Image_Free.transform.Find("Free_Chinese").GetComponent<Image>();
            imageFree.image_free_chinese = Image_Free.transform.Find("Free_Englisth").GetComponent<Image>();
        }
    }

}
