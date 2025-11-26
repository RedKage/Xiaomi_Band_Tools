using XiaomiWatch.Common.Decompress;

namespace XiaomiWatch.Common.Compress
{
	public class ImageCompressFactory
	{
		public static IDataDecompressor GetDecompressor(WatchType deviceType, bool withAlfa = false, bool isList = false, bool noCompress = false)
		{
			switch (deviceType)
			{
			case WatchType.RedmiWatch5Active:
			case WatchType.RedmiWatch5Lite:
				return new RedmiWatch5ActiveDecompressor();
			case WatchType.RedmiWatch3Active:
				return new RedmiWatch3ActiveRgba565Decompressor();
			case WatchType.MiBand9:
				return new MiBand9RleDecompressor();
			case WatchType.RedmiWatch2:
			case WatchType.MiBand7Pro:
			case WatchType.RedmiBandPro:
			case WatchType.RedmiWatch2Lite:
				return new MiBand7ProRleDecompressor();
			case WatchType.RedmiWatch3:
			case WatchType.MiBand8:
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
				return new MiBand8ProRleDecompressor();
			default:
				return new PassDecompressor();
			}
		}

		public static IDataCompressor GetCompressor(WatchType deviceType, bool withAlfa, bool isList = false, bool noCompress = false)
		{
			switch (deviceType)
			{
			case WatchType.RedmiWatch5Active:
			case WatchType.RedmiWatch5Lite:
				return new RedmiWatch5ActiveCompressor(withAlfa, isList, noCompress);
			case WatchType.RedmiWatch2:
			case WatchType.RedmiBandPro:
			case WatchType.RedmiWatch2Lite:
				return new RedmiWatch2Rgba565Compressor();
			case WatchType.RedmiWatch3Active:
				return new RedmiWatch3ActiveRgba565Compressor();
			case WatchType.RedmiWatch3:
			case WatchType.MiBand8:
				return new RedmiWatch3RleCompressor(withAlfa, isList, noCompress);
			case WatchType.MiBand8Pro:
			case WatchType.MiWatchS3:
			case WatchType.RedmiWatch4:
			case WatchType.MiBand9Pro:
			case WatchType.MiWatchS4:
			case WatchType.RedmiWatch5:
				return new MiBand8ProRleCompressor(withAlfa, isList, noCompress);
			case WatchType.MiBand9:
				return new MiBand9RleCompressor(withAlfa: true, isList, noCompress);
			case WatchType.MiBand7Pro:
				return new MiBand7ProRleCompressor(withAlfa, isList);
			default:
				return new PassCompressor();
			}
		}

		public static IDataCompressor GetCompressor(WatchType deviceType, string name, bool withAlfa, bool isList = false, bool noCompress = false)
		{
			noCompress = name.ToLower().EndsWith("_nocompress");
			return GetCompressor(deviceType, withAlfa, isList, noCompress);
		}
	}
}
