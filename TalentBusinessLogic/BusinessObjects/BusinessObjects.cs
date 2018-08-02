using Talent.Common;
using TalentBusinessLogic.BusinessObjects.Data;
using TalentBusinessLogic.BusinessObjects.Environment;

namespace TalentBusinessLogic.BusinessObjects
{
    public class BusinessObjects
    {
        #region Class Level Fields

        private DataSettings _data;
        private EnvironmentSettings _environment;
        private TalentLogging _logging;
        private TalentDataObjects _tDataObjects;
        private Activities _activities;
        private Fees _fees;

        #endregion

        #region Properties

        public DataSettings Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new DataSettings();
                }
                return _data;
            }
        }

        public EnvironmentSettings Environment
        { 
            get
            {
                if (_environment == null)
                {
                    _environment = new EnvironmentSettings();
                }
                return _environment;
            }
        }

        public TalentLogging Logging
        {
            get
            {
                if (_logging == null)
                {
                    _logging = new TalentLogging();
                    _logging.FrontEndConnectionString = Environment.Settings.FrontEndConnectionString;
                }
                return _logging;
            }
        }

        public TalentDataObjects TDataObjects
        {
            get
            {
                if (_tDataObjects == null)
                {
                    _tDataObjects = new TalentDataObjects();
                    _tDataObjects.Settings = Environment.Settings.DESettings;
                }
                return _tDataObjects;
            }
        }

        public Activities Activities
        {
            get 
            { 
                if (_activities == null)
                {
                    _activities = new Activities();
                }
                return _activities;
            }
        }

        public Fees Fees
        {
            get
            {
                if (_fees == null)
                {
                    _fees = new Fees();
                }
                return _fees;
            }
        }
        
        #endregion
    }
}
