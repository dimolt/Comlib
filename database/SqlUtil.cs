using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OracleClient;

namespace ComLib.database
{
	/// <summary>
	/// SQL�p�����񑀍�N���X
	/// </summary>
	public static class SqlUtil
	{
		#region SQL�p�����񑀍�
		/// <summary>
		/// �p�����[�^���擾
		/// </summary>
		/// <param name="column_name">�J������</param>
		/// <param name="value">�ݒ�l</param>
		/// <returns>�ݒ�l:NULL�ȊO��":"�{�J������ NULL��NULL</returns>
		public static string ParamName(string column_name, string value)
		{
			string result = "NULL";
			if (value.Length > 0)
			{
				result = ":" + column_name;
			}
			return result;
		}
		#endregion

		#region �p�����[�^�ݒ�
		/// <summary>
		/// �p�����[�^�ɒl�ݒ�(Int)
		/// </summary>
		/// <param name="vIprm">�p�����[�^</param>
		/// <param name="vIval">�l</param>
		public static void SetPrm(OracleParameter vIprm, int vIval)
		{
			if (vIval == int.MinValue)
			{
				vIprm.Value = DBNull.Value;
			}
			else
			{
				vIprm.Value = vIval;
			}
		}
		/// <summary>
		/// �p�����[�^�ɒl�ݒ�(decimal)
		/// </summary>
		/// <param name="vIprm">�p�����[�^</param>
		/// <param name="vIval">�l</param>
		public static void SetPrm(OracleParameter vIprm, decimal vIval)
		{
			if (vIval == decimal.MinValue)
			{
				vIprm.Value = vIprm.Value = DBNull.Value;
			}
			else
			{
				vIprm.Value = vIval;
			}
		}
		/// <summary>
		/// �p�����[�^�ɒl�ݒ�(datetime)
		/// </summary>
		/// <param name="vIprm">�p�����[�^</param>
		/// <param name="vIval">�l</param>
		public static void SetPrm(OracleParameter vIprm, DateTime vIval)
		{
			if (vIval == DateTime.MinValue)
			{
				vIprm.Value = DBNull.Value;
			}
			else
			{
				vIprm.Value = vIval;
			}
		}
		#endregion
	}
}
