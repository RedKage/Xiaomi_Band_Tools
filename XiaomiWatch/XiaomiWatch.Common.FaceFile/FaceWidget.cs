using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	[XmlInclude(typeof(FaceWidgetImage))]
	[XmlInclude(typeof(FaceWidgetDigitalNum))]
	[XmlInclude(typeof(FaceWidgetImageList))]
	[XmlInclude(typeof(FaceWidgetCircleProgress))]
	[XmlInclude(typeof(FaceWidgetCircleProgressPlus))]
	[XmlInclude(typeof(FaceWidgetAnalogClock))]
	[XmlInclude(typeof(FaceWidgetCircularGauge))]
	[XmlInclude(typeof(FaceWidgetContainer))]
	[XmlInclude(typeof(FaceWidgetConcatenate))]
	[XmlInclude(typeof(FaceWidgetImageCacheList))]
	[XmlInclude(typeof(FaceWidgetTextPlus))]
	public class FaceWidget
	{
		[XmlAttribute]
		public int Shape { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public int X { get; set; }

		[XmlAttribute]
		public int Y { get; set; }

		[XmlAttribute]
		public int Width { get; set; }

		[XmlAttribute]
		public int Height { get; set; }

		[XmlAttribute]
		public int Alpha { get; set; }

		[XmlAttribute("Visible_Src")]
		public int DataSrcDisplay { get; set; }

		[XmlIgnore]
		public uint TargetId { get; set; }

		[XmlIgnore]
		public uint ActionId { get; set; }

		[XmlIgnore]
		public uint AnimationId { get; set; }

		[XmlIgnore]
		public uint ImageId { get; set; }

		[XmlIgnore]
		public uint Id { get; set; }

		[XmlIgnore]
		public byte Digits { get; set; }

		[XmlIgnore]
		public byte Align { get; set; }

		[XmlIgnore]
		public int TypeId { get; set; }

		[XmlIgnore]
		public virtual uint WidgetSize { get; set; } = 16u;

		public bool IsRGBA => Name.ToLower().EndsWith("_rgba");

		public bool IsAnimation => Name.ToLower().StartsWith("anim_");

		public bool IsButton => Name.ToLower().StartsWith("btn_");

		public bool IsRef => Name.ToLower().Contains("_ref[");

		public bool HasSmoothSecondAnimation => Name.ToLower().Contains("_smooth");

		public string RW3AIndex
		{
			get
			{
				Match match = Regex.Match(Name, "^([^_]*)");
				string result = "";
				if (match.Success)
				{
					result = match.Groups[1].Value;
				}
				return result;
			}
		}

		public string RefName
		{
			get
			{
				Match match = Regex.Match(Name.ToLower(), "_ref\\[(.+)\\]");
				string result = "";
				if (match.Success)
				{
					result = match.Groups[1].Value;
				}
				return result;
			}
		}

		public bool HasColor => Name.ToLower().Contains("_color[");

		public bool HasAngle => Name.ToLower().Contains("_angle[");

		public bool IsApp => Name.ToLower().StartsWith("app_");

		public bool IsRGB => Name.ToLower().Contains("_rgb");

		public byte[] RawData { get; set; }

		public int GetSmoothValue()
		{
			Match match = Regex.Match(Name.ToLower(), "_smooth\\[(\\d+)\\]");
			int num = 30;
			if (match.Success && int.TryParse(match.Groups[1].Value, out var result))
			{
				num = result;
			}
			if (num < 30)
			{
				num = 30;
			}
			if (num > 1000)
			{
				num = 1000;
			}
			return num;
		}

		public uint GetColor()
		{
			Match match = Regex.Match(Name.ToLower(), "_color\\[(.+)\\]");
			string s = "";
			if (match.Success)
			{
				s = match.Groups[1].Value;
			}
			return uint.Parse(s, NumberStyles.HexNumber);
		}

		public int GetAngle()
		{
			Match match = Regex.Match(Name.ToLower(), "_angle\\[(\\d+)\\]");
			int num = 0;
			if (match.Success && int.TryParse(match.Groups[1].Value, out var result))
			{
				num = result;
			}
			if (num <= 0)
			{
				num = 0;
			}
			if (num >= 3600)
			{
				num = 0;
			}
			return num;
		}

		public static FaceWidget Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int alpha = bin.GetByte(28);
			return new FaceWidget
			{
				Shape = dWord2,
				Name = $"unknown_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha,
				RawData = bin
			};
		}
	}
}
