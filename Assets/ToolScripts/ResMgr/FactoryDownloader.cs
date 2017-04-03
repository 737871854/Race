using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Need.Mx
{
    public static class FactoryDownloader
    {
        private static Dictionary<string, IDownloader> dicDownloader = new Dictionary<string, IDownloader>();
        private static Dictionary<string, IDownloader> dicSlDownloader = new Dictionary<string, IDownloader>();
     
        public static void RemoveUnusedLoader(LoadHelper helper)
        {
            if (dicDownloader.ContainsKey(helper.OriginalUrl))
            {
                dicDownloader.Remove(helper.OriginalUrl);
            }
        }
        public static IDownloader GetDownloader(LoadHelper helper)
        {
            IDownloader loader = null;
            if (dicDownloader.ContainsKey(helper.OriginalUrl))
            {
                loader = dicDownloader[helper.OriginalUrl];
            }
            else if (dicSlDownloader.ContainsKey(helper.OriginalUrl))
            {
                loader = dicSlDownloader[helper.OriginalUrl];
            }
            else
            {
                GameObject go = null;
                switch (helper.ExtensionName)
                {
                    case ".xml":
                        go = new GameObject();
                        Object.DontDestroyOnLoad(go);
                        loader = go.AddComponent<XMLDownloader>();
                        break;
                    case ".json":
                        loader = new JsonDownloader();
                        break;
                    case ".txt":
                        loader = new TxtDownloader();
                        break;
                    case ".png":
                    case ".jpg":
                        go = new GameObject("imgLoadGO");
                        Object.DontDestroyOnLoad(go);
                        loader = go.AddComponent<ImgDownloader>();
                        break;
                    case ".sl":
                        go = new GameObject();
                        loader = go.AddComponent<SceneLibraryDownloader>();
                        if (!dicSlDownloader.ContainsKey(helper.OriginalUrl))
                        {
                            dicSlDownloader.Add(helper.OriginalUrl, loader);
                        }
                        break;
                    default:
                        go = new GameObject();
                        loader = go.AddComponent<UnityDownloader>();
                        if (!dicDownloader.ContainsKey(helper.OriginalUrl))
                        {
                            dicDownloader.Add(helper.OriginalUrl, loader);
                        }
                        break;
                }
            }
            return loader;
        }
        public static void Clear(bool useGC=true)
        {
            foreach (IDownloader loader in dicDownloader.Values)
            {
                loader.Clear();
            }
            dicDownloader.Clear();
            if (useGC)
            {
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }
        }
       
        public static void ClearSceneLibrary()
        {
            foreach (IDownloader loader in dicSlDownloader.Values)
            {
                loader.Clear();
            }
            dicSlDownloader.Clear();
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}