using UnityEngine;
using System.Collections;
using Need.Mx;
using System;
using Update;
using System.IO;
/// <summary>
/// 资源更新 VO
/// </summary>
public class UpdateModel
{
    private readonly string ServerIP = "113.107.205.161";//单线;
    private readonly string DServerIP = "119.38.160.161";//双线;
    private readonly string IServerIP = "192.168.180.33";//内网;
    private readonly string _updateHost = "yzxxplist.vxinyou.com";//Plist文件所在域名;
    private readonly string Domain = "ylxxres.vxinyou.com";//域名;
    private string resserver = "https://yzxxplist.vxinyou.com/updaterelease/update.php?";

    public UpdateModel()
    {
        //this.ServerHost = Domain;
        //this.HeadHost = Domain;
        //this.Port = 80;
        //string localResServer = IOHelper.OpenText(Application.persistentDataPath + "/Res/resserver.txt");
        //if (!string.IsNullOrEmpty(localResServer.Trim()))
        //{
        //    this.resserver = localResServer;
        //}
        //Version ver = Resources.Load("Version", typeof(Version)) as Version;
        //if (ver != null)
        //{
        //    this._clientVersion = ver.resourceVersion;
        //}
        //this.IsAllPackage = Directory.Exists(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/Res/") ? true : false;
        //this.LocalVersion = new VersionVO(this._clientVersion);
        //this.LocalVersion.InitResversion();
        //this.RunPlaformType = this.GetRunPlaformType();
        //this.LocalResPathRoot = this.GetLocalResPath(this.RunPlaformType);
        //this.Platform = this.RunPlaformType.ToString().ToLower();
        //ChannelType ct;
        //switch (ver.choosePlatform)
        //{
        //    case enumPlatform.APP_STORE:
        //        {
        //            ct = ChannelType.appstore;
        //        }
        //        break;
        //    case enumPlatform.IOS_7659:
        //        {
        //            ct = ChannelType.applehelper;
        //        }
        //        break;
        //    case enumPlatform.IOS_91:
        //        {
        //            ct = ChannelType.fixserver;
        //        }
        //        break;
        //    case enumPlatform.IOS_PP:
        //        {
        //            ct = ChannelType.pp;
        //        }
        //        break;
        //    case enumPlatform.IOS_TONGBUTUI:
        //        {
        //            ct = ChannelType.syncpush;
        //        }
        //        break;
        //    case enumPlatform.UNDEFINED:
        //        {
        //            ct = ChannelType.banshu;
        //        }
        //        break;
        //    case enumPlatform.IOS_ITOOLS:
        //        {
        //            ct = ChannelType.itools;
        //        }
        //        break;
        //    case enumPlatform.IOS_HAIMA:
        //        {
        //            ct = ChannelType.haima;
        //        }
        //        break;
        //    case enumPlatform.IOS_I4:
        //        {
        //            ct = ChannelType.aisi;
        //        }
        //        break;
        //    case enumPlatform.IOS_XY:
        //        {
        //            ct = ChannelType.xyzs;
        //        }
        //        break;
        //    default:
        //        {
        //            ct = ChannelType.appstore;
        //        }
        //        break;
        //}
        //this._channel = ct;

        //this._channel = ChannelType.pp;
        this.RunPlaformType = global::RunPlaformType.WIN;
        this.Platform = "windows";
        //this.IsAllPackage = false;
        //this.LocalVersion = new VersionVO("0.0.0.1");
        //this._clientVersion = "0.0.0.1";
        //this.resserver = "http://192.168.64.203/diablo/update_west/update.php?";
    }

    #region 属性;
    /// <summary>
    /// 资源服务器地址;
    /// </summary>
    public string ResServerUrl
    {
        get
        {
            return string.Format(this.resserver + "os={0}&localversion={1}&clientversion={2}&channel={3}&guid={4}", this.Platform, this.LocalVersion, this.ClientVersion, this.Channel, Guid.NewGuid().ToString());
        }
    }
    private string _clientVersion = string.Empty;

    /// <summary>
    /// 客户端版本号;
    /// </summary>
    public VersionVO ClientVersion
    {
        get
        {
            return new VersionVO(this._clientVersion);
        }
    }
    /// <summary>
    /// 本地资源根目录;
    /// </summary>
    public string LocalResPathRoot
    {
        get;
        private set;
    }
    /// <summary>
    /// 运行平台类型;
    /// </summary>
    public RunPlaformType RunPlaformType
    {
        get;
        private set;
    }
    /// <summary>
    /// 资源服务器IP
    /// </summary>
    public string ServerHost
    {
        get;
        set;
    }
    ///// <summary>
    ///// Plist文件所在域名;
    ///// </summary>
    //public string UpdateHost
    //{
    //    get
    //    {
    //        return _updateHost;
    //    }
    //}
    /// <summary>
    /// 资源服务器端口;
    /// </summary>
    public int Port
    {
        get;
        set;
    }
    /// <summary>
    /// Http 头服务器IP+Port;
    /// </summary>
    public string HeadHost
    {
        get;
        set;
    }
    /// <summary>
    /// 本地版本号;
    /// </summary>
    public VersionVO LocalVersion
    {
        get;
        set;
    }
    /// <summary>
    /// 服务器版本号;
    /// </summary>
    public VersionVO ServerVersion
    {
        get;
        set;
    }
    private ChannelType _channel = ChannelType.syncpush;
    /// <summary>
    /// 渠道;
    /// </summary>
    public string Channel
    {
        get
        {
            return _channel.ToString();
        }
    }
    /// <summary>
    /// 平台;
    /// </summary>
    public string Platform
    {
        get;
        private set;
    }
    /// <summary>
    /// 是否是完整资源包 版本;
    /// </summary>
    public bool IsAllPackage
    {
        get;
        private set;
    } ///
    #endregion

    #region Method
    /// <summary>
    /// 获取运行平台
    /// </summary>
    /// <returns></returns>
    private RunPlaformType GetRunPlaformType()
    {
        RunPlaformType runPlaformType = RunPlaformType.IOS;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                runPlaformType = RunPlaformType.Android;
                break;
            case RuntimePlatform.IPhonePlayer:
                runPlaformType = RunPlaformType.IOS;
                break;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsWebPlayer:
                runPlaformType = RunPlaformType.WIN;
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.OSXWebPlayer:
                runPlaformType = RunPlaformType.MAC;
                break;
        }
        Debug.Log(runPlaformType);
        return runPlaformType;
    }
    /// <summary>
    /// 获取本地资源根路径
    /// </summary>
    /// <returns></returns>
    private string GetLocalResPath(RunPlaformType runPlatformType)
    {
        string path = string.Empty;
        switch (runPlatformType)
        {
            case RunPlaformType.Android:
                //注意这里原本是"jar:file://" + Application.dataPath + "!/assets/";
                //因为Application.dataPath开头有一个/,所以忽略一个"jar:file://"的/
                path = "jar:file:/" + Application.dataPath + "!/assets/Res/";
                break;
            case RunPlaformType.IOS:
                path = Application.dataPath + "/Raw/Res/";
                break;
            case RunPlaformType.WIN:
                path = Application.dataPath +  "/StreamingAssets/Res/";
                break;
            case RunPlaformType.MAC:
                path = Application.dataPath + "/StreamingAssets/Res/";
                break;
        }
        return path;
    }
    #endregion

}
