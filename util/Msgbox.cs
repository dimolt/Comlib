using System;
using System.Reflection;
using System.Windows.Forms;

namespace ComLib
{
	/// <summary>
	/// アプリケーション共通メッセージクラス
	/// </summary>
	public static class Msgbox
	{
		/// <summary>
		/// 情報メッセージボックスの表示
		/// </summary>
		/// <param name="prf">親フォーム</param>
		/// <param name="ms">表示メッセージ</param>
		public static void Info( Form prf, string ms  ){
			MessageBox.Show(prf,ms,"情報",
					MessageBoxButtons.OK,
					MessageBoxIcon.Information);
		}
		/// <summary>
		/// 情報メッセージボックスの表示
		/// </summary>
		/// <param name="ms">表示メッセージ</param>
		public static void Info( string ms){
			MessageBox.Show(ms,"情報",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		/// <summary>
		/// 警告メッセージボックスの表示
		/// </summary>
		/// <param name="prf">親フォーム</param>
		/// <param name="ms">表示メッセージ</param>
		public static void Warning( Form prf, string ms ){
			MessageBox.Show(prf,ms,"警告",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}
		/// <summary>
		/// 警告メッセージボックスの表示
		/// </summary>
		/// <param name="ms">表示メッセージ</param>
		public static void Warning( string ms ){
			MessageBox.Show(ms,"警告",
				MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}

		/// <summary>
		/// エラーメッセージボックスの表示
		/// </summary>
		/// <param name="prf">親フォーム</param>
		/// <param name="ms">表示メッセージ</param>
		public static void Error( Form prf, string ms ){
			MessageBox.Show(prf,ms,"エラー",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
		/// <summary>
		/// エラーメッセージボックスの表示
		/// </summary>
		/// <param name="ms">表示メッセージ</param>
		public static void Error( string ms ){
			MessageBox.Show(ms, "エラー",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}

		/// <summary>
		/// 質問メッセージボックスの表示
		/// </summary>
		/// <param name="prf">親フォーム</param>
		/// <param name="ms">表示メッセージ</param>
		/// <returns>結果</returns>
		public static DialogResult Question(Form prf, string ms)
		{
			return MessageBox.Show(
					prf,ms,"質問",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1
				);
		}
		/// <summary>
		/// 質問メッセージボックスの表示
		/// </summary>
		/// <param name="ms">表示メッセージ</param>
		/// <returns>結果</returns>
		public static DialogResult Question( string ms ){
			return MessageBox.Show(
					ms, "質問",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.Question,
					MessageBoxDefaultButton.Button1
				);
		}

	}
}
