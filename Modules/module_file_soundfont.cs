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
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using DspAudio.Formats;
using DspAudio.IffForm;
using DspAudio.Modules;
using SFGenConst = DspAudio.Formats.SoundFont2.SFGenConst;
using SFSampleLink = DspAudio.Formats.SoundFont2.SFSampleLink;

// generator constants are within the sf-class



namespace GenericWAV.Modules
{

	[Export(typeof(IAudioModule<>))]
	class module_file_soundfont : basic_forms_module<SoundFont2>
	{
		List<SFBank> BankList = new List<SFBank>();
//		DictionaryList<int,PGen> GenList = new DictionaryList<int,PGen>();
		
		public module_file_soundfont(string path)
		{
			this.FilePath = path;
			System.Diagnostics.Debug.Print("Module: SoundFont Initializing Path: {0}\n",path);
			AttachView();
		}
		public override void LoadModule()
		{
		}

		void ClearInstruments()
		{
			BankList.Clear();
		}
		
		public override void DetachView()
		{
			lva.DoubleClick -= Event_ListView_HandleSoundFont;
		}
		
		public override ListViewItem CreateMainListViewItem() { throw new NotImplementedException(); }
		
		public override void ResetView()
		{
			lva.Clear();
			lvb.Clear();
			lv.Items.Clear();
			
			Common.lvcols(ref  lv, new string[]{ "ckID", "ckSize", "pos" });
			Common.lvcols(ref lva, new string[]{ "bnk:lib", "Name" });
			Common.lvcols(ref lvb, new string[]{ "Name", "rate", "Length", "Type", "Key", "Fine" });
			
			if (splitc.Panel2Collapsed == true) splitc.Panel2Collapsed = false;
			
			System.Diagnostics.Debug.Print("Prepared SoundFont View\n");
		}
		
		public override void AttachView()
		{
			this.ResetView();
			AudioModule = new SoundFont2(this.FilePath);
			System.Diagnostics.Debug.Print("Created SoundFont Module\n");
			
			ListViewItem lvx = lv.Items.Add(Path.GetFileName(this.FilePath),0);
			lvx.SubItems.AddRange( new string[]{ IOHelper.GetString(AudioModule.nfo.inam.StrValue) });
			lvx.Tag = this;
			
			lva.DoubleClick += Event_ListView_HandleSoundFont;
			
			ListPresets();
			ListSamples();
		}
		
		void Event_ListView_HandleSoundFont(object sender, EventArgs e)
		{
			Select((int)lva.SelectedItems[0].Tag);
		}

		/// Add Preset Listing to ListView A
		public void ListPresets()
		{
			for (
				int i=0;
				i<AudioModule.hyde.phdr.Count-1;
				i++
			)
				BankList.Add(new SFBank(AudioModule,i));
			BankList.Sort(SFBank.SortBankPreset);
			foreach (SFBank bank in BankList)
				bank.ToListView(lva);
		}
		
		/// Add Sample Listing to ListView B
		public void ListSamples()
		{
			ListViewItem lvx;
			for (int f = 0; f < AudioModule.hyde.shdr.Count-1; f++)
			{
				lvx = lvb.Items.Add(IOHelper.GetString(AudioModule.hyde.shdr[f].iName),"agrad");
				lvx.SubItems.AddRange(
					new string[]{
						AudioModule.hyde.shdr[f].SampleRate.ToString("##,###,##0"),
						(AudioModule.hyde.shdr[f].LenB - AudioModule.hyde.shdr[f].LenA).ToString("##,###,##0"),
						((SFSampleLink)AudioModule.hyde.shdr[f].Type).ToString() + " " + AudioModule.hyde.shdr[f].Link.ToString(),
						AudioModule.nn[AudioModule.hyde.shdr[f].Pitch],
						AudioModule.hyde.shdr[f].PitchC.ToString("##,###,##0")
					});
			}
		}
		
		/// <summary>
		/// This method is a event handler.
		/// The Method iterates through each pbag, and obtains info on each module
		/// the pbag indexes point to.
		/// </summary>
		/// <remarks>
		/// § 7.7 The IBAG Sub-Chunk
		/// <para>
		/// If a zone other than the first zone lacks a sampleID generator as its
		/// last generator, that zone should be ignored. A global zone with no
		/// modulators and no generators should also be ignored.
		/// </para>
		/// </remarks>
		/// <param name="i">PHDR</param>
		public void Select(int i)
		{
			lv.Items.Clear();
			ListViewItem lvx;
			for (
				int
				j = AudioModule.hyde.phdr[i].presetBagIndex;
				j < AudioModule.hyde.phdr[i+1].presetBagIndex;
				j++
			)
			{
				for (
					int
					k = AudioModule.hyde.pbag[j].gen;
					k < AudioModule.hyde.pbag[j+1].gen;
					k++)
				{
					SoundFont2.PGEN gen = AudioModule.hyde.pgen[k];
					switch (gen.GeneratorType)
					{
						case SFGenConst.instrument:
							{
								int PGeneratorID = gen.Shift8;
								
								lvx = lv.Items.Add( IOHelper.GetString( AudioModule.hyde.inst[ gen.Shift8 ].iName ), "agrad" );
								lvx.SubItems.AddRange( new string[]{ AudioModule.hyde.GetStringRange(PGeneratorID) });
								
								for (
									int
									l = AudioModule.hyde.ibag[AudioModule.hyde.BagIndex(PGeneratorID)].gen;
									l < AudioModule.hyde.ibag[AudioModule.hyde.BagIndex(PGeneratorID+1)].gen;
									l++
								)
									Switch_IGEN(i,j,k,l);
							} break;
							
						default:
							lvx = lv.Items.Add(string.Format("{0} (preset-level)",gen.GeneratorType),2);
							break;
					}
				}
			}
			Common.lvsize(ref lv,ColumnHeaderAutoResizeStyle.ColumnContent);
		}
		void Switch_IGEN(int i, int j, int k, int indexIGEN)
		{
			SoundFont2.IGEN igen = AudioModule.hyde.igen[indexIGEN];
			SFGenConst tgen = (SFGenConst)igen.gen;
			//AddInstList(indexIGEN);
			switch	( tgen )
			{
				case SFGenConst.sampleID:
				case SFGenConst.sampleModes:
					AddInstList(indexIGEN,1,null);
					break;
				case SFGenConst.keyRange:
				case SFGenConst.overridingRootKey:
				case SFGenConst.exclusiveClass:
				case SFGenConst.pan:
				case SFGenConst.delayModEnv:
				case SFGenConst.attackModEnv:
				case SFGenConst.decayModEnv:
				case SFGenConst.holdModEnv:
				case SFGenConst.sustainModEnv:
				case SFGenConst.releaseModEnv:
				case SFGenConst.modLfoToFilterFc:
				case SFGenConst.modLfoToPitch:
				case SFGenConst.modLfoToVolume:
				case SFGenConst.delayVibLFO:
				case SFGenConst.delayModLFO:
				case SFGenConst.freqModLFO:
				case SFGenConst.freqVibLFO:
					AddInstList(indexIGEN,2,null);
					break;
				default:
					AddInstList(indexIGEN,3,Color.Silver);
					break;
			}
		}
		
		#region Hide
		/**/
		#endregion
		
		string GetInstImageKey(int indexIGEN)
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
		
		void AddInstList(int indexIGEN, int indent, Color? background)
		{
			ListViewItem l = lv.Items.Add(
				new ListViewItem(
					new string[]{ AudioModule.hyde.igen[indexIGEN].Generator.ToString(), GetInstString(indexIGEN), AudioModule.GetGenValueType(indexIGEN).ToString() },
					GetInstImageKey(indexIGEN)
				));
			l.IndentCount = indent;
			if (background!=null) l.BackColor = background.Value;
		}
		void AddInstList(int indexIGEN)
		{
			AddInstList(indexIGEN,1,null);
		}
		
		string GetInstString(int indexIGEN)
		{
			SoundFont2.IGEN igen = AudioModule.hyde.igen[indexIGEN];
			SFGenConst tgen = (SFGenConst)igen.gen;
			
			switch	( tgen )
			{
					case SFGenConst.sampleID: return string.Format("{0}:“{1}”",sf2str.GenString(AudioModule,indexIGEN),IOHelper.GetZerodStr(AudioModule.hyde.shdr[igen.genHi].iName));
					case SFGenConst.sampleModes: return string.Format("{0}", sf2str.Range2StringA(AudioModule,indexIGEN));
					case SFGenConst.keyRange: return string.Format("{1} ({2})",igen.Generator,sf2str.GetNumRange(igen),sf2str.GetKeyRange(AudioModule,indexIGEN));
					case SFGenConst.overridingRootKey: return string.Format("{0}:{1}",igen.Generator,igen.genHi, sf2str.GetRootKey(AudioModule,igen));
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
	}
	
	
	
	class sf2str
	{
		static public string Range2String(SoundFont2 sf, int genIndex)
		{
			return string.Format("{0:00#} - {1:00#}", sf.hyde.igen[genIndex].genHi, sf.hyde.igen[genIndex].genLo);
		}
		static public string[] Range2StringA(SoundFont2 sf, int genIndex)
		{
			return new string[]{ Range2String(sf,genIndex) };
		}
		
		static public string GenString(SoundFont2 sf, int genIndex)
		{
			return ((SFGenConst)sf.hyde.igen[genIndex].gen).ToString();
		}
		
		/// <summary>
		/// Centibels takes a value from 0 to 6000
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		static public short ConvertCentibels(short value)
		{
			return 0;
		}
		static public string GetRootKey(SoundFont2 mod, SoundFont2.IGEN gen)
		{
			return mod.nn[gen.genHi];
		}
		static public string GetKeyRange(SoundFont2 mod, int gen)
		{
			int hi = (int)mod.hyde.igen[gen].genHi, lo = (int)mod.hyde.igen[gen].genLo;
			return string.Format("{0} to {1}",mod.nn[hi],mod.nn[lo]);
		}
		static public string GetNumRange(SoundFont2.IGEN gen)
		{
			int hi = (int)gen.genHi, lo = (int)gen.genLo;
			return string.Format("{0} to {1}",hi,lo);
		}
	}
	#region Commented
	//	class PGen
//	{
//		int inst, pbag, ibag;
//
//		public SoundFont2.PGEN gen;
//
//		public Generator_Constants GeneratorType { get { return this.gen.GeneratorType; } }
//
//		public short Value { get { return Convert.ToInt16((MSB << 8) | LSB); } }
//
//		/// <summary>
//		/// The first byte.
//		/// </summary>
//		public byte MSB { get { return this.gen.TypeLo; } }
//		/// <summary>
//		/// The second byte.
//		/// </summary>
//		public byte LSB { get { return this.gen.TypeHi; } }
//
//
//		public PGen(int index, int pbag, int ibag, SoundFont2.PGEN gen)
//		{
//			this.inst = index;
//			this.pbag = pbag;
//			this.ibag = ibag;
//			this.gen = gen;
//		}
//	}
	#endregion
}
