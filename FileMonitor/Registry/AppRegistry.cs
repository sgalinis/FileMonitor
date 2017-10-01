using FileMonitor.Models;
using FileMonitor.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Registry
{
	public static class AppRegistry
	{
		//assuming the application is in portable mode with read-write permissions in the executable directory
		//non portable will return common app data folder
		private static bool isPortable = true;
		private static readonly string _settingsFileName = "settings.xml";

		private static string GetSettingsDir()
		{
			if(isPortable)
				return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
			else
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "FileMonitor");
		}

		public static AppSettings LoadSettings()
		{
			AppSettings appSettings = new AppSettings();			
			string fileName = Path.Combine(GetSettingsDir(), _settingsFileName);
			if(File.Exists(fileName))
			{
				DataSerializer<AppSettings> ds = new DataSerializer<AppSettings>();
				appSettings = ds.LoadFromXmlFile(fileName);
			}
			return appSettings;
		}

		public static void SaveSettings(AppSettings appSettings)
		{			
			string fileName = Path.Combine(GetSettingsDir(), _settingsFileName);
			DataSerializer<AppSettings> ds = new DataSerializer<AppSettings>();
			ds.SaveToXmlFile(fileName, appSettings);			
		}

	}//class
}
