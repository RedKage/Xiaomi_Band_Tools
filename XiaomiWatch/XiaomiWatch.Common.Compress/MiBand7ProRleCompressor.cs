using System;

namespace XiaomiWatch.Common.Compress
{
	public class MiBand7ProRleCompressor : IDataCompressor
	{
		private bool isList;

		private bool withAlfa;

		public bool HasCompression => true;

		public bool RequiredCompressWithParams => false;

		public MiBand7ProRleCompressor(bool withAlfa, bool isList)
		{
			this.withAlfa = true;
			this.isList = isList;
		}

		public byte[] Compress(byte[] data)
		{
			if (data == null || data.Length == 0)
			{
				throw new ArgumentNullException("data");
			}
			byte[] data2 = new byte[8] { 224, 33, 165, 90, 0, 0, 0, 0 };
			data2.SetDWord(4, (data.Length << 4) | (withAlfa ? 4 : 2));
			byte[] array = RleEncoder.EncodeV11(data, withAlfa ? 4 : 2);
			byte[] array2 = data2.AppendZero(array.Length);
			array2.SetByteArray(8, array);
			return array2;
		}

		public byte[] Compress(byte[] data, int width, int height, uint type)
		{
			return Compress(data);
		}

		public byte[] GetHeader()
		{
			if (!isList)
			{
				return new byte[4] { 0, 8, 0, 0 };
			}
			return new byte[4] { 0, 0, 8, 0 };
		}
	}
}
