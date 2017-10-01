using FileMonitor.Shared;
using FileMonitor.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileMonitor.FileWatchEngine
{
	public class FileWatcher: IFileWatcher, IDisposable
	{
		private readonly FileSystemWatcher _watcher;		

		private TextFileLogger _logger;
		private FileWatcherParams _watcherParams;
		private QueueTaskWorker _queueTaskWorker;

		public event FileProcessHandler OnFileProcess;

		public FileWatcher()
		{
            _watcher = new FileSystemWatcher();
			_queueTaskWorker = new QueueTaskWorker();
		}

        public void Dispose()
        {
			if(_watcher != null)
				_watcher.Dispose();
            if(_logger != null)
				_logger.Dispose();
        }

        public void Start(FileWatcherParams watcherParams)
		{			
			_watcherParams = watcherParams;
			_watcher.Path = _watcherParams.InputDir;
			_watcher.Filter = _watcherParams.FileFilter;
			_watcher.IncludeSubdirectories = false;

			_watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
					 | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;

			_watcher.Created += new FileSystemEventHandler(OnCreated);
			_watcher.Changed += new FileSystemEventHandler(OnChanged);
			_watcher.Deleted += new FileSystemEventHandler(OnDeleted);
			
			Directory.CreateDirectory(_watcherParams.OutputDir);
			Directory.CreateDirectory(Path.GetDirectoryName(_watcherParams.LogFileName));
			_logger = new TextFileLogger(_watcherParams.LogFileName);
			
			_queueTaskWorker.Start(_watcherParams);
			_watcher.EnableRaisingEvents = true;			
		}		

		public void Stop()
		{
			_watcher.EnableRaisingEvents = false;
			_queueTaskWorker.Stop();
			_logger.Close();
		}

		public bool IsActive()
		{
			return _watcher.EnableRaisingEvents;
		}

		protected void OnCreated(object sender, FileSystemEventArgs e)
		{
			ProcessFileWatcherEvent(e);
		}
	
		private void OnChanged(object sender, FileSystemEventArgs e)
		{
			string destFileName = Path.Combine(_watcherParams.OutputDir, e.Name);
			if(File.Exists(destFileName))
			{
				ProcessFileWatcherEvent(e);				
			}			
		}

		private void OnDeleted(object sender, FileSystemEventArgs e)
		{
			ProcessFileWatcherEvent(e);
		}

		private void ProcessFileWatcherEvent(FileSystemEventArgs e)
		{			
            FileWatcherEventArgs args = new FileWatcherEventArgs();
			args.FileName = e.Name;
			args.EventName = e.ChangeType.ToString();
			args.EventTime = DateTime.Now;
            OnFileProcess?.Invoke(this, args);
            //
            string time_string =
				String.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}:{6:D4}",
				args.EventTime.Year, args.EventTime.Month, args.EventTime.Day, args.EventTime.Hour, args.EventTime.Minute, args.EventTime.Second, args.EventTime.Millisecond);
			string s = String.Format("{0,-24} - {1,-10} - {2,-255}", time_string, args.EventName, args.FileName);
			_logger.WriteLine(s);						
			//
			string destFileName = Path.Combine(_watcherParams.OutputDir, e.Name);
			string sourceFileName = e.FullPath;
            //
			if(!File.Exists(destFileName) && e.ChangeType == WatcherChangeTypes.Created)
			{
				_queueTaskWorker.AddToQueue(args);
			}
			else if(File.Exists(destFileName) && e.ChangeType == WatcherChangeTypes.Deleted)
			{
				File.Delete(destFileName);
			}
			else if(File.Exists(destFileName) && e.ChangeType == WatcherChangeTypes.Changed)
			{
				_queueTaskWorker.AddToQueue(args);
			}
		}		

	}
}
