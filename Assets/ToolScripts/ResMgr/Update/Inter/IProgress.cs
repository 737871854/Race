using System;
using System.Collections.Generic;

namespace Need.Mx
{

	public interface IProgress
	{
		/// <summary>
		/// 显示进度条,rate=0~1.
		/// </summary>	
		void Show(float rate,string info="");
		
		/// <summary>
		/// 关闭进度条.
		/// </summary>
		void Close();


	}
}
