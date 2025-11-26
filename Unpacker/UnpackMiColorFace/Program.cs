using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnpackMiColorFace.Helpers;

namespace UnpackMiColorFace
{
	internal class Program
	{
		public const string library = "UnpackMiColorFace.Lib.Magick.NET-Q16-AnyCPU.dll";

		public const string libraryCommon = "UnpackMiColorFace.Lib.XiaomiWatch.Common.dll";

		[STAThread]
		private static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver;
			MainBody(args);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void MainBody(string[] args)
		{
			if (args.Count() == 0)
			{
				Console.WriteLine("usage: UnpackMiColorFace example.bin");
				return;
			}
			string text = args[0];
			if (!File.Exists(text))
			{
				Console.WriteLine("File " + text + " is not found.");
				return;
			}
			try
			{
				Unpacker.Exec(text);
				if (LogHelper.GotError)
				{
					Console.ReadKey();
				}
			}
			catch (MissingFieldException)
			{
				Console.WriteLine("Seems wrong file passed,\r\nPlease check a source file is correct Watchface");
				Console.ReadKey();
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				Console.WriteLine("Got unexcepted error,\r\nPlease check a source file is correct Watchface");
				Console.ReadKey();
			}
		}

		private static Assembly AssemblyResolver(object sender, ResolveEventArgs args)
		{
			Assembly assembly = null;
			assembly = LoadAssembly(args, "Magick", "UnpackMiColorFace.Lib.Magick.NET-Q16-AnyCPU.dll");
			if (assembly != null)
			{
				return assembly;
			}
			assembly = LoadAssembly(args, "XiaomiWatch", "UnpackMiColorFace.Lib.XiaomiWatch.Common.dll");
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
