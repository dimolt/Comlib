using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
//
using ComLib.log;

namespace ComLib.ftp
{
	/// <summary>
	/// FTP制御クラス
	/// </summary>
	public class FtpControl
	{
		#region inner class
		/// <summary>
		/// ファイル・ディレクトリ情報
		/// </summary>
		internal class EntryInfo
		{
			/// <summary>
			/// OS 種別
			/// </summary>
			private enum FileListStyle
			{
				Unix,
				Windows,
				UnKnown
			}

			#region プロパティ
			/// <summary>
			/// ファイル・ディレクトリ名
			/// </summary>
			public string EntryName
			{
				private set;
				get;
			}
			/// <summary>
			/// ディレクトリフラグ
			/// </summary>
			public bool IsDirectory
			{
				private set;
				get;
			}
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="data">ListDirectoryDetailsで取得した文字列</param>
			public EntryInfo(string data)
			{
				this.EntryName = "";

				//OS判定後、解析
				switch (GuessFileListStyle(data))
				{
					case FileListStyle.Windows:
						ParseWindows(data);
						break;
					case FileListStyle.Unix:
						ParseUnix(data);
						break;
					default:
						break;
				}
			}
			#endregion

			#region [private]メソッド
			/// <summary>
			/// OS判定
			/// </summary>
			/// <param name="data">ListDirectoryDetailsで取得した文字列</param>
			/// <returns>OS種別</returns>
			private FileListStyle GuessFileListStyle(string data)
			{
				if ((data.Length > 10) && 
					(Regex.IsMatch(data.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)")))
				{
					return FileListStyle.Unix;
				}
				else if ((data.Length > 8) &&
					Regex.IsMatch(data.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
				{
					return FileListStyle.Windows;
				}
				else
				{
					return FileListStyle.UnKnown;
				}
			}
			/// <summary>
			/// ListDirectoryDetailsで取得した文字列解析(Windows)
			/// </summary>
			/// <param name="data">ListDirectoryDetailsで取得した文字列</param>
			private void ParseWindows(string data)
			{
				// sample
				// 02-03-04  07:46PM       <DIR>          FileName

				string txt = data.Trim();

				CutElement(ref txt, ' ', 0);	//日付を削除
				CutElement(ref txt, ' ', 0);	//時刻を削除

				//ディレクトリ or ファイル
				if (txt.Substring(0, 5) == "<DIR>")
				{
					this.IsDirectory = false;
					//<DIR> を削除
					txt = txt.Substring(5, txt.Length - 5).Trim();
				}
				else
				{
					this.IsDirectory = false;
					CutElement(ref txt, ' ', 0);
				}
				this.EntryName = txt;
			}
			/// <summary>
			/// ListDirectoryDetailsで取得した文字列解析(Unix)
			/// </summary>
			/// <param name="data">ListDirectoryDetailsで取得した文字列</param>
			private void ParseUnix(string data)
			{
				// sample
				// dr-xr-xr-x   1 owner    group               0 Nov 25  2002 FileName

				string txt = data.Trim();
				string flags = CutElement(ref txt, ' ', 0);
				this.IsDirectory = flags.StartsWith("d");

				CutElement(ref txt, ' ', 0);	//使わない情報なのでスキップ
				CutElement(ref txt, ' ', 0);
				CutElement(ref txt, ' ', 0);
				CutElement(ref txt, ' ', 0);
				CutElement(ref txt, ' ', 8);	//日付
				this.EntryName = txt;
			}

			/// <summary>
			/// 要素抜き出し
			/// </summary>
			/// <param name="src">文字列</param>
			/// <param name="delm">区切文字</param>
			/// <param name="start">開始インデックス</param>
			/// <returns>要素</returns>
			private string CutElement(ref string src, char delm, int start)
			{
				int endIdx = src.IndexOf(delm, start);
				//要素抜き出し
				string element = src.Substring(0, endIdx);
				//抜き出した要素を削除
				src = (src.Substring(endIdx)).Trim();
				return element;
			}

			#endregion
		}
		#endregion

		#region メンバ
		#endregion

		#region プロパティ
		/// <summary>
		/// ロガー
		/// </summary>
		public ILogger Log
		{
			get;
			set;
		}
		/// <summary>
		/// サーバ IP or ホスト名
		/// </summary>
		public string Host
		{
			private set;
			get;
		}
		/// <summary>
		/// 認証情報
		/// </summary>
		public NetworkCredential Credential
		{
			private set;
			get;
		}
		/// <summary>
		/// デフォルトパス(接続後、cdを行う)
		/// </summary>
		public string DefPath
		{
			private set;
			get;
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="host">サーバ IP or ホスト名</param>
		/// <param name="user">ユーザ</param>
		/// <param name="passWord">パスワード</param>
		/// <param name="defPath">デフォルトパス</param>
		public FtpControl(string host, string user, string passWord, string defPath)
		{
			this.Log = null;
			this.Host = host;
			this.Credential = new NetworkCredential(user, passWord);
			this.DefPath = defPath;
		}
		#endregion

		#region [private]メソッド
		#endregion

		#region [public]メソッド
		/// <summary>
		/// ディレクトリ内のファイル一覧取得
		/// </summary>
		/// <param name="subDir">サブディレクトリ</param>
		/// <param name="list">ファイル名のリスト</param>
		/// <returns>T:成功 F:失敗</returns>
		/// <remarks>ファイルとディレクトリの区別はできない
		/// 区別が必要な時、ListDirectoryDetailsを使用
		/// </remarks>
		public bool ListDir(string subDir, List<string> list)
		{
			if (this.Log != null) { this.Log.Start(); }

			string url = StrUtil.AppendUrl("ftp://" + this.Host, this.DefPath);
			url = StrUtil.AppendUrl(url, subDir);
			var request = (FtpWebRequest)WebRequest.Create(url);
			request.Credentials = this.Credential;
			//request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Method = WebRequestMethods.Ftp.ListDirectory;
			request.KeepAlive = true;	//KeepAlive = ON (WebClientに合わせる)
			request.UseBinary = true;
			request.UsePassive = false;

			try
			{
				if (this.Log != null) { this.Log.Info("FTP ListDirectory [{0}]", url); }

				using (var response = (FtpWebResponse)request.GetResponse())
				{
					using (var stream = new StreamReader(response.GetResponseStream()))
					{
						while (stream.Peek() >= 0)
						{
							//var info = new EntryInfo(stream.ReadLine());
							string file = stream.ReadLine();

							//ファイル前のパスを削除(Linux用)
							if (file.Contains("/") == true)
							{
								var split = file.Split('/');
								file = split[split.Length - 1];
							}
							
							list.Add(file);
						}
					}
				}

				if (this.Log != null) { this.Log.Info("ディレクトリ[{0}] ファイル件数[{1}]", url, list.Count); }

				return true;
			}
			catch (Exception ex)
			{
				if (this.Log != null) { this.Log.Error("ファイル一覧取得でエラー発生:" + ex); }
				return false;
			}
			finally
			{
				if (this.Log != null) { this.Log.End(); }
			}
		}
		/// <summary>
		/// GET
		/// </summary>
		/// <param name="subDir">サブディレクトリ</param>
		/// <param name="fileName">ファイ名</param>
		/// <param name="localPath">ローカルパス(フルパス)</param>
		/// <returns>T:成功 F:失敗</returns>
		public bool Get(string subDir, string fileName, string localPath)
		{
			if (this.Log != null) { this.Log.Start(); }

			string url = StrUtil.AppendUrl("ftp://" + this.Host, this.DefPath);
			url = StrUtil.AppendUrl(url, subDir);
			url = StrUtil.AppendUrl(url, fileName);

			try
			{
				if (this.Log != null) { this.Log.Info("FTP Get [{0}]", url); }

				using (var client = new WebClient())
				{
					client.Credentials = this.Credential;
					client.DownloadFile(url, localPath);
				}

				if (this.Log != null) { this.Log.Info("ファイルGET[{0}]→[{1}]", url, localPath); }
				return true;
			}
			catch(Exception ex)
			{
				if (this.Log != null) { this.Log.Error("ファイルGETでエラー発生:" + ex); }
				return false;
			}
			finally
			{
				if (this.Log != null) { this.Log.End(); }
			}
		}
		#endregion
	}
}
