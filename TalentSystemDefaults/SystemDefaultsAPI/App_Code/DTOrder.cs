namespace SystemDefaultsAPI
{
	namespace UtilityClasses
	{
		public class DTOrder
		{
			private int m_Column;
			private DTOrderDir m_Dir;
			/// <summary>
			/// Column to which ordering should be applied.
			/// This is an index reference to the columns array of information that is also submitted to the server.
			/// </summary>
			public int Column
			{
				get
				{
					return m_Column;
				}
				set
				{
					m_Column = value;
				}
			}
			/// <summary>
			/// Ordering direction for this column.
			/// It will be dt-string asc or dt-string desc to indicate ascending ordering or descending ordering, respectively.
			/// </summary>
			public DTOrderDir Dir
			{
				get
				{
					return m_Dir;
				}
				set
				{
					m_Dir = value;
				}
			}
		}
		public enum DTOrderDir
		{
			ASC,
			DESC
		}
	}
}
