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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using gen.snd.Formats;
using gen.snd.IffForm;
using gen.snd.Modules;
using GenericWAV.Modules;

namespace GenericWAV.Views
{
	partial class DspAudioTestForm : Form
	{
		#region Settings
		
		private bool isFileTreeVisible; // default = true;
		
		public bool IsFileListVisible {
			get { return !XPLO.SCo.Panel2Collapsed; }
			set { XPLO.SCo.Panel2Collapsed = !value; }
		}
		
		public bool IsWaveVisible {
			get { return false; }
			set { ; }
		}
		
		public bool IsFileViewVisible {
			get { return !splita.Panel1Collapsed; }
			set { splita.Panel1Collapsed = !value; }
		}
		
		class DspAudioTestFormSettings
		{
			// we need settings to write.
			static readonly string settingsfile = "settings.text";
			
			#region Regex
			const RegexOptions defaultRegexOptions = RegexOptions.Multiline|RegexOptions.IgnoreCase|RegexOptions.CultureInvariant;
			static readonly Regex[] Expressions = new Regex[]{
				new Regex(@"^(IsFileViewVisible)\s=\s(\w*)",defaultRegexOptions),
				new Regex(@"^(IsFileListVisible)\s=\s(\w*)",defaultRegexOptions),
				new Regex(@"^(IsWaveVisible)\s=\s(\w*)",defaultRegexOptions)
			};
			#endregion
			
			static public bool HasSettings
			{
				get { return FilePath.Exists; }
			}
			
			static public FileInfo FilePath
			{
				get
				{
					FileInfo f0 = new FileInfo(Application.ExecutablePath);
					FileInfo f1 = new FileInfo(Path.Combine(f0.Directory.FullName,settingsfile));
					f0 = null;
					return f1;
				}
			}
			static public void Clear(FileInfo f)
			{
				if (f.Exists) File.Delete(f.FullName);
			}
			static public void Save(DspAudioTestForm form)
			{
				FileInfo f = FilePath;
				Clear(f);
				using (FileStream fs = File.Open(f.FullName,FileMode.OpenOrCreate,FileAccess.Write,FileShare.None))
					using (TextWriter writer = new StreamWriter(fs))
				{
					writer.Write("IsFileListVisible = {0}\n",form.IsFileListVisible);
					writer.Write("IsWaveVisible = {0}\n",form.IsWaveVisible);
					writer.Write("IsFileViewVisible = {0}\n",form.IsWaveVisible);
				}
				f = null;
			}
			static public void Read(DspAudioTestForm form)
			{
				FileInfo f = FilePath;
				if (!f.Exists) return; // use defaults
				string content = File.ReadAllText(f.FullName);
				foreach (Regex rx in Expressions)
				{
					Match m = rx.Match(content);
					
					Debug.WriteLine(string.Format("Match Content: {0}",content));
					Debug.WriteLine(string.Format("IsMatch: {0}, Name: {1}, Value: {2}",rx.IsMatch(content), m.Groups[1].Value, m.Groups[2].Value));
					
					string test = m.Groups[1].Value;
					switch (test)
					{
//							case "IsFileViewVisible": form.IsFileViewVisible = Boolean.Parse(m.Groups[2].Value); break;
							case "IsFileListVisible": form.IsFileListVisible = Boolean.Parse(m.Groups[2].Value); break;
							case "IsWaveVisible": form.IsWaveVisible = Boolean.Parse(m.Groups[2].Value); break;
					}
				}
				f = null;
			}
		}
		
		#endregion
		Bitmap SplashImage;
		private gen.snd.wave.views.SplashForm spx;
		private gen.snd.Forms.WaveOutTestForm PlayerForm;

		BasicAudioModule ActiveModule;
		// ===============================================
		// Splash (About) Screen
		// ===============================================
		gen.snd.wave.views.SplashFormController splash;
		
		#region Simple List Hide/Show Util
		
		delegate void Handler();
		
		class ListHandler : IDisposable
		{
			Handler post;
			public ListHandler(Handler before, Handler after)
			{
				this.post = after;
				before();
			}
			void IDisposable.Dispose()
			{
				post();
				post = null;
			}
		}
		// ----------------------------------
		// ListHandler Impl Methods
		// ----------------------------------
		void ActionBefore<T>()
		{
			Common.SetVisibility(new ListView[]{lv,lva,lvb},false);
			InitializeUI<T>();
		}
		// Hide ListView
		void ActionPost()
		{
			Common.SetVisibility(new ListView[]{lv,lva,lvb},true);
		}
		void InitializeUI<T>()
		{
			basic_forms_module<T>.InitializeListViews( lv, lva, lvb );
			basic_forms_module<T>.InitializeSplitContainers( this.splita,this.splitb,this.splitc,this.splitd );
			basic_forms_module<T>.InitializeExplo( XPLO );
		}
		
		#endregion
		
		public DspAudioTestForm()
		{
			InitializeComponent();
			InitializeSpashScreen();
			
			if (!DspAudioTestFormSettings.HasSettings)
			{
				DspAudioTestFormSettings.Save(this);
			}
			WindowsInterop.WindowsTheme.HandleTheme(lv);
			WindowsInterop.WindowsTheme.HandleTheme(lva);
			WindowsInterop.WindowsTheme.HandleTheme(lvb);
			PrepareExplo();
			InitializeUI<ITI>();
			DspAudioTestFormSettings.Read(this);
		}
		
		// ===============================================
		// Alt Key :/: Toggle Main Menu
		// ===============================================
		
		#region Event: KeyHandlers
		void Event_ALT_KeyPressed(object sender, PreviewKeyDownEventArgs e)
		{
			Text = e.Alt.ToString();
			if (this.menuMain.Visible == true && e.Alt)this.menuMain.Visible = false;
			else if (e.Alt && this.menuMain.Visible == false) this.menuMain.Visible = true;
		}
		void Event_ALT_KeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			Text = "GenIO Instrument Compilation Unit";
			//	this.menuMain.Visible = false;
		}
		void Event_ALT_KeyUp(object sender, KeyEventArgs e)
		{
			Text = "GenIO Instrument Compilation Unit";
			//	this.menuMain.Visible = false;
		}
		#endregion
		
		
		
		// ===============================================
		// sndPlaySound API Caller
		// ===============================================
		
		#region Event: API SndPlaySound
		void Event_PlaySound(object sender, EventArgs e) { PlaySoundAction(); }
		void PlaySoundAction()
		{
			if (
				( File.Exists(XPLO.CurPath) ) &&
				( XPLO.CurPath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)) )
			{
				MM_Sys.sndPlaySound(
					XPLO.CurPath,
					MM_Sys.sndFlags.FILENAME | MM_Sys.sndFlags.ASYNC
				);
			}
		}
		#endregion

		#region “event”
		
		void Event_CsShellListView_ItemClicked(object o, EventArgs e)
		{
			string fileExt;
			
			if (File.Exists(XPLO.CurPath)) fileExt = Path.GetExtension(XPLO.CurPath).ToLower();
			else { Debug.Print("ListView-Item Click Error."); return; }
			
			if (ActiveModule!=null) {
				ActiveModule.ViewDetach();
				ActiveModule = null;
			}
			
			// Wave File Loader
			if ( fileExt==".wav" ) using (new ListHandler(ActionBefore<RiffForm>,ActionPost))
			{
//				PlayerForm.WaveFile = XPLO.CurPath;
				module_file_wave module = new module_file_wave(XPLO.CurPath);
			}
			else if ( fileExt==".akp" ) guiakp();
			// Akai Program Loader
			else if ( fileExt==".ds" ) gui_ds();
			// Impulse Tracker Instrument Loader
			else if ( fileExt==".iti" ) using (new ListHandler(ActionBefore<ITI>,ActionPost)) ActiveModule = new module_file_iti(XPLO.CurPath);
			// SoundFont Loader
			else if ( fileExt==".sf2" ) using (new ListHandler(ActionBefore<SoundFont2>,ActionPost)) ActiveModule = new module_file_soundfont(XPLO.CurPath);
			
			Common.lvsize(new ListView[]{lv,lva,lvb}, ColumnHeaderAutoResizeStyle.HeaderSize);
			
		}
		
		#endregion
		
		#region Obsolete
		
		/// <summary>
		/// This method is not referenced.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void eSplitterMoved0(object sender, SplitterEventArgs e)
		{
			Common.lvsize(new ListView[]{lva,lvb}, ColumnHeaderAutoResizeStyle.HeaderSize);
		}
		
		/// <summary>
		/// The idea would be to re-draw the GraphicsPanel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void eSplitterMoved1(object sender, EventArgs e) { TestGraphicsPanelBoundary(); }
		#endregion
		
		#region Form Loader Test
		
		void Event_ShowSineWaveTestForm(object sender, EventArgs e)
		{
			if (PlayerForm.IsDisposed)
				PlayerForm = new gen.snd.Forms.WaveOutTestForm();
			PlayerForm.Show(this);
		}
		
		void Event_BPM_Show(object sender, EventArgs e)
		{
			gen.snd.Forms.BpmCalculatorForm frm = new gen.snd.Forms.BpmCalculatorForm();
			frm.ShowDialog(this);
		}
		
		#endregion

		#region “Type Handlers”
		
		/// <summary>
		/// Attempt to load Akai Program to the provided lists.
		/// </summary>
		void guiakp()
		{
			XPLO.LV.Items.Clear();
			AKP.akpCK mod = new AKP.akpCK();
			
			int joe = System.Runtime.InteropServices.Marshal.SizeOf(mod);
			AKP	akf = new AKP(XPLO.CurPath);
			ListViewItem lvi = lv.Items.Add(Path.GetFileName(XPLO.CurPath),2);
			lvi.SubItems.AddRange(new string[]{
			                      	akf.AkpIO.ckLength.ToString("##,###,###,##0")
			                      });
			akf = null;
		}
		
		/// <summary>
		/// Provides limited support for DrumSynth to the CsShellFileView control.
		/// </summary>
		void gui_ds()
		{
			ds2cs ds = new ds2cs(XPLO.CurPath);
			XPLO.LoadDirectoryContent(System.IO.Directory.GetParent(XPLO.CurPath).FullName);
			Application.DoEvents();
			foreach (ListViewItem vi in XPLO.LV.Items)
			{
				if (vi.Text == Path.GetFileName(XPLO.CurPath))
				{
					vi.EnsureVisible();
					vi.Selected = true;
					vi.Focused = true;
				}
			}
			ds = null;
		}
		
		#endregion

		#region Miscelaneous (Graphics: Panel Render Test)
		void TestGraphicsPanelBoundary()
		{
			Bitmap B = new Bitmap( splitd.Panel2.ClientSize.Width, splitd.Panel2.ClientSize.Height, PixelFormat.Format24bppRgb );
			Graphics G = Graphics.FromImage(B);
//			int cw[] = new int[]{
//				Point(4,splitd.Panel2.ClientSize.Width - (4*2)),
//				Point(splitd.Panel2.ClientSize.Width - (4*2),splitd.Panel2.ClientSize.Height - (4*2))
//			};
			G.Clear(Color.White);
			G.DrawLines(
				new Pen(Color.FromArgb(0,0,0)),
				new Point[]{
					new Point(4,4),
					new Point(splitd.Panel2.ClientSize.Width - (4),4),
					new Point(splitd.Panel2.ClientSize.Width - (4),splitd.Panel2.ClientSize.Height - (4)),
					new Point(4,splitd.Panel2.ClientSize.Height - (4)),
					new Point(4,4)
				}
			);
			splitd.Panel2.BackgroundImage = B;
		}
		#endregion
		
		#region ComponentModel.Initialization
		void InitializeSpashScreen()
		{
			splash = new gen.snd.wave.views.SplashFormController(this,Images.genio2012,true);
			this.aboutToolStripMenuItem.Click += splash.Event_Splash_Show;
		}
		void PrepareExplo()
		{
			XPLO.LV.SuspendLayout();
			XPLO.TV.SuspendLayout();
			XPLO.TV.PreviewKeyDown += new PreviewKeyDownEventHandler(Event_ALT_KeyPressed);
			XPLO.LV.PreviewKeyDown += new PreviewKeyDownEventHandler(Event_ALT_KeyPressed);
			XPLO.TV.KeyUp += new KeyEventHandler(Event_ALT_KeyUp);
			XPLO.KeyUp += new KeyEventHandler(Event_ALT_KeyUp);
			XPLO.LV.Click += new EventHandler(Event_CsShellListView_ItemClicked);
			XPLO.LV.DoubleClick += new EventHandler(Event_PlaySound);
			XPLO.LV.ResumeLayout();
			XPLO.TV.ResumeLayout();
		}
		#endregion
		
		#region Overrides
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Common.lvsize(ref lv, ColumnHeaderAutoResizeStyle.HeaderSize);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (spx!=null) spx.Dispose();
			this.Show();
		}
		#endregion
		
		#region Design
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DspAudioTestForm));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
			this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
			this.menuMain = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openWaveFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.samplePlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.testFrequencyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splita = new System.Windows.Forms.SplitContainer();
			this.XPLO = new Windows.CommonControls.CsShellFileView();
			this.splitb = new System.Windows.Forms.SplitContainer();
			this.splitd = new System.Windows.Forms.SplitContainer();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.lv = new System.Windows.Forms.ListView();
			this.splitc = new System.Windows.Forms.SplitContainer();
			this.lva = new System.Windows.Forms.ListView();
			this.lvb = new System.Windows.Forms.ListView();
			this.waveControl1 = new GenericWAV.Views.WaveControl();
			this.miniToolStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
			this.menuMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splita)).BeginInit();
			this.splita.Panel1.SuspendLayout();
			this.splita.Panel2.SuspendLayout();
			this.splita.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitb)).BeginInit();
			this.splitb.Panel1.SuspendLayout();
			this.splitb.Panel2.SuspendLayout();
			this.splitb.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitd)).BeginInit();
			this.splitd.Panel1.SuspendLayout();
			this.splitd.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitc)).BeginInit();
			this.splitc.Panel1.SuspendLayout();
			this.splitc.Panel2.SuspendLayout();
			this.splitc.SuspendLayout();
			this.miniToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "agrad");
			this.imageList1.Images.SetKeyName(1, "agreen");
			this.imageList1.Images.SetKeyName(2, "aorange");
			this.imageList1.Images.SetKeyName(3, "apurple");
			this.imageList1.Images.SetKeyName(4, "ared");
			this.imageList1.Images.SetKeyName(5, "inst");
			this.imageList1.Images.SetKeyName(6, "amagenta");
			this.imageList1.Images.SetKeyName(7, "acyan");
			this.imageList1.Images.SetKeyName(8, "bgreen");
			this.imageList1.Images.SetKeyName(9, "byellow");
			// 
			// BottomToolStripPanel
			// 
			this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.BottomToolStripPanel.Name = "BottomToolStripPanel";
			this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// TopToolStripPanel
			// 
			this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.TopToolStripPanel.Name = "TopToolStripPanel";
			this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// RightToolStripPanel
			// 
			this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.RightToolStripPanel.Name = "RightToolStripPanel";
			this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// LeftToolStripPanel
			// 
			this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
			this.LeftToolStripPanel.Name = "LeftToolStripPanel";
			this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
			// 
			// ContentPanel
			// 
			this.ContentPanel.AutoScroll = true;
			this.ContentPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.ContentPanel.Size = new System.Drawing.Size(882, 436);
			// 
			// menuMain
			// 
			this.menuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.helpToolStripMenuItem,
			this.toolsToolStripMenuItem});
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Padding = new System.Windows.Forms.Padding(0);
			this.menuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuMain.Size = new System.Drawing.Size(1225, 24);
			this.menuMain.TabIndex = 2;
			this.menuMain.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.openWaveFormToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 24);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openWaveFormToolStripMenuItem
			// 
			this.openWaveFormToolStripMenuItem.Name = "openWaveFormToolStripMenuItem";
			this.openWaveFormToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
			this.openWaveFormToolStripMenuItem.Text = "Open Wave-Form";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
			this.aboutToolStripMenuItem.Text = "&About";
			// 
			// toolsToolStripMenuItem
			// 
			this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.samplePlayerToolStripMenuItem,
			this.bMPToolStripMenuItem,
			this.testButtonToolStripMenuItem,
			this.testFrequencyToolStripMenuItem});
			this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
			this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 24);
			this.toolsToolStripMenuItem.Text = "Tools";
			// 
			// samplePlayerToolStripMenuItem
			// 
			this.samplePlayerToolStripMenuItem.Name = "samplePlayerToolStripMenuItem";
			this.samplePlayerToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.samplePlayerToolStripMenuItem.Text = "Sine Wave Test";
			this.samplePlayerToolStripMenuItem.Click += new System.EventHandler(this.Event_ShowSineWaveTestForm);
			// 
			// bMPToolStripMenuItem
			// 
			this.bMPToolStripMenuItem.Name = "bMPToolStripMenuItem";
			this.bMPToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.bMPToolStripMenuItem.Text = "BPM";
			this.bMPToolStripMenuItem.Click += new System.EventHandler(this.Event_BPM_Show);
			// 
			// testButtonToolStripMenuItem
			// 
			this.testButtonToolStripMenuItem.Name = "testButtonToolStripMenuItem";
			this.testButtonToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.testButtonToolStripMenuItem.Text = "Test Panel Rendering";
			this.testButtonToolStripMenuItem.Click += new System.EventHandler(this.eSplitterMoved1);
			// 
			// testFrequencyToolStripMenuItem
			// 
			this.testFrequencyToolStripMenuItem.Name = "testFrequencyToolStripMenuItem";
			this.testFrequencyToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
			this.testFrequencyToolStripMenuItem.Text = "Test Frequency";
			this.testFrequencyToolStripMenuItem.Click += new System.EventHandler(this.Event_TestFreq);
			// 
			// splita
			// 
			this.splita.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.splita.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splita.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splita.Location = new System.Drawing.Point(0, 24);
			this.splita.Name = "splita";
			// 
			// splita.Panel1
			// 
			this.splita.Panel1.Controls.Add(this.XPLO);
			// 
			// splita.Panel2
			// 
			this.splita.Panel2.Controls.Add(this.splitb);
			this.splita.Panel2.Controls.Add(this.waveControl1);
			this.splita.Size = new System.Drawing.Size(1225, 647);
			this.splita.SplitterDistance = 270;
			this.splita.TabIndex = 3;
			// 
			// XPLO
			// 
			this.XPLO.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this.XPLO.Dock = System.Windows.Forms.DockStyle.Fill;
			this.XPLO.DOPROC = false;
			this.XPLO.Filter = null;
			this.XPLO.Location = new System.Drawing.Point(0, 0);
			this.XPLO.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.XPLO.ModelIconSize = w32.shell.SHGFI.SMALLICON;
			this.XPLO.Name = "XPLO";
			this.XPLO.Size = new System.Drawing.Size(270, 647);
			this.XPLO.TabIndex = 0;
			// 
			// splitb
			// 
			this.splitb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitb.Font = new System.Drawing.Font("Ubuntu Mono", 12F, System.Drawing.FontStyle.Bold);
			this.splitb.Location = new System.Drawing.Point(0, 0);
			this.splitb.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
			this.splitb.Name = "splitb";
			this.splitb.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitb.Panel1
			// 
			this.splitb.Panel1.Controls.Add(this.splitd);
			// 
			// splitb.Panel2
			// 
			this.splitb.Panel2.Controls.Add(this.splitc);
			this.splitb.Size = new System.Drawing.Size(951, 556);
			this.splitb.SplitterDistance = 257;
			this.splitb.TabIndex = 2;
			// 
			// splitd
			// 
			this.splitd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitd.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitd.Location = new System.Drawing.Point(0, 0);
			this.splitd.Name = "splitd";
			this.splitd.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitd.Panel1
			// 
			this.splitd.Panel1.Controls.Add(this.splitContainer1);
			// 
			// splitd.Panel2
			// 
			this.splitd.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitd.Panel2Collapsed = true;
			this.splitd.Size = new System.Drawing.Size(951, 257);
			this.splitd.SplitterDistance = 88;
			this.splitd.SplitterWidth = 3;
			this.splitd.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.lv);
			this.splitContainer1.Size = new System.Drawing.Size(951, 257);
			this.splitContainer1.SplitterDistance = 538;
			this.splitContainer1.TabIndex = 3;
			// 
			// treeView1
			// 
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(538, 257);
			this.treeView1.TabIndex = 0;
			// 
			// lv
			// 
			this.lv.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lv.FullRowSelect = true;
			this.lv.LargeImageList = this.imageList1;
			this.lv.Location = new System.Drawing.Point(0, 0);
			this.lv.MultiSelect = false;
			this.lv.Name = "lv";
			this.lv.Size = new System.Drawing.Size(409, 257);
			this.lv.SmallImageList = this.imageList1;
			this.lv.TabIndex = 2;
			this.lv.UseCompatibleStateImageBehavior = false;
			this.lv.View = System.Windows.Forms.View.Details;
			// 
			// splitc
			// 
			this.splitc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitc.Location = new System.Drawing.Point(0, 0);
			this.splitc.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
			this.splitc.Name = "splitc";
			// 
			// splitc.Panel1
			// 
			this.splitc.Panel1.Controls.Add(this.lva);
			// 
			// splitc.Panel2
			// 
			this.splitc.Panel2.Controls.Add(this.lvb);
			this.splitc.Size = new System.Drawing.Size(951, 295);
			this.splitc.SplitterDistance = 488;
			this.splitc.TabIndex = 0;
			// 
			// lva
			// 
			this.lva.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lva.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lva.FullRowSelect = true;
			this.lva.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lva.Location = new System.Drawing.Point(0, 0);
			this.lva.Name = "lva";
			this.lva.Size = new System.Drawing.Size(488, 295);
			this.lva.SmallImageList = this.imageList1;
			this.lva.TabIndex = 1;
			this.lva.UseCompatibleStateImageBehavior = false;
			this.lva.View = System.Windows.Forms.View.Details;
			// 
			// lvb
			// 
			this.lvb.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lvb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvb.FullRowSelect = true;
			this.lvb.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvb.Location = new System.Drawing.Point(0, 0);
			this.lvb.Name = "lvb";
			this.lvb.Size = new System.Drawing.Size(459, 295);
			this.lvb.SmallImageList = this.imageList1;
			this.lvb.TabIndex = 1;
			this.lvb.UseCompatibleStateImageBehavior = false;
			this.lvb.View = System.Windows.Forms.View.Details;
			// 
			// waveControl1
			// 
			this.waveControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.waveControl1.Location = new System.Drawing.Point(0, 556);
			this.waveControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.waveControl1.Name = "waveControl1";
			this.waveControl1.Size = new System.Drawing.Size(951, 91);
			this.waveControl1.TabIndex = 3;
			// 
			// miniToolStrip
			// 
			this.miniToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripDropDownButton1});
			this.miniToolStrip.Location = new System.Drawing.Point(0, 671);
			this.miniToolStrip.Name = "miniToolStrip";
			this.miniToolStrip.Padding = new System.Windows.Forms.Padding(1, 0, 15, 0);
			this.miniToolStrip.Size = new System.Drawing.Size(1225, 22);
			this.miniToolStrip.TabIndex = 4;
			// 
			// toolStripDropDownButton1
			// 
			this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
			this.toolStripDropDownButton1.Size = new System.Drawing.Size(42, 20);
			this.toolStripDropDownButton1.Text = "play";
			this.toolStripDropDownButton1.Click += new System.EventHandler(this.Event_PlaySound);
			// 
			// DspAudioTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1225, 693);
			this.Controls.Add(this.splita);
			this.Controls.Add(this.miniToolStrip);
			this.Controls.Add(this.menuMain);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Ubuntu Mono", 11F, System.Drawing.FontStyle.Bold);
			this.MinimumSize = new System.Drawing.Size(582, 378);
			this.Name = "DspAudioTestForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "genericwav";
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Event_ALT_KeyUp);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Event_ALT_KeyPressed);
			this.menuMain.ResumeLayout(false);
			this.menuMain.PerformLayout();
			this.splita.Panel1.ResumeLayout(false);
			this.splita.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splita)).EndInit();
			this.splita.ResumeLayout(false);
			this.splitb.Panel1.ResumeLayout(false);
			this.splitb.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitb)).EndInit();
			this.splitb.ResumeLayout(false);
			this.splitd.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitd)).EndInit();
			this.splitd.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitc.Panel1.ResumeLayout(false);
			this.splitc.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitc)).EndInit();
			this.splitc.ResumeLayout(false);
			this.miniToolStrip.ResumeLayout(false);
			this.miniToolStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private GenericWAV.Views.WaveControl waveControl1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem testFrequencyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem testButtonToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem samplePlayerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripContentPanel ContentPanel;
		private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
		private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
		private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
		private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
		private System.Windows.Forms.ToolStripMenuItem bMPToolStripMenuItem;

		private System.Windows.Forms.SplitContainer splitd;
		private System.Windows.Forms.SplitContainer splita;
		private System.Windows.Forms.SplitContainer splitb;
		private System.Windows.Forms.SplitContainer splitc;
		private System.Windows.Forms.ListView lva;
		private System.Windows.Forms.ListView lvb;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuMain;
		private System.Windows.Forms.ToolStripMenuItem openWaveFormToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
		private System.Windows.Forms.StatusStrip miniToolStrip;
		private System.Windows.Forms.ListView lv;
		private Windows.CommonControls.CsShellFileView XPLO;
		#endregion
		
		gen.snd.Forms.FrequencyTestForm FreqTester;
		void Event_TestFreq(object sender, EventArgs e)
		{
			using (FreqTester = new gen.snd.Forms.FrequencyTestForm())
				FreqTester.ShowDialog(this);
		}
	}
}
