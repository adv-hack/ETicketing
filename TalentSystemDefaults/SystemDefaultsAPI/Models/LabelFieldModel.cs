using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class LabelFieldModel : BaseFieldModel
		{
			public const string VIEW = "_LabelField";
			public LabelFieldModel(int id, ModuleConfiguration mConfig)
				: base(id, mConfig, VIEW)
			{
			}
		}
	}
}
