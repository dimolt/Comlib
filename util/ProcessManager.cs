using System;
using System.Diagnostics;
using System.Threading;

namespace ComLib
{
	/// <summary>
	/// プロセス管理
	/// </summary>
	public class ProcessManager
	{
		#region メンバー
		/// <summary>
		/// プロセスのインスタンス
		/// </summary>
		private Process vMproc;
		#endregion

		#region プロパティ
		public string ApName
		{
			get;
			private set;
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIname">プロセス名(exe名)</param>
		public ProcessManager(string vIname)
		{
			vMproc = new Process();
			vMproc.StartInfo.FileName = vIname;
			vMproc.StartInfo.UseShellExecute = true;
			this.ApName = vIname;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIname">プロセス名(exe名)</param>
		/// <param name="vIargs">起動時引数</param>
		public ProcessManager(string vIname, string vIargs)
		{
			vMproc = new Process();
			vMproc.StartInfo.FileName = vIname;
			vMproc.StartInfo.Arguments = vIargs;
			vMproc.StartInfo.UseShellExecute = true;
			this.ApName = vIname;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// プロセス起動
		/// </summary>
		public bool Start()
		{
			try
			{
				return vMproc.Start();
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// プロセス終了
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			try
			{
				//MainWindowを持たないAPにはKillを使用する
				return vMproc.CloseMainWindow();
			}
			catch
			{
				return false;
			}
		}
		/// <summary>
		/// プロセス終了監視
		/// </summary>
		/// <param name="vIendEvent">プロセス終了時イベント</param>
		public void ObserveEnd(Action<ProcessManager> vIendEvent)
		{
			//終了待ちスレッド開始
			Thread vLthread = new Thread(() =>
				{
					vMproc.WaitForInputIdle();
					//終了待ち
					vMproc.WaitForExit();

					if (vIendEvent != null)
					{
						vIendEvent(this);
					}
				}
			);
			vLthread.Start();
		}
		#endregion
	}
}
