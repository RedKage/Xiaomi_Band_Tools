using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	public interface IActionPacker
	{
		byte[] Pack(string actionName, byte[] data, uint actionOffset, FaceWidgetImage widget);

		int GetActionSize(string actionName);

		byte[] SetElement(byte[] data, uint offset, FaceWidget widget);
	}
}
