Imports System.Data.SqlClient
Public Class DBDeliveryCharge
    Inherits DBAccess


    Private Const GET_DELIVERY_OPTIONS As String = "GetDeliveryOptions"

    Private Const GET_DELIVERY_OPTIONS_BY_WEIGHT As String = "GetDeliveryOptionsByWeight"

    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj
        MyBase.ResultDataSet = New DataSet

        Select Case _settings.ModuleName
            Case Is = GET_DELIVERY_OPTIONS : err = GetDeliveryOptions()
            Case Is = GET_DELIVERY_OPTIONS_BY_WEIGHT : err = GetDeliveryOptionsByWeight()
        End Select

        Return err
    End Function


    Public Function GetDeliveryOptions() As ErrorObj
        '1) Select all delivery types
        '2) Switch in Country Specific Versions
        '3) Select all values from tbl_delivery that are linked to the values returned form 1 & 2
        '4) Select all price values
        Dim err As New ErrorObj
        Dim results As New DataTable
        Try
            Dim SelectStr As String = " SELECT     dt.DELIVERY_TYPE, " & _
                                        " 			dt.DESCRIPTION1, " & _
                                        " 			dt.DESCRIPTION2, " & _
                                        " 			dtl.DESCRIPTION1 As LANG_DESCRIPTION1, " & _
                                        " 			dtl.DESCRIPTION2 As LANG_DESCRIPTION2, " & _
                                        " 			d.BUSINESS_UNIT,  " & _
                                        " 			d.PARTNER,  " & _
                                        " 			d.SEQUENCE,  " & _
                                        " 			d.DELIVERY_PARENT, " & _
                                        " 			d.[DEFAULT],  " & _
                                        " 			dv.UPPER_BREAK,  " & _
                                        " 			dv.GREATER,  " & _
                                        "           dv.AVAILABLE, " & _
                                        "           dv.GROSS_VALUE, " & _
                                        "           dv.NET_VALUE, " & _
                                        "           dv.TAX_VALUE, " & _
                                        " 			dv.UPPER_BREAK_MODE  " & _
                                        " FROM         tbl_delivery_type AS dt FULL OUTER JOIN " & _
                                        "                       tbl_delivery_type_lang AS dtl ON dt.DELIVERY_TYPE = dtl.DELIVERY_TYPE INNER JOIN " & _
                                        "                       tbl_delivery AS d ON dt.DELIVERY_TYPE = d.DELIVERY_TYPE AND d.DELIVERY_TYPE_ZONE_CODE IS NULL FULL OUTER JOIN " & _
                                        "                       tbl_delivery_values AS dv ON dt.DELIVERY_TYPE = dv.DELIVERY_TYPE " & _
                                        " WHERE d.BUSINESS_UNIT = @BusinessUnit " & _
                                        " AND d.PARTNER = @Partner " & _
                                        " AND (dv.UPPER_BREAK_MODE IS NULL OR dv.UPPER_BREAK_MODE = 0) " & _
                                        " ORDER BY DELIVERY_PARENT, SEQUENCE , GREATER"

            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(Me.Settings.BackOfficeConnectionString))

            With cmd.Parameters
                .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                .Add("@Partner", SqlDbType.NVarChar).Value = Me.Settings.Partner
            End With

            Try
                cmd.Connection.Open()
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-002"
                err.ErrorMessage = ex.Message
            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(results)
                    If Not results.Rows.Count > 0 Then
                        cmd.Parameters("@Partner").Value = Utilities.GetAllString
                        da.Fill(results)
                        If Not results.Rows.Count > 0 Then
                            cmd.Parameters("@BusinessUnit").Value = Utilities.GetAllString
                            da.Fill(results)
                        End If
                    End If
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-003"
                err.ErrorMessage = ex.Message
            End Try

            Try
                cmd.Connection.Close()
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-004"
                err.ErrorMessage = ex.Message
            End Try
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACDBDEL-001"
            err.ErrorMessage = ex.Message
        End Try
        MyBase.ResultDataSet.Tables.Add(results)
        Return err

    End Function

    Public Function GetDeliveryOptionsByWeight() As ErrorObj
        '1) Select all delivery types
        '2) Switch in Country Specific Versions
        '3) Select all values from tbl_delivery that are linked to the values returned form 1 & 2
        '4) Select all price values
        Dim err As New ErrorObj
        Dim results As New DataTable
        Try
            Dim SelectStr As String = " SELECT delval.* " & _
                 " ,deltypelang.LANGUAGE_CODE, deltypelang.DESCRIPTION1 AS LANG_DESCRIPTION1, deltypelang.DESCRIPTION2 AS LANG_DESCRIPTION2 " & _
                 " ,deltype.DESCRIPTION1, deltype.DESCRIPTION2 " & _
                 " ,del.SEQUENCE, del.DELIVERY_PARENT, del.[DEFAULT], del.DELIVERY_TYPE_ZONE_CODE " & _
                 " ,coun.COUNTRY_CODE, coun.COUNTRY_DESCRIPTION " & _
                 " FROM tbl_delivery del " & _
                 " INNER JOIN tbl_delivery_type deltype ON deltype.DELIVERY_TYPE = del.DELIVERY_TYPE " & _
                 " LEFT OUTER JOIN tbl_delivery_type_lang deltypelang ON deltypelang.DELIVERY_TYPE = del.DELIVERY_TYPE " & _
                 " INNER JOIN tbl_delivery_values delval ON delval.DELIVERY_TYPE = del.DELIVERY_TYPE AND delval.BUSINESS_UNIT = del.BUSINESS_UNIT AND delval.PARTNER = del.PARTNER " & _
                 " LEFT OUTER JOIN tbl_country coun ON coun.DELIVERY_TYPE_ZONE_CODE = del.DELIVERY_TYPE_ZONE_CODE " & _
                 " WHERE del.BUSINESS_UNIT = @BusinessUnit AND del.PARTNER = @Partner " & _
                 " ORDER BY coun.COUNTRY_CODE, del.DELIVERY_PARENT, del.SEQUENCE, delval.GREATER "

            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(Me.Settings.BackOfficeConnectionString))

            With cmd.Parameters
                .Add("@BusinessUnit", SqlDbType.NVarChar).Value = Me.Settings.BusinessUnit
                .Add("@Partner", SqlDbType.NVarChar).Value = Me.Settings.Partner
            End With

            Try
                cmd.Connection.Open()
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-006"
                err.ErrorMessage = ex.Message
            End Try

            Try
                If cmd.Connection.State = ConnectionState.Open Then
                    Dim da As New SqlDataAdapter(cmd)
                    da.Fill(results)
                    If Not results.Rows.Count > 0 Then
                        cmd.Parameters("@Partner").Value = Utilities.GetAllString
                        da.Fill(results)
                        If Not results.Rows.Count > 0 Then
                            cmd.Parameters("@BusinessUnit").Value = Utilities.GetAllString
                            da.Fill(results)
                        End If
                    End If
                End If
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-007"
                err.ErrorMessage = ex.Message
            End Try

            Try
                cmd.Connection.Close()
            Catch ex As Exception
                err.HasError = True
                err.ErrorNumber = "TACDBDEL-008"
                err.ErrorMessage = ex.Message
            End Try
        Catch ex As Exception
            err.HasError = True
            err.ErrorNumber = "TACDBDEL-005"
            err.ErrorMessage = ex.Message
        End Try
        MyBase.ResultDataSet.Tables.Add(results)
        Return err

    End Function

End Class
