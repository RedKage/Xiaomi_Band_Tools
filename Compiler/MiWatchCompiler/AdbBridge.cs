using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using XiaomiWatch.Common;

namespace MiWatchCompiler
{
	internal class AdbBridge
	{
		private const int OffsetFaceId = 40;

		private static string adbFile;

		private static int faceInitId = 362700001;

		private static int faceCurrId = 0;

		private const string watchface_item_new = "{\"id\":\t\"[#####]\",\r\n\"name\":\t\"Storage\",\r\n\"name_translation\":\t[],\r\n\"version\":\t\"0\",\r\n\"type\":\t\"1\",\r\n\"in_use\":\t\"0\",\r\n\"is_delete\":\t\"0\",\r\n\"editable\":\t\"0\",\r\n\"support_album\":\t\"0\",\r\n\"support_AOD\":\t\"0\",\r\n\"support_dark_mode\":\t\"0\",\r\n\"sku\":\t\"0\",\r\n\"power_consumption\":\t\"0\",\r\n\"theme_count\":\t\"1\",\r\n\"color_table\":\t[],\r\n\"color_group_table\":\t[],\r\n\"trial_period\":\t\"0\",\r\n\"theme_type_info\":\t[{\r\n\t\t\"name\":\t\"\",\r\n\t\t\"type\":\t\"0\"\r\n\t}]},\r\n";

		private const string watchfaces_clean_json = "\r\n{\r\n\t\"update_info\":\t{\r\n\t\t\"firmware_version\":\t\"0.0.1\",\r\n\t\t\"update_complete\":\t\"1\"\r\n\t},\r\n\t\"watchface_list\":\t[{\r\n\t\t\t\"id\":\t\"362120001\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Metallurgy\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"交织\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"28\",\r\n\t\t\t\t\t\"translation\":\t\"交織\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"8\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"6\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362120002\",\r\n\t\t\t\"name\":\t\"Dilemma\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"9\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"12\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme5\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme5\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme6\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme6\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362120003\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Around town\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"菱形时间\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"28\",\r\n\t\t\t\t\t\"translation\":\t\"菱形時間\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"259\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"4\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362120011\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Black X\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Black X\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"256\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"8\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"watchface_theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130001\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Wonder\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Wonder\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"8\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130002\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Solitude\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Noisy 01\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"8\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[\"0\", \"15750656\", \"16742446\", \"0\", \"16691520\", \"3430805\", \"1942371\"],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130003\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Neat\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Neat 01\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"1\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"14\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme7\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme7\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130004\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Day to night\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Daynight 01\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"2\",\r\n\t\t\t\t\t\"translation\":\t\"Daynight 01\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"2560\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"4\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[\"2618798\", \"16770595\", \"14614478\"],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130005\",\r\n\t\t\t\"name\":\t\"Minerals-GMT\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"14\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"12\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130006\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Wheel\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Wheel\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"4\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[\"16728064\", \"65504\", \"10474914\", \"16769688\", \"15819315\", \"65504\", \"10474914\", \"16769688\"],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130007\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Order\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Order W\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"2\",\r\n\t\t\t\t\t\"translation\":\t\"Order W\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"1536\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"6\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130008\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Explore\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Explore\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"2\",\r\n\t\t\t\t\t\"translation\":\t\"Explore\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"512\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"2\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[\"6663423\", \"5947804\"],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130009\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Spectrum\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Order\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"2\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130011\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Fortune\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Wonder Yellow\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"6\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130012\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Spotlight\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Spotlight\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"2\",\r\n\t\t\t\t\t\"translation\":\t\"Spotlight\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"2304\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"6\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[\"3708652\", \"15493432\", \"6061409\", \"11776947\", \"6396363\", \"8040322\"],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362130015\",\r\n\t\t\t\"name\":\t\"\",\r\n\t\t\t\"name_translation\":\t[{\r\n\t\t\t\t\t\"language\":\t\"0\",\r\n\t\t\t\t\t\"translation\":\t\"Neat Pro\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"language\":\t\"1\",\r\n\t\t\t\t\t\"translation\":\t\"Neat Pro\"\r\n\t\t\t\t}],\r\n\t\t\t\"version\":\t\"10\",\r\n\t\t\t\"type\":\t\"0\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"1\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"1\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"14\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme7\",\r\n\t\t\t\t\t\"type\":\t\"1\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme3\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme4\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme5\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme6\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme7\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, [#####]{\r\n\t\t\t\"id\":\t\"362150001\",\r\n\t\t\t\"name\":\t\"362150001_默认息屏1\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"257\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"1\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"2\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362150003\",\r\n\t\t\t\"name\":\t\"362150003_默认息屏3\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"257\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"2\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362150004\",\r\n\t\t\t\"name\":\t\"362150004_默认息屏4\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"257\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"1\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"2\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}, {\r\n\t\t\t\t\t\"name\":\t\"theme2\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362150005\",\r\n\t\t\t\"name\":\t\"362150005_默认息屏5\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"257\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"0\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"1\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362150006\",\r\n\t\t\t\"name\":\t\"362150006_默认息屏6\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"1\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"0\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"1\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}, {\r\n\t\t\t\"id\":\t\"362150007\",\r\n\t\t\t\"name\":\t\"长续航模式表盘\",\r\n\t\t\t\"name_translation\":\t[],\r\n\t\t\t\"version\":\t\"1\",\r\n\t\t\t\"type\":\t\"2\",\r\n\t\t\t\"in_use\":\t\"0\",\r\n\t\t\t\"is_delete\":\t\"0\",\r\n\t\t\t\"editable\":\t\"0\",\r\n\t\t\t\"support_album\":\t\"0\",\r\n\t\t\t\"support_AOD\":\t\"0\",\r\n\t\t\t\"support_dark_mode\":\t\"0\",\r\n\t\t\t\"sku\":\t\"0\",\r\n\t\t\t\"power_consumption\":\t\"0\",\r\n\t\t\t\"theme_count\":\t\"1\",\r\n\t\t\t\"color_table\":\t[],\r\n\t\t\t\"color_group_table\":\t[],\r\n\t\t\t\"trial_period\":\t\"0\",\r\n\t\t\t\"theme_type_info\":\t[{\r\n\t\t\t\t\t\"name\":\t\"theme1\",\r\n\t\t\t\t\t\"type\":\t\"0\"\r\n\t\t\t\t}]\r\n\t\t}]\r\n}\r\n";

		internal static int Push(string outputFilename)
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string text = Path.Combine(Path.GetDirectoryName(outputFilename), "resource.bin");
			adbFile = Path.Combine(directoryName, "adb.exe");
			(int, string) tuple = ExecuteAdbCommand("shell ls -l /data/app/watchface/market");
			if (tuple.Item1 != 0)
			{
				return -1;
			}
			bool flag = false;
			if (!string.IsNullOrEmpty(tuple.Item2))
			{
				MatchCollection matchCollection = new Regex("\\d{9}(?=/)").Matches(tuple.Item2);
				if (matchCollection.Count > 0)
				{
					int[] source = (from Match c in matchCollection
						select int.Parse(c.Value) into c
						orderby c
						select c).ToArray();
					int watchfaceId = GetWatchfaceId(text);
					if (source.Contains(watchfaceId))
					{
						faceCurrId = watchfaceId;
						flag = true;
					}
					else
					{
						faceCurrId = source.Last() + 1;
					}
				}
				else
				{
					faceCurrId = faceInitId;
				}
			}
			if (flag)
			{
				DeletePrevInstalledFace();
			}
			if (ExecuteAdbCommand($"shell mkdir /data/app/watchface/market/{faceCurrId}").code != 0)
			{
				return -2;
			}
			SetWatchfaceId(outputFilename, text, faceCurrId);
			if (ExecuteAdbCommand($"push \"{text}\" /data/app/watchface/market/{faceCurrId}/resource.bin").code != 0)
			{
				return -3;
			}
			if (ExecuteAdbCommand("shell rm /data/app/watchface/watchface_list.json").code != 0)
			{
				return -4;
			}
			if (ExecuteAdbCommand("shell reboot").code != 0)
			{
				return -5;
			}
			return 0;
		}

		private static int DeletePrevInstalledFace()
		{
			return DeleteDirectoryFiles($"/data/app/watchface/market/{faceCurrId}");
		}

		private static int DeleteDirectoryFiles(string path)
		{
			(int, string) tuple = ExecuteAdbCommand("shell ls -l " + path);
			if (tuple.Item1 != 0)
			{
				return -1;
			}
			foreach (FileSystemEntry item in ParseDirectoryEntries(tuple.Item2))
			{
				if (item.IsDirectory)
				{
					DeleteDirectoryFiles(path + "/" + item.Name.TrimEnd('/'));
				}
				ExecuteAdbCommand("shell rm " + path + "/" + item.Name);
			}
			return 0;
		}

		private static List<FileSystemEntry> ParseDirectoryEntries(string msg)
		{
			Regex regex = new Regex("(?<type>d|-)[rwx-]{9}\\s+\\d+\\s+(?<size>\\d+)\\s+(?<name>.+)$", RegexOptions.Multiline);
			List<FileSystemEntry> list = new List<FileSystemEntry>();
			foreach (Match item in regex.Matches(msg))
			{
				list.Add(new FileSystemEntry
				{
					IsDirectory = (item.Groups["type"].Value == "d"),
					Size = item.Groups["size"].Value,
					Name = item.Groups["name"].Value
				});
			}
			return list;
		}

		private static int GetWatchfaceId(string outputFilename)
		{
			if (!File.Exists(outputFilename))
			{
				return -1;
			}
			if (int.TryParse(File.ReadAllBytes(outputFilename).GetAsciiString(40u), out var result))
			{
				return result;
			}
			return -1;
		}

		private static void SetWatchfaceId(string inFilename, string outFilename, int faceCurrId)
		{
			byte[] array = File.ReadAllBytes(inFilename);
			array[5] = 10;
			array.SetByteArray(40, Encoding.ASCII.GetBytes(faceCurrId.ToString()));
			File.WriteAllBytes(outFilename, array);
		}

		private static (int code, string msg) ExecuteAdbCommand(string command)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = adbFile,
				Arguments = command,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};
			try
			{
				using (Process process = Process.Start(startInfo))
				{
					string item = process.StandardOutput.ReadToEnd();
					string item2 = process.StandardError.ReadToEnd();
					process.WaitForExit();
					if (process.ExitCode == 0)
					{
						return (code: 0, msg: item);
					}
					return (code: -1, msg: item2);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
				return (code: -2, msg: ex.Message);
			}
		}
	}
}
