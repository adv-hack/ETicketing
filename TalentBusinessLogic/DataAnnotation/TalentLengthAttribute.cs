using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.DataAnnotation.Base;

namespace TalentBusinessLogic.DataAnnotation
{
    /// <summary>
    /// This class extends the behavior of the length attribute to retrieve the attribute
    /// and text values from the database
    /// </summary>
    public class TalentLengthAttribute : TalentValidationAttribute
    {
        #region Class Level Fields

        private const string InvalidLengthMessage = "InvalidLengthMessage";
        private const string InternalErrorMessageMinLength = "The field {0} must be a string with a minimum length of {1}";
        private const string InternalErrorMessageMaxLength = "The field {0} must be a string with a maximum length of {2}";
        private const string InternalErrorMessageMinMaxLength = "The field {0} must be a string with a minimum length of {1} and a maximum length of {2}";
 
        private int defaultMinLength, defaultMaxLength;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="MinLength">Default minimum length</param>
        /// <param name="MaxLength">Default maximum length</param>
        /// <param name="ErrorMessage">Default error messae</param>
        /// <param name="PropertyName">Property name</param>
        /// <param name="OverrideFromDB">Flag indicates whether or not 
        /// override the attribute from database</param>
        public TalentLengthAttribute(int MinLength = -1, int MaxLength = -1, string ErrorMessage = null, string PropertyName = "", bool OverrideFromDB = true)
            : base(ErrorMessage, PropertyName, OverrideFromDB)
        {
            this.defaultMinLength = MinLength;
            this.defaultMaxLength = MaxLength;
            this.internalErrorMessage = (MinLength != -1 && MaxLength != -1)
                                        ? InternalErrorMessageMinMaxLength
                                        : (MinLength != -1)
                                            ? InternalErrorMessageMinLength
                                            : InternalErrorMessageMaxLength;
        }

        #endregion

        #region Public Properties
        
        public int? MinLength { get; private set; }
        public int? MaxLength { get; private set; }

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
                SetLengthValues(modelType);
            else
            {
                MinLength = defaultMinLength;
                MaxLength = defaultMaxLength;
            }

            SetErrorMessage(modelType, InvalidLengthMessage, displayName, MinLength, MaxLength);

            int valueLength = 0;
            if (value != null)
            { 
                valueLength = value.ToString().Trim().Length;
            }

            if (MinLength != -1)
            {
                retval &= (valueLength >= MinLength);
            }
            if (MaxLength != -1)
            {
                retval &= (valueLength <= MaxLength);
            }
            
            return retval;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves the length values from the database and sets them into local fields
        /// </summary>
        /// <param name="modelType"></param>
        private void SetLengthValues(string modelType)
        {
            if (MinLength == null)
            {
                var attributeName = _propertyName + "MinLength";
                bool enableInsert = (defaultMinLength != -1);
                string value = validator.GetAttribute(modelType, attributeName, this.defaultMinLength.ToString(), enableInsert);
                MinLength = Convert.ToInt32(value);
            }

            if (MaxLength == null)
            {
                var attributeName = _propertyName + "MaxLength";
                bool enableInsert = (defaultMaxLength != -1);
                string value = validator.GetAttribute(modelType, attributeName, this.defaultMaxLength.ToString(), enableInsert);
                MaxLength = Convert.ToInt32(value);
            }
        }

        #endregion
    }
}
