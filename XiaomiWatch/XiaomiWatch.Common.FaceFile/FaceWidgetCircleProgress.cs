using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetCircleProgress : FaceWidget
	{
		[XmlAttribute]
		public int Radius { get; set; }

		[XmlAttribute("Rotate_xc")]
		public int RotateX { get; set; }

		[XmlAttribute("Rotate_yc")]
		public int RotateY { get; set; }

		[XmlAttribute]
		public int StartAngle { get; set; }

		[XmlAttribute]
		public int EndAngle { get; set; }

		[XmlAttribute("Line_Width")]
		public int LineWidth { get; set; }

		[XmlAttribute("Range_Min")]
		public int RangeMin { get; set; }

		[XmlAttribute("Range_Max")]
		public int RangeMax { get; set; }

		[XmlAttribute("Range_MinStep")]
		public int RangeMinStep { get; set; }

		[XmlAttribute("Range_Step")]
		public int RangeStep { get; set; }

		[XmlAttribute("Range_Max_Src")]
		public string DbSrcRangeMax { get; set; }

		[XmlAttribute("Range_Val_Src")]
		public string DbSrcRangeValue { get; set; }

		[XmlAttribute("Background_ImageName")]
		public string BackgroundImage { get; set; }

		[XmlAttribute("Foreground_ImageName")]
		public string ForegroundImage { get; set; }

		[XmlIgnore]
		public override uint WidgetSize { get; set; } = 40u;

		public FaceWidgetCircleProgress()
		{
			base.Shape = 29;
		}

		public new static FaceWidgetCircleProgress Get(byte[] bin)
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
			int dWord4 = (int)byteArray.GetDWord(0, 0u);
			int dWord5 = (int)byteArray.GetDWord(4);
			int dWord6 = (int)byteArray.GetDWord(8);
			int dWord7 = (int)byteArray.GetDWord(12);
			int dWord8 = (int)byteArray.GetDWord(16);
			int dWord9 = (int)byteArray.GetDWord(20);
			int word5 = byteArray.GetWord(24);
			int word6 = byteArray.GetWord(26);
			int word7 = byteArray.GetWord(28);
			int word8 = byteArray.GetWord(30);
			int dWord10 = (int)byteArray.GetDWord(32);
			int dWord11 = (int)byteArray.GetDWord(36);
			int num = byteArray.GetByte(44);
			int num2 = byteArray.GetByte(45);
			return new FaceWidgetCircleProgress
			{
				Shape = dWord2,
				Name = $"circleProgress_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				Radius = dWord4,
				RotateX = dWord5,
				RotateY = dWord6,
				LineWidth = dWord7,
				StartAngle = dWord8,
				EndAngle = dWord9,
				RangeMin = word5,
				RangeMax = word6,
				RangeMinStep = word7,
				RangeStep = word8,
				DbSrcRangeMax = num.ToString(),
				DbSrcRangeValue = num2.ToString(),
				BackgroundImage = ((dWord10 == -1) ? "" : $"img_{dWord10:D4}.png"),
				ForegroundImage = ((dWord11 == -1) ? "" : $"img_{dWord11:D4}.png")
			};
		}
	}
}
