using System;
using UnityEngine;
using System.Collections.Generic;

namespace Need.Mx
{
    public class GameConfig
    {
        #region---------------------------配置文件路径常量------------------------------
        public const string SETTING_COINFIG         = "Json/SettingConfig.json";
        #endregion

        #region---------------------------内部定义游戏常量------------------------------
        public static bool    GAME_CONFIG_MODE_TEST                 = false;
        public static bool    DebugMode                             = true;
        public static int     GAME_CONFIG_MAX_LIFE_TIME             = 180;                      // 游戏时间
        public static int     GAME_CONFIG_PER_USE_COIN              = 3;                        // 每次币数
        public static int     GAME_CONFIG_PLAYER_COUNT              = 1;                        // 玩家个数
        public static int     GAME_CONFIG_TICKET_MODLE              = 0;                        // 出票模式
        public static int     GAME_CONFIG_LANGUAGE                  = 0;                        // 游戏语言
        public static string  GAME_CONFIG_CHECK_ID                  = "GAME_CONFIG_CHECK_ID";   // 协议校验ID
        public static int     GAME_CONFIG_MULTI_PLAYER_COUNT        = 4;                        // 联机允许最大人数
        public static float GAME_CONFIG_WAITTING_TIME               = 30;                       // 待机等待
        public static float   ToKmh                                 = 3.6f;
        public static float   ToMetersPerSecs                       = 1.0f / ToKmh;
        public static bool    GAME_COINFIG_TEST                     = false;
        public static float[] GAME_COINFIG_LANES                    = { -3.7f, -1.90f, 0, 1.90f, 3.6f };
        public static int[] GAME_CONIFIG_CHECK_POINT                = { 2800, 5000, 6000};
        public static int[]   GAME_CONFIG_CHECK_TIME                = { 50, 30, 20 };
        public static int[] GAME_CONFIG_STEP_TIME                   = { 80, 130, 160};
        public static float GAME_CONFIG_CONTINUE_TIME               = 10;
        public static float GAME_CONFIG_OBSERVE_TIME                = 6;
        public static float GAME_CONFIG_READY_TIME                  = 15;
        #endregion

        /// <summary>
        /// 初始化函数
        /// </summary>
        public static void ParsingGameConfig()
        {
            SoundController.Instance.ResetVolumeScale();

            GAME_CONFIG_PER_USE_COIN    = SettingManager.Instance.GameRate;
        }
    }
}