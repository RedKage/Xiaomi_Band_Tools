using System;
using System.Linq;
using XiaomiWatch.Common.FaceFile;

namespace XiaomiWatch.Common
{
	public class AnimationHelper
	{
		private enum AnimationTypeEnum
		{
			Loop,
			FadeIn,
			FadeOut
		}

		public static Tuple<int, int> GetAnimationData(FaceWidget widget)
		{
			if (widget == null)
			{
				throw new ApplicationException("Failed to pack animation");
			}
			if (string.IsNullOrWhiteSpace(widget.Name))
			{
				throw new ApplicationException("Animation is missing a name");
			}
			if (!widget.Name.Contains('[') || !widget.Name.Contains(']'))
			{
				throw new ApplicationException("Failed to pack button action: missing or wrong app name");
			}
			string[] array = widget.Name.Split('[', ']')[1].Split('@');
			if (array == null || array.Length != 2)
			{
				throw new ApplicationException("Failed to pack animation: wrong animation params");
			}
			Tuple<int, int> tuple = null;
			try
			{
				int item = int.Parse(array[0]);
				return new Tuple<int, int>(int.Parse(array[1]), item);
			}
			catch
			{
				throw new ApplicationException("Animation count/delay is not integer value: " + array[0] + "/" + array[1]);
			}
		}
	}
}
