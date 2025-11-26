using System.IO;
using MiWatchCompiler.Packers;
using XiaomiWatch.Common;
using XiaomiWatch.Common.FaceFile;

namespace MiWatchCompiler
{
	internal class Compiler
	{
		internal static WatchType Exec(string srcFile, string dstFile)
		{
			if (FaceProject.Deserialize(Namespace.Prepare(File.ReadAllText(srcFile))).DeviceType == 12)
			{
				return RW3ActivePacker.Exec(srcFile, dstFile);
			}
			return DefaultPacker.Exec(srcFile, dstFile);
		}
	}
}
