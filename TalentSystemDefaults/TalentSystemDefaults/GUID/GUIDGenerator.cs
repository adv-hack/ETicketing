using TalentSystemDefaults.DataEntities;
namespace TalentSystemDefaults
{
	public class GUIDGenerator
	{
		private Hasher hasher;
		public GUIDGenerator()
		{
			hasher = new Hasher();
		}
		public string GenerateGUIDString(int id, ConfigurationEntity entity)
		{
			GUIDType guidValue = GenerateGUID(id, entity);
			string value = guidValue.FirstPart + guidValue.SecondPart;
			string tableName = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", value[5], value[7], value[10], value[13], value[22], value[23], value[33], value[46]);
			string defaultName = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", value[51], value[56], value[60], value[61], value[69], value[72], value[78], value[95]);
			var dataLength = guidValue.ThirdPart;
			var uniqueNumber = guidValue.FourthPart;
			return string.Join("-", new[] { hasher.HashString(tableName), hasher.HashString(defaultName), hasher.HashString(dataLength), hasher.HashString(uniqueNumber) });
		}
		private GUIDType GenerateGUID(int id, ConfigurationEntity entity)
		{
			GUIDType guidValue = new GUIDType();
			guidValue.FirstPart = GetFirstPart(entity.TableName);
			guidValue.SecondPart = GetSecondPart(entity.DefaultName);
			guidValue.ThirdPart = GetThirdPart(DataLength(entity).ToString());
			guidValue.FourthPart = GetFourthPart(id.ToString());
			return guidValue;
		}
		private int DataLength(ConfigurationEntity entity)
		{
			int defaultKeysLength = entity.DefaultKey1.Length + entity.DefaultKey2.Length + entity.DefaultKey3.Length + entity.DefaultKey4.Length;
			int variableKeysLength = entity.VariableKey1.Length + entity.VariableKey2.Length + entity.VariableKey3.Length + entity.VariableKey4.Length;
			int length = entity.TableName.Length + entity.DefaultName.Length + entity.MasterConfigId.Length + defaultKeysLength +
				variableKeysLength + entity.DefaultValue.Length + entity.AllowedValues.Length + entity.AllowedPlaceHolders.Length;
			return length;
		}
		private string GetFirstPart(string tableName)
		{
			int length = tableName.Length;
			while (length < 50)
			{
				tableName += tableName;
				length = tableName.Length;
			}
			return tableName.Substring(0, 50);
		}
		private string GetSecondPart(string fieldName)
		{
			var length = fieldName.Length;
			while (length < 50)
			{
				fieldName += fieldName;
				length = fieldName.Length;
			}
			return fieldName.Substring(0, 50);
		}
		private string GetThirdPart(string dataLength)
		{
			if (dataLength.Length < 7)
			{
				dataLength = dataLength.PadLeft(7, '0');
			}
			return dataLength.Substring(0, 7);
		}
		private string GetFourthPart(string id)
		{
			int length = id.Length;
			if (length < 6)
			{
				id = id.PadLeft(6, '0');
			}
			return id.Substring(0, 6);
		}
		public string DeHashGUID(string value)
		{
			string[] values = value.Split("-".ToCharArray());
			return string.Join("-", new[] { hasher.DeHashString(values[0]), hasher.DeHashString(values[1]), hasher.DeHashString(values[2]), hasher.DeHashString(values[3]) });
		}
		public int ExtractUniqueId(string configId)
		{
			string[] values = configId.Split("-".ToCharArray());
			return System.Convert.ToInt32(hasher.DeHashString(values[3]));
		}
	}
}
