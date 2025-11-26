using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetImageCacheList : FaceWidget
	{
		[XmlAttribute]
		public string BitmapList { get; set; }

		public new static FaceWidgetImageCacheList Get(byte[] bin)
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
			byteArray.GetByte(16);
			dWord3 -= 17;
			byteArray = byteArray.GetByteArray(17u, dWord3);
			string text = "img_00{";
			int num = -1;
			string text2 = "";
			do
			{
				num = (int)byteArray.GetDWord(0, 0u);
				text2 = $"{num:D2}";
				text += text2;
				dWord3 -= 4;
				if (dWord3 != 0)
				{
					text += ",";
					byteArray = byteArray.GetByteArray(4u, dWord3);
				}
			}
			while (dWord3 != 0);
			text += "}.png";
			return new FaceWidgetImageCacheList
			{
				Shape = dWord2,
				Name = $"imageCacheList_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				DataSrcDisplay = word5,
				BitmapList = text
			};
		}
	}
}
