Imports Microsoft.VisualBasic
Imports Talent.Common

''' <summary>
''' Provides the functionality to access Data Access Layer and its Objects
''' </summary>
Public Class TalentBase01

#Region "Class Level Fields"
    ''' <summary>
    ''' Instance of TalentDataObjects
    ''' </summary>
    Private _talentData As TalentDataObjects
#End Region

#Region "Constructor"
    ''' <summary>
    ''' Initializes a new instance of the <see cref="TalentBase01" /> class.
    ''' Pass the connection string 
    ''' and destination database information to Settings Property
    ''' </summary>
    Sub New()
        _talentData = New TalentDataObjects
        _talentData.Settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString
        _talentData.Settings.DestinationDatabase = "SQL2005"
    End Sub
#End Region

End Class
