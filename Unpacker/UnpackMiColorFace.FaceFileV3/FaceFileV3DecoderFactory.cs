using System;
using XiaomiWatch.Common;

namespace UnpackMiColorFace.FaceFileV3
{
	internal class FaceFileV3DecoderFactory
	{
		public static IFaceFileV3Decoder GetDecoder(WatchType watchType)
		{
			if (watchType == WatchType.RedmiWatch3Active)
			{
				return new FaceRedmiWatch3ActiveDecoder();
			}
			throw new NotSupportedException($"Can't find decoder for watch: {watchType}");
		}
	}
}
