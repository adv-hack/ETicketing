using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

//IClientValidatable: is from System.Web.Mvc.dll
//ModelClientValidationRule: is from System.Web.WebPages.dll
//These dlls are added into the ThirdPartyLibraries directory
using System.Web.Mvc;
using TalentBusinessLogic.Models;

namespace TalentBusinessLogic.DataAnnotation.Base
{
    /// <summary>
    /// This is class serves as a base class to all the custom validation attributes available
    /// in the business logic project
    /// </summary>
    public abstract class TalentValidationAttribute : ValidationAttribute, IClientValidatable
    {
        #region Protected Fields
        
        protected string _errorMessage, _propertyName;
        protected TalentDataValidator validator;
        protected string defaultErrorMessage;
        protected string internalErrorMessage;
        protected bool overrideFromDB;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ErrorMessage">Default error message</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <param name="OverrideFromDB">Flag indicates whether or not 
        /// override the attribute from database</param>
        public TalentValidationAttribute(string ErrorMessage, string PropertyName, bool OverrideFromDB)
        {
            this.defaultErrorMessage = ErrorMessage;
            this._propertyName = PropertyName;
            this.overrideFromDB = OverrideFromDB;
            validator = new TalentDataValidator();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property is the public accessor to the error message.
        /// This hides the ErrorMessage property from the base class.
        /// </summary>
        public new string ErrorMessage
        {
            get
            {
                if (String.IsNullOrEmpty(_errorMessage))
                {
                    //null-coalescing: it moves to next if one is null

                    //defaultErrorMessage: Its a message supplied by the model
                    //internalErrorMessage: Its a message supplied by the validation attribute
                    //ErrorMessageString: Its a message supplied by the data annotation validation framework
                    return (defaultErrorMessage ?? internalErrorMessage ?? ErrorMessageString);
                }

                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method is called when the validation has been performed on the property of the model.
        /// This works more of a wrapper method and takes the responsibility to invoke validation
        /// on currently executing validation attribute, creates the ValidationResult and returns it.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns>ValidationResult</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (BaseInputModel)validationContext.ObjectInstance;
            string modelType = model.GetType().Name;

            SetDisplayName(validationContext);
            SetPropertyName(model.GetType(), validationContext.DisplayName);

            ValidationResult result = ValidationResult.Success;

            bool exclude = false;
            if (this is TalentRequiredAttribute)
                exclude = model.ExcludeForRequiredFieldValidation != null && model.ExcludeForRequiredFieldValidation.Contains(_propertyName);

            if (!exclude && !IsValid(modelType, validationContext.DisplayName, value))
            {
                //string[] memberNames = validationContext.MemberName != null ? new string[] { validationContext.MemberName } : null;
                result = new ValidationResult(ErrorMessage);
            }
            return result;
        }

        /// <summary>
        /// SetDisplayName:
        /// Manually sets the display name, because
        /// Web API ModelState validation is ignoring the DisplayAttribute.
        /// 
        /// Here is the description of the known issue:
        /// http://aspnetwebstack.codeplex.com/workitem/744
        /// </summary>
        /// <param name="validationContext"></param>
        protected void SetDisplayName(ValidationContext validationContext)
        {
            ModelMetadata metadata;
            try
            {
                metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, validationContext.ObjectType, validationContext.DisplayName);
            }
            catch (ArgumentException)
            {
                //This exception occurs when the validationContext.DisplayName has already been set with the value from the database.
                //E.g. TalentRequired has already populated the value, so it will fail when TalentRegularExpression attempts to do so.
                //ExceptionMessage: TalentBusinessLogic.Models.DataAnnotationInputModel.Prénom could not be found
                metadata = null;
            }

            if (metadata != null && metadata.DisplayName != null)
            {
                validationContext.DisplayName = metadata.DisplayName;
            }
        }

        /// <summary>
        /// SetPropertyName:
        /// This method uses the display name and retrieves the property name through reflection
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        protected void SetPropertyName(Type modelType, string displayName)
        {
            if (String.IsNullOrEmpty(_propertyName))
            {
                _propertyName = validator.GetPropertyName(modelType, displayName);
            }
        }

        /// <summary>
        /// SetErrorMessage:
        /// Sets the error message with the value retrieved from the DB
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="suffix"></param>
        /// <param name="args"></param>
        protected void SetErrorMessage(string modelType, string suffix, params object[] args)
        {
            if (String.IsNullOrEmpty(_errorMessage))
            {
                var requiredFieldKey = _propertyName + suffix;
                ErrorMessage = validator.GetText(modelType, requiredFieldKey, ErrorMessage);
                ErrorMessage = String.Format(ErrorMessage, args);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method determines whether the field is valid or not.
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="displayName"></param>
        /// <param name="value"></param>
        /// <returns>Returns true if valid, false otherwise</returns>
        public abstract bool IsValid(string modelType, string displayName, object value);

        /// <summary>
        /// This method is to enable the client side validation.
        /// This method is called before the page is loaded.
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return Enumerable.Empty<ModelClientValidationRule>();
        }

        #endregion
    }
}
