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
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Forms;

using gen.snd.Formats;
using gen.snd.Modules;

namespace gen.snd.Modules
{
	[Export(typeof(IAudioModule<>))]
	class module_file_iti : basic_forms_module<ITI>
	{
		public override void LoadModule()
		{
			if (!IsModuleLoaded) AudioModule = new ITI(XPLO.CurPath);
			IsModuleLoaded = true;
		}
		
		public module_file_iti(string filename)
		{
			this.FilePath = filename;
			ViewInitialize();
		}
		
		public override ListViewItem CreateMainListViewItem()
		{
			throw new NotImplementedException();
		}
		public override void ViewReset()
		{
			lv.Items.Clear();
		}
		public override void ViewInitialize()
		{
			LoadModule();
			ViewReset();
			
			ListViewItem lvi = lv.Items.Add(Path.GetFileName(XPLO.CurPath),2);
			lvi.SubItems.AddRange(
				new string[]{
					string.Format("name: ‘{0,12}’",AudioModule.ITI_INST.impInstrumentName),	//.ToString("##,###,###,##0")
					string.Format("DOS: ‘{0,26}’",AudioModule.ITI_INST.impDosFileName)	//.ToString("##,###,###,##0")
				});
			lvi = lv.Items.Add("Samples",1);	lvi.IndentCount=1;
			foreach (ITI.impx smp in AudioModule.ITI_SMPH)
			{
				lvi = lv.Items.Add(smp.impsDosFileName,0);
				lvi.IndentCount = 2;
				lvi.SubItems.AddRange( new string[]{ "name: "+smp.impsSampleName, ITI.Resources.GetSampleHeader(smp) });
			}
			AudioModule = null;
		}
		
	}
}
