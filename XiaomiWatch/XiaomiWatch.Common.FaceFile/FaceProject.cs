using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceProject
	{
		[XmlAttribute]
		public int DeviceType { get; set; }

		public FaceScreen Screen { get; set; }

		[XmlIgnore]
		public bool IsAOD { get; set; }

		public FaceProject()
		{
			Screen = new FaceScreen();
		}

		public string Serialize()
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add(string.Empty, string.Empty);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FaceProject));
			StringWriter stringWriter = new StringWriter();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter))
			{
				xmlSerializer.Serialize(xmlWriter, this, xmlSerializerNamespaces);
				return stringWriter.ToString();
			}
		}

		public static FaceProject Deserialize(string str)
		{
			new XmlSerializerNamespaces().Add(string.Empty, string.Empty);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(FaceProject));
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(str)))
			{
				return (FaceProject)xmlSerializer.Deserialize(xmlReader);
			}
		}
	}
}
