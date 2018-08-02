using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TalentBusinessLogic.DataAnnotation.Base;

namespace TalentBusinessLogic.DataAnnotation
{
    /// <summary>
    /// This class extends the behavior of the regular expression attribute to retrieve the attribute
    /// and text values from the database
    /// </summary>
    class TalentRegularExpressionAttribute : TalentValidationAttribute
    {
        #region Class Level Fields

        private const string InvalidContentMessage = "InvalidContentMessage";
        private const string InternalErrorMessage = "The field {0} has invalid value";
        private string defaultPattern;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Pattern">Default pattern</param>
        /// <param name="ErrorMessage">Default error message</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <param name="OverrideFromDB">Flag indicates whether or not 
        /// override the attribute from database</param>
        public TalentRegularExpressionAttribute(string Pattern, string ErrorMessage = null, string PropertyName = "", bool OverrideFromDB = true) : base(ErrorMessage, PropertyName, OverrideFromDB)
        {
            this.defaultPattern = Pattern;
            this.internalErrorMessage = InternalErrorMessage;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Regular expression pattern to perform validation against
        /// </summary>
        public string Pattern { get; private set; }

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
            SetErrorMessage(modelType, InvalidContentMessage, displayName);
            if (overrideFromDB)
                SetPattern(modelType);
            else
                Pattern = defaultPattern;

            if (value == null)
                return true;

            string str = (string)value;
            Regex regex = new Regex(Pattern);
            return regex.IsMatch(str);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves the pattern value from the database and sets it into local field
        /// </summary>
        /// <param name="modelType"></param>
        private void SetPattern(string modelType)
        {
            if (String.IsNullOrEmpty(Pattern))
            {
                var attributeName = _propertyName + "Pattern";
                Pattern = validator.GetAttribute(modelType, attributeName, this.defaultPattern);
            }
        }

        #endregion
    }
}
