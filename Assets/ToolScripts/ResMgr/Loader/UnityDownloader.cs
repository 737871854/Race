using UnityEngine;
using System.Collections;

namespace Need.Mx
{
    public class UnityDownloader : MonoBehaviour, IDownloader
    {
        private static GameObject UnityLoadGO;
        private bool isDownling = false;
        void Awake()
        {
            if (UnityLoadGO == null)
            {
                UnityLoadGO = new GameObject("_UnityLoad");
                GameObject.DontDestroyOnLoad(UnityLoadGO);
            }
            this.transform.SetParent(UnityLoadGO.transform);
            DontDestroyOnLoad(this);
        }
        private LoadedData data = null;
        private Object[] objs;
        public void StartDown(LoadHelper loadHelper)
        {
            if (this.data == null && !this.isDownling)
            {
                this.name = loadHelper.OriginalUrl;
                this.StartCoroutine("DownAsset", loadHelper);
            }
            else
            {
                this.StartCoroutine("ReDown", loadHelper);
            }
        }

        IEnumerator DownAsset(object parm)
        {
            this.isDownling = true;
            LoadHelper helper = parm as LoadHelper;
            WWW www = new WWW("file://" + helper.Url);
            while (true)
            {
                if (www.error != null)
                {
                    Debug.LogError("UnityDownloader DownAsset error:" + helper.Url +www.error);
                    break;
                }
                if (www.isDone)
                {
                    try
                    {
                        this.data = new LoadedData(www, www.url, helper.OriginalUrl);
                        this.isDownling = false;
                        if (helper.IsPublicRes && www.assetBundle != null)
                        {
                            this.objs = www.assetBundle.LoadAllAssets();
                            //if(GlobalDef.IsDev)
                            //{
                            //    Debug.Log("Load Asset:" + helper.Url);
                            //    foreach (object temp in objs)
                            //    {
                            //        Debug.Log("object in Asset in LoadedData:" + temp.ToString());
                            //    }
                            //    Debug.Log("AssetBundle End.");
                            //}
                        }
                        helper.CompleteHandler(this.data);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("加载资源" + helper.Url + "错误:" + ex.ToString());
                    }
                    finally
                    {
                        //if (!helper.IsPublicRes)
                        //{
                        //    if (www.assetBundle != null)
                        //    {
                        //        www.assetBundle.Unload(false);
                        //    }
                        //    www.Dispose();
                        //    GameObject.Destroy(this.gameObject);
                        //}
                    }
                    break;
                }
                else
                {
                    yield return www;
                }
            }
        }
        private IEnumerator ReDown(LoadHelper loadHelper)
        {
            if (this.data == null)
            {
                while (true)
                {
                    if (this.data != null)
                    {
                        try
                        {
                            loadHelper.CompleteHandler(this.data);
                        }
                        catch (System.Exception ex)
                        {
                            LoadedData loadedData = this.data as LoadedData;
                            Debug.LogError("加载资源" + loadedData.FilePath + "错误:" + ex.ToString());
                        }
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                loadHelper.CompleteHandler(data);
            }
        }


        public void Clear()
        {
            StartCoroutine("StartClear");
        }
        IEnumerator StartClear()
        {
            if (this.data == null && this.isDownling)
            {
                while (true)
                {
                    if (this.data != null)
                    {
                        ReallyClear();
                        break;
                    }
                    else
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                ReallyClear();
            }
        }
        void ReallyClear()
        {
            WWW www = this.data.Value as WWW;
            if (www != null && www.assetBundle != null)
            {
                www.assetBundle.Unload(true);
                www.Dispose();
                this.data = null;
            }
            GameObject.Destroy(this.gameObject);
        }
    }
}
