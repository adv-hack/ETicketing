using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Web.ModelBinding;
using TalentBusinessLogic.DataAnnotation.Base;
using System.Linq;
using System.Web.Mvc;

namespace TalentBusinessLogic.DataAnnotation
{
    /// <summary>
    /// This class serves as a custom display attribute which
    /// retrieves the label value from the database
    /// </summary>
    public class TalentDisplayAttribute : DisplayNameAttribute
    {
        #region Class Level Fields

        private string _defaultName;
        private string _displayName;
        private string _propertyName;
        private string _modelName;
        private TalentDataValidator validator;

        #endregion

        #region Constructos

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Name">
        /// Default display name, further the display name from database will overwrite it.
        /// We will strip off the spaces to form the property name.
        /// </param>
        /// <param name="Model">
        /// The display attribute do not have any way to retrieve model, hence
        /// the model has been supplied as a parameter</param>
        /// <param name="PropertyName"></param>
        public TalentDisplayAttribute(string Name, Type Model, string PropertyName = "")
        {
            _defaultName = Name;
            _modelName = Model.Name;
            _propertyName = PropertyName;
            validator = new TalentDataValidator();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// This property provides the public accessor for the model name
        /// </summary>
        public string ModelName
        {
            get
            {
                return _modelName;
            }
        }

        /// <summary>
        /// This property has been overridden to 
        /// retrieve the value from the database
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (String.IsNullOrEmpty(_displayName))
                {
                    var controlCode = ModelName;
                    var textCode = _propertyName;
                    if (String.IsNullOrEmpty(textCode))
                    {
                        textCode = _defaultName.Replace(" ", String.Empty);
                    }
                    textCode += "Label";
                    _displayName = validator.GetText(controlCode, textCode, _defaultName);
                }
                return _displayName;
            }
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Retrieves the model name through reflection
        /// </summary>
        /// <returns></returns>
        private static string GetModelName()
        {
            var modelName = String.Empty;
            FieldInfo info = typeof(CachedAssociatedMetadataProvider<CachedDataAnnotationsModelMetadata>)
                                .GetField("_typeIds", BindingFlags.NonPublic | BindingFlags.Static);
            var types = (ConcurrentDictionary<Type, string>)info.GetValue(null);
            modelName = types.FirstOrDefault().Key.Name;
            return modelName;
        }

        #endregion
    }
}