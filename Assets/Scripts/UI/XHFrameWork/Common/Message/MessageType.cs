//
// /**************************************************************************
//
// MessageType.cs
//
//
// **************************************************************************/

using System;
namespace Need.Mx
{
	public class MessageType 
	{
		public static string Net_MessageTestOne = "Net_MessageTestOne";
		public static string Net_MessageTestTwo = "Net_MessageTestTwo";

        #region ------------------键盘按键替代Button----------------------------
        /// <summary>
        /// 开始游戏
        /// </summary>
        public static string Message_Key_Game_Start = "Message_Key_Game_Start";
        /// <summary>
        /// 投币
        /// </summary>
        public static string Message_Refresh_Check_Coin     = "Message_Refresh_Check_Coin";
        public static string Message_Key_Game_Left          = "Message_Key_Game_Left";
        public static string Message_Key_Game_Right         = "Message_Key_Game_Right";
        #endregion

        #region ------------------下位机反馈消息--------------------------------

        #endregion

        #region -----------------------场景消息-----------------------
        public static string Message_Level_Loaded = "Message_Level_Loaded";
        #endregion

        /// <summary>
        /// 每帧刷新
        /// </summary>
        public static string Message_Update_Pre_Frame = "Message_Update_Pre_Frame";
        /// <summary>
        /// 固定时间刷新
        /// </summary>
        public static string Message_Update_Fix_Frame = "Message_Update_Fix_Frame";
        /// <summary>
        /// 投币后更新界面玩家币数
        /// </summary>
        public static string Message_Refresh_Coin = "Message_Refresh_Coin";
        /// <summary>
        /// 界面打开事件
        /// </summary>
        public static string Message_View_Is_Open = "Message_View_Is_Open";

        public static string Message_Refresh_Score  = "Message_Refresh_Score";
    
        /// <summary>
        /// 后台A按钮
        /// </summary>
        public static string EVNET_SETTING_CONFIRM = "EVNET_SETTING_CONFIRM";
        /// <summary>
        /// 后台B按钮
        /// </summary>
        public static string EVNET_SETTING_SELECT = "EVNET_SETTING_SELECT";
        /// <summary>
        /// 拍照
        /// </summary>
        public static string Message_Take_Picture = "Message_Take_Picture";

        // 游戏地图选择 ---后期考虑做消息合并    --TODO
        public static string Message_Map_Is_Choosed = "Message_Map_Is_Choosed";
        public static string Message_Select_Map = "Message_Select_Map";

        // Boss来袭
        public static string Message_Boss_Come = "Message_Boss_Come";

        // 变豹子
        public static string Message_Magic = "Message_Magic";

        // 加速
        public static string Message_Speed_Accle = "Message_Speed_Accle";

        // 完美躲避
        public static string Message_Avoid_Landmine_Typhoon = "Message_Avoid_Landmine_Typhoon";

        // 连击
        public static string Message_Combo = "Message_Combo";

        public static string Message_OverTake = "Message_OverTake";

        // 吃道具
        public static string Message_Trigger_Tips = "Message_Trigger_Tips";

        // 加分
        public static string Message_Show_Score = "Message_Show_Score";

        //续币倒计时
        public static string Message_Mode_Continue = "Message_Mode_Continue";

        // 死亡
        public static string Message_Mode_Dead = "Message_Mode_Dead";

        // 进入观察模式
        public static string Message_Change_Observe = "Message_Change_Observe";

        // 加时
        public static string Message_Add_Time = "Message_Add_Time";

        // boss受到伤害
        public static string Message_Boss_On_Damage = "Message_Boss_On_Damage";

        // 选车操作
        public static string Message_Car_Selected = "Message_Car_Selected";

        // 游戏结束
        public static string Message_Game_End = "Message_Game_End";

        // 显示更随车
        public static string Message_Show_Follow_Card = "Message_Show_Follow_Card";
        // 相机震动
        public static string Message_Camera_Shake = "Message_Camera_Shake";
	}
}

