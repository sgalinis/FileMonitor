using FileMonitor.Shared;
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
	public class QueueTaskWorker: IDisposable
	{
		private static readonly UniqueQueue<FileWatcherEventArgs> _uniqueQueue = new UniqueQueue<FileWatcherEventArgs>();
		private static readonly Object _lockObject = new Object();

		private static FileWatcherParams _fileWatcherParams;

		private Thread _queueProcessThread;
		private static bool _stopQueue;

		public QueueTaskWorker()
		{						
		}

		public void AddToQueue(FileWatcherEventArgs fileWatcherEventArgs)
		{
			lock(_lockObject)
			{
				_uniqueQueue.Enqueue(fileWatcherEventArgs);
			}
		}

		public void Start(FileWatcherParams fileWatcherParams)
		{
			_fileWatcherParams = fileWatcherParams;
			_stopQueue = false;
			_queueProcessThread = new Thread(new ThreadStart(ProcessQueue));
			_queueProcessThread.Start();
		}
		
		public void Stop()
		{
			_stopQueue = true;
			_queueProcessThread.Join();
		}
		
		private static void ProcessQueue()
		{						
			while(_stopQueue == false)
			{
				int count = 0;
				FileWatcherEventArgs fileWatcherEventArgs = null;
				lock(_lockObject)
				{
					count = _uniqueQueue.Count;
				}

				while(_stopQueue == false && count > 0)
				{
					fileWatcherEventArgs = _uniqueQueue.Peek();
					string sourceFileName = Path.Combine(_fileWatcherParams.InputDir, Path.GetFileName(fileWatcherEventArgs.FileName));
					string destFileName = Path.Combine(_fileWatcherParams.OutputDir, Path.GetFileName(fileWatcherEventArgs.FileName));

					Task task = CopyFileEx(sourceFileName, destFileName);
					task.Wait();

					lock(_lockObject)
					{
						_uniqueQueue.Dequeue();
						count = _uniqueQueue.Count;
						Debug.WriteLine($"{Path.GetFileName(fileWatcherEventArgs.FileName)}, Queue items: {count}");
					}
				}
				Thread.Sleep(100);
			}			
		}

		private static async Task CopyFileEx(string sourceFileName, string destFileName)
		{
			//await Task.Delay(1000);

			while(!TryCopyFile(sourceFileName, destFileName))
			{
				await Task.Delay(100);
			}
		}

		private static bool TryCopyFile(string sourceFileName, string destFileName)
		{
			try
			{
				File.Copy(sourceFileName, destFileName, true);
			}
			catch(IOException)
			{
				return false;
			}
			return true;
		}

		public void Dispose()
		{
			_stopQueue = true;
			if(_queueProcessThread.IsAlive == true)
			{
				_queueProcessThread.Join();
			}
		}
	}
}
