using System;
using System.Linq;

namespace XiaomiWatch.Common.Compress
{
	public class RedmiWatch3ActiveRgba565Compressor : IDataCompressor
	{
		public bool HasCompression => true;

		public bool RequiredCompressWithParams => true;

		public byte[] Compress(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Compress(byte[] data, int width, int height, uint type = 0u)
		{
			switch (type & 0xFF)
			{
			case 3u:
				return CompressType3(data, width, height, (int)(type >> 8));
			case 2u:
				return CompressType2(data, width, height, (int)(type >> 8));
			default:
				return CompressDefault(data, width, height);
			}
		}

		private byte[] CompressType3(byte[] data, int width, int height, int offset)
		{
			int num = 4;
			int num2 = width * num;
			int num3 = num2 * height;
			RW3ActiveImageInfo imageInfo = RW3ActiveImageHelper.GetImageInfo(data, width, height, alphaOnly: true);
			int imageCount = imageInfo.ImageCount;
			byte[] array = new byte[imageInfo.ImageCount * imageInfo.ImageHeaderItemSize];
			int imageHeaderItemSize = imageInfo.ImageHeaderItemSize;
			for (int i = 0; i < imageCount; i++)
			{
				byte[] byteArray = data.GetByteArray(i * num3, num3);
				imageInfo = RW3ActiveImageHelper.GetImageInfo(byteArray, width, height, alphaOnly: true);
				array.SetWord(i * imageHeaderItemSize, imageInfo.MaxColumnLength);
				array.SetWord(i * imageHeaderItemSize + 6, imageInfo.AlignX);
				array.SetWord(i * imageHeaderItemSize + 8, imageInfo.AlignY);
				array.SetWord(i * imageHeaderItemSize + 10, imageInfo.MaxRowLength);
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				byte[] array2 = Array.Empty<byte>();
				for (int j = imageInfo.AlignY; j < imageInfo.MaxColumnLength + imageInfo.AlignY; j++)
				{
					byte[] lineBytes = byteArray.GetByteArray(j * num2, num2);
					byte[] source = (from c in (from num11 in Enumerable.Range(0, lineBytes.Length / 4)
							select BitConverter.ToUInt32(lineBytes, num11 * 4)).ToArray()
						select (byte)(c >> 24)).ToArray();
					int num7 = source.TakeWhile((byte value) => value == 0).Count();
					int num8 = source.Reverse().TakeWhile((byte value) => value == 0).Count();
					if (num7 == width || num8 == width)
					{
						num7 = 0;
						num8 = 0;
					}
					int num9 = num7;
					int num10 = width - num9 - num8;
					if (num10 < 0)
					{
						throw new Exception($"Error in compression: {width}/{num9}/{num8}");
					}
					byte[] array3 = source.Skip(num7).Take(num10).ToArray();
					num4 = array2.Length;
					array2 = array2.AppendZero(array3.Length);
					array2.SetByteArray(num4, array3);
					num5 += num10;
					array.SetWord(i * imageHeaderItemSize + 12 + num6 * 4, num9);
					array.SetWord(i * imageHeaderItemSize + 12 + num6 * 4 + 2, num5);
					num6++;
				}
				int val = offset + array.Length;
				array.SetDWord(i * imageHeaderItemSize + 2, val);
				num4 = array.Length;
				array = array.AppendZero(array2.Length);
				array.SetByteArray(num4, array2);
			}
			return array;
		}

		private byte[] CompressType2(byte[] data, int width, int height, int offset)
		{
			int num = 4;
			int num2 = width * num;
			int num3 = num2 * height;
			RW3ActiveImageInfo imageInfo = RW3ActiveImageHelper.GetImageInfo(data, width, height);
			int imageCount = imageInfo.ImageCount;
			byte[] array = new byte[imageInfo.ImageCount * imageInfo.ImageHeaderItemSize];
			int imageHeaderItemSize = imageInfo.ImageHeaderItemSize;
			for (int i = 0; i < imageCount; i++)
			{
				byte[] byteArray = data.GetByteArray(i * num3, num3);
				imageInfo = RW3ActiveImageHelper.GetImageInfo(byteArray, width, height);
				array.SetWord(i * imageHeaderItemSize, imageInfo.MaxColumnLength);
				array.SetWord(i * imageHeaderItemSize + 6, imageInfo.AlignX);
				array.SetWord(i * imageHeaderItemSize + 8, imageInfo.AlignY);
				array.SetWord(i * imageHeaderItemSize + 10, imageInfo.MaxRowLength);
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				byte[] array2 = Array.Empty<byte>();
				for (int j = imageInfo.AlignY; j < imageInfo.MaxColumnLength + imageInfo.AlignY; j++)
				{
					byte[] lineBytes = byteArray.GetByteArray(j * num2, num2);
					uint[] source = (from num11 in Enumerable.Range(0, lineBytes.Length / 4)
						select BitConverter.ToUInt32(lineBytes, num11 * 4)).ToArray();
					int num7 = source.TakeWhile((uint value) => value == 0).Count();
					int num8 = source.Reverse().TakeWhile((uint value) => value == 0).Count();
					if (num7 == num8)
					{
						num7 = 0;
						num8 = 0;
					}
					int num9 = num7;
					int num10 = width - num9 - num8;
					if (num10 < 0)
					{
						throw new Exception($"Error in compression: {width}/{num9}/{num8}");
					}
					byte[] array3 = RepackRgbaToRgba565(source.Skip(num7).Take(num10).SelectMany(BitConverter.GetBytes)
						.ToArray());
					num4 = array2.Length;
					array2 = array2.AppendZero(array3.Length);
					array2.SetByteArray(num4, array3);
					num5 += num10;
					array.SetWord(i * imageHeaderItemSize + 12 + num6 * 4, num9);
					array.SetWord(i * imageHeaderItemSize + 12 + num6 * 4 + 2, num5);
					num6++;
				}
				int val = offset + array.Length;
				array.SetDWord(i * imageHeaderItemSize + 2, val);
				num4 = array.Length;
				array = array.AppendZero(array2.Length);
				array.SetByteArray(num4, array2);
			}
			return array;
		}

		private byte[] CompressDefault(byte[] dataRGBA, int width, int height)
		{
			byte[] array = new byte[height * 2];
			byte[] array2 = Array.Empty<byte>();
			byte[] data = RepackRgbaToRgb565(dataRGBA);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < height; i++)
			{
				int num3 = 0;
				ushort num4 = 0;
				for (int j = 0; j < width; j++)
				{
					int offset = j * 2 + i * width * 2;
					ushort word = data.GetWord(offset);
					if (num3 > 0 && (word != num4 || num3 >= 255))
					{
						array2 = array2.AppendZero(3);
						array2.SetWord(num, num4);
						array2[num + 2] = (byte)num3;
						num += 3;
						num3 = 0;
					}
					num4 = word;
					num3++;
				}
				if (num3 > 0)
				{
					array2 = array2.AppendZero(3);
					array2.SetWord(num, num4);
					array2[num + 2] = (byte)num3;
					num += 3;
				}
				ushort val = (ushort)(array2.Length / 3);
				array.SetWord(num2, val, 1u);
				num2 += 2;
			}
			return array.Concat(array2).ToArray();
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
			uint num = 0u;
			for (int i = 0; i < pack.Length; i += 4)
			{
				byte num2 = pack[i];
				byte b = pack[i + 1];
				byte b2 = pack[i + 2];
				_ = pack[i + 3];
				ushort val = (ushort)((ushort)((double)(int)num2 * 31.0 / 255.0) | ((ushort)((double)(int)b * 63.0 / 255.0) << 5) | ((ushort)((double)(int)b2 * 31.0 / 255.0) << 11));
				array.SetWord(num, val);
				num += 2;
			}
			return array;
		}

		private static byte[] RepackRgbaToRgba565(byte[] pack)
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
				array[num + 2] = b3;
				num += 3;
			}
			return array;
		}

		public byte[] GetHeader()
		{
			return Array.Empty<byte>();
		}
	}
}
