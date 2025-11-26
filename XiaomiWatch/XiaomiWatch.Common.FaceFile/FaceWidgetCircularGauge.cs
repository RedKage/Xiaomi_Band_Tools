using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetCircularGauge : FaceWidget
	{
		[XmlAttribute("Range_Max_Src")]
		public int DataSrcMax { get; set; }

		[XmlAttribute("Range_Val_Src")]
		public string DataSrcVal { get; set; }

		[XmlAttribute("Origo_x")]
		public int GaugeX { get; set; }

		[XmlAttribute("Origo_y")]
		public int GaugeY { get; set; }

		[XmlAttribute("StartAngle")]
		public int StartAngle { get; set; }

		[XmlAttribute("EndAngle")]
		public int EndAngle { get; set; }

		[XmlAttribute("Range_Min")]
		public int RangeMin { get; set; }

		[XmlAttribute("Range_Max")]
		public int RangeMax { get; set; }

		[XmlAttribute("Range_MinStep")]
		public int RangeMinStep { get; set; }

		[XmlAttribute("Range_Step")]
		public int RangeStep { get; set; }

		[XmlAttribute("Pointer_Bitmap")]
		public string PointerImage { get; set; }

		[XmlAttribute("Pointer_Rotate_xc")]
		public int PointerRotateX { get; set; }

		[XmlAttribute("Pointer_Rotate_yc")]
		public int PointerRotateY { get; set; }

		[XmlAttribute("Render_Height_Quality_En")]
		public int RenderHeightQualityEnabled { get; set; }

		public FaceWidgetCircularGauge()
		{
			base.Shape = 43;
		}

		public new static FaceWidgetCircularGauge Get(byte[] bin)
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
			int word5 = byteArray.GetWord(16);
			int word6 = byteArray.GetWord(18);
			int word7 = byteArray.GetWord(20);
			int word8 = byteArray.GetWord(22);
			int word9 = byteArray.GetWord(24);
			int word10 = byteArray.GetWord(28);
			int word11 = byteArray.GetWord(32);
			int renderHeightQualityEnabled = byteArray.GetByte(36);
			int dataSrcMax = byteArray.GetByte(37);
			int num = byteArray.GetByte(38);
			return new FaceWidgetCircularGauge
			{
				Shape = dWord2,
				Name = $"circularGauge_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				GaugeX = dWord4,
				GaugeY = dWord5,
				StartAngle = dWord6,
				EndAngle = dWord7,
				RangeMin = word5,
				RangeMax = word6,
				RangeMinStep = word7,
				RangeStep = word8,
				PointerImage = ((word9 == -1) ? "" : $"img_{word9:D4}.png"),
				PointerRotateX = word10,
				PointerRotateY = word11,
				RenderHeightQualityEnabled = renderHeightQualityEnabled,
				DataSrcMax = dataSrcMax,
				DataSrcVal = num.ToString()
			};
		}
	}
}
