using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Shared
{
	public class FileWatcherEventArgs: EventArgs
	{
		public string FileName { get; set; }
		public string EventName { get; set; }
		public DateTime EventTime { get; set; }
	}
}
