


//
// /**************************************************************************
//
// UIManager.cs
//
// **************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Need.Mx
{
	/// <summary>
	/// User interface manager.
	/// </summary>
	public class UIManager : Singleton<UIManager>
	{
		#region UIInfoData class
		/// <summary>
		/// User interface UIInfoData.
		/// </summary>
		class UIInfoData
		{
			/// <summary>
			/// Gets the type of the user interface.
			/// </summary>
			/// <value>The type of the user interface.</value>
			public EnumUIType UIType { get; private set; }

			public Type ScriptType { get; private set; }
			/// <summary>
			/// Gets the path.
			/// </summary>
			/// <value>The path.</value>
			public string Path { get; private set; }

            public GameObject Parent { get; private set; }

			/// <summary>
			/// Gets the user interface parameters.
			/// </summary>
			/// <value>The user interface parameters.</value>
			public object[] UIParams { get; private set; }

            public UIInfoData(EnumUIType _uiType, string _path, GameObject _parent, params object[] _uiParams)
			{
				this.UIType = _uiType;
				this.Path = _path;
                this.Parent = _parent;
				this.UIParams = _uiParams;
				this.ScriptType = UIPathDefines.GetUIScriptByType(this.UIType);
			}
		}
		#endregion

		/// <summary>
		/// The dic open U is.
		/// </summary>
		private Dictionary<EnumUIType, GameObject> dicOpenUIs = null;

		/// <summary>
		/// The stack open U is.
		/// </summary>
		private Stack<UIInfoData> stackOpenUIs = null;

        // UI相机
        private Camera uiCamera = null;

        private GameObject uiRoot;
        /// <summary>
        /// 获取UIRoot
        /// </summary>
        public GameObject UIRootObj
        {
            get 
            {
                return uiRoot;
            }
            set
            {
                uiRoot = value;
            }
        }

        public Camera UICamera
        {
            get
            {
                if (null != UIRootObj)
                {
                    uiCamera = UIRootObj.transform.Find("UICamera").GetComponent<Camera>();
                }
                return uiCamera;
            }
        }

		/// <summary>
		/// Init this Singleton.
		/// </summary>
		public override void Init ()
		{
			dicOpenUIs = new Dictionary<EnumUIType, GameObject>();
			stackOpenUIs = new Stack<UIInfoData>();

            if (UIRootObj == null)
            {
                UIRootObj = MonoBehaviour.Instantiate(ResManager.Instance.Load(UIPathDefines.GetPrefabPathByType(EnumUIType.UIRoot))) as GameObject;
                GameObject.DontDestroyOnLoad(UIRootObj);
            }
		}

		#region Get UI & UIObject By EnunUIType 
		/// <summary>
		/// Gets the U.
		/// </summary>
		/// <returns>The U.</returns>
		/// <param name="_uiType">_ui type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T GetUI<T>(EnumUIType _uiType) where T : BaseUI
		{
			GameObject _retObj = GetUIObject(_uiType);
			if (_retObj != null)
			{
				return _retObj.GetComponent<T>();
			}
			return null;
		}

		/// <summary>
		/// Gets the user interface object.
		/// </summary>
		/// <returns>The user interface object.</returns>
		/// <param name="_uiType">_ui type.</param>
		public GameObject GetUIObject(EnumUIType _uiType)
		{
			GameObject _retObj = null;
			if (!dicOpenUIs.TryGetValue(_uiType, out _retObj))
				throw new Exception("dicOpenUIs TryGetValue Failure! _uiType :" + _uiType.ToString());
			return _retObj;
		}
		#endregion
		
		#region Preload UI Prefab By EnumUIType
		/// <summary>
		/// Preloads the U.
		/// </summary>
		/// <param name="_uiTypes">_ui types.</param>
		public void PreloadUI(EnumUIType[] _uiTypes)
		{
			for (int i=0; i<_uiTypes.Length; i++)
			{
				PreloadUI(_uiTypes[i]);
			}
		}
		
		/// <summary>
		/// Preloads the U.
		/// </summary>
		/// <param name="_uiType">_ui type.</param>
		public void PreloadUI(EnumUIType _uiType)
		{
			string path = UIPathDefines.GetPrefabPathByType(_uiType);
            //ResManager.Instance.ResourcesLoad(path);
		}
		
		#endregion

		#region Open UI By EnumUIType
		/// <summary>
		/// 打开界面。
		/// </summary>
		/// <param name="uiTypes">User interface types.</param>
		public void OpenUI(EnumUIType[] uiTypes, GameObject parent = null)
		{
            OpenUI(false, uiTypes, parent, null);
		}

		/// <summary>
		/// Opens the U.
		/// </summary>
		/// <param name="uiType">User interface type.</param>
		/// <param name="uiObjParams">User interface object parameters.</param>
		public void OpenUI(EnumUIType uiType, GameObject uiParent = null, params object[] uiObjParams)
		{
			EnumUIType[] uiTypes = new EnumUIType[1];
			uiTypes[0] = uiType;
            OpenUI(false, uiTypes, uiParent, uiObjParams);
		}

		/// <summary>
		/// Opens the user interface close others.
		/// </summary>
		/// <param name="uiTypes">User interface types.</param>
		public void OpenUICloseOthers(EnumUIType[] uiTypes)
		{
			OpenUI(true, uiTypes, null, null);
		}

		/// <summary>
		/// Opens the user interface close others.
		/// </summary>
		/// <param name="uiType">User interface type.</param>
		/// <param name="uiObjParams">User interface object parameters.</param>
        public void OpenUICloseOthers(EnumUIType uiType, GameObject uiParent = null, params object[] uiObjParams)
		{
			EnumUIType[] uiTypes = new EnumUIType[1];
			uiTypes[0] = uiType;
            OpenUI(true, uiTypes, uiParent, uiObjParams);
		}

		/// <summary>
		/// Opens the U.
		/// </summary>
		/// <param name="_isCloseOthers">If set to <c>true</c> _is close others.</param>
		/// <param name="_uiTypes">_ui types.</param>
		/// <param name="_uiParams">_ui parameters.</param>
		private void OpenUI(bool _isCloseOthers, EnumUIType[] _uiTypes, GameObject _uiParent = null, params object[] _uiParams)
		{
			// Close Others UI.
			if (_isCloseOthers)
			{
				CloseUIAll();
			}

			// push _uiTypes in Stack.
			for (int i=0; i<_uiTypes.Length; i++)
			{
				EnumUIType _uiType = _uiTypes[i];
				if (!dicOpenUIs.ContainsKey(_uiType))
				{
					string _path = UIPathDefines.GetPrefabPathByType(_uiType);
                    stackOpenUIs.Push(new UIInfoData(_uiType, _path, _uiParent, _uiParams));
				}
			}

			// Open UI.
			if (stackOpenUIs.Count > 0)
			{
				CoroutineController.Instance.StartCoroutine(AsyncLoadData());
			}
		}

		private IEnumerator<int> AsyncLoadData()
		{
			UIInfoData _uiInfoData = null;
			UnityEngine.Object _prefabObj = null;
			GameObject _uiObject = null;
            GameObject _uiParent = null;

			if (stackOpenUIs != null && stackOpenUIs.Count > 0)
			{
				do 
				{
					_uiInfoData = stackOpenUIs.Pop();
                    _uiParent = _uiInfoData.Parent;
                    _prefabObj = ResManager.Instance.Load(_uiInfoData.Path);
					if (_prefabObj != null)
					{
						//_uiObject = NGUITools.AddChild(Game.Instance.mainUICamera.gameObject, _prefabObj as GameObject);
						_uiObject = MonoBehaviour.Instantiate(_prefabObj) as GameObject;
                        if (null != UIRootObj)
                        {
                            if (null == _uiParent)
                            {
                                _uiObject.transform.SetParent(UIRootObj.transform);
                                _uiObject.transform.localScale = Vector3.one;
                                _uiObject.GetComponent<Canvas>().worldCamera = UICamera;
                            }
                            else
                            {
                                _uiObject.transform.parent = _uiParent.transform;
                                _uiObject.transform.localScale = Vector3.one;
                            }
                        }
                       
						BaseUI _baseUI = _uiObject.GetComponent<BaseUI>();
						if (null == _baseUI)
						{
							_baseUI = _uiObject.AddComponent(_uiInfoData.ScriptType) as BaseUI;
						}
						if (null != _baseUI)
						{
							_baseUI.SetUIWhenOpening(_uiInfoData.UIParams);
						}
						dicOpenUIs.Add(_uiInfoData.UIType, _uiObject);
					}

				} while(stackOpenUIs.Count > 0);
			}
			yield return 0;
		}

		#endregion

		#region Close UI By EnumUIType
		/// <summary>
		/// 关闭界面。
		/// </summary>
		/// <param name="uiType">User interface type.</param>
		public void CloseUI(EnumUIType _uiType)
		{
			GameObject _uiObj = null;
			if (!dicOpenUIs.TryGetValue(_uiType, out _uiObj))
			{
				Debug.Log("dicOpenUIs TryGetValue Failure! _uiType :" + _uiType.ToString());
				return;
			}
			CloseUI(_uiType, _uiObj);
		}

		/// <summary>
		/// Closes the U.
		/// </summary>
		/// <param name="_uiTypes">_ui types.</param>
		public void CloseUI(EnumUIType[] _uiTypes)
		{
			for (int i=0; i<_uiTypes.Length; i++)
			{
				CloseUI(_uiTypes[i]);
			}
		}
		
		/// <summary>
		/// 关闭所有UI界面
		/// </summary>
		public void CloseUIAll()
		{
			List<EnumUIType> _keyList = new List<EnumUIType>(dicOpenUIs.Keys);
			foreach (EnumUIType _uiType in _keyList)
			{
                if (_uiType == EnumUIType.UIRoot)
                {
                    continue;
                }
				GameObject _uiObj = dicOpenUIs[_uiType];
				CloseUI(_uiType, _uiObj);
			}
            dicOpenUIs.Clear();
		}

		private void CloseUI(EnumUIType _uiType, GameObject _uiObj)
		{
			if (_uiObj == null)
			{
				dicOpenUIs.Remove(_uiType);
			}
			else
			{
				BaseUI _baseUI = _uiObj.GetComponent<BaseUI>();
				if (_baseUI != null)
				{
					_baseUI.StateChanged += CloseUIHandler;
					_baseUI.Release();
				}
				else
				{
					GameObject.Destroy(_uiObj);
					dicOpenUIs.Remove(_uiType);
				}
			}
		}

		/// <summary>
		/// Closes the user interface handler.
		/// </summary>
		/// <param name="_sender">_sender.</param>
		/// <param name="_newState">_new state.</param>
		/// <param name="_oldState">_old state.</param>
		private void CloseUIHandler(object _sender, EnumObjectState _newState, EnumObjectState _oldState)
		{
			if (_newState == EnumObjectState.Closing)
			{
				BaseUI _baseUI = _sender as BaseUI;
				dicOpenUIs.Remove(_baseUI.GetUIType());
				_baseUI.StateChanged -= CloseUIHandler;
			}
		}
		#endregion

        #region Hide Or Show UI By EnumUIType
        public void HideUI(EnumUIType _uiType)
        {
            GameObject _uiObj = GetUIObject(_uiType);
            _uiObj.SetActive(false);
        }

        public void ShowUI(EnumUIType _uiType)
        {
            GameObject _uiObj = GetUIObject(_uiType);
            _uiObj.SetActive(true); 
        }
        #endregion
    }
}

