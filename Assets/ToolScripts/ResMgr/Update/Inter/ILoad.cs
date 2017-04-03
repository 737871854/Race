using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Update
{
    public interface ILoad
    {
        /// <summary>
        /// 下载本地文件;
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="completeLoadHandler"></param>
        void GetLocalFile(string filePath,UpdateEventHandler completeLoadHandler);
        /// <summary>
        /// 下载远程文件;
        /// </summary>
        /// <param name="downFileVO">DownFileVO</param>
        /// <param name="needSave">是否需要保存在本地</param>
        void GetWebFile(DownFileVO downFileVO, UpdateEventHandler completeLoadHandler, bool needSave);
    }
}
