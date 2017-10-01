using FileMonitor.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Utils
{
	public class TextFileLogger : IDisposable
	{
		private StreamWriter _writer;
		private int _flushCount;


		public TextFileLogger(string logFileName)
		{
			_flushCount = 0;			
			_writer = File.AppendText(logFileName);
		}
		
		public void WriteLine(string logMessage)
		{			
			_writer.WriteLine(logMessage);
			++_flushCount;
			if(_flushCount % 10 == 0)
				_writer.Flush();
		}		
		
		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			_writer.Close();
		}

	}
}
