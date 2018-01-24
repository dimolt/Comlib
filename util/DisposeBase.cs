using System;

namespace ComLib
{
	/// <summary>
	/// Disposeクラスのテンプレート
	/// </summary>
	public abstract class DisposeBase : IDisposable
	{
		#region メンバ変数
		/// <summary>
		/// 開放済みフラグ
		/// </summary>
		private bool vMisReleased = false;
		#endregion

		#region コンストラクタ・デストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DisposeBase()
		{
		}
		/// <summary>
		/// デストラクタ
		/// </summary>
		~DisposeBase()
		{
			//Unmanaged だけを解放する
			this.Dispose(false);
		}
		#endregion

		#region インターフェース実装
		/// <summary>
		/// Dispose() の実装
		/// </summary>
		public void Dispose()
		{
			//Unmanaged・Managed ともに解放
			this.Dispose(true);
			//GCへ指示
			GC.SuppressFinalize(this);
		}
		#endregion

		#region [private]メソッド
		/// <summary>
		/// 開放処理
		/// </summary>
		/// <param name="vIreleaseManaged">Managedリソース開放有無</param>
		private void Dispose(bool vIreleaseManaged)
		{
			if (vMisReleased == true)
			{
				//開放済
				return;
			}

			if (vIreleaseManaged)
			{
				//Managed リソース解放
				ReleaseUnManaged();
			}
			//UnManaged リソース解放
			ReleaseUnManaged();

			//開放完了
			vMisReleased = true;
		}
		#endregion

		#region [proteced]メソッド
		/// <summary>
		/// Managed リソース解放
		/// </summary>
		protected virtual void ReleaseManaged()
		{
		}
		/// <summary>
		/// UnManaged リソース解放
		/// </summary>
		protected abstract void ReleaseUnManaged();
		#endregion
	}
}
