using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class BaseTextFieldModel : BaseFieldModel
		{
			public BaseTextFieldModel(int id, ModuleConfiguration mConfig, string viewName)
				: base(id, mConfig, viewName)
			{
			}
		}
	}
}
