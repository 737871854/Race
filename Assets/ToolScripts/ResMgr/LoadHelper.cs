/**
* Copyright (c) 2015,Need Corp. ltd;
* All rights reserved.
* 
* 文件名称：LoadHelper.cs
* 简    述：在资源管理类ResManager.Instance中内部使用的加载帮助类，在加载成功时的回调函数也会当做传入
* (现在似乎不需要，考虑去除)。
* 创建标识：Lorry  2012/10/28
*/
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Update;

namespace Need.Mx
{
    /// <summary>
    /// Load helper.
    /// </summary>
    public class LoadHelper
    {
        #region 私有变量;


        #endregion

        #region 属性;
        /// <summary>
        /// 是否已经开始下载;
        /// </summary>
        public bool Started
        {
            get
            {
                if (this.WWWObj == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 加载进度回调;
        /// </summary>
        public LoadHandler ProgressHandler
        {
            get;
            private set;
        }
        /// <summary>
        /// 加载错误回调;
        /// </summary>
        public LoadHandler ErrorHandler
        {
            get;
            private set;
        }
        /// <summary>
        /// 加载完成回调;
        /// </summary>
        public LoadHandler CompleteHandler
        {
            get;
            private set;
        }
        public XMLResolverHandler XMLResolver
        {
            get;
            private set;
        }
        /// <summary>
        /// 最多下载次数;
        /// </summary>
        public readonly int MaxTryCount = 3;
        /// <summary>
        /// 当前已经请求次数;
        /// </summary>
        public int currentRequestCount = 0;
        private string _url = string.Empty;
        public string Url
        {
            get
            {
                return _url;
            }
            private set
            {
                _url = value;
            }
        }
        /// <summary>
        /// 判断是否是公共资源;
        /// </summary>
        public bool IsPublicRes
        {
            get;
            private set;
        }
        public WWW WWWObj
        {
            get;
            private set;
        }
        private string _fileName = string.Empty;
        /// <summary>
        /// 下载的文件名称;
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(_fileName))
                {
                    _fileName = this._url.Substring(this._url.LastIndexOf('/') + 1).Split('.')[0];
                }
                return _fileName;
            }
        }
        private string _extensionName = string.Empty;
        /// <summary>
        /// 扩展名;
        /// </summary>
        public string ExtensionName
        {
            get
            {
                return _extensionName;
            }
            private set
            {
                _extensionName = value;
            }
        }
        /// <summary>
        /// 原始url;
        /// </summary>
        public string OriginalUrl
        {
            get;
            private set;
        }
        #endregion

        #region 构造函数;
        public LoadHelper(string url, LoadHandler completeHandler, LoadHandler errorHandler, LoadHandler progressHandler, XMLResolverHandler resolverHandler = null)
        {
            this.CompleteHandler = completeHandler;
            this.ErrorHandler = errorHandler;
            this.ProgressHandler = progressHandler;
            this.OriginalUrl = url;
            this.ExtensionName = Path.GetExtension(url);
            this._url = this.ConvertUrl(url,ExtensionName);
            this.IsPublicRes = this.FileName.Contains("_public");
            this.XMLResolver = resolverHandler;
        }
        #endregion

        #region 公开方法;

        /// <summary>
        /// 是否可以重新下载;
        /// </summary>
        /// <returns></returns>
        public bool NeedLoadAgain()
        {
            if (currentRequestCount < MaxTryCount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 是否是外网资源;
        /// </summary>
        public bool IsWebResource
        {
            get;
            private set;
        }
        #endregion

        #region 私有方法;

        private string ConvertUrl(string url, string extension)
        {
            //switch (extension)
            //{
            //    case ".xml":
            //    case ".unity3d":
            //        //根据当前平台选择对应资源;现在假定只有编辑器生成的配置
            //        //使用xml配置; 之后需要进一步修改Lorry
            //        break;
            //    default:
            //        return url;
            //}
            return ResUpdateManager.Instance.GetFilePath(url);
        }
        #endregion
    }

}