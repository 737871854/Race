//
// /**************************************************************************
//
// SceneManager.cs
//
// **************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Need.Mx
{
    public class SceneManager : Singleton<SceneManager>
    {
        #region SceneInfoData class

        public class SceneInfoData
        {
            public Type SceneType {get; private set;}

            public string SceneName {get;private set;}

            public object[] Params {get;private set;}
            
            public SceneInfoData(string _sceneName, Type _sceneType, params object[] _params)
            {
                this.SceneName = _sceneName;
                this.SceneType = _sceneType;
                this.Params = _params;
            }
        }
	    #endregion

        private Dictionary<EnumSceneType, SceneInfoData> dicSceneInfos = null;

        private BaseScene currentScene = new BaseScene();

        public EnumSceneType LastSceneType { get; set; }

        public EnumSceneType ChangeSceneType { get; private set; }

        private EnumUIType sceneOpenUIType = EnumUIType.None;

        private GameObject sceneOpenUIParent = null;

        private object[] sceneOpenUIParams = null;

        public BaseScene CurrentScene
        {
            get { return currentScene; }
            set 
            {
                currentScene = value; 
            }
        }

        public override void Init()
        {
            dicSceneInfos = new Dictionary<EnumSceneType, SceneInfoData>();
        }

        public void RegisterAllScene()
        {
            // 暂时没有StartGame，LoginScene，MainScene等类,故而用null替代
            RegisterScene(EnumSceneType.StartGame, "StartGame", typeof(StartGame), null);
            RegisterScene(EnumSceneType.LoginScene, "LoginScene", typeof(LoginScene), null);
            RegisterScene(EnumSceneType.MovieScene, "MovieScene", typeof(MovieScene), null);
            RegisterScene(EnumSceneType.LoadingScene, "LoadingScene", typeof(LoadingScene), null);
            RegisterScene(EnumSceneType.SettingScene, "SettingScene", typeof(SettingScene), null);
            RegisterScene(EnumSceneType.SelectScene, "SelectScene", typeof(SelectScene), null);
            RegisterScene(EnumSceneType.Town_Scene, "Town_Scene", typeof(Town_Scene), null);
            RegisterScene(EnumSceneType.Sea_Scene, "Sea_Scene", typeof(Sea_Scene), null);
            RegisterScene(EnumSceneType.Hallowmas_Scene, "Hallowmas_Scene", typeof(Hallowmas_Scene), null);
        }

        public void RegisterScene(EnumSceneType _sceneId, string _sceneName, Type _sceneType, params object[] _params)
        {
            if (null == _sceneType || _sceneType.BaseType != typeof(BaseScene))
            {
                throw new SingletonException("Register scene type must nor null and extends BaseScene !");
            }
            if (!dicSceneInfos.ContainsKey(_sceneId))
            {
                SceneInfoData info = new SceneInfoData(_sceneName, _sceneType, _params);
                dicSceneInfos.Add(_sceneId, info);
            }
        }

        public void UnRegisterScene(EnumSceneType _sceneType)
        {
            if (dicSceneInfos.ContainsKey(_sceneType))
            {
                dicSceneInfos.Remove(_sceneType);
            }
        }

        public bool IsRegisterScene(EnumSceneType _sceneId)
        {
            return dicSceneInfos.ContainsKey(_sceneId);
        }

        public BaseScene GetBaseScene(EnumSceneType _sceneType)
        {
            Debug.Log("GetBaseScene sceneId = " + _sceneType.ToString());
            SceneInfoData sceneInfo = GetSceneInfo(_sceneType);
            if (null == sceneInfo || null == sceneInfo.SceneType)
            {
                return null;
            }
            BaseScene scene = System.Activator.CreateInstance(sceneInfo.SceneType) as BaseScene;
            return scene;
        }

        public SceneInfoData GetSceneInfo(EnumSceneType _sceneId)
        {
            if (dicSceneInfos.ContainsKey(_sceneId))
            {
                return dicSceneInfos[_sceneId];
            }
            Debug.LogError("This Scene is not register ! ID: " + _sceneId.ToString());
            return null;
        }

        public string GetSceneName(EnumSceneType _sceneId)
        {
            if (dicSceneInfos.ContainsKey(_sceneId))
            {
                return dicSceneInfos[_sceneId].SceneName;
            }
            Debug.LogError("This Scene is not register ! ID: " + _sceneId.ToString());
            return null;
        }

        public void ClearScene()
        {
            dicSceneInfos.Clear();
        }

        #region Change Scene

        /// <summary>
        /// 打开Scene
        /// </summary>
        /// <param name="_sceneType"></param>
        public void ChangeSceneDirect(EnumSceneType _sceneType)
        {
            UIManager.Instance.CloseUIAll();
            if (CurrentScene != null)
            {
                //CurrentScene.Release();
                CurrentScene = null;
            }


            LastSceneType = ChangeSceneType;
            ChangeSceneType = _sceneType;
            string sceneName = GetSceneName(_sceneType);
            // chanage scene
            CoroutineController.Instance.StartCoroutine(AsyncLoadScene(sceneName));
        }

        /// <summary>
        /// 判断是否打开目标Scene
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <param name="_uiType"></param>
        /// <param name="_params"></param>
        public void ChangeSceneDirect(EnumSceneType _sceneType, EnumUIType _uiType, GameObject _uiParent = null, params object[] _params)
        {
            sceneOpenUIType     = _uiType;
            sceneOpenUIParams   = _params;
            sceneOpenUIParent   = _uiParent;
            if (ChangeSceneType == _sceneType)
            {
                if (sceneOpenUIType == EnumUIType.None)
                {
                    return;
                }
                UIManager.Instance.OpenUI(sceneOpenUIType, sceneOpenUIParent, sceneOpenUIParams);
                sceneOpenUIType = EnumUIType.None;
            }
            else
            {
                ChangeSceneDirect(_sceneType);
            }
        }

        private IEnumerator<AsyncOperation> AsyncLoadScene(string sceneName)
        {
            AsyncOperation oper = Application.LoadLevelAsync(sceneName);
            yield return oper;
            currentScene = ModuleManager.Instance.Get(sceneName) as BaseScene;
            currentScene.InitScene();
            // message send
            if (sceneOpenUIType != EnumUIType.None)
            {
                UIManager.Instance.OpenUI(sceneOpenUIType, sceneOpenUIParent, sceneOpenUIParams);
                sceneOpenUIType = EnumUIType.None;
                sceneOpenUIParent = null;
            }
        }
        #endregion



        /// <summary>
        /// 打开Scene
        /// </summary>
        /// <param name="_sceneType"></param>
        public void ChangeScene(EnumSceneType _sceneType)
        {
            UIManager.Instance.CloseUIAll();
            if (CurrentScene != null)
            {
                //CurrentScene.Release();
                CurrentScene = null;
            }

            LastSceneType = ChangeSceneType;
            ChangeSceneType = _sceneType;
            string sceneName = GetSceneName(_sceneType);
            // chanage loading scene
            CoroutineController.Instance.StartCoroutine(AsyncLoadOtherScene());
        }

        /// <summary>
        /// 判断是否打开目标Scene
        /// </summary>
        /// <param name="_sceneType"></param>
        /// <param name="_uiType"></param>
        /// <param name="_params"></param>
        public void ChangeScene(EnumSceneType _sceneType, EnumUIType _uiType, GameObject _uiParent = null, params object[] _params)
        {
            sceneOpenUIType = _uiType;
            sceneOpenUIParams = _params;
            sceneOpenUIParent = _uiParent;
            if (ChangeSceneType == _sceneType)
            {
                if (sceneOpenUIType == EnumUIType.None)
                {
                    return;
                }
                UIManager.Instance.OpenUI(sceneOpenUIType, sceneOpenUIParent, sceneOpenUIParams);
                sceneOpenUIType = EnumUIType.None;
                sceneOpenUIParent = null;
            }
            else
            {
                ChangeScene(_sceneType);
            }
        }

        private IEnumerator AsyncLoadOtherScene()
        {
            string sceneName = GetSceneName(EnumSceneType.LoadingScene);
            AsyncOperation oper = Application.LoadLevelAsync(sceneName);
            yield return oper;
            // message send
            if (oper.isDone)
            {
                currentScene = ModuleManager.Instance.Get(sceneName) as BaseScene;
                currentScene.InitScene();
                UIManager.Instance.OpenUI(EnumUIType.PanelLoading);
                LoadingBar loadingBar = UIManager.Instance.GetUI<LoadingBar>(EnumUIType.PanelLoading);
                if (!SceneManager.Instance.IsRegisterScene(ChangeSceneType))
                {
                    Debug.LogError("没有注册次场景！ " + ChangeSceneType.ToString());
                }
                loadingBar.Load(ChangeSceneType);
                loadingBar.LoadingComplete += SceneLoadCompleted;
                //GameObject go = UIManager.Instance.GetUIObject
                //GameObject go = GameObject.Find("LoadingScenePanel");
                //LoadingSceneUI loadingSceneUI = go.GetComponent<LoadingSceneUI>();
                //BaseScene scene = CurrentScene;
                //if (null != scene)
                //{
                //    scene.CurrentScendId = ChangeSceneId;
                //}
                //// 检测是否注册该场景
                //if (!SceneManager.Instance.IsRegisterScene(ChangeSceneId))
                //{
                //    Debug.LogError("没有注册次场景！ " + ChangeSceneId.ToString());
                //}
                //loadingSceneUI.Load(ChangeSceneId);
                //loadingSceneUI.LoadCompleted += SceneLoadCompleted;
            }
        }

        void SceneLoadCompleted(object sender, EventArgs e)
        {
            Debug.Log("切换场景完成 + " + sender as string);
            // 切换场景完成
            //MessageCenter.Instance.SendMessage(MessageType.GAMESCENE_CHANGECOMPLETE, this, null, false);

            // 有要打开的UI
            if (sceneOpenUIType != EnumUIType.None)
            {
                UIManager.Instance.OpenUICloseOthers(sceneOpenUIType,null, sceneOpenUIParams);
                sceneOpenUIType = EnumUIType.None;
            }
        }


        //// 加载场景代码 loading
        //private IEnumerator AsynLoadednextScene()
        //{
        //    string sceneName = SceneManager.Instance.GetSceneName(ChangeSceneId);
        //    AsyncOperation oper = Application.LoadLevelAsync(sceneName);
        //    yield return oper;
        //    if (oper.isDone)
        //    {
        //        if (LoadCompleted != null)
        //        {
        //            LoadCompleted(changeSceneId, null);
        //        }
        //    }
        //}


        //这里面的Game就是GameController
        //BaseScene scene = Game.Instance.GetBaseScene(Game.Instance.ChangeSceneId);
        //Game.Instance.CurrentScene = scene; // CurrentScene = scene;
        //scene.Load();
    }
}
