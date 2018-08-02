Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.Xml.Schema
Imports System.IO
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This is a Base class 
'
'       Date                        Nov 2006
'
'       Author                       
'
'       © CS Group 2006             All rights reserved.
'  
'       Error Number Code base      TTPCMSB- 
'                                   
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class Cms

        Private _company As String = String.Empty
        Private _loginId As String = String.Empty
        Private _senderID As String = String.Empty
        Private _webServiceName As String = String.Empty
        Private _writeLog As Boolean = False

        Public Property Company() As String
            Get
                Return _company
            End Get
            Set(ByVal value As String)
                _company = value
            End Set
        End Property
        Public Property LoginId() As String
            Get
                Return _loginId
            End Get
            Set(ByVal value As String)
                _loginId = value
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
        Public Property WebServiceName() As String
            Get
                Return _webServiceName
            End Get
            Set(ByVal value As String)
                _webServiceName = value
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

    End Class

End Namespace