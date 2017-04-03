using UnityEngine;
using System.Collections;

namespace Need.Mx
{
    public class ImgDownloader : MonoBehaviour, IDownloader
    {
        public void StartDown(LoadHelper loadHelper)
        {
            this.name = loadHelper.OriginalUrl;
            this.StartCoroutine("DownAsset", loadHelper);
        }

        IEnumerator DownAsset(object parm)
        {
            LoadHelper helper = parm as LoadHelper;
            WWW www = new WWW("file://" + helper.Url);
            while (true)
            {
                if (www.error!= null)
                {
                    Debug.LogError("ImgDownloader DownAsset error:" + helper.Url + www.error);
                    break;
                }
                if (www.isDone)
                {
                    LoadedData data = new LoadedData(www.texture, www.url, helper.OriginalUrl);
                    helper.CompleteHandler(data);
                    if (www != null)
                    {
                        www.Dispose();
                        GameObject.Destroy(this.gameObject);
                    }
                    break;
                }
                else
                {
                    yield return www;
                }
            }
        }


        public void Clear()
        {
            
        }
    }
}