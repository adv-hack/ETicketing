using System;
using System.Data;
namespace TalentSystemDefaults
{
	/// <summary>
	/// Class Provides the Properties for building up the DB2Command instance
	/// </summary>
	[Serializable()]
	public class DEDB2Command
	{
		#region Class Lavel Fields
		/// <summary>
		/// Command Text
		/// </summary>
		private string _commandText;
		/// <summary>
		/// Command Type
		/// </summary>
		private CommandType _commandType = System.Data.CommandType.Text;
		/// <summary>
		/// Command Parameter of type DEDB2Parameter (DB2Parameter)
		/// </summary>
		private System.Collections.Generic.List<DEDB2Parameter> _commandParameter;
		/// <summary>
		/// Command Execution Method of type CommandExecution Enum
		/// </summary>
		private CommandExecution _commandExecutionType;
		#endregion
		#region Properties
		/// <summary>
		/// Gets or sets the command text Transact SQL Statement
		/// or Parameterised SQL Statement
		/// or Stored Procedure Name
		/// </summary>
		/// <value>The command text.</value>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
			set
			{
				_commandText = value;
			}
		}
		/// <summary>
		/// Gets or sets the type of the command based on Data.CommandType
		/// </summary>
		/// <value>The type of the command.</value>
		public CommandType CommandType
		{
			get
			{
				return _commandType;
			}
			set
			{
				_commandType = value;
			}
		}
		/// <summary>
		/// Gets or sets the execution type for the command based on Talent.Common.CommandExecution
		/// </summary>
		/// <value>The type of the command execution.</value>
		public CommandExecution CommandExecutionType
		{
			get
			{
				return _commandExecutionType;
			}
			set
			{
				_commandExecutionType = value;
			}
		}
		/// <summary>
		/// Gets or sets the command parameter details
		/// through Generic List collection of DEDB2Parameter instance
		/// </summary>
		/// <value>The command parameter.</value>
		public System.Collections.Generic.List<DEDB2Parameter> CommandParameter
		{
			get
			{
				if (_commandParameter == null)
				{
					_commandParameter = new System.Collections.Generic.List<DEDB2Parameter>();
				}
				return _commandParameter;
			}
			set
			{
				_commandParameter = value;
			}
		}
		#endregion
	}
}
