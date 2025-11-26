using System;

namespace XiaomiWatch.Common.Compress.Deflate
{
	public interface IInflater : IDisposable
	{
		bool NeedsInput();

		void SetInput(byte[] inputBytes, byte[] inputTreeBytes);

		int Inflate(byte[] bytes, int offset, int length);

		bool Finished();
	}
}
