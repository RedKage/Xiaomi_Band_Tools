namespace XiaomiWatch.Common.Action
{
	public class ActionFactory
	{
		public static IActionPacker GetPacker(WatchType deviceType)
		{
			switch (deviceType)
			{
			case WatchType.RedmiWatch2:
			case WatchType.RedmiWatch3:
			case WatchType.RedmiBandPro:
			case WatchType.RedmiWatch2Lite:
				return new RedmiWatch3ActionPacker();
			case WatchType.MiBand7Pro:
				return new MiBand7ProActionPacker();
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
			case WatchType.RedmiWatch5Active:
			case WatchType.RedmiWatch5Lite:
				return new RedmiWatch4ActionPacker();
			case WatchType.MiBand8:
				return new MiBand8ActionPacker();
			default:
				return new DefaultActionPacker();
			}
		}
	}
}
