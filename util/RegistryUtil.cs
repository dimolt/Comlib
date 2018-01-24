using Microsoft.Win32;

namespace ComLib
{
	/// <summary>
	/// レジストリ操作
	/// </summary>
	public static class RegsitryUtil
	{
		/// <summary>
		/// 値取得　文字列
		/// </summary>
		/// <param name="key">キー名</param>
		/// <param name="entry">エントリー名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>レジストリ設定値</returns>
		public static string GetStr(string key, string entry, string defVal)
		{
			return (string)Registry.GetValue(key, entry, defVal);
		}
		/// <summary>
		/// 値取得　数値
		/// </summary>
		/// <param name="key">キー名</param>
		/// <param name="entry">エントリー名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>レジストリ設定値</returns>
		public static int GetInt(string key, string entry, int defVal)
		{
			return (int)Registry.GetValue(key, entry, defVal);
		}
	}
}
