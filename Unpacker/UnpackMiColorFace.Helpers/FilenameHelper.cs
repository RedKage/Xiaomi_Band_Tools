using System.IO;
using XiaomiWatch.Common;

namespace UnpackMiColorFace.Helpers
{
	internal class FilenameHelper
	{
		private string filename;

		public string NameNoExt => Path.GetFileNameWithoutExtension(filename);

		public FilenameHelper(string filename)
		{
			this.filename = filename;
		}

		public string GetFaceSlotImagesFolder(WatchType watchType, int slotId, int subversion)
		{
			switch (watchType)
			{
			case WatchType.Gen3:
				return NameNoExt + "\\images";
			case WatchType.MiWatchS3:
				return NameNoExt + "\\images";
			case WatchType.MiBand9:
				return NameNoExt + "\\images";
			case WatchType.MiBand8Pro:
				return NameNoExt + "\\images";
			default:
			{
				string result;
				switch (slotId)
				{
				default:
					result = NameNoExt + $"\\images_{slotId}";
					break;
				case 1:
					result = NameNoExt + "\\AOD\\images";
					break;
				case 0:
					result = NameNoExt + "\\images";
					break;
				}
				return result;
			}
			}
		}

		internal string GetFaceSlotFilename(WatchType watchType, int slotId, int subversion)
		{
			string result = ((slotId > 0) ? $"{NameNoExt}_{slotId}" : NameNoExt);
			if (watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand9 || watchType == WatchType.MiBand8Pro)
			{
				if ((subversion & 4) > 0 && slotId == 1)
				{
					result = NameNoExt + "_AOD";
				}
			}
			else if (slotId == 1)
			{
				result = "AOD\\" + NameNoExt;
			}
			return result;
		}
	}
}
