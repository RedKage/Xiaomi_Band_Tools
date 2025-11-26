using System;
using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.Compress.Deflate;

namespace XiaomiWatch.Common.Compress
{
	internal class RedmiWatch5ActiveCompressor : IDataCompressor
	{
		private bool isList;

		private bool noCompress;

		private bool withAlfa;

		private uint[] colorTable;

		private byte compressTypeUsed;

		public bool HasCompression => true;

		public bool RequiredCompressWithParams => true;

		public RedmiWatch5ActiveCompressor(bool withAlfa, bool isList, bool noCompress = false)
		{
			this.withAlfa = true;
			this.isList = isList;
			this.noCompress = noCompress;
		}

		public byte[] Compress(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Compress(byte[] data, int width, int height, uint type = 0u)
		{
			colorTable = (from i in Enumerable.Range(0, data.Length / 4)
				select (uint)(data[i * 4] | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24))).Distinct().ToArray();
			byte[] array = Array.Empty<byte>();
			if (isList)
			{
				return EncodeListDeflate(data, width, height);
			}
			return EncodeDeflate(data, width, height, type);
		}

		private byte[] EncodeDeflate(byte[] data, int width, int height, uint type)
		{
			compressTypeUsed = 1;
			_ = 2;
			_ = 4;
			byte[] array = new byte[24];
			byte[] array2 = new byte[8];
			byte[] inputData = ConvertToIndexed8Bit(data, colorTable);
			inputData = AddPadding(inputData, width, height, 1m);
			List<byte[]> list = DeflateEncoder.Encode(inputData, height);
			int val = array.Length - 8 + (colorTable.Length << 2) + array2.Length + list.Sum((byte[] c) => c.Length.GetDWordAligned());
			int val2 = array.Length - 8 + (colorTable.Length << 2) + array2.Length + list[0].Length.GetDWordAligned();
			array.SetDWord(0, width * height);
			array.SetDWord(8, val, 1u);
			array.SetDWord(12, 1124605952, 1u);
			array.SetWord(16, width, 1u);
			array.SetWord(18, height, 1u);
			array.SetDWord(20, colorTable.Length << 8);
			array2.SetWord(0, height, 1u);
			array2.SetWord(2, list.Count - 1, 1u);
			array2.SetDWord(4, val2, 1u);
			byte[] first = array.Concat(colorTable.SelectMany((uint color) => BitConverter.GetBytes(color))).ToArray();
			first = first.Concat(array2).ToArray();
			foreach (byte[] item in list)
			{
				first = first.Concat(item.AlignByDWord()).ToArray();
			}
			return first;
		}

		private byte[] EncodeListDeflate(byte[] data, int width, int height)
		{
			if (colorTable.Length > 256)
			{
				throw new ArgumentOutOfRangeException("Colors count is more than 256 pcs, unsupported now");
			}
			compressTypeUsed = 1;
			byte[] array = new byte[24];
			byte[] array2 = new byte[8];
			byte[] inputData = ConvertToIndexed8Bit(data, colorTable);
			inputData = AddPadding(inputData, width, height, 1m);
			List<byte[]> list = DeflateEncoder.Encode(inputData, height);
			int val = array.Length - 8 + (colorTable.Length << 2) + array2.Length + list.Sum((byte[] c) => c.Length.GetDWordAligned());
			int val2 = array.Length - 8 + (colorTable.Length << 2) + array2.Length + list[0].Length.GetDWordAligned();
			array.SetDWord(0, width * height);
			array.SetDWord(8, val, 1u);
			array.SetDWord(12, 1124605952, 1u);
			array.SetWord(16, width, 1u);
			array.SetWord(18, height, 1u);
			array.SetDWord(20, colorTable.Length << 8);
			array2.SetWord(0, height, 1u);
			array2.SetWord(2, list.Count - 1, 1u);
			array2.SetDWord(4, val2, 1u);
			byte[] first = array.Concat(colorTable.SelectMany((uint color) => BitConverter.GetBytes(color))).ToArray();
			first = first.Concat(array2).ToArray();
			foreach (byte[] item in list)
			{
				first = first.Concat(item.AlignByDWord()).ToArray();
			}
			return first;
		}

		private byte[] EncodeNoCompression(byte[] data, int width, int height)
		{
			if (colorTable.Length > 256)
			{
				throw new ArgumentOutOfRangeException("Colors count is more than 256 pcs, unsupported now");
			}
			compressTypeUsed = 0;
			byte[] array = new byte[12];
			byte[] array2 = ConvertToIndexed8Bit(data, colorTable);
			array.SetDWord(0, array2.Length);
			array.SetDWord(8, colorTable.Length);
			return array.Concat(colorTable.SelectMany((uint color) => BitConverter.GetBytes(color))).Concat(array2).ToArray();
		}

		private byte[] ConvertToIndexed8Bit(byte[] data, uint[] colorTable)
		{
			IEnumerable<uint> source = from i in Enumerable.Range(0, data.Length / 4)
				select (uint)(data[i * 4] | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24));
			Dictionary<uint, byte> colorMap = colorTable.Select((uint color, int index) => new { color, index }).ToDictionary(x => x.color, x => (byte)x.index);
			return source.Select((uint pixel) => colorMap[pixel]).ToArray();
		}

		private byte[] AddPadding(byte[] inputData, int width, int height, decimal bytes = 3m)
		{
			int num = (int)Math.Ceiling((decimal)width * bytes);
			int num2 = (int)Math.Ceiling((decimal)width * bytes) + 1;
			byte[] array = new byte[num2 * height];
			if (inputData.Length < num * height)
			{
				inputData = inputData.Concat(new byte[height]).ToArray();
			}
			for (int i = 0; i < height; i++)
			{
				Array.Copy(inputData, i * num, array, 1 + i * num2, num);
			}
			return array;
		}

		public byte[] GetHeader()
		{
			if (isList)
			{
				if (compressTypeUsed == 0)
				{
					return new byte[4] { 16, 0, 0, 0 };
				}
				return new byte[4] { 40, 0, 12, 0 };
			}
			if (compressTypeUsed == 0)
			{
				return new byte[4] { 16, 0, 0, 0 };
			}
			return new byte[4] { 40, 12, 0, 0 };
		}
	}
}
