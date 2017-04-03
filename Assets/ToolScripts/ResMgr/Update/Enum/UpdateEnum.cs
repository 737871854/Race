using UnityEngine;
/// <summary>
/// 运行平台类型
/// </summary>
public enum RunPlaformType
{
    IOS,
    Android,
    WIN,
    MAC
}

/// <summary>
/// 资源更新方式
/// </summary>
public enum ResUpdateType
{
    /// <summary>
    /// 开发
    /// </summary>
    Development,
    /// <summary>
    /// 产品
    /// </summary>
    Production
}
public enum UpdatePathType
{
    /// <summary>
    ///本地资源;
    /// </summary>
    Local,
    /// <summary>
    /// 局域网;
    /// </summary>
    LocalNetwork,
    Net,
    /// <summary>
    /// 策划调试地址;
    /// </summary>
    Debug
}
public enum SvnNodeKind
{
    None,
    File,
    Directory,
    Unknown
}
public enum ChannelType
{
    /// <summary>
    /// App Store;
    /// </summary>
    appstore,
    /// <summary>
    /// PP助手;
    /// </summary>
    pp,
    /// <summary>
    /// 同步推;
    /// </summary>
    syncpush,
    /// <summary>
    /// 快用苹果助手;
    /// </summary>
    applehelper,
    /// <summary>
    /// 91混服;
    /// </summary>
    fixserver,
    /// <summary>
    /// 版署;
    /// </summary>
    banshu,
    /// <summary>
    ///海马;
    /// </summary>
    haima,
    /// <summary>
    ///itools;
    /// </summary>
    itools,
    /// <summary>
    ///爱思;
    /// </summary>
    aisi,
    /// <summary>
    ///xy助手;
    /// </summary>
    xyzs
}
public enum UpdateType
{
    /// <summary>
    /// 资源;
    /// </summary>
    resource,
    /// <summary>
    /// 客户端;
    /// </summary>
    client,
    /// <summary>
    /// 平台客户端更新;
    /// </summary>
    platform,
    /// <summary>
    /// 不做任何更新;
    /// </summary>
    none
}