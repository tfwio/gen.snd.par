#region User/License
// oio * 8/15/2012 * 4:08 AM

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
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using gen.snd.IffForm;
using GraphicsUnit=System.Cor3.Drawing.DblUnit;

namespace gen.snd
{
	
	/// <summary>
	/// Description of WaveFormControl.
	/// </summary>
	public class WaveFormControl : UserControl
	{
		protected override bool ShowFocusCues {
			get { return true; }
		}
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Debug.Print("GotFocus: HasFocus={0}",Focused);
		}
		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Debug.Print("LostFocus: HasFocus={0}",Focused);
		}
		
		class WaveCalculations
		{
			static public TimeSpan GetWaveLength(WaveFormat format, int bitLength)
			{
				return TimeSpan.FromSeconds(format.fmtRate / (bitLength / format.fmtBPSmp));
			}
		}
		public class BufferedGraphic<TControl>
			where TControl:UserControl
		{
			TControl DrawingControl;
			GraphicsUnit DrawingSpace;
			
			static readonly Color DefaultBackgroundColor=Color.Black, DefaultForegroundColor=Color.White;
			static readonly float DefaultPenWidth = 1;
			Color ForegroundColor= Color.White;
			Brush ForegroundBrush = new SolidBrush(DefaultForegroundColor);
			Pen		ForegroundPen		= new Pen(DefaultForegroundColor,DefaultPenWidth);
			
			Color BackgroundColor = Color.Black;
			Brush BackgroundBrush = new SolidBrush(DefaultBackgroundColor);
			Pen		BackgroundPen		= new Pen(DefaultBackgroundColor,DefaultPenWidth);
			
//		Image BackgroundBuffer, ForegroundBuffer;
			FloatRect Boundary = Rectangle.Empty;
			
			public void DrawBackground(Graphics g)
			{
				g.Clear(BackgroundColor);
				RoundRectRenderer r = new RoundRectRenderer(DrawingControl.Bounds, 4f, 0.5f);
				using (LinearGradientBrush lgb = new LinearGradientBrush(DrawingControl.Bounds,Color.Black,Color.Gray,LinearGradientMode.Vertical))
					RoundRectRenderer.Fill(r,g,lgb);
				r = null;
			}
			public BufferedGraphic(TControl ctl)
			{
				this.DrawingControl = ctl;
			}
		}
		
		public bool HasBackground;
		public bool HasForeground;
		
		public WaveFormControl()
		{
			this.DoubleBuffered = true;
			this.InitializeComponent();
			bg = new BufferedGraphic<WaveFormControl>(this);
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			Invalidate();
		}
		
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.RenderClock = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// RenderClock
			// 
			this.RenderClock.Interval = 500;
			this.RenderClock.Tick += new System.EventHandler(this.RendererWake);
			// 
			// WaveFormControl
			// 
			this.Name = "WaveFormControl";
			this.Size = new System.Drawing.Size(646, 151);
			this.ResumeLayout(false);
		}
		
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Timer RenderClock;
		
		/// <summary>
		/// Start the render timer;  if forcerepaint, ignore render timer
		/// and go to render process;
		/// </summary>
		/// <param name="forceRepaint"></param>
		void PreRenderer(bool forceRepaint)
		{
			
		}
		BufferedGraphic<WaveFormControl> bg;
		void Paint(Graphics gfx)
		{
			bg.DrawBackground(gfx);
		}
		
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Debug.Print("Trying to paint");
			try {
				Paint(e.Graphics);
			} catch (Exception ex) {
				Debug.Print("{0}",ex);
			}
		}
		void RendererWake(object sender, EventArgs e)
		{
			
		}
	}
}
