using System;
using System.Linq;

namespace XiaomiWatch.Common.Decompress
{
	public class RedmiWatch3ActiveRgba565Decompressor : IDataDecompressor
	{
		public bool HasCompression => true;

		public byte[] Decompress(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Decompress(byte[] data, int width, int height, uint type = 0u)
		{
			int num = width * 2 * height;
			if (data.Length == num)
			{
				return data.ReverseWord();
			}
			switch (type & 0xFF)
			{
			case 3u:
				return DecompressType3(data, width, height, (int)(type >> 8));
			case 2u:
				return DecompressType2(data, width, height, (int)(type >> 8));
			default:
				return DecompressDefault(data, width, height);
			}
		}

		private byte[] DecompressType3(byte[] data, int width, int height, int headerSize)
		{
			byte[] array = new byte[width * height];
			byte[] byteArray = data.GetByteArray(0, headerSize);
			byte[] byteArray2 = data.GetByteArray(headerSize, data.Length - headerSize);
			int word = byteArray.GetWord(0, 0);
			byteArray.GetWord(6);
			int word2 = byteArray.GetWord(8);
			byteArray.GetWord(10);
			int num = (word << 2) + 12;
			int num2 = 12;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < height; i++)
			{
				if (i < word2)
				{
					num4 += width;
					continue;
				}
				if (i > word2 + word)
				{
					num4 += width;
					continue;
				}
				int word3 = byteArray.GetWord(num2);
				ushort word4 = byteArray.GetWord(num2 + 2);
				int len = word4 - num3;
				byte[] byteArray3 = byteArray2.GetByteArray(num3, len);
				array.SetByteArray(num4 + word3, byteArray3);
				num4 += width;
				num3 = word4;
				num2 += 4;
				if (num2 >= num)
				{
					break;
				}
			}
			return array;
		}

		private byte[] DecompressType2(byte[] data, int width, int height, int headerSize)
		{
			int num = 3;
			byte[] array = new byte[width * num * height];
			byte[] byteArray = data.GetByteArray(0, headerSize);
			byte[] byteArray2 = data.GetByteArray(headerSize, data.Length - headerSize);
			int word = byteArray.GetWord(0, 0);
			byteArray.GetWord(6);
			int word2 = byteArray.GetWord(8);
			byteArray.GetWord(10);
			int num2 = (word << 2) + 12;
			int num3 = 12;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < height; i++)
			{
				if (i < word2)
				{
					num5 += width * 3;
					continue;
				}
				if (i > word2 + word)
				{
					num5 += width * 3;
					continue;
				}
				int word3 = byteArray.GetWord(num3);
				ushort word4 = byteArray.GetWord(num3 + 2);
				int num6 = word4 - num4;
				byte[] byteArray3 = byteArray2.GetByteArray(num4 * 3, num6 * 3);
				array.SetByteArray(num5 + word3 * 3, byteArray3);
				num5 += width * 3;
				num4 = word4;
				num3 += 4;
				if (num3 >= num2)
				{
					break;
				}
			}
			return array;
		}

		private byte[] DecompressDefault(byte[] data, int width, int height)
		{
			int num = 2;
			byte[] array = new byte[width * num * height];
			data.GetByteArray(0, height * 2);
			byte[] array2 = data.Skip(height * 2).ToArray();
			int num2 = 0;
			int num3 = 0;
			do
			{
				ushort word = array2.GetWord(num2);
				int num4 = array2.GetByte(num2 + 2);
				for (int i = 0; i < num4; i++)
				{
					array.SetWord(num3, word);
					num3 += 2;
				}
				num2 += 3;
			}
			while (num2 < array2.Length);
			return array;
		}

		public byte[] GetHeader()
		{
			throw new NotImplementedException();
		}
	}
}
