//#define FRAMEWORK_V2
#define FRAMEWORK_V4

using System;
using System.Data;

namespace ComLib.database
{
	/// <summary>
	/// DataRowデータ取得 クラス
	/// </summary>
	public class RowAdaptor
	{
		#region メンバー変数
		/// <summary>
		/// データ
		/// </summary>
		private DataRow row = null;
		#endregion

		#region プロパティ
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIrow">データ</param>
		public RowAdaptor(DataRow vIrow)
		{
			this.row = vIrow;
		}
		#endregion

		#region [publlc]メソッド
		/// <summary>
		/// データNULL 確認
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <returns>T:Null F:Not Null</returns>
		public bool IsNull(string field)
		{
			if (row.Table.Columns.Contains(field) == true)
			{
				return Convert.IsDBNull(row[field]);
			}
			else
			{
				return false;
			}
		}
		/// <summary>
		/// データ取得(文字列)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <returns>データ値</returns>
		public string StrVal(string field)
		{
			if (row.Table.Columns.Contains(field) == true)
			{
				return row[field].ToString();
			}
			else
			{
				return "";
			}
		}

		#region 数値
		/// <summary>
		/// データ取得(数値)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>データ値</returns>
		public int IntVal(string field, int defVal = int.MinValue)
		{
			string strVal = StrVal(field);
			int val;
			if (int.TryParse(strVal, out val) == true)
			{
				return val;
			}
			else
			{
				return defVal;
			}
		}
		/// <summary>
		/// データ取得(数値)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>データ値</returns>
		/// <remarks>DBNULLをデフォルト値に使用</remarks>
		public object ObjValInt(string field, object defVal = null)
		{
			string strVal = StrVal(field);
			int val;
			if (int.TryParse(strVal, out val) == true)
			{
				return val;
			}
			else
			{
				return (defVal == null) ? DBNull.Value : defVal;
			}
		}

		/// <summary>
		/// データ取得(数値)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>データ値</returns>
		public long LongVal(string field, long defVal = long.MinValue)
		{
			string strVal = StrVal(field);
			long val;
			if (long.TryParse(strVal, out val) == true)
			{
				return val;
			}
			else
			{
				return defVal;
			}
		}
		#endregion

		#region 日付
		/// <summary>
		/// データ取得(日付)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>データ値</returns>
		public DateTime DateVal(string field, DateTime defVal)
		{
			try
			{
#if FRAMEWORK_V2
				return (DateTime)row[field];
#else
				return row.Field<DateTime>(field);
#endif
			}
			catch
			{
				return defVal;
			}
		}
		/// <summary>
		/// データ取得(日付)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <returns>データ値</returns>
		public DateTime DateVal(string field)
		{
			return DateVal(field, DateTime.MinValue);
		}
		/// <summary>
		/// データ取得(日付)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="defVal">デフォルト値</param>
		/// <returns>データ値</returns>
		/// <remarks>DBNULLをデフォルト値に使用</remarks>
		public object ObjValDate(string field, object defVal = null)
		{
			try
			{
#if FRAMEWORK_V2
				return (DateTime)row[field];
#else
				return row.Field<DateTime>(field);
#endif
			}
			catch
			{
				return (defVal == null) ? DBNull.Value : defVal;
			}
		}
		/// <summary>
		/// データ取得(日付Nullable)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <returns>データ値</returns>
		public DateTime? DateValNullable(string field)
		{
			if (row.Table.Columns.Contains(field) == true)
			{
				if (Convert.IsDBNull(row[field]) == true)
				{
					return null;
				}
				else
				{
					DateTime? val = new DateTime?();
					val = (DateTime)row[field];
					return val;
				}
			}
			else
			{
				return null;
			}
		}
		/// <summary>
		/// データ取得(日付→文字列)
		/// </summary>
		/// <param name="field">カラム名</param>
		/// <param name="format">フォーマット</param>
		/// <returns>データ値</returns>
		public string DateToStr(string field, string format)
		{
			if (row.Table.Columns.Contains(field) == true)
			{
				if (Convert.IsDBNull(row[field]) == true)
				{
					return "";
				}
				else
				{
					DateTime val = (DateTime)row[field];
					return val.ToString(format);
				}
			}
			else
			{
				return "";
			}
		}
		#endregion
		#endregion
	}
}
