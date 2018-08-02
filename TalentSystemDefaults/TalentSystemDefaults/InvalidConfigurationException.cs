using System;
namespace TalentSystemDefaults
{
	public class InvalidConfigurationException : Exception
	{
		public InvalidConfigurationException(string message)
			: base(message)
		{
		}
	}
}
