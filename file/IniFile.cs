using System;
using System.Text;
using System.Runtime.InteropServices;

namespace ComLib
{
	/// <summary>
	/// INIファイル制御
	/// </summary>
	public static class IniFile
	{
		#region "WIN32API"
		[DllImport("KERNEL32.DLL")]
		private static extern uint
			GetPrivateProfileString(
			string lpAppName,
			string lpKeyName, string lpDefault,
			StringBuilder lpReturnedString, uint nSize,
			string lpFileName);

		[DllImport("KERNEL32.DLL")]
		private static extern uint
			GetPrivateProfileInt(
			string lpAppName,
			string lpKeyName, int nDefault, string lpFileName);

		#endregion

		#region "定数"
		/// <summary>
		/// バッファ(256文字)
		/// </summary>
		private const int BUFF_LEN = 1024;
		#endregion

		#region メソッド
		/// <summary>
		/// INIファイル 文字取得
		/// </summary>
		/// <param name="lpszSection">セクション</param>
		/// <param name="lpszEntry">キー</param>
		/// <param name="lpszDefault">デフォルト値</param>
		/// <returns>取得文字列</returns>
		public static string GetString(string lpszSection, string lpszEntry, string lpszDefault, string filename)
		{
			StringBuilder sb = new StringBuilder(BUFF_LEN);
			uint ret = GetPrivateProfileString(lpszSection, lpszEntry, lpszDefault, sb, Convert.ToUInt32(sb.Capacity), filename);
			return sb.ToString();
		}
		/// <summary>
		/// INIファイル 数値取得
		/// </summary>
		/// <param name="lpszSection">セクション</param>
		/// <param name="lpszEntry">キー</param>
		/// <param name="lpintDefault">デフォルト値</param>
		/// <returns>取得数値</returns>
		public static int GetInteger(string lpszSection, string lpszEntry, int lpintDefault, string filename)
		{
			uint ret = GetPrivateProfileInt(lpszSection, lpszEntry, lpintDefault, filename);
			return Convert.ToInt32(ret);
		}
		/// <summary>
		/// INIファイル 真偽取得
		/// </summary>
		/// <param name="lpszSection">セクション</param>
		/// <param name="lpszEntry">キー</param>
		/// <param name="lpintDefault">デフォルト値</param>
		/// <returns>真偽数値</returns>
		public static bool GetBool(string lpszSection, string lpszEntry, bool lpintDefault, string filename)
		{
			StringBuilder sb = new StringBuilder(BUFF_LEN);
			uint ret = GetPrivateProfileString(lpszSection, lpszEntry, "", sb, Convert.ToUInt32(sb.Capacity), filename);

			bool vLrslt = false;
			if (bool.TryParse(sb.ToString(), out vLrslt) == true)
			{
				return vLrslt;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}

