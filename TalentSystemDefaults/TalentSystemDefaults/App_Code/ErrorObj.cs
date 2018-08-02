using System;
namespace TalentSystemDefaults
{
	[Serializable()]
	public class ErrorObj
	{
		private bool _hasError = false; // true false flag
		private string _errorMessage = string.Empty; // System generated error message
		private string _errorNumber = string.Empty; // program generated error number
		private string _errorStatus = string.Empty; // program generated error code
		private string[] _itemErrorCode = new string[501]; // Error code for multiple items (e.g orders) per transaction
		private string[] _itemErrorMessage = new string[501]; // Error message for multiple items (e.g orders) per transaction
		private string[] _itemErrorStatus = new string[501]; // Error status for multiple items (e.g orders) per transaction
		public bool HasError
		{
			get
			{
				return _hasError;
			}
			set
			{
				_hasError = value;
			}
		}
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
			set
			{
				_errorMessage = value;
			}
		}
		public string ErrorNumber
		{
			get
			{
				return _errorNumber;
			}
			set
			{
				_errorNumber = value;
			}
		}
		public string ErrorStatus
		{
			get
			{
				return _errorStatus;
			}
			set
			{
				_errorStatus = value;
			}
		}
		public string get_ItemErrorCode(int item)
		{
			return _itemErrorCode[item];
		}
		public void set_ItemErrorCode(int item, string value)
		{
			_itemErrorCode[item] = value;
		}
		public string get_ItemErrorMessage(int item)
		{
			return _itemErrorMessage[item];
		}
		public void set_ItemErrorMessage(int item, string value)
		{
			_itemErrorMessage[item] = value;
		}
		public string get_ItemErrorStatus(int item)
		{
			return _itemErrorStatus[item];
		}
		public void set_ItemErrorStatus(int item, string value)
		{
			_itemErrorStatus[item] = value;
		}
	}
}
