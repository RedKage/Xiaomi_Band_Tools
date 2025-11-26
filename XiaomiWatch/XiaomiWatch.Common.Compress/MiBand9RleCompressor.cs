using System;

namespace XiaomiWatch.Common.Compress
{
	public class MiBand9RleCompressor : IDataCompressor
	{
		private bool isList;

		private bool noCompress;

		private bool withAlfa;

		public bool HasCompression => !noCompress;

		public bool RequiredCompressWithParams => false;

		public MiBand9RleCompressor(bool withAlfa, bool isList, bool noCompress = false)
		{
			this.withAlfa = true;
			this.isList = isList;
			this.noCompress = noCompress;
		}

		public byte[] Compress(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				throw new ArgumentNullException("data");
			}
			byte[] data2 = new byte[8] { 224, 33, 165, 90, 0, 0, 0, 0 };
			if (noCompress)
			{
				data2 = new byte[0];
			}
			data = PackRgbaToRgb565Alpha(data);
			if (!noCompress)
			{
				data2.SetDWord(4, (data.Length << 4) | 3);
			}
			byte[] array = null;
			array = ((!noCompress) ? RleEncoder.EncodeV10(data, 3) : data);
			data2 = data2.AppendZero(array.Length);
			data2.SetByteArray(8, array);
			return data2;
		}

		public byte[] Compress(byte[] data, int width, int height, uint type)
		{
			return Compress(data);
		}

		public byte[] GetHeader()
		{
			if (isList)
			{
				if (!withAlfa)
				{
					return new byte[4] { 3, 0, 4, 0 };
				}
				return new byte[4] { 6, 0, 4, 0 };
			}
			if (!withAlfa)
			{
				return new byte[4] { 3, 4, 0, 0 };
			}
			return new byte[4] { 6, 4, 0, 0 };
		}

		private byte[] PackRgbaToRgb565Alpha(byte[] pack)
		{
			if (pack == null)
			{
				return pack;
			}
			if (pack.Length == 0)
			{
				return pack;
			}
			byte[] array = new byte[pack.Length / 4 * 3];
			uint num = 0u;
			for (int i = 0; i < pack.Length; i += 4)
			{
				byte num2 = pack[i];
				byte b = pack[i + 1];
				byte b2 = pack[i + 2];
				byte b3 = pack[i + 3];
				ushort val = (ushort)((ushort)((double)(int)num2 * 31.0 / 255.0) | ((ushort)((double)(int)b * 63.0 / 255.0) << 5) | ((ushort)((double)(int)b2 * 31.0 / 255.0) << 11));
				array.SetWord(num, val);
				num += 2;
				array[num] = b3;
				num++;
			}
			return array;
		}
	}
}
