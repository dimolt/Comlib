using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace ComLib
{
	/// <summary>
	/// 文字列操作
	/// 
	/// Copyright (C) 2015, CST
	///  
	///	(Rev.)		  (Date)		(ID / NAME)			(Comment)
	/// V1.00.00	: 2015.09.30	: S.Matsumot@CST	: 新規作成
	/// </summary>
	public static class StrUtil
	{
		/// <summary>
		/// エンコード
		/// </summary>
		private static Encoding vMencode = Encoding.GetEncoding("Shift_Jis");

		#region 文字列操作
		/// <summary>
		/// 部分文字列取得(文字数)
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIstart">開始位置(0～)</param>
		/// <param name="vIlen">文字数</param>
		/// <returns>部分文字列</returns>
		public static string SubStr(string vIsrc, int vIstart, int vIlen)
		{
			string result = "";
			try
			{
				int vLlen = vIsrc.Length;
				if (vIstart > vLlen)
				{
					return "";
				}
				if (vIstart + vIlen > vLlen)
				{
					vIlen = vLlen - vIstart;
				}

				result = vIsrc.Substring(vIstart, vIlen);
			}
			catch
			{
				result = "";
			}
			return result;
		}
		/// <summary>
		/// 部分文字列取得(バイト数)
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIbyte">バイト数</param>
		/// <returns>部分文字列</returns>
		public static string LeftByte(string vIsrc, int vIbyte)
		{
			string vLrslt = "";
			try
			{
				byte[] vLbytes = vMencode.GetBytes(vIsrc);
				//文字列が指定バイト数より小さい
				if (vLbytes.Length <= vIbyte)
				{
					return vIsrc;
				}
				
				//指定バイト数の文字列取得
				vLrslt = vMencode.GetString(vLbytes, 0, vIbyte);
				//マルチバイト判定
				if (vLrslt.EndsWith("\0") || vLrslt.EndsWith("・"))
				{
					vLrslt = vMencode.GetString(vLbytes, 0, vIbyte - 1);
				}				
			}
			catch
			{
				vLrslt = "";
			}
			return vLrslt;
		}
		/// <summary>
		/// 左埋め文字列作成
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIcount">カウント</param>
		/// <param name="vIpad">パディング文字</param>
		/// <param name="vIisByte">文字カウント方法 T:バイト F:文字数</param>
		/// <returns>左埋めされた文字列</returns>
		public static string LPad(string vIsrc, int vIcount, string vIpad, bool vIisByte)
		{
			int vLlen = vIisByte ? ByteCount(vIsrc) : vIsrc.Length;
			if (vLlen < vIcount)
			{
				return new StringBuilder().Insert(0, vIpad, vIcount - vLlen) + vIsrc;
			}
			else
			{
				return vIsrc;
			}
			
		}
		/// <summary>
		/// 右埋め文字列作成
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIcount">カウント</param>
		/// <param name="vIpad">パディング文字</param>
		/// <param name="vIisByte">文字カウント方法 T:バイト F:文字数</param>
		/// <returns>左埋めされた文字列</returns>
		public static string RPad(string vIsrc, int vIcount, string vIpad, bool vIisByte)
		{
			int vLlen = vIisByte ? ByteCount(vIsrc) : vIsrc.Length;
			if (vLlen < vIcount)
			{
				return vIsrc + new StringBuilder().Insert(0, vIpad, vIcount - vLlen);
			}
			else
			{
				return vIsrc;
			}
		}
		/// <summary>
		/// 全角を半角に変換する
		/// </summary>
		/// <param name="vIsrc">全角文字列</param>
		/// <returns>半角文字列</returns>
		public static string ZenToHan(string vIsrc)
		{
			return Strings.StrConv(vIsrc, VbStrConv.Narrow);
		}
		/// <summary>
		/// 半角を全角に変換する
		/// </summary>
		/// <param name="vIsrc">半角文字列</param>
		/// <returns>全角文字列</returns>
		public static string HanToZen(string vIsrc)
		{
			return Strings.StrConv(vIsrc, VbStrConv.Wide);
		}
		/// <summary>
		/// 文字列連結
		/// </summary>
		/// <param name="vIsrc">連結先の文字列</param>
		/// <param name="vIadd">連結する文字列</param>
		/// <param name="vIdelim">区切り文字</param>
		/// <returns>連結後の文字列</returns>
		public static string Concat(string vIsrc, string vIadd, string vIdelim)
		{
			if ((string.IsNullOrEmpty(vIsrc) == false) && (string.IsNullOrEmpty(vIadd) == false))
			{
				return vIsrc + vIdelim + vIadd;
			}
			return vIsrc + vIadd;
		}
		/// <summary>
		/// 文字列を「"」で囲む
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <returns>囲まれた文字列</returns>
		public static string DoubleQuote(string vIsrc)
		{
			if (string.IsNullOrEmpty(vIsrc) == true)
			{
				return "\"\"";
			}
			//「"」→「""」
			string val = vIsrc.Replace("\"", "\"\"");
			return string.Format("\"{0}\"", val);
		}
		/// <summary>
		/// トリム
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIquote">トリム文字</param>
		/// <returns>トリム後文字列</returns>
		public static string Trim(string vIsrc, char vIquote)
		{
			if (string.IsNullOrEmpty(vIsrc) == true)
			{
				return "";
			}
			return vIsrc.Trim(vIquote);
		}
		/// <summary>
		/// 文字列繰返
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="count">回数</param>
		/// <returns>文字列</returns>
		public static string Repeat(char vIsrc, int count)
		{
			if (count <= 0)
			{
				return "";
			}
			return new string(vIsrc, count);
		}
		/// <summary>
		/// 半角全角を無視して文字列比較
		/// </summary>
		/// <param name="src">文字列1</param>
		/// <param name="target">文字列2</param>
		/// <returns>T:一致 F:不一致</returns>
		public static bool IgnoreWidthEqual(string src, string target)
		{
			var compareInfo = CultureInfo.CurrentCulture.CompareInfo;
			int rslt = compareInfo.Compare(src, target, CompareOptions.IgnoreWidth);
			return (rslt == 0);
		}
		/// <summary>
		/// 文字列分割
		/// </summary>
		/// <param name="txt">文字列</param>
		/// <param name="spliter">区切り文字</param>
		/// <returns>文字列リスト</returns>
		public static List<string> Split(string txt, string spliter)
		{
			var list = new List<string>();

			if (string.IsNullOrEmpty(txt) == false)
			{
				list.AddRange(txt.Split(new string[] { spliter }, StringSplitOptions.RemoveEmptyEntries));
			}
			return list;
		}
		/// <summary>
		/// 改行コード 調整
		/// </summary>
		/// <param name="src">文字列</param>
		/// <returns>調整済文字列</returns>
		public static string AdjustLine(string src)
		{
			if (string.IsNullOrEmpty(src) == true)
			{
				return "";
			}

			string txt = "";
			using (StringReader reader = new StringReader(src))
			{
				while (reader.Peek() >= 0)
				{
					txt = StrUtil.Concat(txt, reader.ReadLine(), "\r\n");
				}
			}
			return txt;
		}
		#endregion

		#region 型変換
		/// <summary>
		/// 文字列→数値変換(int)
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIdef">デフォルト値</param>
		/// <returns>数値</returns>
		public static int ToInt(string vIsrc, int vIdef)
		{
			int rslt;
			if (int.TryParse(vIsrc, out rslt) == true)
			{
				return rslt;
			}
			else 
			{
				return vIdef;
			}
		}
		/// <summary>
		/// 文字列→数値変換(long)
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIdef">デフォルト値</param>
		/// <returns>数値</returns>
		public static long ToLong(string vIsrc, long defval)
		{
			long rslt;
			if (long.TryParse(vIsrc, out rslt) == true)
			{
				return rslt;
			}
			else
			{
				return defval;
			}
		}
		/// <summary>
		/// 文字列→数値変換(decimal)
		/// </summary>
		/// <param name="vIsrc">文字列</param>
		/// <param name="vIdef">デフォルト値</param>
		/// <returns>数値</returns>
		public static decimal ToDecimal(string vIsrc, decimal defval)
		{
			decimal rslt;
			if (decimal.TryParse(vIsrc, out rslt) == true)
			{
				return rslt;
			}
			else
			{
				return defval;
			}
		}
		/// <summary>
		/// 文字列→日付変換
		/// </summary>
		/// <param name="vIsrc">日付文字列</param>
		/// <param name="vIformat">日付フォーマット</param>
		/// <param name="vOrslt">変換後の日付</param>
		/// <returns>変換結果 T:成功 F:失敗</returns>
		public static bool TryParseDateTime(string vIsrc, string vIformat, out DateTime vOrslt)
		{
			vOrslt = DateTime.MinValue;
			try
			{
				vOrslt = DateTime.ParseExact(vIsrc, vIformat, null);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		/// <summary>
		/// 文字列→日付変換
		/// </summary>
		/// <param name="vIsrc">日付文字列</param>
		/// <param name="vIformat">日付フォーマット</param>
		/// <returns>変換後の日付(失敗時はNULL)</returns>
		public static Nullable<DateTime> ParseDateTime(string vIsrc, string vIformat)
		{
			DateTime? value = null;
			try
			{
				value = DateTime.ParseExact(vIsrc, vIformat, null);
				return value;
			}
			catch (Exception)
			{
				return null;
			}
		}
		/// <summary>
		/// 日付(Nullable)→文字列
		/// </summary>
		/// <param name="vIsrc">日付</param>
		/// <param name="vIformat">フォーマット</param>
		/// <returns>日付文字列</returns>
		public static string DateTimeToStr(DateTime? vIsrc, string vIformat)
		{
			if (vIsrc.HasValue == false)
			{
				return "";
			}

			return vIsrc.Value.ToString(vIformat);
		}
		#endregion

		#region その他
		/// <summary>
		/// バイト数取得(Shift_Jis)
		/// </summary>
		/// <param name="vIstr">文字列</param>
		/// <returns>バイト数</returns>
		public static int ByteCount(string vIstr)
		{
			if (string.IsNullOrEmpty(vIstr) == true)
			{
				return 0;
			}
			else
			{
				return vMencode.GetByteCount(vIstr);
			}
		}
		#endregion

		#region URL関連
		/// <summary>
		/// URL/URI結合
		/// </summary>
		/// <param name="baseUrl">結合元</param>
		/// <param name="subPath">追加部分</param>
		/// <returns>結合済URL/URI</returns>
		public static string AppendUrl(string baseUrl, string subPath)
		{
			if ((string.IsNullOrEmpty(subPath) == true) ||
				subPath == "/")
			{
				return baseUrl;
			}

			string url = baseUrl;
			if (baseUrl.EndsWith("/") == false)
			{
				url += "/";
			}

			return url + subPath;
		}
		#endregion
	}
}
