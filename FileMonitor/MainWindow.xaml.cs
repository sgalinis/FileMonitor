using FileMonitor.Models;
using FileMonitor.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileMonitor
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			//
			this.Title = "Directory Watcher";
			mainGrid.ShowGridLines = false;
			//
			//boostrapping, dependency injection in main window because the application is implemented as a single window
			Shared.IFileWatcher fileWatcher = new FileWatchEngine.FileWatcher();			
			AppSettings appSettings = Registry.AppRegistry.LoadSettings();
			this.DataContext = new FileMonitorViewModel(fileWatcher, appSettings);
			this.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
		}

   private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
		{
			FileMonitorViewModel vm = this.DataContext as FileMonitorViewModel;
			if(vm != null)
			{
				AppSettings appSettings = new AppSettings();
				appSettings.InputDir = vm.InputDir;
				appSettings.OutputDir = vm.OutputDir;
				Registry.AppRegistry.SaveSettings(appSettings);
			}

			IDisposable disposable = this.DataContext as IDisposable;
			if(disposable != null)
				disposable.Dispose();
		}
	}
}
