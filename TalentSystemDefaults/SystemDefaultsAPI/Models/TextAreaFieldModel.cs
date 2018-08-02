using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class TextAreaFieldModel : BaseTextFieldModel
		{
			public const string VIEW = "_TextAreaField";
			public TextAreaFieldModel(int id, ModuleConfiguration mConfig)
				: base(id, mConfig, VIEW)
			{
			}
		}
	}
}
