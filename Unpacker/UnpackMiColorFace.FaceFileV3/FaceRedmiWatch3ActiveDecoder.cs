using System;
using System.Collections.Generic;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace UnpackMiColorFace.FaceFileV3
{
	public class FaceRedmiWatch3ActiveDecoder : IFaceFileV3Decoder
	{
		public FaceWidget GetWidget(int sectionId, List<string> imageNameList, int posX, int posY, int width, int height, uint color = 0u)
		{
			int percentLevel = -10;
			int numIndex = 0;
			string arg = "";
			if (color != 0)
			{
				arg = $"_color[{color:X6}]";
			}
			switch (sectionId)
			{
			case 4:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_batt{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({percentLevel += 10}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 50,
					DataSrcIndex = "0841"
				};
			case 5:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_battCN1{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 6:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_battEN{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 7:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_battCN2{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 9:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_batt{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 3,
					Blanking = 1,
					DataSrcValue = "0841"
				};
			case 10:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_delim{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 13:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_month{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 2,
					DataSrcValue = "1012"
				};
			case 14:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_delim{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 15:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_day{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 2,
					DataSrcValue = "1812"
				};
			case 16:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_weekCN1",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0,
					DefaultIndex = 1,
					DataSrcIndex = "2012"
				};
			case 17:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_weekEN",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 1,
					DataSrcIndex = "2012"
				};
			case 18:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_weekCN2",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0,
					DefaultIndex = 1,
					DataSrcIndex = "2012"
				};
			case 19:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_AMPM-CN1{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0,
					DefaultIndex = 1,
					DataSrcIndex = "0813"
				};
			case 20:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_AMPM-EN{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 1,
					DataSrcIndex = "0813"
				};
			case 21:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_AMPM-CN2{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({++numIndex}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0,
					DefaultIndex = 1,
					DataSrcIndex = "0813"
				};
			case 24:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_stepsPercent{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({percentLevel += 10}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 50,
					DataSrcIndex = "1021"
				};
			case 25:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_stepsCN1{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 26:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_stepsEN{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 27:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_stepsCN2{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 28:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_steps{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 5,
					Blanking = 1,
					DataSrcValue = "0821"
				};
			case 35:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_calPercent{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({percentLevel += 10}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 50,
					DataSrcIndex = "1023"
				};
			case 36:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_calCN1{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 37:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_calEN{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 38:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_calCN2{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 39:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_calories{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 4,
					Blanking = 1,
					DataSrcValue = "0823"
				};
			case 49:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_hrm{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 3,
					Blanking = 1,
					DataSrcValue = "0822"
				};
			case 50:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_noHrm{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 51:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_hrmCN1{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 52:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_hrmEN{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 53:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_img_hrmCN2{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 0
				};
			case 56:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_hourHigh{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "1000911"
				};
			case 57:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_hourLow{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "0911"
				};
			case 58:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_delim{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 59:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_minHigh{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "1211"
				};
			case 60:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_minLow{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "1111"
				};
			case 61:
				return new FaceWidgetImage
				{
					Name = $"{sectionId:X2}_delim{arg}",
					Bitmap = imageNameList[0],
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255
				};
			case 62:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_secHigh{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "1001911"
				};
			case 63:
				return new FaceWidgetDigitalNum
				{
					Name = $"{sectionId:X2}_num_secLow{arg}",
					BitmapList = string.Join("|", imageNameList.Append(imageNameList[0])),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					Digits = 1,
					DataSrcValue = "1911"
				};
			case 66:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_hour{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({numIndex++}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 10,
					DataSrcIndex = "0811"
				};
			case 67:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_min{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({numIndex++}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DataSrcIndex = "1011"
				};
			case 68:
				return new FaceWidgetImageList
				{
					Name = $"{sectionId:X2}_imgList_second{arg}",
					BitmapList = imageNameList.Aggregate("", (string final, string next) => final + ((final.Length > 0) ? "|" : "") + $"({numIndex++}):{next}"),
					X = posX,
					Y = posY,
					Width = width,
					Height = height,
					Alpha = 255,
					DefaultIndex = 5,
					DataSrcIndex = "1811"
				};
			default:
				throw new NotSupportedException($"Can't detect widget for sectionId: {sectionId:X2}");
			}
		}
	}
}
