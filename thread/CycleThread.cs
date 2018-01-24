using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
//
using ComLib.log;

namespace ComLib.thread
{
	/// <summary>
	/// 定周期処理実行用スレッド
	/// </summary>
	public class CycleThread : DisposeBase
	{
		#region メンバ
		/// <summary>
		/// ロガー
		/// </summary>
		private ILogger vMlog;
		/// <summary>
		/// スレッド名称
		/// </summary>
		private string vMname;
		/// <summary>
		/// スレッド
		/// </summary>
		private Thread vMthread;
		/// <summary>
		/// 定周期処理実行間隔(ms)
		/// </summary>
		private int vMinterval;
		/// <summary>
		/// 定周期処理
		/// </summary>
		private MethodInvoker vMaction = null;
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="vIinterval">定周期処理間隔(ms)</param>
		/// <param name="vIaction">定周期処理</param>
		/// <param name="vIlog">ロガー</param>
		public CycleThread(int vIinterval, MethodInvoker vIaction, ILogger vIlog)
		{
			vMinterval = vIinterval;
			vMaction = vIaction;
			vMlog = vIlog;
		}
		#endregion

		#region [public]メソッド
		/// <summary>
		/// スレッド開始
		/// </summary>
		public void Start(string threadName)
		{
			//スレッド開始
			vMthread = new Thread(MainLoop);
			vMthread.Start();
			vMname = threadName;
			vMlog.Info("[{0}]Thread start --->", vMname);
		}
		#endregion

		#region [protected]メソッド
		/// <summary>
		/// UnManaged リソース解放
		/// </summary>
		protected override void ReleaseUnManaged()
		{
			//スレッド停止
			this.Abort();
		}
		#endregion

		#region [private]メソッド
		/// <summary>
		/// スレッド停止
		/// </summary>
		private void Abort()
		{
			if (vMthread != null)
			{
				vMthread.Abort();
				vMthread = null;
				vMlog.Info("[{0}]Thread end <---", vMname);
			}
		}
		/// <summary>
		/// メインループ処理
		/// </summary>
		private void MainLoop()
		{
			//メインループ
			while (true)
			{
				try
				{
					//Sleep
					Thread.Sleep(vMinterval);

					if (vMaction != null)
					{
						vMaction();
					}
				}
				catch (ThreadAbortException)
				{
					vMlog.Info("定周期処理Thread abort");
					break;
				}
				catch (Exception ex)
				{
					vMlog.Error("定周期処理Thread エラー発生\n" + ex);
				}
			}
		}
		#endregion
	}
}
