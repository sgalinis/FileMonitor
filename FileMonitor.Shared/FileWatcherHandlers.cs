using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Shared
{
	public delegate void FileProcessHandler(Object sender, FileWatcherEventArgs e);
}
