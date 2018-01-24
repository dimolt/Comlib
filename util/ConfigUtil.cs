using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Reflection;

namespace ComLib.util
{
	/// <summary>
	/// 
	/// app.Configアクセス ユーティリティ
	/// 
	/// Copyright (C) 2017, Yokogawa Medical Solutions Corpration
	///  
	///	(Rev.)		  (Date)		  (ID / NAME)			  (Comment)
	/// V1.00.00	: 2017.09.15      : S.Matsumot@CST        : 新規作成
	/// </summary>
	public static class ConfigUtil
	{
		#region [public]メソッド
		/// <summary>
		/// app.configファイルのパス
		/// </summary>
		public static string AppConfigPath
		{
			get
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				string configPath = Path.Combine(Path.GetDirectoryName(asm.Location), Process.GetCurrentProcess().ProcessName);
				configPath += ".exe.config";
				return configPath;
			}
		}

		/// <summary>
		/// 設定値取得(文字)
		/// </summary>
		/// <param name="settingName">エントリー名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>設定値</returns>
		public static string StrVal(string settingName, string defVal)
		{
			if (Array.IndexOf(ConfigurationManager.AppSettings.AllKeys, settingName) >= 0)
			{
				return ConfigurationManager.AppSettings[settingName];
			}
			else
			{
				return defVal;
			}
		}
		/// <summary>
		/// 設定値取得(数値)
		/// </summary>
		/// <param name="vIsetting">設定名称</param>
		/// <param name="vIdef">デフォルト値</param>
		/// <returns>数値</returns>
		public static int IntVal(string settingName, int vIdef)
		{
			string val = StrVal(settingName, "");
			int result = 0;
			if (int.TryParse(val, out result) == false)
			{
				result = vIdef;
			}
			return result;
		}
		/// <summary>
		/// 設定値取得(文字List)
		/// </summary>
		/// <param name="settingName">エントリー名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>設定値</returns>
		public static List<string> ListVal(string settingName, string defVal)
		{
			string val = StrVal(settingName, defVal);
			return StrUtil.Split(val, ",");
		}
		#endregion
	}
}