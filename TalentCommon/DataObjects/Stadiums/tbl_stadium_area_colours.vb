Imports System.Data.SqlClient

Namespace DataObjects.TableObjects

    ''' <summary>
    ''' Provides the functionality to manage data from the table tbl_stadium_area_colours based on business functionality
    ''' </summary>
    <Serializable()> _
    Public Class tbl_stadium_area_colours
        Inherits DBObjectBase

#Region "Class Level Fields"

        ''' <summary>
        ''' Instance of DESettings
        ''' </summary>
        Private _settings As New DESettings

        ''' <summary>
        ''' Class Name which is used in cache key construction
        ''' </summary>
        Const CACHEKEY_CLASSNAME As String = "tbl_stadium_area_colours"

#End Region

#Region "Constructors"
        Sub New()
        End Sub
        ''' <summary>
        ''' Initializes a new instance of the <see cref="tbl_stadium_area_colours" /> class.
        ''' </summary>
        ''' <param name="settings">The DESettings instance</param>
        Sub New(ByVal settings As DESettings)
            _settings = settings
        End Sub
#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Gets the stadium pricing colours and text
        ''' </summary>
        ''' <param name="businessUnit">The given business unit string</param>
        ''' <param name="cacheing">Cacheing setting as boolean, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time in mins, default 30</param>
        ''' <returns>A datatable of area colours and text values</returns>
        ''' <remarks></remarks>
        Public Function GetStadiumPricingColoursAndText(ByVal businessUnit As String, ByVal stadiumCode As String, Optional ByVal cacheing As Boolean = True, _
                                                   Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStadiumPricingColoursAndText_" & stadiumCode)
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Stadium
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))

                Dim sqlStatement As String = "SELECT * FROM [tbl_stadium_area_colours] WHERE KEY_NAME = 'PRICING' AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & " ORDER BY SEQUENCE"
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If (outputDataTable.Rows.Count > 0) Then
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                outputDataTable = New DataTable
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Gets the stadium availablity colours and text
        ''' </summary>
        ''' <param name="businessUnit">The given business unit string</param>
        ''' <param name="cacheing">Cacheing setting as boolean, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time in mins, default 30</param>
        ''' <returns>A datatable of area colours and text values</returns>
        ''' <remarks></remarks>
        Public Function GetStadiumAvailabilityColoursAndText(ByVal businessUnit As String, ByVal stadiumCode As String, Optional ByVal cacheing As Boolean = True, _
                                                   Optional ByVal cacheTimeMinutes As Integer = 30) As DataTable
            Dim outputDataTable As New DataTable
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetStadiumAvailabilityColoursAndText_" & stadiumCode)
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Stadium
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))

                Dim sqlStatement As String = "SELECT * FROM [tbl_stadium_area_colours] WHERE KEY_NAME = 'AVAILABILITY' AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter) & " ORDER BY SEQUENCE"
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0)
                        If (outputDataTable.Rows.Count > 0) Then
                            Exit For
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                outputDataTable = New DataTable
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function

        ''' <summary>
        ''' Gets the stadium hover colour
        ''' </summary>
        ''' <param name="businessUnit">The given business unit string</param>
        ''' <param name="cacheing">Cacheing setting as boolean, default true</param>
        ''' <param name="cacheTimeMinutes">Cache time in mins, default 30</param>
        ''' <returns>A datatable of area colours and text values</returns>
        ''' <remarks></remarks>
        Public Function GetSingleStadiumColour(ByVal businessUnit As String, ByVal stadiumCode As String, ByVal singleColourType As String, Optional ByVal cacheing As Boolean = True, _
                                                   Optional ByVal cacheTimeMinutes As Integer = 30) As String
            Dim outputDataTable As String = String.Empty
            Dim cacheKeyPrefix As String = GetCacheKeyPrefix(CACHEKEY_CLASSNAME, "GetSingleStadiumColour" & stadiumCode & businessUnit & singleColourType)
            Dim talentSqlAccessDetail As New TalentDataAccess

            'Permutation and Combinations Select statement
            'BusinessUnit   Stadium
            'Given          Given
            'Given          *ALL
            '*ALL           Given
            '*ALL           *ALL

            Dim whereClauseFetchHierarchy(3) As String
            Dim cacheKeyHierarchyBased(3) As String

            whereClauseFetchHierarchy(0) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(0) = ToUpper(businessUnit) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(1) = "BUSINESS_UNIT=@BusinessUnit AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(1) = ToUpper(businessUnit) & ToUpper(Utilities.GetAllString)

            whereClauseFetchHierarchy(2) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE=@StadiumCode"
            cacheKeyHierarchyBased(2) = ToUpper(Utilities.GetAllString) & ToUpper(stadiumCode)

            whereClauseFetchHierarchy(3) = "BUSINESS_UNIT='" & ReplaceSingleQuote(Utilities.GetAllString) & "' AND STADIUM_CODE='" & ReplaceSingleQuote(Utilities.GetAllString) & "'"
            cacheKeyHierarchyBased(3) = ToUpper(Utilities.GetAllString) & ToUpper(Utilities.GetAllString)

            Try
                'Construct The Call
                talentSqlAccessDetail.Settings = _settings
                talentSqlAccessDetail.Settings.Cacheing = cacheing
                talentSqlAccessDetail.Settings.CacheTimeMinutes = cacheTimeMinutes
                talentSqlAccessDetail.CommandElements.CommandExecutionType = CommandExecution.ExecuteDataSet
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@BusinessUnit", businessUnit))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@StadiumCode", stadiumCode))
                talentSqlAccessDetail.CommandElements.CommandParameter.Add(ConstructParameter("@SingleColourType", singleColourType))

                Dim sqlStatement As String = "SELECT * FROM [tbl_stadium_area_colours] WHERE [KEY_NAME] = @SingleColourType AND "
                Dim err As New ErrorObj

                'Execute the permutaions and combination till records are found
                For whereClauseFetchHierarchyCounter As Integer = 0 To 3 Step 1
                    talentSqlAccessDetail.Settings.CacheStringExtension = cacheKeyPrefix & cacheKeyHierarchyBased(whereClauseFetchHierarchyCounter)
                    talentSqlAccessDetail.CommandElements.CommandText = sqlStatement & whereClauseFetchHierarchy(whereClauseFetchHierarchyCounter)
                    err = talentSqlAccessDetail.SQLAccess()
                    If (Not (err.HasError)) And (Not (talentSqlAccessDetail.ResultDataSet Is Nothing)) Then
                        If talentSqlAccessDetail.ResultDataSet.Tables(0).Rows.Count > 0 Then
                            outputDataTable = talentSqlAccessDetail.ResultDataSet.Tables(0).Rows(0).Item("COLOUR")
                            If Not String.IsNullOrEmpty(outputDataTable) Then
                                Exit For
                            End If
                        End If
                    Else
                        Exit For
                    End If
                Next
            Catch ex As Exception
                outputDataTable = String.Empty
            Finally
                talentSqlAccessDetail = Nothing
            End Try

            'Return the results 
            Return outputDataTable
        End Function
#End Region

    End Class

End Namespace
