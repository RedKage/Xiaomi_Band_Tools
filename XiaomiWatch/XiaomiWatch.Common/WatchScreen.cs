namespace XiaomiWatch.Common
{
	public class WatchScreen
	{
		public static int GetScreenHeight(int watchTypeId)
		{
			return GetScreenHeight((WatchType)watchTypeId);
		}

		public static int GetScreenHeight(WatchType watchType)
		{
			switch (watchType)
			{
			case WatchType.Gen1:
				return 454;
			case WatchType.Gen2:
				return 466;
			case WatchType.Gen3:
				return 480;
			case WatchType.MiWatchS3:
				return 466;
			case WatchType.MiWatchS4:
				return 466;
			case WatchType.RedmiWatch2:
				return 360;
			case WatchType.RedmiWatch2Lite:
				return 360;
			case WatchType.RedmiWatch3:
				return 450;
			case WatchType.RedmiWatch4:
				return 450;
			case WatchType.RedmiWatch5:
				return 514;
			case WatchType.RedmiBandPro:
				return 456;
			case WatchType.MiBand7Pro:
				return 456;
			case WatchType.MiBand8:
				return 490;
			case WatchType.MiBand9:
				return 490;
			case WatchType.MiBand10:
				return 520;
			case WatchType.MiBand8Pro:
				return 480;
			case WatchType.MiBand9Pro:
				return 480;
			case WatchType.RedmiWatch3Active:
				return 280;
			case WatchType.RedmiWatch5Active:
				return 385;
			case WatchType.RedmiWatch5Lite:
				return 502;
			default:
				return 0;
			}
		}

		public static int GetScreenWidth(int watchTypeId)
		{
			return GetScreenWidth((WatchType)watchTypeId);
		}

		public static int GetScreenWidth(WatchType watchType)
		{
			switch (watchType)
			{
			case WatchType.Gen1:
				return 454;
			case WatchType.Gen2:
				return 466;
			case WatchType.Gen3:
				return 480;
			case WatchType.MiWatchS3:
				return 466;
			case WatchType.MiWatchS4:
				return 466;
			case WatchType.RedmiWatch2:
				return 320;
			case WatchType.RedmiWatch2Lite:
				return 320;
			case WatchType.RedmiWatch3:
				return 390;
			case WatchType.RedmiWatch4:
				return 390;
			case WatchType.RedmiWatch5:
				return 432;
			case WatchType.RedmiWatch3Active:
				return 240;
			case WatchType.MiBand7Pro:
				return 280;
			case WatchType.MiBand8:
				return 192;
			case WatchType.MiBand9:
				return 192;
			case WatchType.MiBand10:
				return 212;
			case WatchType.MiBand8Pro:
				return 336;
			case WatchType.MiBand9Pro:
				return 336;
			case WatchType.RedmiBandPro:
				return 368;
			case WatchType.RedmiWatch5Active:
				return 320;
			case WatchType.RedmiWatch5Lite:
				return 410;
			default:
				return 0;
			}
		}
	}
}
