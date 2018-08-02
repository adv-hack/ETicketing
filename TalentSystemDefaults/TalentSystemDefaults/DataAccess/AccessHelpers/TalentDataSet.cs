using System;
using System.Data;
namespace TalentSystemDefaults
{
	/// <summary>
	/// This class will hold the custom result set so that custom type can be moved to cache
	/// </summary>
	[Serializable()]
	public class TalentDataSet
	{
		/// <summary>
		/// Gets or sets the result data set of type System.Data.DataSet
		/// </summary>
		/// <value>
		/// The result data set.
		/// </value>
		public DataSet ResultDataSet { get; set; }
		/// <summary>
		/// Gets or sets the dictionary data set of type Generic.Dictionary(Of String, String)
		/// </summary>
		/// <value>
		/// The dictionary data set.
		/// </value>
		public System.Collections.Generic.Dictionary<string, string> DictionaryDataSet { get; set; }
	}
}
