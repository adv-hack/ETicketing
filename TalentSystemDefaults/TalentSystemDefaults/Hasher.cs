using System.Text;
namespace TalentSystemDefaults
{
	public class Hasher
	{
		private System.String characterSet = "XH1fyNPRw3itcjE6dgLKZ2hJYTMmqlF7SkUVs98b#nC0arWveQOx4GIoBApu5Dz";
		private System.String fullCharacterSet = "XH1fyNPRw 3itc*jE6dgLKZ,2hJYTMmqlF.7SkUVs98b#nC0arW:veQOx_4GIoB/Apu5Dz";
		public string HashString(string value)
		{
			return HashStringValue(value, characterSet);
		}
		public string HashFullString(string value)
		{
			return HashStringValue(value, fullCharacterSet);
		}
		private string HashStringValue(string value, string characterSet)
		{
			var length = characterSet.Length;
			StringBuilder result = new StringBuilder();
			var i = 1;
			foreach (char c in value)
			{
				var index = characterSet.IndexOf(c) + i;
				if (index >= length)
				{
					index = index % length;
				}
				result.Append(characterSet[index]);
				i++;
			}
			return result.ToString();
		}
		public string DeHashString(string value)
		{
			return DeHashStringValue(value, characterSet);
		}
		public string DeHashFullString(string value)
		{
			return DeHashStringValue(value, fullCharacterSet);
		}
		private string DeHashStringValue(string value, string characterSet)
		{
			var length = characterSet.Length;
			StringBuilder result = new StringBuilder();
			var i = 1;
			foreach (char c in value)
			{
				var index = characterSet.IndexOf(c) - i;
				while (index < 0)
				{
					index += length;
				}
				result.Append(characterSet[index]);
				i++;
			}
			return result.ToString();
		}
	}
}
