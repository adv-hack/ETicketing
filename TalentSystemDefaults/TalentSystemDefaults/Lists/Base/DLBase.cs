using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	public class DLBase
	{
		protected DESettings settings;
		protected Dictionary<string, string> values;
		public DEList DEList { get; set; }
		public DLBase(DESettings settings, Dictionary<string, string> filters = null)
		{
			this.settings = settings;
			DEList = new DEList();
			Initialise();
			DEList.AddURL = GetAddURL();
			DEList.UpdateURL = GetUpdateURL();
			DEList.DeleteURL = GetDeleteURL();
		}
		protected virtual void Initialise()
		{
		}
		public void Populate()
		{
			RetrieveData();
			if (DEList.Data.Count == 0)
			{
				DEList.StatusMessage = "No results found";
                DEList.HasError = true;
			}
		}
		protected virtual void RetrieveData()
		{
			if (!string.IsNullOrWhiteSpace(DEList.TableName))
			{
				DBObjectBase dataObj = DMFactory.GetDBObject(DEList.TableName, settings);
				DataTable dtForList = dataObj.RetrieveDTForList();
				DEList.Data = GetDEEntities(DEList.DataColumns, DEList.VariableKeyColumns, dtForList);
			}
			else if (!string.IsNullOrWhiteSpace(DEList.FunctionName))
			{
				//function name goes to module object and call the function and get the customized datatable which may be from mroe than one table
				DBObjectBase modObject = DMFactory.GetModuleObject(DEList.ModuleObjectName, settings);
				DataTable dtForList = modObject.RetrieveDTForList();
				DEList.Data = GetDEEntities(DEList.DataColumns, DEList.VariableKeyColumns, dtForList);
			}
		}
		public virtual void Delete()
		{
			int affectedRows = 0;
			if (!string.IsNullOrWhiteSpace(DEList.TableName))
			{
				DBObjectBase dataObj = DMFactory.GetDBObject(DEList.TableName, settings);

				if (dataObj.DBTypeForTable == DBObjectBase.DBType.SQLSERVER)
				{
					affectedRows = dataObj.DeleteCurrentValues();
				}
				else if (dataObj.DBTypeForTable == DBObjectBase.DBType.DB2)
				{
					affectedRows += dataObj.DeleteCurrentValuesDB2();
				}
			}
			else if (!string.IsNullOrWhiteSpace(DEList.FunctionName))
			{
				DBObjectBase modObject = DMFactory.GetModuleObject(DEList.ModuleObjectName, settings);
				affectedRows = modObject.DeleteCurrentValues();
			}
			affectedRows += settings.DB2AccessQueue.Execute(settings);

			if (affectedRows > 0)
			{
				DEList.StatusMessage = "Delete succeeded";
                DEList.HasError = false;
			}
			else
			{
				DEList.StatusMessage = "Delete failed";
                DEList.HasError = true;
			}
		}
		protected List<DEListDetail> GetDEEntities(System.Collections.Generic.List<DataColumn> dataColumns, System.Collections.Generic.List<KeyColumn> defaultKeyColumns, DataTable dataTable)
		{
			System.Collections.Generic.List<DEListDetail> result = new System.Collections.Generic.List<DEListDetail>();
			foreach (DataRow dr in dataTable.Rows)
			{
				System.Collections.Generic.List<string> values = new System.Collections.Generic.List<string>();
				foreach (DataColumn column in dataColumns)
				{
					string value = dr[column.Name].ToString();
					values.Add(value);
				}
				System.Collections.Generic.List<string> VariableKeyValues = new System.Collections.Generic.List<string>();
				foreach (KeyColumn keyColumn in defaultKeyColumns)
				{
					string value = dr[keyColumn.Name].ToString();
					VariableKeyValues.Add(value);
				}
				DEListDetail entity = new DEListDetail(values.ToArray(), VariableKeyValues.ToArray());
				entity.SelectURL = GetSelectURL(entity);
				result.Add(entity);
			}
			Format(ref result);
			return result;
		}
		protected virtual void Format(ref System.Collections.Generic.List<DEListDetail> items)
		{
		}
		protected virtual string GetAddURL()
		{
			return string.Empty;
		}
		protected virtual string GetSelectURL(DEListDetail listDetail)
		{
			return string.Empty;
		}
		protected virtual string GetUpdateURL()
		{
			return string.Empty;
		}
		protected virtual string GetDeleteURL()
		{
			return string.Empty;
		}
	}
}
