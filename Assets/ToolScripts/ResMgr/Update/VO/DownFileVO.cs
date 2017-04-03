using UnityEngine;
using System.Collections;

/// <summary>
/// 文件需要下载的路径;文件需要保存的路径;
/// </summary>
public class DownFileVO
{
    public DownFileVO(string downFilePath, string saveFilePath = "")
    {
        this.DownFilePath = downFilePath;
        this.SaveFilePath = saveFilePath;
    }
    /// <summary>
    ///  文件需要下载的路径;
    /// </summary>
    public string DownFilePath
    {
        get;
        private set;
    }
    /// <summary>
    /// 文件需要保存的路径;
    /// </summary>
    public string SaveFilePath
    {
        get;
        private set;
    }
    public WWW www
    {
        get;
        set;
    }
}