Imports System
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    TransactionResponse.vb
'
'       Date                        Apr 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      COMMTRS- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       03/04/06    /000    Ben     Created. Imported from Orion  
'
'--------------------------------------------------------------------------------------------------
Namespace CardProcessing.Commidea
    Public Class TransactionResponse

        Private _AuthCode As String = String.Empty
        Private _AuthResult As String = String.Empty
        Private _AuthMessage As String = String.Empty
        Private _CvcResult As String = String.Empty
        Private _EFTSN As String = String.Empty
        Private _ResultID As String = String.Empty
        Private _MerchantData As String = String.Empty
        Private _TransactionID As String = String.Empty

        Public Sub New(ByVal resultid As String, _
        ByVal transactionid As String, _
            ByVal EFTSN As String, _
            ByVal authresult As String, _
            ByVal authcode As String, _
            ByVal authmessage As String, _
            ByVal merchantdata As String, _
            ByVal cvcresult As String)

            _ResultID = resultid
            _TransactionID = transactionid
            _EFTSN = EFTSN
            _AuthResult = authresult
            _AuthCode = authcode
            _AuthMessage = authmessage
            _MerchantData = merchantdata
            _CvcResult = cvcresult
        End Sub
        Public ReadOnly Property AuthCode() As String
            Get
                Return _AuthCode
            End Get
        End Property
        Public ReadOnly Property AuthMessage() As String
            Get
                Return _AuthMessage
            End Get
        End Property
        Public ReadOnly Property AuthResult() As String
            Get
                Return _AuthResult
            End Get
        End Property
        Public ReadOnly Property CvcResult() As String
            Get
                Return _CvcResult
            End Get
        End Property
        Public ReadOnly Property EFTSN() As String
            Get
                Return _EFTSN
            End Get
        End Property
        Public ReadOnly Property MerchantData() As String
            Get
                Return _MerchantData
            End Get
        End Property
        Public ReadOnly Property ResultID() As String
            Get
                Return _ResultID
            End Get
        End Property
        Public ReadOnly Property TransactionID() As String
            Get
                Return _TransactionID
            End Get
        End Property

    End Class
End Namespace
