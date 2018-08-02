''' <summary>
''' This class will hold the custom result set so that custom type can be moved to cache 
''' </summary>
<Serializable()> _
Public Class TalentDataSet
    ''' <summary>
    ''' Gets or sets the result data set of type System.Data.DataSet
    ''' </summary>
    ''' <value>
    ''' The result data set.
    ''' </value>
    Public Property ResultDataSet As DataSet
    ''' <summary>
    ''' Gets or sets the dictionary data set of type Generic.Dictionary(Of String, String)
    ''' </summary>
    ''' <value>
    ''' The dictionary data set.
    ''' </value>
    Public Property DictionaryDataSet As Generic.Dictionary(Of String, String)
    Public Property DictionaryOfPriceCodes As Generic.Dictionary(Of String, DEPriceCode)
End Class
