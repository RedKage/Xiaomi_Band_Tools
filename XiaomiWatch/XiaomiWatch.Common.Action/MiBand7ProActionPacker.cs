using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common.Action
{
	internal class MiBand7ProActionPacker : IActionPacker
	{
		private Dictionary<string, byte[]> actionList = new Dictionary<string, byte[]>
		{
			{
				"steps",
				new byte[26]
				{
					49, 0, 3, 0, 1, 0, 0, 0, 2, 0,
					0, 0, 0, 0, 55, 0, 54, 0, 0, 0,
					0, 0, 39, 0, 4, 0
				}
			},
			{
				"calories",
				new byte[26]
				{
					50, 0, 3, 0, 1, 0, 0, 0, 2, 0,
					0, 0, 0, 0, 55, 0, 54, 0, 0, 0,
					0, 0, 39, 0, 4, 0
				}
			},
			{
				"standing",
				new byte[26]
				{
					51, 0, 3, 0, 1, 0, 0, 0, 2, 0,
					0, 0, 0, 0, 55, 0, 54, 0, 0, 0,
					0, 0, 39, 0, 4, 0
				}
			},
			{
				"moving",
				new byte[26]
				{
					52, 0, 3, 0, 1, 0, 0, 0, 2, 0,
					0, 0, 0, 0, 55, 0, 54, 0, 0, 0,
					0, 0, 39, 0, 4, 0
				}
			},
			{
				"hrm",
				new byte[26]
				{
					54, 0, 3, 0, 1, 0, 4, 0, 0, 0,
					0, 0, 0, 0, 57, 0, 64, 0, 0, 0,
					0, 0, 38, 0, 17, 0
				}
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
				new byte[26]
				{
					49, 50, 3, 0, 1, 0, 4, 0, 0, 0,
					0, 0, 0, 0, 37, 0, 14, 0, 0, 0,
					0, 0, 57, 0, 59, 0
				}
			},
			{
				"alarm",
				new byte[6] { 49, 57, 1, 12, 135, 2 }
			},
			{
				"music",
				new byte[6] { 50, 52, 1, 12, 87, 1 }
			}
		};

		public int GetActionSize(string actionName)
		{
			return 68;
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
				data.SetDWord(actionOffset + 56, widget.ImageId);
				data.SetDWord(actionOffset + 64, widget.ImageId);
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
