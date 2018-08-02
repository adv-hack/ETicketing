using System;
using System.Data;
using System.Xml;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class DB2AccessQueue : System.Collections.Generic.List<TalentDB2Access>
	{
		#region Public Methods
		public int Execute(DESettings settings)
		{
			int affectedRows = 0;
			System.Collections.Generic.List<TalentDB2Access> removeList = new System.Collections.Generic.List<TalentDB2Access>();
			foreach (TalentDB2Access db2Access in this)
			{
				bool hasQuote = false;
				foreach (DEDB2Parameter param in db2Access.CommandElements.CommandParameter)
				{
					if (param.ParamValue != null && System.Convert.ToString(param.ParamValue).Contains("'"))
					{
						hasQuote = true;
						break;
					}
				}
				if (hasQuote)
				{
					ErrorObj err = default(ErrorObj);
					affectedRows += System.Convert.ToInt32(Execute(db2Access, ref err));
					if (err.HasError)
					{
						throw new Exception(err.ErrorMessage);
					}
					removeList.Add(db2Access);
				}
			}
			foreach (TalentDB2Access item in removeList)
			{
				this.Remove(item);
			}
			if (Count > 0)
			{
				if (Count == 1)
				{
					ErrorObj err = default(ErrorObj);
					affectedRows += Execute(this[0], ref err);
					if (err.HasError)
					{
						throw new Exception(err.ErrorMessage);
					}
				}
				else
				{
					ErrorObj err = default(ErrorObj);
					settings.FunctionName = "UpdateDB2Data";
					string xmlString = GetXML();
					DEDB2Defaults deDB2Defaults = new DEDB2Defaults();
					deDB2Defaults.XMLString = xmlString;
					TalentDB2Access talentDB2AccessDetail = new TalentDB2Access();
					DataTable outputDataTable = default(DataTable);
					try
					{
						talentDB2AccessDetail.Settings = settings;
						//Execute
						err = talentDB2AccessDetail.DB2DefaultsAccess(DestinationDatabase.TALENTTKT, deDB2Defaults);
						if ((!(err.HasError)) && (!(talentDB2AccessDetail.ResultDataSet == null)))
						{
							outputDataTable = talentDB2AccessDetail.ResultDataSet.Tables[1];
							affectedRows = System.Convert.ToInt32(outputDataTable.Rows[0]["AffectedRows"]);
						}
					}
					catch (Exception)
					{
						throw;
					}
					finally
					{
						talentDB2AccessDetail = null;
					}
				}
			}
			Clear();
			return affectedRows;
		}
		#endregion
		#region Private Methods
		private string GetXML()
		{
			XmlDocument doc = new XmlDocument();
			XmlNode talentDefaultsNode = doc.CreateElement("TalentDefaults");
			doc.AppendChild(talentDefaultsNode);
			foreach (TalentDB2Access item in this)
			{
				XmlNode sqlNode = doc.CreateElement("SQL");
				talentDefaultsNode.AppendChild(sqlNode);
				XmlNode statementNode = doc.CreateElement("Statement");
				statementNode.AppendChild(doc.CreateTextNode(item.CommandElements.CommandText));
				sqlNode.AppendChild(statementNode);
				XmlNode parametersNode = doc.CreateElement("Parameters");
				sqlNode.AppendChild(parametersNode);
				foreach (DEDB2Parameter param in item.CommandElements.CommandParameter)
				{
					XmlNode parameterNode = doc.CreateElement("Parameter");
					parametersNode.AppendChild(parameterNode);
					XmlNode nameNode = doc.CreateElement("Name");
					nameNode.AppendChild(doc.CreateTextNode(param.ParamName));
					parameterNode.AppendChild(nameNode);
					XmlNode valueNode = doc.CreateElement("Value");
					valueNode.AppendChild(doc.CreateTextNode(param.ParamValue));
					parameterNode.AppendChild(valueNode);
					XmlNode useQuotesNode = doc.CreateElement("UseQuotes");
					useQuotesNode.AppendChild(doc.CreateTextNode(GetUseQuotes(param)));
					parameterNode.AppendChild(useQuotesNode);
				}
			}
			return doc.InnerXml;
		}
		private string GetUseQuotes(DEDB2Parameter param)
		{
			bool isNumeric = param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2BigInt |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2Decimal |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2Double |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2Integer |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2Numeric |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2Real |
				param.ParamType == IBM.Data.DB2.iSeries.iDB2DbType.iDB2SmallInt;
			return (isNumeric ? "No" : "Yes");
		}
		private dynamic Execute(TalentDB2Access db2Access, ref ErrorObj err)
		{
			int affectedRows = 0;
			err = db2Access.DB2Access(DestinationDatabase.TALENTTKT);
			if ((!(err.HasError)) && (!(db2Access.ResultDataSet == null)))
			{
				affectedRows = System.Convert.ToInt32(db2Access.ResultDataSet.Tables[0].Rows[0][0]);
			}
			return affectedRows;
		}
		#endregion
	}
}
