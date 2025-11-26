using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImageMagick;
using UnpackMiColorFace.FaceFileV3;
using UnpackMiColorFace.Helpers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.Compress;
using XiaomiWatch.Common.Decompress;
using XiaomiWatch.Common.FaceFile;

namespace UnpackMiColorFace.Decompiler
{
	internal class FaceV3Decompiler : IFaceDecompiler
	{
		private FilenameHelper filenameHelper;

		public FaceV3Decompiler(FilenameHelper filenameHelper)
		{
			this.filenameHelper = filenameHelper;
		}

		public void Process(WatchType watchType, byte[] data)
		{
			if (IsSupported(watchType))
			{
				DirectoryInfo directoryInfo = Directory.CreateDirectory(filenameHelper.NameNoExt);
				string text = directoryInfo.FullName + "\\";
				string path = directoryInfo.FullName + "\\images";
				uint num = 89u;
				uint dWord = data.GetDWord(num);
				uint dWord2 = data.GetDWord(num + 4);
				List<FaceWidget> list = new List<FaceWidget>();
				list.AddRange(ExtractImages(watchType, data.GetByteArray(dWord, dWord2), text));
				num -= 8;
				dWord = data.GetDWord(num);
				dWord2 = data.GetDWord(num + 4);
				list.AddRange(ExtractImageList(watchType, data.GetByteArray(dWord, dWord2), path));
				int strlen = data.GetByte(20);
				string uTF8String = data.GetUTF8String(21u, strlen);
				BuildFaceFile(watchType, uTF8String, list, Path.Combine(text, filenameHelper.NameNoExt));
			}
		}

		private FaceProject BuildFaceFile(WatchType watchType, string title, IEnumerable<FaceWidget> faceWidgets, string faceFileName)
		{
			FaceProject faceProject = new FaceProject();
			faceProject.DeviceType = (int)watchType;
			faceProject.Screen.Title = title;
			faceProject.Screen.Bitmap = "preview.png";
			faceProject.Screen.Widgets.AddRange(faceWidgets);
			string text = faceProject.Serialize();
			text = text.Replace("<FaceProject", "\r\n<FaceProject");
			text = text.Replace("<Screen", "\r\n<Screen");
			text = text.Replace("</FaceProject", "\r\n</FaceProject");
			text = text.Replace("</Screen", "\r\n</Screen");
			File.WriteAllText(contents: text.Replace("<Widget", "\r\n<Widget"), path: faceFileName + ".fprj");
			return faceProject;
		}

		private IEnumerable<FaceWidget> ExtractImages(WatchType watchType, byte[] data, string path)
		{
			uint num = 0u;
			uint num2 = 112u;
			uint num3 = 2u;
			List<FaceWidget> list = new List<FaceWidget>();
			string[] array = new string[2] { "preview", "background" };
			IDataDecompressor decompressor = ImageCompressFactory.GetDecompressor(watchType);
			for (int i = 0; i < num3; i++)
			{
				int num4 = data.GetByte(num);
				num++;
				int word = data.GetWord(num);
				int word2 = data.GetWord(num + 2);
				int word3 = data.GetWord(num + 4);
				int word4 = data.GetWord(num + 6);
				int num5 = data.GetByte(num + 9);
				uint dWord = data.GetDWord(num + 10);
				uint dWord2 = data.GetDWord(num + 14);
				Console.WriteLine($"image: {num:X8}, 0x{num4:X2}, compress: {num5}");
				if (i > 0)
				{
					list.Add(new FaceWidgetImage
					{
						Name = array[i],
						Bitmap = array[i] + ".png",
						X = word,
						Y = word2,
						Width = word3,
						Height = word4,
						Alpha = 255
					});
				}
				byte[] byteArray = data.GetByteArray(dWord, dWord2);
				byte[] bytes = BmpHelper.ConvertToBmpGTR(decompressor.Decompress(byteArray, word3, word4), word3, word4, 2);
				string text = path + array[i] + ".bmp";
				string text2 = path + array[i] + ".png";
				string text3 = path + "images\\" + array[i] + ".png";
				if (File.Exists(text2))
				{
					File.Delete(text2);
				}
				File.WriteAllBytes(text, bytes);
				using (MagickImage magickImage = new MagickImage())
				{
					magickImage.Read(text);
					magickImage.ColorType = ColorType.TrueColorAlpha;
					magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
					magickImage.Format = MagickFormat.Png32;
					magickImage.Write(text2);
				}
				File.Delete(text);
				if (!Directory.Exists(Path.GetDirectoryName(text3)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(text3));
				}
				if (File.Exists(text3))
				{
					File.Delete(text3);
				}
				File.Copy(text2, text3);
				if (i > 0)
				{
					File.Delete(text2);
				}
				num += num2 - 1;
			}
			return list;
		}

		private IEnumerable<FaceWidget> ExtractImageList(WatchType watchType, byte[] data, string path)
		{
			uint num = 0u;
			uint num2 = 112u;
			uint num3 = 30u;
			List<FaceWidget> list = new List<FaceWidget>();
			data.GetByteArray(0u, num3);
			num += num3;
			IDataDecompressor decompressor = ImageCompressFactory.GetDecompressor(watchType);
			while (data.GetByte(num) > 0)
			{
				int num4 = data.GetByte(num);
				num++;
				int word = data.GetWord(num);
				int word2 = data.GetWord(num + 2);
				int word3 = data.GetWord(num + 4);
				int word4 = data.GetWord(num + 6);
				int num5 = data.GetByte(num + 8);
				int num6 = data.GetByte(num + 9);
				if (num5 > 0)
				{
					Console.WriteLine($"imageList: {num:X8}/{num5:X2}, 0x{num4:X2}, compress: {num6}");
				}
				switch (num6)
				{
				case 0:
				{
					List<string> list3 = new List<string>();
					for (uint num8 = 0u; num8 < num5; num8++)
					{
						uint dWord3 = data.GetDWord(num + 10 + num8 * 8);
						uint dWord4 = data.GetDWord(num + 14 + num8 * 8);
						byte[] byteArray3 = data.GetByteArray(dWord3, dWord4);
						byte[] bytes2 = BmpHelper.ConvertToBmpGTR(decompressor.Decompress(byteArray3, word3, word4), word3, word4, 2);
						string text3 = $"{path}\\image_{num4:D2}_{num8:D4}.bmp";
						string text4 = $"{path}\\image_{num4:D2}_{num8:D4}.png";
						list3.Add(Path.GetFileName(text4) ?? "");
						if (File.Exists(text4))
						{
							File.Delete(text4);
						}
						File.WriteAllBytes(text3, bytes2);
						using (MagickImage magickImage2 = new MagickImage())
						{
							magickImage2.Read(text3);
							magickImage2.ColorType = ColorType.TrueColorAlpha;
							magickImage2.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
							magickImage2.Format = MagickFormat.Png32;
							magickImage2.Write(text4);
						}
						File.Delete(text3);
					}
					if (list3.Count > 0)
					{
						IFaceFileV3Decoder decoder2 = FaceFileV3DecoderFactory.GetDecoder(watchType);
						list.Add(decoder2.GetWidget(num4, list3, word, word2, word3, word4));
					}
					break;
				}
				case 2:
				{
					List<string> list4 = new List<string>();
					uint dWord5 = data.GetDWord(num + 10);
					uint dWord6 = data.GetDWord(num + 14);
					for (uint num9 = 0u; num9 < num5; num9++)
					{
						byte[] byteArray4 = data.GetByteArray(dWord5 + num9 * dWord6, dWord6);
						int offset3 = (byteArray4.GetWord(0, 0) << 2) + 12 - 2;
						int len = byteArray4.GetWord(offset3) * 3;
						byte[] byteArray5 = data.GetByteArray(byteArray4.GetDWord(2), len);
						int offset4 = byteArray4.Length;
						int dWordAligned2 = byteArray5.Length.GetDWordAligned();
						byte[] data3 = byteArray4.AppendZero(dWordAligned2);
						data3.SetByteArray(offset4, byteArray5);
						byte[] bytes3 = BmpHelper.ConvertToBmpGTR(decompressor.Decompress(data3, word3, word4, (dWord6 << 8) | (byte)num6).Rgb565AlphaToRGBA(), word3, word4, 4);
						string text5 = $"{path}\\image_{num4:D2}_{num9:D4}.bmp";
						string text6 = $"{path}\\image_{num4:D2}_{num9:D4}.png";
						list4.Add(Path.GetFileName(text6) ?? "");
						if (File.Exists(text6))
						{
							File.Delete(text6);
						}
						File.WriteAllBytes(text5, bytes3);
						using (MagickImage magickImage3 = new MagickImage())
						{
							magickImage3.Read(text5);
							magickImage3.ColorType = ColorType.TrueColorAlpha;
							magickImage3.Transparent(MagickColor.FromRgba(0, 0, 0, byte.MaxValue));
							magickImage3.Format = MagickFormat.Png32;
							magickImage3.Write(text6);
						}
						File.Delete(text5);
					}
					if (list4.Count > 0)
					{
						IFaceFileV3Decoder decoder3 = FaceFileV3DecoderFactory.GetDecoder(watchType);
						list.Add(decoder3.GetWidget(num4, list4, word, word2, word3, word4));
					}
					break;
				}
				case 3:
				{
					List<string> list2 = new List<string>();
					uint dWord = data.GetDWord(num + 10);
					uint dWord2 = data.GetDWord(num + 14);
					uint color = data.GetDWord(num + 106, 1u);
					for (uint num7 = 0u; num7 < num5; num7++)
					{
						byte[] byteArray = data.GetByteArray(dWord + num7 * dWord2, dWord2);
						int offset = (byteArray.GetWord(0, 0) << 2) + 12 - 2;
						int word5 = byteArray.GetWord(offset);
						byte[] byteArray2 = data.GetByteArray(byteArray.GetDWord(2), word5);
						int offset2 = byteArray.Length;
						int dWordAligned = byteArray2.Length.GetDWordAligned();
						byte[] data2 = byteArray.AppendZero(dWordAligned);
						data2.SetByteArray(offset2, byteArray2);
						byte[] bytes = BmpHelper.ConvertToBmpGTR((from i in decompressor.Decompress(data2, word3, word4, (dWord2 << 8) | (byte)num6)
							select (uint)((color >> 8) | (i << 24))).SelectMany(BitConverter.GetBytes).ToArray(), word3, word4, 4);
						string text = $"{path}\\image_{num4:D2}_{num7:D4}.bmp";
						string text2 = $"{path}\\image_{num4:D2}_{num7:D4}.png";
						list2.Add(Path.GetFileName(text2) ?? "");
						if (File.Exists(text2))
						{
							File.Delete(text2);
						}
						File.WriteAllBytes(text, bytes);
						using (MagickImage magickImage = new MagickImage())
						{
							magickImage.Read(text);
							magickImage.ColorType = ColorType.TrueColorAlpha;
							magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
							magickImage.Format = MagickFormat.Png32;
							magickImage.Write(text2);
						}
						File.Delete(text);
					}
					if (list2.Count > 0)
					{
						IFaceFileV3Decoder decoder = FaceFileV3DecoderFactory.GetDecoder(watchType);
						list.Add(decoder.GetWidget(num4, list2, word, word2, word3, word4, color >> 8));
					}
					break;
				}
				}
				num += num2 - 1;
			}
			return list;
		}

		private bool IsSupported(WatchType watchType)
		{
			if (watchType == WatchType.RedmiWatch3Active)
			{
				return true;
			}
			return false;
		}
	}
}
