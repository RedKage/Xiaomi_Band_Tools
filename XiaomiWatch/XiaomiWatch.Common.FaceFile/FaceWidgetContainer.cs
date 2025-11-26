namespace XiaomiWatch.Common.FaceFile
{
	public class FaceWidgetContainer : FaceWidget
	{
		public FaceWidgetContainer()
		{
			base.Shape = 34;
		}

		public new static FaceWidgetContainer Get(byte[] bin)
		{
			int dWord = (int)bin.GetDWord(0, 0u);
			int dWord2 = (int)bin.GetDWord(4);
			int word = bin.GetWord(8);
			int word2 = bin.GetWord(12);
			int word3 = bin.GetWord(16);
			int word4 = bin.GetWord(20);
			int alpha = bin.GetByte(28);
			uint dWord3 = bin.GetDWord(44);
			bin.GetByteArray(48u, dWord3);
			return new FaceWidgetContainer
			{
				Shape = dWord2,
				Name = $"container_{dWord}",
				X = word,
				Y = word2,
				Width = word3,
				Height = word4,
				Alpha = alpha
			};
		}
	}
}
