using UnityEngine;
using System.Collections;


namespace Need.Mx
{
    public class SceneLibraryDownloader : MonoBehaviour, IDownloader
    {
        private static GameObject UnityLoadGO;
        private bool isDownling = false;
        void Awake()
        {
            if (UnityLoadGO == null)
            {
                UnityLoadGO = new GameObject("_SceneLibraryLoad");
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
                if (www.isDone)
                {
                   
                    this.isDownling = false;
                    if (www.assetBundle != null)
                    {
                        this.objs = www.assetBundle.LoadAllAssets();
                    }
                    this.data = new LoadedData(this.objs, www.url, helper.OriginalUrl);
                    helper.CompleteHandler(this.data);
                    if (www.assetBundle != null)
                    {
                        www.assetBundle.Unload(false);
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
                        loadHelper.CompleteHandler(this.data);
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
            this.data = null;
            GameObject.Destroy(this.gameObject);
        }
    }
}