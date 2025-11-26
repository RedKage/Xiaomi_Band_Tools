using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetImage : FaceWidget
	{
		[XmlAttribute]
		public string Bitmap { get; set; }

		public FaceWidgetImage()
		{
			base.Shape = 30;
		}

		public new static FaceWidgetImage Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int alpha = bin.GetByte(28);
			uint dWord3 = bin.GetDWord(44);
			int dWord4 = (int)bin.GetByteArray(48u, dWord3).GetDWord(0, 0u);
			return new FaceWidgetImage
			{
				Shape = dWord2,
				Name = $"image_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				Bitmap = ((dWord4 == -1) ? "" : $"img_{dWord4:D4}.png")
			};
		}
	}
}
