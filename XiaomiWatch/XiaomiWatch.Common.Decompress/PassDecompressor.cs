namespace XiaomiWatch.Common.Decompress
{
	public class PassDecompressor : IDataDecompressor
	{
		public bool HasCompression => false;

		public byte[] Decompress(byte[] data)
		{
			return data;
		}

		public byte[] Decompress(byte[] data, int width, int height, uint type = 0u)
		{
			return data;
		}

		public byte[] GetHeader()
		{
			return new byte[4];
		}
	}
}
