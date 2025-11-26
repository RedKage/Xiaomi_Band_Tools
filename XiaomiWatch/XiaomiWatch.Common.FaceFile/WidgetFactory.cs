namespace XiaomiWatch.Common.FaceFile
{
	public class WidgetFactory
	{
		public static FaceWidget Get(uint itemType, byte[] bin)
		{
			switch (itemType)
			{
			case 27u:
				return FaceWidgetAnalogClock.Get(bin);
			case 29u:
				return FaceWidgetCircleProgress.Get(bin);
			case 42u:
				return FaceWidgetCircleProgressPlus.Get(bin);
			case 30u:
				return FaceWidgetImage.Get(bin);
			case 31u:
				return FaceWidgetImageList.Get(bin);
			case 32u:
				return FaceWidgetDigitalNum.Get(bin);
			case 33u:
				return FaceWidgetTextPlus.Get(bin);
			case 34u:
				return FaceWidgetContainer.Get(bin);
			case 39u:
				return FaceWidgetConcatenate.Get(bin);
			case 41u:
				return FaceWidgetImageCacheList.Get(bin);
			case 43u:
				return FaceWidgetCircularGauge.Get(bin);
			default:
				return FaceWidget.Get(bin);
			}
		}
	}
}
