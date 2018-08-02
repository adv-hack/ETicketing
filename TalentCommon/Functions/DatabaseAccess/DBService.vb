Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with service and control calls to the DB
'
'       Date                        8th Jan 2007
'
'       Author                      Andy White   
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBSV- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBService
    Inherits DBAccess
    '
    Private _businessUnit As String = "TRADING_PORTAL"
    Private _waitTime As Long = 0
    Private _waitMinutes As Long = 0
    Private _webServices As DataTable
    Public SqlServer2005 As String = String.Empty

    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property WaitMinutes() As Long
        Get
            Return _waitMinutes
        End Get
        Set(ByVal value As Long)
            _waitMinutes = value
        End Set
    End Property
    Public Property WebServices() As DataTable
        Get
            Return _webServices
        End Get
        Set(ByVal value As DataTable)
            _webServices = value
        End Set
    End Property
    Private _dtPartnerWS As New DataTable("DtPartnerWS")
    Public Property DtPartnerWS() As DataTable
        Get
            Return _dtPartnerWS
        End Get
        Set(ByVal value As DataTable)
            _dtPartnerWS = value
        End Set
    End Property

    Public Function GetPartnerWebServices(ByVal webServiceName As String) As ErrorObj
        Dim err As New ErrorObj
        Dim _partnerWebServices As SqlDataReader = Nothing
        DtPartnerWS.Clear()

        If DtPartnerWS.Columns.Count = 0 Then
            With DtPartnerWS.Columns
                .Add("FOLDER1", GetType(String))
                .Add("FOLDER2", GetType(String))
                .Add("PARTNER", GetType(String))
                .Add("LOGIN_ID", GetType(String))
                .Add("CWS_EMAIL", GetType(String))
                .Add("C_EMAIL", GetType(String))
                .Add("AUTO_PROCESS_SPLIT", GetType(String))
                .Add("EMAIL_XML_RESPONSE", GetType(String))
                .Add("PASSWORD", GetType(String))
            End With
        End If
        '-------------------------------------------------------------------
        If ResultDataSet Is Nothing Then
            ResultDataSet = New DataSet
            ResultDataSet.Tables.Add(DtPartnerWS)
        End If

        Dim dr As Data.DataRow
        '-------------------------------------------------------------------------------
        Const strService As String = _
                         " SELECT smd.OUTGOING_XML_RESPONSE AS x1, " & _
                         "        smpd.OUTGOING_XML_RESPONSE AS x2, " & _
                         "        au.PARTNER, au.LOGINID, smpd.EMAIL AS cwsEMAIL, " & _
                         "        p.EMAIL AS cEMAIL, smpd.AUTO_PROCESS_SPLIT, " & _
                         "        smpd.EMAIL_XML_RESPONSE, au.PASSWORD " & _
                         " FROM  tbl_partner AS p WITH (NOLOCK)  " & _
                         "  INNER JOIN tbl_authorized_users AS au WITH (NOLOCK)  " & _
                         "  INNER JOIN tbl_supplynet_module_partner_defaults AS smpd WITH (NOLOCK)  " & _
                         "      ON au.PARTNER = smpd.PARTNER " & _
                         "      ON p.PARTNER = au.PARTNER " & _
                         "  INNER JOIN tbl_supplynet_module_defaults AS smd WITH (NOLOCK)  " & _
                         "      ON smpd.MODULE = smd.MODULE " & _
                         " WHERE smd.module = @Param1 " & _
                         "  AND smd.business_unit = @Param2 " & _
                         "  AND au.business_unit = @Param3 " & _
                         "  AND au.AUTO_PROCESS_DEFAULT_USER = 1 "
        '-------------------------------------------------------------------------------
        Try
            Settings.FrontEndConnectionString = SqlServer2005
            err = Sql2005Open()
            If Not err.HasError Then
                Dim cmdService As SqlCommand = New SqlCommand(strService.Trim, conSql2005)
                With cmdService
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = webServiceName.Trim
                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = _businessUnit
                    .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = _businessUnit
                    _partnerWebServices = .ExecuteReader()

                    While _partnerWebServices.Read()
                        dr = DtPartnerWS.NewRow
                        dr("FOLDER1") = _partnerWebServices("x1").ToString
                        dr("FOLDER2") = _partnerWebServices("x2").ToString
                        dr("PARTNER") = _partnerWebServices("PARTNER").ToString
                        dr("LOGIN_ID") = _partnerWebServices("LOGINID").ToString
                        dr("CWS_EMAIL") = _partnerWebServices("cwsEMAIL").ToString
                        dr("C_EMAIL") = _partnerWebServices("cEMAIL").ToString
                        dr("AUTO_PROCESS_SPLIT") = _partnerWebServices("AUTO_PROCESS_SPLIT").ToString
                        dr("EMAIL_XML_RESPONSE") = _partnerWebServices("EMAIL_XML_RESPONSE").ToString
                        dr("PASSWORD") = _partnerWebServices("PASSWORD").ToString
                        DtPartnerWS.Rows.Add(dr)

                    End While

                End With
            End If
            err = Sql2005Close()
            'conSql2005 = Nothing

        Catch ex As Exception
            With err
                .ErrorStatus = String.Empty
                .ErrorMessage = ex.Message
                .ErrorNumber = "TACDBSV-03"
                .HasError = True
            End With
        Finally
            Try
                _partnerWebServices.Close()
                _partnerWebServices = Nothing

                err = Sql2005Close()
            Catch ex As Exception

            End Try

        End Try
        Return err

    End Function
    Public Function GetWaitMinutes() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Const strSelect As String = "SELECT AUTO_PROCESS_WAIT_PERIOD_MINUTES " & _
                                        " FROM tbl_supplynet_defaults WITH (NOLOCK)  " & _
                                        " WHERE BUSINESS_UNIT = @Param1 "
        Try
            Settings.FrontEndConnectionString = SqlServer2005
            err = Sql2005Open()
            If Not err.HasError Then
                Dim dr As SqlDataReader = Nothing
                Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = BusinessUnit
                    dr = .ExecuteReader()
                End With
                If dr.HasRows Then
                    dr.Read()
                    WaitMinutes = Convert.ToInt32(dr("AUTO_PROCESS_WAIT_PERIOD_MINUTES"))
                Else
                    WaitMinutes = 5
                End If
                dr = Nothing
            End If
            err = Sql2005Close()

        Catch ex As Exception
            With err
                .ErrorStatus = String.Empty
                .ErrorMessage = ex.Message
                .ErrorNumber = "TACDBSV-01"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Public Function GetWebServices() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Const strSelect As String = " SELECT * FROM tbl_supplynet_module_defaults WITH (NOLOCK)  " & _
                                      " WHERE AUTO_PROCESS = 1  " & _
                                      " AND BUSINESS_UNIT = @Param1"
        Dim dr As SqlDataReader = Nothing
        WebServices = New DataTable
        With WebServices.Columns
            .Add("Name", GetType(String))
            .Add("Wait", GetType(Long))
            .Add("LastRun", GetType(DateTime))
        End With
        Try
            Settings.FrontEndConnectionString = SqlServer2005
            err = Sql2005Open()
            If Not err.HasError Then
                Dim cmdSelect As SqlCommand = New SqlCommand(strSelect, conSql2005)
                With cmdSelect
                    .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = _businessUnit
                    dr = .ExecuteReader()
                End With
                '
                If dr.HasRows Then
                    While dr.Read
                        _waitTime = dr.Item("AUTO_PROCESS_WAIT_PERIOD_MINUTES") + 0
                        If _waitTime = 0 Then _waitTime = WaitMinutes
                        WebServices.Rows.Add(dr.Item("MODULE"), _waitTime, DateAdd(DateInterval.Hour, -1, Now))
                    End While
                End If
            End If
            dr = Nothing
            err = Sql2005Close()

        Catch ex As Exception
            With err
                .ErrorStatus = String.Empty
                .ErrorMessage = ex.Message
                .ErrorNumber = "TACDBSV-04"
                .HasError = True
            End With
        End Try
        Return err
        '
    End Function

End Class
