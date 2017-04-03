//
// /**************************************************************************
//
// IDynamicProperty.cs
//
//
// **************************************************************************/

using System;

namespace Need.Mx
{
	public interface IDynamicProperty
	{
		void DoChangeProperty(int id, object oldValue, object newValue);
		PropertyItem GetProperty(int id);
	}
}

