using System;
using XiaomiWatch.Common.Decompress;

namespace XiaomiWatch.Common.Compress
{
	internal class MiBand7ProRleDecompressor : IDataDecompressor
	{
		public bool HasCompression => true;

		public byte[] Decompress(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Decompress(byte[] data, int width, int height, uint type = 0u)
		{
			byte recordSize = (byte)(type & 0xF);
			int destLen = (int)(type >> 4);
			return RleDecoder.UncompressRLEv11(data, destLen, recordSize);
		}

		public byte[] GetHeader()
		{
			throw new NotImplementedException();
		}
	}
}
