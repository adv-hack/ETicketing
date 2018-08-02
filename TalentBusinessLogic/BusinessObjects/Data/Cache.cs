using System;
using Talent.Common;

namespace TalentBusinessLogic.BusinessObjects.Data
{
    public class Cache
    {
        #region Class Level Fields
        
        private CacheUtility _cacheUtility;
        private CacheUtility CacheUtility 
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

        #endregion

        #region Public Methods

        public Object Get(String cacheKey)
        {
            return CacheUtility.GetItemFromCache(cacheKey);
        }

        public void Add(String cacheKey, Object cacheItem, DESettings settings)
        {
            CacheUtility.AddItemToCache(cacheKey, cacheItem, settings);
        }

        public void Add(String cacheKey, Object cacheItem, string cacheTimeInMins)
        {
            DateTime cacheDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(cacheTimeInMins));
            CacheUtility.AddIemToCache(cacheKey, cacheItem, cacheDateTime);
        }

        public void Remove(String cacheKey)
        {
            CacheUtility.RemoveItemFromCache(cacheKey);
        }

        #endregion
    }
}
