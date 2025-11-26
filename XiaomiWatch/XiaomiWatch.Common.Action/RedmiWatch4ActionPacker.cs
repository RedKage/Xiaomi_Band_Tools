using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	internal class RedmiWatch4ActionPacker : IActionPacker
	{
		private Dictionary<string, byte[]> actionList = new Dictionary<string, byte[]>
		{
			{
				"steps",
				new byte[6] { 49, 0, 1, 0, 51, 1 }
			},
			{
				"calories",
				new byte[6] { 50, 0, 1, 0, 51, 1 }
			},
			{
				"standing",
				new byte[6] { 51, 0, 1, 0, 51, 1 }
			},
			{
				"moving",
				new byte[6] { 52, 0, 1, 0, 51, 1 }
			},
			{
				"hrm",
				new byte[6] { 53, 0, 1, 0, 83, 0 }
			},
			{
				"spo2",
				new byte[6] { 54, 0, 1, 0, 227, 0 }
			},
			{
				"stress",
				new byte[6] { 55, 0, 1, 0, 51, 0 }
			},
			{
				"sleep",
				new byte[6] { 56, 0, 1, 0, 195, 0 }
			},
			{
				"weather",
				new byte[6] { 49, 48, 5, 0, 19, 2 }
			},
			{
				"alarm",
				new byte[6] { 49, 49, 1, 0, 131, 2 }
			},
			{
				"timer",
				new byte[6] { 49, 50, 1, 0, 99, 2 }
			},
			{
				"stopwatch",
				new byte[6] { 49, 51, 1, 0, 3, 2 }
			},
			{
				"cards",
				new byte[6] { 49, 52, 1, 12, 179, 2 }
			},
			{
				"music",
				new byte[6] { 49, 53, 1, 12, 83, 1 }
			},
			{
				"compass",
				new byte[6] { 50, 48, 1, 0, 131, 1 }
			},
			{
				"scheduler",
				new byte[6] { 50, 49, 0, 0, 179, 1 }
			},
			{
				"flashlight",
				new byte[6] { 50, 50, 1, 0, 163, 1 }
			},
			{
				"photo",
				new byte[6] { 50, 51, 1, 0, 195, 1 }
			},
			{
				"findphone",
				new byte[6] { 50, 52, 1, 0, 115, 2 }
			},
			{
				"breath",
				new byte[6] { 50, 53, 1, 0, 67, 0 }
			},
			{
				"contact",
				new byte[6] { 50, 54, 1, 0, 227, 2 }
			},
			{
				"egg",
				new byte[6] { 50, 55, 1, 0, 51, 3 }
			},
			{
				"todo",
				new byte[6] { 50, 56, 1, 0, 195, 3 }
			},
			{
				"tomato",
				new byte[6] { 50, 57, 1, 0, 163, 3 }
			},
			{
				"worldclock",
				new byte[6] { 53, 48, 1, 0, 179, 3 }
			},
			{
				"phone",
				new byte[6] { 53, 49, 1, 0, 51, 2 }
			},
			{
				"voice",
				new byte[6] { 53, 50, 1, 0, 211, 2 }
			},
			{
				"mihome",
				new byte[6] { 53, 51, 1, 0, 243, 3 }
			},
			{
				"women",
				new byte[6] { 53, 52, 1, 0, 243, 1 }
			},
			{
				"pressure",
				new byte[6] { 53, 53, 1, 0, 51, 0 }
			},
			{
				"vitality",
				new byte[6] { 53, 54, 1, 0, 99, 3 }
			},
			{
				"settings",
				new byte[6] { 53, 55, 1, 0, 99, 1 }
			},
			{
				"launcher",
				new byte[6] { 53, 58, 1, 0, 19, 0 }
			},
			{
				"wechat",
				new byte[6] { 51, 48, 1, 0, 83, 2 }
			},
			{
				"alipay",
				new byte[6] { 51, 49, 1, 0, 227, 1 }
			},
			{
				"checktool",
				new byte[6] { 52, 48, 1, 0, 19, 4 }
			},
			{
				"calendar",
				new byte[6] { 52, 49, 1, 0, 83, 4 }
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
				data.SetDWord(actionOffset + 48, widget.ImageId);
				data.SetWord(actionOffset + 52, widget.Width);
				data.SetWord(actionOffset + 52 + 2, widget.Height);
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
