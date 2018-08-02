
namespace TalentBusinessLogic.BusinessObjects.Environment
{
    public class EnvironmentSettings
    {
        #region Class Level Fields

        private Settings _settings;
        private Agent _agent;

        #endregion

        #region Properties

        public Settings Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;
            }
        }

        public Agent Agent
        {
            get
            {
                if (_agent == null)
                {
                    _agent = new Agent();
                }
                return _agent;
            }
        }

        #endregion
    }
}
