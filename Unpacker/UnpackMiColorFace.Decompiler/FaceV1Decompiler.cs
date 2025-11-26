using System;
using System.IO;
using ImageMagick;
using UnpackMiColorFace.Helpers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.FaceFile;

namespace UnpackMiColorFace.Decompiler
{
	internal class FaceV1Decompiler : IFaceDecompiler
	{
		private FilenameHelper filenameHelper;

		public FaceV1Decompiler(FilenameHelper filename)
		{
			filenameHelper = filename;
		}

		public void Process(WatchType watchType, byte[] data)
		{
			string text = Directory.CreateDirectory(filenameHelper.NameNoExt).FullName + "\\";
			string path = Directory.CreateDirectory(filenameHelper.NameNoExt + "\\images").FullName + "\\";
			uint num = 330u;
			uint dWord = data.GetDWord(num);
			num += 4;
			int num2 = 0;
			for (int i = 0; i < dWord; i++)
			{
				switch (data.GetDWord(num))
				{
				case 3u:
					num2 |= 1;
					ExtractImages(data, num, path);
					break;
				case 0u:
					num2 |= 2;
					BuildFaceFile(data, num, text + filenameHelper.NameNoExt);
					break;
				}
				num += 16;
			}
			if (num2 != 3)
			{
				throw new MissingMemberException();
			}
		}

		private void ExtractImages(byte[] data, uint offset, string path)
		{
			offset = data.GetDWord(offset + 12);
			uint num = 0u;
			num = data.GetDWord(offset);
			uint num2 = offset;
			offset += 4;
			for (int i = 0; i < num; i++)
			{
				try
				{
					uint dWord = data.GetDWord(offset);
					uint dWord2 = data.GetDWord(offset + 8);
					uint dWord3 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(num2 + dWord3, dWord2);
					uint word = byteArray.GetWord(9);
					uint word2 = byteArray.GetWord(11);
					int num3 = (int)(byteArray.GetWord(5) / word);
					byte[] clut = null;
					byte[] byteArray2 = byteArray.GetByteArray(21u, (uint)(byteArray.Length - 21));
					if (num3 == 1)
					{
						uint num4 = 21 + word * word2;
						clut = byteArray.GetByteArray(num4 + 4, (uint)(byteArray.Length - (int)num4 - 4));
						byteArray2 = byteArray.GetByteArray(21u, num4 - 21);
					}
					byte[] bytes = BmpHelper.ConvertToBmpGTR(byteArray2, (int)word, (int)word2, num3, clut);
					string text = path + $"img_{dWord:D4}.bmp";
					string fileName = path + $"img_{dWord:D4}.png";
					File.WriteAllBytes(text, bytes);
					using (MagickImage magickImage = new MagickImage())
					{
						magickImage.Read(text);
						magickImage.ColorType = ColorType.TrueColorAlpha;
						magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
						magickImage.Format = MagickFormat.Png32;
						magickImage.Write(fileName);
					}
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Console.WriteLine("image processing err: " + ex);
				}
				offset += 16;
			}
		}

		private void BuildFaceFile(byte[] data, uint offset, string facefile)
		{
			string unicodeString = data.GetUnicodeString(26u);
			uint dWord = data.GetDWord(70);
			offset = data.GetDWord(offset + 12);
			uint num = 0u;
			num = data.GetDWord(offset);
			uint num2 = offset;
			offset += 4;
			FaceProject faceProject = new FaceProject();
			faceProject.DeviceType = 1;
			faceProject.Screen.Title = unicodeString;
			faceProject.Screen.Bitmap = $"img_{dWord:D4}.png";
			for (int i = 0; i < num; i++)
			{
				data.GetDWord(offset);
				uint dWord2 = data.GetDWord(offset + 8);
				uint dWord3 = data.GetDWord(offset + 12);
				byte[] byteArray = data.GetByteArray(num2 + dWord3, dWord2);
				byteArray.GetDWord(0, 0u);
				uint dWord4 = byteArray.GetDWord(4);
				byteArray.GetWord(8);
				byteArray.GetWord(12);
				byteArray.GetWord(16);
				byteArray.GetWord(20);
				byteArray.GetByte(28);
				faceProject.Screen.Widgets.Add(WidgetFactory.Get(dWord4, byteArray));
				offset += 16;
			}
			string text = faceProject.Serialize();
			text = text.Replace("<FaceProject", "\r\n<FaceProject");
			text = text.Replace("<Screen", "\r\n<Screen");
			text = text.Replace("</FaceProject", "\r\n</FaceProject");
			text = text.Replace("</Screen", "\r\n</Screen");
			text = text.Replace("<Widget", "\r\n<Widget");
			File.WriteAllText(facefile + ".fprj", text);
		}
	}
}
