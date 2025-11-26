using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MiWatchCompiler.OldHelpers
{
	public static class BinaryExtender
	{
		public static string ExtendTo(this int val, int cnt)
		{
			string text = val.ToString();
			for (int i = text.Length; i < cnt; i++)
			{
				text = " " + text;
			}
			return text;
		}

		public static bool IsArabic(this string val)
		{
			if (string.IsNullOrEmpty(val))
			{
				return false;
			}
			for (int i = 0; i < val.Length; i++)
			{
				if ((val[i] & 0xFF00) == 65024)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetDWordAligned(this int v)
		{
			int num = v % 4;
			int num2 = v / 4 * 4;
			if (num <= 0)
			{
				return num2;
			}
			return num2 + 4;
		}

		public static uint GetDWordAligned(this uint v)
		{
			uint num = v % 4;
			uint num2 = v / 4 * 4;
			if (num == 0)
			{
				return num2;
			}
			return num2 + 4;
		}

		public static byte[] ReverseDWord(this byte[] data, int cprType)
		{
			if (cprType != 1)
			{
				return data;
			}
			for (int i = 0; i < data.Length - 4; i += 4)
			{
				byte b = data[i];
				byte b2 = data[i + 1];
				byte b3 = data[i + 2];
				byte b4 = data[i + 3];
				data[i] = b3;
				data[i + 1] = b2;
				data[i + 2] = b;
				data[i + 3] = b4;
			}
			return data;
		}

		public static byte[] To32Argb(this byte[] data, int cprType)
		{
			byte[] array = data;
			switch (cprType)
			{
			case 0:
			{
				int num3 = data.Length / 3 * 4;
				array = new byte[num3];
				int num4 = 0;
				for (int j = 0; j < num3 - 4; j += 4)
				{
					byte b = data[num4];
					byte b2 = data[num4 + 1];
					byte b3 = data[num4 + 2];
					byte b4 = byte.MaxValue;
					array[j] = b3;
					array[j + 1] = b2;
					array[j + 2] = b;
					array[j + 3] = b4;
					num4 += 3;
				}
				break;
			}
			case 3:
			{
				int num = data.Length * 4;
				array = new byte[num];
				int num2 = 0;
				for (int i = 0; i < num - 4; i += 4)
				{
					array[i] = data[num2];
					array[i + 1] = data[num2];
					array[i + 2] = data[num2];
					array[i + 3] = byte.MaxValue;
					num2++;
				}
				break;
			}
			}
			return array;
		}

		public static byte[] AlignByWord(this byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			int num = data.Length % 2;
			if (num > 0)
			{
				num = data.Length + 1;
				byte[] array = new byte[num];
				Array.Copy(data, array, num - 1);
				data = array;
			}
			return data;
		}

		public static byte[] AlignByDWord(this byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			if (data.Length % 4 > 0)
			{
				byte[] array = new byte[data.Length / 4 * 4 + 4];
				Array.Copy(data, array, data.Length);
				data = array;
			}
			return data;
		}

		public static byte[] AppendZero(this byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			Array.Resize(ref data, data.Length + 1);
			return data;
		}

		public static byte[] AppendZero(this byte[] data, int len)
		{
			if (data == null)
			{
				return null;
			}
			Array.Resize(ref data, data.Length + len);
			return data;
		}

		public static byte[] AppendZero(this byte[] data, uint len)
		{
			return data.AppendZero((int)len);
		}

		public static uint GetDWord(this byte[] data, uint offset = 0u, uint bigEndian = 0u)
		{
			byte[] array = new byte[4];
			Array.Copy(data, offset, array, 0L, array.Length);
			if (bigEndian == 1)
			{
				array = array.Reverse().ToArray();
			}
			return BitConverter.ToUInt32(array, 0);
		}

		public static void SetDWord(this byte[] data, uint offset = 0u, uint val = 0u, uint bigEndian = 0u)
		{
			byte[] array = BitConverter.GetBytes(val);
			if (bigEndian == 1)
			{
				array = array.Reverse().ToArray();
			}
			Array.Copy(array, 0L, data, offset, array.Length);
		}

		public static void SetDWord(this byte[] data, uint offset = 0u, int val = 0, uint bigEndian = 0u)
		{
			data.SetDWord(offset, (uint)val, bigEndian);
		}

		public static string GetAsciiString(this byte[] data, uint offset = 0u)
		{
			uint num;
			for (num = 0u; data[offset + num] != 0; num++)
			{
			}
			byte[] array = new byte[num];
			Array.Copy(data, offset, array, 0L, num);
			return Encoding.ASCII.GetString(array);
		}

		public static string GetUnicodeString(this byte[] data, uint offset = 0u, uint bigEndian = 0u)
		{
			uint num;
			for (num = 0u; data.GetWord(offset + num, bigEndian) != 0; num += 2)
			{
			}
			byte[] array = new byte[num];
			Array.Copy(data, offset, array, 0L, num);
			if (bigEndian == 1)
			{
				for (int i = 0; i < num; i += 2)
				{
					byte b = array[i + 1];
					array[i + 1] = array[i];
					array[i] = b;
				}
			}
			return Encoding.Unicode.GetString(array);
		}

		public static int SetUnicodeString(this byte[] data, uint offset = 0u, string str = "", uint bigEndian = 0u)
		{
			if (string.IsNullOrEmpty(str))
			{
				return 0;
			}
			byte[] array = Encoding.Unicode.GetBytes(str).AppendZero().AlignByDWord();
			int num = array.Length;
			if (bigEndian == 1)
			{
				for (int i = 0; i < num; i += 2)
				{
					byte b = array[i + 1];
					array[i + 1] = array[i];
					array[i] = b;
				}
			}
			Array.Copy(array, 0L, data, offset, num);
			return num;
		}

		public static int SetUnicodeStringNoAlign(this byte[] data, uint offset = 0u, string str = "", uint bigEndian = 0u)
		{
			if (string.IsNullOrEmpty(str))
			{
				return 0;
			}
			byte[] bytes = Encoding.Unicode.GetBytes(str);
			int num = bytes.Length;
			if (bigEndian == 1)
			{
				for (int i = 0; i < num; i += 2)
				{
					byte b = bytes[i + 1];
					bytes[i + 1] = bytes[i];
					bytes[i] = b;
				}
			}
			Array.Copy(bytes, 0L, data, offset, num);
			return num;
		}

		public static string GetUTF8String(this byte[] data, uint offset = 0u, int strlen = 0, uint bigEndian = 0u)
		{
			int i = 0;
			if (strlen > 0)
			{
				i = strlen;
			}
			else
			{
				for (; data[offset + i] != 0; i++)
				{
				}
			}
			byte[] array = new byte[i];
			Array.Copy(data, offset, array, 0L, i);
			if (bigEndian == 1)
			{
				for (int j = 0; j < i; j += 2)
				{
					byte b = array[j + 1];
					array[j + 1] = array[j];
					array[j] = b;
				}
			}
			return Encoding.UTF8.GetString(array);
		}

		public static byte[] GetUnicodeArray(this byte[] data, uint offset = 0u, uint bigEndian = 0u)
		{
			uint num;
			for (num = 0u; data.GetWord(offset + num, bigEndian) != 0; num += 2)
			{
			}
			byte[] array = new byte[num];
			Array.Copy(data, offset, array, 0L, num);
			if (bigEndian == 1)
			{
				for (int i = 0; i < num; i += 2)
				{
					byte b = array[i + 1];
					array[i + 1] = array[i];
					array[i] = b;
				}
			}
			return array;
		}

		public static string GetUnicodeMultiString(this byte[] data, uint[] offsetList)
		{
			return string.Join("|", offsetList.Select((uint o) => data.GetUnicodeString(o)));
		}

		public static ushort GetWord(this byte[] data, uint offset = 0u, uint bigEndian = 0u)
		{
			byte[] array = new byte[2];
			Array.Copy(data, offset, array, 0L, array.Length);
			if (bigEndian == 1)
			{
				array = array.Reverse().ToArray();
			}
			return (ushort)((array[1] << 8) | array[0]);
		}

		public static void SetWord(this byte[] data, uint offset = 0u, ushort val = 0, uint bigEndian = 0u)
		{
			byte[] array = BitConverter.GetBytes(val);
			if (bigEndian == 1)
			{
				array = array.Reverse().ToArray();
			}
			Array.Copy(array, 0L, data, offset, array.Length);
		}

		public static void SetWord(this byte[] data, uint offset = 0u, uint val = 0u, uint bigEndian = 0u)
		{
			data.SetWord(offset, (ushort)val, bigEndian);
		}

		public static void SetWord(this byte[] data, uint offset = 0u, int val = 0, uint bigEndian = 0u)
		{
			data.SetWord(offset, (ushort)val, bigEndian);
		}

		public static byte GetByte(this byte[] data, uint offset = 0u)
		{
			return data[offset];
		}

		public static byte[] GetByteArray(this byte[] data, uint offset = 0u, uint len = 1u)
		{
			byte[] array = new byte[len];
			Array.Copy(data, offset, array, 0L, array.Length);
			return array;
		}

		public static void SetByteArray(this byte[] data, int offset, byte[] src)
		{
			Array.Copy(src, 0, data, offset, src.Length);
		}

		public static void SetByteArray(this byte[] data, uint offset, byte[] src)
		{
			Array.Copy(src, 0L, data, offset, src.Length);
		}

		public static uint GetLeftAlignedDWord(this byte[] data)
		{
			int length = data.Length;
			byte[] array = new byte[4];
			Array.Copy(data, array, length);
			return (uint)((array[0] << 24) | (array[1] << 16) | (array[2] << 8) | array[3]);
		}

		public static byte[] GetBytes(this uint data, int length = 4)
		{
			byte[] sourceArray = BitConverter.GetBytes(data).Reverse().ToArray();
			byte[] array = new byte[length];
			Array.Copy(sourceArray, array, length);
			return array;
		}

		public static byte[] HexToByteArray(this string hexString, bool rev = true)
		{
			if (hexString.Length % 2 != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
			}
			byte[] array = new byte[hexString.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				string s = hexString.Substring(i * 2, 2);
				array[i] = byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			if (rev)
			{
				return array.Reverse().ToArray();
			}
			return array;
		}

		public static uint CountBits(this uint value)
		{
			uint num = value;
			num -= (num >> 1) & 0x55555555;
			num = (num & 0x33333333) + ((num >> 2) & 0x33333333);
			return ((num + (num >> 4)) & 0xF0F0F0F) * 16843009 >> 24;
		}

		public static uint CountLeadingZeroes(this uint x)
		{
			x |= x >> 1;
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x -= (x >> 1) & 0x55555555;
			x = ((x >> 2) & 0x33333333) + (x & 0x33333333);
			x = ((x >> 4) + x) & 0xF0F0F0F;
			x += x >> 8;
			x += x >> 16;
			return 32 - (x & 0x3F);
		}

		public static IEnumerable<string> SplitInParts(this string s, int partLength)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			if (partLength <= 0)
			{
				throw new ArgumentException("Part length has to be positive.", "partLength");
			}
			for (int i = 0; i < s.Length; i += partLength)
			{
				yield return s.Substring(i, Math.Min(partLength, s.Length - i));
			}
		}

		public static string ToHex(this byte[] bytes)
		{
			char[] array = new char[bytes.Length * 2];
			int num = 0;
			int num2 = 0;
			while (num < bytes.Length)
			{
				byte b = (byte)(bytes[num] >> 4);
				array[num2] = (char)((b > 9) ? (b + 55 + 32) : (b + 48));
				b = (byte)(bytes[num] & 0xF);
				array[++num2] = (char)((b > 9) ? (b + 55 + 32) : (b + 48));
				num++;
				num2++;
			}
			return new string(array);
		}

		public static byte[] HexToBytes(this string str)
		{
			if (str.Length == 0 || str.Length % 2 != 0)
			{
				return new byte[0];
			}
			byte[] array = new byte[str.Length / 2];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				char c = str[num2];
				array[num] = (byte)(((c > '9') ? ((c > 'Z') ? (c - 97 + 10) : (c - 65 + 10)) : (c - 48)) << 4);
				c = str[++num2];
				array[num] |= (byte)((c > '9') ? ((c > 'Z') ? (c - 97 + 10) : (c - 65 + 10)) : (c - 48));
				num++;
				num2++;
			}
			return array;
		}
	}
}
