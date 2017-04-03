using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using UnityEngine;
using System.Collections;
using Update.Factory;
using Need.Mx;

namespace Update
{
    public class HttpLoad : BaseLoad
    {
        private List<HttpLoadHelper> listHttpLoadHelper = new List<HttpLoadHelper>();
        public GameObject helperGO;
        private int MaxDownCount = 3;//最大同时下载数;
        private int currentDownCount = 0;//当前下载数;
        private int timeOut = 20;
        public HttpLoad()
        {
            if (helperGO == null)
            {
                helperGO = new GameObject("HttpLoadHelper");
                helperGO.AddComponent<HttpLoadGameObject>();
            }
        }
        #region 重写父类的方法;
        public override void GetLocalFile(string filePath, UpdateEventHandler completeLoadHandler)
        {
            listHttpLoadHelper.Add(GetHttpLoadHelper(new DownFileVO(filePath, ""), completeLoadHandler));
        }
        public override void GetWebFile(DownFileVO downFileVO, UpdateEventHandler completeLoadHandler, bool needSave)
        {
            listHttpLoadHelper.Add(GetHttpLoadHelper(downFileVO, completeLoadHandler, false, needSave));
        }
        #endregion

        public void Update()
        {
            for (int i = 0; i < listHttpLoadHelper.Count; i++)
            {
                HttpLoadHelper loadObj = listHttpLoadHelper[i];
                if (currentDownCount < maxTryCount && loadObj.isStartDown == false)
                {
                    loadObj.StartLoad();
                    currentDownCount++;
                    continue;
                }
                else if (loadObj.isStartDown == false || loadObj.WWWObj==null)
                {
                    continue;
                }
                bool isTimeOut = loadObj.IsTimeOut();
                if (loadObj.isStartDown)
                {
                    if (isTimeOut || !string.IsNullOrEmpty(loadObj.WWWObj.error))
                    {
                        if (isTimeOut)
                        {
                            Debug.Log("Load TimeOut:" + loadObj.WWWObj.url);
                            Log.Print("Load TimeOut:" + loadObj.WWWObj.url);
                        }
                        else
                        {
                            Debug.Log(loadObj.WWWObj.error);
                            Log.Print(loadObj.WWWObj.error + ":" + loadObj.WWWObj.url);
                        }
                        if (loadObj.NeedLoadAgain())
                        {
                            Debug.Log("Load Again:" + loadObj.WWWObj.url);
                            Log.Print("Load Again:" + loadObj.WWWObj.url);
                            loadObj.StartLoad();
                            continue;
                        }
                        else
                        {
                            ErrorHandler(loadObj);
                            loadObj.WWWObj.Dispose();
                            listHttpLoadHelper.RemoveAt(i);
                            currentDownCount--;
                        }
                        continue;
                    }

                    if (loadObj.WWWObj.isDone)
                    {
                        try
                        {
                            if (loadObj.NeedSave)
                            {
                                BytesToFile(loadObj.WWWObj.bytes, loadObj.FileVO.SaveFilePath);
                            }
                            if (loadObj.completeLoadHandler != null)
                            {
                                loadObj.completeLoadHandler(loadObj);
                            }
                            if (loadObj.WWWObj.url.Split('.')[1] == "unity3d")
                            {
                                loadObj.WWWObj.assetBundle.Unload(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            if (loadObj.NeedSave) {
                                Debug.Log("Save File Error:" + loadObj.WWWObj.url + ",Exception:" + ex.ToString());
                                Log.Print("Save File Error:" + loadObj.WWWObj.url + ",Exception:" + ex.ToString());
                            }
                            else
                            {
                                Debug.Log("Down Success But Callback Error:" + loadObj.WWWObj.url + ",Exception:" + ex.ToString());
                                Log.Print("Down Success But Callback Error:" + loadObj.WWWObj.url + ",Exception:" + ex.ToString());
                            }
                        }
                        finally
                        {
                            loadObj.WWWObj.Dispose();
                            listHttpLoadHelper.RemoveAt(i);
                            currentDownCount--;
                            Caching.CleanCache();
                        }
                        return;
                    }
                }
            }
        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
        private HttpLoadHelper GetHttpLoadHelper(DownFileVO downFileVO, UpdateEventHandler completeLoadHandler, bool isLocal = true, bool needSave = false)
        {
            HttpLoadHelper htttpLoadHelper = new HttpLoadHelper();
            htttpLoadHelper.completeLoadHandler = completeLoadHandler;
            htttpLoadHelper.FileVO = downFileVO;
            htttpLoadHelper.NeedSave = needSave;
            htttpLoadHelper.TimeOut = timeOut;
            htttpLoadHelper.IsLocal = isLocal;
            return htttpLoadHelper;
        }
    }
    public class HttpLoadGameObject : MonoBehaviour
    {
        private float WaitForSecondsTime = 0.001f;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            InvokeRepeating("HttpLoadUpdate", WaitForSecondsTime, WaitForSecondsTime);
        }
        void HttpLoadUpdate()
        {
            UpdateFactory.httpLoad.Update();
        }
    }
}
