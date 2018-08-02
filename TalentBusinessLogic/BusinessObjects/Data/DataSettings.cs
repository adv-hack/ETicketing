using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Talent.Common;
using TalentBusinessLogic.Models;
using System.Globalization;

namespace TalentBusinessLogic.BusinessObjects.Data
{
    public class DataSettings
    {
        #region Class Level Fields

        private Validation _validation;
        private Cache _cache;
        private Session _session;

        #endregion

        #region Properties

        public Validation Validation
        {
            get
            {
                if (_validation == null)
                {
                    _validation = new Validation();
                }
                return _validation;
            }
        }

        public Cache Cache
        {
            get
            {
                if (_cache == null)
                {
                    _cache = new Cache();
                }
                return _cache;
            }
        }

        public Session Session
        {
            get
            {
                if (_session == null)
                {
                    _session = new Session();
                }
                return _session;
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Returns errored error model if table is empty
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public ErrorModel CheckTableHasData(DataTable table)
        {
            ErrorModel model = new ErrorModel();
            model.HasError = false;

            if (table == null)
            {
                model.HasError = true;
                model.ErrorMessage = "Empty table";
            }
            else
            {
                if (table.Rows.Count == 0)
                {
                    model.HasError = true;
                    model.ErrorMessage = "Empty table";
                }
            }
            return model;
        }

        /// <summary>
        /// Checks that the resultset has the correct number of tables
        /// </summary>
        /// <param name="results"></param>
        /// <param name="numberOfTables"></param>
        /// <returns></returns>
        private bool ResultSetHasCorrectNumberOfTables(DataSet results, int? numberOfTables)
        {
            if (numberOfTables != null)
            {
                if (results.Tables.Count == numberOfTables.Value)
                    return true;
            }
            else
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Overload method to call PopulateObjectFromRow without the object
        /// </summary>
        /// <typeparam name="T">Generic type which inherits from BaseModel</typeparam>
        /// <param name="row">Row from which to populate object from</param>
        public T PopulateObjectFromRow<T>(DataRow row)
        {
            T DataEntity = (T)Activator.CreateInstance(typeof(T));
            return PopulateObjectFromRow(row, DataEntity);
        }

        /// <summary>
        /// Returns single object after populating properties with data from DataRow
        /// </summary>
        /// <typeparam name="T">Generic type which inherits from BaseModel</typeparam>
        /// <param name="row">Row from which to populate object from</param>
        /// <param name="DataEntity">Object to continue populating.  This allows this method to be called more than once</param>
        public T PopulateObjectFromRow<T>(DataRow row, T DataEntity)
        {
            if (row != null)
            {
                Type t = typeof(T);
                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo typeProperty = t.GetProperty(column.ColumnName.ToString());
                    if (null != typeProperty && typeProperty.CanWrite && row[column.ColumnName] != DBNull.Value)
                    {
                        typeProperty.SetValue(DataEntity, row[column.ColumnName], null);
                    }
                }
            }
            return DataEntity;
        }

        private string toTitleCaseColumnName(string colName)
        {
            string titleCase = colName.Replace("_", " ");
            TextInfo ti = new CultureInfo("en-GB", false).TextInfo;
            titleCase = ti.ToLower(titleCase);
            titleCase = ti.ToTitleCase(titleCase);
            titleCase = titleCase.Replace(" ", "");

            return titleCase;
        }

        /// <summary>
        /// Returns list of objects of a generic type after populating properties with data from DataTable
        /// </summary>
        /// <typeparam name="T">Generic type</typeparam>
        /// <param name="Table">Table from which to populate object from</param>
        public List<T> PopulateObjectListFromTable<T>(DataTable table)
        {
            List<T> DataEntityList = new List<T>();

            if (table != null && table.Rows.Count > 0)
            {
                Type t = typeof(T);
                foreach (DataRow row in table.Rows)
                {
                    var dataEntityInstance = (T)Activator.CreateInstance(typeof(T));

                    foreach (DataColumn column in row.Table.Columns)
                    {
                        PropertyInfo typeProperty = t.GetProperty(column.ColumnName.ToString());
                        if (null == typeProperty)
                        {
                            typeProperty = t.GetProperty(toTitleCaseColumnName(column.ColumnName.ToString()));
                        }

                        if (null != typeProperty && typeProperty.CanWrite && row[column.ColumnName] != DBNull.Value)
                        {
                            typeProperty.SetValue(dataEntityInstance, row[column.ColumnName], null);
                        }
                    }
                    DataEntityList.Add(dataEntityInstance);
                }
            }
            return DataEntityList;
        }

        /// <summary>
        /// Returns populated error model object after error handling - operates on tables only
        /// </summary>
        /// <param name="error">Talent error object - logs .net error</param>
        /// <param name="table"></param>
        /// <returns></returns>
        public ErrorModel PopulateErrorObject(ErrorObj error, DataTable table, DESettings settings)
        {
            ErrorModel model = new ErrorModel();
            model.HasError = false;

            if (error.HasError)
            {
                model.HasError = true;
                model.ReturnCode = "TBL-DNError";
            }
            else
            {
                if (table != null)
                {
                    if (table.Rows.Count == 0)
                    {
                        model.HasError = true;
                        model.ReturnCode = "TBL-EmptyTable";
                    }
                }
            }

            if (model.HasError)
            {
                model.ErrorMessage = AddErrorMessageText(model.ReturnCode, settings);
            }
            return model;
        }
        /// <summary>
        /// Returns populated error model object after error handling - operates on dataSets only
        /// </summary>
        /// <param name="error">Talent error object - logs .net error</param>
        /// <param name="resultSet"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ErrorModel PopulateErrorObject(ErrorObj error, DataSet resultSet, DESettings settings, string statusTableName, int? numberOfTables, bool populateStatus = false)
        {
            ErrorModel model = new ErrorModel();
            model.HasError = false;
            if (error.HasError)
            {
                model.HasError = true;
                model.ReturnCode = "TBL-DNError";
                model.TechnicalErrorMessage = error.ErrorMessage;
            }
            else
            {
                if (resultSet != null)
                {
                    if (!ResultSetHasCorrectNumberOfTables(resultSet, numberOfTables))
                    {
                        model.HasError = true;
                        model.ReturnCode = "TBL-TableCountError";
                    }
                    if (!model.HasError)
                    {
                        model = PopulateObjectFromRow<ErrorModel>(resultSet.Tables[statusTableName].Rows[0]);
                        if (model.ErrorOccurred.Trim() == GlobalConstants.ERRORFLAG)
                        {
                            model.HasError = true;
                        }
                    }
                }
                else
                {
                    model.HasError = true;
                    model.ReturnCode = "TBL-ResultSetError";
                }
            }

            LogsErrorMessage(settings, ref model);
            return model;
        }

        /// <summary>
        /// Adds text to ErrorModel object
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="settings"></param>
        /// <returns>Error text for model</returns>
        private string AddErrorMessageText(string returnCode, DESettings settings)
        {
            if (returnCode.Equals("TBL-DNError") || returnCode.Equals("TBL-ResultSetError"))
            {
                returnCode = "*ALL";
            }
            TalentErrorMessages errorMessage = new TalentErrorMessages("ENG", settings.BusinessUnit,
                                                                        settings.Partner, settings.FrontEndConnectionString);
            return errorMessage.GetErrorMessage(returnCode).ERROR_MESSAGE;
        }
        /// <summary>
        /// Logs and sets error message
        /// </summary>
        /// <param name="returnCode"></param>
        /// <param name="settings"></param>
        /// <param name="model"></param>
        private void LogsErrorMessage(DESettings settings, ref ErrorModel model)
        {
            if (model.HasError && !string.IsNullOrEmpty(model.ReturnCode)) 
            {
                model.ErrorMessage = AddErrorMessageText(model.ReturnCode, settings);
                settings.Logging.ErrorObjectLog("BusinessObjects.Data.DataSettings.cs - PopulateErrorObject | - " + settings.ModuleName, model.ReturnCode, model.ErrorMessage + " " + model.TechnicalErrorMessage);
            }
        }
        /// <summary>
        /// Returns populated error model object after error handling - operates on dataSets only
        /// </summary>
        /// <param name="error">Talent error object - logs .net error</param>
        /// <param name="resultSet"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public ErrorModel PopulateErrorObject(ErrorObj error, DataSet resultSet, DESettings settings, int? numberOfTables, bool populateStatus = false)
        {
            return PopulateErrorObject(error, resultSet, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, numberOfTables, populateStatus);
        }

        #endregion
    }
}
