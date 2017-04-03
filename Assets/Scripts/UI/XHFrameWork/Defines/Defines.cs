//
// /**************************************************************************
//
// Defines.cs
//
//
// **************************************************************************/


using System;
using UnityEngine;
using System.Collections;

namespace Need.Mx
{

	#region Global delegate 委托
	public delegate void StateChangedEvent (object sender,EnumObjectState newState,EnumObjectState oldState);
	
	public delegate void MessageEvent(Message message);

	public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);

	public delegate void PropertyChangedHandle(BaseActor actor, int id, object oldValue, object newValue);

    public delegate void LoadingCompletedHandle(object sender, EventArgs e);
	#endregion

	#region Global enum 枚举
	/// <summary>
	/// 对象当前状态 
	/// </summary>
	public enum EnumObjectState
	{
		/// <summary>
		/// The none.
		/// </summary>
		None,
		/// <summary>
		/// The initial.
		/// </summary>
		Initial,
		/// <summary>
		/// The loading.
		/// </summary>
		Loading,
		/// <summary>
		/// The ready.
		/// </summary>
		Ready,
		/// <summary>
		/// The disabled.
		/// </summary>
		Disabled,
		/// <summary>
		/// The closing.
		/// </summary>
		Closing
	}

	/// <summary>
	/// Enum user interface type.
	/// UI面板类型
	/// </summary>
	public enum EnumUIType : int
	{
		/// <summary>
		/// The none.
		/// </summary>
		None = -1,
        /// <summary>
        /// UI Root
        /// </summary>
        UIRoot,
        /// <summary>
        /// 开始界面
        /// </summary>
        PanelStart,
        /// <summary>
        /// 选择界面
        /// </summary>
        PanelSelect,
        /// <summary>
        /// 进度条界面
        /// </summary>
        PanelLoading,
        /// <summary>
        /// 播放视频界面
        /// </summary>
        PanelMain,
        /// <summary>
        /// 结算
        /// </summary>
        PanelEnd,
        /// <summary>
        /// 
        /// </summary>
        PanelSetting,
        /// <summary>
        /// 
        /// </summary>
        PanelOverLay,
        /// <summary>
        /// 
        /// </summary>
        PanelMovie,

        PanelTemp,
     
	}

    public enum EnumSpriteType : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = -1,
        /// <summary>
        /// 进度条数字
        /// </summary>
        //SpriteLoadNum,
        /// <summary>
        /// 结算数字
        /// </summary>
        SpriteEndNum,
        /// <summary>
        /// 计时
        /// </summary>
        SpriteTimeNum,
        /// <summary>
        /// 投币数字
        /// </summary>
        SpriteCoinNum,
        /// <summary>
        /// 速度
        /// </summary>
        SpriteSpeed,
        /// <summary>
        /// 加速倍数
        /// </summary>
        SpriteSpeedScale,
        /// <summary>
        /// 行驶距离
        /// </summary>
        SpriteDistance,
        /// <summary>
        /// 击飞次数
        /// </summary>
        SpriteCombo,
        /// <summary>
        /// 倒计时开始
        /// </summary>
        SpriteCountDown,
        /// <summary>
        /// 道具图标
        /// </summary>
        SpriteProp,
        /// <summary>
        /// 倒计时结束
        /// </summary>
        //SpriteTimeOver,
        /// <summary>
        /// 结算计数
        /// </summary>
        SpriteAccount,
        /// <summary>
        /// 结算排名
        /// </summary>
        SpriteRankEnd,
        ///// <summary>
        ///// 计算距离数字
        ///// </summary>
        //SpriteDistanceEnd,
        /// <summary>
        /// 游戏中排名
        /// </summary>
        SpriteRank,
        /// <summary>
        /// 游戏中序号
        /// </summary>
        SpriteOrder,
        /// <summary>
        /// 选车倒计时
        /// </summary>
        //SpriteSelectTime,
        /// <summary>
        /// 加时UI
        /// </summary>
        SpriteAddTime,
        /// <summary>
        /// 多人结算
        /// </summary>
        SpriteEnd,
        /// <summary>
        /// 还需投多少币
        /// </summary>
        SpritePlease,
        /// <summary>
        /// 
        /// </summary>
        SpriteHead0,
        /// <summary>
        /// 
        /// </summary>
        SpriteHead1,

        SpriteGreat,
    }

    public enum EnumModel :int
    {
        /// <summary>
        /// 
        /// </summary>
        None = -1,
        /// <summary>
        /// 
        /// </summary>
        Player0,
        /// <summary>
        /// 
        /// </summary>
        Player1,
        /// <summary>
        /// 
        /// </summary>
        Player2,
        /// <summary>
        /// 
        /// </summary>
        ShanDong,
    }

	public enum EnumTouchEventType
	{
		OnClick,
		OnDoubleClick,
		OnDown,
		OnUp,
		OnEnter,
		OnExit,
		OnSelect,  
		OnUpdateSelect,  
		OnDeSelect, 
		OnDrag, 
		OnDragEnd,
		OnDrop,
		OnScroll, 
		OnMove,
	}

	public enum EnumPropertyType : int
	{
		RoleName = 1, // 角色名
		Sex,     // 性别
		RoleID,  // Role ID
		Gold,    // 宝石(元宝)
		Coin,    // 金币(铜板)
		Level,   // 等级
		Exp,     // 当前经验

		AttackSpeed,//攻击速度
		HP,     //当前HP
		HPMax,  //生命最大值
		Attack, //普通攻击（点数）
		Water,  //水系攻击（点数）
		Fire,   //火系攻击（点数）
	}

	public enum EnumActorType
	{
		None = 0,
		Role,
		Monster,
		NPC,
	}

    /// <summary>
    /// 场景枚举
    /// </summary>
    public enum EnumSceneType
    {
        None = 0,
        StartGame,
        LoadingScene,
        LoginScene,
        Town_Scene,
        Sea_Scene,
        Hallowmas_Scene,
        MovieScene,
        SettingScene,
        SelectScene,
    }
	

    public enum EnumMovieType : int
    {
        None = -1,
        /// <summary>
        /// 欢迎
        /// </summary>
        Welcome,
        /// <summary>
        /// 待机
        /// </summary>
        Waitting0,

        Waitting1,

        Waitting2,
    }
	#endregion

	#region Defines static class & cosnt
    /// <summary>
    /// 
    /// </summary>
    public static class EnvirPathDefines
    {
        public const string ENVIR_PREFAB = "Prefabs/Envir/";

        public static string GetEnvirPathByTpye(EnvirType _envirType)
        {
            return ENVIR_PREFAB + _envirType.ToString();
        }
    }

	/// <summary>
	/// 路径定义。
	/// </summary>
	public static class UIPathDefines
	{
		/// <summary>
		/// UI预设。
		/// </summary>
        public const string UI_PREFAB = "Prefabs/UI/";
      
		/// <summary>
		/// Gets the type of the prefab path by.
		/// </summary>
		/// <returns>The prefab path by type.</returns>
		/// <param name="_uiType">_ui type.</param>
		public static string GetPrefabPathByType(EnumUIType _uiType)
		{
			string _path = string.Empty;
			switch (_uiType)
			{
                case EnumUIType.UIRoot:
                    _path = UI_PREFAB + "UIRoot";
                    break;
                // UI
                case EnumUIType.PanelStart:
                    _path = UI_PREFAB + "PanelStart";
                    break;
                case EnumUIType.PanelLoading:
                    _path = UI_PREFAB + "PanelLoading";
                    break;
                case EnumUIType.PanelMain:
                    _path = UI_PREFAB + "PanelMain";
                    break;
                case EnumUIType.PanelEnd:
                    _path = UI_PREFAB + "PanelEnd";
                    break;
                case EnumUIType.PanelSetting:
                    _path = UI_PREFAB + "PanelSetting";
                    break;
                case EnumUIType.PanelSelect:
                    _path = UI_PREFAB + "PanelSelect";
                    break;
                case EnumUIType.PanelTemp:
                    _path = UI_PREFAB + "PanelTemp";
                    break;
                case EnumUIType.PanelOverLay:
                    _path = UI_PREFAB + "PanelOverLay";
                    break;
                case EnumUIType.PanelMovie:
                    _path = UI_PREFAB + "PanelMovie";
                    break;
			    default:
				    Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
				    break;
			 }
			return _path;
		}

		/// <summary>
		/// Gets the type of the user interface script by.
		/// </summary>
		/// <returns>The user interface script by type.</returns>
		/// <param name="_uiType">_ui type.</param>
		public static System.Type GetUIScriptByType(EnumUIType _uiType)
		{
			System.Type _scriptType = null;
			switch (_uiType)
			{
                case EnumUIType.PanelStart:
                    _scriptType = typeof(LoginLogic);
                    break;
                case EnumUIType.PanelLoading:
                    _scriptType = typeof(LoadingBar);
                    break;
                case EnumUIType.PanelMain:
                    _scriptType = typeof(PlayLogic);
                    break;
                case EnumUIType.PanelEnd:
                    _scriptType = typeof(EndLogic);
                    break;
                case EnumUIType.PanelSetting:
                    _scriptType = typeof(SettingLogic);
                    break;
                case EnumUIType.PanelSelect:
                    _scriptType = typeof(SelectLogic);
                    break;
                case EnumUIType.PanelMovie:
                    _scriptType = typeof(MovieLogic);
                    break;
			    default:
				    Debug.Log("Not Find EnumUIType! type: " + _uiType.ToString());
				    break;
			}
			return _scriptType;
		}

	}

    public static class SpritePathDefine
    {
        /// <summary>
        /// 精灵预设
        /// </summary>
        public const string SPRITE_PREFAB = "Prefabs/Sprite/";

        public static string GetPrefabPathByType(EnumSpriteType _spriteType)
        {
            string _path = string.Empty;
            switch (_spriteType)
            {
                // 精灵
                case EnumSpriteType.SpriteTimeNum:
                    _path = SPRITE_PREFAB + "SpriteTime";
                    break;
                case EnumSpriteType.SpriteCoinNum:
                    _path = SPRITE_PREFAB + "SpriteCoinNum";
                    break;
                case EnumSpriteType.SpriteCombo:
                    _path = SPRITE_PREFAB + "SpriteCombo";
                    break;
                case EnumSpriteType.SpriteDistance:
                    _path = SPRITE_PREFAB + "SpriteDistance";
                    break;
                //case EnumSpriteType.SpriteLoadNum:
                //    _path = SPRITE_PREFAB + "SpriteLoadNum";
                //    break;
                case EnumSpriteType.SpriteSpeedScale:
                    _path = SPRITE_PREFAB + "SpriteSpeedScale";
                    break;
                case EnumSpriteType.SpriteSpeed:
                    _path = SPRITE_PREFAB + "SpriteSpeed";
                    break;
                case EnumSpriteType.SpriteCountDown:
                    _path = SPRITE_PREFAB + "SpriteCountDown";
                    break;
                case EnumSpriteType.SpriteProp:
                    _path = SPRITE_PREFAB + "SpriteProp";
                    break;
                //case EnumSpriteType.SpriteTimeOver:
                //    _path = SPRITE_PREFAB + "SpriteTimeOver";
                //    break;
                case EnumSpriteType.SpriteAccount:
                    _path = SPRITE_PREFAB + "SpriteAccount";
                    break;
                //case EnumSpriteType.SpriteDistanceEnd:
                //    _path = SPRITE_PREFAB + "SpriteDistanceEnd";
                //    break;
                case EnumSpriteType.SpriteRankEnd:
                    _path = SPRITE_PREFAB + "SpriteRankEnd";
                    break;
                case EnumSpriteType.SpriteRank:
                    _path = SPRITE_PREFAB + "SpriteRank";
                    break;
                case EnumSpriteType.SpriteOrder:
                    _path = SPRITE_PREFAB+ "SpriteOrder";
                    break;
                //case EnumSpriteType.SpriteSelectTime:
                //    _path = SPRITE_PREFAB + "SpriteSelectTime";
                //    break;
                case EnumSpriteType.SpriteEnd:
                    _path = SPRITE_PREFAB + "SpriteEnd";
                    break;
                case EnumSpriteType.SpritePlease:
                    _path = SPRITE_PREFAB + "SpritePlease";
                    break;
                case EnumSpriteType.SpriteAddTime:
                    _path = SPRITE_PREFAB + "SpriteAddTime";
                    break;
                case EnumSpriteType.SpriteHead0:
                    _path = SPRITE_PREFAB + "SpriteHead0";
                    break;
                case EnumSpriteType.SpriteHead1:
                    _path = SPRITE_PREFAB + "SpriteHead1";
                    break;
                case EnumSpriteType.SpriteGreat:
                    _path = SPRITE_PREFAB + "SpriteGreat";
                    break;
                default:
                    Debug.Log("Not Find EnumUIType! type: " + _spriteType.ToString());
                    break;
            }
            return _path;
        }
    }

    public static class MoviePathDefine
    {
        public const string MOVIE_OBJ = "Movie/";

        public static string GetMoviePathByType(EnumMovieType _movieType)
        {
            string _path = string.Empty;
            switch(_movieType)
            {
                case EnumMovieType.Welcome:
                    {
                        _path = MOVIE_OBJ + "GameStart";
                        break;
                    }
                case EnumMovieType.Waitting0:
                    {
                        _path = MOVIE_OBJ + "Waitting0";
                        break;
                    }
                case EnumMovieType.Waitting1:
                    {
                        _path = MOVIE_OBJ + "Waitting1";
                        break;
                    }
                case EnumMovieType.Waitting2:
                    {
                        _path = MOVIE_OBJ + "Waitting2";
                        break;
                    }
                default:
                    Debug.Log("Not Find EnumMovieType! type: " + _movieType.ToString());
                    break;
            }
            return _path;
        }
    }


	#endregion
	//public delegate void OnTouchEventHandle(EventTriggerListener _listener, object _args, params object[] _params);
	

	public class Defines : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}
