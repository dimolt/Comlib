using System;
using System.IO;
//
using ComLib.log;

namespace ComLib
{
	/// <summary>
	/// ファイル操作
	/// </summary>
	public class FileControl
	{
		#region メンバ変数
		/// <summary>
		/// ロガー
		/// </summary>
		private ILogger vMlog;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIlog">ロガー</param>
		public FileControl(ILogger vIlog)
		{
			vMlog = vIlog;
		}
		#endregion

		#region [public]メソッド
		/// <summary>
		/// ファイル作成(中身は空)
		/// </summary>
		/// <param name="vIfile">ファイルパス</param>
		/// <returns>処理結果</returns>
		public bool CreateFile(string vIfile)
		{
			try
			{
				using (var strm = File.Create(vIfile))
				{
				}
				vMlog.Info("ファイル作成:" + vIfile);

				return true;
			}
			catch (Exception ex)
			{
				vMlog.Error("ファイル[{0}]作成でエラーが発生\n" + ex, vIfile);
				return false;
			}
		}
		/// <summary>
		/// ディレクトリ作成
		/// </summary>
		/// <param name="vIdir">ディレクトリパス</param>
		/// <returns>処理結果</returns>
		public bool CreateDir(string vIdir)
		{
			try
			{
				if (Directory.Exists(vIdir) == false)
				{
					Directory.CreateDirectory(vIdir);
					vMlog.Info("ディレクトリ作成:" + vIdir);
				}
				return true;
			}
			catch (Exception ex)
			{
				vMlog.Error("ディレクトリ[{0}]作成でエラーが発生\n" + ex, vIdir);
				return false;
			}
		}
		/// <summary>
		/// ファイルコピー
		/// </summary>
		/// <param name="vIsrc">コピー元(フルパス)</param>
		/// <param name="vIdst">コピー先(フルパス)</param>
		/// <returns>処理結果</returns>
		public bool Copy(string vIsrc, string vIdst)
		{
			try
			{
				string dir = Path.GetDirectoryName(vIdst);
				if (Directory.Exists(dir) == false)
				{
					Directory.CreateDirectory(dir);
					vMlog.Info("ディレクトリ作成:" + dir);
				}
				File.Copy(vIsrc, vIdst, true);
				vMlog.Info("ファイルをコピー[{0}]→[{1}]", vIsrc, vIdst);

				return true;
			}
			catch (Exception ex)
			{
				vMlog.Error("ファイルコピーでエラーが発生\n" + ex);
				return false;
			}
		}

		/// <summary>
		/// ファイル移動
		/// </summary>
		/// <param name="vIsrc">移動元(フルパス)</param>
		/// <param name="vIdst">移動先(フルパス)</param>
		/// <returns>処理結果</returns>
		public bool Move(string vIsrc, string vIdst)
		{
			try
			{
				//移動先ディレクトリ作成
				string dstDir = Path.GetDirectoryName(vIdst);
				if (this.CreateDir(dstDir) == false)
				{
					return false;
				}

				//移動先に同名のファイルがある → 先に削除
				if (File.Exists(vIdst) == true)
				{
					if (this.Delete(vIdst) == false)
					{
						return false;
					}
				}
				File.Move(vIsrc, vIdst);
				vMlog.Info("ファイルを移動[{0}]→[{1}]", vIsrc, vIdst);
				return true;
			}
			catch (Exception ex)
			{
				vMlog.Error("ファイル移動でエラーが発生\n" + ex);
				return false;
			}
		}
		/// <summary>
		/// ファイル削除
		/// </summary>
		/// <param name="vIfile">ファイルパス</param>
		/// <returns>処理結果</returns>
		public bool Delete(string vIfile)
		{
			try
			{
				if (File.Exists(vIfile) == false)
				{
					//ファイルなし
					return true;
				}

				File.Delete(vIfile);
				vMlog.Info("ファイル[{0}]を削除", vIfile);
				return true;
			}
			catch (Exception ex)
			{
				vMlog.Error("ファイル削除でエラーが発生\n" + ex);
				return false;
			}
		}
		/// <summary>
		/// ファイル削除(保存日数指定)
		/// </summary>
		/// <param name="vIpath">ファイルパス</param>
		/// <param name="vIwildCard">ワイルドカード</param>
		/// <param name="vIkeepDays">対象日付</param>
		public void DeleteExpiredFile(string vIpath, string vIwildCard, int vIkeepDays)
		{
			if (Directory.Exists(vIpath) == false)
			{
				vMlog.Warn("ディレクトリ[{0}]がありません", vIpath);
				return;
			}

			//削除対象日付
			DateTime expireDate = DateTime.Now.AddDays(- vIkeepDays);
			try
			{
				foreach (string targetFile in Directory.GetFiles(vIpath, vIwildCard))
				{
					DateTime fileDate = File.GetLastWriteTime(targetFile);
					if (expireDate >= fileDate)
					{
						//ファイル削除
						File.Delete(targetFile);
						//ログ出力
						vMlog.Info("ファイル[{0}]を削除しました", targetFile);
					}
				}
			}
			catch(Exception ex)
			{
				vMlog.Warn("ファイルの削除でエラーが発生\n" + ex);
			}
		}
		/// <summary>
		/// ディレクトリ削除(保存日数指定)
		/// </summary>
		/// <param name="vIpath">ファイルパス</param>
		/// <param name="vIwildCard">ワイルドカード</param>
		/// <param name="vIkeepDays">対象日付</param>
		/// <remarks>指定パスの「yyyyMMdd」形式のディレクトリを削除</remarks>
		public void DeleteExpiredDirectory(string vIpath, int vIkeepDays)
		{
			if (Directory.Exists(vIpath) == false)
			{
				vMlog.Warn("ディレクトリ[{0}]がありません", vIpath);
				return;
			}

			//削除対象日付
			DateTime expireDate = DateTime.Now.AddDays(-vIkeepDays);
			try
			{
				foreach (string targetDir in Directory.GetDirectories(vIpath))
				{
					//ディレクトリ日付取得
					DateTime dirDate;
					if (StrUtil.TryParseDateTime(Path.GetFileName(targetDir), "yyyyMMdd", out dirDate) == false)
					{
						continue;
					}

					//日付を比較
					if (expireDate.Date >= dirDate.Date)
					{
						//ディレクトリ削除
						Directory.Delete(targetDir, true);
						//ログ出力
						vMlog.Info("ディレクトリ[{0}]を削除しました", targetDir);
					}
				}
			}
			catch (Exception ex)
			{
				vMlog.Warn("ディレクトリの削除でエラーが発生\n" + ex);
			}
		}
		#endregion
	}
}
