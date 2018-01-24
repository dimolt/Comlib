using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading;
using System.Data.OracleClient;
//
using ComLib.log;

namespace ComLib.database
{
	/// <summary>
	/// オラクルDB共通処理クラス
	/// </summary>
	public class OraCom
	{
		#region メンバー変数
		/// <summary>
		/// 接続文字列
		/// </summary>
		private string vMconStr;
		/// <summary>
		/// Logger SQL
		/// </summary>
		private ILogger vMsqlLog;
		/// <summary>
		/// Logger 処理
		/// </summary>
		private ILogger vMappLog;
		/// <summary>
		/// DB接続
		/// </summary>
		private OracleConnection vMconnection = null;
		/// <summary>
		/// DBトランザクション
		/// </summary>
		private OracleTransaction vMtran = null;
		/// <summary>
		/// DB再接続 施行回数
		/// </summary>
		private int vMretryCount;
		/// <summary>
		/// DB再接続 間隔(ms)
		/// </summary>
		private int vMretryInterval;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIconStr">接続文字列</param>
		/// <param name="vIapLog">処理用ロガー</param>
		/// <param name="vIsqlLogger">SQL用ロガー</param>
		/// <param name="vIretryCount">DB再接続 施行回数</param>
		/// <param name="vIretryInterval">DB再接続 間隔</param>
		public OraCom(string vIconStr, ILogger vIapLog, ILogger vIsqlLog, int vIretryCount, int vIretryInterval)
		{
			vMconStr = vIconStr;
			vMappLog = vIapLog;
			vMsqlLog = vIsqlLog;
			vMretryCount = vIretryCount;
			vMretryInterval = vIretryInterval;
		}
		#endregion

		#region [public]メソッド
		#region 接続
		/// <summary>
		/// DB接続を開く
		/// </summary>
		public bool Open()
		{
			try
			{
				if (vMconnection != null)
				{
					vMappLog.Warn("*** DB接続CloseせずにOpenしている ***");
				}
				vMappLog.Info("DB接続:[{0}]", vMconStr);
				vMconnection = new OracleConnection(vMconStr);
				vMconnection.Open();
				return true;
			}
			catch (Exception ex)
			{
				vMappLog.Error("DB接続に失敗しました\n" + ex);
				return false;
			}
		}
		/// <summary>
		/// DB接続を閉じる
		/// </summary>
		public void Close()
		{
			if (vMconnection == null)
			{
				return;
			}

			try
			{
				vMconnection.Close();
			}
			catch (Exception ex)
			{
				vMappLog.Error("DBCloseに失敗しました\n" + ex);
			}
			finally
			{
				vMconnection.Dispose();
				vMconnection = null;
			}
		}
		/// <summary>
		/// DB接続確認をし、再接続を試みる
		/// </summary>
		/// <returns></returns>
		public bool KeepConnection()
		{
			if (vMconnection == null)
			{
				this.Open();
			}

			for (int LvCnt = 1; LvCnt < vMretryCount; LvCnt++)
			{
				if (this.CheckConnection() == true)
				{
					//接続OK → 再接続OK
					return true;
				}

				//間隔を置く
				Thread.Sleep(vMretryInterval);

				try
				{
					//再接続
					vMconnection.Close();
					vMconnection.Open();
				}
				catch (Exception exp)
				{
					vMappLog.Warn("DB再接続NG\n" + exp);
				}
			}
			vMappLog.Warn("DB再接続 RetryNG");

			//再接続 失敗
			return false;
		}
		#endregion

		#region トランザクション
		/// <summary>
		/// トランザクション開始
		/// </summary>
		public void BeginTrans()
		{
			if (vMconnection != null)
			{
				vMtran = vMconnection.BeginTransaction();
				vMappLog.Debug("->> トランザクション");
			}
			else
			{
				vMappLog.Warn("DB接続がOpenしていないのでトランザクションを開始できない");
				throw new OraConnectException("DB接続がOpenしていないのでトランザクションを開始できません");
			}
		}
		/// <summary>
		/// トランザクション開始
		/// </summary>
		/// <param name="vIisCommit">T:コミット F:ロールバック</param>
		public void EndTrans(bool vIisCommit)
		{
			if (vIisCommit == true)
			{
				Commit();
			}
			else
			{
				Rollback();
			}
		}
		/// <summary>
		/// コミット
		/// </summary>
		private void Commit()
		{
			if (vMtran != null)
			{
				vMtran.Commit();
				vMappLog.Debug("<<- コミット");
				vMtran = null;
			}
			else
			{
				vMappLog.Warn("トランザクションが開始していないのでCommitできない");
				throw new OraConnectException("トランザクションが開始していないのでCommitできません");
			}
		}
		/// <summary>
		/// ロールバック
		/// </summary>
		private void Rollback()
		{
			if (vMtran != null)
			{
				vMtran.Rollback();
				vMappLog.Debug("<<- ロールバック");
				vMtran = null;
			}
			else
			{
				vMappLog.Warn("トランザクションが開始していないのでRollBackできない");
				throw new OraConnectException("トランザクションが開始していないのでRollBackできません");
			}
		}
		#endregion

		#region SQL実行
		/// <summary>
		/// データテーブルの取得
		/// </summary>
		/// <param name="sSQL">実行するSQL</param>
		/// <returns>取得したデータテーブル</returns>
		public DataTable ExecuteDataTable(string sSQL)
		{
			if (vMconnection == null)
			{
				vMappLog.Warn("DB接続がOpenしていません");
				throw new OraConnectException("DB接続がOpenしていません");
			}

			//SQLLog出力
			this.LoggingSQL(sSQL, null);

			DataTable dt = new DataTable();
			using (OracleDataAdapter da = new OracleDataAdapter())
			{
				using (OracleCommand cmd = new OracleCommand(sSQL, vMconnection, vMtran))
				{
					da.SelectCommand = cmd;
					da.Fill(dt);
				}
			}
			return dt;
		}
		/// <summary>
		/// データテーブルの取得
		/// </summary>
		/// <param name="sSQL">実行するSQL</param>
		/// <param name="prms">実行時パラメータ</param>
		/// <returns>取得したデータテーブル</returns>
		public DataTable ExecuteDataTable(string sSQL, OraParams prms)
		{
			if (vMconnection == null)
			{
				vMappLog.Warn("DB接続がOpenしていません");
				throw new OraConnectException("DB接続がOpenしていません");
			}

			//SQLLog出力
			this.LoggingSQL(sSQL, prms);

			DataTable dt = new DataTable();
			using (OracleDataAdapter da = new OracleDataAdapter())
			{
				using (OracleCommand cmd = new OracleCommand(sSQL, vMconnection, vMtran))
				{
					cmd.CommandType = CommandType.Text;
					if (prms != null && prms.Count > 0)
					{
						AddParam(cmd, prms);
					}

					da.SelectCommand = cmd;
					da.Fill(dt);
				}
				return dt;
			}
		}
		/// <summary>
		/// ＳＱＬ実行
		/// </summary>
		/// <param name="sSQL">実行するSQL</param>
		/// <param name="prms">パラメータ名</param>
		/// <returns>実行結果数</returns>
		public int ExecuteSQL(string sSQL, OraParams prms)
		{
			if (vMconnection == null)
			{
				vMappLog.Warn("DB接続がOpenしていません");
				throw new OraConnectException("DB接続がOpenしていません");
			}

			//SQLLog出力
			this.LoggingSQL(sSQL, prms);

			using (OracleCommand cmd = new OracleCommand(sSQL, vMconnection, vMtran))
			{
				if (vMtran != null)
				{
					//トランザクション設定
					cmd.Transaction = vMtran;
				}
				cmd.CommandType = CommandType.Text;
				if (prms != null && prms.Count > 0)
				{
					AddParam(cmd, prms);
				}
				return cmd.ExecuteNonQuery();
			}
		}
		/// <summary>
		/// SQL実行
		/// </summary>
		/// <param name="sSQL">SQL文字列</param>
		/// <returns>実行結果数</returns>
		public int ExecuteSQL(string sSQL)
		{
			if (vMconnection == null)
			{
				vMappLog.Warn("DB接続がOpenしていません");
				throw new OraConnectException("DB接続がOpenしていません");
			}

			using (OracleCommand cmd = new OracleCommand(sSQL, vMconnection, vMtran))
			{
				if (vMtran != null)
				{
					//トランザクション設定
					cmd.Transaction = vMtran;
				}
				cmd.CommandType = CommandType.Text;

				//SQLLog出力
				this.LoggingSQL(sSQL, null);

				return cmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// オラクルSysdateの取得
		/// </summary>
		/// <returns>SysDate</returns>
		public DateTime GetOraSysdate()
		{
			if (vMconnection == null)
			{
				vMappLog.Warn("DB接続がOpenしていません");
				throw new OraConnectException("DB接続がOpenしていません");
			}

			string sSQL = @"select sysdate from dual";
			using(DataTable dt = ExecuteDataTable(sSQL))
			{
				return Convert.ToDateTime(dt.Rows[0][0]);
			}

		}
		/// <summary>
		/// オラクルCOUNT(*)取得
		/// </summary>
		/// <param name="table">テーブル名</param>
		/// <returns>COUNT(*)数</returns>
		public int GetOraCount(string table)
		{
			return this.GetCount(table, "");
		}
		/// <summary>
		/// オラクルCOUNT(*)取得
		/// </summary>
		/// <param name="table">テーブル名</param>
		/// <param name="where">条件</param>
		/// <returns>COUNT(*)数</returns>
		public int GetCount(string table, string where)
		{
			string sqlWhere = "";
			string sqlSelect = "";

			//WHERE文字列作成
			if (where.Length > 0)
			{
				sqlWhere = "WHERE " + where;
			}

			//SQL文字列作成
			sqlSelect = "select COUNT(*) from {0} {1}";
			sqlSelect = string.Format(sqlSelect, table, sqlWhere);

			//SQL実行
			DataTable dt = ExecuteDataTable(sqlSelect);

			return Convert.ToInt32(dt.Rows[0][0]);
		}
		/// <summary>
		/// ｼｰｹﾝｽの取得
		/// </summary>
		/// <param name="SequenceName">シーケンス名</param>
		/// <returns>取得したｼｰｹﾝｽの数値</returns>
		public int GetSequence(string SequenceName)
		{
			string sqlStr = "SELECT " + SequenceName + ".NEXTVAL from dual ";
			DataTable dt = ExecuteDataTable(sqlStr);
			if (dt.Rows.Count == 0)
			{
				return 0;
			}
			return Convert.ToInt32(dt.Rows[0][0]);
		}
		#endregion
		#endregion

		#region [private]メソッド
		/// <summary>
		/// パラメータの追加
		/// </summary>
		/// <param name="cmd">ORACLEコマンドクラス</param>
		/// <param name="prms">パラメータ</param>
		private void AddParam(OracleCommand cmd, OraParams prms)
		{
			foreach(OracleParameter prm in prms)
			{
				cmd.Parameters.Add(prm);
			}
		}
		/// <summary>
		/// SQLログ出力
		/// </summary>
		/// <param name="sSQL">SQL</param>
		/// <param name="prms">パラメータ</param>
		private void LoggingSQL(string sSQL, OraParams prms)
		{
			//SQLLog
			vMsqlLog.Info(string.Format("実行SQL:{0}", sSQL));
			if (prms != null)
			{
				if (prms.Count > 0)
				{
					vMsqlLog.Info("実行SQLパラメータ-->");
				}
				foreach (OracleParameter LvPrm in prms)
				{
					if (LvPrm.Value != null)
					{
						vMsqlLog.Info(string.Format("{0}:{1}", LvPrm.ParameterName, LvPrm.Value.ToString())); 
					}
					else 
					{ 
						vMsqlLog.Info(LvPrm.ParameterName + ":");
					}
				}
			}
		}
		/// <summary>
		/// DB接続確認
		/// </summary>
		/// <returns>確認結果</returns>
		private bool CheckConnection()
		{
			using (OracleCommand cmd = new OracleCommand("SELECT SYSDATE FROM DUAL", vMconnection))
			{
				cmd.CommandType = CommandType.Text;
				try
				{
					using (OracleDataReader reader = cmd.ExecuteReader())
					{
						vMappLog.Info("DB接続確認OK:" + vMconnection.ConnectionString);
						return true;
					}
				}
				catch (Exception ex)
				{
					vMappLog.Info("DB接続確認NG:" + vMconnection.ConnectionString + "\n" + ex);
					return false;
				}
			}
		}
		#endregion
	}

	/// <summary>
	/// Oracleパラメータクラス
	/// </summary>
	public class OraParams : CollectionBase
	{
		#region インデクサ
		/// <summary>
		/// デフォルトインデクサ
		/// </summary>
		/// <param name="index">インデックス</param>
		/// <returns>パラメータ</returns>
		public OracleParameter this[int index]
		{
			get
			{
				return ((OracleParameter)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}
		/// <summary>
		/// デフォルトインデクサ
		/// </summary>
		/// <param name="prm_name">パラメータ名</param>
		/// <returns>パラメータ</returns>
		public OracleParameter this[string prm_name]
		{
			get
			{
				int index = FindParam(prm_name);
				if (index < 0)
					return null;
				else
					return (OracleParameter)List[index];
			}
			set
			{
				int index = FindParam(prm_name);
				if (index < 0)
					return;
				else
					List[index] = value;
			}
		}
		#endregion

		#region [public]メソッド
		#region 日付型
		/// <summary>
		/// オラクルパラメータの追加(日付型)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="oraType">オラクルデータタイプ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			ParameterDirection direct,
			DateTime value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.DateTime);
			pm.Direction = direct;
			pm.Value = value;
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(日付型)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="oraType">オラクルデータタイプ</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			DateTime value
			)
		{
			Add(ParameterName, ParameterDirection.Input, value);
		}

		/// <summary>
		/// オラクルパラメータの追加(日付型 NULLあり)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="oraType">オラクルデータタイプ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			ParameterDirection direct,
			DateTime? value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.DateTime);
			pm.Direction = direct;
			if (value.HasValue == true)
			{
				pm.Value = value;
			}
			else
			{
				pm.Value = DBNull.Value;
			}
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(日付型 NULLあり)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="oraType">オラクルデータタイプ</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			DateTime? value
			)
		{
			Add(ParameterName, ParameterDirection.Input, value);
		}
		#endregion

		#region 文字列
		/// <summary>
		/// オラクルパラメータの追加(文字列)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			string value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.VarChar, size);
			pm.Direction = direct;
			pm.Value = value;
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(文字列)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			string value
			)
		{
			Add(ParameterName, size, ParameterDirection.Input, value);
		}
		/// <summary>
		/// オラクルパラメータの追加(文字列)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		/// <param name="isCut">カット有無</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			string value,
			bool isCut = false
			)
		{
			if (isCut == true)
			{
				value = StrUtil.LeftByte(value, size);
			}
			Add(ParameterName, size, direct, value);
		}
		#endregion

		#region 数値
		/// <summary>
		/// オラクルパラメータの追加(数値)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			int value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, size);
			pm.Direction = direct;
			pm.Value = value;
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(数値 NULLあり)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			int? value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, size);
			pm.Direction = direct;
			if (value.HasValue == true)
			{
				pm.Value = value;
			}
			else
			{
				pm.Value = DBNull.Value;
			}
			List.Add(pm);
		}

		/// <summary>
		/// オラクルパラメータの追加(数値)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			decimal value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, size);
			pm.Direction = direct;
			pm.Value = value;
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(数値 NULLあり)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="size">データサイズ</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			int size,
			ParameterDirection direct,
			decimal? value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, size);
			pm.Direction = direct;
			if (value.HasValue == true)
			{
				pm.Value = value;
			}
			else
			{
				pm.Value = DBNull.Value;
			}
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(数値)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="precision">データサイズ</param>
		/// <param name="scale">小数点以下桁数</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			byte precision, byte scale,
			ParameterDirection direct,
			decimal value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, precision, ParameterDirection.Input, true,
				precision, scale,
				ParameterName, DataRowVersion.Current,
				value);
			List.Add(pm);
		}
		/// <summary>
		/// オラクルパラメータの追加(数値)
		/// </summary>
		/// <param name="ParameterName">パラメータ名</param>
		/// <param name="precision">データサイズ</param>
		/// <param name="scale">小数点以下桁数</param>
		/// <param name="direct">in/out/inout</param>
		/// <param name="value">値</param>
		public void Add(
			string ParameterName,
			byte precision, byte scale,
			ParameterDirection direct,
			decimal? value
			)
		{
			OracleParameter pm = new OracleParameter(ParameterName, OracleType.Number, precision, ParameterDirection.Input, true,
				precision, scale,
				ParameterName, DataRowVersion.Current,
				DBNull.Value);
			if (value.HasValue == true)
			{
				pm.Value = value.Value;
			}
			List.Add(pm);
		}
		#endregion
		#endregion

		#region [private]メソッド
		/// <summary>
		/// パラメータ名からパラメータインデックスを検索する
		/// </summary>
		/// <param name="prmname">検索対象のパラメータ名</param>
		/// <returns>該当のインデックス</returns>
		private int FindParam(string prmname)
		{
			int ifind = int.MinValue;
			
			for (int index = 0; index < List.Count; index++)
			{
				OracleParameter oprm = (OracleParameter)List[index];
				if (oprm.ParameterName == prmname)
				{
					ifind = index;
					break;
				}
			}
			return ifind;
		}
		#endregion
	}

	/// <summary>
	/// Oracle接続時例外クラス
	/// </summary>
	public class OraConnectException : Exception
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OraConnectException() { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="msg">メッセージ</param>
		public OraConnectException(string msg) : base(msg) { }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="msg">メッセージ</param>
		/// <param name="inner">innerException</param>
		public OraConnectException(string msg, Exception inner) : base(msg, inner) { }
		#endregion
	}
}

