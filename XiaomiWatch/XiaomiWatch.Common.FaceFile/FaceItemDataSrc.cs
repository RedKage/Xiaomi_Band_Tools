using System.Globalization;

namespace XiaomiWatch.Common.FaceFile
{
	public class FaceItemDataSrc
	{
		public const string Hour = "0811";

		public const string HourLow = "0911";

		public const string HourHigh = "1000911";

		public const string Minute = "1011";

		public const string MinuteLow = "1111";

		public const string MinuteHigh = "1211";

		public const string Second = "1811";

		public const string SecondLow = "1911";

		public const string SecondHigh = "1001911";

		public const string AMPM = "0813";

		public const string Month = "1012";

		public const string Day = "1812";

		public const string DayLow = "1912";

		public const string DayHigh = "1001912";

		public const string Weekday = "2012";

		public const string Param182A = "12818";

		public const string Param182B_Vitality = "22818";

		public const string Steps = "0821";

		public const string StepsPercent = "1021";

		public const string Hrm = "0822";

		public const string HrmInverval = "1022";

		public const string Calories = "0823";

		public const string CaloriesPercent = "1023";

		public const string DistanceMeters = "0851";

		public const string DistanceMetersD1 = "0951";

		public const string DistanceMetersD2 = "10951";

		public const string DistanceMetersD3 = "20951";

		public const string DistanceKm = "1051";

		public const string ActivityCount = "0824";

		public const string Stress = "0826";

		public const string Sleep = "0828";

		public const string Battery = "0841";

		public const string BatteryCharging = "1041";

		public const string SleepStatus = "1841";

		public const string BluetoothStatus = "2041";

		public const string ScreenLockStatus = "3041";

		public const string WeatherTemp = "2031";

		public const string WeatherTempFahr = "40009031";

		public const string WeatherType = "3031";

		public const string WeatherAir = "5031";

		public static ushort Parse(string dataSrcValue)
		{
			if (dataSrcValue.Length > 4)
			{
				string text = dataSrcValue.Substring(dataSrcValue.Length - 4);
				string s = dataSrcValue.Replace(text, "");
				ushort num = ushort.Parse(text, NumberStyles.HexNumber);
				ushort num2 = ushort.Parse(s, NumberStyles.HexNumber);
				return (ushort)(num + num2);
			}
			return ushort.Parse(dataSrcValue, NumberStyles.HexNumber);
		}
	}
}
