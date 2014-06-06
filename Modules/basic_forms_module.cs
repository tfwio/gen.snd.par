#region User/License
// oio * 2005-11-12 * 04:19 PM
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

//using Windows.CommonControls;

using gen.snd.Modules;

namespace gen.snd.Modules
{
	abstract class basic_forms_module<TModule> : BasicAudioModule<TModule>
	{
		#region Static
		static internal ListView lv, lva, lvb;
		static internal SplitContainer splita, splitb, splitc, splitd;
		static internal Windows.CommonControls.CsShellFileView XPLO;
		
		static internal void InitializeExplo(Windows.CommonControls.CsShellFileView x)
		{
			XPLO = x;
		}
		
		static internal void InitializeSplitContainers(
			SplitContainer a, SplitContainer b, SplitContainer c, SplitContainer d)
		{
			splita = a;
			splitb = b;
			splitc = c;
			splitd = d;
		}
		static internal void InitializeListViews(ListView list1, ListView list2, ListView list3)
		{
			lv		= list1;
			lva		= list2;
			lvb		= list3;
		}
		
		static internal void Clear()
		{
			lva.Clear();
			lvb.Clear();
			lv.Items.Clear();
		}
		#endregion
		
		virtual public void AttachView(){}
		virtual public void ResetView(){}
		virtual public void DetachView(){}
		abstract public ListViewItem CreateMainListViewItem();
	}
}
