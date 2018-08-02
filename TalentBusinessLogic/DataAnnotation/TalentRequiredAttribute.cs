using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TalentBusinessLogic.DataAnnotation.Base;

namespace TalentBusinessLogic.DataAnnotation
{
    /// <summary>
    /// This and other such classes provide the extended behaviour to
    /// data validation attributes. The advantage of having such class is,
    /// it enables you to mantain your configurations and texts within the database.
    /// </summary>
    public class TalentRequiredAttribute : TalentValidationAttribute
    {
        #region Class Level Fields
        
        private const string RequiredField = "RequiredField";
        private const string RequiredFieldMessage = "RequiredFieldMessage";
        private const string InternalErrorMessage = "The {0} field is required";

        private bool defaultIsRequired;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ErrorMessage">Default error message</param>
        /// <param name="isRequired">Default value for required flag</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="OverrideFromDB">Flag indicates whether or not 
        /// override the attribute from database</param>
        public TalentRequiredAttribute(string ErrorMessage = null, bool IsRequired = true, string PropertyName = "", bool OverrideFromDB = true)
            : base(ErrorMessage, PropertyName, OverrideFromDB)
        {
            this.defaultIsRequired = IsRequired;
            this.internalErrorMessage = InternalErrorMessage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property that provides accessor to is-required flag
        /// </summary>
        public bool? IsRequired { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method determines whether the field is valid or not.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        /// <param name="value"></param>
        /// <returns>Returns true if valid, false otherwise</returns>
        public override bool IsValid(string modelType, string displayName, object value)
        {
            bool retval = true;

            if (overrideFromDB)
                SetRequiredFlag(modelType);
            else
                IsRequired = true;

            if (IsRequired == true)
            {
                SetErrorMessage(modelType, RequiredFieldMessage, displayName);

                if (value == null)
                {
                    retval = false;
                }
                else
                {
                    var stringValue = value as string;
                    retval = (stringValue.Trim().Length != 0);
                }
            }

            return retval;
        }

        /// <summary>
        /// This method is to enable the client side validation.
        /// This method is called before the page is loaded.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rules = Enumerable.Empty<ModelClientValidationRule>();
            
            //Retrieve property name via reflection as it's not contained in the meta data or the control context
            Type modelType = metadata.ContainerType;
            string displayName = metadata.GetDisplayName();
            SetPropertyName(modelType, displayName);

            SetRequiredFlag(modelType.Name);
            if (IsRequired == true)
            {
                rules = GetRequiredRule(modelType.Name, defaultIsRequired, displayName);
            }
            return rules;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates the validation rule for jQuery Required Field Validation
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        private IEnumerable<ModelClientValidationRule> GetRequiredRule(string modelType, bool isRequired, string displayName)
        {
            var rules = new List<ModelClientValidationRule>();
            var rule = new ModelClientValidationRule();
            rule.ValidationParameters.Add("flag", isRequired.ToString().ToLower());

            SetErrorMessage(modelType, RequiredFieldMessage, displayName);
            rule.ErrorMessage = ErrorMessage;
            rule.ValidationType = "required";
            rules.Add(rule);
            return rules;
        }

        /// <summary>
        /// Invokes the database and determines whether the model property is mandatory
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        private void SetRequiredFlag(string modelType)
        {
            if (IsRequired == null)
            {
                var attributeName = _propertyName + RequiredField;
                var attributeValue = Convert.ToString(defaultIsRequired);
                IsRequired = validator.GetBooleanAttribute(modelType, attributeName, attributeValue);
            }
        }

        #endregion
    }
}