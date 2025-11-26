using System.Linq;

namespace XiaomiWatch.Common.Compress
{
	public class RedmiWatch2Rgba565Compressor : IDataCompressor
	{
		public bool HasCompression => false;

		public bool RequiredCompressWithParams => false;

		public byte[] Compress(byte[] data)
		{
			return RepackRgbaToRgb565(data);
		}

		public byte[] Compress(byte[] data, int width, int height, uint type)
		{
			return Compress(data);
		}

		public byte[] GetHeader()
		{
			return new byte[4] { 4, 0, 0, 0 };
		}

		private static byte[] RepackRgbaToRgb565(byte[] pack)
		{
			if (pack == null)
			{
				return pack;
			}
			if (pack.Length == 0)
			{
				return pack;
			}
			byte[] array = new byte[pack.Length / 2];
			byte[] array2 = new byte[pack.Length / 4];
			uint num = 0u;
			uint num2 = 0u;
			for (int i = 0; i < pack.Length; i += 4)
			{
				byte num3 = pack[i];
				byte b = pack[i + 1];
				byte b2 = pack[i + 2];
				byte b3 = pack[i + 3];
				ushort val = (ushort)((ushort)((double)(int)num3 * 31.0 / 255.0) | ((ushort)((double)(int)b * 63.0 / 255.0) << 5) | ((ushort)((double)(int)b2 * 31.0 / 255.0) << 11));
				array.SetWord(num, val);
				array2[num2] = b3;
				num += 2;
				num2++;
			}
			return array.Concat(array2).ToArray();
		}
	}
}
