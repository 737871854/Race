using UnityEngine;
using System.Collections;
using System;
public class VersionVO : IComparable<VersionVO>
{
    private string version = string.Empty;
    public VersionVO(string version)
    {

        string[] versions = version.Split('.');
        this.version = version;
        if (versions.Length!=0)
        {
            this.ExpansionVersion = versions[0].ToInt();
        }
        if (versions.Length > 1)
        {
            this.ClientVersion = versions[1].ToInt();
        }
        if (versions.Length > 2)
        {
            this.ClientChildVersion = versions[2].ToInt();
        }
        if (versions.Length >3)
        {
            this.ResVersion = versions[3].ToInt();
        }
    }
    ///// <summary>
    ///// 判断是否检查客户端版本;
    ///// </summary>
    //public bool JudgeClient
    //{
    //    get;
    //    set; 
    //}
    /// <summary>
    /// 资料片号;
    /// </summary>
    public int ExpansionVersion
    {
        get;
        set;
    }
    /// <summary>
    /// 大版本号;
    /// </summary>
    public int ClientVersion
    {
        get;
        private set;
    }
    /// <summary>
    /// 小版本号;
    /// </summary>
    public int ClientChildVersion
    {
        get;
        private set;
    }
    /// <summary>
    /// 资源号;
    /// </summary>
    public int ResVersion
    {
        get;
        private set;
    }
   
    public override string ToString()
    {
        return this.version;
    }
    /// <summary>
    /// 初始化资源号;
    /// </summary>
    /// <returns></returns>
    public void InitResversion()
    {
        this.ResVersion = -1;
    }
    //public int CompareTo(VersionVO other)
    //{
    //    float c =  (this.ExpansionVersion.ToString() + "." + this.ClientVersion.ToString()).ToFloat();
    //    float o = (other.ExpansionVersion.ToString() + "." + other.ClientVersion.ToString()).ToFloat();
    //    return c.CompareTo(o);
    //}

    public int CompareTo(VersionVO other)
    {
        int result =this.ExpansionVersion.CompareTo(other.ExpansionVersion);
        if (result == 0)
        {
            result = this.ClientVersion.CompareTo(other.ClientVersion);
            if (result == 0)
            {
                result = this.ClientChildVersion.CompareTo(other.ClientChildVersion);
                if (result == 0)
                {
                    result = this.ResVersion.CompareTo(other.ResVersion);
                }
            }
        }
        return result;
    }
}
