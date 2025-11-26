using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using XiaomiWatch.Common;

namespace MiWatchCompiler
{
	internal class Program
	{
		private const string adbUtilExeName = "adb.exe";

		private const string library = "MiWatchCompiler.Lib.Magick.NET-Q16-AnyCPU.dll";

		public const string libraryCommon = "MiWatchCompiler.Lib.XiaomiWatch.Common.dll";

		private const string info = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><FaceInfo ID=\"1340923058\" Type=\"1\" Size=\"1875887\" DeviceType=\"1\" Title=\"{0}\" DeviceVersion=\"7.1.0\" DeciceTypeName=\"{1}\" />";

		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			MainBody(args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void MainBody(string[] args)
		{
			if (args.Length < 5)
			{
				Console.WriteLine("Error: wrong argument size");
				Console.WriteLine("Usage: Compiler -b {full path to .fprj} output {name of file.face} {id}");
				return;
			}
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			_ = args[0];
			string text = args[1];
			string path = args[2];
			string text2 = args[3];
			int.Parse(args[4]);
			string directoryName = Path.GetDirectoryName(text);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text2);
			string directoryName2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			bool flag = true;
			Console.WriteLine("Loading " + text);
			string text3 = null;
			string text4 = "Mi Band 7 Pro";
			try
			{
				text3 = Path.Combine(path, text2);
				WatchType watchType = Compiler.Exec(text, text3);
				text4 = Enum.GetName(typeof(WatchType), watchType);
				Console.WriteLine("Watch: " + text4);
				Console.WriteLine("Watchface: " + text2 + " is ready");
				Console.WriteLine("------------------------------- No Errors -------------------------------");
				flag = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				Console.WriteLine("============ Errors: 1 ============");
			}
			File.WriteAllText(directoryName + "\\output\\" + fileNameWithoutExtension + ".info", $"<?xml version=\"1.0\" encoding=\"utf-8\" ?><FaceInfo ID=\"1340923058\" Type=\"1\" Size=\"1875887\" DeviceType=\"1\" Title=\"{fileNameWithoutExtension}\" DeviceVersion=\"7.1.0\" DeciceTypeName=\"{text4}\" />");
			if (!flag)
			{
				if (File.Exists(Path.Combine(directoryName2, "adb.exe")))
				{
					Console.WriteLine("Uploading watchface via adb into emulator..");
					AdbBridge.Push(text3);
					Console.WriteLine("Upload is complete");
				}
				else
				{
					Console.WriteLine("adb util is missing, skipping to upload into emulator.");
				}
				Console.WriteLine("-------------------------------------------------------------------------------");
			}
		}

		private static Assembly AssemblyResolver(object sender, ResolveEventArgs args)
		{
			Assembly assembly = null;
			assembly = LoadAssembly(args, "Magick", "MiWatchCompiler.Lib.Magick.NET-Q16-AnyCPU.dll");
			if (assembly != null)
			{
				return assembly;
			}
			assembly = LoadAssembly(args, "XiaomiWatch", "MiWatchCompiler.Lib.XiaomiWatch.Common.dll");
			_ = assembly != null;
			return assembly;
		}

		private static Assembly LoadAssembly(ResolveEventArgs args, string name, string libName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if (args.Name.Contains(name))
			{
				using (Stream stream = executingAssembly.GetManifestResourceStream(libName))
				{
					using (BinaryReader binaryReader = new BinaryReader(stream))
					{
						byte[] array = new byte[stream.Length];
						binaryReader.Read(array, 0, array.Length);
						return Assembly.Load(array);
					}
				}
			}
			return null;
		}
	}
}
