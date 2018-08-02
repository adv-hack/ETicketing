using System;
using System.Collections.Generic;
using System.Data;
using TalentSystemDefaults.DataAccess.DataObjects;
namespace TalentSystemDefaults
{
	namespace DataAccess.ModuleObjects
	{
		public class GenderType : DBObjectBase
		{			
			#region Class Level Fields
			private const string MD501_TYPE_CODE = "GEND";
			#endregion
			#region Public Methods
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="settings"></param>
			/// <remarks></remarks>
			public GenderType(ref DESettings settings)
				: base(settings)
			{
				businessUnit = settings.BusinessUnit;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public override DataTable RetrieveDTForList()
			{
				DataTable dtForList = null;
				dtForList = SelectDTGenderTypes(SelectDB2GenderTypes());
				return dtForList;
			}
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="dtFromSQL"></param>
			/// <param name="dtFromDB2"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectDTGenderTypes(DataTable dtFromDB2)
			{
				DataTable dtMerged = new DataTable();
				dtMerged.Columns.Add("GENDER_TYPE_CODE", typeof(string));
				dtMerged.Columns.Add("GENDER_TYPE_DESCRIPTION", typeof(string));
				dtMerged.Columns.Add("IS_OTHER_TYPE", typeof(string));
				if (dtFromDB2 != null && dtFromDB2.Rows.Count > 0)
				{
					DataRow dr = null;
					for (int rowindex = 0; rowindex <= dtFromDB2.Rows.Count - 1; rowindex++)
					{
                        int i = 0;
						dr = dtMerged.NewRow();
						dr["GENDER_TYPE_CODE"] = Utilities.CheckForDBNull_String(dtFromDB2.Rows[rowindex].ItemArray[i]);
						dr["GENDER_TYPE_DESCRIPTION"] = Utilities.CheckForDBNull_String(dtFromDB2.Rows[rowindex].ItemArray[i+1]);
						dr["IS_OTHER_TYPE"] = Utilities.CheckForDBNull_String(dtFromDB2.Rows[rowindex].ItemArray[i + 2]);
						dtMerged.Rows.Add(dr);
						dr = null;
					}
				}
				return dtMerged;
			}
			
			/// <summary>
			/// XML Comment
			/// </summary>
			/// <returns></returns>
			/// <remarks></remarks>
			public DataTable SelectDB2GenderTypes()
			{
				//how to get company code, type code and active flag
				MD501 md501Obj = new MD501(ref settings);
				DataTable outputDataTable = md501Obj.SelectByCompAndType(settings.Company, MD501_TYPE_CODE);
				return outputDataTable;
			}

			/// <summary>
			/// XML Comment
			/// </summary>
			/// <param name="givenTransaction"></param>
			/// <returns></returns>
			/// <remarks></remarks>
			public override int DeleteCurrentValues(System.Data.SqlClient.SqlTransaction givenTransaction = null)
			{
				int affectedRows = 0;
				MD501 dataObj = new MD501(ref settings);
				affectedRows += dataObj.DeleteData("GEND", settings.VariableKey1);
				return affectedRows;
			}
			#endregion
		}
	}
}
