using System;

namespace XiaomiWatch.Common.Compress.Deflate
{
	public interface IDeflater : IDisposable
	{
		bool NeedsInput();

		void SetInput(byte[] inputBuffer);

		(int huffman, int data) GetDeflateOutput(byte[] huffmanBuffer, byte[] outputBuffer);

		bool Finish(byte[] outputBuffer, out int bytesRead);
	}
}
