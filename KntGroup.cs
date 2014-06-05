/*
TFOOO - With SharpDevelop2
 */
using System;
using System.Runtime.InteropServices;

namespace xplo
{
	[StructLayout(LayoutKind.Sequential)]
	public struct KntGroup
	{
		public string Title;
		public string Path;
		public string Type;
		public string Filter;
		public bool IncSubDirs;
		public string FileTemplate;
		public string FileOut;
	}
}
