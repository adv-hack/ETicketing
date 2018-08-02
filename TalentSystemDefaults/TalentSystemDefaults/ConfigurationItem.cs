using System.Collections.Generic;
namespace TalentSystemDefaults
{
	public class ConfigurationItem
	{
		private string _TableName = string.Empty;
		public string TableName
		{
			get
			{
				return _TableName;
			}
			set
			{
				_TableName = value;
			}
		}
		private string _DefaultKey1 = string.Empty;
		public string DefaultKey1
		{
			get
			{
				return _DefaultKey1;
			}
			set
			{
				_DefaultKey1 = value;
			}
		}
		private string _DefaultKey2 = string.Empty;
		public string DefaultKey2
		{
			get
			{
				return _DefaultKey2;
			}
			set
			{
				_DefaultKey2 = value;
			}
		}
		private string _DefaultKey3 = string.Empty;
		public string DefaultKey3
		{
			get
			{
				return _DefaultKey3;
			}
			set
			{
				_DefaultKey3 = value;
			}
		}
		private string _DefaultKey4 = string.Empty;
		public string DefaultKey4
		{
			get
			{
				return _DefaultKey4;
			}
			set
			{
				_DefaultKey4 = value;
			}
		}
		private string _VariableKey1 = string.Empty;
		public string VariableKey1
		{
			get
			{
				return _VariableKey1;
			}
			set
			{
				_VariableKey1 = value;
			}
		}
		private string _VariableKey2 = string.Empty;
		public string VariableKey2
		{
			get
			{
				return _VariableKey2;
			}
			set
			{
				_VariableKey2 = value;
			}
		}
		private string _VariableKey3 = string.Empty;
		public string VariableKey3
		{
			get
			{
				return _VariableKey3;
			}
			set
			{
				_VariableKey3 = value;
			}
		}
		private string _VariableKey4 = string.Empty;
		public string VariableKey4
		{
			get
			{
				return _VariableKey4;
			}
			set
			{
				_VariableKey4 = value;
			}
		}
		private string _DisplayName = string.Empty;
		public string DisplayName
		{
			get
			{
				return _DisplayName;
			}
			set
			{
				_DisplayName = value;
			}
		}
		private string _DefaultName = string.Empty;
		public string DefaultName
		{
			get
			{
				return _DefaultName;
			}
			set
			{
				_DefaultName = value;
			}
		}
		private string _DefaultValue = string.Empty;
		public string DefaultValue
		{
			get
			{
				return _DefaultValue;
			}
			set
			{
				_DefaultValue = value;
			}
		}
		private string _CurrentValue = string.Empty;
		public string CurrentValue
		{
			get
			{
				return _CurrentValue;
			}
			set
			{
				_CurrentValue = value;
			}
		}
		private string _UpdatedValue = string.Empty;
		public string UpdatedValue
		{
			get
			{
				return _UpdatedValue;
			}
			set
			{
				_UpdatedValue = value;
			}
		}
		private string _Description = string.Empty;
		public string Description
		{
			get
			{
				return _Description;
			}
			set
			{
				_Description = value;
			}
		}
		private List<string> _AllowedValues = null;
		public List<string> AllowedValues
		{
			get
			{
				return _AllowedValues;
			}
			set
			{
				_AllowedValues = value;
			}
		}
		private string _AllowedPlaceHolders = null;
		public string AllowedPlaceHolders
		{
			get
			{
				return _AllowedPlaceHolders;
			}
			set
			{
				_AllowedPlaceHolders = value;
			}
		}
		private string _GuidValue = string.Empty;
		public string GuidValue
		{
			get
			{
				return _GuidValue;
			}
			set
			{
				_GuidValue = value;
			}
		}
	}
}
