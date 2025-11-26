using System;

namespace XiaomiWatch.Common.Decompress
{
	internal class RedmiWatch5ActiveDecompressor : IDataDecompressor
	{
		public bool HasCompression => true;

		public byte[] Decompress(byte[] data)
		{
			throw new NotImplementedException();
		}

		public byte[] Decompress(byte[] data, int width, int height, uint type = 0u)
		{
			throw new NotImplementedException();
		}

		public byte[] GetHeader()
		{
			throw new NotImplementedException();
		}
	}
}
