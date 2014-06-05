#region User/License
// oio * 7/26/2012 * 11:05 PM

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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace GenericWAV.Views
{
	/// <summary>
	/// Description of DspAudioTestControl.
	/// </summary>
	public partial class DspAudioTestControl : UserControl
	{
		public DspAudioTestControl()
		{
			InitializeComponent();
		}
		
		#region Designer
		
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
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
			this.splitb = new System.Windows.Forms.SplitContainer();
			this.splitd = new System.Windows.Forms.SplitContainer();
			this.lv = new System.Windows.Forms.ListView();
			this.splitc = new System.Windows.Forms.SplitContainer();
			this.lva = new System.Windows.Forms.ListView();
			this.lvb = new System.Windows.Forms.ListView();
			this.splitb.Panel1.SuspendLayout();
			this.splitb.Panel2.SuspendLayout();
			this.splitb.SuspendLayout();
			this.splitd.Panel1.SuspendLayout();
			this.splitd.SuspendLayout();
			this.splitc.Panel1.SuspendLayout();
			this.splitc.Panel2.SuspendLayout();
			this.splitc.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitb
			// 
			this.splitb.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitb.Location = new System.Drawing.Point(0, 0);
			this.splitb.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
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
			this.splitb.Size = new System.Drawing.Size(682, 427);
			this.splitb.SplitterDistance = 189;
			this.splitb.SplitterWidth = 2;
			this.splitb.TabIndex = 3;
			// 
			// splitd
			// 
			this.splitd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitd.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitd.Location = new System.Drawing.Point(0, 0);
			this.splitd.Margin = new System.Windows.Forms.Padding(2);
			this.splitd.Name = "splitd";
			this.splitd.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitd.Panel1
			// 
			this.splitd.Panel1.Controls.Add(this.lv);
			// 
			// splitd.Panel2
			// 
			this.splitd.Panel2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.splitd.Panel2Collapsed = true;
			this.splitd.Size = new System.Drawing.Size(682, 189);
			this.splitd.SplitterDistance = 88;
			this.splitd.SplitterWidth = 2;
			this.splitd.TabIndex = 0;
			// 
			// lv
			// 
			this.lv.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lv.FullRowSelect = true;
			this.lv.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lv.Location = new System.Drawing.Point(0, 0);
			this.lv.Margin = new System.Windows.Forms.Padding(2);
			this.lv.MultiSelect = false;
			this.lv.Name = "lv";
			this.lv.Size = new System.Drawing.Size(682, 189);
			this.lv.TabIndex = 2;
			this.lv.UseCompatibleStateImageBehavior = false;
			this.lv.View = System.Windows.Forms.View.Details;
			// 
			// splitc
			// 
			this.splitc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitc.Location = new System.Drawing.Point(0, 0);
			this.splitc.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
			this.splitc.Name = "splitc";
			// 
			// splitc.Panel1
			// 
			this.splitc.Panel1.Controls.Add(this.lva);
			// 
			// splitc.Panel2
			// 
			this.splitc.Panel2.Controls.Add(this.lvb);
			this.splitc.Size = new System.Drawing.Size(682, 236);
			this.splitc.SplitterDistance = 343;
			this.splitc.SplitterWidth = 1;
			this.splitc.TabIndex = 0;
			// 
			// lva
			// 
			this.lva.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lva.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lva.FullRowSelect = true;
			this.lva.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lva.Location = new System.Drawing.Point(0, 0);
			this.lva.Margin = new System.Windows.Forms.Padding(2);
			this.lva.Name = "lva";
			this.lva.Size = new System.Drawing.Size(343, 236);
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
			this.lvb.Margin = new System.Windows.Forms.Padding(2);
			this.lvb.Name = "lvb";
			this.lvb.Size = new System.Drawing.Size(338, 236);
			this.lvb.TabIndex = 1;
			this.lvb.UseCompatibleStateImageBehavior = false;
			this.lvb.View = System.Windows.Forms.View.Details;
			// 
			// DspAudioTestControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitb);
			this.Name = "DspAudioTestControl";
			this.Size = new System.Drawing.Size(682, 427);
			this.splitb.Panel1.ResumeLayout(false);
			this.splitb.Panel2.ResumeLayout(false);
			this.splitb.ResumeLayout(false);
			this.splitd.Panel1.ResumeLayout(false);
			this.splitd.ResumeLayout(false);
			this.splitc.Panel1.ResumeLayout(false);
			this.splitc.Panel2.ResumeLayout(false);
			this.splitc.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ListView lvb;
		private System.Windows.Forms.ListView lva;
		private System.Windows.Forms.SplitContainer splitc;
		private System.Windows.Forms.ListView lv;
		private System.Windows.Forms.SplitContainer splitd;
		private System.Windows.Forms.SplitContainer splitb;
		#endregion
		
	}
}
