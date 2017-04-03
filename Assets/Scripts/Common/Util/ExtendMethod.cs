using UnityEngine;
using System.Collections;
using Need.Mx;
using System.Collections.Generic;

/// <summary>
/// 添加扩展方法类;字符串的浮点整形转换，以及克隆方法。
/// </summary>
public static class ExtendMethod
{
    /// <summary>
    /// 给指定对象添加T组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
    {
        T t = obj.GetComponent<T>();
        if (null == t)
        {
            t = obj.AddComponent<T>();
        }

        return t;
    }

	/// <summary>
    /// 字符串转换浮点 如果转换失败 返回0;
	/// </summary>
	/// <param name="self"></param>
	/// <returns></returns>
	public static float ToFloat (this string self)
	{
		float f = 0;
		float.TryParse (self, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out f);
		return f;
	}
	/// <summary>
    /// 字符串转整形 如果转换失败 返回0;
	/// </summary>
	/// <param name="self"></param>
	/// <returns></returns>
	public static int ToInt (this string self)
	{
		int i = 0;
		int.TryParse (self, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat, out i);
		return i;
	}
	
	/// <summary>
	/// 拷贝一个int链表出来
	/// </summary>
	/// <param name="self"></param>
	/// <returns></returns>
    public static List<int> Clone(this List<int> self)
    {
        List<int> listInt = new List<int>();
        foreach(int item in self){
            listInt.Add(item);
        }
        return listInt;
    }

    /// <summary>
    /// 按钮变灰，和恢复;
    /// </summary>
    /// <param name="self"></param>
    /// <param name="grey">是否变灰</param>
    /// <returns></returns>
    public static void Grey(this GameObject self, bool grey)
    {
        BoxCollider boxCollider = self.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            GameObject greyImgGO = self.transform.Find("Grey").gameObject;
            if (greyImgGO != null)
            {
                greyImgGO.SetActive(grey);
                boxCollider.enabled = !grey;
            }
        }
    }


}
