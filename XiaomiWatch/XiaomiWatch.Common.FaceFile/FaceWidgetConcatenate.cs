using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetConcatenate : FaceWidget
	{
		[XmlAttribute]
		public int Alignment { get; set; }

		[XmlAttribute]
		public int Spacing { get; set; }

		[XmlAttribute("AutoWidth_En")]
		public int AutoWidthEnabled { get; set; }

		public new static FaceWidgetConcatenate Get(byte[] bin)
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
			return new FaceWidgetConcatenate
			{
				Shape = dWord2,
				Name = $"concate_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha
			};
		}
	}
}
