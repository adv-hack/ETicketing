'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - HTML Include
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      UCHTML- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_HTMLInclude
    Inherits ControlBase
    Private _sequence As String
    Private _usage As String

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Try
            If CType(Me.Page, TalentBase01).HTMLStrings.ContainsKey(Usage & Sequence) Then
                HtmlIncludeLabel.Text = CType(Me.Page, TalentBase01).HTMLStrings(Usage & Sequence)
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Property Sequence() As String
        Get
            Return _sequence
        End Get
        Set(ByVal value As String)
            _sequence = value
        End Set
    End Property
    Public Property Usage() As String
        Get
            Return _usage
        End Get
        Set(ByVal value As String)
            _usage = value
        End Set
    End Property

End Class
