/**
 * Copyright (c) 2014,Need Corp. ltd
 * All rights reserved.
 * 
 * 文件名称：Util.cs
 * 简    述：通用快捷函数的集合，如果只在某些特殊功能中使用就不要放在这里了。
 * 特别是具体功能，比如获得指定VO什么的，这个文件要求可以直接在其他游戏工程中使用。
 * 创建标识：Terry.2013/2/26
 */
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;

namespace Need.Mx
{
    /// <summary>
    /// 工具.
    /// </summary>
    public class Util
    {
        #region 颜色处理
        /// <summary>
        /// 颜色转换方便函数，由1~255的颜色分量int值，转换为Color结构。
        /// </summary>
        /// <returns>转换后的颜色</returns>
        public static Color Value2Color(int alpha, int red, int green, int blue)
        {
            float a = alpha / 255.0f;
            float r = red / 255.0f;
            float g = green / 255.0f;
            float b = blue / 255.0f;
            if (a > 1) { a = 1; } else if (a < 0) { a = 0; }
            if (r > 1) { r = 1; } else if (r < 0) { r = 0; }
            if (g > 1) { g = 1; } else if (g < 0) { g = 0; }
            if (b > 1) { b = 1; } else if (b < 0) { b = 0; }
            return new Color(r, g, b, a);
        }

        /// <summary>
        /// 处理文字的颜色
        /// </summary>
        public static string ToColorString(string text, string color)
        {
            return "[" + color + "]" + text + "[-]";
        }

        #endregion //颜色处理

        #region 3D位置判断处理
        /// <summary>
        /// 起点到终点的向量相对于参考点是否为顺时针方向.
        /// </summary>
        /// <param name='start'>起点</param>
        /// <param name='end'>终点</param>
        /// <param name='reference'>参考点</param>
        /// <param name='angle'>向量1(参考点到向量2中点)和向量2(起点到终点)和之间的夹角</param>
        public static bool IsVectorClockWise(Vector3 start, Vector3 end, Vector3 reference, out float angle)
        {
            Vector3 vecMR = ((end + start) / 2 - reference); //vec1;
            Vector3 vecES = (end - start);	//vec2;

            angle = Vector3.Angle(vecES, vecMR);

            float x0 = vecMR.x;
            float z0 = vecMR.z;
            float x1 = vecES.x;
            float z1 = vecES.z;
            if (((x0 > 0 && z0 > 0) && (x1 > 0 && z1 < 0))		//顺时针向右(参考点正上方);
                || ((x0 > 0 && z0 < 0) && (x1 < 0 && z1 < 0))	//顺时针向下(参考点正左方);
                || ((x0 < 0 && z0 < 0) && (x1 < 0 && z1 > 0))	//顺时针向左(参考点正下方);
                || ((x0 < 0 && z0 > 0) && (x1 > 0 && z1 > 0)))	//顺时针向上(参考点正右方);
            {
                return true;
            }

            if (((x0 > 0 && z0 > 0) && (x1 < 0 && z1 > 0))		//逆时针向左(参考点正上方);
                || ((x0 > 0 && z0 < 0) && (x1 > 0 && z1 > 0))	//逆时针向上(参考点正右方);
                || ((x0 < 0 && z0 < 0) && (x1 > 0 && z1 < 0))	//逆时针向右(参考点正下方);
                || ((x0 < 0 && z0 > 0) && (x1 < 0 && z1 < 0)))	//逆时针向下(参考点正左方);
            {
                return false;
            }

            //Debug.Log("UnDefine!");
            return false;
        }

        /// <summary>
        /// 计算point到直线线的距离（直线由start和end决定）
        /// </summary>
        /// <returns>点到直线的距离</returns>
        public static float DistanceToLine(Vector2 start, Vector2 end, Vector2 point)
        {
            float deltaX = (start.x - end.x);
            if (Mathf.Abs(deltaX) <= 0.01f)
            {
                return Mathf.Abs(point.x - start.x);
            }

            float deltaY = (start.y - end.y);
            if (Mathf.Abs(deltaY) <= 0.01f)
            {
                return Mathf.Abs(point.y - start.y);
            }

            //直线判定需要的参数;
            //float kParam = (start.y - end.y) / (start.x - end.x);
            float kParam = deltaY / deltaX;
            float bParam = start.y - kParam * start.x;
            //Debug.Log("y=kx+b  K:" + kParam + "  B:" + bParam);
            //float dis = Mathf.Abs(kParam * point.x - point.y + bParam) / Mathf.Sqrt(kParam * kParam + 1);
            float dis = Mathf.Abs(kParam * point.x - point.y + bParam) / Mathf.Sqrt(kParam * kParam + 1);
            return dis;
        }

        //判断是否顺时针,原点起点终点，为true为顺时针，false为逆时针kh
        public static bool IsClockwise(Vector2 center, Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 middleP = GetCenterPoint(startPoint, endPoint);
            Vector3 tempVector2 = middleP - center;
            Vector3 tempVector3 = new Vector3(tempVector2.x, tempVector2.y, 0);
            Vector3 temp = Quaternion.Euler(0, 0, -90) * tempVector3;
            Vector2 clockwiseNormal = new Vector2(temp.x, temp.y);
            Vector2 moveToward = endPoint - startPoint;
            if (Vector2.Dot(moveToward, clockwiseNormal) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //获得两点中点kh
        private static Vector2 GetCenterPoint(Vector2 aPoint, Vector2 bPoint)
        {
            float x = (aPoint.x + bPoint.x) / 2;
            float y = (aPoint.y + bPoint.y) / 2;
            return new Vector2(x, y);
        }

        /// <summary>
        /// 获得指定orgP为中心，半径为dis内的，可以站立的位置;
        /// 但是这里的do循环代码其实有问题，因为如果orgP有问题，这个搜索很可能陷入死循环。
        /// 所以这里其实应该要做一个break的操作，暂时没有想好，先做一个强行退出避免错误。 Terry
        /// </summary>
        public static Vector3 Vector3GetPos(Vector3 orgP, float dis)
        {
            NavMeshHit hit;
            bool isHit = false;
            int i = 0;
            do
            {
                if (i > 256)
                    return orgP;
                isHit = NavMesh.SamplePosition(orgP + Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0) * Vector3.forward * UnityEngine.Random.Range(0.2f, dis), out hit, dis, 1);
                i++;
            }
            while (isHit == false);

            return hit.position;
        }

        #endregion //位置判断处理

        public static string ReplaceText(string recText)
        {
            return recText.Replace("L##F", "\n");
        }

        //正则表达只能输入数字不包括小数点(.) SJ
        public static string GetNumber(string str)
        {
            if (str != null && str != string.Empty)
            {
                // 正则表达式剔除非数字字符（不包含小数点.）
                str = Regex.Replace(str, @"[^\d\d]", "");
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos">需要转化的坐标</param>
        /// <param name="dis">目标车辆距离玩家角色距离（负落后玩家，正超过玩家）</param>
        /// <returns></returns>
        public static Vector3 GetMiniMapPos(float dis, float x, bool islocal = false)
        {
            Vector3 pos = Vector3.zero;
            pos.x = x * 11.66667f + 12;

            if (islocal)
            {
                return pos;
            }

            pos.y = dis;
          
            return pos;
        }

        public static string GetCurTime()
        {
            return DateTime.Now.ToString("HH:mm:ss.fff");
        }


        public static int Random(int min, int max)
        {
            int rand = UnityEngine.Random.Range(min, max);
            return rand;
        }

        public static float Random(float min, float max)
        {
            float rand = UnityEngine.Random.Range(min, max);
            return rand;
        }
        #region 简单加密解密
        /// <summary>
        /// 加密(循环左移3位，再+7);
        /// </summary>
        public static uint Encryption(uint data)
        {
            return data;
            //uint retData = ((data >> (32 - 3)) | (data << 3));
            //retData += 7;
            //return retData;
        }

        /// <summary>
        /// 解密(先-7, 再循环右移3位);
        /// </summary>
        public static uint Decryption(uint data)
        {
            return data;

            //uint retData = data - 7;
            //retData = ((retData << (32 - 3)) | (retData >> 3));
            //return retData;
        }
        #endregion //简单加密解密

        /// <summary>
        /// 域名转换为IP地址
        /// </summary>
        /// <param name="hostname">域名或IP地址</param>
        /// <returns>IP地址</returns>
        public static string Hostname2ip(string hostname)
        {
            try
            {
                IPAddress ip;
                if (IPAddress.TryParse(hostname, out ip))
                    return ip.ToString();
                else
                    return Dns.GetHostEntry(hostname).AddressList[0].ToString();
            }
            catch (Exception)
            {
                throw new Exception("IP Address Error");
            }
        }

        /// <summary>
        /// 用于确定当前是否震动手机
        /// </summary>
        public static bool IsPhoneVibrate = true;

        /// <summary>
        /// 快捷的手机震动函数,每次调用震动0.5秒，长度基本靠调用间隔和频率调整。
        /// 如果有更多需要，接下来可以考虑强化函数功能。
        /// </summary>
        public static void Vibrate()
        {
            if (IsPhoneVibrate)
            {
#if UNITY_IPHONE
                Handheld.Vibrate();
#endif

#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
            }
        }
    }
}