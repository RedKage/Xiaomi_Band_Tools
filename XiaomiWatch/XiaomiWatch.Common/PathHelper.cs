namespace XiaomiWatch.Common
{
	public class PathHelper
	{
		private static string pathApp;

		private static string pathImages;

		private static string pathImagesAod;

		public static string PathApp => pathApp;

		public static string PathImages => pathImages;

		public static string PathImagesAod => pathImagesAod;

		public static void SetRootPath(string root)
		{
			pathApp = root + "\\app";
			pathImages = root + "\\images";
			pathImagesAod = root + "\\AOD\\images";
		}
	}
}
