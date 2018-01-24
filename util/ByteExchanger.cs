using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ComLib
{
	/// <summary>
	/// オブジェクト⇔バイト変換
	/// </summary>
	public static class ByteExchanger
	{
		/// <summary>
		/// オブジェクト→バイト変換
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="vIobj">オブジェクト</param>
		/// <returns>バイト配列</returns>
		public static byte[] Serialize<T>(T vIobj) where T : class, new()
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();

				formatter.Serialize(stream, vIobj);

				return stream.ToArray();
			}
		}

		/// <summary>
		/// オブジェクト←バイト変換
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="bytes">バイト配列</param>
		/// <param name="vIsize">バイト数</param>
		/// <returns>オブジェクト</returns>
		public static T DeSerialize<T>(byte[] bytes, int vIsize) where T : class, new()
		{
			using (MemoryStream stream = new MemoryStream(vIsize))
			{
				stream.Write(bytes, 0, vIsize);
				stream.Seek(0, SeekOrigin.Begin);
				BinaryFormatter vLformatter = new BinaryFormatter();

				return (T)vLformatter.Deserialize(stream);
			}

		}

		/// <summary>
		/// オブジェクト変換時のバイト数取得
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <returns>バイト数</returns>
		public static int Size<T>() where T : class, new()
		{
			//バイト数を求める
			T vLobj = new T();
			return Serialize(vLobj).Length;
		}
	}
}

