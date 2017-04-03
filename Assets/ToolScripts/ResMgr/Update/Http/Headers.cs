using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Update
{
	/// <summary>
    /// HTTP style headers;
	/// </summary>
	public class Headers
	{
		
		/// <summary>
		/// 添加header;
		/// </summary>
		public void Add (string name, string value)
		{
			GetAll (name).Add (value);
		}
		
		/// <summary>
		/// 通过name 获取header;
		/// </summary>
		public string Get (string name)
		{
			List<string> header = GetAll (name);
			if (header.Count == 0) {
				return "";
			}
			return header [0];
		}
		
		/// <summary>
		/// 是否包含此name header;
		/// </summary>
		public bool Contains (string name)
		{
			List<string> header = GetAll (name);
			if (header.Count == 0) {
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// 获取此name header 所有值;
		/// </summary>
		public List<string> GetAll (string name)
		{
			foreach (string key in headers.Keys) {
				if (string.Compare (name, key, true) == 0) {
					return headers [key];
				}
			}
			List<string> newHeader = new List<string> ();
			headers.Add (name, newHeader);
			return newHeader;
		}
		
		/// <summary>
		/// 给此name header 设置唯一值;
		/// </summary>
		public void Set (string name, string value)
		{
			List<string> header = GetAll (name);
			header.Clear ();
			header.Add (value);
		}
		
		/// <summary>
		/// 从header列表中移除此name header;
		/// </summary>
		public void Pop (string name)
		{
			if (headers.ContainsKey (name)) {
				headers.Remove (name);
			}
		}
		
		
		
		/// <summary>
		/// 获取所有列表中的header name值;
		/// </summary>
		public List<string> Keys {
			get {
				return headers.Keys.ToList();		
			}
		}
		
		/// <summary>
		/// 清空列表;
		/// </summary>
		public void Clear() {
			headers.Clear();
		}
	
		Dictionary<string, List<string>> headers = new Dictionary<string, List<string>> ();

        public Hashtable GetHashtable() {
            return new Hashtable(headers);
        }
	}
}