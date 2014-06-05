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
using DspAudio.Formats;

namespace GenericWAV.Modules
{
	
	public struct SFBank
	{
		public int index;
		public short bank;
		public short preset;
		public string PresetName;
		
		public SFBank(SoundFont2 sf, int index)
		{
			this.index = index;
			this.bank = sf.hyde.phdr[index].bank;
			this.preset = sf.hyde.phdr[index].preset;
			this.PresetName = IOHelper.GetString(sf.hyde.phdr[index].presetName);
		}
		
		public void ToListView(ListView list)
		{
			list.Items.Add(new ListViewItem( new string[]{ BankPresetString, this.PresetName }, "inst" ))
				.Tag = this.index;
		}
		
		public string BankPresetString
		{
			get { return string.Format( "{0:00#}:{1:00#}", bank, preset ); }
		}
		
		public int IntIndex
		{
			get { return (int)bank << 8 | (int)preset; }
		}
		
		static public int SortBankPreset(SFBank a, SFBank b)
		{
			return a.IntIndex - b.IntIndex;
		}
	}
}
