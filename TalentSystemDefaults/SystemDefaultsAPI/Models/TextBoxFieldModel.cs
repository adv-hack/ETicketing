using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class TextBoxFieldModel : BaseTextFieldModel
		{
			public const string VIEW = "_TextBoxField";
			public TextBoxFieldModel(int id, ModuleConfiguration mConfig)
				: base(id, mConfig, VIEW)
			{
			}
		}
	}
}
