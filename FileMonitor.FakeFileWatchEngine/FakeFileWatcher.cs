using FileMonitor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.FakeFileWatchEngine
{
	public class FakeFileWatcher : IFileWatcher
	{
		private bool _isActive;

		public FakeFileWatcher()
		{
			_isActive = false;
		}

		public event FileProcessHandler OnFileProcess;

		public void Start(FileWatcherParams watcherParams)
		{
			_isActive = true;
			SendNotification();
		}

		public void Stop()
		{
			_isActive = false;
		}
		
		public bool IsActive()
		{
			return _isActive;
		}

		private void SendNotification()
		{
			for(int i = 1; i <= 20; i++)
			{
				FileWatcherEventArgs args = new FileWatcherEventArgs();
				args.FileName = i.ToString().PadLeft(4, '0') + ".xml";
				args.EventName = (i % 2 == 0) ? "Changed" : "Created";
				args.EventTime = DateTime.Now;
				//
				OnFileProcess?.Invoke(this, args);
			}			
		}

	}//class
}
