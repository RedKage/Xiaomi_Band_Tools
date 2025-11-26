using System;
using System.IO;
using System.Linq;
using ImageMagick;
using XiaomiWatch.Common;

namespace MiWatchCompiler.Helpers
{
	internal class MagikHelper
	{
		private const string ColorMappingBGRA = "BGRA";

		private const string ColorMappingRGBA = "RGBA";

		internal static byte[] GetBitmapFromPng(WatchType watchType, string imgFile, int fmt, out int width, out int height)
		{
			using (MagickImage magickImage = new MagickImage())
			{
				magickImage.Read(imgFile);
				magickImage.HasAlpha = true;
				magickImage.Format = MagickFormat.Bmp;
				string mapping = "BGRA";
				if (watchType == WatchType.RedmiWatch2Lite)
				{
					mapping = "RGBA";
				}
				byte[] result = magickImage.GetPixels().ToByteArray(mapping);
				width = magickImage.Width;
				height = magickImage.Height;
				return result;
			}
		}

		private static byte[] GetBitmapFromPngListInner(WatchType watchType, string imgFile, int fmt, out int width, out int height)
		{
			using (MagickImage magickImage = new MagickImage())
			{
				magickImage.Read(imgFile);
				string mapping = "BGRA";
				if (watchType == WatchType.RedmiWatch2Lite)
				{
					mapping = "RGBA";
				}
				byte[] array = magickImage.GetPixels().ToByteArray(mapping);
				if (watchType == WatchType.RedmiWatch2 || watchType == WatchType.RedmiWatch2Lite || watchType == WatchType.RedmiBandPro)
				{
					array = RepackRgbaToRgb565(array);
				}
				width = magickImage.Width;
				height = magickImage.Height;
				return array;
			}
		}

		private static byte[] RepackRgbaToRgb565(byte[] pack)
		{
			if (pack == null)
			{
				return pack;
			}
			if (pack.Length == 0)
			{
				return pack;
			}
			byte[] array = new byte[pack.Length / 2];
			byte[] array2 = new byte[pack.Length / 4];
			uint num = 0u;
			uint num2 = 0u;
			for (int i = 0; i < pack.Length; i += 4)
			{
				byte num3 = pack[i];
				byte b = pack[i + 1];
				byte b2 = pack[i + 2];
				byte b3 = pack[i + 3];
				ushort val = (ushort)((ushort)((double)(int)num3 * 31.0 / 255.0) | ((ushort)((double)(int)b * 63.0 / 255.0) << 5) | ((ushort)((double)(int)b2 * 31.0 / 255.0) << 11));
				array.SetWord(num, val);
				array2[num2] = b3;
				num += 2;
				num2++;
			}
			return array.Concat(array2).ToArray();
		}

		internal static byte[] GetBitmapFromPngList(WatchType watchType, string[] imageList, int fmt, out int width, out int height)
		{
			byte[] array = new byte[0];
			int? num = null;
			int? num2 = null;
			width = 0;
			height = 0;
			foreach (string text in imageList)
			{
				byte[] bitmapFromPngListInner = GetBitmapFromPngListInner(watchType, text, fmt, out width, out height);
				if (num.HasValue && num != width)
				{
					throw new Exception("Error in imageList: " + Path.GetFileName(text) + " images in list must have the same width");
				}
				if (num2.HasValue && num2 != height)
				{
					throw new Exception("Error in imageList: " + Path.GetFileName(text) + " images in list must have the same height");
				}
				num = width;
				num2 = height;
				array = array.Concat(bitmapFromPngListInner).ToArray();
			}
			return array;
		}

		internal static int GetBitmapWidth(string imgFile)
		{
			using (MagickImage magickImage = new MagickImage())
			{
				magickImage.Read(imgFile);
				return magickImage.Width;
			}
		}

		internal static void ConvertImagesToMonoColor(string[] imageList)
		{
			for (int i = 0; i < imageList.Length; i++)
			{
				ConvertImageToMonoColor(imageList[i]);
			}
		}

		private static void ConvertImageToMonoColor(string filename)
		{
			using (MagickImage magickImage = new MagickImage(filename))
			{
				using (MagickImage magickImage2 = new MagickImage((IMagickImage)magickImage))
				{
					magickImage2.HasAlpha = false;
					magickImage2.Grayscale(PixelIntensityMethod.Average);
					IPixelCollection pixels = magickImage2.GetPixels();
					IPixelCollection pixels2 = magickImage.GetPixels();
					for (int i = 0; i < magickImage.Height; i++)
					{
						for (int j = 0; j < magickImage.Width; j++)
						{
							Pixel pixel = pixels.GetPixel(j, i);
							double num = (double)(int)pixels2.GetPixel(j, i).GetChannel(3) / 65535.0;
							ushort num2 = (ushort)((double)(int)pixel[0] * num);
							int x = j;
							int y = i;
							ushort[] obj = new ushort[4] { 65535, 65535, 65535, 0 };
							obj[3] = num2;
							pixels2.SetPixel(x, y, obj);
						}
					}
					magickImage.Write(filename);
				}
			}
		}

		internal static void ApplyPreviewMask(WatchType watchType, string filename)
		{
			if (watchType != WatchType.RedmiWatch3Active)
			{
				return;
			}
			using (MagickImage magickImage = new MagickImage(filename))
			{
				using (MagickImage magickImage2 = new MagickImage(PreviewMask.RW3A))
				{
					magickImage2.Grayscale(PixelIntensityMethod.Average);
					IPixelCollection pixels = magickImage.GetPixels();
					IPixelCollection pixels2 = magickImage2.GetPixels();
					for (int i = 0; i < magickImage.Height; i++)
					{
						for (int j = 0; j < magickImage.Width; j++)
						{
							ushort channel = pixels2.GetPixel(j, i).GetChannel(1);
							Pixel pixel = pixels.GetPixel(j, i);
							if (pixel.GetChannel(3) == 0)
							{
								return;
							}
							pixels.SetPixel(j, i, new ushort[4]
							{
								pixel[0],
								pixel[1],
								pixel[2],
								channel
							});
						}
					}
					magickImage.Write(filename);
				}
			}
		}
	}
}
