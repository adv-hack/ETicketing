Imports System
Imports System.Collections
Imports System.Text
Imports System.Xml
Imports Talent.Common.CardProcessing.Commidea.WebServices
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    CommideaClient.vb
'
'       Date                        Apr 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      COMMCCL- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       30/03/07    /000    Ben     Created. Imported from GDHA
'
'--------------------------------------------------------------------------------------------------
Namespace CardProcessing.Commidea

    Public Class CommideaClient

        Private _AVSorCSCRejected As Boolean = False
        Private _Completed As Boolean = False
        Private _confirmURL As String
        Private _ErrorText As String = String.Empty
        Private _ErrorNo As String = String.Empty
        Private _HasErrors As Boolean = False
        Private _logRoot As String
        Private _logText As String = String.Empty
        Private _transactionURL As String

        Public ReadOnly Property AvsOrCscRejected() As Boolean
            Get
                Return _AVSorCSCRejected
            End Get
        End Property
        Public ReadOnly Property Completed() As Boolean
            Get
                Return _Completed
            End Get
        End Property
        Public Property ConfirmURL() As String
            Get
                Return _confirmURL
            End Get
            Set(ByVal value As String)
                _confirmURL = value
            End Set
        End Property
        Public ReadOnly Property ErrorText() As String
            Get
                Return _ErrorText
            End Get
        End Property
        Public ReadOnly Property HasErrors() As Boolean
            Get
                Return _HasErrors
            End Get
        End Property
        Public ReadOnly Property ErrorNo() As String
            Get
                Return _ErrorNo
            End Get
        End Property
        Public Property LogText() As String
            Get
                Return _logText
            End Get
            Set(ByVal value As String)
                _logText = value
            End Set
        End Property
        Public Property TransactionURL() As String
            Get
                Return _transactionURL
            End Get
            Set(ByVal value As String)
                _transactionURL = value
            End Set
        End Property

        Function CheckAvsCsc(ByVal txnres As Commidea.WebServices.TxnResponse, ByVal tr As TransactionRequest) As Boolean
            '------------------------------------------------------------------
            ' CheckAvsCsc - Check rules set up for Avs/Csc rejection, and 
            ' check return values. 
            ' Result Codes:
            ' 0=Matched
            ' 1=Not Checked
            ' 2=Partial Match (AVS Only)
            ' 3=Not supported by acquirer
            ' 4=Not matched
            ' 5=Feature not enabled on server
            '------------------------------------------------------------------
            Dim rejectDueToCscAvs As Boolean = False
            Dim pcAvsResult As String = txnres.StdResponse.PcAvsResult.Trim
            Dim adAvsResult As String = txnres.StdResponse.Ad1AvsResult.Trim
            Dim cscResult As String = txnres.StdResponse.CvcResult.Trim
            '----------------
            ' Check CSC is OK
            '----------------
            If tr.rejectCsc AndAlso (cscResult = "1" Or cscResult = "4") Then
                rejectDueToCscAvs = True
            Else
                '----------------------
                ' Check Post Code is OK
                '----------------------
                If tr.rejectPcAvs AndAlso (pcAvsResult = "1" Or pcAvsResult = "4" Or (pcAvsResult = "2" AndAlso Not tr.allowPartialAvs)) Then
                    rejectDueToCscAvs = True
                Else
                    '-----------------------
                    ' Check if address is OK
                    '-----------------------
                    If tr.rejectAdAvs AndAlso (adAvsResult = "1" Or adAvsResult = "4" Or (adAvsResult = "2" AndAlso Not tr.allowPartialAvs)) Then
                        rejectDueToCscAvs = True
                    End If
                End If
            End If
            Return rejectDueToCscAvs
        End Function
        Function ConfirmationXMLString(ByVal txnres As TxnResponse, ByVal command As Integer) As String
            Dim confdocbuilder As New StringBuilder
            With confdocbuilder
                'opening statements.
                .Append("<?xml version=""1.0"" encoding=""utf-8""?>")
                .Append("<CommideaConfirmation xmlns=""https://www.commidea.webservices.com"">")

                'details.
                .Append("<ResultID>")
                .Append(txnres.StdResponse.ResultID)
                .Append("</ResultID>")
                .Append("<TRecordID>")
                .Append(txnres.StdResponse.TransactionID)
                .Append("</TRecordID>")
                .Append("<Command>")
                .Append(command.ToString())
                .Append("</Command>")
                .Append("<AuthCode>")
                .Append(txnres.StdResponse.AuthCode)
                .Append("</AuthCode>")
                .Append("<EFTSN>")
                .Append(txnres.StdResponse.EFTSN)
                .Append("</EFTSN>")

                'closing statements.
                .Append("</CommideaConfirmation>")
            End With
            Return confdocbuilder.ToString
        End Function
        Public Function DoTransactionRequest(ByVal tr As TransactionRequest) As StandardResponse
            Dim last4CardDigits As String = tr.Pan.Substring(tr.Pan.Length - 4)

            ' Process the transaction request.
            ' If the request produces an AuthResult of 6 then process the confirmation.
            ' Otherwise flag any errors and return messages as appropriate.
            Dim cctr As New CCTransaction(TransactionURL)
            Dim txnres As Talent.Common.CardProcessing.Commidea.WebServices.TxnResponse = Nothing
            Dim conftxn As New ConfirmTxn(ConfirmURL)
            Dim rejectDueToCSCAVS As Boolean = False

            Try

                LogText = "Card ending: " & last4CardDigits & ". Building XML doc and sending request."
                Try
                    txnres = cctr.CardTxn(TRequestToXMLString(tr))
                Catch ex As Exception
                    _HasErrors = True
                    _ErrorNo = "COMMCCL-010"
                    _ErrorText = ex.Message
                    LogText = "Card ending: " & last4CardDigits & _
                                   ". Exception:" & ex.Message
                    Throw New Exception(LogText)
                End Try
                '----------------------------------
                ' If no Auth Result then log errors
                '----------------------------------
                If txnres.StdResponse.AuthResult = Nothing Then
                    If Not txnres.InternalError Is Nothing Then
                        LogText = "Card ending: " & last4CardDigits & ". " & txnres.InternalError.ToString
                    Else
                        LogText = "Card ending: " & last4CardDigits & ". " & txnres.StdResponse.ErrorMsg
                    End If
                Else
                    '------------------------------------------------
                    ' Check whether need to reject for AVS/CSC or not
                    '------------------------------------------------
                    rejectDueToCSCAVS = CheckAvsCsc(txnres, tr)
                    If txnres.StdResponse.AuthResult = 6 Then
                        '------------------
                        ' Check AVS results
                        '------------------
                        rejectDueToCSCAVS = CheckAvsCsc(txnres, tr)

                        If rejectDueToCSCAVS Then
                            '-----------------------------
                            ' Failed Avs/Csc - send reject
                            '-----------------------------
                            LogText = "Card ending: " & last4CardDigits & _
                                    ". Failed CSC/AVS Check, sending reject. Csc,AvsAd,AvsPc:" & txnres.StdResponse.CvcResult & _
                                           txnres.StdResponse.Ad1AvsResult & txnres.StdResponse.PcAvsResult & _
                                           " - CSC: " & tr.CSC & ", ADD1: " & tr.AddressLine1 & ", PC: " & tr.PostCode
                            _AVSorCSCRejected = True
                            conftxn.ConfirmTransaction(ConfirmationXMLString(txnres, 2))
                        Else
                            '--------------------------------------
                            ' Successfull, process the confirmation
                            '--------------------------------------
                            LogText = "Card ending: " & last4CardDigits & _
                                      ". Success, sending confirm. Auth Code:" & txnres.StdResponse.AuthCode
                            txnres = conftxn.ConfirmTransaction(ConfirmationXMLString(txnres, 1))
                            If txnres.StdResponse.AuthResult = 6 Then
                                _Completed = True
                                LogText = "Card ending: " & last4CardDigits & _
                                                          ". Transaction completed successfully. Auth code: " & _
                                                             txnres.StdResponse.AuthCode & _
                                                            " - CSC: " & tr.CSC & ", ADD1: " & tr.AddressLine1 & ", PC: " & tr.PostCode
                            End If
                        End If
                    Else
                        If txnres.StdResponse.AuthResult = 2 Then
                            '--------------------------------------------
                            ' Rejected by bank - Send a rejection message
                            '--------------------------------------------
                            LogText = "Card ending: " & last4CardDigits & _
                                        ". Rejected by bank. Sending rejection. Auth Message: " & txnres.StdResponse.AuthMessage
                            conftxn.ConfirmTransaction(ConfirmationXMLString(txnres, 2))
                        Else
                            If txnres.StdResponse.AuthResult = 5 Then
                                '---------
                                ' Declined
                                '---------
                                LogText = "Card ending: " & last4CardDigits & _
                                                       ". Declined by bank."
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                _HasErrors = True
                _ErrorText = ex.Message
                If _ErrorNo = String.Empty Then
                    _ErrorNo = "COMMCCL-020"
                End If
                LogText = ex.Message
                Throw New Exception(LogText)
            End Try

            Return txnres.StdResponse

        End Function
        Function GetHouseNumber(ByVal addressLine1 As String) As String
            '------------------------------------------------------------------
            ' GetHouseNumber - This retrieves the first portion of the address
            ' required for AVS, which can either be the house number or name
            '------------------------------------------------------------------
            Dim houseNumber As String = String.Empty
            Dim firstSpace As Integer = addressLine1.IndexOf(" ")
            '-------------------------------------
            ' To long - just return first 30 chars
            '-------------------------------------
            If firstSpace > 30 Or firstSpace < 0 Then
                '-------------------------------------
                ' To long - just return first 30 chars
                '-------------------------------------
                houseNumber = addressLine1.Substring(0, addressLine1.Length)
            Else
                '--------------------------------
                ' Just return first section
                '--------------------------------
                houseNumber = addressLine1.Substring(0, firstSpace)
            End If

            Return houseNumber
        End Function
        Private Function TRequestToXMLString(ByRef tr As TransactionRequest) As String
            Dim docbuilder As New StringBuilder
            Dim returnString As String = String.Empty

            Dim houseNumber As String = GetHouseNumber(tr.AddressLine1)

            With docbuilder
                'open statements
                .Append("<?xml version=""1.0"" encoding=""utf-8"" ?>")
                .Append("<CommideaTransaction xmlns=""https://www.commidea.webservices.com"">")

                ' MerchantData
                .Append("<Merchant>")
                .Append("<GUID>")
                .Append(tr.GUID)
                .Append("</GUID>")
                .Append("<MerchantData>")
                .Append(tr.MerchantData)
                .Append("</MerchantData>")
                .Append("</Merchant>")

                ' TRecord
                .Append("<TRecord>")
                .Append("<AccountID>")
                .Append(tr.AccountID)
                .Append("</AccountID>")
                .Append("<AccountNumber>")
                .Append(tr.AccountNumber)
                .Append("</AccountNumber>")
                .Append("<TxnType>")
                .Append(tr.TxnType)
                .Append("</TxnType>")
                .Append("<CNP>")
                If tr.CNP Then
                    .Append("Y")
                Else
                    .Append("N")
                End If
                .Append("</CNP>")
                .Append("<ECom>")
                If tr.ECom Then
                    .Append("Y")
                Else
                    .Append("N")
                End If
                .Append("</ECom>")
                .Append("<Pan>")
                .Append(tr.Pan)
                .Append("</Pan>")
                .Append("<CSC>")
                .Append(tr.CSC)
                .Append("</CSC>")
                .Append("<AVS><![CDATA[")
                .Append(houseNumber.Trim)
                .Append(";")
                .Append(tr.PostCode.Trim)
                .Append("]]>")
                .Append("</AVS>")
                .Append("<ExpiryDate>")
                .Append(tr.ExpiryDate)
                .Append("</ExpiryDate>")
                .Append("<Issue>")
                .Append(tr.Issue)
                .Append("</Issue>")
                .Append("<StartDate>")
                .Append(tr.StartDate)
                .Append("</StartDate>")
                .Append("<TxnValue>")
                .Append(tr.TxnValue)
                .Append("</TxnValue>")
                .Append("<Reference>")
                .Append(tr.Reference)
                .Append("</Reference>")
                .Append("</TRecord>")

                'closing statements.
                .Append("</CommideaTransaction>")
            End With
            returnString = docbuilder.ToString
            Return returnString
        End Function

    End Class
End Namespace
