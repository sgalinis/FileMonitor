using FileMonitor.Common;
using FileMonitor.Models;
using FileMonitor.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileMonitor.Presentation.ViewModels
{
	public class FileMonitorViewModel: Notifier
	{
		private readonly IFileWatcher _fileWatcher;
		private readonly AppSettings _appSettings;		

		public FileMonitorViewModel(IFileWatcher fileWatcher, AppSettings appSettings)
		{
			_fileWatcher = fileWatcher;
			_appSettings = appSettings;
			//
			_infoItems = new ObservableCollection<string>();			
			_startCmd = new RelayCommand(Start, CanStart);
			_stopCmd = new RelayCommand(Stop, CanStop);
			
			//setup design time data
#if DEBUG
			if(DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject()))
			{
				InputDir = @"D:\Temp\FileWatch";
				OutputDir = @"D:\Temp\FileWatchOutput";
				//
				_infoItems.Add("2017-07-18 17:44:53:0827 - Created    - 2836SC.XML");
				_infoItems.Add("2017-07-18 17:45:49:0892 - Created    - 2827SP.XML");
				_infoItems.Add("2017-07-18 17:45:49:0892 - Created    - 2828AC.XML");
				_infoItems.Add("2017-07-18 17:46:00:0436 - Changed    - 2836SC.XML");
				_infoItems.Add("2017-07-18 17:46:00:0443 - Created    - 2837SC.XML");
			}
#endif
		}
		
		public string InputDir
		{
			get { return _appSettings.InputDir; }
			set
			{
				_appSettings.InputDir = value;
				OnPropertyChanged("InputDir");
			}
		}
		
		public string OutputDir
		{
			get { return _appSettings.OutputDir; }
			set
			{
				_appSettings.OutputDir = value;
				OnPropertyChanged("OutputDir");
			}
		}

		private readonly ObservableCollection<string> _infoItems;
		public ObservableCollection<string> InfoItems { get { return _infoItems; } }

		private readonly ICommand _startCmd;
		public ICommand StartCmd { get { return _startCmd; } }

		private bool CanStart(object obj)
		{
			if(!String.IsNullOrWhiteSpace(InputDir) && !String.IsNullOrWhiteSpace(OutputDir) && !_fileWatcher.IsActive())
				return true;
			return false;
		}

		private void Start(object obj)
		{
			FileWatcherParams p = new FileWatcherParams();
			p.InputDir = InputDir;
			p.OutputDir = OutputDir;
			p.LogFileName = System.IO.Path.Combine(OutputDir, "Logs", _appSettings.LogFileName);
			_fileWatcher.OnFileProcess += _fileWatcher_OnFileProcess;
			_fileWatcher.Start(p);
		}

		private readonly ICommand _stopCmd;
		public ICommand StopCmd { get { return _stopCmd; } }

		private bool CanStop(object obj)
		{
			return _fileWatcher.IsActive();
		}

		private void Stop(object obj)
		{
			_fileWatcher.Stop();
		}

		private readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

		private void _fileWatcher_OnFileProcess(object sender, FileWatcherEventArgs e)
		{
			_syncContext.Post(o => AddInfoLine(e.FileName, e.EventName, e.EventTime), null);
		}

		private void AddInfoLine(string fileName, string eventName, DateTime eventTime)
		{			
			string time_string = 
				String.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}:{6:D4}", 
				eventTime.Year, eventTime.Month, eventTime.Day, eventTime.Hour, eventTime.Minute, eventTime.Second, eventTime.Millisecond);
			string s = String.Format("{0,-24} - {1,-10} - {2,-255}", time_string, eventName, fileName);
			_infoItems.Add(s);
			SelectedItem = s;
		}

		private string _selectedItem;
		public string SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged("SelectedItem");
			}
		}

	}//class
}
