using System;
using System.Text;
using System.Globalization;
using Microsoft.VisualBasic;

namespace ComLib
{
	/// <summary>
	/// 日付操作
	/// </summary>
	public static class DateUtil
	{
		/// <summary>
		/// 指定日付 00:00:00 を取得
		/// </summary>
		/// <param name="date">日付</param>
		/// <returns>指定日付 00:00:00</returns>
		public static DateTime StartOfDate(DateTime date)
		{
			return date.Date;
		}
		/// <summary>
		/// 指定日付 23:59:59 を取得
		/// </summary>
		/// <param name="date">日付</param>
		/// <returns>指定日付 23:59:59</returns>
		public static DateTime EndOfDate(DateTime date)
		{
			return date.Date.AddDays(1).AddMilliseconds(-1);
		}
		/// <summary>
		/// 経過年数(年齢)取得
		/// </summary>
		/// <param name="vIbase">基準日</param>
		/// <param name="vItarget">判定日</param>
		/// <remarks>基準日 ＜ 判定日を前提条件とする</remarks>
		/// <returns>経過年数</returns>
		public static int PastYear(DateTime vIbase, DateTime vItarget)
		{
			int vLpast = vItarget.Year - vIbase.Year;
			//日付の比較
			if (vItarget < new DateTime(vItarget.Year, vIbase.Month, vIbase.Day))
			{
				vLpast -= 1;
			}
			return vLpast;
		}
	}
}
