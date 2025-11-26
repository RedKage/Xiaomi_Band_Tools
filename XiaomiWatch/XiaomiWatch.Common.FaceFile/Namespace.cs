namespace XiaomiWatch.Common.FaceFile
{
	public class Namespace
	{
		private const string ns = "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" ";

		public static string Prepare(string src)
		{
			if (src.Length == 0)
			{
				return src;
			}
			if (src.Contains("xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" "))
			{
				return src;
			}
			int startIndex = 0;
			while ((startIndex = src.IndexOf("Shape", startIndex)) > 0)
			{
				string nsByTypeId = GetNsByTypeId(int.Parse(src.Substring(startIndex).Substring("Shape=\"".Length).Split('"')[0]));
				src = src.Insert(startIndex, nsByTypeId);
				startIndex += nsByTypeId.Length + "Shape".Length;
			}
			return src;
		}

		private static string GetNsByTypeId(int typeId)
		{
			switch (typeId)
			{
			case 27:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetAnalogClock\" ";
			case 29:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetCircleProgressPlus\" ";
			case 30:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetImage\" ";
			case 31:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetImageList\" ";
			case 32:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetDigitalNum\" ";
			case 34:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetContainer\" ";
			case 42:
				return "xmlns:p3=\"http://www.w3.org/2001/XMLSchema-instance\" p3:type=\"FaceWidgetCircleProgressPlus\" ";
			default:
				return "";
			}
		}
	}
}
