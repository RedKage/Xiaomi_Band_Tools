namespace XiaomiWatch.Common.Compress.Deflate
{
	internal interface FileFormats
	{
		byte[] GetHeader();

		void UpdateWithBytesRead(byte[] buffer, int offset, int bytesToCopy);

		byte[] GetFooter();
	}
}
