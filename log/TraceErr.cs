using System;
using System.Collections.Generic;
using System.Text;

namespace ComLib.log
{
	/// <summary>
	/// エラーLog出力 クラス
	/// </summary>
	public static class TraceErr
	{
		/// <summary>
		/// 例外詳細情報をLog出力
		/// </summary>
		/// <param name="IpLog">ロガー</param>
		/// <param name="IpExp">例外</param>
		public static void Fatal(ILogger IpLog, Exception IpExp)
		{
			string LvMsg = "[例外情報]-------------------------------------\n" + IpExp;
			IpLog.Fatal(LvMsg);

			//内部例外を出力
			Exception LvErr = null;
			while (IpExp.InnerException != null)
			{
				//無限ループ回避
				if (LvErr == IpExp.InnerException) { return; }
				LvErr = IpExp.InnerException;

				LvMsg = "[内部例外]-------------------------------------\n" + IpExp.InnerException;
				IpLog.Fatal(LvMsg);
			}
		}
	}
}
