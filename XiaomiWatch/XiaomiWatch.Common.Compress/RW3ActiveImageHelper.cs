namespace XiaomiWatch.Common.Compress
{
	public class RW3ActiveImageHelper
	{
		public static RW3ActiveImageInfo GetImageInfo(byte[] bmp, int width, int height, bool alphaOnly = false)
		{
			int num = 4;
			int num2 = width * num * height;
			int num3 = width * num;
			int num4 = bmp.Length / num2;
			int num5 = width;
			int num6 = height;
			int num7 = width;
			int num8 = height;
			for (int i = 0; i < num4; i++)
			{
				byte[] byteArray = bmp.GetByteArray(i * num2, num2);
				int num9 = 0;
				int num10 = 0;
				int num11 = 0;
				int num12 = 0;
				for (int j = 0; j < height; j++)
				{
					bool flag = true;
					for (int k = 0; k < width; k++)
					{
						uint num13 = byteArray.GetDWord(j * num3 + k * num);
						if (alphaOnly)
						{
							num13 >>= 24;
						}
						if (num13 != 0)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
					num9++;
				}
				for (int num14 = height - 1; num14 >= 0; num14--)
				{
					bool flag2 = true;
					for (int l = 0; l < width; l++)
					{
						uint num15 = byteArray.GetDWord(num14 * num3 + l * num);
						if (alphaOnly)
						{
							num15 >>= 24;
						}
						if (num15 != 0)
						{
							flag2 = false;
							break;
						}
					}
					if (!flag2)
					{
						break;
					}
					num10++;
				}
				for (int m = 0; m < width; m++)
				{
					bool flag3 = true;
					for (int n = 0; n < height; n++)
					{
						uint num16 = byteArray.GetDWord(n * num3 + m * num);
						if (alphaOnly)
						{
							num16 >>= 24;
						}
						if (num16 != 0)
						{
							flag3 = false;
							break;
						}
					}
					if (!flag3)
					{
						break;
					}
					num11++;
				}
				for (int num17 = width - 1; num17 >= 0; num17--)
				{
					bool flag4 = true;
					for (int num18 = 0; num18 < height; num18++)
					{
						uint num19 = byteArray.GetDWord(num18 * num3 + num17 * num);
						if (alphaOnly)
						{
							num19 >>= 24;
						}
						if (num19 != 0)
						{
							flag4 = false;
							break;
						}
					}
					if (!flag4)
					{
						break;
					}
					num12++;
				}
				if (num5 > num11)
				{
					num5 = num11;
				}
				if (num6 > num9)
				{
					num6 = num9;
				}
				if (num7 > num12)
				{
					num7 = num12;
				}
				if (num8 > num10)
				{
					num8 = num10;
				}
			}
			int maxRowLength = width - num7 - num5;
			int num20 = height - num8 - num6;
			int dWordAligned = (12 + (num20 << 2)).GetDWordAligned();
			return new RW3ActiveImageInfo
			{
				AlignX = num5,
				AlignY = num6,
				MaxRowLength = maxRowLength,
				MaxColumnLength = num20,
				ImageHeaderItemSize = dWordAligned,
				ImageCount = num4
			};
		}
	}
}
