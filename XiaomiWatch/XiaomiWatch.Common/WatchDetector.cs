using System;
using System.Collections.Generic;
using System.Linq;

namespace XiaomiWatch.Common
{
	public class WatchDetector
	{
		private static ushort height;

		private static uint offsetPreviewImg;

		private static ushort width;

		private const uint OffsetPreviewV2 = 32u;

		private const uint OffsetPreviewV3 = 89u;

		private const uint OffsetFaceId = 40u;

		private static List<(WatchType type, int width, int height)> previewSize = new List<(WatchType, int, int)>
		{
			(WatchType.Gen2, 246, 246),
			(WatchType.Gen3, 280, 280),
			(WatchType.MiWatchS3, 326, 326),
			(WatchType.MiWatchS3, 464, 464),
			(WatchType.MiWatchS4, 326, 326),
			(WatchType.MiWatchS4, 464, 464),
			(WatchType.RedmiWatch2, 178, 178),
			(WatchType.RedmiWatch2Lite, 178, 178),
			(WatchType.RedmiWatch3, 234, 270),
			(WatchType.RedmiWatch3Active, 156, 182),
			(WatchType.RedmiWatch4, 234, 270),
			(WatchType.RedmiWatch4, 390, 450),
			(WatchType.RedmiWatch5, 432, 514),
			(WatchType.RedmiWatch5Active, 180, 216),
			(WatchType.RedmiWatch5Active, 320, 385),
			(WatchType.RedmiWatch5Lite, 244, 298),
			(WatchType.RedmiWatch5Lite, 410, 502),
			(WatchType.RedmiBandPro, 110, 208),
			(WatchType.MiBand8, 122, 310),
			(WatchType.MiBand9, 122, 310),
			(WatchType.MiBand9, 192, 490),
			(WatchType.MiBand10, 212, 520),
			(WatchType.MiBand7Pro, 220, 358),
			(WatchType.MiBand8Pro, 230, 328),
			(WatchType.MiBand8Pro, 336, 480),
			(WatchType.MiBand9Pro, 230, 328),
			(WatchType.MiBand9Pro, 336, 480)
		};

		public static WatchType GetWatchType(byte[] data, int watchVersion)
		{
			string text = "";
			switch (watchVersion)
			{
			case 1:
				return WatchType.Gen1;
			case 2:
				offsetPreviewImg = data.GetDWord(32u);
				width = data.GetWord(offsetPreviewImg + 4);
				height = data.GetWord(offsetPreviewImg + 6);
				text = data.GetAsciiString(40u);
				break;
			case 3:
				offsetPreviewImg = data.GetDWord(89u);
				if (data.GetByte(offsetPreviewImg) != 1)
				{
					throw new ApplicationException($"Watchface has wrong preview data, version: {watchVersion}");
				}
				width = data.GetWord(offsetPreviewImg + 5);
				height = data.GetWord(offsetPreviewImg + 7);
				break;
			}
			WatchType watchType = GetWatchType(width, height);
			if (watchType == WatchType.RedmiWatch3 && text.StartsWith("365"))
			{
				watchType = WatchType.RedmiWatch4;
			}
			if (watchType == WatchType.MiBand8 && text.StartsWith("366"))
			{
				watchType = WatchType.MiBand9;
			}
			if (watchType == WatchType.MiBand8 && text.StartsWith("1209"))
			{
				watchType = WatchType.MiBand9;
			}
			if (watchType == WatchType.MiBand8Pro && text.StartsWith("1209"))
			{
				watchType = WatchType.MiBand9Pro;
			}
			return watchType;
		}

		public static WatchType GetWatchType(int width, int height)
		{
			(WatchType, int, int)? tuple = previewSize.Where(((WatchType type, int width, int height) c) => c.width == width && c.height == height).Cast<(WatchType, int, int)?>().FirstOrDefault();
			if (tuple.HasValue)
			{
				return tuple.Value.Item1;
			}
			return WatchType.Undefined;
		}

		public static (int width, int height) GetWatchPreviewSize(WatchType watchType)
		{
			(WatchType, int, int)? tuple = previewSize.Where(((WatchType type, int width, int height) c) => c.type == watchType).Cast<(WatchType, int, int)?>().FirstOrDefault();
			if (tuple.HasValue)
			{
				return (width: tuple.Value.Item2, height: tuple.Value.Item3);
			}
			return (width: 0, height: 0);
		}
	}
}
