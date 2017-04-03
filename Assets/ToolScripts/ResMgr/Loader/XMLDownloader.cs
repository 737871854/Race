using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Update;
using System.Text.RegularExpressions;
using System;

namespace Need.Mx
{
    public delegate object XMLResolverHandler(Dictionary<string, string> rowResolver);
    public class XMLDownloader :MonoBehaviour, IDownloader
    {
        private bool needDecrupt = false;

        void Awake()
        {
            this.needDecrupt = false;
        }
        public void StartDown(LoadHelper loadHelper)
        {
            this.name = loadHelper.Url;
            StartCoroutine("DownXML", loadHelper);
        }
        private void WriteDebug(string error)
        {
            //ThreadManager.DispatchToMainThread(() =>
            //{
                Debug.LogError(error);
            //});
        }
        IEnumerator DownXML(object parm)
        {
            LoadHelper loadHelper = parm as LoadHelper;
            string xmlText = IOHelper.OpenText(loadHelper.Url);
            //if (needDecrupt)
            //{
            //    xmlText = AESManager.AESDecrypt(xmlText);
            //}
            yield return null;
            Regex regex = new Regex("<object .*/>");
            MatchCollection matchCollection = regex.Matches(xmlText);
            string pattern = "([A-Za-z0-9_-]*?=\".*?\"){1}";
            int totalRow = matchCollection.Count;
            yield return null;
            for (int i = 0; i < totalRow; i++)
            {
                MatchCollection mc = Regex.Matches(matchCollection[i].Value, pattern, RegexOptions.IgnoreCase);
                Dictionary<string, string> dic = new Dictionary<string, string>();
                if (mc.Count > 0)
                {
                    for (int j = 0; j < mc.Count; j++)
                    {
                        string str = mc[j].Value.Replace("\"", "");
                        string[] keyValue = str.Split('=');
                        string key = keyValue[0];
                        string value = keyValue[1];
                        if (dic.ContainsKey(key))
                        {
                            string error = string.Format("{0} has exist key:{1},Row index is:{2}", loadHelper.FileName, key, j);
                            Debug.LogError(error);
                        }
                        else
                            dic.Add(key, value);
                    }
                }
                if (loadHelper.XMLResolver != null)
                {
                    try
                    {
                        loadHelper.XMLResolver(dic);

                    }
                    catch (Exception ex)
                    {
                        string info = string.Empty;
                        foreach (KeyValuePair<string, string> keyValue in dic)
                        {
                            info += keyValue.Key + ":" + keyValue.Value + " ";
                        }
                        string error = string.Format("Resolver {0}.xml wrong:row index {1}---{2} exception:{3}", loadHelper.FileName, i, info, ex.ToString());
                        Debug.LogError(error);
                    }

                }
                else
                {
                    Debug.LogError("Resolver" + loadHelper.FileName + ".xml error:XMLResolver is null");
                }
            }
            yield return null;
            if (loadHelper.CompleteHandler != null)
            {
                loadHelper.CompleteHandler(new LoadedData(xmlText, loadHelper.Url, loadHelper.OriginalUrl));
                GameObject.Destroy(this.gameObject);

            }
        }


        public void Clear()
        {
          
        }
    }
}