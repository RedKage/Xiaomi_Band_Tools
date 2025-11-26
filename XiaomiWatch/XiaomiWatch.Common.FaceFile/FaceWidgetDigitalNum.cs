using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetDigitalNum : FaceWidget
	{
		[XmlAttribute]
		public new int Digits { get; set; }

		[XmlAttribute]
		public int Alignment { get; set; }

		[XmlAttribute]
		public int Spacing { get; set; }

		[XmlAttribute]
		public int Blanking { get; set; }

		[XmlAttribute("Value_Src")]
		public string DataSrcValue { get; set; }

		[XmlAttribute]
		public string BitmapList { get; set; }

		[XmlIgnore]
		public int BitmapListHashCode
		{
			get
			{
				if (string.IsNullOrWhiteSpace(BitmapList))
				{
					return 0;
				}
				string[] value = BitmapList.Split('|');
				return string.Join("", value).GetHashCode();
			}
		}

		[XmlIgnore]
		public override uint WidgetSize { get; set; } = 20u;

		public FaceWidgetDigitalNum()
		{
			base.Shape = 32;
		}

		public new static FaceWidgetDigitalNum Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int alpha = bin.GetByte(28);
			uint dWord3 = bin.GetDWord(44);
			byte[] byteArray = bin.GetByteArray(48u, dWord3);
			int num = byteArray.GetByte(0);
			int dataSrcDisplay = byteArray.GetByte(1);
			int spacing = byteArray.GetByte(2);
			int alignment = byteArray.GetByte(3);
			int digits = byteArray.GetByte(4);
			dWord3 -= 5;
			byteArray = byteArray.GetByteArray(5u, dWord3);
			string text = "";
			do
			{
				uint dWord4 = byteArray.GetDWord(0, 0u);
				text += $"img_{dWord4:D4}.png";
				dWord3 -= 4;
				if (dWord3 != 0)
				{
					text += "|";
					byteArray = byteArray.GetByteArray(4u, dWord3);
				}
			}
			while (dWord3 != 0);
			return new FaceWidgetDigitalNum
			{
				Shape = dWord2,
				Name = $"digitNum_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				Spacing = spacing,
				Alignment = alignment,
				Digits = digits,
				DataSrcValue = num.ToString(),
				DataSrcDisplay = dataSrcDisplay,
				BitmapList = text
			};
		}
	}
}
