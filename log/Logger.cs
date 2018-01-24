
namespace ComLib.log
{
	/// <summary>
	/// Log出力 インターフェース
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// メソッド開始
		/// </summary>
		void Start();
		/// <summary>
		/// メソッド終了
		/// </summary>
		void End();

		/// <summary>
		/// 出力先取得
		/// </summary>
		/// <returns>出力先パス</returns>
		string FilePath();

		/// <summary>
		/// Log出力 Debug
		/// </summary>
		/// <param name="msg">ログ内容</param>
		void Debug(object msg);
		/// <summary>
		/// Log出力 Info
		/// </summary>
		/// <param name="msg">ログ内容</param>
		void Info(object msg);
		/// <summary>
		/// Log出力 Warn
		/// </summary>
		/// <param name="msg">ログ内容</param>
		void Warn(object msg);
		/// <summary>
		/// Log出力 Error
		/// </summary>
		/// <param name="msg">ログ内容</param>
		void Error(object msg);
		/// <summary>
		/// Log出力 Fatal
		/// </summary>
		/// <param name="msg">ログ内容</param>
		void Fatal(object msg);

		/// <summary>
		/// Log出力(フォーマット付) Debug
		/// </summary>
		/// <param name="msg">ログ内容</param>
		/// <param name="args">パラメータ</param>
		void Debug(string msg, params object[] args);
		/// <summary>
		/// Log出力(フォーマット付) Info
		/// </summary>
		/// <param name="msg">ログ内容</param>
		/// <param name="args">パラメータ</param>
		void Info(string msg, params object[] args);
		/// <summary>
		/// Log出力(フォーマット付) Warn
		/// </summary>
		/// <param name="msg">ログ内容</param>
		/// <param name="args">パラメータ</param>
		void Warn(string msg, params object[] args);
		/// <summary>
		/// Log出力(フォーマット付) Error
		/// </summary>
		/// <param name="msg">ログ内容</param>
		/// <param name="args">パラメータ</param>
		void Error(string msg, params object[] args);
		/// <summary>
		/// Log出力(フォーマット付) Fatal
		/// </summary>
		/// <param name="msg">ログ内容</param>
		/// <param name="args">パラメータ</param>
		void Fatal(string msg, params object[] args);
	}
}
