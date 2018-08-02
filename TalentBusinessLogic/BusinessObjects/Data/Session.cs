using System;
using Talent.Common;

namespace TalentBusinessLogic.BusinessObjects.Data
{
    public class Session
    {
        #region Class Level Fields

        private CacheUtility _cacheUtility;

        #endregion

        #region Properties

        public CacheUtility CacheUtility
        {
            get
            {
                if (_cacheUtility == null)
                {
                    _cacheUtility = new CacheUtility();
                }
                return _cacheUtility;
            }
        }

        public string SessionId
        { 
            get
            {
                return CacheUtility.SessionId;
            }
        }

        #endregion

        #region Public Methods

        public Object Get(String sessionKey)
        {
            return CacheUtility.GetItemFromSession(sessionKey);
        }

        public void Add(String sessionKey, Object cacheItem)
        {
            CacheUtility.AddItemToSession(sessionKey, cacheItem);
        }

        public void Remove(String sessionKey)
        {
            CacheUtility.RemoveItemFromSession(sessionKey);
        }

        #endregion
    }
}
