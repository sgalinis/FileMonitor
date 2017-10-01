using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Shared
{
	public interface IFileWatcher
	{		
		event FileProcessHandler OnFileProcess;
		void Start(FileWatcherParams watcherParams);
		void Stop();
		bool IsActive();
	}
}
