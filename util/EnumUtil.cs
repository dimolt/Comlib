using System;

namespace ComLib
{
	/// <summary>
	/// Enumユーティリティ
	/// </summary>
	public static class EnumUtil
	{
		/// <summary>
		/// enum 変換(数値)
		/// </summary>
		/// <typeparam name="T">変換するenum</typeparam>
		/// <param name="value">値</param>
		/// <returns>変換後の値</returns>
		/// <remarks>変換チェックなし</remarks>
		public static T Parse<T>(int value)
		{
			//TryParseでは正しく変換できない → IsDefinedを使用
			if (Enum.IsDefined(typeof(T), value) == true)
			{
				return (T)Enum.Parse(typeof(T), value.ToString());
			}
			else
			{
				return default(T);
			}
		}
		/// <summary>
		/// enum 変換(文字列)
		/// </summary>
		/// <typeparam name="T">変換するenum</typeparam>
		/// <param name="value">値</param>
		/// <returns>変換後の値</returns>
		/// <remarks>変換チェックなし</remarks>
		public static T Parse<T>(string value)
		{
			T vLrslt = default(T);
			int vLval = 0;
			if (int.TryParse(value, out vLval) == true)
			{
				vLrslt = Parse<T>(vLval);
			}
			return vLrslt;
		}

		/// <summary>
		/// enum 変換(数値)
		/// </summary>
		/// <typeparam name="T">変換するenum</typeparam>
		/// <param name="value">値</param>
		/// <param name="enumVal">変換後の値</param>
		/// <returns>T:成功 F:失敗</returns>
		public static bool TryParse<T>(int value, out T enumVal)
		{
			//TryParseでは正しく変換できない → IsDefinedを使用
			if (Enum.IsDefined(typeof(T), value) == true)
			{
				enumVal = (T)Enum.Parse(typeof(T), value.ToString());
				return true;
			}
			else
			{
				enumVal = default(T);
				return false;
			}
		}
		/// <summary>
		/// enum 変換(文字列)
		/// </summary>
		/// <typeparam name="T">変換するenum</typeparam>
		/// <param name="value">値</param>
		/// <param name="enumVal">変換後の値</param>
		/// <returns>T:成功 F:失敗</returns>
		public static bool TryParse<T>(string value, out T enumVal)
		{
			//TryParseでは正しく変換できない → IsDefinedを使用
			if (Enum.IsDefined(typeof(T), value) == true)
			{
				enumVal = (T)Enum.Parse(typeof(T), value);
				return true;
			}
			else
			{
				enumVal = default(T);
				return false;
			}
		}
	}
}
