using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.DataAnnotation.Base;

namespace TalentBusinessLogic.DataAnnotation
{
    /// <summary>
    /// This class extends the behavior of the range attribute to retrieve the attribute
    /// and text values from the database
    /// </summary>
    class TalentRangeAttribute : TalentValidationAttribute
    {
        #region Class Level Fields

        private const string InvalidRangeMessage = "InvalidRangeMessage";
        private const string InternalErrorMessage = "The field {0} must be between {1} and {2}";
        private int defaultMinimum, defaultMaximum;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Minimum">Default minimum length</param>
        /// <param name="Maximum">Default maximum length</param>
        /// <param name="ErrorMessage">Default error message</param>
        /// <param name="PropertyName">Name of the property</param>
        /// <param name="OverrideFromDB">Flag indicates whether or not 
        /// override the attribute from database</param>
        public TalentRangeAttribute(int Minimum, int Maximum, string ErrorMessage = null, string PropertyName = "", bool OverrideFromDB = true) : base(ErrorMessage, PropertyName, OverrideFromDB)
        {
            this.defaultMinimum = Minimum;
            this.defaultMaximum = Maximum;
            this.internalErrorMessage = InternalErrorMessage;
        }

        #endregion

        #region Public Properties

        public int? Minimum { get; private set; }
        public int? Maximum { get; private set; }

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

            if (value  != null && value.ToString().Trim().Length != 0)
            {
                if (overrideFromDB)
                    SetMinMaxValues(modelType);
                else
                {
                    Minimum = defaultMinimum;
                    Maximum = defaultMaximum;
                }

                SetErrorMessage(modelType, InvalidRangeMessage, displayName, Minimum, Maximum);

                IComparable min = (IComparable)this.Minimum;
                IComparable max = (IComparable)this.Maximum;
                int convertedValue = Convert.ToInt32(value);
                retval = min.CompareTo(convertedValue) <= 0 && max.CompareTo(convertedValue) >= 0;
            }
            return retval;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method retrieves the minimum and maximum values from the database and sets them into local fields
        /// </summary>
        /// <param name="modelType"></param>
        private void SetMinMaxValues(string modelType)
        {
            if (Minimum == null)
            {
                var attributeName = _propertyName + "MinValue";
                string value = validator.GetAttribute(modelType, attributeName, this.defaultMinimum.ToString());
                Minimum = Convert.ToInt32(value);
            }

            if (Maximum == null)
            {
                var attributeName = _propertyName + "MaxValue";
                string value = validator.GetAttribute(modelType, attributeName, this.defaultMaximum.ToString());
                Maximum = Convert.ToInt32(value);
            }
        }

        #endregion
    }
}
