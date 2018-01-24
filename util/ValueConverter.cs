using System.Collections.Generic;

namespace ComLib
{
	/// <summary>
	/// 値変換
	/// </summary>
	public class ValueConverter<T1, T2>
	{
		#region メンバ変数
		/// <summary>
		/// ディクショナリ
		/// </summary>
		private Dictionary<T1, T2> vMdic = new Dictionary<T1, T2>();
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ValueConverter()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">配列　元の値</param>
		/// <param name="value">配列　変換後の値</param>
		public ValueConverter(T1[] key, T2[] value)
		{
			if (key.Length != value.Length)
			{
				return;
			}

			for (int i = 0; i < key.Length; i++)
			{
				vMdic.Add(key[i], value[i]);
			}
		}
		#endregion

		#region [public]メソッド
		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="key">元の値</param>
		/// <param name="value">変換後の値</param>
		public void Add(T1 key, T2 value)
		{
			vMdic.Add(key, value);
		}
		/// <summary>
		/// 値変換
		/// </summary>
		/// <param name="key">元の値</param>
		/// <param name="value">変換後の値</param>
		/// <returns>変換結果</returns>
		public bool TryParse(T1 key, ref T2 value)
		{
			if (vMdic.ContainsKey(key) == true)
			{
				value = vMdic[key];
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
