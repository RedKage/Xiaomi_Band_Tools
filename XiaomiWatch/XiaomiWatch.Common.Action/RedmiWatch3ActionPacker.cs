using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	internal class RedmiWatch3ActionPacker : IActionPacker
	{
		private Dictionary<string, byte[]> actionList = new Dictionary<string, byte[]>
		{
			{
				"steps",
				new byte[6] { 49, 0, 1, 1, 55, 1 }
			},
			{
				"calories",
				new byte[6] { 50, 0, 1, 2, 55, 1 }
			},
			{
				"standing",
				new byte[6] { 51, 0, 1, 3, 55, 1 }
			},
			{
				"moving",
				new byte[6] { 52, 0, 1, 4, 55, 1 }
			},
			{
				"hrm",
				new byte[6] { 54, 0, 1, 6, 87, 0 }
			},
			{
				"spo2",
				new byte[6] { 55, 0, 1, 7, 7, 0 }
			},
			{
				"stress",
				new byte[6] { 56, 0, 1, 8, 55, 0 }
			},
			{
				"weather",
				new byte[6] { 49, 50, 1, 11, 23, 2 }
			},
			{
				"alarm",
				new byte[6] { 49, 57, 1, 12, 135, 2 }
			},
			{
				"music",
				new byte[6] { 50, 52, 1, 12, 87, 1 }
			},
			{
				"compass",
				new byte[6] { 57, 49, 1, 0, 135, 1 }
			},
			{
				"wechat",
				new byte[6] { 57, 50, 1, 0, 87, 2 }
			},
			{
				"8703",
				new byte[6] { 57, 51, 1, 0, 135, 3 }
			}
		};

		public int GetActionSize(string actionName)
		{
			return 56;
		}

		public byte[] Pack(string actionName, byte[] data, uint actionOffset, FaceWidgetImage widget)
		{
			string key = actionName.ToLower();
			if (actionList.ContainsKey(key))
			{
				byte[] source = actionList[key];
				data.SetByteArray(actionOffset, source.Take(2).ToArray());
				data.SetByteArray(actionOffset + 40, source.Skip(2).ToArray());
				data.SetWord(actionOffset + 44, widget.X);
				data.SetWord(actionOffset + 44 + 2, widget.Y);
				data.SetDWord(actionOffset + 36, widget.ImageId);
				data.SetDWord(actionOffset + 48, widget.ImageId);
				data.SetDWord(actionOffset + 52, widget.ImageId);
			}
			return data;
		}

		public byte[] SetElement(byte[] data, uint offset, FaceWidget widget)
		{
			data.SetDWord(offset, widget.ActionId);
			return data;
		}
	}
}
