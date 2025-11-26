using XiaomiWatch.Common.App;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	internal class MiBand8ActionPacker : IActionPacker
	{
		public int GetActionSize(string actionName)
		{
			return 48;
		}

		public byte[] Pack(string actionName, byte[] data, uint actionOffset, FaceWidgetImage widget)
		{
			data.SetDWord(actionOffset + 32, widget.ImageId);
			data.SetDWord(actionOffset + 40, 594678784);
			data.SetDWord(actionOffset + 44, AppHelper.GetAppActionId(actionName));
			return data;
		}

		public byte[] SetElement(byte[] data, uint offset, FaceWidget widget)
		{
			data.SetDWord(offset, widget.TargetId);
			data.SetWord(offset + 4, widget.X);
			data.SetWord(offset + 6, widget.Y);
			return data;
		}
	}
}
