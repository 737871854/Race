using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Safari指令获取;
/// </summary>
public class SafariCommand  {
    /// <summary>
    /// 转换 Itms-services协议 获取更新地址;
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string TransferUpdateUrl(string url)
    {
        return "itms-services://?action=download-manifest&url=https://" + url + "?" + RandomNum();
    }
    private static string RandomNum()
    {
        System.Random random = new System.Random((int)DateTime.Now.Ticks);
        return random.NextDouble().ToString();
    }
}
