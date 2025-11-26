using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetTextPlus : FaceWidget
	{
		[XmlAttribute]
		public int FontIndex { get; set; }

		[XmlAttribute]
		public string Color { get; set; }

		[XmlAttribute]
		public int MaxLength { get; set; }

		[XmlAttribute]
		public int Alignment { get; set; }

		[XmlAttribute]
		public string Data_Src_List { get; set; }

		[XmlAttribute]
		public string Format { get; set; }

		public new static FaceWidgetTextPlus Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int alpha = bin.GetByte(28);
			uint dWord3 = bin.GetDWord(44);
			bin.GetByteArray(48u, dWord3);
			return new FaceWidgetTextPlus
			{
				Shape = dWord2,
				Name = $"textPlus_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha
			};
		}
	}
}
