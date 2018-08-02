namespace SystemDefaultsAPI
{
	namespace UtilityClasses
	{
		public class TextSearchUpdateData
		{
			private string _id = string.Empty;
			public string id
			{
				get
				{
					return _id;
				}
				set
				{
					_id = value;
				}
			}
			private string _dbTable = string.Empty;
			public string dbTable
			{
				get
				{
					return _dbTable;
				}
				set
				{
					_dbTable = value;
				}
			}
			private string _text_code = string.Empty;
			public string text_code
			{
				get
				{
					return _text_code;
				}
				set
				{
					_text_code = value;
				}
			}
			private string _text_content = string.Empty;
			public string text_content
			{
				get
				{
					return _text_content;
				}
				set
				{
					_text_content = value;
				}
			}
		}
	}
}
