using System.Collections.Generic;
using System.Web.Mvc;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public class DropdownFieldModel : BaseFieldModel
		{
			public const string VIEW = "_DropDownField";
			public List<SelectListItem> Items { get; set; }
			public DropdownFieldModel(int id, ModuleConfiguration mConfig)
				: base(id, mConfig, VIEW)
			{
				Items = new List<SelectListItem>();
				BindItems();
			}
			private void BindItems()
			{
				if (mConfig.ConfigurationItem.AllowedValues != null)
				{
					foreach (var value in mConfig.ConfigurationItem.AllowedValues)
					{
						bool selected = false;
						if (value == UpdatedValue)
						{
							selected = true;
						}
						Items.Add(new SelectListItem { Text = value, Value = value, Selected = selected });
					}
				}
			}
		}
	}
}
