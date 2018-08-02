Namespace DataObjects.TableObjects
    ''' <summary>
    ''' Provides the functionality to log the activity in tbl_log_header table
    ''' </summary>
    <Serializable()> _
        Public Class tbl_log_header
        Inherits DBObjectBase

#Region "Class Level Fields"
        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings
#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_log_header" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Create a log for the specified functionality in table and returns log header id as reference and no of rows affected
        ''' </summary>
        ''' <param name="sourceApplication">The source application.</param>
        ''' <param name="sourceClass">The source class.</param>
        ''' <param name="sourceMethod">The source method.</param>
        ''' <param name="status">The status of type ActivityStatusEnum</param>
        ''' <param name="additionalDetails">The additional details.</param>
        ''' <param name="logHeaderId">The log header id as reference</param>
        ''' <returns>log header id as reference and no of affected rows</returns>
        Public Function Create(ByVal sourceApplication As String, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal status As ActivityStatusEnum, ByVal additionalDetails As String, ByRef logHeaderId As Integer) As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As String = "SET NOCOUNT ON " & _
                "INSERT INTO TBL_LOG_HEADER (" & _
                "SOURCE_APPLICATION, SOURCE_CLASS, SOURCE_METHOD, STATUS, ADDITIONAL_DETAILS) VALUES (" & _
                "@SourceApplication, @SourceClass, @SourceMethod, @Status, @AdditionalDetails) " & _
                "SELECT SCOPE_IDENTITY()"
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            Dim statusString As String = String.Empty
            If (status <> Nothing) Then
                statusString = System.Enum.GetName(GetType(ActivityStatusEnum), status)
            End If
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceApplication", sourceApplication))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceClass", sourceClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceMethod", sourceMethod))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AdditionalDetails", additionalDetails))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", statusString))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                If (rowsAffected > 0) Then
                    logHeaderId = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                End If
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return rowsAffected
        End Function

        ''' <summary>
        ''' Updates the specified log header id.
        ''' </summary>
        ''' <param name="logHeaderId">The log header id.</param>
        ''' <param name="status">The status of type ActivityStatusEnum</param>
        ''' <param name="additionalDetails">The additional details.</param>
        ''' <returns>No of affected rows</returns>
        Public Function Update(ByVal logHeaderId As Integer, ByVal status As ActivityStatusEnum, ByVal additionalDetails As String) As Integer
            Dim rowsAffected As Integer = 0
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.Settings.Cacheing = False
            talentSqlAccessDetail.Settings.CacheStringExtension = ""
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteNonQuery
            Dim sqlStatement As String = "UPDATE TBL_LOG_HEADER " & _
                "SET STATUS=@Status, ADDITIONAL_DETAILS= ADDITIONAL_DETAILS + @AdditionalDetails " & _
                "WHERE LOG_HEADER_ID=@LogHeaderId "
            Dim statusString As String = String.Empty
            If (status <> Nothing) Then
                statusString = System.Enum.GetName(GetType(ActivityStatusEnum), status)
            End If
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@LogHeaderId", logHeaderId))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", statusString))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@AdditionalDetails", additionalDetails))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                rowsAffected = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
            End If
            talentSqlAccessDetail = Nothing

            'Return results
            Return rowsAffected
        End Function

        ''' <summary>
        ''' Determines whether the specified functionality is already in progress.
        ''' </summary>
        ''' <param name="sourceApplication">The source application.</param>
        ''' <param name="sourceClass">The source class.</param>
        ''' <param name="sourceMethod">The source method.</param>
        ''' <param name="status">The status.</param>
        ''' <returns>
        ''' <c>true</c> if the specified functionality is in progress; otherwise, <c>false</c>.
        ''' </returns>
        Public Function IsActive(ByVal sourceApplication As String, ByVal sourceClass As String, ByVal sourceMethod As String, ByVal status As ActivityStatusEnum) As Boolean
            Dim isFunctionalityActive As Boolean = False
            Dim talentSqlAccessDetail As New TalentDataAccess
            Dim err As New ErrorObj
            'Construct The Call
            talentSqlAccessDetail.Settings = _settings
            talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
            Dim sqlStatement As String = "SELECT COUNT(LOG_HEADER_ID) FROM TBL_LOG_HEADER " & _
                "WHERE SOURCE_APPLICATION=@SourceApplication AND SOURCE_CLASS=@SourceClass AND SOURCE_METHOD=@SourceMethod AND STATUS=@Status "
            talentSqlAccessDetail.CommandElements.CommandText = sqlStatement
            Dim statusString As String = String.Empty
            If (status <> Nothing) Then
                statusString = System.Enum.GetName(GetType(ActivityStatusEnum), status)
            End If
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceApplication", sourceApplication))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceClass", sourceClass))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SourceMethod", sourceMethod))
            talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@Status", statusString))

            'Execute
            err = talentSqlAccessDetail.SQLAccess()
            Dim noOfExistingRows As Integer = 0
            If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                noOfExistingRows = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0)(0)
                If (noOfExistingRows = 0) Then
                    isFunctionalityActive = False
                Else
                    isFunctionalityActive = True
                End If
            End If
            talentSqlAccessDetail = Nothing
            'Return results
            Return isFunctionalityActive
        End Function

#End Region

    End Class
End Namespace
