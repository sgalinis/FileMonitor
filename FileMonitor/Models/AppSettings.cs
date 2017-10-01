using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Models
{
	public class AppSettings
	{
		public string InputDir { get; set; }
		public string OutputDir { get; set; }
		public string LogFileName { get; set; }

		public AppSettings()
		{
			LogFileName = "FileEventLog.log";
		}
	}
}
