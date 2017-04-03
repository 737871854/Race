using UnityEngine;
using System.Collections;
using System;
using Need.Mx;
using System.IO;
using Update;
using Update.Factory;
public class ResUpdateManager
{
    string progressInfo = string.Empty;

    #region 单例;
    private ResUpdateManager()
    {
        //this.progress = Progress.Instance;
       
        this.updateModel = new UpdateModel();
        NotifyFileSize = (uint fileSize) =>
        {
            //totalZipFileSize = fileSize;
            //this.progressInfo = "(" + Math.Round(Convert.ToDouble(fileSize) / (1024d * 1024d), 3).ToString() + "M)" + LangMgr.GetString(Language.UPDATE_RES_TIPS);
            //Debug.Log("更新包字节数:" + totalZipFileSize);
        };
        //更新进度条显示;
        NotifyDownLoadedSize = (int size) =>
        {
            currZipFileSize = currZipFileSize + (uint)size;
            //float percent = Mathf.Clamp01((currZipFileSize * 1.0f) / (totalZipFileSize * 1.0f));
            //progress.Show(percent, this.progressInfo);
        };
        NotifyDownLoadError = () =>
        {
            Debug.Log("下载更新包错误");
            ClearHttpDownloadData();
            //progress.Close();
            GameObject.Destroy(HttpDownLoader.instance.gameObject);
        };
        //更新包下载完毕回调;
        NotifyDownLoadedComplete = () =>
        { 
            IsDownLoadOver = true;
            GameObject.DestroyImmediate(HttpDownLoader.instance.gameObject);
            Debug.Log("更新包下载完:" + SaveZippath);
            if (!ZipHelper.UnZip(SaveZippath, this.updateModel.LocalResPathRoot))
            {
                //解压失败处理;
                if (File.Exists(SaveZippath))
                {
                    File.Delete(SaveZippath);
                }
                this.Start();
                return;
            }
            DeleteOldFile();

            // 在完成一次下载解压过程后，需要判断是否还有其他的包未下载
            // 则继续执行下载过程
            if (resServerVO.GetUrlCount() == 0)
            {
                SaveNewVersionFile(updateModel.ServerVersion.ToString());
                this.CurrentVersion = this.updateModel.ServerVersion.ToString();
                this.updateCompeleHandler();
                //BxFacade.Instance.SendNotification(NotifyDef.ResUpdateComplete);
            }
            else
            {
                Uri uri = new Uri(resServerVO.PopUrl());
                this.StartUpdatePackage(uri);
            }
        };
        this.load = UpdateFactory.CreateTcpLoad((loadHelper) =>
        {
            //MessageBoxVO vo = new MessageBoxVO();
            //vo.Confirm = "重试";
            //vo.Content = "网络故障 请重试";
            //vo.Type = MessageBoxType.SINGLE_BUTTON;
            //vo.ResultCallback = (r) =>
            //{
            //    if (r)
            //    {
            //        this.Start();
            //    }
            //};
            //BxFacade.Instance.SendNotification(NotifyDef.OPEN_MESSAGE_BOX_PANEL, vo);
        });
    }
    private static ResUpdateManager _instance;
    public static ResUpdateManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ResUpdateManager();
            }
            return _instance;
        }
    }
    #endregion

    #region 共用下载参数;
    public bool IsPromptOver;                                               //网络状态提示是否结束
    public bool IsRequestVerOver;                                           //请求资源版本信息是否结束
    public bool IsDownLoadOver;                                             //下载是否结束
    public bool IsInitResOver;                                              //zip方式，初始资源是否结束
    public int InternetDownLoadType;                                        //下载资源类型
    public string ResUrl;                                                   //资源下载地址
    public ResUpdateType ResUpdateType = ResUpdateType.Development;         //资源更新方式
    public int SaveResVer = 0;                                              //需要保存资源版本号
    #endregion

    private ILoad load;
    public UpdateModel updateModel = new UpdateModel();
    public ResServerVO resServerVO = new ResServerVO();
    public string AbsolutePath { get; set; }
    public string SaveZippath = string.Empty;
    public ulong ContentLength;
    public NotifyFileSizeHandler NotifyFileSize;

    public NotifyDownLoadedSizeHandler NotifyDownLoadedSize;

    public Action NotifyDownLoadError = null;
    public Action NotifyDownLoadedComplete = null;
    //private Progress progress;

    private Action updateCompeleHandler;
    uint totalZipFileSize = 0;
    uint currZipFileSize = 0;
    //long zipResFileTotalCount = 0;
    //long unPackZipResFileCount = 0;

    private readonly string _currentVersion = "CurrentVersion";
    public string CurrentVersion
    {
        get
        {
            return PlayerPrefs.GetString(_currentVersion, updateModel.LocalVersion.ToString());
        }
        private set
        {
            PlayerPrefs.SetString(_currentVersion, value);
        }
    }
    /// <summary>
    /// 初始化，进入状态后设置
    /// </summary>
    /// <param name="updateCompeleHandler"></param>
    /// <param name="tmpResUpdateType"></param>
    public void Init(Action updateCompeleHandler, ResUpdateType tmpResUpdateType = ResUpdateType.Development)
    {
        //this.progress.text = LangMgr.GetString(Language.UPDATE_RES_TIPS);//Language.UPDATE_RES_TIPS;
        this.updateCompeleHandler = updateCompeleHandler;
        ResUpdateType = tmpResUpdateType;
        ClearDataLoadData();                            //确保状态为初始状态，重试下需要;
    }

    public void Start()
    {
        //ClearHttpDownloadData();
        //resServerVO.Clear();
        //if (NetPrompt() && this.ResUpdateType == global::ResUpdateType.Production)
        //{
        //    #region 不要的方法;
        //    //GetServerVersion(() =>
        //    //{
        //    //    if (this.updateModel.ServerVersion.JudgeClient && this.updateModel.ClientVersion.CompareTo(this.updateModel.ServerVersion) < 0)
        //    //    {
        //    //        // UITool.ShowFloatMsg("客户端版本过低,请更新客户端" + this.updateModel.ServerVersion + ":" + this.updateModel.LocalVersion);
        //    //        MessageBoxVO vo = new MessageBoxVO();
        //    //        vo.Content = "版本过低,请更新游戏";
        //    //        vo.Type = MessageBoxType.SINGLE_BUTTON;
        //    //        vo.ResultCallback = (result) =>
        //    //        {
        //    //            string str = this.updateModel.UpdateHost + "/Package/" + this.updateModel.Platform + "/" + this.updateModel.Channel + "/thewest.plist";
        //    //            Application.OpenURL(SafariCommand.TransferUpdateUrl(str));
        //    //            Application.Quit();
        //    //        };
        //    //        BxFacade.Instance.SendNotification(NotifyDef.OPEN_MESSAGE_BOX_PANEL, vo);
        //    //        return;
        //    //    }
        //    //    else
        //    //    {
        //    //        string localVersionFile = this.updateModel.LocalResPathRoot + "lastversion.txt";
        //    //        if (File.Exists(localVersionFile))
        //    //        {
        //    //            this.updateModel.LocalVersion = new VersionVO(IOHelper.OpenText(localVersionFile).Trim());
        //    //            if (this.updateModel.IsAllPackage)
        //    //            {
        //    //                //比对客户端资源号和本地资源号;
        //    //                if (this.updateModel.ClientVersion.ResVersion > this.updateModel.LocalVersion.ResVersion)
        //    //                {
        //    //                    this.CopyRes((result) =>
        //    //                    {
        //    //                        if (result)
        //    //                        {
        //    //                            CompareResVersion();
        //    //                        }
        //    //                    });
        //    //                }
        //    //                else
        //    //                {
        //    //                    CompareResVersion();
        //    //                }
        //    //            }
        //    //            else
        //    //            {
        //    //                CompareResVersion();
        //    //            }
        //    //        }
        //    //        else
        //    //        {
        //    //            if (this.updateModel.IsAllPackage)
        //    //            {
        //    //                this.CopyRes((result) =>
        //    //                {
        //    //                    if (result)
        //    //                    {
        //    //                        CompareResVersion();
        //    //                    }
        //    //                });
        //    //            }
        //    //            else
        //    //            {
        //    //                DownAllPackage();
        //    //            }
        //    //        }
        //    //    }
        //    //}); 
        //    #endregion
        //    string localVersionFile = this.updateModel.LocalResPathRoot + "lastversion.txt";
        //    if (File.Exists(localVersionFile))
        //    {
        //        this.updateModel.LocalVersion = new VersionVO(IOHelper.OpenText(localVersionFile).Trim());
        //        if (this.updateModel.IsAllPackage)
        //        {
        //            //比对客户端版本号和本地版本号;
        //            if (this.updateModel.ClientVersion.CompareTo(this.updateModel.LocalVersion)==1)
        //            {
        //                this.CopyRes((result) =>
        //                {
        //                    if (result)
        //                    {
        //                        Main.CtrlContainer.StartCoroutine(RequestResServer());
        //                    }
        //                });
        //            }
        //            else
        //            {
        //                Main.CtrlContainer.StartCoroutine(RequestResServer());
        //            }
        //        }
        //        else
        //        {
        //            Main.CtrlContainer.StartCoroutine(RequestResServer()); 
        //        }
        //    }
        //    else
        //    {
        //        if (this.updateModel.IsAllPackage)
        //        {
        //            this.CopyRes((result) =>
        //            {
        //                if (result)
        //                {
        //                    Main.CtrlContainer.StartCoroutine(RequestResServer());
        //                }
        //            });
        //        }
        //        else
        //        {
        //            Main.CtrlContainer.StartCoroutine(RequestResServer());
        //        }
        //    }
        //}
        //else if (this.ResUpdateType == global::ResUpdateType.Development)
        //{
        //    updateCompeleHandler();
        //}
    }
    //IEnumerator RequestResServer()
    //{
        //Debug.Log("Res request parms:" + this.updateModel.ResServerUrl); 
        //WWW www = new WWW(this.updateModel.ResServerUrl);
        //yield return www;
        //if (www.isDone)
        //{
        //    string result = www.text.Trim();
        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        resServerVO.Init(LitJson.JsonMapper.ToObject(result));
        //        if (resServerVO.Success)
        //        {
        //            this.updateModel.ServerVersion = resServerVO.ServerVersion;
        //            switch (resServerVO.UType)
        //            {
        //                case UpdateType.resource:
        //                    Uri uri = new Uri(resServerVO.PopUrl());
        //                    NotifyFileSize(resServerVO.TotalLength);
        //                    this.StartUpdatePackage(uri);
        //                    break;
        //                case UpdateType.client:
        //                    //MessageBoxVO vo = new MessageBoxVO();
        //                    //vo.Content = "版本过低,请更新游戏";
        //                    //vo.Type = MessageBoxType.SINGLE_BUTTON;
        //                    //vo.ResultCallback = (r) =>
        //                    //{
        //                    //    Application.OpenURL(resServerVO.PopUrl());
        //                    //    Application.Quit();
        //                    //};
        //                    //BxFacade.Instance.SendNotification(NotifyDef.OPEN_MESSAGE_BOX_PANEL, vo);
        //                    break;
        //                case UpdateType.platform:
        //                    break;
        //                case UpdateType.none:
        //                    if (this.updateModel.LocalVersion.CompareTo(this.updateModel.ServerVersion) != 0)
        //                    {
        //                        this.updateModel.LocalVersion = this.updateModel.ServerVersion;
        //                        SaveNewVersionFile(this.updateModel.ServerVersion.ToString());
        //                    }
        //                    updateCompeleHandler();
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            Debug.Log("资源服务器返回的信息有错:" + result);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("资源服务器返回的消息为空");
        //    }
        //}
    //}
    ///// <summary>
    ///// 比对资源版本号 并开始下载资源;
    ///// </summary>
    //private void CompareResVersion()
    //{
    //    //比对资源号;
    //    if (this.updateModel.LocalVersion.ResVersion != this.updateModel.ServerVersion.ResVersion)
    //    {
    //        if (this.updateModel.LocalVersion.ResVersion < this.updateModel.ServerVersion.ResVersion)
    //            DownPartPackage();
    //        else
    //            Debug.Log("外网资源服务器版本号未更新!");
    //    }
    //    else
    //    {
    //        updateCompeleHandler();
    //    }
    //}
    ///// <summary>
    ///// 获取服务器版本号;
    ///// </summary>
    ///// <returns></returns>
    //private void GetServerVersion(Action action)
    //{
    //    if (this.updateModel.ServerVersion == null)
    //    {
    //        this.load.GetWebFile(new DownFileVO(this.updateModel.ServerHost + ":" + this.updateModel.Port.ToString() + "/Package/" + this.updateModel.Platform + "/" + this.updateModel.Channel + "/lastversion.txt"), (loadHelper) =>
    //        {

    //            string version = (loadHelper as HttpLoadHelper) == null ? loadHelper.ToString() : (loadHelper as HttpLoadHelper).WWWObj.text.Trim();
    //            this.updateModel.ServerVersion = new VersionVO(version);
    //            Debug.Log("serverversion:" + version);
    //            action();
    //        }, false);
    //    }
    //    else
    //    {
    //        action();
    //    }
    //}

    #region zip下载成功后操作;
    //通过delete.json文件删除不需要的资源文件;
    private void DeleteOldFile()
    {
        try
        {
            string deleteFile = this.updateModel.LocalResPathRoot + "delete.json";
            if (File.Exists(deleteFile))
            {
                string json = File.ReadAllText(deleteFile, System.Text.Encoding.UTF8);
                SVNDeleteVO deleteVO = LitJson.JsonMapper.ToObject<SVNDeleteVO>(json);
                if (deleteVO != null && deleteVO.ListDModel != null)
                {
                    foreach (SVNDeleteModel item in deleteVO.ListDModel)
                    {
                        switch ((SvnNodeKind)item.NodeType)
                        {
                            case SvnNodeKind.File:
                                if (File.Exists(item.Path))
                                {
                                    File.Delete(item.Path);
                                }
                                break;
                            case SvnNodeKind.Directory:
                                if (Directory.Exists(item.Path))
                                {
                                    IOHelper.DeleteFolder(item.Path);
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            if (Directory.Exists(this.updateModel.LocalResPathRoot + "update"))
            {
                IOHelper.DeleteFolder(this.updateModel.LocalResPathRoot + "update");
            }
        }
        catch (Exception ex)
        {
            Debug.Log("删除Old资源错误:" + ex.ToString());
        }
    }
    //保存新的版本文件;
    private void SaveNewVersionFile(string txt)
    {
        try
        {
            string localVersionFile = this.updateModel.LocalResPathRoot + "lastversion.txt";
            IOHelper.CreateTextFile(txt, localVersionFile);
        }
        catch (Exception ex)
        {
            Debug.Log("保存最新version文件错误:" + ex.ToString());
        }

    }
    #endregion

    #region 辅助方法;
    /// <summary>
    /// 复制ipa中整个资源文件夹;
    /// </summary>
    /// <returns></returns>
    private void CopyRes(Action<bool> action)
    {
        //GameStartLoadingView.Instance.Start();
        //bool result = false;
        //ThreadManager.StartSingleThread(() =>
        //{
        //    string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/Res";
        //    try
        //    {
        //        if (Directory.Exists(path))
        //        {

        //            if (Directory.Exists(this.updateModel.LocalResPathRoot))
        //            {
        //                IOHelper.DeleteFolder(this.updateModel.LocalResPathRoot);
        //            }
        //            IOHelper.CopyDirectory(path, this.updateModel.LocalResPathRoot);
        //            SaveNewVersionFile(this.updateModel.ClientVersion.ToString());
        //            this.updateModel.LocalVersion = this.updateModel.ClientVersion;
        //            result = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.LogError("Copy res folder fail:" + ex.ToString());
        //    }
        //    finally
        //    {
        //        ThreadManager.DispatchToMainThread(() =>
        //        {
        //            GameStartLoadingView.Instance.Close();

        //        });
        //        ThreadManager.WaitForNextFrame();
        //        ThreadManager.DispatchToMainThread(() =>
        //        {
        //            action(result);
        //        });
        //    }
        //});

    }

    /// <summary>
    /// 检测当前网络，并提示
    /// </summary>
    private bool NetPrompt()
    {
        bool result = false;
        switch (Application.internetReachability)
        {
            case NetworkReachability.NotReachable:
                //MessageBoxVO vo = new MessageBoxVO();
                //vo.Confirm = "重试";
                //vo.Content = "网络不通 请检查网络";
                //vo.Type = MessageBoxType.SINGLE_BUTTON;
                //vo.ResultCallback = (r) =>
                //{
                //    if (r)
                //    {
                //        this.Start();
                //    }
                //};
                //BxFacade.Instance.SendNotification(NotifyDef.OPEN_MESSAGE_BOX_PANEL, vo);
                Debug.Log("网络不可用");
                //Log.Print("网络不可用");
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                //弹出提示框建议wifi环境下更新资源，不下载也可进入游戏下载资源，但会走运营商流量;
                Debug.Log("运营商网络");
                result = true;
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                //Wifi 本地网络;
                result = true;
                break;
        }
        return result;
    }

    #region 资源包下载;
    ///// <summary>
    ///// 下载部分资源包;
    ///// </summary>
    //private void DownPartPackage()
    //{
    //    Uri uri = new Uri(@"http://" + this.updateModel.ServerHost + ":" + this.updateModel.Port + "/Package/" + this.updateModel.Platform + "/" + this.updateModel.Channel + "/" + this.updateModel.LocalVersion.ResVersion + "_" + this.updateModel.ServerVersion.ResVersion + "/package.zip");
    //    this.StartUpdatePackage(uri);
    //}
    ///// <summary>
    ///// 下载完整资源包;
    ///// </summary>
    //private void DownAllPackage()
    //{
    //    Uri uri = new Uri(@"http://" + this.updateModel.ServerHost + ":" + this.updateModel.Port + "/Package/" + this.updateModel.Platform + "/" + this.updateModel.Channel + "/apackage.zip");
    //    this.StartUpdatePackage(uri);
    //}
    /// <summary>
    /// 开始下载资源包;
    /// </summary>
    private void StartUpdatePackage(Uri uri)
    {
        this.updateModel.ServerHost = uri.Host;
        this.updateModel.Port = uri.Port;
        this.updateModel.HeadHost = this.updateModel.ServerHost;
        if (uri.Port != 80)
            this.updateModel.HeadHost += ":" + uri.Port;
        AbsolutePath = uri.AbsolutePath;
        SaveZippath = this.updateModel.LocalResPathRoot + "update/package.zip";
        HttpDownLoader httpDownLoader = HttpDownLoader.instance;
    }
    #endregion
    /// <summary>
    /// 清除下载数据
    /// </summary>
    private void ClearDataLoadData()
    {
        IsPromptOver = false;
        IsRequestVerOver = false;
        IsDownLoadOver = false;
        IsInitResOver = false;
    }

    /// <summary>
    /// 清除Http下载数据
    /// </summary>
    private void ClearHttpDownloadData()
    {
        totalZipFileSize = 0;
        currZipFileSize = 0;
        //zipResFileTotalCount = 0;
        //unPackZipResFileCount = 0;
    }
    /// <summary>
    /// 获取文件完整路径;
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public string GetFilePath(string file)
    {
        string filePath = string.Empty;

        if (this.updateModel.RunPlaformType == RunPlaformType.IOS)
        {
            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "IOS/", false);
        }
        else if (this.updateModel.RunPlaformType == RunPlaformType.WIN)
        {

            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "WIN/", false);
        }
        else if (this.updateModel.RunPlaformType == RunPlaformType.Android)
        {
            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "Android/", false);
        }
        //Debug.Log("FilePath1:" + filePath);
        if (!File.Exists(filePath))
        {
            if (this.updateModel.RunPlaformType == RunPlaformType.WIN || this.updateModel.RunPlaformType == RunPlaformType.MAC)
            {
                filePath = GetFilePath(file,Application.dataPath + "/StreamingAssets/Res/WIN/", true);
            }
            else if(this.updateModel.RunPlaformType == RunPlaformType.IOS)
            {
                filePath = GetFilePath(file,Application.dataPath + "/Raw/Res/IOS/", true);
            }
            else if (this.updateModel.RunPlaformType == RunPlaformType.Android)
            {
                filePath = GetFilePath(file, "jar:file:/" + Application.dataPath + "!/assets/Res/Android/", true);
            }
        }
        //Debug.Log("FilePath2:" + filePath);
        return filePath;
    }
    /// <summary>
    /// 判断文件是否存在;
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool IsFileExist(string file)
    {
        bool result = false;
        string filePath = string.Empty;
        if (this.updateModel.RunPlaformType == RunPlaformType.IOS)
        {
            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "IOS/", false);
        }
        else if (updateModel.RunPlaformType == RunPlaformType.Android)
        {
            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "Android/", false);
        }
        else if (this.updateModel.RunPlaformType == RunPlaformType.WIN)
        {

            filePath = this.GetFilePath(file, this.updateModel.LocalResPathRoot + "WIN/", false);
        }
        if (!File.Exists(filePath))
        {
            if (this.updateModel.RunPlaformType == RunPlaformType.WIN || this.updateModel.RunPlaformType == RunPlaformType.MAC)
            {
                filePath = GetFilePath(file, Application.dataPath + "/StreamingAssets/Res/WIN/", true);
            }
            else if (this.updateModel.RunPlaformType == RunPlaformType.IOS)
            {
                filePath = GetFilePath(file, Application.dataPath + "/Raw/Res/IOS/", true);
            }
            else if (this.updateModel.RunPlaformType == RunPlaformType.Android)
            {
                filePath = GetFilePath(file, "jar:file:/" + Application.dataPath + "!/assets/Res/Android/", true);
            }
        }
        result = File.Exists(filePath);
        return result;
    }
    private string GetFilePath(string file, string path, bool needLog = true)
    {
        string filePath = string.Empty;
        filePath = path + file;
        if (needLog)
        {
            if (!File.Exists(filePath))
            {
                string error = "加载错误 获取不到完整路径---需要加载的文件:" + filePath;
                UnityEngine.Debug.LogWarning(error);
                Log.Print(error);
            }
        }
        return filePath;
    }

    #endregion
}
