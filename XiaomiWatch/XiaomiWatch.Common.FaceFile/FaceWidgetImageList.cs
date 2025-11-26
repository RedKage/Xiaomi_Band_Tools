using System.Linq;
using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetImageList : FaceWidget
	{
		private int[] customValues;

		private bool? hasCustomValues;

		[XmlAttribute("Index_Src")]
		public string DataSrcIndex { get; set; }

		[XmlAttribute]
		public int DefaultIndex { get; set; }

		[XmlAttribute]
		public string BitmapList { get; set; }

		[XmlIgnore]
		public int[] CustomValues
		{
			get
			{
				if (customValues == null)
				{
					return new int[0];
				}
				return customValues;
			}
		}

		[XmlIgnore]
		public bool HasCustomValues
		{
			get
			{
				if (hasCustomValues.HasValue)
				{
					return hasCustomValues.Value;
				}
				hasCustomValues = false;
				if (string.IsNullOrEmpty(BitmapList))
				{
					return hasCustomValues.Value;
				}
				if (BitmapList.Length > 0)
				{
					string[] array = BitmapList.Split('|');
					if (array != null && array.Length != 0)
					{
						int[] array2 = new int[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							string[] array3 = array[i].Split(':');
							if (array3 == null || array3.Length == 0)
							{
								continue;
							}
							string text = array3[0].Replace("(", "").Replace(")", "");
							if (text.Length > 0)
							{
								array2[i] = int.Parse(text);
								if (array2[i] != i)
								{
									hasCustomValues = true;
								}
							}
							else
							{
								array2[i] = i;
							}
						}
						hasCustomValues = true;
						if (hasCustomValues == true)
						{
							customValues = array2;
							WidgetSize = (uint)(16 + array2.Length * 4);
						}
					}
				}
				return hasCustomValues.Value;
			}
		}

		[XmlIgnore]
		public int BitmapListHashCode
		{
			get
			{
				if (string.IsNullOrWhiteSpace(BitmapList))
				{
					return 0;
				}
				string[] source = BitmapList.Split('|');
				source = source.Select((string s) => s.Split(':')[1]).ToArray();
				return string.Join("", source).GetHashCode();
			}
		}

		public FaceWidgetImageList()
		{
			base.Shape = 31;
		}

		public new static FaceWidgetImageList Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int word5 = bin.GetWord(24);
			int alpha = bin.GetByte(28);
			uint dWord3 = bin.GetDWord(44);
			byte[] byteArray = bin.GetByteArray(48u, dWord3);
			int defaultIndex = byteArray.GetByte(0);
			int num = byteArray.GetByte(1);
			byteArray.GetByte(2);
			dWord3 -= 3;
			byteArray = byteArray.GetByteArray(3u, dWord3);
			string text = "";
			int num2 = -1;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			string text2 = "";
			string text3 = "";
			do
			{
				num2 = (int)byteArray.GetDWord(0, 0u);
				if (num2 == num3)
				{
					num4 = (int)byteArray.GetDWord(4);
					num5 = (int)byteArray.GetDWord(8);
					num6 = (int)byteArray.GetDWord(12);
					num7 = (int)byteArray.GetDWord(16);
					text3 = ((num4 != num5) ? (text3 + $",{num4}/{num5}") : (text3 + $",{num4}"));
					string arg = "";
					if (num6 != word || num7 != word2)
					{
						arg = $"[{num6},{num7}]";
					}
					text = text.Substring(0, text.Length - text2.Length);
					if (text.Length > 0)
					{
						text = text.Substring(0, text.Length - 1);
					}
					text2 = $"({text3}){arg}:img_{num2:D4}.png";
				}
				else
				{
					num4 = (int)byteArray.GetDWord(4);
					num5 = (int)byteArray.GetDWord(8);
					num6 = (int)byteArray.GetDWord(12);
					num7 = (int)byteArray.GetDWord(16);
					text3 = "";
					text3 = ((num4 != num5) ? $"{num4}/{num5}" : $"{num4}");
					string arg2 = "";
					if (num6 != word || num7 != word2)
					{
						arg2 = $"[{num6},{num7}]";
					}
					text2 = $"({text3}){arg2}:img_{num2:D4}.png";
				}
				text += text2;
				dWord3 -= 20;
				if (dWord3 != 0)
				{
					text += "|";
					byteArray = byteArray.GetByteArray(20u, dWord3);
				}
				num3 = num2;
			}
			while (dWord3 != 0);
			return new FaceWidgetImageList
			{
				Shape = dWord2,
				Name = $"imageList_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				DataSrcDisplay = word5,
				DataSrcIndex = num.ToString(),
				DefaultIndex = defaultIndex,
				BitmapList = text,
				WidgetSize = 16u
			};
		}
	}
}
