using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AtlasMaker : Editor
{
    [MenuItem("MyTools/UI Art To Prefab")]
    static private void MakeAtlas()
    {
        string spriteDir = Application.dataPath + "/Resources/Prefabs/Sprite";
        if (Directory.Exists(spriteDir))
            Directory.Delete(spriteDir, true);

        Directory.CreateDirectory(spriteDir);

        string uiDir = Application.dataPath + "/Art/UI/RawUI";
        if (!Directory.Exists(uiDir))
        {
            Directory.CreateDirectory(uiDir); ;
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(uiDir);
        foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
        {
            GameObject go = new GameObject(dirInfo.Name);
            PrefabComponent pc = go.AddComponent<PrefabComponent>();
            FileInfo[] files = dirInfo.GetFiles("*png", SearchOption.AllDirectories);
            UIPrefab[] uiPrefab = new UIPrefab[files.Length];
            string allPath = "";
            int count = 0;
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                UIPrefab uiP = new UIPrefab();
                allPath = pngFile.FullName;
				string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                uiP.name = sprite.name;
                uiP.sprite = sprite;
                uiPrefab[count++] = uiP;
            }
            pc.uiPrefab = uiPrefab;
            allPath = spriteDir + "/" + go.name + ".prefab";
            string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));
            PrefabUtility.CreatePrefab(prefabPath, go);
            GameObject.DestroyImmediate(go);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/UI Prefab/Windows")]
	static private void BuildAssetBundleWIN()
	{
        string dir = Application.dataPath + "/StreamingAssets/Res/WIN/UI/";
		if(!Directory.Exists(dir)){
			Directory.CreateDirectory(dir);
		}
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UI");
        foreach (FileInfo file in rootDirInfo.GetFiles(".prefab", SearchOption.AllDirectories))
        {
            List<Object> asset = new List<Object>();
            string path = dir +"/"+file.Name+".assetbundle";
            string filePath = file.FullName;
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            asset.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
            if (BuildPipeline.BuildAssetBundle(null, asset.ToArray(), path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows)) 
            {
                Debug.Log("Building Sucessfully!");
            }
        }

        AssetDatabase.Refresh();
	}

    [MenuItem("MyTools/Assetbundle/UI Prefab/Android")]
    static private void BuildAssetBundleAndroid()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/Android/UI/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UI");
        foreach (FileInfo file in rootDirInfo.GetFiles(".prefab", SearchOption.AllDirectories))
        {
            List<Object> asset = new List<Object>();
            string path = dir + "/" + file.Name + ".assetbundle";
            string filePath = file.FullName;
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            asset.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
            if (BuildPipeline.BuildAssetBundle(null, asset.ToArray(), path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/UI Prefab/IOS")]
    static private void BuildAssetBundleIOS()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/IOS/UI/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UI");
        foreach (FileInfo file in rootDirInfo.GetFiles(".prefab", SearchOption.AllDirectories))
        {
            List<Object> asset = new List<Object>();
            string path = dir + "/" + file.Name + ".assetbundle";
            string filePath = file.FullName;
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            asset.Add(AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object)));
            if (BuildPipeline.BuildAssetBundle(null, asset.ToArray(), path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }

        AssetDatabase.Refresh();
    }

	static private BuildTarget GetBuildTarget()
	{
		BuildTarget target = BuildTarget.WebPlayer;
		#if UNITY_STANDALONE
		target = BuildTarget.StandaloneWindows;
		#elif UNITY_IPHONE
		target = BuildTarget.iPhone;
		#elif UNITY_ANDROID
		target = BuildTarget.Android;
		#endif
		return target;
	}

    [MenuItem("MyTools/Assetbundle/Modle Prefab/Windows")]
    static private void BuildAssetBundleModleWIN()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/WIN/Modle/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Modle");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/Modle Prefab/Android")]
    static private void BuildAssetBundleModleAndroid()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/Android/Modle/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Modle");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/Modle Prefab/IOS")]
    static private void BuildAssetBundleModleIOS()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/IOS/Modle/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Modle");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/UIEffect Prefab/Windows")]
    static private void BuildAssetBundleUIEffectWIN()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/WIN/UIEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UIEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/UIEffect Prefab/Android")]
    static private void BuildAssetBundleUIEffectAndroid()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/Android/UIEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UIEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/UIEffect Prefab/IOS")]
    static private void BuildAssetBundleUIEffectIOS()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/IOS/UIEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/UIEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/Sprite Prefab/Windows")]
    static private void BuildAssetBundleSpriteWIN()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/WIN/Sprite/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Sprite");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/Sprite Prefab/Android")]
    static private void BuildAssetBundleSpriteAndroid()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/Android/Sprite/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Sprite");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/Sprite Prefab/IOS")]
    static private void BuildAssetBundleSpriteIOS()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/IOS/Sprite/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/Sprite");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }


    [MenuItem("MyTools/Assetbundle/SceneEffect Prefab/Windows")]
    static private void BuildAssetBundleSceneEffectWIN()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/WIN/SceneEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/SceneEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/SceneEffect Prefab/Android")]
    static private void BuildAssetBundleSceneEffectAndroid()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/Android/SceneEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/SceneEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/Assetbundle/SceneEffect Prefab/IOS")]
    static private void BuildAssetBundleSceneEffectIOS()
    {
        string dir = Application.dataPath + "/StreamingAssets/Res/IOS/SceneEffect/";
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Resources/Prefabs/SceneEffect");
        foreach (FileInfo file in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            string filePath = file.FullName;
            if (filePath.Contains(".meta"))
            {
                continue;
            }
            string path = dir + "/" + file.Name + ".assetbundle";
            string assetPath = filePath.Substring(filePath.IndexOf("Assets"));
            if (BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(assetPath), null, path, BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows))
            {
                Debug.Log("Building Sucessfully!");
            }
        }
        AssetDatabase.Refresh();
    }
}
