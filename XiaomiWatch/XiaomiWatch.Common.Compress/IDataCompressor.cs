namespace XiaomiWatch.Common.Compress
{
	public interface IDataCompressor
	{
		bool HasCompression { get; }

		bool RequiredCompressWithParams { get; }

		byte[] Compress(byte[] data);

		byte[] Compress(byte[] data, int width, int height, uint type = 0u);

		byte[] GetHeader();
	}
}
