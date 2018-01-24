using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ComLib
{
	/// <summary>
	/// オブジェクトのシリアライズ
	/// </summary>
	public static class Serializer
	{
		/// <summary>
		/// ファイルのエンコード
		/// </summary>
		private static readonly Encoding FILE_ENCODE = Encoding.GetEncoding("Shift_Jis");

		/// <summary>
		/// シリアライズしてファイルに保存
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="vIfile">保存先ファイルパス</param>
		/// <param name="vIobj">オブジェクト</param>
		/// <remarks>呼び元で例外をキャッチすること</remarks>
		public static void SaveToFile<T>(string vIfile, T vIobj) where T : class, new()
		{
			var vLserial = new XmlSerializer(typeof(T));
			//既存ファイルは上書き
			using (var sw = new StreamWriter(vIfile, false, FILE_ENCODE))
			{
				//シリアル化し、XMLファイルに保存する
				vLserial.Serialize(sw, vIobj);
			}
		}
		/// <summary>
		/// ファイルからデシリアライズ
		/// </summary>
		/// <typeparam name="T">オブジェクトの型</typeparam>
		/// <param name="vIfile">ファイルパス</param>
		/// <returns>オブジェクト</returns>
		/// <remarks>呼び元で例外をキャッチすること</remarks>
		public static T LoadFromFile<T>(string vIfile) where T : class, new()
		{
			var vLserial = new XmlSerializer(typeof(T));
			using (var reader = new StreamReader(vIfile, FILE_ENCODE))
			{
				return (T)vLserial.Deserialize(reader);
			}
		}
	}
}
