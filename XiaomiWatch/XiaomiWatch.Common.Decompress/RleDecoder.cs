using System;

namespace XiaomiWatch.Common.Decompress
{
	public class RleDecoder
	{
		public static byte[] UncompressRLEv10(byte[] data, int destLen, byte recordSize = 4)
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
			}
			return array2;
		}

		public static byte[] UncompressRLEv11(byte[] data, int destLen, byte recordSize = 4)
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
			}
			return array2;
		}

		public static byte[] UncompressRLEvLVGL(byte[] data, int destLen)
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
						if (num >= data.Length - 1)
						{
							break;
						}
						b2 = (byte)(b & 0x7F);
						byte b3 = data[num];
						for (int i = 0; i < b2; i++)
						{
							array2[num2] = b3;
							num2++;
						}
						num++;
					}
					else
					{
						b2 = (byte)(b & 0x7F);
						array = data.GetByteArray(num, b2);
						array2.SetByteArray(num2, array);
						num2 += b2;
						num += b2;
					}
				}
				while (num < data.Length);
			}
			catch (Exception)
			{
			}
			return array2;
		}
	}
}
