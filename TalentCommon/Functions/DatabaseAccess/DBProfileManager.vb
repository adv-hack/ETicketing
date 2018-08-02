Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports System.Text
Imports IBM.Data.DB2.iSeries
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Profile Manager Requests
'
'       Date                        24/11/08
'
'       Author                      Ben Ford
'
'       CS Group 2007               All rights reserved.
'
'       Error Number Code base      TACDBPMR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBProfileManager
    Inherits DBAccess

    Private _dtHeader As New DataTable
    Private InvoiceNumber As String = String.Empty
    Private CompanyNumber As String = String.Empty
    Private PoNumber As String = String.Empty

    Private _profileManagerTransactions As DEProfileManagerTransactions
    Public Property ProfileManagerTransactions() As DEProfileManagerTransactions
        Get
            Return _profileManagerTransactions
        End Get
        Set(ByVal value As DEProfileManagerTransactions)
            _profileManagerTransactions = value
        End Set
    End Property

    Public Property DtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseTALENTCRM() As ErrorObj
        Dim err As New ErrorObj
        Select Case _settings.ModuleName

            Case Is = "ReceiveProfileManagerTransactions"
                err = AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions()
        End Select
        Return err
    End Function

  
    Protected Function AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions() As ErrorObj

        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '
        '---------------------
        ' Setup Calls to As400
        '---------------------
        Dim SqlHead As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                "/RCVPMHDR(@PARAM1, @PARAM2)"
        Dim SqlItem As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                "/RCVPMDTL(@PARAM1, @PARAM2)"

        Dim cmdHead As iDB2Command = Nothing
        Dim cmdItem As iDB2Command = Nothing

        Dim iItems As Integer = 0
        Dim iCount As Integer = 0

        Dim parmInput, parmOutput As iDB2Parameter
        Dim PARMOUT As String = String.Empty
        Dim parmCompanyCode As String = String.Empty
        Dim parmSupplierCode As String = String.Empty
        Dim parmPoNumber As String = String.Empty

        Dim strGeneratedHeaderId As String = String.Empty
        '--------------------------
        ' Loop through transactions 
        '-------------------------- 
        Try
            For Each transaction As DEProfileManagerTransaction In ProfileManagerTransactions.CollDEHeader
                strGeneratedHeaderId = String.Empty
                '-------------------------------------------------------
                ' Transaction detail is written first so that incomplete 
                ' transactions are not picked up by profile manager
                '-------------------------------------------------------
                For Each item As DEProfileManagerTransactionLine In transaction.ColTransactionLines
                    '---------------------------------------------------------
                    ' Call Profile Manager Detail procedure to write to Talent  
                    '---------------------------------------------------------
                    cmdItem = New iDB2Command(SqlItem, conTALENTCRM)
                    With cmdItem
                        PARMOUT = String.Empty
                        parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        parmInput.Value = AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions_BuildParmItem(item, strGeneratedHeaderId)
                        parmInput.Direction = ParameterDirection.Input
                        parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        parmOutput.Value = String.Empty
                        parmOutput.Direction = ParameterDirection.InputOutput
                        .ExecuteNonQuery()
                        PARMOUT = .Parameters(Param2).Value.ToString
                    End With
                    If strGeneratedHeaderId.Trim = String.Empty Then
                        strGeneratedHeaderId = Utilities.FixStringLength(PARMOUT.Substring(0, 13), 13)
                    End If

                    If PARMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ItemErrorMessage(iCount) = PARMOUT.Substring(1020, 4)
                            .ItemErrorCode(iCount) = "TACDBPMR-17"
                            .ItemErrorStatus(iCount) = "Error creating Profile manager item"
                            .ErrorMessage = PARMOUT.Substring(1019, 4)
                            .ErrorNumber = "TACDBPMR-17"
                            .ErrorStatus = "Error creating profile manager item - " & err.ErrorMessage & _
                                         " - " & Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", err.ErrorMessage) & _
                                         " - HEADER, PRODUCT: " & item.DetailHeaderId & "," & item.ProductCode
                            .HasError = True
                            Return err
                        End With
                    End If
                Next item

                '------------------------------------------------
                ' Call header stored procedure to write to Talent  
                ' - Only if it has transaction lines
                '------------------------------------------------
                If transaction.ColTransactionLines.Count > 0 Then
                    cmdHead = New iDB2Command(SqlHead, conTALENTCRM)
                    With cmdHead
                        parmInput = .Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        parmInput.Value = AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions_BuildParmHead(transaction, strGeneratedHeaderId)
                        parmInput.Direction = ParameterDirection.Input
                        parmOutput = .Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        parmOutput.Value = String.Empty
                        parmOutput.Direction = ParameterDirection.InputOutput
                        .ExecuteNonQuery()
                        PARMOUT = .Parameters(Param2).Value.ToString
                    End With

                    If PARMOUT.Substring(1023, 1) = "Y" Then
                        With err
                            .ItemErrorMessage(iCount) = PARMOUT.Substring(1020, 4)
                            .ItemErrorCode(iCount) = "TACDBPMR-16"
                            .ItemErrorStatus(iCount) = "Error creating Profile Manager header " & PARMOUT.Substring(1019, 4)
                            .ErrorMessage = PARMOUT.Substring(1019, 4)
                            .ErrorNumber = "TACDBPMR-16"
                            .ErrorStatus = "Error creating Profile Manager header " & PARMOUT.Substring(1019, 4) & " - " & Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4)) & _
                                             " - HEADER: " & transaction.HeaderID
                            .HasError = True
                            Return err
                        End With
                    Else

                    End If
                End If

            Next

        Catch ex As Exception
            Const strError3 As String = "Error during database access"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError3
                .ErrorNumber = "TACDBPMR-19"
                .HasError = True
                Return err
            End With
        End Try

        Return err
    End Function
    Protected Function AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions_BuildParmHead(ByVal transaction As DEProfileManagerTransaction, ByVal strGeneratedHeaderId As String) As String
        Dim parmHead As String = String.Empty
        Dim sb As New StringBuilder

        With sb
            .Append(Utilities.FixStringLength(strGeneratedHeaderId, 13))
            .Append(transaction.HeaderID.PadLeft(13, "0"))
            .Append(Utilities.FixStringLength(transaction.SourceSystemID, 10))
            .Append(Utilities.FixStringLength(transaction.SourceRecordType, 10))
            .Append(Utilities.FixStringLength(transaction.RecordEntryDate, 8))
            .Append(Utilities.FixStringLength(transaction.RecordEntryTime, 6))
            .Append(Utilities.FixStringLength(transaction.RecordEntryMethod, 10))
            .Append(Utilities.FixStringLength(transaction.HeaderUnitPrice, 18))
            .Append(Utilities.FixStringLength(transaction.HeaderTotalPrice, 18))
            .Append(Utilities.FixStringLength(transaction.HeaderVatValue, 18))
            .Append(Utilities.FixStringLength(transaction.HeaderMargin, 18))
            .Append(Utilities.FixStringLength(transaction.HeaderTotalQuantity, 16))
            .Append(Utilities.FixStringLength(transaction.SourceCustomerID, 13))
            .Append(Utilities.FixStringLength(transaction.TalentCustomerId, 13))
            .Append(Utilities.FixStringLength(transaction.TalentContactId, 13))
            .Append(Utilities.FixStringLength(transaction.MemberNo.PadLeft(12, "0"), 12))
            .Append(Utilities.FixStringLength(transaction.NoteType, 12))
            .Append(Utilities.FixStringLength(transaction.ActionType, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute1, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute2, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute3, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute4, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute5, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute6, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute7, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute8, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute9, 12))
            .Append(Utilities.FixStringLength(transaction.Attribute10, 12))

        End With
        parmHead = sb.ToString

        Return parmHead
    End Function
    Protected Function AccessDataBaseTALENTCRM_ReceiveProfileManagerTransactions_BuildParmItem(ByVal item As DEProfileManagerTransactionLine, ByVal strGeneratedHeaderId As String) As String
        Dim parmItem As String = String.Empty
        Dim sb As New StringBuilder

        With sb
            .Append(Utilities.FixStringLength(strGeneratedHeaderId, 13))
            .Append(item.DetailHeaderId.PadLeft(13, "0"))
            .Append(Utilities.FixStringLength(item.DetailSourceSystemId, 10))
            .Append(Utilities.FixStringLength(item.DetailSourceRecordType, 10))
            .Append(Utilities.FixStringLength(item.Type, 4))
            .Append(Utilities.FixStringLength(item.LineDate, 8))
            .Append(Utilities.FixStringLength(item.LineTime, 6))
            .Append(Utilities.FixStringLength(item.Agent, 10))
            .Append(Utilities.FixStringLength(item.SaleLocation, 30))
            .Append(Utilities.FixStringLength(item.ProductCode, 30))
            .Append(Utilities.FixStringLength(item.ProductCategory1, 12))
            .Append(Utilities.FixStringLength(item.ProductCategory2, 12))
            .Append(Utilities.FixStringLength(item.ProductCategory3, 12))
            .Append(Utilities.FixStringLength(item.ProductCategory4, 12))
            .Append(Utilities.FixStringLength(item.ProductCategory5, 12))
            .Append(Utilities.FixStringLength(item.ProductCategory6, 12))
            .Append(Utilities.FixStringLength(item.ProductDescription, 50))
            .Append(Utilities.FixStringLength(item.ProductSupplier, 30))
            .Append(Utilities.FixStringLength(item.Quantity, 16))
            .Append(Utilities.FixStringLength(item.UnitPrice, 18))
            .Append(Utilities.FixStringLength(item.TotalPrice, 18))
            .Append(Utilities.FixStringLength(item.VatValue, 18))
            .Append(Utilities.FixStringLength(item.LineNumber, 5))
            .Append(Utilities.FixStringLength(item.PaymentMethod, 12))
            .Append(Utilities.FixStringLength(item.CreditCardType, 12))
            .Append(Utilities.FixStringLength(item.Margin, 18))
            .Append(Utilities.FixStringLength(item.UOM, 6))
            .Append(Utilities.FixStringLength(item.ConversionFactor, 18))
            .Append(Utilities.FixStringLength(item.Currency, 3))
            .Append(Utilities.FixStringLength(item.Campaign, 13))
            .Append(Utilities.FixStringLength(item.CampaignCode, 12))
            .Append(Utilities.FixStringLength(item.EventCode, 12))
            .Append(Utilities.FixStringLength(item.SpecificDetail, 100))
            .Append(Utilities.FixStringLength(item.DiscountValue, 18))
            .Append(Utilities.FixStringLength(item.NoteType, 12))
            .Append(Utilities.FixStringLength(item.ActionType, 12))

        End With
        parmItem = sb.ToString

        Return parmItem
    End Function

End Class

