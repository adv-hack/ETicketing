using System.Text;
using System.Web;
using TalentSystemDefaults;
namespace SystemDefaultsAPI
{
	namespace Models
	{
		public abstract class BaseFieldModel
		{
			public int Id { get; set; }
			public string Data { get; set; }
			private const string PATH = "~/Views/PartialViews/";
			private const string EXT = ".cshtml";
			public BaseFieldModel(int id, ModuleConfiguration mConfig, string viewName)
			{
				this.Id = id;
				this.mConfig = mConfig;
				this.ViewName = PATH + viewName + EXT;
				_currentValue = mConfig.ConfigurationItem.CurrentValue;
				_updatedValue = mConfig.ConfigurationItem.UpdatedValue;
				Hasher hasher = new Hasher();
				object[] parameters = new object[] { FieldType, TableName, DefaultKey1, FieldName, id };
				this.Data = hasher.HashFullString(string.Join(",", parameters));
				this.HashedCurrentValue = hasher.HashFullString(CurrentValue);
                PrepareValidationAttributes();
			}
			public string ViewName { get; set; }
			public ModuleConfiguration mConfig { get; set; }
			public string HashedCurrentValue { get; set; }
			private string _currentValue;
			public string CurrentValue
			{
				get
				{
					return _currentValue;
				}
				set
				{
					_currentValue = value;
				}
			}
			private string _updatedValue;
			public string UpdatedValue
			{
				get
				{
					return _updatedValue;
				}
				set
				{
					_updatedValue = value;
				}
			}
			public string MetaDataId
			{
				get
				{
					return "hf_" + FieldName;
				}
			}
			public string CurrentValueId
			{
				get
				{
					return "hf_" + FieldName + "_v";
				}
			}
			public string FieldType
			{
				get
				{
					return mConfig.FieldType.ToString();
				}
			}
			public string TableName
			{
				get
				{
					return mConfig.ConfigurationItem.TableName;
				}
			}
			public string DefaultKey1
			{
				get
				{
					return mConfig.ConfigurationItem.DefaultKey1;
				}
			}
			public string DisplayName
			{
				get
				{
					return mConfig.ConfigurationItem.DisplayName;
				}
			}
			public string FieldName
			{
				get
				{
					return mConfig.ConfigurationItem.DefaultName + mConfig.UniqueId;
				}
			}
			public string Description
			{
				get
				{
					return mConfig.ConfigurationItem.Description;
				}
			}
			public string AllowedPlaceHolders
			{
				get
				{
					//<<PlaceHolder>> encode the angular brackets, so it doesn't consider it as a Tag!
					return HttpUtility.HtmlEncode(mConfig.ConfigurationItem.AllowedPlaceHolders);
				}
			}
			public string ErrorMessage
			{
				get
				{
					return mConfig.ErrorMessage;
				}
			}
			public bool IsValid
			{
				get
				{
					return string.IsNullOrEmpty(mConfig.ErrorMessage);
				}
			}
			private StringBuilder __validationAttributes = new StringBuilder();
			private StringBuilder _validationAttributes
			{
				get
				{
					return __validationAttributes;
				}
				set
				{
					__validationAttributes = value;
				}
			}
			public string ValidationAttributes
			{
				get
				{
					return _validationAttributes.ToString();
				}
			}
			private void PrepareValidationAttributes()
			{
                if (!(mConfig.ValidationGroup == null))
                {
                    string hasMandatory = (mConfig.ValidationGroup.HasMandatory ? "1" : "0");
                    _validationAttributes.Append("hasRFV=\"" + hasMandatory + "\"");
                    if (mConfig.ValidationGroup.HasRegularExp)
                    {
                        _validationAttributes.Append(GetAttribute("hasRE", "1"));
                        _validationAttributes.Append(GetAttribute("RE", mConfig.ValidationGroup.RegularExp));
                    }
                    else 
                    {
                        if (mConfig.ValidationGroup.HasNumeric)
                        {
                            string hasNumeric = (mConfig.ValidationGroup.HasNumeric ? "1" : "0");
                            _validationAttributes.Append("hasNum=\"" + hasNumeric + "\"");

                            if (mConfig.ValidationGroup.HasMinValue)
                            {
                                _validationAttributes.Append(GetAttribute("hasMin", "1"));
                                _validationAttributes.Append(GetAttribute("Min", mConfig.ValidationGroup.MinValue.ToString()));
                            }
                            if (mConfig.ValidationGroup.HasMaxValue)
                            {
                                _validationAttributes.Append(GetAttribute("hasMax", "1"));
                                _validationAttributes.Append(GetAttribute("Max", mConfig.ValidationGroup.MaxValue.ToString()));
                            }
                        }
                        else
                        {
                            if (mConfig.ValidationGroup.HasMinLength)
                            {
                                _validationAttributes.Append(GetAttribute("hasMinL", "1"));
                                _validationAttributes.Append(GetAttribute("MinL", mConfig.ValidationGroup.MinLength.ToString()));
                            }
                            if (mConfig.ValidationGroup.HasMaxLength)
                            {
                                _validationAttributes.Append(GetAttribute("hasMaxL", "1"));
                                _validationAttributes.Append(GetAttribute("MaxL", mConfig.ValidationGroup.MaxLength.ToString()));
                            }
                        }   
                    }
                }
			}
			private string GetAttribute(string attributeName, string attributeValue)
			{
                var objAttribute = "{0}=\"{1}\" ";
                return string.Format(objAttribute,attributeName, attributeValue);
			}
		}
	}
}
