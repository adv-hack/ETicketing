using System;

namespace TalentBusinessLogic.BusinessObjects.Data
{
    public class Validation
    {
        #region "Check For DB Null Functions"

        public long CheckForDBNull_Long(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return 0;
                else
                    return Convert.ToInt64(obj);
            }
            catch (Exception)
            {
                return long.MinValue;
            }
        }
        
        public int CheckForDBNull_Int(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return 0;
                else
                    return Convert.ToInt32(obj);
            }
            catch (Exception)
            {
                return int.MinValue;
            }
        }

        public string CheckForDBNull_String(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return string.Empty;
                else
                    return Convert.ToString(obj);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public System.DateTime CheckForDBNull_Date(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value))
                    return DateTime.Now;
                else
                    return Convert.ToDateTime(obj);
            }
            catch (Exception)
            {
                return System.DateTime.MinValue;
            }
        }

        public decimal CheckForDBNull_Decimal(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return 0;
                else
                    return Convert.ToDecimal(obj);
            }
            catch (Exception)
            {
                return decimal.MinValue;
            }
        }

        public bool CheckForDBNull_Boolean_DefaultFalse(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return false;
                else
                    return Convert.ToBoolean(obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckForDBNull_Boolean_DefaultTrue(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value) || obj.Equals(string.Empty))
                    return true;
                else
                    return Convert.ToBoolean(obj);
            }
            catch (Exception)
            {
                return true;
            }
        }

        public bool CheckForDBNullOrBlank_Boolean_DefaultFalse(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value))
                {
                    return false;
                }
                else
                {
                    if (obj.ToString().Equals(string.Empty))
                    {
                        return false;
                    }
                    else
                    {
                        return Convert.ToBoolean(obj);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckForDBNullOrBlank_Boolean_DefaultTrue(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value))
                {
                    return true;
                }
                else
                {
                    if (obj.ToString().Equals(string.Empty))
                    {
                        return true;
                    }
                    else
                    {
                        return Convert.ToBoolean(obj);
                    }
                }
            }
            catch (Exception)
            {
                return true;
            }
        }

        public bool CheckIsDBNull(object obj)
        {
            try
            {
                if (obj.Equals(DBNull.Value))
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object CheckForDBNull(object value)
        {
            try
            {
                if (value.Equals(DBNull.Value))
                {
                    return null;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object CheckForDBNull(object value, object defaultReturnObject)
        {
            try
            {
                if (value.Equals(DBNull.Value))
                {
                    return defaultReturnObject;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        #endregion
    }
}
