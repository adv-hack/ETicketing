Imports System
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    ConfirmationRequest.vb
'
'       Date                        Apr 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      COMMCRQ- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       30/03/07    /000    Ben     Created. Imported from Orion De
'
'--------------------------------------------------------------------------------------------------
Namespace CardProcessing.Commidea
    Public Class ConfirmationRequest

        Private _AuthCode As String = String.Empty
        Private _Command As String = String.Empty
        Private _EFTSN As String = String.Empty
        Private _ResultID As String = String.Empty
        Private _TRecordID As String = String.Empty

        Public Property AuthCode() As String
            Get
                Return _AuthCode
            End Get
            Set(ByVal Value As String)
                _AuthCode = Value
            End Set
        End Property
        Public Property Command() As String
            Get
                Return _Command
            End Get
            Set(ByVal Value As String)
                _Command = Value
            End Set
        End Property
        Public Property EFTSN() As String
            Get
                Return _EFTSN
            End Get
            Set(ByVal Value As String)
                _EFTSN = Value
            End Set
        End Property
        Public Property ResultID() As String
            Get
                Return _ResultID
            End Get
            Set(ByVal Value As String)
                _ResultID = Value
            End Set
        End Property
        Public Property TRecordID() As String
            Get
                Return _TRecordID
            End Get
            Set(ByVal Value As String)
                _TRecordID = Value
            End Set
        End Property

    End Class
End Namespace
