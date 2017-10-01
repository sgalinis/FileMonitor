using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileMonitor.Utils
{
	public class DataSerializer<T> where T : new()
	{
		public void SaveToXmlFile(string XmlFileName, T t)
		{
			try
			{
				XmlSerializer xml = new XmlSerializer(typeof(T));
				using(FileStream fstream = new FileStream(XmlFileName, FileMode.Create, FileAccess.Write, FileShare.None))
				{
					xml.Serialize(fstream, t);
					fstream.Close();
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}

		public T LoadFromXmlFile(string XmlFileName)
		{
			T t = new T();
			try
			{
				XmlSerializer xml = new XmlSerializer(typeof(T));
				using(FileStream fstream = new FileStream(XmlFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					t = (T)xml.Deserialize(fstream);
					fstream.Close();
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
			return t;
		}

	}//class
}