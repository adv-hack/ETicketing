Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This is a Base class 
'
'       Date                        Nov 2006
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'  
'       Error Number Code base      TTPCMSB- 
'                                   
'--------------------------------------------------------------------------------------------------
Public Class Cms

    Private _businessUnit As String = String.Empty
    Private _cMSUseStoredProcedures As Boolean = False
    Private _err As ErrorObj                                    ' Expose scope to outside world so values are not lost
    Private _frontEndConnectionString As String = String.Empty
    Private _keycode As String = String.Empty

    Private _pageCode As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _senderID As String = String.Empty
    Private _useStoredProcedures As Boolean = False
    Private _writeLog As Boolean = False
    '
    Protected Const MER_CACHE_NAME As String = "CMS_Menu_Cache_"
    Protected Const WFR_CACHE_NAME As String = "CMS_Webform_Cache_"
    Protected Const UCR_CACHE_NAME As String = "CMS_UserControl_Cache_"
    Protected Const _uScore As String = "_"
    '
    Protected conSql2005 As SqlConnection = Nothing
    Protected Const Param1 As String = "@Param1"
    Protected Const Param2 As String = "@Param2"
    Protected Const Param3 As String = "@Param3"
    Protected Const Param4 As String = "@Param4"
    Protected Const Param5 As String = "@Param5"
    Protected Const Param6 As String = "@Param6"

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property CMSUseStoredProcedures() As Boolean
        Get
            Return _cMSUseStoredProcedures
        End Get
        Set(ByVal value As Boolean)
            _cMSUseStoredProcedures = value
        End Set
    End Property
    Public Property Err() As ErrorObj
        Get
            Return _err
        End Get
        Set(ByVal value As ErrorObj)
            _err = value
        End Set
    End Property
    Public Property FrontEndConnectionString() As String
        Get
            Return _frontEndConnectionString
        End Get
        Set(ByVal value As String)
            _frontEndConnectionString = value
        End Set
    End Property
    Public Property PartnerCode() As String
        Get
            Return _partnerCode
        End Get
        Set(ByVal value As String)
            _partnerCode = value
        End Set
    End Property
    Public Property KeyCode() As String
        Get
            Return _keycode
        End Get
        Set(ByVal value As String)
            _keycode = value
        End Set
    End Property
    Public Property PageCode() As String
        Get
            Return _pageCode
        End Get
        Set(ByVal value As String)
            _pageCode = value
        End Set
    End Property
    Public Property SenderID() As String
        Get
            Return _senderID
        End Get
        Set(ByVal value As String)
            _senderID = value
        End Set
    End Property
    Public Property UseStoredProcedures() As Boolean
        Get
            Return _useStoredProcedures
        End Get
        Set(ByVal value As Boolean)
            _useStoredProcedures = value
        End Set
    End Property
    Public Property WriteLog() As Boolean
        Get
            Return _writeLog
        End Get
        Set(ByVal value As Boolean)
            _writeLog = value
        End Set
    End Property
    Public Property ExactValuesOnly As Boolean
    Protected Function Sql2005Open() As ErrorObj
        Dim err As New ErrorObj
        '---------------------------------------------------------------------------------
        '   Attempt to open database
        '
        Try
            If conSql2005 Is Nothing Then
                conSql2005 = New SqlConnection(FrontEndConnectionString)
                conSql2005.Open()
            ElseIf conSql2005.State <> ConnectionState.Open Then
                conSql2005 = New SqlConnection(FrontEndConnectionString)
                conSql2005.Open()
            End If
        Catch ex As Exception
            Const strError As String = "Could not establish connection to the Sql2005 database"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TTPCMSB-P1"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Protected Function Sql2005Close() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------
        '   Warning :   Using ErrorObj here when closing the DB does cause a   
        '               problem as it will clear the err object when an actual
        '               error has occured elsewhere   
        '------------------------------------------------------------------------
        Try
            If Not (conSql2005 Is Nothing) Then
                If (conSql2005.State = ConnectionState.Open) Then
                    conSql2005.Close()
                End If
            End If
        Catch ex As Exception
            Const strError4 As String = "Failed to close the Sql2005 database connection"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError4
                .ErrorNumber = "TTPCMSB-P2"
                .HasError = True
                Return err
            End With
        End Try
        Return err
    End Function

    Public Sub Clear_Cache()
        '
        AttHashtable = New Hashtable
        MultiLingualContent = New Hashtable
        MultilingualTitles = New Hashtable
        MultilingualHeaderTexts = New Hashtable
        MultilingualMetaKeywords = New Hashtable
        MultilingualMetaDescription = New Hashtable
        CmsMnuHashtable = New Hashtable
        CmsUcrHashtable = New Hashtable
        CmsWfrHashtable = New Hashtable
        MnuHashtable = New Hashtable
        UcrHashtable = New Hashtable
        WfrHashtable = New Hashtable
        '
    End Sub

End Class
