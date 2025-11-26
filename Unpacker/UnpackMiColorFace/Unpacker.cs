using System;
using System.IO;
using UnpackMiColorFace.Decompiler;
using XiaomiWatch.Common;

namespace UnpackMiColorFace
{
	internal class Unpacker
	{
		private const uint magic_v1_1 = 1180787557u;

		private const uint magic_v1_2 = 1181314149u;

		private const uint magic_v2 = 1520776210u;

		private const uint magic_v3_1 = 1543u;

		private const uint magic_v3_2 = 131073u;

		private static WatchType watchType;

		internal static void Exec(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename))
			{
				throw new ArgumentNullException("filename");
			}
			byte[] data = File.ReadAllBytes(filename);
			int num = 0;
			if (data.GetDWord(0, 1u) == 1180787557 && data.GetDWord(4, 1u) == 1181314149)
			{
				num = 1;
			}
			if (data.GetDWord(0, 1u) == 1520776210)
			{
				num = 2;
			}
			if (data.GetDWord(0, 1u) == 1543 && data.GetDWord(4, 1u) == 131073)
			{
				num = 3;
			}
			if (num == 0)
			{
				throw new MissingFieldException();
			}
			watchType = WatchDetector.GetWatchType(data, num);
			Console.WriteLine($"Watch detected: {watchType}");
			DecompilerFactory.GetDecompiler(num, filename).Process(watchType, data);
		}
	}
}
