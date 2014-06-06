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
using System.Drawing;
using System.Windows.Forms;

using gen.snd.IffForm;
using gen.snd.Modules;

namespace GenericWAV.Modules
{
	// when in doubt: http://www.sonicspot.com/guide/wavefiles.html
	[Export(typeof(IAudioModule<>))]
	class module_file_wave : basic_forms_module<RiffForm>
	{
		#region Constants
		const string ChunkFormatPosition = "pos: {0:###,###,###,##0}";
		const string ChunkFormatSampleLoopInfo =
			"Begin: {1:###,###,###,##0}, " +
			"End: {2:###,###,###,##0}, " +
			"Frac: {3:###,###,###,##0}, " +
			"Samples: {4:###,###,###,##0}, " +
			"Cue ID: ‘{0}’";
		const string ChunkFormatFact =
			"Position: {0:###,###,###,##0}, Value: {1:###,###,###,##0}";
		#endregion
		
		#region static: ListViewItem Abstraction Helpersnew string[]{
		static ListViewItem ListInfo_SampleLoop(long position, _smpLoop slx)
		{
//			string ckid = slx.CuePointID[0]==0 ? string.Empty : slx.CuePointID;
			return new ListViewItem(
				new string[]{
					string.Format("{0}",slx.smplType),
					string.Format(ChunkFormatPosition, position),
					string.Format(
						ChunkFormatSampleLoopInfo,
						slx.CuePointID,
						slx.smplStart,		slx.smplEnd,
						slx.smplFraction,	slx.smplCount)
				},
				2
			);
		}
		static ListViewItem ListInfo_Fact(long position, /*SUBCHUNK chunk,*/ ChunkFact fact)
		{
			return new ListViewItem(
				new string[]{
					string.Format("{0}",fact.ckID),
					string.Format("{0:###,###,###,##0}",fact.ckLength),
					string.Format(ChunkFormatFact, position, fact.data)
				},
				0
			);
//			lvi.IndentCount = 1;
//					chunk.Value.ckLength.ToString("###,###,###,##0"),
//				•	"pos:" + chunk.Key.ToString("##,###,###,##0") + " | " +
//					"value:" + AudioModule.Cks.ckFact.data.ToString("##,###,###,##0")
		}
		#endregion
		
		string MainChunkLength	{ get { return AudioModule.Cks.ckMain.ckLength.ToString("##,###,###,##0"); } }
		string FormatInfo		{ get { return string.Format( gen.snd.Strings.WaveInfo, AudioModule.Cks.ckFmt.fmtRate, AudioModule.Cks.ckFmt.fmtBlock, AudioModule.Cks.ckFmt.fmtTag, AudioModule.Cks.ckFmt.fmtChannels ); } }
		
		public override void LoadModule()
		{
			AudioModule = RiffForm.Load(this.FilePath);
			this.IsModuleLoaded = true;
		}
		
		public module_file_wave(string path)
		{
			this.FilePath = path;
			
			Clear();
			LoadModule();
			CreateMainListViewItem();
			AttachView();
		}
		
		public override ListViewItem CreateMainListViewItem()
		{
			//	Insert Column Headers
			gen.snd.Common.lvcols(ref lv,new string[]{"ckID","ckSize","pos"});
			//	Create RAM
			ListViewItem lvx = lv.Items.Add(new ListViewItem(new string[]{ FileName, MainChunkLength, FormatInfo },0));
			lvx.BackColor = Color.Black;
			lvx.ForeColor = Color.AliceBlue;
			return lvx;
		}

		public override void AttachView()
		{
			foreach (KeyValuePair<long,SUBCHUNK> chunk in AudioModule.Cks.SubChunks)
			{
				ListViewItem lvi;
				if (chunk.Value.ckID != null) switch (chunk.Value.ckID.ToLower())
				{
					case "fact":
						lv.Items.Add(ListInfo_Fact(chunk.Key, AudioModule.Cks.ckFact))
							.IndentCount = 1;
//						lvi = lv.Items.Add(chunk.Value.ckID,0);
//						lvi.IndentCount = 1;
//						lvi.SubItems.AddRange(
//							new string[]{
//								chunk.Value.ckLength.ToString("##,###,###,##0"),
//								"pos:" + chunk.Key.ToString("##,###,###,##0") + " | " +
//								"value:" + AudioModule.Cks.ckFact.data.ToString("##,###,###,##0")
//							});
						break;
					case "smpl":
						
						// --------------------------------------------
						// 'smpl'
						// --------------------------------------------
						lvi = lv.Items.Add(chunk.Value.ckID,0);
						lvi.IndentCount = 1;
						lvi.SubItems.AddRange(
							new string[]{
								chunk.Value.ckLength.ToString("##,###,###,##0"),
								"pos:" + chunk.Key.ToString("##,###,###,##0") + " | " +
								"loops:" + AudioModule.Cks.ckSmpl.smpSampleLoops.ToString("##,###,###,##0")
							});
						//Text = rw.Cks.ckSmpLoop.Length.ToString();
						
						// --------------------------------------------
						// Sample Loop Information
						// --------------------------------------------
						
						foreach (_smpLoop slx in AudioModule.Cks.ckSmpLoop)
						{
							lv.Items.Add(ListInfo_SampleLoop(chunk.Key,slx))
								.IndentCount = 2;
						}
						break;
					case "inst":
						lvi = lv.Items.Add(chunk.Value.ckID,0);
						lvi.IndentCount = 1;
						//	lvi.SubItems.Add(kv.Value.ckLength.ToString("##,###,###,##0"));
						//	lvi.SubItems.Add(kv.Key.ToString("##,###,###,##0"));
						lvi.SubItems.AddRange(
							new string[]{
								"pos:" + chunk.Key.ToString("##,###,###,##0"),
								"start:" + AudioModule.Cks.ckInst.ckID + " | " +
									"tun:" + AudioModule.Cks.ckInst.fineTune.ToString("##,###,###,##0") + " | " +
									"nn:" + AudioModule.Cks.ckInst.uNote.ToString("##,###,###,##0") + " | " +
									"vHi:" + AudioModule.Cks.ckInst.velHigh.ToString("##,###,###,##0") + " | " +
									"vLo:" + AudioModule.Cks.ckInst.velLow.ToString("##,###,###,##0") + " | " +
									"kHi:" + AudioModule.Cks.ckInst.noteHigh.ToString("##,###,###,##0") + " | " +
									"kLo:" + AudioModule.Cks.ckInst.noteLow.ToString("##,###,###,##0")
							});
						break;
					default:
						lvi = lv.Items.Add(chunk.Value.ckID,1);
						lvi.IndentCount = 1;
						lvi.SubItems.Add(chunk.Value.ckLength.ToString("##,###,###,##0"));
						lvi.SubItems.Add(chunk.Key.ToString("##,###,###,##0"));
						break;
				}
			}
//			AudioModule = null;
			IsModuleLoaded = true;
		}
//		
//		public override void ViewSampleList()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public override void ViewMainList()
//		{
//			throw new NotImplementedException();
//		}
//		
//		public override void ViewInstrumentList()
//		{
//			throw new NotImplementedException();
//		}
	}
}
