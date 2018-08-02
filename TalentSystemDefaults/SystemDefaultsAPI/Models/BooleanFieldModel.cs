using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class BooleanFieldModel : BaseFieldModel
		{
			public const string VIEW = "_BooleanField";
			public BooleanFieldModel(int id, ModuleConfiguration mConfig)
				: base(id, mConfig, VIEW)
			{
				_currentValue = mConfig.ConfigurationItem.CurrentValue;
			}
			private string _currentValue;
			new public bool CurrentValue
			{
				get
				{
					string value = _currentValue.ToLower();
					return value == "true" || value == "y" || value == "1" ? true : false;
				}
				set
				{
					_currentValue = value.ToString();
				}
			}
		}
	}
}
