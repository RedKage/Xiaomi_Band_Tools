using System;
using System.Collections.Generic;
using System.Linq;

namespace XiaomiWatch.Common.Compress
{
	public class RleEncoder
	{
		private const int MinLen = 4;

		private const int MaxLen = 127;

		public static byte[] EncodeV10(byte[] source, int bytes = 2)
		{
			switch (bytes)
			{
			case 4:
				return EncodeV10_4(source);
			case 3:
				return EncodeV10_3(source);
			case 2:
				return EncodeV10_2(source);
			case 1:
				return EncodeV10_1(source);
			default:
				throw new NotSupportedException($"Encoding size: {bytes} is not supported");
			}
		}

		public static byte[] EncodeV11(byte[] source, int bytes = 2)
		{
			if (bytes != 4)
			{
				return EncodeV11_2(source);
			}
			return EncodeV11_4(source);
		}

		private static byte[] EncodeV10_5(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 4)
			{
				byte b = 4;
				while (b < 127 && i + 4 < source.Length && source[i] == source[i + 4] && source[i + 1] == source[i + 5] && source[i + 2] == source[i + 6] && source[i + 3] == source[i + 7])
				{
					b += 4;
					i += 4;
				}
				if (b == 4)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
				list.Add(source[i + 2]);
				list.Add(source[i + 3]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV10_4(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 4)
			{
				byte b = 1;
				while (b < 127 && i + 4 < source.Length && source[i] == source[i + 4] && source[i + 1] == source[i + 5] && source[i + 2] == source[i + 6] && source[i + 3] == source[i + 7])
				{
					b++;
					i += 4;
				}
				if (b == 1)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
				list.Add(source[i + 2]);
				list.Add(source[i + 3]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV10_3(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 3)
			{
				byte b = 1;
				while (b < 127 && i + 3 < source.Length && source[i] == source[i + 3] && source[i + 1] == source[i + 4] && source[i + 2] == source[i + 5])
				{
					b++;
					i += 3;
				}
				if (b == 1)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
				list.Add(source[i + 2]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV10_2(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 2)
			{
				byte b = 1;
				while (b < 127 && i + 2 < source.Length && source[i] == source[i + 2] && source[i + 1] == source[i + 3])
				{
					b++;
					i += 2;
				}
				if (b == 1)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV10_1(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i++)
			{
				byte b = 1;
				while (b < 127 && i + 1 < source.Length && source[i] == source[i + 1])
				{
					b++;
					i++;
				}
				if (b == 1)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV11_4(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 4)
			{
				byte b = 1;
				while (b <= 127 && i + 4 < source.Length && source[i] == source[i + 4] && source[i + 1] == source[i + 5] && source[i + 2] == source[i + 6] && source[i + 3] == source[i + 7])
				{
					b++;
					i += 4;
				}
				b--;
				if (b > 0)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
				list.Add(source[i + 2]);
				list.Add(source[i + 3]);
			}
			return list.ToArray();
		}

		private static byte[] EncodeV11_2(byte[] source)
		{
			List<byte> list = new List<byte>();
			for (int i = 0; i < source.Length; i += 2)
			{
				byte b = 1;
				while (b < 127 && i + 2 < source.Length && source[i] == source[i + 2] && source[i + 1] == source[i + 3])
				{
					b++;
					i += 2;
				}
				b--;
				if (b > 0)
				{
					b |= 0x80;
				}
				list.Add(b);
				list.Add(source[i]);
				list.Add(source[i + 1]);
			}
			return list.ToArray();
		}

		public static byte[] EncodeLVGL(byte[] source, byte bytes = 1)
		{
			List<byte> list = new List<byte>();
			List<byte> list2 = new List<byte>();
			for (int i = 0; i < source.Length; i++)
			{
				bool flag = false;
				byte b = 1;
				while (i + 1 < source.Length)
				{
					list2.Add(source[i]);
					if (source[i] != source[i + 1])
					{
						break;
					}
					i++;
					b++;
					if (b > 4)
					{
						list2 = list2.Take(list2.Count - 4).ToList();
						if (list2.Count > 0)
						{
							foreach (byte[] item in list2.ToArray().Split(127))
							{
								list.Add((byte)(item.Length | 0x80));
								list.AddRange(item);
							}
						}
						list2.Clear();
					}
					if (b == 127)
					{
						list.Add(127);
						list.Add(source[i]);
						flag = true;
						list2.Clear();
						break;
					}
				}
				if (flag || b < 4)
				{
					continue;
				}
				if (b == 1)
				{
					b |= 0x80;
				}
				list2 = list2.Take(list2.Count - 4).ToList();
				if (list2.Count > 0)
				{
					foreach (byte[] item2 in list2.ToArray().Split(127))
					{
						list.Add((byte)(item2.Length | 0x80));
						list.AddRange(item2);
					}
				}
				list2.Clear();
				list.Add(b);
				list.Add(source[i]);
			}
			return list.ToArray();
		}
	}
}
