using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	internal class MiBand8ProActionPacker : IActionPacker
	{
		private Dictionary<string, byte[]> actionList = new Dictionary<string, byte[]>
		{
			{
				"steps",
				new byte[6] { 49, 0, 3, 2, 55, 1 }
			},
			{
				"calories",
				new byte[6] { 50, 0, 3, 3, 55, 1 }
			},
			{
				"standing",
				new byte[6] { 51, 0, 3, 4, 55, 1 }
			},
			{
				"moving",
				new byte[6] { 52, 0, 3, 5, 55, 1 }
			},
			{
				"hrm",
				new byte[6] { 53, 0, 3, 6, 87, 0 }
			},
			{
				"spo2",
				new byte[6] { 54, 0, 1, 7, 7, 0 }
			},
			{
				"stress",
				new byte[6] { 55, 0, 1, 8, 55, 0 }
			},
			{
				"sleep",
				new byte[6] { 56, 0, 1, 0, 199, 0 }
			},
			{
				"weather",
				new byte[6] { 49, 48, 3, 11, 23, 2 }
			},
			{
				"alarm",
				new byte[6] { 49, 49, 1, 12, 135, 2 }
			},
			{
				"timer",
				new byte[6] { 49, 50, 1, 12, 103, 2 }
			},
			{
				"stopwatch",
				new byte[6] { 49, 51, 1, 0, 7, 2 }
			},
			{
				"cards",
				new byte[6] { 49, 52, 1, 12, 183, 2 }
			},
			{
				"music",
				new byte[6] { 49, 53, 1, 12, 87, 1 }
			},
			{
				"compass",
				new byte[6] { 50, 48, 1, 0, 135, 1 }
			},
			{
				"calendar",
				new byte[6] { 50, 49, 1, 1, 183, 1 }
			},
			{
				"flashlight",
				new byte[6] { 50, 50, 1, 0, 167, 1 }
			},
			{
				"photo",
				new byte[6] { 50, 51, 1, 0, 199, 1 }
			},
			{
				"findphone",
				new byte[6] { 50, 52, 1, 0, 119, 2 }
			},
			{
				"voice",
				new byte[6] { 53, 50, 1, 0, 211, 2 }
			},
			{
				"wechat",
				new byte[6] { 51, 48, 1, 0, 87, 2 }
			},
			{
				"alipay",
				new byte[6] { 51, 49, 1, 0, 231, 1 }
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
