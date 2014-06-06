#region User/License
// oio * 8/15/2012 * 10:36 AM

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

namespace gen.snd
{
	/// <summary>
	/// Description of WaveControl.
	/// </summary>
	public partial class WaveControl : UserControl
	{
		static readonly float[] resolutions = new float[]{
			0.00000f, 0.11111f, 0.25000f,
			0.33333f, 0.44444f, 0.50000f,
			0.66667f, 0.75000f, 0.86667f
		};
		static float[] ZoomTable()
		{
			return ZoomTable(9);
		}
		static float[] ZoomTable(int iterations)
		{
			int count=9*iterations;
			float[] values = new float[9*iterations-1];
			for (int i = 0; i < count; i++)
			{
				if (i > 0) values[i-1] = resolutions[i % iterations] + (i / iterations);
			}
			return values;
		}
		public WaveControl()
		{
			InitializeComponent();
			this.comboBox1.DataSource = ZoomTable();
			this.comboBox1.SelectedIndex = 8;
			
		}
	}
}
