using System;
using System.Data;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Provides the properties to build sql paramater object
	/// </summary>
	[Serializable()]
	public class DESQLParameter
	{
		#region Class Level Fields
		/// <summary>
		/// Paramater Name
		/// </summary>
		private string _paramName;
		/// <summary>
		/// Parameter Value
		/// </summary>
		private object _paramValue;
		/// <summary>
		/// Parameter Type
		/// </summary>
		private SqlDbType _paramType;
		/// <summary>
		/// Parameter Direction
		/// </summary>
		private ParameterDirection _paramDirection;
		#endregion
		#region Properties
		/// <summary>
		/// Gets or sets the name of the parameter
		/// </summary>
		/// <value>The name of the param.</value>
		public string ParamName
		{
			get
			{
				return _paramName;
			}
			set
			{
				_paramName = value;
			}
		}
		/// <summary>
		/// Gets or sets the parameter value
		/// </summary>
		/// <value>The param value.</value>
		public dynamic ParamValue
		{
			get
			{
				return _paramValue;
			}
			set
			{
				_paramValue = value;
			}
		}
		/// <summary>
		/// Gets or sets the type of the parameter (SqlDbType)
		/// </summary>
		/// <value>The type of the param.</value>
		public SqlDbType ParamType
		{
			get
			{
				return _paramType;
			}
			set
			{
				_paramType = value;
			}
		}
		/// <summary>
		/// Gets or sets the parameter direction of type Data.ParameterDirection
		/// </summary>
		/// <value>The param direction.</value>
		public ParameterDirection ParamDirection
		{
			get
			{
				return _paramDirection;
			}
			set
			{
				_paramDirection = value;
			}
		}
		#endregion
	}
}
