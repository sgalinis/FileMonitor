using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileMonitor.Utils
{
	public static class FileUtils
	{
		public static bool FilesAreEqual(string first, string second)
		{
			byte[] firstHash;
			using(var md5 = MD5.Create())
			{
				using(var stream = File.OpenRead(first))
				{
					firstHash = md5.ComputeHash(stream);
				}
			}

			byte[] secondHash;
			using(var md5 = MD5.Create())
			{
				using(var stream = File.OpenRead(second))
				{
					secondHash = md5.ComputeHash(stream);
				}
			}

			for(int i = 0; i < firstHash.Length; i++)
			{
				if(firstHash[i] != secondHash[i])
					return false;
			}
			return true;			
		}
	}	
}
