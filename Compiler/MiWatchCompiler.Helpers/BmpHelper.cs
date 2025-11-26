using System;
using System.Diagnostics;
using System.Linq;
using XiaomiWatch.Common;

namespace MiWatchCompiler.Helpers
{
	public class BmpHelper
	{
		private static byte[] headerRgb565 = new byte[122]
		{
			66, 77, 246, 220, 2, 0, 0, 0, 0, 0,
			122, 0, 0, 0, 108, 0, 0, 0, 3, 1,
			0, 0, 181, 0, 0, 0, 1, 0, 16, 0,
			3, 0, 0, 0, 124, 220, 2, 0, 196, 14,
			0, 0, 196, 14, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 248, 0, 0, 224, 7,
			0, 0, 31, 0, 0, 0, 0, 0, 0, 0,
			66, 71, 82, 115, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0
		};

		private static byte[] headerARgb32 = new byte[122]
		{
			66, 77, 246, 220, 2, 0, 0, 0, 0, 0,
			122, 0, 0, 0, 108, 0, 0, 0, 3, 1,
			0, 0, 181, 0, 0, 0, 1, 0, 32, 0,
			3, 0, 0, 0, 124, 220, 2, 0, 196, 14,
			0, 0, 196, 14, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 255, 0, 0, 0, 0, 255,
			0, 0, 0, 0, 255, 0, 0, 0, 0, 255,
			66, 71, 82, 115, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0
		};

		private static byte[] headerARgb32_GTR = new byte[122]
		{
			66, 77, 246, 220, 2, 0, 0, 0, 0, 0,
			122, 0, 0, 0, 108, 0, 0, 0, 3, 1,
			0, 0, 181, 0, 0, 0, 1, 0, 32, 0,
			3, 0, 0, 0, 124, 220, 2, 0, 196, 14,
			0, 0, 196, 14, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 255, 0, 0, 255,
			0, 0, 255, 0, 0, 0, 0, 0, 0, 255,
			66, 71, 82, 115, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0
		};

		private static byte[] headerIndex8 = new byte[1078]
		{
			66, 77, 86, 52, 0, 0, 0, 0, 0, 0,
			54, 4, 0, 0, 40, 0, 0, 0, 110, 0,
			0, 0, 110, 0, 0, 0, 1, 0, 8, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 196, 14,
			0, 0, 196, 14, 0, 0, 0, 1, 0, 0,
			0, 1, 0, 0, 1, 1, 1, 255, 4, 4,
			3, 255, 5, 5, 5, 255, 7, 8, 7, 255,
			8, 8, 7, 255, 8, 7, 8, 255, 7, 8,
			8, 255, 9, 9, 9, 255, 12, 11, 11, 255,
			11, 12, 11, 255, 12, 12, 11, 255, 11, 11,
			12, 255, 12, 11, 12, 255, 11, 12, 12, 255,
			13, 13, 13, 255, 16, 14, 15, 255, 14, 16,
			14, 255, 15, 14, 16, 255, 15, 17, 16, 255,
			17, 17, 17, 255, 20, 19, 19, 255, 19, 20,
			19, 255, 20, 20, 19, 255, 20, 19, 20, 255,
			19, 20, 20, 255, 21, 21, 21, 255, 23, 24,
			23, 255, 24, 24, 23, 255, 23, 23, 24, 255,
			24, 23, 24, 255, 23, 24, 24, 255, 25, 25,
			25, 255, 28, 26, 27, 255, 27, 28, 27, 255,
			27, 27, 28, 255, 28, 27, 28, 255, 27, 28,
			28, 255, 29, 29, 29, 255, 31, 32, 30, 255,
			32, 32, 31, 255, 32, 31, 32, 255, 31, 32,
			32, 255, 33, 33, 33, 255, 36, 35, 35, 255,
			35, 36, 35, 255, 36, 36, 35, 255, 36, 35,
			36, 255, 35, 36, 36, 255, 37, 37, 37, 255,
			40, 39, 39, 255, 39, 40, 39, 255, 40, 40,
			39, 255, 39, 39, 40, 255, 40, 38, 40, 255,
			39, 40, 40, 255, 41, 41, 41, 255, 44, 42,
			42, 255, 43, 44, 43, 255, 44, 44, 43, 255,
			43, 43, 44, 255, 44, 43, 44, 255, 43, 44,
			44, 255, 45, 45, 45, 255, 48, 47, 47, 255,
			47, 48, 47, 255, 48, 48, 47, 255, 46, 46,
			48, 255, 48, 46, 48, 255, 47, 48, 48, 255,
			49, 49, 49, 255, 52, 51, 51, 255, 51, 52,
			51, 255, 52, 52, 51, 255, 51, 51, 52, 255,
			52, 50, 52, 255, 51, 52, 52, 255, 53, 53,
			53, 255, 56, 54, 55, 255, 54, 56, 55, 255,
			56, 56, 55, 255, 55, 55, 56, 255, 56, 54,
			56, 255, 55, 56, 56, 255, 57, 57, 57, 255,
			60, 58, 59, 255, 58, 60, 58, 255, 60, 60,
			59, 255, 59, 59, 60, 255, 60, 59, 60, 255,
			61, 61, 61, 255, 64, 62, 63, 255, 62, 64,
			63, 255, 64, 64, 63, 255, 63, 62, 64, 255,
			64, 63, 64, 255, 63, 64, 64, 255, 67, 67,
			67, 255, 72, 70, 71, 255, 71, 72, 71, 255,
			72, 72, 71, 255, 71, 70, 72, 255, 73, 71,
			73, 255, 71, 72, 72, 255, 75, 75, 75, 255,
			80, 78, 79, 255, 79, 80, 78, 255, 80, 80,
			79, 255, 78, 78, 80, 255, 80, 79, 80, 255,
			79, 80, 80, 255, 83, 83, 83, 255, 88, 86,
			86, 255, 86, 88, 86, 255, 88, 88, 87, 255,
			87, 86, 88, 255, 88, 87, 88, 255, 87, 88,
			88, 255, 91, 91, 91, 255, 96, 94, 95, 255,
			95, 96, 95, 255, 96, 96, 95, 255, 95, 94,
			96, 255, 96, 95, 96, 255, 95, 96, 96, 255,
			99, 99, 99, 255, 104, 102, 102, 255, 102, 104,
			102, 255, 104, 104, 103, 255, 103, 102, 104, 255,
			104, 103, 104, 255, 103, 104, 104, 255, 107, 107,
			107, 255, 112, 110, 111, 255, 110, 112, 110, 255,
			112, 112, 111, 255, 110, 110, 112, 255, 112, 110,
			112, 255, 111, 112, 112, 255, 115, 115, 115, 255,
			120, 119, 118, 255, 118, 120, 119, 255, 120, 120,
			119, 255, 118, 118, 120, 255, 120, 119, 120, 255,
			119, 120, 120, 255, 123, 123, 123, 255, 128, 127,
			127, 255, 127, 128, 127, 255, 128, 128, 127, 255,
			126, 127, 128, 255, 128, 127, 128, 255, 126, 128,
			128, 255, 131, 131, 131, 255, 136, 134, 134, 255,
			135, 136, 134, 255, 136, 136, 135, 255, 134, 135,
			136, 255, 136, 135, 136, 255, 135, 136, 136, 255,
			139, 139, 139, 255, 144, 142, 142, 255, 143, 144,
			142, 255, 144, 144, 143, 255, 143, 142, 144, 255,
			144, 142, 144, 255, 143, 144, 144, 255, 147, 147,
			147, 255, 152, 151, 151, 255, 150, 152, 150, 255,
			152, 152, 151, 255, 150, 150, 152, 255, 152, 151,
			152, 255, 151, 152, 152, 255, 155, 155, 155, 255,
			160, 158, 158, 255, 158, 160, 158, 255, 160, 160,
			159, 255, 158, 158, 160, 255, 160, 159, 160, 255,
			159, 160, 160, 255, 163, 163, 163, 255, 168, 166,
			167, 255, 166, 168, 166, 255, 168, 168, 167, 255,
			167, 167, 168, 255, 168, 167, 168, 255, 166, 168,
			168, 255, 171, 171, 171, 255, 176, 175, 174, 255,
			174, 176, 174, 255, 176, 176, 175, 255, 175, 174,
			176, 255, 176, 175, 176, 255, 175, 176, 176, 255,
			179, 179, 179, 255, 184, 182, 182, 255, 183, 184,
			183, 255, 184, 184, 183, 255, 183, 182, 184, 255,
			184, 183, 184, 255, 183, 184, 184, 255, 187, 187,
			187, 255, 192, 191, 190, 255, 190, 192, 190, 255,
			192, 192, 191, 255, 191, 190, 192, 255, 192, 190,
			192, 255, 191, 192, 192, 255, 195, 195, 195, 255,
			200, 198, 199, 255, 199, 200, 199, 255, 200, 200,
			199, 255, 198, 199, 200, 255, 200, 199, 200, 255,
			199, 200, 200, 255, 203, 203, 203, 255, 207, 208,
			207, 255, 208, 208, 207, 255, 207, 207, 208, 255,
			208, 207, 208, 255, 207, 208, 208, 255, 211, 211,
			211, 255, 216, 215, 215, 255, 215, 216, 214, 255,
			216, 216, 215, 255, 215, 214, 216, 255, 216, 214,
			216, 255, 215, 216, 216, 255, 219, 219, 219, 255,
			224, 223, 223, 255, 223, 224, 223, 255, 224, 224,
			223, 255, 223, 223, 224, 255, 224, 223, 224, 255,
			223, 224, 224, 255, 227, 227, 227, 255, 232, 230,
			231, 255, 231, 232, 230, 255, 230, 231, 232, 255,
			232, 231, 232, 255, 235, 235, 235, 255, 240, 239,
			239, 255, 239, 240, 239, 255, 240, 240, 239, 255,
			239, 239, 240, 255, 240, 239, 240, 255, 239, 240,
			240, 255, 243, 243, 243, 255, 246, 248, 246, 255,
			248, 248, 247, 255, 248, 246, 248, 255, 252, 252,
			252, 255, 0, 0, 0, 255, 0, 0, 0, 255,
			0, 0, 0, 255, 0, 0, 0, 255
		};

		private static byte[] headerIndex8v2 = new byte[54]
		{
			66, 77, 86, 52, 0, 0, 0, 0, 0, 0,
			54, 4, 0, 0, 40, 0, 0, 0, 110, 0,
			0, 0, 110, 0, 0, 0, 1, 0, 8, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 196, 14,
			0, 0, 196, 14, 0, 0, 0, 1, 0, 0,
			0, 1, 0, 0
		};

		public static byte[] ConvertToBmp(byte[] data, int width, int height, int type, byte[] clut = null)
		{
			int num = data.Length;
			byte[] array = headerRgb565;
			switch (type)
			{
			case 2:
				array = headerIndex8;
				break;
			case 1:
				array = headerARgb32;
				break;
			default:
				array = headerRgb565;
				break;
			}
			if (type == 0)
			{
				data = AlignRowData(data, width, height, type);
			}
			num = data.Length;
			data = FlipImageData(data, width, height, type);
			data = array.Concat(data).ToArray();
			int val = data.Length;
			data.SetDWord(2, (uint)val);
			if (type == 2)
			{
				data.SetByteArray(54, clut);
			}
			else
			{
				data.SetDWord(34, (uint)num);
			}
			data.SetDWord(18, (uint)width);
			data.SetDWord(22, (uint)height);
			return data;
		}

		public static byte[] ConvertToBmpGTR(byte[] data, int width, int height, int type, byte[] clut = null)
		{
			int num = data.Length;
			byte[] array = headerRgb565;
			switch (type)
			{
			case 4:
				array = headerARgb32_GTR;
				break;
			case 3:
				array = headerRgb565;
				break;
			case 2:
				array = headerRgb565;
				break;
			case 1:
				array = headerIndex8;
				break;
			default:
				array = headerRgb565;
				break;
			}
			int dstWidth = 0;
			if (type != 4)
			{
				data = AlignRowData(data, width, height, type, out dstWidth);
			}
			else
			{
				dstWidth = width;
			}
			num = data.Length;
			width = dstWidth;
			data = FlipImageData(data, width, height, type);
			data = array.Concat(data).ToArray();
			int val = data.Length;
			data.SetDWord(2, (uint)val);
			if (type == 1)
			{
				data.SetByteArray(54, clut);
			}
			else
			{
				data.SetDWord(34, (uint)num);
			}
			data.SetDWord(18, (uint)width);
			data.SetDWord(22, (uint)height);
			return data;
		}

		internal static byte[] ConvertToBmpGTRv2(byte[] data, int width, int height, int type)
		{
			int val = data.Length;
			byte[] array = headerIndex8v2;
			switch (type)
			{
			case 4:
				array = headerARgb32_GTR;
				break;
			case 2:
				array = headerRgb565;
				break;
			default:
				array = headerIndex8v2;
				break;
			}
			int num = 256;
			if (type == 1)
			{
				byte b = 0;
				if (b != 0)
				{
					num = b;
				}
				int count = num << 2;
				byte[] second = data.Take(count).ToArray();
				byte[] src = data.Skip(count).ToArray();
				int dstWidth = 0;
				if (type != 4)
				{
					src = AlignRowData(src, width, height, type, out dstWidth);
				}
				else
				{
					dstWidth = width;
				}
				val = data.Length;
				width = dstWidth;
				src = FlipImageData(src, width, height, 1);
				data = array.Concat(second).Concat(src).ToArray();
			}
			else
			{
				data = FlipImageData(data, width, height, type);
				data = array.Concat(data).ToArray();
			}
			data.SetDWord(2, (uint)data.Length);
			data.SetDWord(18, (uint)width);
			data.SetDWord(22, (uint)height);
			if (type == 1)
			{
				data.SetDWord(46, (uint)num);
				data.SetDWord(50, (uint)num);
			}
			else
			{
				data.SetDWord(34, (uint)val);
			}
			return data;
		}

		private static byte[] AlignRowData(byte[] src, int width, int height, int type)
		{
			uint num = 2u;
			if (type == 1)
			{
				num = 4u;
			}
			if (type == 2)
			{
				num = 1u;
			}
			uint num2 = (uint)width * num;
			uint num3 = (uint)(width + width % 2) * num;
			byte[] array = new byte[num3 * height];
			uint num4 = 0u;
			for (int i = 0; i < height; i++)
			{
				array.SetByteArray((int)(i * num3), src.GetByteArray(num4, num2));
				num4 += num2;
			}
			return array;
		}

		private static byte[] AlignRowData(byte[] src, int width, int height, int type, out int dstWidth)
		{
			uint num = (uint)(width * type);
			dstWidth = width.GetDWordAligned();
			uint num2 = (uint)(dstWidth * type);
			if (num == num2)
			{
				return src;
			}
			byte[] array = new byte[num2 * height];
			uint num3 = 0u;
			for (int i = 0; i < height; i++)
			{
				array.SetByteArray((int)(i * num2), src.GetByteArray(num3, num));
				num3 += num;
			}
			return array;
		}

		private static byte[] FlipImageData(byte[] src, int width, int height, int type)
		{
			uint num = (uint)(width * type);
			byte[] array = new byte[src.Length];
			uint num2 = (uint)src.Length - num;
			for (int i = 0; i < height; i++)
			{
				array.SetByteArray((int)(i * num), src.GetByteArray(num2, num));
				num2 -= num;
			}
			return array;
		}

		internal static byte[] UncompressRLE(byte[] data, int destLen)
		{
			uint num = 0u;
			int num2 = 0;
			byte[] array = null;
			byte[] array2 = new byte[destLen];
			try
			{
				do
				{
					byte b = data[num];
					num++;
					if (b == byte.MaxValue)
					{
						break;
					}
					byte b2 = 0;
					if ((b & 0x80) == 128)
					{
						b2 = (byte)(b & 0x7F);
						array = data.GetByteArray(num, 4);
						for (int i = 0; i <= b2; i++)
						{
							array2.SetByteArray(num2, array);
							num2 += 4;
						}
						num += 4;
					}
					else
					{
						b2 = b;
						for (int j = 0; j <= b2; j++)
						{
							array = data.GetByteArray(num, 4);
							array2.SetByteArray(num2, array);
							num2 += 4;
							num += 4;
						}
					}
				}
				while (num < data.Length);
			}
			catch (Exception)
			{
				Debugger.Break();
			}
			return array2;
		}

		internal static byte[] UncompressRLEv20(byte[] data, int destLen)
		{
			uint num = 0u;
			int num2 = 0;
			byte b = 0;
			byte[] array = new byte[destLen];
			try
			{
				do
				{
					byte b2 = data[num];
					num++;
					byte b3 = 0;
					if ((b2 & 0x80) == 0)
					{
						if (num >= data.Length)
						{
							break;
						}
						b3 = (byte)(b2 & 0x7F);
						b = data.GetByte(num);
						for (int i = 0; i < b3; i++)
						{
							if (num2 >= array.Length)
							{
								break;
							}
							array[num2] = b;
							num2++;
						}
						num++;
						continue;
					}
					b3 = (byte)(b2 & 0x7F);
					for (int j = 0; j < b3; j++)
					{
						if (num2 >= array.Length)
						{
							break;
						}
						b = data.GetByte(num);
						array[num2] = b;
						num2++;
						num++;
					}
				}
				while (num < data.Length);
			}
			catch (Exception)
			{
				Debugger.Break();
			}
			return array;
		}

		internal static byte[] UncompressRLEv10(byte[] data, int destLen, byte recordSize = 4)
		{
			uint num = 0u;
			int num2 = 0;
			byte[] array = null;
			byte[] array2 = new byte[destLen];
			try
			{
				do
				{
					byte b = data[num];
					num++;
					byte b2 = 0;
					if ((b & 0x80) == 0)
					{
						if (num >= data.Length - recordSize)
						{
							break;
						}
						b2 = (byte)(b & 0x7F);
						array = data.GetByteArray(num, recordSize);
						for (int i = 0; i < b2; i++)
						{
							array2.SetByteArray(num2, array);
							num2 += recordSize;
						}
						num += recordSize;
					}
					else
					{
						b2 = (byte)(b & 0x7F);
						for (int j = 0; j < b2; j++)
						{
							array = data.GetByteArray(num, recordSize);
							array2.SetByteArray(num2, array);
							num2 += recordSize;
							num += recordSize;
						}
					}
				}
				while (num < data.Length);
			}
			catch (Exception)
			{
				Debugger.Break();
			}
			return array2;
		}

		internal static byte[] UncompressRLEv11(byte[] data, int destLen, byte recordSize = 4)
		{
			uint num = 0u;
			int num2 = 0;
			byte[] array = null;
			byte[] array2 = new byte[destLen];
			try
			{
				do
				{
					byte b = data[num];
					num++;
					byte b2 = 0;
					if ((b & 0x80) == 128)
					{
						if (num >= data.Length - recordSize)
						{
							break;
						}
						b2 = (byte)(b & 0x7F);
						array = data.GetByteArray(num, recordSize);
						for (int i = 0; i <= b2; i++)
						{
							array2.SetByteArray(num2, array);
							num2 += recordSize;
						}
						num += recordSize;
					}
					else
					{
						b2 = (byte)(b & 0x7F);
						for (int j = 0; j <= b2; j++)
						{
							array = data.GetByteArray(num, recordSize);
							array2.SetByteArray(num2, array);
							num2 += recordSize;
							num += recordSize;
						}
					}
				}
				while (num < data.Length);
			}
			catch (Exception)
			{
				Debugger.Break();
			}
			return array2;
		}
	}
}
