using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MiWatchCompiler.Helpers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.CheckSum;
using XiaomiWatch.Common.Compress;
using XiaomiWatch.Common.FaceFile;

namespace MiWatchCompiler.Packers
{
	internal class RW3ActivePacker
	{
		private const WatchType CurrentWatchType = WatchType.RedmiWatch3Active;

		private const int TotalWidgetCount = 68;

		private const int WidgetHeaderSize = 112;

		private static int offsetImageList = 7648;

		private static byte[] header = new byte[100]
		{
			0, 0, 6, 7, 0, 2, 0, 1, 0, 1,
			2, 1, 9, 0, 0, 0, 0, 2, 0, 1,
			1, 66, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 100, 0, 0, 0, 36, 237, 0, 0, 136,
			237, 0, 0, 254, 120, 0, 0, 0, 0, 0
		};

		private static byte[] widgetsPropHeader;

		private static byte[] previewHeader;

		private static byte[] backHeader;

		private static Dictionary<int, (byte offset, byte mask, bool hasAlign, Func<int, FaceWidget, (byte[] header, byte[] data)> handler)> widgetHandlers;

		internal static WatchType Exec(string srcFile, string dstFile)
		{
			PathHelper.SetRootPath(Path.GetDirectoryName(srcFile));
			if (!Directory.Exists(PathHelper.PathImages))
			{
				throw new Exception("images path is not found");
			}
			FaceProject faceProject = FaceProject.Deserialize(Namespace.Prepare(File.ReadAllText(srcFile)));
			byte[] data = header;
			SetupHeader(data, faceProject);
			ProcessWidgets(ref data, faceProject);
			ProcessPreviewBackground(ref data, faceProject);
			uint val = new Crc32a().Get(data);
			data = data.AppendZero(4);
			data.SetDWord(data.Length - 4, val);
			File.WriteAllBytes(dstFile, data);
			return (WatchType)faceProject.DeviceType;
		}

		private static (byte[], byte[]) BuildImage(int index, FaceWidget widget)
		{
			byte[] array = new byte[112];
			int num = 0;
			if (!(widget is FaceWidgetImage faceWidgetImage))
			{
				throw new Exception($"Failed to build image {index:X2}");
			}
			string text = PathHelper.PathImages + "\\" + faceWidgetImage.Bitmap;
			if (!File.Exists(text))
			{
				throw new Exception("background file is missing");
			}
			array[num++] = (byte)index;
			array.SetWord(num, faceWidgetImage.X);
			array.SetWord(num + 2, faceWidgetImage.Y);
			array.SetWord(num + 4, faceWidgetImage.Width);
			array.SetWord(num + 6, faceWidgetImage.Height);
			array[num + 8] = 1;
			array[num + 9] = 0;
			array.SetDWord(num + 10, offsetImageList);
			int width;
			int height;
			byte[] bitmapFromPng = MagikHelper.GetBitmapFromPng(WatchType.RedmiWatch3Active, text, 0, out width, out height);
			byte[] array2 = ImageCompressFactory.GetCompressor(WatchType.RedmiWatch3Active, withAlfa: false).Compress(bitmapFromPng, width, height);
			array.SetDWord(num + 10 + 4, array2.Length);
			offsetImageList += array2.Length.GetDWordAligned();
			return (array, array2);
		}

		private static (byte[], byte[]) BuildImageList(int index, FaceWidget widget)
		{
			byte[] array = new byte[112];
			int num = 0;
			byte[] array2 = null;
			array[0] = (byte)index;
			FaceWidgetDigitalNum faceWidgetDigitalNum = widget as FaceWidgetDigitalNum;
			FaceWidgetImageList faceWidgetImageList = widget as FaceWidgetImageList;
			FaceWidgetImage faceWidgetImage = widget as FaceWidgetImage;
			if (faceWidgetDigitalNum == null && faceWidgetImageList == null && faceWidgetImage == null)
			{
				throw new Exception($"Failed to build image {index:X2}");
			}
			string[] array3 = new string[0];
			if (faceWidgetDigitalNum == null)
			{
				array3 = ((faceWidgetImageList == null) ? new string[1] { faceWidgetImage.Bitmap } : faceWidgetImageList.BitmapList.Split('|'));
			}
			else
			{
				array3 = faceWidgetDigitalNum.BitmapList.Split('|');
				string[] array4 = new string[array3.Length - 1];
				Array.Copy(array3, array4, array3.Length - 1);
				array3 = array4;
			}
			if (array3.Length > 255)
			{
				throw new Exception("image list can't be greather than 255");
			}
			byte b = (byte)array3.Length;
			array[num++] = (byte)index;
			array.SetWord(num, widget.X);
			array.SetWord(num + 2, widget.Y);
			array[num + 8] = b;
			array.SetDWord(num + 10, offsetImageList);
			if (faceWidgetImageList != null)
			{
				array3 = array3.Select((string s) => s.Split(':')[1]).ToArray();
			}
			array3 = array3.Select((string s) => PathHelper.PathImages + "\\" + s).ToArray();
			int width;
			int height;
			byte[] bmpData = MagikHelper.GetBitmapFromPngList(WatchType.RedmiWatch3Active, array3, 0, out width, out height);
			array.SetWord(num + 4, width);
			array.SetWord(num + 6, height);
			uint[] array5 = (from i in Enumerable.Range(0, bmpData.Length / 4)
				select BitConverter.ToUInt32(bmpData, i * 4) into c
				select c & 0xFFFFFF).Distinct().Except(new uint[1]).ToArray();
			uint val = 0u;
			byte b2 = 0;
			if (array5.Count() == 1)
			{
				val = array5[0];
				b2 = 3;
			}
			if (widget.HasColor)
			{
				val = widget.GetColor();
				b2 = 3;
				if (array5.Count() > 1)
				{
					MagikHelper.ConvertImagesToMonoColor(array3);
					bmpData = MagikHelper.GetBitmapFromPngList(WatchType.RedmiWatch3Active, array3, 0, out width, out height);
				}
			}
			array[num + 9] = b2;
			IDataCompressor compressor = ImageCompressFactory.GetCompressor(WatchType.RedmiWatch3Active, withAlfa: false);
			if (b2 == 3)
			{
				array2 = compressor.Compress(bmpData, width, height, (uint)((offsetImageList << 8) | b2));
				RW3ActiveImageInfo imageInfo = RW3ActiveImageHelper.GetImageInfo(bmpData, width, height, b2 == 3);
				array.SetWord(num + 4, width);
				array.SetWord(num + 6, height);
				array.SetDWord(num + 10 + 4, imageInfo.ImageHeaderItemSize);
				if (b2 == 3)
				{
					array.SetDWord(num + 105, val, 1u);
				}
				offsetImageList += array2.Length.GetDWordAligned();
			}
			else
			{
				int chunkSize = bmpData.Length / b;
				int num2 = 0;
				array2 = Array.Empty<byte>();
				foreach (byte[] item in from i in Enumerable.Range(0, (bmpData.Length + chunkSize - 1) / chunkSize)
					select bmpData.Skip(i * chunkSize).Take(chunkSize).ToArray())
				{
					byte[] array6 = compressor.Compress(item, width, height);
					array.SetDWord(num + 10 + 8 * num2, offsetImageList);
					array.SetDWord(num + 10 + 8 * num2 + 4, array6.Length);
					num2++;
					offsetImageList += array6.Length.GetDWordAligned();
					int offset = array2.Length;
					array2 = array2.AppendZero(array6.Length.GetDWordAligned());
					array2.SetByteArray(offset, array6);
				}
			}
			return (array, array2);
		}

		private static void ProcessWidgets(ref byte[] data, FaceProject face)
		{
			byte[] array = widgetsPropHeader;
			byte[] array2 = new byte[7616];
			for (int i = 0; i < 68; i++)
			{
				array2[i * 112] = (byte)(i + 3);
			}
			int num = array.Length;
			Array.Resize(ref array, (num + array2.Length).GetDWordAligned());
			array.SetByteArray(num, array2);
			offsetImageList = array.Length;
			foreach (FaceWidget item in face.Screen.Widgets.Skip(1))
			{
				string rW3AIndex = item.RW3AIndex;
				if (string.IsNullOrWhiteSpace(rW3AIndex))
				{
					continue;
				}
				int num2 = int.Parse(rW3AIndex, NumberStyles.HexNumber);
				if (!widgetHandlers.ContainsKey(num2))
				{
					continue;
				}
				array[widgetHandlers[num2].offset] |= widgetHandlers[num2].mask;
				if (widgetHandlers[num2].hasAlign && item is FaceWidgetDigitalNum faceWidgetDigitalNum)
				{
					byte b = (byte)((faceWidgetDigitalNum.Alignment == 0) ? 1u : ((faceWidgetDigitalNum.Alignment == 1) ? 2u : 0u));
					if (faceWidgetDigitalNum.Blanking == 0)
					{
						b = 0;
					}
					array[widgetHandlers[num2].offset - 1] |= b;
				}
				(byte[], byte[]) tuple = widgetHandlers[num2].handler(num2, item);
				array.SetByteArray(widgetsPropHeader.Length + (num2 - 3) * 112, tuple.Item1);
				num = array.Length;
				Array.Resize(ref array, num + tuple.Item2.Length.GetDWordAligned());
				array.SetByteArray(num, tuple.Item2);
			}
			data.SetDWord(85, array.Length);
			num = data.Length;
			Array.Resize(ref data, num + array.Length);
			data.SetByteArray(num, array);
		}

		private static void ProcessPreviewBackground(ref byte[] data, FaceProject face)
		{
			string text = PathHelper.PathImages + "\\" + face.Screen.Bitmap;
			if (!File.Exists(text))
			{
				throw new Exception("preview file is missing");
			}
			byte[] bitmapFromPng = MagikHelper.GetBitmapFromPng(WatchType.RedmiWatch3Active, text, 0, out var width, out var height);
			byte[] array = previewHeader;
			if (WatchDetector.GetWatchType(width, height) != WatchType.RedmiWatch3Active)
			{
				(int, int) watchPreviewSize = WatchDetector.GetWatchPreviewSize(WatchType.RedmiWatch3Active);
				if (width != watchPreviewSize.Item1 || height != watchPreviewSize.Item2)
				{
					throw new Exception($"Preview has wrong size: {width}x{height}, expected: {watchPreviewSize.Item1}x{watchPreviewSize.Item2}");
				}
			}
			MagikHelper.ApplyPreviewMask(WatchType.RedmiWatch3Active, text);
			bitmapFromPng = MagikHelper.GetBitmapFromPng(WatchType.RedmiWatch3Active, text, 0, out width, out height);
			IDataCompressor compressor = ImageCompressFactory.GetCompressor(WatchType.RedmiWatch3Active, withAlfa: false);
			byte[] array2 = compressor.Compress(bitmapFromPng, width, height);
			if (!(face.Screen.Widgets[0] is FaceWidgetImage faceWidgetImage) || faceWidgetImage.Shape != 30)
			{
				throw new Exception("First item must be a background image");
			}
			text = PathHelper.PathImages + "\\" + faceWidgetImage.Bitmap;
			if (!File.Exists(text))
			{
				throw new Exception("background file is missing");
			}
			bitmapFromPng = MagikHelper.GetBitmapFromPng(WatchType.RedmiWatch3Active, text, 0, out width, out height);
			byte[] array3 = compressor.Compress(bitmapFromPng, width, height);
			int offset = array.Length;
			array = array.AppendZero(backHeader.Length);
			array.SetByteArray(offset, backHeader);
			array.SetDWord(15, array2.Length);
			int dWordAligned = (array2.Length + array.Length).GetDWordAligned();
			array.SetDWord(123, dWordAligned);
			array.SetDWord(127, array3.Length);
			offset = array.Length;
			array = array.AppendZero(array2.Length.GetDWordAligned());
			array.SetByteArray(offset, array2);
			offset = array.Length;
			array = array.AppendZero(array3.Length.GetDWordAligned());
			array.SetByteArray(offset, array3);
			data.SetDWord(89, data.Length);
			data.SetDWord(93, array.Length);
			offset = data.Length;
			data = data.AppendZero(array.Length);
			data.SetByteArray(offset, array);
		}

		private static void SetupHeader(byte[] data, FaceProject face)
		{
			if (face.Screen.Title.Length > 58)
			{
				throw new Exception("Title too long");
			}
			int num = 20;
			data[num] = (byte)face.Screen.Title.Length;
			data.SetByteArray(num + 1, Encoding.UTF8.GetBytes(face.Screen.Title));
		}

		static RW3ActivePacker()
		{
			byte[] array = new byte[30];
			array[0] = 3;
			widgetsPropHeader = array;
			previewHeader = new byte[112]
			{
				1, 42, 0, 38, 0, 156, 0, 182, 0, 1,
				0, 224, 0, 0, 0, 107, 200, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0
			};
			backHeader = new byte[112]
			{
				2, 0, 0, 0, 0, 240, 0, 24, 1, 1,
				0, 76, 201, 0, 0, 156, 166, 1, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0
			};
			widgetHandlers = new Dictionary<int, (byte, byte, bool, Func<int, FaceWidget, (byte[], byte[])>)>
			{
				{
					4,
					(4, 2, false, BuildImageList)
				},
				{
					5,
					(4, 16, false, BuildImageList)
				},
				{
					6,
					(4, 8, false, BuildImageList)
				},
				{
					7,
					(4, 4, false, BuildImageList)
				},
				{
					9,
					(4, 64, true, BuildImageList)
				},
				{
					10,
					(4, 128, false, BuildImageList)
				},
				{
					13,
					(8, 8, true, BuildImageList)
				},
				{
					14,
					(8, 4, false, BuildImageList)
				},
				{
					15,
					(8, 2, true, BuildImageList)
				},
				{
					16,
					(8, 64, false, BuildImageList)
				},
				{
					17,
					(8, 32, false, BuildImageList)
				},
				{
					18,
					(8, 16, false, BuildImageList)
				},
				{
					19,
					(8, 128, false, BuildImageList)
				},
				{
					20,
					(9, 2, false, BuildImageList)
				},
				{
					21,
					(9, 1, false, BuildImageList)
				},
				{
					24,
					(12, 2, false, BuildImageList)
				},
				{
					25,
					(12, 64, false, BuildImageList)
				},
				{
					26,
					(12, 32, false, BuildImageList)
				},
				{
					27,
					(12, 16, false, BuildImageList)
				},
				{
					28,
					(12, 32, true, BuildImageList)
				},
				{
					35,
					(16, 2, false, BuildImageList)
				},
				{
					36,
					(16, 0, false, BuildImageList)
				},
				{
					37,
					(16, 0, false, BuildImageList)
				},
				{
					38,
					(16, 0, false, BuildImageList)
				},
				{
					39,
					(16, 32, true, BuildImageList)
				},
				{
					49,
					(20, 32, true, BuildImageList)
				},
				{
					50,
					(20, 16, false, BuildImageList)
				},
				{
					51,
					(20, 0, false, BuildImageList)
				},
				{
					52,
					(20, 0, false, BuildImageList)
				},
				{
					53,
					(20, 0, false, BuildImageList)
				},
				{
					56,
					(22, 16, true, BuildImageList)
				},
				{
					57,
					(22, 8, true, BuildImageList)
				},
				{
					58,
					(22, 4, false, BuildImageList)
				},
				{
					59,
					(22, 2, true, BuildImageList)
				},
				{
					60,
					(22, 1, true, BuildImageList)
				},
				{
					61,
					(22, 32, false, BuildImageList)
				},
				{
					62,
					(22, 128, true, BuildImageList)
				},
				{
					63,
					(22, 64, true, BuildImageList)
				}
			};
		}
	}
}
