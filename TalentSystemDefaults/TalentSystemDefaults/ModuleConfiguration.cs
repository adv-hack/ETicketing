namespace TalentSystemDefaults
{
	public class ModuleConfiguration
	{
		private string _ConfigID = string.Empty;
		public string ConfigID
		{
			get
			{
				return _ConfigID;
			}
			set
			{
				_ConfigID = value;
			}
		}
		private DataType _FieldType = DataType.TEXT;
		public DataType FieldType
		{
			get
			{
				return _FieldType;
			}
			set
			{
				_FieldType = value;
			}
		}
		public string TabHeaderKey { get; set; }
		public string TabHeader { get; set; }
		private ValidationGroup _ValidationGroup = null;
		public ValidationGroup ValidationGroup
		{
			get
			{
				return _ValidationGroup;
			}
			set
			{
				_ValidationGroup = value;
			}
		}
		private string _ErrorMessage = null;
		public string ErrorMessage
		{
			get
			{
				return _ErrorMessage;
			}
			set
			{
				_ErrorMessage = value;
			}
		}
		private string _AccordionGroup = null;
		public string AccordionGroup
		{
			get
			{
				return _AccordionGroup;
			}
			set
			{
				_AccordionGroup = value;
			}
		}
		private string _UniqueId = string.Empty;
		public string UniqueId
		{
			get
			{
				return _UniqueId;
			}
			set
			{
				_UniqueId = value;
			}
		}
		private ConfigurationItem _ConfigurationItem = null;
		public ConfigurationItem ConfigurationItem
		{
			get
			{
				return _ConfigurationItem;
			}
			set
			{
				_ConfigurationItem = value;
			}
		}
		public bool HasError
		{
			get
			{
				return ErrorMessage != string.Empty;
			}
		}
		public ModuleConfiguration()
		{
		}
	}
}
