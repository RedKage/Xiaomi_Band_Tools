using XiaomiWatch.Common;

namespace MiWatchCompiler.Helpers
{
	public class EnumerationHelper
	{
		private WatchType deviceType;

		private byte imageIndex;

		private byte imageListIndex;

		private byte widgetIndex;

		private byte appIndex;

		public EnumerationHelper(int deviceType)
		{
			this.deviceType = (WatchType)deviceType;
			imageIndex = 0;
			imageListIndex = 0;
			widgetIndex = 0;
			appIndex = 0;
		}

		public void ResetIndex()
		{
			switch (deviceType)
			{
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
			case WatchType.MiBand10:
				return;
			}
			imageIndex = 0;
		}

		public byte GetImageIndex()
		{
			return imageIndex++;
		}

		public void ResetListIndex()
		{
			switch (deviceType)
			{
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
			case WatchType.MiBand10:
				return;
			}
			imageListIndex = 0;
		}

		public byte GetImageListIndex()
		{
			return imageListIndex++;
		}

		public void ResetWidgetIndex()
		{
			switch (deviceType)
			{
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
			case WatchType.MiBand10:
				return;
			}
			widgetIndex = 0;
		}

		public byte GetWidgetIndex()
		{
			return widgetIndex++;
		}

		public void ResetAppIndex()
		{
			switch (deviceType)
			{
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
			case WatchType.MiBand10:
				return;
			}
			appIndex = 0;
		}

		public byte GetAppIndex()
		{
			return appIndex++;
		}
	}
}
