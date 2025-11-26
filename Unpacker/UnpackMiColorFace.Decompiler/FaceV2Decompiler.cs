using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ImageMagick;
using UnpackMiColorFace.Helpers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.Compress;
using XiaomiWatch.Common.FaceFile;

namespace UnpackMiColorFace.Decompiler
{
	internal class FaceV2Decompiler : IFaceDecompiler
	{
		private FilenameHelper filenameHelper;

		private const uint OffsetTitle = 104u;

		public FaceV2Decompiler(FilenameHelper filenameHelper)
		{
			this.filenameHelper = filenameHelper;
		}

		public void Process(WatchType watchType, byte[] data)
		{
			DirectoryInfo directoryInfo = Directory.CreateDirectory(filenameHelper.NameNoExt);
			string text = directoryInfo.FullName + "\\";
			if (File.Exists(text + "source.bin"))
			{
				File.Delete(text + "source.bin");
			}
			uint offset = 32u;
			uint num = 168u;
			int num2 = data.GetByte(24);
			int num3 = data.GetByte(28);
			int num4 = data.GetByte(29);
			int word = data.GetWord(30);
			ProcessPreview(watchType, data, offset, text);
			num += (uint)(num4 * 4);
			num += (uint)(num2 * 4);
			for (int i = 0; i < num3; i++)
			{
				string faceSlotImagesFolder = filenameHelper.GetFaceSlotImagesFolder(watchType, i, word);
				directoryInfo = ((!Directory.Exists(faceSlotImagesFolder)) ? Directory.CreateDirectory(faceSlotImagesFolder) : new DirectoryInfo(faceSlotImagesFolder));
				string path = directoryInfo.FullName + "\\";
				List<FaceElement> list = null;
				List<FaceWidget> lstw = null;
				List<FaceImage> lsti = null;
				List<FaceImageList> lstil = null;
				List<FaceAction> lsta = null;
				List<FaceAppItem> lstApp = null;
				uint dWord = data.GetDWord(num);
				data.GetDWord(num + 4);
				num += 8;
				for (int j = 0; j < 10; j++)
				{
					if (j == 0)
					{
						list = ProcessElements(data, num, text);
					}
					if (j == 2)
					{
						lsti = ProcessImageSingle(watchType, data, num, path);
					}
					if (j == 3)
					{
						lstil = ProcessImageList(watchType, data, num, path);
					}
					if (j == 5)
					{
						lstApp = ProcessAppData(watchType, data, num);
					}
					if (j == 7)
					{
						lstw = ProcessWidgets(data, num, text);
					}
					if (j == 9)
					{
						lsta = ProcessAction(data, num, text);
					}
					num += 8;
				}
				if (watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand9)
				{
					num += 68;
					uint dWord2 = data.GetDWord(num);
					num += 4;
					int num5 = (int)(dWord2 & 0xFF) >> 2;
					if (num5 > 1)
					{
						num += (uint)(num5 * 4);
					}
				}
				if (watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3 || watchType == WatchType.RedmiWatch4)
				{
					list.Insert(0, new FaceElement(dWord));
				}
				string faceSlotFilename = filenameHelper.GetFaceSlotFilename(watchType, i, word);
				string faceTitle = GetFaceTitle(data, watchType);
				FaceProject face = BuildFaceFile(faceTitle, watchType, list, lsti, lstil, lstw, lstApp, lsta, text + faceSlotFilename);
				BuildPreview(face, watchType, faceSlotImagesFolder, text + faceSlotFilename);
			}
		}

		private string GetFaceTitle(byte[] data, WatchType watchType)
		{
			if (data.GetDWord(104u) == uint.MaxValue)
			{
				byte num = data.GetByte(111u);
				uint dWord = data.GetDWord(116u);
				data.GetDWord(120u);
				if (num == 6 && data.GetDWord(dWord) == 7)
				{
					int num2 = 3;
					uint num3 = (uint)(dWord + 8 + num2 * 4);
					string[] array = new string[num2];
					for (int i = 0; i < num2; i++)
					{
						uint dWord2 = data.GetDWord((uint)(dWord + 8 + i * 4));
						array[i] = data.GetUTF8String(num3, (int)dWord2).Trim();
						num3 += dWord2;
					}
					return string.Join("|", array);
				}
				return string.Empty;
			}
			return data.GetUTF8String(104u);
		}

		private void ProcessPreview(WatchType watchType, byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(dWord + 8);
			byte[] byteArray = data.GetByteArray(dWord, dWord2 + 12);
			uint word = byteArray.GetWord(4);
			uint word2 = byteArray.GetWord(6);
			int num = byteArray[0];
			int num2 = byteArray[1];
			if (byteArray.GetDWord(0, 0u) == 0)
			{
				num2 = 4;
			}
			else if (watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiBandPro || watchType == WatchType.MiBand7Pro)
			{
				num2 = byteArray[0] & 0xF;
				if (num2 == 4)
				{
					num2 = 2;
				}
			}
			uint num3 = 0u;
			byte[] clut = null;
			byte[] array = byteArray.GetByteArray(12u, (uint)(byteArray.Length - 12));
			if (watchType == WatchType.MiWatchS3 || (watchType == WatchType.MiBand9 && num == 16))
			{
				num2 = 1;
				clut = array.Take(1024).ToArray();
				array = array.Skip(1024).ToArray();
			}
			num3 = byteArray.GetDWord(12);
			if (num3 != 1520771552)
			{
				if (num2 == 1 && watchType != WatchType.MiWatchS3 && watchType != WatchType.MiBand9)
				{
					uint num4 = 21 + word * word2;
					clut = byteArray.GetByteArray(num4 + 4, (uint)(byteArray.Length - (int)num4 - 4));
					array = byteArray.GetByteArray(12u, num4 - 12);
				}
			}
			else
			{
				byte[] byteArray2 = byteArray.GetByteArray(20u, dWord2 - 8);
				num2 = byteArray[16] & 0xF;
				byte[] array2 = ImageCompressFactory.GetDecompressor(watchType).Decompress(byteArray2, (int)word, (int)word2, byteArray.GetDWord(16));
				switch (num)
				{
				case 16:
					array = array2.ConvertToRGBA();
					num2 = 4;
					break;
				case 6:
					array = array2.Rgb565AlphaToRGBA();
					num2 = 4;
					break;
				}
			}
			byte[] array3 = null;
			array3 = ((num3 != 1520771552) ? BmpHelper.ConvertToBmpGTR(array, (int)word, (int)word2, num2, clut) : BmpHelper.ConvertToBmpGTRv2(array, (int)word, (int)word2, num2));
			string text = path + "preview.bmp";
			string text2 = path + "preview.png";
			string text3 = path + "images\\preview.png";
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.WriteAllBytes(text, array3);
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
		}

		private List<FaceAppItem> ProcessAppData(WatchType watchType, byte[] data, uint offset)
		{
			string text = filenameHelper.NameNoExt + "\\app\\";
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(offset + 4);
			List<FaceAppItem> list = new List<FaceAppItem>();
			if (dWord == 0)
			{
				return list;
			}
			offset = dWord2;
			for (int i = 0; i < dWord; i++)
			{
				data.GetWord(offset);
				uint dWord3 = data.GetDWord(offset);
				uint dWord4 = data.GetDWord(offset + 8);
				uint dWord5 = data.GetDWord(offset + 12);
				uint num = data.GetDWord(dWord4) & 0xFFFFFF;
				uint num2 = data[dWord4 + 3];
				dWord5 = 20 + num2 + num;
				byte[] byteArray = data.GetByteArray(dWord4, dWord5);
				string text2 = Encoding.ASCII.GetString(byteArray.GetByteArray(20u, num2));
				string path = text + text2;
				string directoryName = Path.GetDirectoryName(path);
				if (Directory.Exists(directoryName))
				{
					new DirectoryInfo(directoryName);
				}
				else
				{
					Directory.CreateDirectory(directoryName);
				}
				File.WriteAllBytes(path, byteArray.GetByteArray(20 + num2, num));
				list.Add(new FaceAppItem
				{
					Id = dWord3,
					Name = text2
				});
				offset += 16;
			}
			return list;
		}

		private void BuildPreview(FaceProject face, WatchType watchType, string imagesFolder, string facefile)
		{
			int num = 0;
			int screenWidth = WatchScreen.GetScreenWidth(watchType);
			int screenHeight = WatchScreen.GetScreenHeight(watchType);
			using (MagickImage magickImage = new MagickImage(MagickColor.FromRgb(0, 0, 0), screenWidth, screenHeight))
			{
				foreach (FaceWidget widget in face.Screen.Widgets)
				{
					switch (widget.Shape)
					{
					case 30:
					{
						FaceWidgetImage faceWidgetImage = widget as FaceWidgetImage;
						string fileName2 = imagesFolder + "\\" + faceWidgetImage.Bitmap;
						using (MagickImage magickImage3 = new MagickImage())
						{
							magickImage3.Read(fileName2);
							magickImage.Composite(magickImage3, widget.X, widget.Y, CompositeOperator.Over);
						}
						break;
					}
					case 43:
					{
						FaceWidgetCircularGauge faceWidgetCircularGauge = widget as FaceWidgetCircularGauge;
						if (!string.IsNullOrEmpty(faceWidgetCircularGauge.PointerImage))
						{
							string pathImage4 = imagesFolder + "\\" + faceWidgetCircularGauge.PointerImage;
							ApplyCircleImage(magickImage, faceWidgetCircularGauge, pathImage4, 150);
						}
						break;
					}
					case 31:
					{
						FaceWidgetImageList faceWidgetImageList = widget as FaceWidgetImageList;
						string[] array2 = faceWidgetImageList.BitmapList.Split('|');
						int num4 = int.Parse(GetStringSource(faceWidgetImageList.DataSrcIndex));
						if (num4 > array2.Length)
						{
							num4 = 1;
						}
						string text = array2[num4].Split(':')[1];
						string fileName3 = imagesFolder + "\\" + text;
						using (MagickImage magickImage4 = new MagickImage())
						{
							magickImage4.Read(fileName3);
							magickImage.Composite(magickImage4, widget.X, widget.Y, CompositeOperator.Over);
						}
						break;
					}
					case 32:
					{
						FaceWidgetDigitalNum faceWidgetDigitalNum = widget as FaceWidgetDigitalNum;
						using (MagickImage magickImage2 = new MagickImage())
						{
							int width = faceWidgetDigitalNum.Width;
							int spacing = faceWidgetDigitalNum.Spacing;
							string stringSource = GetStringSource(faceWidgetDigitalNum.DataSrcValue);
							int num2 = widget.X;
							int y = widget.Y;
							int num3 = ((faceWidgetDigitalNum.Digits > stringSource.Length) ? stringSource.Length : faceWidgetDigitalNum.Digits);
							if (faceWidgetDigitalNum.Alignment == 0 && (watchType == WatchType.Gen2 || watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand8 || watchType == WatchType.MiBand9 || watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiWatch3 || watchType == WatchType.RedmiWatch4 || watchType == WatchType.MiBand8Pro || watchType == WatchType.MiBand7Pro))
							{
								num2 -= (width + spacing) * num3 / 2;
							}
							string[] array = faceWidgetDigitalNum.BitmapList.Split('|');
							for (int i = 0; i < num3; i++)
							{
								string fileName = imagesFolder + "\\" + array[int.Parse(stringSource[i].ToString())];
								magickImage2.Read(fileName);
								magickImage.Composite(magickImage2, num2, y, CompositeOperator.Over);
								num2 += width + spacing;
							}
						}
						break;
					}
					case 27:
					{
						FaceWidgetAnalogClock faceWidgetAnalogClock = widget as FaceWidgetAnalogClock;
						if (!string.IsNullOrEmpty(faceWidgetAnalogClock.HourHandImage))
						{
							string pathImage = imagesFolder + "\\" + faceWidgetAnalogClock.HourHandImage;
							ApplyClockImage(magickImage, faceWidgetAnalogClock, faceWidgetAnalogClock.HourImageRotateX, faceWidgetAnalogClock.HourImageRotateY, pathImage, -60);
						}
						if (!string.IsNullOrEmpty(faceWidgetAnalogClock.MinuteHandImage))
						{
							string pathImage2 = imagesFolder + "\\" + faceWidgetAnalogClock.MinuteHandImage;
							ApplyClockImage(magickImage, faceWidgetAnalogClock, faceWidgetAnalogClock.MinuteImageRotateX, faceWidgetAnalogClock.MinuteImageRotateY, pathImage2, 50);
						}
						if (!string.IsNullOrEmpty(faceWidgetAnalogClock.SecondHandImage))
						{
							string pathImage3 = imagesFolder + "\\" + faceWidgetAnalogClock.SecondHandImage;
							ApplyClockImage(magickImage, faceWidgetAnalogClock, faceWidgetAnalogClock.SecondImageRotateX, faceWidgetAnalogClock.SecondImageRotateY, pathImage3, 120);
						}
						break;
					}
					}
					num++;
				}
				if (watchType == WatchType.Gen1 || watchType == WatchType.Gen2 || watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3)
				{
					magickImage.Alpha(AlphaOption.Set);
					using (IMagickImage magickImage5 = magickImage.Clone())
					{
						magickImage5.Distort(DistortMethod.DePolar, default(double));
						magickImage5.VirtualPixelMethod = VirtualPixelMethod.HorizontalTile;
						magickImage5.BackgroundColor = MagickColors.None;
						magickImage5.Distort(DistortMethod.Polar, default(double));
						magickImage.Compose = CompositeOperator.DstIn;
						magickImage.Composite(magickImage5, CompositeOperator.CopyAlpha);
					}
				}
				magickImage.Write(facefile + "_preview.png");
			}
		}

		private void ApplyCircleImage(MagickImage preview, FaceWidgetCircularGauge gauge, string pathImage, int angle)
		{
			using (MagickImage magickImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), gauge.Width, gauge.Height))
			{
				int x = (gauge.Width - gauge.X) / 2 - gauge.PointerRotateX;
				int y = (gauge.Height - gauge.Y) / 2 - gauge.PointerRotateY;
				MagickImage image = new MagickImage(pathImage);
				magickImage.Composite(image, x, y, CompositeOperator.Over);
				magickImage.Distort(DistortMethod.ScaleRotateTranslate, gauge.Width / 2, gauge.Height / 2, 1.0, angle);
				preview.Composite(magickImage, 0, 0, CompositeOperator.Over);
			}
		}

		private string GetStringSource(string dataSrcValue)
		{
			switch (dataSrcValue)
			{
			case "1012":
				return "10";
			case "1812":
				return "21";
			case "2012":
				return "3";
			case "0811":
				return "12";
			case "1000911":
				return "1";
			case "0911":
				return "2";
			case "1011":
				return "45";
			case "1211":
				return "4";
			case "1111":
				return "5";
			case "1811":
				return "28";
			case "1001911":
				return "2";
			case "1911":
				return "8";
			case "0821":
				return "15652";
			case "0823":
				return "12175";
			case "0822":
				return "174";
			case "0824":
				return "10";
			case "0826":
				return "42";
			case "0828":
				return "60";
			case "0841":
				return "100";
			case "2031":
				return "23";
			case "3031":
				return "1";
			case "5031":
				return "2";
			default:
				return "120000";
			}
		}

		private void ApplyClockImage(MagickImage preview, FaceWidgetAnalogClock clock, int rotateX, int rotateY, string pathImage, int angle)
		{
			using (MagickImage magickImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), clock.Width, clock.Height))
			{
				int x = (clock.Width - clock.X) / 2 - rotateX;
				int y = (clock.Height - clock.Y) / 2 - rotateY;
				MagickImage image = new MagickImage(pathImage);
				magickImage.Composite(image, x, y, CompositeOperator.Over);
				magickImage.Distort(DistortMethod.ScaleRotateTranslate, clock.Width / 2, clock.Height / 2, 1.0, angle);
				preview.Composite(magickImage, 0, 0, CompositeOperator.Over);
			}
		}

		private FaceProject BuildFaceFile(string title, WatchType watchType, List<FaceElement> lste, List<FaceImage> lsti, List<FaceImageList> lstil, List<FaceWidget> lstw, List<FaceAppItem> lstApp, List<FaceAction> lsta, string facefile)
		{
			FaceProject faceProject = new FaceProject();
			faceProject.DeviceType = (int)watchType;
			faceProject.Screen.Title = title;
			faceProject.Screen.Bitmap = "preview.png";
			int num = 0;
			foreach (FaceElement e in lste)
			{
				try
				{
					if (e.TargetId >> 24 == 2)
					{
						FaceImage faceImage = lsti.Find((FaceImage c) => (c.Id & 0xFF00FFFFu) == e.TargetId);
						faceProject.Screen.Widgets.Add(new FaceWidgetImage
						{
							Name = $"image_{num:D2}",
							X = e.PosX,
							Y = e.PosY,
							Width = faceImage.Width,
							Height = faceImage.Height,
							Alpha = 255,
							Bitmap = faceImage.Name
						});
					}
					else if (e.TargetId >> 24 == 7)
					{
						FaceWidget wdgt = lstw.Find((FaceWidget c) => c.Id == e.TargetId);
						FaceImageList faceImageList = lstil.Find((FaceImageList c) => c.Id == wdgt.TargetId);
						FaceImage faceImage2 = lsti.Find((FaceImage c) => c.Id == wdgt.TargetId);
						if (faceImageList == null && faceImage2 == null)
						{
							continue;
						}
						if (wdgt.TypeId == 1)
						{
							if (faceImageList.NameList.Count() == 10)
							{
								List<string> list = faceImageList.NameList.ToList();
								list.Add(faceImageList.NameList[0]);
								faceImageList.NameList = list.ToArray();
							}
							byte digits = wdgt.Digits;
							string stringSource = GetStringSource($"{wdgt.Shape:X2}{wdgt.DataSrcDisplay:X2}");
							int num2 = ((digits > stringSource.Length) ? stringSource.Length : digits);
							int num3 = e.PosX;
							int width = faceImageList.Width;
							int num4 = 0;
							if (wdgt.Align == 2 && (watchType == WatchType.Gen2 || watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3 || watchType == WatchType.RedmiBandPro || watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiWatch3 || watchType == WatchType.RedmiWatch4 || watchType == WatchType.MiBand8 || watchType == WatchType.MiBand9 || watchType == WatchType.MiBand8Pro || watchType == WatchType.MiBand7Pro))
							{
								num3 -= (width + num4) * num2 / 2;
							}
							else if (wdgt.Align == 0 && (watchType == WatchType.Gen2 || watchType == WatchType.Gen3 || watchType == WatchType.MiWatchS3 || watchType == WatchType.RedmiBandPro || watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiWatch3 || watchType == WatchType.RedmiWatch4 || watchType == WatchType.MiBand8 || watchType == WatchType.MiBand9 || watchType == WatchType.MiBand8Pro || watchType == WatchType.MiBand7Pro))
							{
								num3 -= (width + num4) * num2;
							}
							faceProject.Screen.Widgets.Add(new FaceWidgetDigitalNum
							{
								Name = $"widget_{num:D2}",
								X = num3,
								Y = e.PosY,
								Width = faceImageList.Width,
								Height = faceImageList.Height,
								Digits = digits,
								Alignment = GetTextAlignment(wdgt.Align),
								Alpha = 255,
								DataSrcValue = $"{wdgt.Shape:X2}{wdgt.DataSrcDisplay:X2}",
								BitmapList = string.Join("|", faceImageList.NameList)
							});
						}
						else if (wdgt.TypeId == 2)
						{
							bool flag = wdgt.RawData.GetWord(12) == 512;
							string text = "";
							for (int num5 = 0; num5 < faceImageList.NameList.Length; num5++)
							{
								int num6 = 0;
								try
								{
									if (flag)
									{
										num6 = (int)wdgt.RawData.GetDWord(16 + num5 * 4);
									}
								}
								catch
								{
								}
								if (text.Length > 0)
								{
									text += "|";
								}
								text += $"({(flag ? num6 : num5)}):{faceImageList.NameList[num5]}";
							}
							faceProject.Screen.Widgets.Add(new FaceWidgetImageList
							{
								Name = $"widget_{num:D2}",
								X = e.PosX,
								Y = e.PosY,
								Width = faceImageList.Width,
								Height = faceImageList.Height,
								Alpha = 255,
								DataSrcIndex = $"{wdgt.Shape:X2}{wdgt.DataSrcDisplay:X2}",
								BitmapList = text
							});
						}
						else if (wdgt.TypeId == 3)
						{
							if (wdgt.DataSrcDisplay == 17)
							{
								if (!faceProject.Screen.Widgets.Any((FaceWidget w) => w.Shape == 27))
								{
									faceProject.Screen.Widgets.Add(new FaceWidgetAnalogClock
									{
										Name = $"analogClock_{num:D2}",
										Width = WatchScreen.GetScreenWidth(watchType),
										Height = WatchScreen.GetScreenHeight(watchType),
										Alpha = 255
									});
								}
								FaceWidgetAnalogClock faceWidgetAnalogClock = faceProject.Screen.Widgets.First((FaceWidget w) => w.Shape == 27) as FaceWidgetAnalogClock;
								if (wdgt.Shape == 8)
								{
									faceWidgetAnalogClock.HourHandImage = faceImage2.Name;
									faceWidgetAnalogClock.HourImageRotateX = wdgt.X;
									faceWidgetAnalogClock.HourImageRotateY = wdgt.Y;
								}
								else if (wdgt.Shape == 16)
								{
									faceWidgetAnalogClock.MinuteHandImage = faceImage2.Name;
									faceWidgetAnalogClock.MinuteImageRotateX = wdgt.X;
									faceWidgetAnalogClock.MinuteImageRotateY = wdgt.Y;
								}
								else if (wdgt.Shape == 24)
								{
									faceWidgetAnalogClock.SecondHandImage = faceImage2.Name;
									faceWidgetAnalogClock.SecondImageRotateX = wdgt.X;
									faceWidgetAnalogClock.SecondImageRotateY = wdgt.Y;
								}
							}
							else
							{
								faceProject.Screen.Widgets.Add(new FaceWidgetCircularGauge
								{
									Name = $"widget_{num:D2}",
									X = e.PosX,
									Y = e.PosY,
									Width = faceImage2.Width,
									Height = faceImage2.Height,
									Alpha = 255,
									DataSrcVal = $"{wdgt.Shape:X2}{wdgt.DataSrcDisplay:X2}",
									PointerImage = faceImage2.Name
								});
							}
						}
					}
					else if (e.TargetId >> 24 == 9)
					{
						FaceAction action = lsta.Find((FaceAction c) => c.Id == e.TargetId);
						if (action != null)
						{
							string arg = "";
							if (action.ActionId == 594678784)
							{
								FaceAppItem faceAppItem = lstApp.Find((FaceAppItem c) => c.Id == action.AppId);
								if (faceAppItem != null)
								{
									arg = "_[" + faceAppItem.Name + "]";
								}
							}
							faceProject.Screen.Widgets.Add(new FaceWidgetImage
							{
								Name = $"btn_{num:D2}{arg}",
								X = e.PosX,
								Y = e.PosY,
								Alpha = 255,
								Bitmap = action.ImageName
							});
						}
					}
					else if (e.TargetId >> 24 == 5)
					{
						FaceAppItem faceAppItem2 = lstApp.Find((FaceAppItem c) => c.Id == e.TargetId);
						faceProject.Screen.Widgets.Add(new FaceWidgetContainer
						{
							Name = "app_" + Uri.EscapeDataString(faceAppItem2.Name),
							X = 0,
							Y = 0,
							Width = WatchScreen.GetScreenWidth(watchType),
							Height = WatchScreen.GetScreenHeight(watchType)
						});
					}
				}
				catch (Exception arg2)
				{
					LogHelper.GotError = true;
					Console.WriteLine($"Failed to build watchface item: {arg2}");
				}
				num++;
			}
			string text2 = faceProject.Serialize();
			text2 = text2.Replace("<FaceProject", "\r\n<FaceProject");
			text2 = text2.Replace("<Screen", "\r\n<Screen");
			text2 = text2.Replace("</FaceProject", "\r\n</FaceProject");
			text2 = text2.Replace("</Screen", "\r\n</Screen");
			text2 = text2.Replace("<Widget", "\r\n<Widget");
			File.WriteAllText(facefile + ".fprj", text2);
			return faceProject;
		}

		private static int GetTextAlignment(byte align)
		{
			switch (align)
			{
			case 2:
				return 1;
			case 1:
				return 2;
			default:
				return 0;
			}
		}

		private List<FaceAction> ProcessAction(byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(offset + 4);
			List<FaceAction> list = new List<FaceAction>();
			offset = dWord2;
			for (int i = 0; i < dWord; i++)
			{
				try
				{
					data.GetWord(offset);
					uint dWord3 = data.GetDWord(offset);
					uint dWord4 = data.GetDWord(offset + 8);
					uint dWord5 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(dWord4, dWord5);
					uint num = byteArray.GetDWord(32) & 0xFF;
					if (num == 0)
					{
						continue;
					}
					list.Add(new FaceAction
					{
						RawData = byteArray,
						Id = dWord3,
						ImageName = $"img_{num:D4}.png",
						ActionId = byteArray.GetDWord(40),
						AppId = byteArray.GetDWord(44)
					});
					goto IL_00e6;
				}
				catch (Exception ex)
				{
					Console.WriteLine("action processing err: " + ex);
					goto IL_00e6;
				}
				IL_00e6:
				offset += 16;
			}
			return list;
		}

		private List<FaceElement> ProcessElements(byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(offset + 4);
			List<FaceElement> list = new List<FaceElement>();
			offset = dWord2;
			for (int i = 0; i < dWord; i++)
			{
				try
				{
					data.GetWord(offset);
					uint dWord3 = data.GetDWord(offset + 8);
					uint dWord4 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(dWord3, dWord4);
					list.Add(new FaceElement
					{
						TargetId = byteArray.GetDWord(0, 0u),
						PosX = byteArray.GetWord(4),
						PosY = byteArray.GetWord(6)
					});
				}
				catch (Exception ex)
				{
					Console.WriteLine("image processing err: " + ex);
				}
				offset += 16;
			}
			return list;
		}

		private List<FaceWidget> ProcessWidgets(byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(offset + 4);
			List<FaceWidget> list = new List<FaceWidget>();
			offset = dWord2;
			for (int i = 0; i < dWord; i++)
			{
				try
				{
					data.GetWord(offset);
					uint dWord3 = data.GetDWord(offset);
					uint dWord4 = data.GetDWord(offset + 8);
					uint dWord5 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(dWord4, dWord5);
					byte digits = byteArray[2];
					if (byteArray.GetWord(0, 0) == 4361 || byteArray.GetWord(0, 0) == 4362 || byteArray.GetWord(0, 0) == 4369 || byteArray.GetWord(0, 0) == 4370 || byteArray.GetWord(0, 0) == 4377 || byteArray.GetWord(0, 0) == 4378)
					{
						digits = 1;
					}
					list.Add(new FaceWidget
					{
						RawData = byteArray,
						Shape = byteArray[0],
						DataSrcDisplay = byteArray[1],
						X = ((byteArray.Length >= 32) ? byteArray.GetWord(20) : 0),
						Y = ((byteArray.Length >= 32) ? byteArray.GetWord(22) : 0),
						Width = 0,
						Height = 0,
						Id = dWord3,
						Digits = digits,
						Align = (byte)(byteArray[3] & 3),
						TypeId = byteArray[3] >> 4,
						TargetId = byteArray.GetDWord(8)
					});
				}
				catch (Exception ex)
				{
					Console.WriteLine("image processing err: " + ex);
				}
				offset += 16;
			}
			return list;
		}

		private List<FaceImage> ProcessImageSingle(WatchType watchType, byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(16);
			uint dWord2 = data.GetDWord(offset);
			uint dWord3 = data.GetDWord(offset + 4);
			List<FaceImage> list = new List<FaceImage>();
			offset = dWord3;
			for (int i = 0; i < dWord2; i++)
			{
				try
				{
					uint word = data.GetWord(offset);
					uint dWord4 = data.GetDWord(offset);
					uint dWord5 = data.GetDWord(offset + 8);
					uint dWord6 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(dWord5, dWord6);
					int word2 = byteArray.GetWord(4);
					int word3 = byteArray.GetWord(6);
					int num = byteArray[0];
					int num2 = byteArray[1];
					if (byteArray.GetDWord(0, 0u) == 0)
					{
						num2 = 4;
					}
					if (watchType == WatchType.Gen2 && dWord == 2048)
					{
						if (byteArray.GetByte(0) == 0)
						{
							num2 = 4;
						}
						if (byteArray.GetByte(0) == 3)
						{
							num2 = 2;
						}
					}
					uint dWord7 = byteArray.GetDWord(12);
					byte[] array = null;
					byte[] array2 = null;
					byte[] array3 = byteArray.GetByteArray(12u, (uint)(byteArray.Length - 12));
					if ((watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiBandPro || watchType == WatchType.MiBand7Pro) && (num & 0xF) == 4)
					{
						num2 = 2;
						uint num3 = (uint)(word2 * 2 * word3);
						array2 = array3.GetByteArray(num3, (uint)(word2 * word3));
						array3 = array3.GetByteArray(0u, num3);
					}
					if (watchType == WatchType.MiWatchS3 || (watchType == WatchType.MiBand9 && num == 16))
					{
						array3 = array3.ConvertToRGBA();
						num2 = 4;
					}
					if (dWord7 != 1520771552)
					{
						if (num2 == 1 && watchType != WatchType.MiWatchS3 && watchType != WatchType.MiBand9)
						{
							uint num4 = (uint)(21 + word2 * word3);
							array = byteArray.GetByteArray(num4 + 4, (uint)(byteArray.Length - (int)num4 - 4));
							array3 = byteArray.GetByteArray(12u, num4 - 12);
						}
					}
					else
					{
						byteArray = data.GetByteArray(dWord5, dWord6 + 4);
						uint dWord8 = byteArray.GetDWord(8);
						byte[] byteArray2 = byteArray.GetByteArray(20u, dWord8 - 8);
						num2 = byteArray[16] & 0xF;
						byteArray.GetDWord(16);
						Console.WriteLine($"got compressed image[{path.GetLastDirectory()}:{word}]: {dWord5:X8}, type:{num2:X2}, rle:{num:X2}");
						byte[] array4 = ImageCompressFactory.GetDecompressor(watchType).Decompress(byteArray2, word2, word3, byteArray.GetDWord(16));
						switch (num)
						{
						case 16:
							array3 = array4.ConvertToRGBA();
							num2 = 4;
							break;
						case 6:
							array3 = array4.Rgb565AlphaToRGBA();
							num2 = 4;
							break;
						}
					}
					byte[] array5 = null;
					array5 = ((dWord7 != 1520771552) ? BmpHelper.ConvertToBmpGTR(array3, word2, word3, num2, array) : BmpHelper.ConvertToBmpGTRv2(array3, word2, word3, num2));
					string text = path + $"img_{word:D4}_{num2}_{((array != null) ? array.Length : 0)}.bmp";
					string text2 = path + $"img_{word:D4}.png";
					File.WriteAllBytes(text, array5);
					using (MagickImage magickImage = new MagickImage())
					{
						magickImage.Read(text);
						magickImage.ColorType = ColorType.TrueColorAlpha;
						if (dWord7 == 1520771552 && watchType != WatchType.RedmiWatch3 && watchType != WatchType.RedmiWatch4 && watchType != WatchType.MiBand8Pro)
						{
							magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, byte.MaxValue));
						}
						else
						{
							magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
						}
						magickImage.Format = MagickFormat.Png32;
						if (num2 == 2 && (watchType == WatchType.RedmiWatch2 || watchType == WatchType.MiBand7Pro))
						{
							int num5 = 0;
							int num6 = 0;
							foreach (Pixel pixel in magickImage.GetPixels())
							{
								if (pixel.Channels == 4)
								{
									if (num6 >= word2)
									{
										pixel.SetChannel(3, 0);
									}
									else
									{
										pixel.SetChannel(3, (ushort)((array2[num5++] << 8) | 0xFF));
									}
									num6++;
									if (num6 >= magickImage.Width)
									{
										num6 = 0;
									}
								}
							}
						}
						magickImage.Write(text2);
					}
					File.Delete(text);
					list.Add(new FaceImage
					{
						Id = dWord4,
						Width = word2,
						Height = word3,
						Name = Path.GetFileName(text2)
					});
				}
				catch (Exception ex)
				{
					LogHelper.GotError = true;
					Console.WriteLine("image processing err: " + ex);
				}
				offset += 16;
			}
			return list;
		}

		private List<FaceImageList> ProcessImageList(WatchType watchType, byte[] data, uint offset, string path)
		{
			uint dWord = data.GetDWord(offset);
			uint dWord2 = data.GetDWord(offset + 4);
			List<FaceImageList> list = new List<FaceImageList>();
			offset = dWord2;
			for (int i = 0; i < dWord; i++)
			{
				try
				{
					uint word = data.GetWord(offset);
					uint dWord3 = data.GetDWord(offset);
					uint dWord4 = data.GetDWord(offset + 8);
					uint dWord5 = data.GetDWord(offset + 12);
					byte[] byteArray = data.GetByteArray(dWord4, dWord5);
					int word2 = byteArray.GetWord(4);
					int word3 = byteArray.GetWord(6);
					int num = byteArray[0];
					int num2 = byteArray[2];
					int num3 = num2;
					if (num == 0 && num3 == 0)
					{
						num3 = 4;
					}
					uint num4 = (uint)(word2 * 4 * word3);
					uint num5 = 0u;
					if (watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiBandPro || watchType == WatchType.RedmiWatch3 || watchType == WatchType.RedmiWatch4 || watchType == WatchType.MiBand8Pro || watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand9 || watchType == WatchType.MiBand7Pro)
					{
						if ((num & 0xF) == 4)
						{
							num3 = 2;
							num4 = (uint)(word2 * 2 * word3);
							num5 = (uint)(word2 * word3);
						}
						if ((watchType == WatchType.RedmiWatch3 || watchType == WatchType.RedmiWatch4 || watchType == WatchType.MiBand8Pro) && (num & 0xF) == 3)
						{
							num3 = 2;
							num4 = (uint)(word2 * 2 * word3);
							num5 = 0u;
						}
						if (num2 == 0 && (watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand9) && (num & 0xFF) == 16)
						{
							num3 = 1;
							num4 = (uint)(word2 * word3 + 1024);
							num5 = 0u;
						}
					}
					uint num6 = byteArray[1];
					uint num7 = byteArray.GetDWord(8) + 12;
					uint dWord6 = byteArray.GetDWord(12 + 4 * num6);
					Console.WriteLine($"got compressed imageList[{path.GetLastDirectory()}:{word}]: {dWord4:X8}, type:{num3:X2}, rle:{num:X2}");
					List<string> list2 = new List<string>();
					for (int j = 0; j < num6; j++)
					{
						uint num8 = (uint)(12 + (int)(j * (num4 + num5)));
						uint len = ((num7 - num8 >= num4) ? num4 : (num7 - num8));
						byte[] array = null;
						byte[] array2 = null;
						byte[] array3 = null;
						byte[] array4 = null;
						if (dWord6 == 1520771552)
						{
							num8 = 12 + 4 * num6;
							for (int k = 0; k < j; k++)
							{
								num8 += byteArray.GetDWord((uint)(12 + k * 4));
							}
							len = ((num7 - num8 >= num4) ? num4 : (num7 - num8));
							uint dWord7 = byteArray.GetDWord((uint)(12 + j * 4));
							byte[] byteArray2 = byteArray.GetByteArray(num8 + 8, dWord7 - 8);
							num3 = byteArray[num8 + 4] & 0xF;
							byteArray.GetDWord(num8 + 4);
							byte[] array5 = ImageCompressFactory.GetDecompressor(watchType).Decompress(byteArray2, word2, word3, byteArray.GetDWord(num8 + 4));
							switch (num)
							{
							case 16:
								array = array5.ConvertToRGBA();
								num3 = 4;
								break;
							case 6:
								array = array5.Rgb565AlphaToRGBA();
								num3 = 4;
								break;
							}
							array2 = array;
						}
						else
						{
							array = byteArray.GetByteArray(num8, len);
							array2 = new byte[num4];
							Array.Copy(array, array2, array.Length);
							if (num3 == 2 && (watchType == WatchType.RedmiWatch2 || watchType == WatchType.MiBand7Pro))
							{
								array3 = new byte[num5];
								Array.Copy(byteArray.GetByteArray(num8 + num4, num5), array3, num5);
							}
							if (num3 == 1 && (watchType == WatchType.MiWatchS3 || watchType == WatchType.MiBand9))
							{
								array2 = array.ConvertToRGBA();
							}
						}
						byte[] array6 = null;
						array6 = ((dWord6 != 1520771552) ? BmpHelper.ConvertToBmpGTR(array2, word2, word3, (num3 == 1 && array4 == null) ? 4 : num3, array4) : BmpHelper.ConvertToBmpGTRv2(array, word2, word3, num3));
						string text = path + $"img_arr_{word:D4}_{j:D2}.bmp";
						string text2 = path + $"img_arr_{word:D4}_{j:D2}.png";
						File.WriteAllBytes(text, array6);
						list2.Add(Path.GetFileName(text2));
						using (MagickImage magickImage = new MagickImage())
						{
							magickImage.Read(text);
							magickImage.ColorType = ColorType.TrueColorAlpha;
							if (dWord6 == 1520771552)
							{
								magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, byte.MaxValue));
							}
							else
							{
								magickImage.Transparent(MagickColor.FromRgba(0, 0, 0, 0));
							}
							magickImage.Format = MagickFormat.Png32;
							if (num3 == 2 && (watchType == WatchType.RedmiWatch2 || watchType == WatchType.MiBand7Pro))
							{
								int num9 = 0;
								int num10 = 0;
								foreach (Pixel pixel in magickImage.GetPixels())
								{
									if (pixel.Channels == 4)
									{
										if (num10 >= word2)
										{
											pixel.SetChannel(3, 0);
										}
										else
										{
											pixel.SetChannel(3, (ushort)((array3[num9++] << 8) | 0xFF));
										}
										num10++;
										if (num10 >= magickImage.Width)
										{
											num10 = 0;
										}
									}
								}
							}
							magickImage.Write(text2);
						}
						File.Delete(text);
					}
					list.Add(new FaceImageList
					{
						Id = dWord3,
						Width = word2,
						Height = word3,
						NameList = list2.ToArray()
					});
				}
				catch (Exception ex)
				{
					LogHelper.GotError = true;
					Console.WriteLine("image processing err: " + ex);
				}
				offset += 16;
			}
			return list;
		}
	}
}
