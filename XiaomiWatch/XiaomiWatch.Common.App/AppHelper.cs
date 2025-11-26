using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XiaomiWatch.Common.App
{
	public class AppHelper
	{
		private static int? appCount;

		public static int GetAppCount()
		{
			if (string.IsNullOrEmpty(PathHelper.PathApp))
			{
				return 0;
			}
			if (appCount.HasValue)
			{
				return appCount.Value;
			}
			appCount = 0;
			if (Directory.Exists(PathHelper.PathApp))
			{
				Console.WriteLine("App found, packing..");
				IEnumerable<string> source = Directory.EnumerateFiles(PathHelper.PathApp, "*", SearchOption.AllDirectories);
				appCount = source.Count();
			}
			return appCount.Value;
		}

		public static uint GetAppActionId(string actionFilename)
		{
			uint num = 0u;
			foreach (string appFile in GetAppFileList())
			{
				if (Path.GetFileName(appFile) == actionFilename)
				{
					return num | 0x5000000;
				}
				num++;
			}
			return 0u;
		}

		public static IEnumerable<string> GetAppFileList()
		{
			if (Directory.Exists(PathHelper.PathApp))
			{
				return Directory.EnumerateFiles(PathHelper.PathApp, "*", SearchOption.AllDirectories);
			}
			return null;
		}
	}
}
