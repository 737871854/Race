using UnityEngine;
using System.Collections;
using System.IO;
using Update;


namespace Need.Mx
{
    public class JsonDownloader : IDownloader
    {
        private bool needDecrupt = false;
        public JsonDownloader()
        {
            this.needDecrupt = false;
        }
        public void StartDown(LoadHelper loadHelper)
        {
            string xmlText = IOHelper.OpenText(loadHelper.Url);
            //if (needDecrupt)
            //{
            //    xmlText = AESManager.AESDecrypt(xmlText);
            //}
            if (loadHelper.CompleteHandler != null)
            {
                loadHelper.CompleteHandler(new LoadedData(xmlText, loadHelper.Url, loadHelper.OriginalUrl));
            }
        }

        public void Clear()
        {
           
        }
    }
}