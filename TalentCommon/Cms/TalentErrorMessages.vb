Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Data
Imports System.Data.SqlClient

Public Class TalentErrorMessages
    Inherits Cms

    Private allStr As String = Utilities.GetAllString
    Private ErrorMessageFound As Boolean = False

    Private _langCode As String
    Public Property LANGUAGE_CODE() As String
        Get
            Return _langCode
        End Get
        Set(ByVal value As String)
            _langCode = value
        End Set
    End Property

    Private _errorMessages As Generic.List(Of TalentErrorMessage)
    Public Property MyErrorMessages() As Generic.List(Of TalentErrorMessage)
        Get
            Return _errorMessages
        End Get
        Set(ByVal value As Generic.List(Of TalentErrorMessage))
            _errorMessages = value
        End Set
    End Property



    Private CacheActive As Boolean = False

    Sub New(ByVal myLanguageCode As String, _
            ByVal myBusinessUnit As String, _
            ByVal myPartner As String, _
            ByVal myFrontEndConnectionString As String)

        MyBase.New()
        Me.LANGUAGE_CODE = myLanguageCode
        Me.BusinessUnit = myBusinessUnit
        Me.PartnerCode = myPartner
        Me.FrontEndConnectionString = myFrontEndConnectionString

        'Check that we have access to the HttpContext.Current Object
        'in order to use caching
        Try
            If Not System.Web.HttpContext.Current Is Nothing Then
                CacheActive = True
            End If
        Catch ex As Exception
        End Try

        Me.MyErrorMessages = New Generic.List(Of TalentErrorMessage)

    End Sub

    Public Function GetErrorMessage(ByVal myErrorCode As String) As TalentErrorMessage
        Return GetErrorMessage(allStr, allStr, myErrorCode)
    End Function

    Public Function GetErrorMessage(ByVal myModule As String, _
                                       ByVal myPageCode As String, _
                                       ByVal myErrorCode As String, _
                              Optional ByVal useDefaultMsgWhenNotFound As Boolean = True) As TalentErrorMessage


        Dim myCacheKey As String = "TalentErrorMessages_" & _
                                        Me.LANGUAGE_CODE & "_" & _
                                        Me.BusinessUnit & "_" & _
                                        Me.PartnerCode & "_" & _
                                        myModule & "_" & myPageCode & "_" & myErrorCode

        'Populate the error messages
        Try
            If CacheActive Then
                If Not Talent.Common.TalentThreadSafe.ItemIsInCache(myCacheKey) Then
                    Me.MyErrorMessages = LoadErrorMessages(myModule, myPageCode, myErrorCode)
                    HttpContext.Current.Cache.Insert(myCacheKey, Me.MyErrorMessages, Nothing, Date.MaxValue, TimeSpan.FromMinutes(30), Caching.CacheItemPriority.Normal, Nothing)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(myCacheKey)
                Else
                    Me.MyErrorMessages = CType(System.Web.HttpContext.Current.Cache(myCacheKey), Generic.List(Of TalentErrorMessage))
                End If
            Else
                Me.MyErrorMessages = LoadErrorMessages(myModule, myPageCode, myErrorCode)
            End If
        Catch ex As Exception
        End Try

        'Reset the found flag
        '---------------------------
        ErrorMessageFound = False

        'Check for match on all values
        Dim myError As New TalentErrorMessage
        If MyErrorMessages.Count > 0 Then
            myError = CheckAllAttributeVariations(myModule, myPageCode, myErrorCode)
        End If

        'Finally, if we still have no error message, attempt to get the default message
        If Not ErrorMessageFound AndAlso useDefaultMsgWhenNotFound Then
            myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, allStr, allStr, allStr, allStr)
        End If

        Return myError

    End Function


    Protected Function CheckAllAttributeVariations(ByVal myModule As String, _
                                                   ByVal myPageCode As String, _
                                                   ByVal myErrorCode As String) As TalentErrorMessage

        Dim myError As New TalentErrorMessage


        '==========================================================
        '   Most Qualified Combos
        '==========================================================
        'Check for match on all values supplied
        myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, Me.PartnerCode, myModule, myPageCode, myErrorCode)

        If Not ErrorMessageFound _
            AndAlso Not UCase(myPageCode) = UCase(allStr) Then
            'Check for match on                  LANGUAGE_CODE,    BusinessUnit,   PartnerCode,    Module,   *ALL,   ERROR_CODE
            myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, Me.PartnerCode, myModule, allStr, myErrorCode)
        End If

        If Not ErrorMessageFound _
            AndAlso Not UCase(myModule) = UCase(allStr) Then
            'Check for match on                  LANGUAGE_CODE,    BusinessUnit,     PartnerCode,  *ALL,   PageCode,   ERROR_CODE
            myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, Me.PartnerCode, allStr, myPageCode, myErrorCode)
        End If

        If Not ErrorMessageFound _
            AndAlso (Not UCase(myModule) = UCase(allStr) Or Not UCase(myPageCode) = UCase(allStr)) Then
            'Check for match on                  LANGUAGE_CODE,    BusinessUnit,    PartnerCode,   *ALL,   *ALL,   ERROR_CODE
            myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, Me.PartnerCode, allStr, allStr, myErrorCode)
        End If
        '==========================================================


        '=========================================================================
        '   *ALL on PARTNER Combos
        '   -   Only do this if the PartnerCode supplied is not *ALL already
        '       otherwise this combination will have been checked above
        '=========================================================================
        If Not UCase(Me.PartnerCode) = UCase(allStr) AndAlso Not ErrorMessageFound Then
            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    BusinessUnit,    *ALL,    Module,   PageCode,  ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, allStr, myModule, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                  LANGUAGE_CODE,     BusinessUnit,  *ALL,    Module,  *ALL,  ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, allStr, myModule, allStr, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                  LANGUAGE_CODE,    BusinessUnit,   *ALL,   *ALL,    PageCode,  ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, allStr, allStr, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                  LANGUAGE_CODE,    BusinessUnit,   *ALL,   *ALL,   *ALL,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, Me.BusinessUnit, allStr, allStr, allStr, myErrorCode)
            End If
        End If
        '=========================================================================



        '==========================================================================
        '   *ALL on BUSINESS UNIT Combos
        '    -   Only do this if the BUSINESS_UNIT supplied is not *ALL already
        '        otherwise this combination will have been checked above
        '==========================================================================
        If Not UCase(Me.BusinessUnit) = UCase(allStr) AndAlso Not ErrorMessageFound Then
            'Check for match on all values
            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,    PartnerCode,   Module,   PageCode,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, Me.PartnerCode, myModule, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,   PartnerCode,     Module,  *ALL,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, Me.PartnerCode, myModule, allStr, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,    PartnerCode,   *ALL,   PageCode,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, Me.PartnerCode, allStr, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,     *ALL,    PartnerCode,  *ALL,   *ALL,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, Me.PartnerCode, allStr, allStr, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,     *ALL,   *ALL,   Module,  PageCode,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, allStr, myModule, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,   *ALL,    Module,   *ALL,  ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, allStr, myModule, allStr, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,   *ALL,   *ALL,   PageCode,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, allStr, allStr, myPageCode, myErrorCode)
            End If

            If Not ErrorMessageFound Then
                'Check for match on                 LANGUAGE_CODE,    *ALL,   *ALL,   *ALL,   *ALL,   ERROR_CODE
                myError = CheckCollectionForMessage(Me.LANGUAGE_CODE, allStr, allStr, allStr, allStr, myErrorCode)
            End If
        End If
        '==========================================================================

        Return myError
    End Function

    Protected Function CheckCollectionForMessage(ByVal LanguageCodeToTest As String, _
                                                   ByVal BusinessUnitToTest As String, _
                                                   ByVal PartnerToTest As String, _
                                                   ByVal ModuleToTest As String, _
                                                   ByVal PageCodeToTest As String, _
                                                   ByVal ErrorCodeToTest As String) As TalentErrorMessage

        Dim myError As New TalentErrorMessage

        'Reset the found flag
        '---------------------------
        ErrorMessageFound = False

        'Attempt to match an error message on the supplied criteria
        '-----------------------------------------------------------
        For Each tem As TalentErrorMessage In MyErrorMessages
            If UCase(tem.LANGUAGE_CODE) = UCase(LanguageCodeToTest) _
                AndAlso UCase(tem.BUSINESS_UNIT) = UCase(BusinessUnitToTest) _
                    AndAlso UCase(tem.PARTNER_CODE) = UCase(PartnerToTest) _
                        AndAlso UCase(tem.MODULE_) = UCase(ModuleToTest) _
                            AndAlso UCase(tem.PAGE_CODE) = UCase(PageCodeToTest) _
                                AndAlso UCase(tem.ERROR_CODE) = UCase(ErrorCodeToTest) Then
                myError = tem
                ErrorMessageFound = True
                Exit For
            End If
        Next

        Return myError
    End Function


    Private Function LoadErrorMessages(ByVal myModule As String, _
                                       ByVal myPageCode As String, _
                                       ByVal myErrorCode As String) As Generic.List(Of TalentErrorMessage)

        Dim myErrors As New Generic.List(Of TalentErrorMessage)

        Dim selectStr As String = " SELECT * " & _
                                    " FROM tbl_error_messages WITH (NOLOCK) " & _
                                    " WHERE LANGUAGE_CODE IN (@LANGUAGE_CODE, @ALL_STRING) " & _
                                    " AND BUSINESS_UNIT IN (@BUSINESS_UNIT, @ALL_STRING) " & _
                                    " AND PARTNER_CODE IN (@PARTNER_CODE, @ALL_STRING) " & _
                                    " AND MODULE IN (@MODULE, @ALL_STRING) " & _
                                    " AND PAGE_CODE IN (@PAGE_CODE, @ALL_STRING) " & _
                                    " AND ERROR_CODE IN (@ERROR_CODE, @ALL_STRING) " & _
                                    ""

        Dim messages As New DataTable

        Try
            Me.Sql2005Open()
        Catch ex As Exception
        End Try

        If Me.conSql2005.State = ConnectionState.Open Then

            Dim myCmd As New SqlCommand(selectStr, Me.conSql2005)

            Try
                With myCmd.Parameters
                    .Clear()
                    .Add("@LANGUAGE_CODE", SqlDbType.NVarChar).Value = Me.LANGUAGE_CODE
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = Me.BusinessUnit
                    .Add("@PARTNER_CODE", SqlDbType.NVarChar).Value = Me.PartnerCode
                    .Add("@MODULE", SqlDbType.NVarChar).Value = myModule
                    .Add("@PAGE_CODE", SqlDbType.NVarChar).Value = myPageCode
                    .Add("@ERROR_CODE", SqlDbType.NVarChar).Value = myErrorCode
                    .Add("@ALL_STRING", SqlDbType.NVarChar).Value = allStr
                End With

                Dim da As New SqlDataAdapter(myCmd)
                da.Fill(messages)
            Catch ex As Exception
                Me.Err = New ErrorObj
                Me.Err.HasError = True
                Me.Err.ErrorMessage = ex.Message
                Me.Err.ErrorNumber = "TCCMSLEM-1"
            End Try

        End If

        Try
            Me.Sql2005Close()
        Catch ex As Exception
        End Try

        'Popultate the myErrors generic list from the data table
        If messages.Rows.Count > 0 Then
            Dim properties As ArrayList = Utilities.GetPropertyNames(New TalentErrorMessage)
            For Each message As DataRow In messages.Rows

                'Populate TalentErrorMessage from this row
                Dim myTem As New TalentErrorMessage
                myTem = Utilities.PopulateProperties(properties, message, myTem)

                'Add the error code to the end of the description when its the default message
                If UCase(myTem.LANGUAGE_CODE) = UCase(Me.LANGUAGE_CODE) _
                    AndAlso UCase(myTem.BUSINESS_UNIT) = UCase(allStr) _
                        AndAlso UCase(myTem.PARTNER_CODE) = UCase(allStr) _
                            AndAlso UCase(myTem.MODULE_) = UCase(allStr) _
                                AndAlso UCase(myTem.PAGE_CODE) = UCase(allStr) _
                                    AndAlso UCase(myTem.ERROR_CODE) = UCase(allStr) Then
                    myTem.ERROR_MESSAGE += " (" & myErrorCode & ")"
                End If

                'Add to the list
                If Not myErrors.Contains(myTem) Then myErrors.Add(myTem)
            Next
        End If

        Return myErrors
    End Function

End Class
