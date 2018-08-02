Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with XML Load Requests
'
'       Date                        Mar 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAPR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentXmlLoad

    Private _settings As New DESettings
    Private _resultDataSet As DataSet
    Private _businessUnit As String = String.Empty
    Private _partnerCode As String = String.Empty
    Private _xml As Collection
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
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
    Public Property Xml() As Collection
        Get
            Return _xml
        End Get
        Set(ByVal value As Collection)
            _xml = value
        End Set
    End Property
    Public Property Settings() As DESettings
        Get
            Return _settings
        End Get
        Set(ByVal value As DESettings)
            _settings = value
        End Set
    End Property
    Public Property ResultDataSet() As DataSet
        Get
            Return _resultDataSet
        End Get
        Set(ByVal value As DataSet)
            _resultDataSet = value
        End Set
    End Property
    Public Function LoadData() As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Dim dbXmlLoad As New DBXmlLoad
        With dbXmlLoad
            .BusinessUnit = BusinessUnit
            .PartnerCode = PartnerCode
            .Xml = Xml
            .Settings = Settings
            err = .ValidateAgainstDatabase()
            If Not err.HasError Then
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    ResultDataSet = .ResultDataSet
                End If
            End If
        End With
        Return err
    End Function

End Class

