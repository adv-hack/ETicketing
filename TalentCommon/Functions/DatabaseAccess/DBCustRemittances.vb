Imports Microsoft.VisualBasic
Imports System.Data.SqlClient
Imports IBM.Data.DB2.iSeries
Imports System.Text
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Customer Remittances Requests
'
'       Date                        Apr 2007
'
'       Author                      Andy White 
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDBCRA- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DBCustRemittances
    Inherits DBAccess
    Private _der As New DeRemittances
    Private _parmHEAD(500) As String
    Private _parmITEM(500, 500) As String
    Private _remitLineNo As Int32
    Public Property Der() As DeRemittances
        Get
            Return _der
        End Get
        Set(ByVal value As DeRemittances)
            _der = value
        End Set
    End Property

    Protected Overrides Function AccessDataBaseSystem21() As ErrorObj
        '---------------------------------------------------------------------------
        '   AccessDataBaseSystem21: Write the order to System 21 via Stored Procedures
        '
        Dim err As New ErrorObj
        Dim sProcError As String = String.Empty
        '---------------------------------------------------------------------------
        '   Open up collection and deal with by putting to stored procedures parameters 
        '
        Dim collDates As New Collection
        collDates.Add(Date.Now)

        '--------------------------------------------------------------------------------------------------
        '   Three SQL stored procedures, say RCVRMTHDR, RCVRMTLIN and RCVRMTEND.  
        '   The first of these receives the remittance header and writes a record to XMRIP100.  
        '   The next receives each remittance line, storing the detail in XMRIP200.  And finally  
        '   RCVRMTEND gets control at the end of each remittance, copying the data into CSP67 and CSP68. 
        '--------------------------------------------------------------------------------------------------

        err = DataEntityUnPackSystem21()
        If Not err.HasError Then
            '------------------------------------------------------------------
            ' Setup Calls to As400
            ' 
            Dim strHeader As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/RCVRMTHDR(@PARAM1, @PARAM2)"
            Dim strLines As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/RCVRMTLIN(@PARAM1, @PARAM2)"
            Dim strEnd As String = "CALL " & Settings.StoredProcedureGroup.Trim & _
                                                "/RCVRMTEND(@PARAM1, @PARAM2)"

            Dim cmdSelectHeader As iDB2Command = Nothing
            Dim cmdSelectLines As iDB2Command = Nothing
            Dim cmdSelectEnd As iDB2Command = Nothing

            Dim iHeader As Integer = 0
            Dim iItems As Integer = 0

            Dim ParamInput, ParamOutput As iDB2Parameter
            Dim PARMOUT As String = String.Empty

            Dim strTalentNumberNo As String = String.Empty
            '----------------------------------------------------------------
            '   Loop through Remittances in transaction
            '
            Try
                For iHeader = 1 To 500
                    If ParmHEAD(iHeader) > String.Empty Then
                        '----------------------------------------------------------------
                        '   Call Remittance  Header stored procedure towrite to Talent 
                        '
                        cmdSelectHeader = New iDB2Command(strHeader, conSystem21)

                        ParamInput = cmdSelectHeader.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                        ParamInput.Value = ParmHEAD(iHeader)
                        ParamInput.Direction = ParameterDirection.Input

                        ParamOutput = cmdSelectHeader.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                        ParamOutput.Value = String.Empty
                        ParamOutput.Direction = ParameterDirection.InputOutput
                        cmdSelectHeader.ExecuteNonQuery()
                        ' Interpret results
                        PARMOUT = cmdSelectHeader.Parameters(Param2).Value.ToString
                        strTalentNumberNo = Utilities.FixStringLength(PARMOUT.Substring(0, 15), 15)
                        ' Error? 
                        If PARMOUT.Substring(1023, 1) = "Y" Then
                            With err
                                .ItemErrorMessage(iHeader) = String.Empty
                                .ItemErrorCode(iHeader) = "TACDBCRA-16"
                                .ItemErrorStatus(iHeader) = "Error creating Remittance header - " & _
                                                    PARMOUT.Substring(1019, 4) & "-" & _
                                                    Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                    "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                                If Der.CollDeRemittHeader.Count = 1 Then
                                    .ErrorMessage = PARMOUT.Substring(1019, 4)
                                    .ErrorStatus = "Error creating Remittance header - " & _
                                                    PARMOUT.Substring(1019, 4) & "-" & _
                                                    Descriptions.GetDescription(Settings.FrontEndConnectionString, _
                                                    "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4))
                                    .ErrorNumber = "TACDBCRA-03"
                                    .HasError = True
                                End If
                            End With
                        Else
                            '----------------------------------------------------------------
                            '   Loop through items in current Number
                            ' 
                            For iItems = 1 To 500
                                If ParmITEM(iHeader, iItems) > String.Empty Then
                                    '---------------------------------------------------------
                                    '   Call Number detail stored procedure to write to Talent (NumberREQ2)
                                    ' 
                                    cmdSelectLines = New iDB2Command(strLines, conSystem21)
                                    Dim strItemLine As String = ParmITEM(iHeader, iItems)
                                    PARMOUT = String.Empty
                                    ParamInput = cmdSelectLines.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                                    '          ParamInput.Value = strTalentNumberNo & strItemLine.Substring(15)
                                    ParamInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                                            strItemLine
                                    ParamInput.Direction = ParameterDirection.Input

                                    ParamOutput = cmdSelectLines.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                                    ParamOutput.Value = String.Empty
                                    ParamOutput.Direction = ParameterDirection.InputOutput
                                    cmdSelectLines.ExecuteNonQuery()
                                    PARMOUT = cmdSelectLines.Parameters(Param2).Value.ToString
                                    If PARMOUT.Substring(1023, 1) = "Y" Then
                                        With err
                                            .ItemErrorMessage(iHeader) = String.Empty
                                            .ItemErrorCode(iHeader) = "TACDBCRA-17"
                                            .ItemErrorStatus(iHeader) = "Invalid item line - " & PARMOUT.Substring(1019, 4) & "-" & _
                                                            Descriptions.GetDescription(Settings.FrontEndConnectionString, "ENG", "ERRORCODE", PARMOUT.Substring(1019, 4)) & _
                                                            " (" & strItemLine.Substring(15, 15).Trim & ")"
                                        End With
                                    End If
                                Else
                                    Exit For
                                End If
                            Next iItems
                            '-----------------------------------------------------------
                            '   Last item now written - if no error then write Number to System 21 (WRITEORD)
                            ' 
                            cmdSelectEnd = New iDB2Command(strEnd, conSystem21)
                            Dim strWriteOrd As String = String.Empty
                            PARMOUT = String.Empty
                            ParamInput = cmdSelectEnd.Parameters.Add(Param1, iDB2DbType.iDB2Char, 1024)
                            ParamInput.Value = Utilities.FixStringLength(Settings.AccountNo3, 2) & _
                                                ParmHEAD(iHeader).Substring(14, 15)
                            ParamInput.Direction = ParameterDirection.Input

                            ParamOutput = cmdSelectEnd.Parameters.Add(Param2, iDB2DbType.iDB2Char, 1024)
                            ParamOutput.Value = String.Empty
                            ParamOutput.Direction = ParameterDirection.InputOutput
                            cmdSelectEnd.ExecuteNonQuery()
                            PARMOUT = cmdSelectEnd.Parameters(Param2).Value.ToString
                            '---------------------------------------------------------
                            '   If no error then create results data set
                            '
                            If PARMOUT.Substring(1023, 1).Equals("Y") Then
                                Const strError2 As String = "Error writing to System 21"
                                With err
                                    .ErrorMessage = String.Empty
                                    .ErrorStatus = strError2
                                    .ErrorNumber = "TACDBCRA02"
                                    .HasError = True
                                End With

                            End If
                        End If
                    Else
                        Exit For
                    End If
                Next iHeader
            Catch ex As Exception
                Const strError3 As String = "Error during database access"
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError3
                    .ErrorNumber = "TACDBCRA-03"
                    .HasError = True
                    Return err
                End With
            End Try

        End If
        Return err
    End Function

    Private Function DataEntityUnPackSystem21() As ErrorObj
        Dim err As New ErrorObj
        '------------------------------------------------------------------------------
        '   put the parameter generation in its own function as it is likely to be used  
        '   several times
        '
        Dim derh As New DeRemittanceHeader
        Dim derl As New DeRemittanceLines
        Dim detr As New DETransaction           ' Items
        ' 
        Dim iHeader As Integer = 0
        Dim iLines As Integer = 0
        Dim iCount As Integer = 0
        '
        Try
            With Der
                detr = .CollDETrans.Item(1)
                '-----------------------------------------------------------------------------------------
                For iHeader = 1 To .CollDeRemittHeader.Count

                    derh = .CollDeRemittHeader.Item(iHeader)
                    ParmHEAD(iHeader) = BuildParmHEAD(derh)
                    iLines = 0

                    For iCount = 1 To .CollDERemittLines.Count
                        derl = .CollDERemittLines.Item(iCount)
                        If derl.RemittanceHeader.Equals(derh.RemittanceHeader) Then
                            iLines += 1
                            ParmITEM(iHeader, iLines) = BuildParmITEM(derl, derh)
                        End If
                    Next

                Next iHeader
            End With
        Catch ex As Exception
            Const strError17 As String = "Could not Unpack Data Entity "
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError17
                err.ErrorNumber = "TACDBOR17"
                err.HasError = True
            End With

        End Try
        Return err
    End Function
    Private Function BuildParmHEAD(ByVal derh As DeRemittanceHeader) As String
        '------------------------------------------------------------------------------
        ' BuildParmpayment - Build IN parameter for Write Order  stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------
        'Dim parmCompanyCode As String = Utilities.FixStringLength(derh.CompanyCode, 2)
        Dim parmCompanyCode As String = Utilities.FixStringLength(Settings.AccountNo3, 2)

        Dim parmSystemNo As String = Utilities.FixStringLength(derh.CompanyCode, 2)
        Dim parmAllocationNo As String = Utilities.FixStringLength(derh.CompanyCode, 2)
        Dim parmSequenceNo As String = Utilities.FixStringLength(derh.CompanyCode, 2)

        Dim parmBankAccountNo As String = Utilities.FixStringLength(derh.BankAccountNo, 12)
        Dim parmSOPorderNo As String = Utilities.FixStringLength(derh.SOPorderNo, 15)
        Dim parmBankReference As String = Utilities.FixStringLength(derh.BankReference, 15)
        Dim parmPaymentMethod As String = Utilities.FixStringLength(derh.PaymentMethod, 3)
        Dim parmPostingDate As String = Utilities.FixStringLength(derh.PostingDate, 10)                                  ' *cymd	7 S 0	
        Dim parmCurrencyCode As String = Utilities.FixStringLength(derh.CurrencyCode, 3)
        Dim parmCurrencyValue As String = Utilities.FixStringLength(derh.CurrencyValue, 17)                             ' 17 P 2
        Dim parmConfirmedBaseCurrencyValue As String = Utilities.FixStringLength(derh.ConfirmedBaseCurrencyValue, 17)   '17 P 2
        Dim parmCustomerBankCode As String = Utilities.FixStringLength(derh.CustomerBankCode, 35)
        Dim parmThirdPartyName As String = Utilities.FixStringLength(derh.ThirdPartyName, 35)
        Dim parmThirdPartyAddressLine1 As String = Utilities.FixStringLength(derh.ThirdPartyAddressLine1, 35)
        Dim parmThirdPartyAddressLine2 As String = Utilities.FixStringLength(derh.ThirdPartyAddressLine2, 35)
        Dim parmThirdPartyAddressLine3 As String = Utilities.FixStringLength(derh.ThirdPartyAddressLine3, 35)
        Dim parmThirdPartyAddressLine4 As String = Utilities.FixStringLength(derh.ThirdPartyAddressLine4, 35)
        Dim parmThirdPartyAddressLine5 As String = Utilities.FixStringLength(derh.ThirdPartyAddressLine5, 35)
        Dim parmThirdPartyPostcode As String = Utilities.FixStringLength(derh.ThirdPartyPostcode, 10)
        Dim parmOriginatingAccountName As String = Utilities.FixStringLength(derh.OriginatingAccountName, 35)
        Dim parmOurBankDetails As String = Utilities.FixStringLength(derh.OurBankDetails, 35)
        Dim parmThirdPartyCountry As String = Utilities.FixStringLength(derh.ThirdPartyCountry, 3)
        Dim parmOurBankCountryCode As String = Utilities.FixStringLength(derh.OurBankCountryCode, 3)
        '------------------------------------------------------------------------------
        Dim sb As New StringBuilder
        With sb
            .Append(parmCompanyCode)
            .Append(parmBankAccountNo)

            .Append(parmSOPorderNo)
            .Append(parmBankReference)
            .Append(parmPaymentMethod)
            .Append(parmPostingDate)
            .Append(parmCurrencyCode)
            .Append(parmCurrencyValue)
            .Append(parmConfirmedBaseCurrencyValue)
            .Append(parmCustomerBankCode)
            .Append(parmThirdPartyName)
            .Append(parmThirdPartyAddressLine1)
            .Append(parmThirdPartyAddressLine2)
            .Append(parmThirdPartyAddressLine3)
            .Append(parmThirdPartyAddressLine4)
            .Append(parmThirdPartyAddressLine5)
            .Append(parmThirdPartyPostcode)
            .Append(parmOriginatingAccountName)
            .Append(parmOurBankDetails)
            .Append(parmThirdPartyCountry)
            .Append(parmOurBankCountryCode)
        End With

        Return sb.ToString

    End Function

    Private Function BuildParmITEM(ByVal derl As DeRemittanceLines, ByVal derh As DeRemittanceHeader) As String
        '------------------------------------------------------------------------------
        ' BuildParmpayment - Build IN parameter for Write Order  stored procedure by 
        ' formatting strings to correct length then concatenating
        '------------------------------------------------------------------------------
        '        Dim _parmLineNo As String = derl.LineNumber.ToString.PadLeft(6, "0")
        Dim parmSOPorderNo As String = Utilities.FixStringLength(derh.SOPorderNo, 15)
        Dim _parmLineNo As String = derl.LineNumber.ToString.PadLeft(3, "0")
        Dim parmLineNumber As String = Utilities.FixStringLength(_parmLineNo, 3)
        Dim parmMasterItemType As String = Utilities.FixStringLength(derl.MasterItemType, 2)
        Dim parmLedgerEntryDocumentReference As String = Utilities.FixStringLength(derl.LedgerEntryDocumentReference, 8)
        Dim parmPostingAmountPrime As String = Utilities.FixStringLength(derl.PostingAmountPrime.ToString, 17)
        Dim parmDiscountAmountPrime As String = Utilities.FixStringLength(derl.DiscountAmountPrime.ToString, 17)
        Dim parmSuppliersReference As String = Utilities.FixStringLength(derl.SuppliersReference, 20)

        ' SEQN68	Sequence no.	            8 S 0	counter, starting 1
        ' ETYP68	Master item type	        2 A	    <MasterItemType>
        ' LREF68	Ledger entry document ref.	8 A	    <LedgerEntryDocumentReference>
        ' PTMT68	Posting amount (prime)	    15 P 2	<PostingAmountPrime>
        ' PDSC68	Discount amount (prime)	    15 P 2	<DiscountAmountPrime>
        ' SREF68(Supplier) 's ref.	            20 A	<SuppliersReference>

        Dim sb As New StringBuilder
        With sb
            .Append(parmSOPorderNo)
            .Append(parmLineNumber)
            .Append(parmMasterItemType)
            .Append(parmLedgerEntryDocumentReference)
            .Append(parmPostingAmountPrime)
            .Append(parmDiscountAmountPrime)
            .Append(parmSuppliersReference)
        End With

        Return sb.ToString

    End Function

    Private Property ParmHEAD(ByVal order As Integer) As String
        Get
            Return _parmHEAD(order)
        End Get
        Set(ByVal value As String)
            _parmHEAD(order) = value
        End Set
    End Property
    Private Property ParmITEM(ByVal order As Integer, ByVal index As Integer) As String
        Get
            Return _parmITEM(order, index)
        End Get
        Set(ByVal value As String)
            _parmITEM(order, index) = value
        End Set
    End Property

End Class
