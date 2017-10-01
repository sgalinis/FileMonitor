using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Shared
{
	public class FileWatcherParams
	{
		public string InputDir { get; set; }
		public string OutputDir { get; set; }
		public string FileFilter { get; set; }
		public string LogFileName { get; set; }
	}
}
