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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DspAudio.Formats;
using DspAudio.IffForm;
using SFGenConst = DspAudio.Formats.SoundFont2.SFGenConst;
using SFSampleLink = DspAudio.Formats.SoundFont2.SFSampleLink;

// generator constants are within the sf-class



namespace GenericWAV.Modules
{
	class SoundFontModule : basic_forms_module<SoundFont2>
	{
		public List<SFBank> BankList { get { return bankList; } }
		
		List<SFBank> bankList = new List<SFBank>();
		
		internal string GetInstString(int indexIGEN)
		{
			SoundFont2.IGEN igen = AudioModule.hyde.igen[indexIGEN];
			SFGenConst tgen = (SFGenConst)igen.gen;
			
			switch	( tgen )
			{
				case SFGenConst.sampleID:
					return string.Format("{0}:“{1}”",sf2str.GenString(AudioModule,indexIGEN),IOHelper.GetZerodStr(AudioModule.hyde.shdr[igen.genHi].iName));
				case SFGenConst.sampleModes:
					return string.Format("{0}", sf2str.Range2StringA(AudioModule,indexIGEN));
				case SFGenConst.keyRange:
					return string.Format("{1} ({2})",igen.Generator,sf2str.GetNumRange(igen),sf2str.GetKeyRange(AudioModule,indexIGEN));
				case SFGenConst.overridingRootKey:
					return string.Format("{0}:{1}",igen.Generator,igen.genHi, sf2str.GetRootKey(AudioModule,igen));
				case SFGenConst.exclusiveClass:
				case SFGenConst.delayModEnv:
				case SFGenConst.attackModEnv:
				case SFGenConst.decayModEnv:
				case SFGenConst.holdModEnv:
				case SFGenConst.sustainModEnv:
				case SFGenConst.releaseModEnv:
				case SFGenConst.pan:
					return string.Format("{0:X2}:{1:X2}", igen.genHi, igen.genLo);
				case SFGenConst.delayVibLFO:
				case SFGenConst.delayModLFO:			// timecents
				case SFGenConst.freqModLFO:		  // freq:cents
				case SFGenConst.freqVibLFO:		  // freq:cents
				case SFGenConst.modLfoToFilterFc:// degree:cents
				case SFGenConst.modLfoToPitch:		// degree:cents
				case SFGenConst.modLfoToVolume:	// degree:cents
					return string.Format("amt: 0x{1:X2}:{1} hex: 0x{0:X4}, short: {0}", (igen.genHi << 8) +igen.genLo, igen.genAmt);
				default:
					return string.Format("{0:X2}:{1:X2} - {1:X2} - {2}", igen.genHi, igen.genLo, igen.Int16Bits);
			}
		}
		internal string GetInstImageKey(int indexIGEN)
		{
			switch (AudioModule.hyde.igen[indexIGEN].Generator)
			{
				case SFGenConst.sampleID:
					return "agreen";
				case SFGenConst.sampleModes:
					return "bgreen";
				case SFGenConst.keyRange:
				case SFGenConst.overridingRootKey:
				case SFGenConst.exclusiveClass:
				case SFGenConst.pan:
					return "ared";
				case SFGenConst.delayModEnv:
				case SFGenConst.attackModEnv:
				case SFGenConst.decayModEnv:
				case SFGenConst.holdModEnv:
				case SFGenConst.delayModLFO:
				case SFGenConst.freqModLFO:
				case SFGenConst.sustainModEnv:
				case SFGenConst.releaseModEnv:
				case SFGenConst.modLfoToFilterFc:
				case SFGenConst.modLfoToPitch:
				case SFGenConst.modLfoToVolume:
					return "acyan";
				default:
					return "agrad";
			}
		}
//		
//		public override void ViewSampleList()
//		{
//		}
//		public override void ViewMainList()
//		{
//		}
//		public override void ViewInstrumentList()
//		{
//		}
		
		public override void AttachView()
		{
			this.ViewReset();
			AudioModule = new SoundFont2(this.FilePath);
			System.Diagnostics.Debug.Print("Created SoundFont Module\n");
			
			ListViewItem lvx = lv.Items.Add(Path.GetFileName(this.FilePath),0);
			lvx.SubItems.AddRange( new string[]{ IOHelper.GetString(AudioModule.nfo.inam.StrValue) });
			lvx.Tag = this;
			
			lva.DoubleClick += Event_ListView_HandleSoundFont;
			
			for (
				int i=0;
				i<AudioModule.hyde.phdr.Count-1;
				i++
			)
				BankList.Add(new SFBank(AudioModule,i));
			BankList.Sort(SFBank.SortBankPreset);
//			ListPresets();
			ViewSampleList();
		}

		
		public override ListViewItem CreateMainListViewItem()
		{
			throw new NotImplementedException();
		}
	}
}
