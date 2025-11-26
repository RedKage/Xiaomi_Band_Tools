using System;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	public class DefaultActionPacker : IActionPacker
	{
		public int GetActionSize(string actionName)
		{
			throw new NotImplementedException();
		}

		public byte[] Pack(string actionName, byte[] data, uint actionOffset, FaceWidgetImage widget)
		{
			throw new NotImplementedException();
		}

		public byte[] SetElement(byte[] data, uint offset, FaceWidget widget)
		{
			throw new NotImplementedException();
		}
	}
}
