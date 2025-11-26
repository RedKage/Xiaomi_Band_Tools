using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MiWatchCompiler.Helpers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.Action;
using XiaomiWatch.Common.App;
using XiaomiWatch.Common.Compress;
using XiaomiWatch.Common.FaceFile;

namespace MiWatchCompiler.Packers
{
	internal class DefaultPacker
	{
		private static byte[] header = new byte[168]
		{
			90, 165, 52, 18, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 1, 7, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
			0, 0, 255, 255, 255, 255, 0, 0, 0, 0,
			49, 54, 55, 50, 49, 48, 48, 54, 53, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0
		};

		private static byte[] faceHeader = new byte[88]
		{
			0, 0, 0, 0, 255, 255, 255, 255, 255, 255,
			255, 255, 0, 255, 255, 255, 0, 0, 0, 0,
			0, 0, 0, 0, 255, 255, 255, 255, 2, 255,
			255, 255, 255, 255, 255, 255, 3, 255, 255, 255,
			0, 0, 0, 0, 255, 255, 255, 255, 0, 0,
			0, 0, 255, 255, 255, 255, 0, 0, 0, 0,
			255, 255, 255, 255, 255, 255, 255, 255, 7, 255,
			255, 255, 0, 0, 0, 0, 255, 255, 255, 255,
			0, 0, 0, 0, 255, 255, 255, 255
		};

		private static byte[] aodHeader = new byte[88]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 255, 255,
			255, 255, 0, 255, 255, 255, 0, 0, 0, 0,
			0, 0, 0, 0, 255, 255, 255, 255, 2, 255,
			255, 255, 255, 255, 255, 255, 3, 255, 255, 255,
			0, 0, 0, 0, 255, 255, 255, 255, 0, 0,
			0, 0, 255, 255, 255, 255, 0, 0, 0, 0,
			255, 255, 255, 255, 255, 255, 255, 255, 7, 255,
			255, 255, 0, 0, 0, 0, 255, 255, 255, 255,
			0, 0, 0, 0, 255, 255, 255, 255
		};

		private const uint OffsetVersion = 16u;

		private const uint OffsetPreview = 32u;

		private const uint OffsetFaceCount = 28u;

		private const uint OffsetFace = 168u;

		private const uint OffsetId = 40u;

		private const int OffsetName = 104;

		private const int ElementSize = 16;

		private const int FaceSize = 88;

		private static int faceCount = 1;

		private static uint offsetData = 168u;

		private static EnumerationHelper enumerationHelper;

		private static bool hasAod = false;

		internal static WatchType Exec(string mainFile, string dstFile)
		{
			string directoryName = Path.GetDirectoryName(mainFile);
			PathHelper.SetRootPath(directoryName);
			if (!Directory.Exists(PathHelper.PathImages))
			{
				throw new Exception("images path is not found");
			}
			string[] files = Directory.GetFiles(directoryName, "*.fprj");
			if (files == null || files.Length < 1)
			{
				throw new Exception("fprj files is not found by path: " + directoryName);
			}
			string[] array = files;
			faceCount = array.Length;
			if (Directory.Exists(PathHelper.PathImagesAod))
			{
				hasAod = true;
				faceCount++;
			}
			FaceProject faceProject = FaceProject.Deserialize(Namespace.Prepare(File.ReadAllText(mainFile)));
			FaceProject faceProject2 = null;
			List<FaceProject> list = new List<FaceProject>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!(text == mainFile))
				{
					FaceProject item = FaceProject.Deserialize(Namespace.Prepare(File.ReadAllText(text)));
					list.Add(item);
				}
			}
			byte[] array3 = header;
			for (int j = 0; j < faceCount; j++)
			{
				byte[] byteArray = faceHeader.GetByteArray(0, faceHeader.Length);
				if (faceProject.DeviceType == 7 || faceProject.DeviceType == 9)
				{
					byteArray[3] = 2;
				}
				array3 = array3.Concat(byteArray).ToArray();
			}
			SetupHeader(array3, faceProject, hasAod);
			if (hasAod)
			{
				files = Directory.GetFiles(directoryName + "\\AOD", "*.fprj");
				if (files == null || files.Count() != 1)
				{
					throw new Exception("AOD face file is not found");
				}
				faceProject2 = FaceProject.Deserialize(Namespace.Prepare(File.ReadAllText(files[0])));
				faceProject2.IsAOD = true;
			}
			int num = 0;
			uint num2 = (uint)(168 + faceCount * 88);
			uint baseOffset = num2;
			uint num3 = (uint)CountElementsSize(array3, faceProject, num++, baseOffset);
			if (hasAod)
			{
				baseOffset = num2 + num3;
				num3 += (uint)CountElementsSize(array3, faceProject2, num++, baseOffset);
			}
			foreach (FaceProject item2 in list)
			{
				baseOffset = num2 + num3;
				num3 += (uint)CountElementsSize(array3, item2, num++, baseOffset);
			}
			array3 = array3.AppendZero(num3);
			offsetData = (uint)(168 + faceCount * 88) + num3;
			enumerationHelper = new EnumerationHelper(faceProject.DeviceType);
			num = 0;
			array3 = SetupElements(array3, faceProject, num);
			array3 = SetupWidgets(array3, faceProject, num);
			array3 = SetupImage(array3, faceProject, num);
			array3 = SetupImageList(array3, faceProject, num);
			array3 = SetupAnimation(array3, faceProject, num);
			array3 = SetupApp(array3, faceProject, num);
			array3 = SetupAction(array3, faceProject, num);
			array3 = SetupOption(array3, faceProject, num);
			if (hasAod)
			{
				num++;
				array3 = SetupElements(array3, faceProject2, num);
				array3 = SetupWidgets(array3, faceProject2, num);
				array3 = SetupImage(array3, faceProject2, num);
				array3 = SetupImageList(array3, faceProject2, num);
				array3 = SetupAction(array3, faceProject2, num);
				array3 = SetupOption(array3, faceProject2, num);
			}
			foreach (FaceProject item3 in list)
			{
				num++;
				array3 = SetupElements(array3, item3, num);
				array3 = SetupWidgets(array3, item3, num);
				array3 = SetupImage(array3, item3, num);
				array3 = SetupImageList(array3, item3, num);
				array3 = SetupAnimation(array3, item3, num);
				array3 = SetupApp(array3, item3, num);
				array3 = SetupAction(array3, item3, num);
				array3 = SetupOption(array3, item3, num);
			}
			num = 0;
			array3 = SetElementProp(array3, faceProject, num);
			array3 = SetWidgetProp(array3, faceProject, num);
			if (hasAod)
			{
				num++;
				array3 = SetElementProp(array3, faceProject2, num);
				array3 = SetWidgetProp(array3, faceProject2, num);
			}
			try
			{
				foreach (FaceProject item4 in list)
				{
					num++;
					array3 = SetElementProp(array3, item4, num);
					array3 = SetWidgetProp(array3, item4, num);
				}
			}
			catch
			{
			}
			array3 = SetupPreview(array3, faceProject, 0);
			File.WriteAllBytes(dstFile, array3);
			return (WatchType)faceProject.DeviceType;
		}

		private static byte[] SetWidgetProp(byte[] data, FaceProject face, int faceIndex)
		{
			uint num = data.GetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 56 + 4);
			List<FaceWidgetImage> source = face.Screen.Widgets.Where((FaceWidget d) => d.IsRef).OfType<FaceWidgetImage>().ToList();
			foreach (FaceWidget widget in face.Screen.Widgets)
			{
				if (widget is FaceWidgetContainer || widget is FaceWidgetImage || (widget is FaceWidgetImageList && widget.IsAnimation))
				{
					continue;
				}
				uint dWord = data.GetDWord(num + 8);
				uint dWord2 = data.GetDWord(num + 12);
				if (widget is FaceWidgetImageList)
				{
					FaceWidgetImageList faceWidgetImageList = widget as FaceWidgetImageList;
					data.SetWord(dWord, FaceItemDataSrc.Parse(faceWidgetImageList.DataSrcIndex), 1u);
					data[dWord + 3] = 32;
					data.SetDWord(dWord + 8, faceWidgetImageList.ImageId);
					if (faceWidgetImageList.HasCustomValues && dWord2 == 16)
					{
						throw new Exception("ImageList[" + widget.Name + "] has incorrect custom values");
					}
					if (faceWidgetImageList.HasAngle)
					{
						data.SetWord(dWord + 14, faceWidgetImageList.GetAngle());
					}
					if (faceWidgetImageList.HasCustomValues)
					{
						data.SetDWord(dWord + 12, 512);
						for (int num2 = 0; num2 < faceWidgetImageList.CustomValues.Length; num2++)
						{
							data.SetDWord(dWord + 16 + (uint)(num2 * 4), faceWidgetImageList.CustomValues[num2]);
						}
					}
				}
				else if (widget is FaceWidgetDigitalNum)
				{
					FaceWidgetDigitalNum wgt = widget as FaceWidgetDigitalNum;
					data.SetWord(dWord, FaceItemDataSrc.Parse(wgt.DataSrcValue), 1u);
					data[dWord + 2] = (byte)wgt.Digits;
					byte b = ((wgt.Blanking == 0) ? ((byte)1) : ((byte)0));
					data[dWord + 3] = (byte)(0x10 | ((b << 2) & 4) | ParseAlignment(wgt.Alignment));
					data.SetDWord(dWord + 8, wgt.ImageId);
					data[dWord + 13] = (byte)wgt.Spacing;
					if (face.DeviceType == 3652 || face.DeviceType == 3651)
					{
						data.SetWord(dWord + 6, 1000);
					}
					if (wgt.HasAngle)
					{
						data.SetWord(dWord + 14, wgt.GetAngle());
					}
					FaceWidgetImage faceWidgetImage = source.FirstOrDefault((FaceWidgetImage d) => d.RefName == wgt.Name);
					if (faceWidgetImage != null)
					{
						data.SetDWord(dWord + 16, faceWidgetImage.ImageId);
					}
				}
				else if (widget is FaceWidgetAnalogClock)
				{
					bool flag = false;
					FaceWidgetAnalogClock faceWidgetAnalogClock = widget as FaceWidgetAnalogClock;
					if (faceWidgetAnalogClock.HourTargetId != 0)
					{
						data.SetWord(dWord, FaceItemDataSrc.Parse("0811"), 1u);
						data.SetDWord(dWord + 8, faceWidgetAnalogClock.HourImageId);
						data[dWord + 3] = 48;
						data.SetWord(dWord + 6, 1000);
						data[dWord + 17] = 24;
						data.SetWord(dWord + 20, faceWidgetAnalogClock.HourImageRotateX);
						data.SetWord(dWord + 22, faceWidgetAnalogClock.HourImageRotateY);
						data.SetWord(dWord + 24, 0, 0u);
						data.SetWord(dWord + 26, 7200);
						flag = true;
					}
					if (faceWidgetAnalogClock.MinuteTargetId != 0)
					{
						if (flag)
						{
							num += 16;
							dWord = data.GetDWord(num + 8);
							if (dWord == 0)
							{
								throw new ApplicationException("Analog Clock minute image apply failure");
							}
						}
						data.SetWord(dWord, FaceItemDataSrc.Parse("1011"), 1u);
						data.SetDWord(dWord + 8, faceWidgetAnalogClock.MinuteImageId);
						data[dWord + 3] = 48;
						data.SetWord(dWord + 6, 1000);
						data[dWord + 17] = 60;
						data.SetWord(dWord + 20, faceWidgetAnalogClock.MinuteImageRotateX);
						data.SetWord(dWord + 22, faceWidgetAnalogClock.MinuteImageRotateY);
						data.SetWord(dWord + 24, 0, 0u);
						data.SetWord(dWord + 26, 3600);
						flag = true;
					}
					if (faceWidgetAnalogClock.SecondTargetId != 0)
					{
						if (flag)
						{
							num += 16;
							dWord = data.GetDWord(num + 8);
							if (dWord == 0)
							{
								throw new ApplicationException("Analog Clock seconds image apply failure");
							}
						}
						data.SetWord(dWord, FaceItemDataSrc.Parse("1811"), 1u);
						data.SetDWord(dWord + 8, faceWidgetAnalogClock.SecondImageId);
						data[dWord + 3] = 48;
						data.SetWord(dWord + 6, faceWidgetAnalogClock.HasSmoothSecondAnimation ? faceWidgetAnalogClock.GetSmoothValue() : 1000);
						data[dWord + 17] = 60;
						data.SetWord(dWord + 20, faceWidgetAnalogClock.SecondImageRotateX);
						data.SetWord(dWord + 22, faceWidgetAnalogClock.SecondImageRotateY);
						data.SetWord(dWord + 24, 0, 0u);
						data.SetWord(dWord + 26, 3600);
					}
				}
				else if (widget is FaceWidgetCircleProgress)
				{
					FaceWidgetCircleProgress faceWidgetCircleProgress = widget as FaceWidgetCircleProgress;
					data.SetWord(dWord, FaceItemDataSrc.Parse(faceWidgetCircleProgress.DbSrcRangeValue), 1u);
					data[dWord + 3] = 64;
					data.SetDWord(dWord + 8, faceWidgetCircleProgress.ImageId);
				}
				else if (widget is FaceWidgetCircleProgressPlus)
				{
					FaceWidgetCircleProgressPlus faceWidgetCircleProgressPlus = widget as FaceWidgetCircleProgressPlus;
					data.SetWord(dWord, FaceItemDataSrc.Parse(faceWidgetCircleProgressPlus.DbSrcRangeValue), 1u);
					if (faceWidgetCircleProgressPlus.Name.ToLower().StartsWith("lineprogress_"))
					{
						data[dWord + 3] = 80;
					}
					else
					{
						data[dWord + 3] = 64;
					}
					data.SetDWord(dWord + 8, faceWidgetCircleProgressPlus.ImageId);
					data.SetDWord(dWord + 20, faceWidgetCircleProgressPlus.RangeMin << 8);
					data.SetDWord(dWord + 24, faceWidgetCircleProgressPlus.RangeMax << 8);
					data.SetWord(dWord + 28, faceWidgetCircleProgressPlus.RotateX);
					data.SetWord(dWord + 30, faceWidgetCircleProgressPlus.RotateY);
					if (faceWidgetCircleProgressPlus.Name.ToLower().StartsWith("lineprogress_"))
					{
						data.SetWord(dWord + 32, (short)faceWidgetCircleProgressPlus.StartAngle);
						data.SetWord(dWord + 34, (short)faceWidgetCircleProgressPlus.EndAngle);
					}
					else
					{
						if (face.DeviceType == 7)
						{
							short num3 = (short)(faceWidgetCircleProgressPlus.StartAngle - 90);
							if (num3 < 0)
							{
								num3 = (short)(270 + faceWidgetCircleProgressPlus.StartAngle);
							}
							short num4 = (short)(faceWidgetCircleProgressPlus.EndAngle - 90);
							if (num4 < 0)
							{
								num4 = (short)(270 + faceWidgetCircleProgressPlus.EndAngle);
							}
							short num5 = 0;
							num5 = (short)((num4 - num3 < 360) ? ((num4 - num3 > 180) ? ((short)(-((num3 - num4) * 10))) : ((short)((num4 - num3) * 10))) : 0);
							data.SetWord(dWord + 32, num3 * 10);
							data.SetWord(dWord + 34, num5);
						}
						else
						{
							data.SetWord(dWord + 32, (short)faceWidgetCircleProgressPlus.StartAngle * 10);
							data.SetWord(dWord + 34, (short)((short)faceWidgetCircleProgressPlus.EndAngle - (short)faceWidgetCircleProgressPlus.StartAngle) * 10);
						}
						data.SetWord(dWord + 36, faceWidgetCircleProgressPlus.Radius);
					}
					data.SetWord(dWord + 38, faceWidgetCircleProgressPlus.LineWidth);
					data[dWord + 16] = ((faceWidgetCircleProgressPlus.ButtonCapEndingStyleEnabled != 1) ? ((byte)1) : ((byte)0));
				}
				num += 16;
			}
			return data;
		}

		private static byte ParseAlignment(int alignment)
		{
			switch (alignment)
			{
			case 1:
				return 2;
			case 2:
				return 0;
			default:
				return 1;
			}
		}

		private static byte[] SetElementProp(byte[] data, FaceProject face, int faceIndex)
		{
			uint num = data.GetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 4);
			foreach (FaceWidget widget in face.Screen.Widgets)
			{
				if (widget is FaceWidgetImage && widget.IsRef)
				{
					continue;
				}
				uint dWord = data.GetDWord(num + 8);
				int num2 = widget.X;
				int val = widget.Y;
				FaceWidgetContainer obj = widget as FaceWidgetContainer;
				if (obj != null && obj.IsApp)
				{
					data.SetDWord(dWord, widget.TargetId);
					data.SetDWord(dWord + 8, 0, 0u);
					data.SetDWord(dWord + 12, 0, 0u);
				}
				else if (widget is FaceWidgetImage)
				{
					if ((widget as FaceWidgetImage).IsButton || (widget as FaceWidgetImage).IsApp)
					{
						data = ActionFactory.GetPacker((WatchType)face.DeviceType).SetElement(data, dWord, widget);
						data.SetDWord(dWord, widget.TargetId);
					}
					else
					{
						data.SetDWord(dWord, widget.ImageId);
					}
				}
				else if (widget is FaceWidgetDigitalNum)
				{
					FaceWidgetDigitalNum faceWidgetDigitalNum = widget as FaceWidgetDigitalNum;
					string text = faceWidgetDigitalNum.BitmapList.Split('|')[0];
					text = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + text;
					int bitmapWidth = MagikHelper.GetBitmapWidth(text);
					if (faceWidgetDigitalNum.Alignment == 1)
					{
						num2 += bitmapWidth * faceWidgetDigitalNum.Digits / 2;
					}
					else if (faceWidgetDigitalNum.Alignment == 2)
					{
						num2 += bitmapWidth * faceWidgetDigitalNum.Digits;
					}
					data.SetDWord(dWord, widget.TargetId);
				}
				else if (widget is FaceWidgetAnalogClock)
				{
					bool flag = false;
					FaceWidgetAnalogClock faceWidgetAnalogClock = widget as FaceWidgetAnalogClock;
					if (faceWidgetAnalogClock.HourTargetId != 0)
					{
						int num3 = faceWidgetAnalogClock.X + faceWidgetAnalogClock.Width / 2;
						int num4 = faceWidgetAnalogClock.Y + faceWidgetAnalogClock.Height / 2;
						num2 = num3 - faceWidgetAnalogClock.HourImageRotateX;
						val = num4 - faceWidgetAnalogClock.HourImageRotateY;
						data.SetWord(dWord + 4, num2);
						data.SetWord(dWord + 6, val);
						data.SetDWord(dWord, faceWidgetAnalogClock.HourTargetId);
						flag = true;
					}
					if (faceWidgetAnalogClock.MinuteTargetId != 0)
					{
						if (flag)
						{
							num += 16;
							dWord = data.GetDWord(num + 8);
							int num5 = faceWidgetAnalogClock.X + faceWidgetAnalogClock.Width / 2;
							int num6 = faceWidgetAnalogClock.Y + faceWidgetAnalogClock.Height / 2;
							num2 = num5 - faceWidgetAnalogClock.MinuteImageRotateX;
							val = num6 - faceWidgetAnalogClock.MinuteImageRotateY;
							data.SetWord(dWord + 4, num2);
							data.SetWord(dWord + 6, val);
						}
						data.SetDWord(dWord, faceWidgetAnalogClock.MinuteTargetId);
						flag = true;
					}
					if (faceWidgetAnalogClock.SecondTargetId != 0)
					{
						if (flag)
						{
							num += 16;
							dWord = data.GetDWord(num + 8);
							int num7 = faceWidgetAnalogClock.X + faceWidgetAnalogClock.Width / 2;
							int num8 = faceWidgetAnalogClock.Y + faceWidgetAnalogClock.Height / 2;
							num2 = num7 - faceWidgetAnalogClock.SecondImageRotateX;
							val = num8 - faceWidgetAnalogClock.SecondImageRotateY;
							data.SetWord(dWord + 4, num2);
							data.SetWord(dWord + 6, val);
						}
						data.SetDWord(dWord, faceWidgetAnalogClock.SecondTargetId);
						flag = true;
					}
				}
				else if (widget is FaceWidgetCircleProgress)
				{
					data.SetDWord(dWord, widget.TargetId);
				}
				else if (widget is FaceWidgetCircleProgressPlus)
				{
					data.SetDWord(dWord, widget.TargetId);
				}
				else if (widget is FaceWidgetImageList)
				{
					if (widget.IsAnimation)
					{
						data.SetDWord(dWord, widget.AnimationId);
					}
					else
					{
						data.SetDWord(dWord, widget.TargetId);
					}
				}
				else
				{
					data.SetDWord(dWord, widget.TargetId);
				}
				data.SetWord(dWord + 4, num2);
				data.SetWord(dWord + 6, val);
				num += 16;
			}
			return data;
		}

		private static byte[] SetupPreview(byte[] data, FaceProject face, int faceIndex)
		{
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 4;
			if (string.IsNullOrWhiteSpace(face.Screen.Bitmap))
			{
				throw new Exception("preview file is missing");
			}
			string text = PathHelper.PathImages + "\\" + face.Screen.Bitmap;
			bool withAlfa = face.Screen.Bitmap.ToLower().Contains("_rgba");
			if (!File.Exists(text))
			{
				throw new Exception("preview file is missing");
			}
			data.SetDWord(32u, offsetData);
			data.SetDWord(offset, offsetData);
			WatchType deviceType = (WatchType)face.DeviceType;
			int width;
			int height;
			byte[] bitmapFromPng = MagikHelper.GetBitmapFromPng(deviceType, text, 0, out width, out height);
			if (WatchDetector.GetWatchType(width, height) != deviceType)
			{
				(int, int) watchPreviewSize = WatchDetector.GetWatchPreviewSize(deviceType);
				if (width != watchPreviewSize.Item1 || height != watchPreviewSize.Item2)
				{
					throw new Exception($"Preview has wrong size: {width}x{height}, expected: {watchPreviewSize.Item1}x{watchPreviewSize.Item2}");
				}
			}
			IDataCompressor compressor = ImageCompressFactory.GetCompressor(deviceType, withAlfa);
			byte[] array = null;
			try
			{
				array = (compressor.RequiredCompressWithParams ? compressor.Compress(bitmapFromPng, width, height) : compressor.Compress(bitmapFromPng));
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to compress preview: " + ex.Message);
			}
			uint v = (uint)(12 + array.Length);
			uint num = offsetData;
			offsetData += v.GetDWordAligned();
			data = data.AppendZero((uint)(offsetData - data.Length));
			data.SetByteArray(num, compressor.GetHeader());
			data.SetWord(num + 4, width);
			data.SetWord(num + 6, height);
			data.SetDWord(num + 8, array.Length);
			data.SetByteArray(num + 12, array);
			return data;
		}

		private static byte[] SetupElements(byte[] data, FaceProject face, int faceIndex)
		{
			int num = face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage && !w.IsRef)
				{
					return 1;
				}
				if (w is FaceWidgetAnalogClock)
				{
					int num4 = 0;
					FaceWidgetAnalogClock obj = w as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(obj.HourHandImage))
					{
						num4++;
					}
					if (!string.IsNullOrEmpty(obj.MinuteHandImage))
					{
						num4++;
					}
					if (!string.IsNullOrEmpty(obj.SecondHandImage))
					{
						num4++;
					}
					return num4;
				}
				return 1;
			});
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 4;
			uint num2 = data.GetDWord(offset);
			for (int num3 = 0; num3 < num; num3++)
			{
				data[num2] = (byte)num3;
				data.SetDWord(num2 + 8, offsetData);
				data[num2 + 12] = 16;
				offsetData += 16u;
				num2 += 16;
			}
			data = data.AppendZero((uint)(offsetData - data.Length));
			return data;
		}

		private static byte[] SetupWidgets(byte[] data, FaceProject face, int faceIndex)
		{
			enumerationHelper.ResetWidgetIndex();
			face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage)
				{
					return 0;
				}
				if (w is FaceWidgetImageList)
				{
					return (!w.IsAnimation) ? 1 : 0;
				}
				if (w is FaceWidgetAnalogClock)
				{
					int num2 = 0;
					FaceWidgetAnalogClock obj2 = w as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(obj2.HourHandImage))
					{
						num2++;
					}
					if (!string.IsNullOrEmpty(obj2.MinuteHandImage))
					{
						num2++;
					}
					if (!string.IsNullOrEmpty(obj2.SecondHandImage))
					{
						num2++;
					}
					return num2;
				}
				FaceWidgetContainer obj3 = w as FaceWidgetContainer;
				return (obj3 == null || !obj3.IsApp) ? 1 : 0;
			});
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 56 + 4;
			offset = data.GetDWord(offset);
			foreach (FaceWidget widget in face.Screen.Widgets)
			{
				if (widget is FaceWidgetImage || (widget is FaceWidgetImageList && widget.IsAnimation))
				{
					continue;
				}
				FaceWidgetContainer obj = widget as FaceWidgetContainer;
				if (obj != null && obj.IsApp)
				{
					continue;
				}
				data[offset] = enumerationHelper.GetWidgetIndex();
				data[offset + 3] = 7;
				data.SetDWord(offset + 8, offsetData);
				uint num = 16u;
				if (widget is FaceWidgetImageList)
				{
					FaceWidgetImageList faceWidgetImageList = widget as FaceWidgetImageList;
					widget.TargetId = data.GetDWord(offset);
					num = widget.WidgetSize;
					if (faceWidgetImageList.HasCustomValues)
					{
						num = faceWidgetImageList.WidgetSize;
					}
				}
				else if (widget is FaceWidgetDigitalNum)
				{
					widget.TargetId = data.GetDWord(offset);
					num = widget.WidgetSize;
				}
				else if (widget is FaceWidgetAnalogClock)
				{
					bool flag = false;
					num = widget.WidgetSize;
					FaceWidgetAnalogClock faceWidgetAnalogClock = widget as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.HourHandImage))
					{
						faceWidgetAnalogClock.HourTargetId = data.GetDWord(offset);
						flag = true;
					}
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.MinuteHandImage))
					{
						if (flag)
						{
							data.SetDWord(offset + 12, num);
							offsetData += num;
							offset += 16;
							data[offset] = enumerationHelper.GetWidgetIndex();
							data[offset + 3] = 7;
							data.SetDWord(offset + 8, offsetData);
						}
						faceWidgetAnalogClock.MinuteTargetId = data.GetDWord(offset);
						flag = true;
					}
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.SecondHandImage))
					{
						if (flag)
						{
							data.SetDWord(offset + 12, num);
							offsetData += num;
							offset += 16;
							data[offset] = enumerationHelper.GetWidgetIndex();
							data[offset + 3] = 7;
							data.SetDWord(offset + 8, offsetData);
						}
						faceWidgetAnalogClock.SecondTargetId = data.GetDWord(offset);
					}
				}
				else if (widget is FaceWidgetCircleProgress)
				{
					widget.TargetId = data.GetDWord(offset);
					num = ((face.DeviceType != 7) ? widget.WidgetSize : 52u);
				}
				else if (widget is FaceWidgetCircleProgressPlus)
				{
					widget.TargetId = data.GetDWord(offset);
					num = ((face.DeviceType != 7) ? widget.WidgetSize : 52u);
				}
				data.SetDWord(offset + 12, num);
				offsetData += num;
				offset += 16;
			}
			data = data.AppendZero((uint)(offsetData - data.Length));
			return data;
		}

		private static byte[] SetupAnimation(byte[] data, FaceProject face, int faceIndex)
		{
			int num = 0;
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 32;
			uint dWord = data.GetDWord(offset);
			offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 32 + 4;
			offset = data.GetDWord(offset);
			if (dWord == 0)
			{
				return data;
			}
			Console.WriteLine("Found animation: packing..");
			int num2 = 12;
			foreach (FaceWidget item in face.Screen.Widgets.Where((FaceWidget w) => (w as FaceWidgetImageList)?.IsAnimation ?? false))
			{
				data[offset] = (byte)num++;
				data[offset + 3] = 4;
				data.SetDWord(offset + 8, offsetData);
				data.SetDWord(offset + 12, num2);
				FaceWidgetImageList obj = item as FaceWidgetImageList;
				uint imageId = obj.ImageId;
				obj.AnimationId = data.GetDWord(offset);
				uint num3 = offsetData;
				offsetData += (uint)num2;
				data = data.AppendZero((uint)(offsetData - data.Length));
				Tuple<int, int> animationData = AnimationHelper.GetAnimationData(item);
				data.SetDWord(num3, imageId);
				data.SetWord(num3 + 6, animationData.Item1);
				data.SetWord(num3 + 8, animationData.Item2);
				offset += 16;
			}
			return data;
		}

		private static byte[] SetupOption(byte[] data, FaceProject face, int faceIndex)
		{
			int num = 0;
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 64;
			uint dWord = data.GetDWord(offset);
			offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 64 + 4;
			offset = data.GetDWord(offset);
			if (dWord == 0)
			{
				return data;
			}
			int num2 = (int)(4 * dWord + 4);
			foreach (FaceWidget item in face.Screen.Widgets.Where((FaceWidget w) => false))
			{
				data[offset] = (byte)num++;
				data[offset + 3] = 8;
				data.SetDWord(offset + 8, offsetData);
				data.SetDWord(offset + 12, num2);
				FaceWidgetImage obj = item as FaceWidgetImage;
				uint targetId = obj.TargetId;
				obj.ActionId = data.GetDWord(offset);
				uint num3 = offsetData;
				offsetData += (uint)num2;
				data = data.AppendZero((uint)(offsetData - data.Length));
				data.SetDWord(num3, dWord);
				for (uint num4 = 0u; num4 < dWord; num4++)
				{
					data.SetDWord(num3 + 4 + 4 * num4, targetId);
				}
				offset += 16;
			}
			return data;
		}

		private static byte[] SetupAction(byte[] data, FaceProject face, int faceIndex)
		{
			int num = 0;
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 72;
			uint dWord = data.GetDWord(offset);
			offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 72 + 4;
			offset = data.GetDWord(offset);
			if (dWord == 0)
			{
				return data;
			}
			foreach (FaceWidget item in face.Screen.Widgets.Where((FaceWidget w) => w is FaceWidgetImage && (w.IsButton || w.IsApp)))
			{
				data[offset] = (byte)num++;
				data[offset + 3] = 9;
				data.SetDWord(offset + 8, offsetData);
				FaceWidgetImage faceWidgetImage = item as FaceWidgetImage;
				faceWidgetImage.TargetId = data.GetDWord(offset);
				if (!faceWidgetImage.Name.Contains('[') || !faceWidgetImage.Name.Contains(']'))
				{
					throw new ApplicationException("Failed to pack button action: missing or wrong app name");
				}
				try
				{
					string actionName = faceWidgetImage.Name.Split('[', ']')[1];
					IActionPacker packer = ActionFactory.GetPacker((WatchType)face.DeviceType);
					int actionSize = packer.GetActionSize(actionName);
					data.SetDWord(offset + 12, actionSize);
					uint actionOffset = offsetData;
					offsetData += (uint)actionSize;
					data = data.AppendZero((uint)(offsetData - data.Length));
					data = packer.Pack(actionName, data, actionOffset, faceWidgetImage);
				}
				catch
				{
					throw new ApplicationException("Failed to pack button action: missing or wrong app name");
				}
				offset += 16;
			}
			return data;
		}

		private static byte[] SetupApp(byte[] data, FaceProject face, int faceIndex)
		{
			enumerationHelper.ResetAppIndex();
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 40;
			uint dWord = data.GetDWord(offset);
			offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 40 + 4;
			offset = data.GetDWord(offset);
			if (dWord == 0)
			{
				return data;
			}
			List<FaceWidgetContainer> source = (from w in face.Screen.Widgets.OfType<FaceWidgetContainer>()
				where w.IsApp
				select w).ToList();
			foreach (string appFile in AppHelper.GetAppFileList())
			{
				data[offset] = enumerationHelper.GetAppIndex();
				data[offset + 3] = 5;
				data.SetDWord(offset + 8, offsetData);
				if (!File.Exists(appFile))
				{
					throw new ApplicationException("App packing filed, missing file: " + appFile);
				}
				byte[] array = File.ReadAllBytes(appFile);
				string text = appFile.Replace(PathHelper.PathApp + "\\", "").Replace('\\', '/');
				string escAppName = "app_" + Uri.EscapeDataString(text);
				FaceWidgetContainer faceWidgetContainer = source.FirstOrDefault((FaceWidgetContainer w) => w.Name == escAppName);
				if (faceWidgetContainer != null)
				{
					faceWidgetContainer.TargetId = data.GetDWord(offset);
				}
				int num = 20 + text.Length + array.Length;
				int dWordAligned = num.GetDWordAligned();
				data.SetDWord(offset + 12, num);
				uint num2 = offsetData;
				offsetData += (uint)dWordAligned;
				data = data.AppendZero((uint)(offsetData - data.Length));
				data.SetDWord(num2, array.Length | (text.Length << 24));
				data.SetByteArray(num2 + 20, Encoding.ASCII.GetBytes(text));
				data.SetByteArray((int)(num2 + 20 + text.Length), array);
				offset += 16;
			}
			return data;
		}

		private static byte[] SetupImage(byte[] data, FaceProject face, int faceIndex)
		{
			enumerationHelper.ResetIndex();
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 16 + 4;
			offset = data.GetDWord(offset);
			foreach (FaceWidget item in face.Screen.Widgets.Where(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage)
				{
					return true;
				}
				if (w is FaceWidgetAnalogClock)
				{
					return true;
				}
				if (w is FaceWidgetCircleProgress)
				{
					return true;
				}
				return (w is FaceWidgetCircleProgressPlus) ? true : false;
			}))
			{
				data[offset] = enumerationHelper.GetImageIndex();
				data[offset + 3] = 2;
				data.SetDWord(offset + 8, offsetData);
				if (item is FaceWidgetImage)
				{
					FaceWidgetImage faceWidgetImage = item as FaceWidgetImage;
					faceWidgetImage.ImageId = data.GetDWord(offset);
					string imgFile = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + faceWidgetImage.Bitmap;
					bool isRGBA = item.IsRGBA;
					PackImageData(ref data, face, ref offset, imgFile, isRGBA);
				}
				if (item is FaceWidgetAnalogClock)
				{
					bool flag = false;
					FaceWidgetAnalogClock faceWidgetAnalogClock = item as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.HourHandImage))
					{
						faceWidgetAnalogClock.HourImageId = data.GetDWord(offset);
						string imgFile2 = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + faceWidgetAnalogClock.HourHandImage;
						PackImageData(ref data, face, ref offset, imgFile2, withAlfa: true);
						flag = true;
					}
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.MinuteHandImage))
					{
						if (flag)
						{
							data[offset] = enumerationHelper.GetImageIndex();
							data[offset + 3] = 2;
							data.SetDWord(offset + 8, offsetData);
						}
						flag = true;
						faceWidgetAnalogClock.MinuteImageId = data.GetDWord(offset);
						string imgFile3 = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + faceWidgetAnalogClock.MinuteHandImage;
						PackImageData(ref data, face, ref offset, imgFile3, withAlfa: true);
					}
					if (!string.IsNullOrEmpty(faceWidgetAnalogClock.SecondHandImage))
					{
						if (flag)
						{
							data[offset] = enumerationHelper.GetImageIndex();
							data[offset + 3] = 2;
							data.SetDWord(offset + 8, offsetData);
						}
						faceWidgetAnalogClock.SecondImageId = data.GetDWord(offset);
						string imgFile4 = PathHelper.PathImages + "\\" + faceWidgetAnalogClock.SecondHandImage;
						PackImageData(ref data, face, ref offset, imgFile4, withAlfa: true);
					}
				}
				else if (item is FaceWidgetCircleProgress)
				{
					FaceWidgetCircleProgress faceWidgetCircleProgress = item as FaceWidgetCircleProgress;
					faceWidgetCircleProgress.ImageId = data.GetDWord(offset);
					string imgFile5 = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + faceWidgetCircleProgress.ForegroundImage;
					bool isRGBA2 = item.IsRGBA;
					PackImageData(ref data, face, ref offset, imgFile5, isRGBA2);
				}
				else if (item is FaceWidgetCircleProgressPlus)
				{
					FaceWidgetCircleProgressPlus faceWidgetCircleProgressPlus = item as FaceWidgetCircleProgressPlus;
					faceWidgetCircleProgressPlus.ImageId = data.GetDWord(offset);
					string imgFile6 = (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + faceWidgetCircleProgressPlus.ForegroundImage;
					bool isRGBA3 = item.IsRGBA;
					PackImageData(ref data, face, ref offset, imgFile6, isRGBA3);
				}
			}
			return data;
		}

		private static void PackImageData(ref byte[] data, FaceProject face, ref uint offset, string imgFile, bool withAlfa = false)
		{
			int width;
			int height;
			byte[] bitmapFromPng = MagikHelper.GetBitmapFromPng((WatchType)face.DeviceType, imgFile, 0, out width, out height);
			IDataCompressor compressor = ImageCompressFactory.GetCompressor((WatchType)face.DeviceType, withAlfa);
			byte[] array = null;
			try
			{
				array = (compressor.RequiredCompressWithParams ? compressor.Compress(bitmapFromPng, width, height) : compressor.Compress(bitmapFromPng));
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Failed to compress image[" + Path.GetFileName(imgFile) + "]: " + ex.Message);
			}
			uint num = (uint)(12 + array.Length);
			uint num2 = offsetData;
			data.SetDWord(offset + 12, num);
			offsetData += num.GetDWordAligned();
			offset += 16u;
			data = data.AppendZero((uint)(offsetData - data.Length));
			data.SetByteArray(num2, compressor.GetHeader());
			data.SetWord(num2 + 4, width);
			data.SetWord(num2 + 6, height);
			data.SetDWord(num2 + 8, array.Length);
			data.SetByteArray(num2 + 12, array);
		}

		private static byte[] SetupImageList(byte[] data, FaceProject face, int faceIndex, bool withAlfa = false)
		{
			enumerationHelper.ResetListIndex();
			uint offset = GetOffsetFaceEntryBase(faceIndex) + 8 + 24 + 4;
			offset = data.GetDWord(offset);
			Dictionary<int, uint> dictionary = new Dictionary<int, uint>();
			bool flag = false;
			foreach (FaceWidget item in face.Screen.Widgets.Where(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImageList)
				{
					return true;
				}
				return (w is FaceWidgetDigitalNum) ? true : false;
			}))
			{
				int? num = null;
				int? num2 = null;
				int width = 0;
				int height = 0;
				int num3 = 0;
				byte[] array = null;
				withAlfa = !item.IsRGB;
				if (item is FaceWidgetImageList)
				{
					FaceWidgetImageList faceWidgetImageList = item as FaceWidgetImageList;
					int bitmapListHashCode = faceWidgetImageList.BitmapListHashCode;
					if (dictionary.ContainsKey(bitmapListHashCode))
					{
						flag = true;
						faceWidgetImageList.ImageId = dictionary[bitmapListHashCode];
					}
					else
					{
						data[offset] = enumerationHelper.GetImageListIndex();
						data[offset + 3] = 3;
						data.SetDWord(offset + 8, offsetData);
						flag = false;
						uint dWord = data.GetDWord(offset);
						dictionary.Add(bitmapListHashCode, dWord);
						faceWidgetImageList.ImageId = dWord;
						string[] array2 = faceWidgetImageList.BitmapList.Split('|');
						num3 = array2.Length;
						array2 = array2.Select((string s) => s.Split(':')[1]).ToArray();
						array2 = array2.Select((string s) => (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + s).ToArray();
						array = MagikHelper.GetBitmapFromPngList((WatchType)face.DeviceType, array2, 0, out width, out height);
						if (num.HasValue && num != width)
						{
							throw new Exception("All images in list must have the same width");
						}
						if (num2.HasValue && num2 != height)
						{
							throw new Exception("All images in list must have the same height");
						}
						num = width;
						num2 = height;
					}
				}
				else if (item is FaceWidgetDigitalNum)
				{
					FaceWidgetDigitalNum faceWidgetDigitalNum = item as FaceWidgetDigitalNum;
					int bitmapListHashCode2 = faceWidgetDigitalNum.BitmapListHashCode;
					if (dictionary.ContainsKey(bitmapListHashCode2))
					{
						flag = true;
						faceWidgetDigitalNum.ImageId = dictionary[bitmapListHashCode2];
					}
					else
					{
						data[offset] = enumerationHelper.GetImageListIndex();
						data[offset + 3] = 3;
						data.SetDWord(offset + 8, offsetData);
						flag = false;
						uint dWord2 = data.GetDWord(offset);
						dictionary.Add(bitmapListHashCode2, dWord2);
						faceWidgetDigitalNum.ImageId = dWord2;
						string[] array3 = faceWidgetDigitalNum.BitmapList.Split('|');
						num3 = array3.Length;
						array3 = array3.Select((string s) => (face.IsAOD ? PathHelper.PathImagesAod : PathHelper.PathImages) + "\\" + s).ToArray();
						array = MagikHelper.GetBitmapFromPngList((WatchType)face.DeviceType, array3, 0, out width, out height);
						if (num.HasValue && num != width)
						{
							throw new Exception("All images in list must have the same width");
						}
						if (num2.HasValue && num2 != height)
						{
							throw new Exception("All images in list must have the same height");
						}
						num = width;
						num2 = height;
					}
				}
				if (flag)
				{
					continue;
				}
				IDataCompressor compressor = ImageCompressFactory.GetCompressor((WatchType)face.DeviceType, item.Name, withAlfa, isList: true);
				uint num4 = offsetData;
				uint num5 = (uint)(12 + array.Length);
				byte[] array4 = new byte[num3 * 4];
				byte[] array5 = new byte[0];
				if (compressor.HasCompression)
				{
					int num6 = 0;
					try
					{
						int num7 = width * 4 * height;
						for (int num8 = 0; num8 < num3; num8++)
						{
							num6 = num8;
							byte[] data2 = array.Skip(num8 * num7).Take(num7).ToArray();
							byte[] array6 = (compressor.RequiredCompressWithParams ? compressor.Compress(data2, width, height) : compressor.Compress(data2));
							array4.SetDWord((uint)(4 * num8), (uint)array6.Length);
							array5 = array5.Concat(array6).ToArray();
						}
						num5 = (uint)(12 + array4.Length + array5.Length);
					}
					catch (Exception ex)
					{
						throw new ApplicationException($"Compression failed for imageList[{num6}]: {item.Name}, Check that all images has the same width/height, {ex.Message}");
					}
				}
				data.SetDWord(offset + 12, num5);
				offsetData += num5.GetDWordAligned();
				offset += 16;
				data = data.AppendZero((uint)(offsetData - data.Length));
				data.SetByteArray(num4, compressor.GetHeader());
				data[num4 + 1] = (byte)num3;
				data.SetWord(num4 + 4, width);
				data.SetWord(num4 + 6, height);
				if (compressor.HasCompression)
				{
					data.SetDWord(num4 + 8, array5.Length);
					data.SetByteArray(num4 + 12, array4.Concat(array5).ToArray());
				}
				else
				{
					data.SetDWord(num4 + 8, array.Length);
					data.SetByteArray(num4 + 12, array);
				}
			}
			return data;
		}

		private static int CountElementsSize(byte[] data, FaceProject face, int faceIndex, uint baseOffset)
		{
			int num = face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage && !w.IsRef)
				{
					return 1;
				}
				if (w is FaceWidgetAnalogClock)
				{
					int num23 = 0;
					FaceWidgetAnalogClock obj = w as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(obj.HourHandImage))
					{
						num23++;
					}
					if (!string.IsNullOrEmpty(obj.MinuteHandImage))
					{
						num23++;
					}
					if (!string.IsNullOrEmpty(obj.SecondHandImage))
					{
						num23++;
					}
					return num23;
				}
				return 1;
			});
			int num2 = num * 16;
			int num3 = face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage)
				{
					return 1;
				}
				if (w is FaceWidgetCircleProgress)
				{
					int num23 = 0;
					FaceWidgetCircleProgress obj = w as FaceWidgetCircleProgress;
					if (!string.IsNullOrEmpty(obj.ForegroundImage))
					{
						num23++;
					}
					if (!string.IsNullOrEmpty(obj.BackgroundImage))
					{
						num23++;
					}
					return num23;
				}
				if (w is FaceWidgetCircleProgressPlus)
				{
					int num24 = 0;
					FaceWidgetCircleProgressPlus obj2 = w as FaceWidgetCircleProgressPlus;
					if (!string.IsNullOrEmpty(obj2.ForegroundImage))
					{
						num24++;
					}
					if (!string.IsNullOrEmpty(obj2.BackgroundImage))
					{
						num24++;
					}
					return num24;
				}
				if (w is FaceWidgetAnalogClock)
				{
					int num25 = 0;
					FaceWidgetAnalogClock obj3 = w as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(obj3.HourHandImage))
					{
						num25++;
					}
					if (!string.IsNullOrEmpty(obj3.MinuteHandImage))
					{
						num25++;
					}
					if (!string.IsNullOrEmpty(obj3.SecondHandImage))
					{
						num25++;
					}
					return num25;
				}
				return 0;
			});
			int num4 = num3 * 16;
			uint num5 = baseOffset + (uint)num2;
			num2 += num4;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8, num);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12, baseOffset);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 8, num5);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 16, num3);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 16, num5);
			int num6 = (from s in face.Screen.Widgets.Select(delegate(FaceWidget w)
				{
					if (w is FaceWidgetImageList)
					{
						return (w as FaceWidgetImageList).BitmapListHashCode;
					}
					return (w is FaceWidgetDigitalNum) ? (w as FaceWidgetDigitalNum).BitmapListHashCode : 0;
				})
				where s != 0
				select s).ToArray().Distinct().Count();
			int num7 = num6 * 16;
			uint num8 = num5 + (uint)num4;
			num2 += num7;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 24, num6);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 24, num8);
			int num9 = face.Screen.Widgets.Sum((FaceWidget w) => (w is FaceWidgetImageList && w.IsAnimation) ? 1 : 0);
			int num10 = num9 * 16;
			uint num11 = num8 + (uint)num7;
			num2 += num10;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 32, num9);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 32, num11);
			int num12 = ((face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				FaceWidgetContainer obj = w as FaceWidgetContainer;
				return (obj != null && obj.IsApp) ? 1 : 0;
			}) > 0) ? AppHelper.GetAppCount() : 0);
			int num13 = num12 * 16;
			uint num14 = num11 + (uint)num10;
			num2 += num13;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 40, num12);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 40, num14);
			int num15 = face.Screen.Widgets.Sum(delegate(FaceWidget w)
			{
				if (w is FaceWidgetImage)
				{
					return 0;
				}
				if (w is FaceWidgetImageList)
				{
					return (!w.IsAnimation) ? 1 : 0;
				}
				if (w is FaceWidgetAnalogClock)
				{
					int num23 = 0;
					FaceWidgetAnalogClock obj = w as FaceWidgetAnalogClock;
					if (!string.IsNullOrEmpty(obj.HourHandImage))
					{
						num23++;
					}
					if (!string.IsNullOrEmpty(obj.MinuteHandImage))
					{
						num23++;
					}
					if (!string.IsNullOrEmpty(obj.SecondHandImage))
					{
						num23++;
					}
					return num23;
				}
				FaceWidgetContainer obj2 = w as FaceWidgetContainer;
				return (obj2 == null || !obj2.IsApp) ? 1 : 0;
			});
			int num16 = num15 * 16;
			uint num17 = num14 + (uint)num13;
			num2 += num16;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 48, num17);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 56, num15);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 56, num17);
			int num18 = face.Screen.Widgets.Sum((FaceWidget w) => 0);
			int num19 = num18 * 16;
			uint num20 = num17 + (uint)num16;
			num2 += num19;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 64, num18);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 64, num20);
			int num21 = face.Screen.Widgets.Sum((FaceWidget w) => (w is FaceWidgetImage) ? ((w.IsButton || w.IsApp) ? 1 : 0) : 0);
			int num22 = num21 * 16;
			uint val = num20 + (uint)num19;
			num2 += num22;
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 8 + 72, num21);
			data.SetDWord(GetOffsetFaceEntryBase(faceIndex) + 12 + 72, val);
			return num2;
		}

		private static void SetupHeader(byte[] data, FaceProject face, bool hasAOD)
		{
			data[28] = (byte)faceCount;
			bool flag = faceCount - (hasAod ? 1 : 0) > 1;
			if (face.DeviceType == 7)
			{
				data.SetDWord(16u, 2048);
				data[30] = (byte)(hasAOD ? 6u : 0u);
			}
			else if (new WatchType[2]
			{
				WatchType.RedmiWatch5Active,
				WatchType.RedmiWatch5Lite
			}.Contains((WatchType)face.DeviceType))
			{
				data.SetDWord(16u, 2048);
				data[6] = 1;
				data[22] = 1;
				data[30] = (byte)((hasAOD ? 4 : 0) | (flag ? 2 : 0));
			}
			else if (new WatchType[10]
			{
				WatchType.Gen3,
				WatchType.MiBand8,
				WatchType.MiBand8Pro,
				WatchType.MiBand9Pro,
				WatchType.RedmiWatch4,
				WatchType.RedmiWatch5,
				WatchType.MiBand9,
				WatchType.MiBand10,
				WatchType.MiWatchS3,
				WatchType.MiWatchS4
			}.Contains((WatchType)face.DeviceType))
			{
				data.SetDWord(16u, 2048);
				data[30] = (byte)((hasAOD ? 4 : 0) | (flag ? 2 : 0));
			}
			else if (face.DeviceType == 6)
			{
				data.SetDWord(16u, 2048);
			}
			data[28] = (byte)faceCount;
			data.SetByteArray(104, Encoding.UTF8.GetBytes(face.Screen.Title));
		}

		private static uint GetOffsetFaceEntryBase(int index)
		{
			return (uint)(168 + index * 88);
		}

		private static uint GetOffsetFieldBase()
		{
			return (uint)(168 + faceCount * 88);
		}
	}
}
