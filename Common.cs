#region User/License
// oio * 7/17/2012 * 7:56 AM

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Windows.Forms;

namespace GenericWAV
{
	static class Strings
	{
		internal const string Thousands = "##,###,###,##0";
		internal const string WaveInfo =
			"Rate: {0:##,###,###,##0} | " +
			"Block: {1:##,###,###,##0} | " +
			"Tag: {2:##,###,###,##0} | " +
			"CH: {3:##,###,###,##0}";
	}
	
	/// <summary>
	/// ListView Helper.
	/// </summary>
	static class Common
	{
		internal static void lvcols(ref ListView lv, string[] columns)
		{
			lv.Columns.Clear();
			foreach (string str in columns) { lv.Columns.Add(str);  }
		}
		internal static void lvsize(ref ListView lv, ColumnHeaderAutoResizeStyle style)
		{
			try {
				foreach (ColumnHeader ch in lv.Columns) ch.AutoResize(style);
			}catch{}
		}
		internal static void lvsize(ListView[] lv, ColumnHeaderAutoResizeStyle style)
		{
			for (int i=0;i<lv.Length;i++) lvsize(ref lv[i],style);
		}
		internal static void lVisi(ListView lv, bool flag)
		{
			lv.Visible = flag;
		}
		internal static void SetVisibility(ListView[] lvz, bool flag)
		{
			foreach (ListView lv in lvz) lv.Visible = flag;
		}
		
	}
}
