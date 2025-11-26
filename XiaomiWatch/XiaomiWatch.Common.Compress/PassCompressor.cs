namespace XiaomiWatch.Common.Compress
{
	public class PassCompressor : IDataCompressor
	{
		public bool HasCompression => false;

		public bool RequiredCompressWithParams => false;

		public byte[] Compress(byte[] data)
		{
			return data;
		}

		public byte[] Compress(byte[] data, int width, int height, uint type)
		{
			return Compress(data);
		}

		public byte[] GetHeader()
		{
			return new byte[4];
		}
	}
}
