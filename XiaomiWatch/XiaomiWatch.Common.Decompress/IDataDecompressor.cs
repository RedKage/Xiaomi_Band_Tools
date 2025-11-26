namespace XiaomiWatch.Common.Decompress
{
	public interface IDataDecompressor
	{
		bool HasCompression { get; }

		byte[] Decompress(byte[] data);

		byte[] Decompress(byte[] data, int width, int height, uint type = 0u);

		byte[] GetHeader();
	}
}
