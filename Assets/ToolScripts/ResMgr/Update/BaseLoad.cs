using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Update
{
    public delegate void UpdateEventHandler(object loadHelper);
    public class BaseLoad : ILoad
    {
        protected UpdateEventHandler ErrorHandler;// 下载失败;
        protected readonly int maxTryCount = 5;//尝试下载最多次数;
        protected string _errorStr = string.Empty;
        /// <summary>
        /// 下载错误信息;
        /// </summary>
        public string errorStr {
            get {
                return _errorStr;
            }
            set {
                _errorStr = value;
            }
        }
        #region 属性;
        public string FilePath
        {
            get;
            private set;
        }
        #endregion
        

        #region 需要重写的方法;
        public virtual void GetLocalFile(string filePath, UpdateEventHandler completeLoadHandler)
        {
        }
        public virtual void GetWebFile(DownFileVO downFileVO, UpdateEventHandler completeLoadHandler, bool needSave)
        {
        }
        #endregion
      
        public void SetEventHandler(UpdateEventHandler errorHandler)
        {
            this.ErrorHandler = errorHandler;
        }
        #region IO操作;
        ///// <summary>
        ///// 将流转换成文件;
        ///// </summary>
        ///// <param name="stream"></param>
        ///// <param name="fileName"></param>
        //public void StreamToFile(Stream stream, string fileName)
        //{
        //    using (stream)
        //    {
        //        // 把 Stream 转换成 byte[] 
        //        byte[] bytes = new byte[stream.Length];
        //        stream.Read(bytes, 0, bytes.Length);
        //        // 设置当前流的位置为流的开始;
        //        stream.Seek(0, SeekOrigin.Begin);
        //        // 把 byte[] 写入文件 
        //        string dir =fileName.Substring(0,fileName.LastIndexOf('/'));
        //        if (!Directory.Exists(dir)) {
        //            Directory.CreateDirectory(dir);
        //        }
        //        using (FileStream fs = new FileStream(fileName, FileMode.Create))
        //        {
        //            using (BinaryWriter bw = new BinaryWriter(fs))
        //            {
        //                bw.Write(bytes);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 将二进制流保存为文件;
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="fileName"></param>
        public void BytesToFile(byte[] bytes, string fileName)
        {
            string dir = fileName.Substring(0, fileName.LastIndexOf('/'));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            FileInfo fileInfo = new FileInfo(fileName);
            if(!fileInfo.Exists){
                using( Stream sw =fileInfo.OpenWrite()){
                    sw.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                using (Stream sw = fileInfo.Create())
                {
                    sw.Write(bytes, 0, bytes.Length);
                }
            }
            //using (FileStream fs = new FileStream(fileName, FileMode.Create))
            //{
            //    using (BinaryWriter bw = new BinaryWriter(fs))
            //    {
            //        bw.Write(bytes);
            //    }
            //}
        }
        /// <summary>
        /// 将流转换成二进制数组;
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public byte[] StreamToByteArray(Stream stream)
        {
            using (stream)
            {
                byte[] byteArray = new byte[stream.Length];
                stream.Read(byteArray, 0, byteArray.Length);
                // 设置当前流的位置为流的开始;
                stream.Seek(0, SeekOrigin.Begin);
                return byteArray;
            }
        }
        /// <summary>
        /// 将二进制转换成流;
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public Stream ByteToStream(byte[] bytes) {
            return new MemoryStream(bytes);
        }
        #endregion






    }
}