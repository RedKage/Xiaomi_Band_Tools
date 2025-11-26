using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetAnalogClock : FaceWidget
	{
		[XmlAttribute("HourHandCorrection_En")]
		public int CorrectionEnableHourHand { get; set; }

		[XmlAttribute("MinuteHandCorrection_En")]
		public int CorrectionEnableMinuteHand { get; set; }

		[XmlAttribute("Background_ImageName")]
		public string BackgroundImage { get; set; }

		[XmlAttribute("BgImage_rotate_xc")]
		public int BackImageRotateX { get; set; }

		[XmlAttribute("BgImage_rotate_yc")]
		public int BackImageRotateY { get; set; }

		[XmlAttribute("HourHand_ImageName")]
		public string HourHandImage { get; set; }

		[XmlAttribute("HourImage_rotate_xc")]
		public int HourImageRotateX { get; set; }

		[XmlAttribute("HourImage_rotate_yc")]
		public int HourImageRotateY { get; set; }

		[XmlAttribute("MinuteHand_Image")]
		public string MinuteHandImage { get; set; }

		[XmlAttribute("MinuteImage_rotate_xc")]
		public int MinuteImageRotateX { get; set; }

		[XmlAttribute("MinuteImage_rotate_yc")]
		public int MinuteImageRotateY { get; set; }

		[XmlAttribute("SecondHand_Image")]
		public string SecondHandImage { get; set; }

		[XmlAttribute("SecondImage_rotate_xc")]
		public int SecondImageRotateX { get; set; }

		[XmlAttribute("SecondImage_rotate_yc")]
		public int SecondImageRotateY { get; set; }

		[XmlIgnore]
		public uint HourTargetId { get; set; }

		[XmlIgnore]
		public uint MinuteTargetId { get; set; }

		[XmlIgnore]
		public uint SecondTargetId { get; set; }

		[XmlIgnore]
		public uint HourImageId { get; set; }

		[XmlIgnore]
		public uint MinuteImageId { get; set; }

		[XmlIgnore]
		public uint SecondImageId { get; set; }

		[XmlIgnore]
		public override uint WidgetSize { get; set; } = 32u;

		public FaceWidgetAnalogClock()
		{
			base.Shape = 27;
			base.X = 0;
			base.Y = 0;
		}

		public new static FaceWidgetAnalogClock Get(byte[] bin)
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
			int dWord4 = (int)byteArray.GetDWord(0, 0u);
			int dWord5 = (int)byteArray.GetDWord(4);
			int dWord6 = (int)byteArray.GetDWord(8);
			int dWord7 = (int)byteArray.GetDWord(12);
			int dWord8 = (int)byteArray.GetDWord(16);
			int dWord9 = (int)byteArray.GetDWord(20);
			int dWord10 = (int)byteArray.GetDWord(24);
			int dWord11 = (int)byteArray.GetDWord(28);
			int dWord12 = (int)byteArray.GetDWord(32);
			int dWord13 = (int)byteArray.GetDWord(36);
			int dWord14 = (int)byteArray.GetDWord(40);
			int dWord15 = (int)byteArray.GetDWord(44);
			int correctionEnableHourHand = byteArray.GetByte(48);
			int correctionEnableMinuteHand = byteArray.GetByte(49);
			return new FaceWidgetAnalogClock
			{
				Shape = dWord2,
				Name = $"analogClock_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				DataSrcDisplay = word5,
				CorrectionEnableHourHand = correctionEnableHourHand,
				CorrectionEnableMinuteHand = correctionEnableMinuteHand,
				BackgroundImage = ((dWord4 == -1) ? "" : $"img_{dWord4:D4}.png"),
				BackImageRotateX = dWord5,
				BackImageRotateY = dWord6,
				HourHandImage = $"img_{dWord7:D4}.png",
				HourImageRotateX = dWord8,
				HourImageRotateY = dWord9,
				MinuteHandImage = $"img_{dWord10:D4}.png",
				MinuteImageRotateX = dWord11,
				MinuteImageRotateY = dWord12,
				SecondHandImage = $"img_{dWord13:D4}.png",
				SecondImageRotateX = dWord14,
				SecondImageRotateY = dWord15
			};
		}
	}
}
