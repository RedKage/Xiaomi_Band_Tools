using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceAction
	{
		public uint ActionId { get; set; }

		public uint AppId { get; set; }

		[XmlIgnore]
		public uint Id { get; set; }

		public string ImageName { get; set; }

		public byte[] RawData { get; set; }
	}
}
