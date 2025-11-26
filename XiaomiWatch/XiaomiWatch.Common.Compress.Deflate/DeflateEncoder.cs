using System.Collections.Generic;

namespace XiaomiWatch.Common.Compress.Deflate
{
	internal class DeflateEncoder
	{
		internal static List<byte[]> Encode(byte[] uncompressed, int height)
		{
			byte[] array = new byte[uncompressed.Length];
			byte[] array2 = new byte[256];
			List<byte[]> list = new List<byte[]>();
			MultiPartDeflater multiPartDeflater = new MultiPartDeflater();
			multiPartDeflater.SetInput(uncompressed);
			while (!multiPartDeflater.NeedsInput())
			{
				(int, int) deflateOutput = multiPartDeflater.GetDeflateOutput(array2, array);
				int len = multiPartDeflater.Finish();
				array2 = array2.GetByteArray(0, deflateOutput.Item1);
				list.Add(array2);
				array = array.GetByteArray(0, len);
				array = array.ShiftLeft(3);
				array[0] |= 5;
				list.Add(array);
			}
			return list;
		}
	}
}
