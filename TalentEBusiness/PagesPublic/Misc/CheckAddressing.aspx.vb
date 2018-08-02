Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports Talent.eCommerce
Imports com.qas.proweb
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    CheckAddressing.aspx. Check if QAS is working
'
'       Date                        17/12/08
'
'       Author                      Ben Ford
'
'       @ CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_Misc_CheckAddressing
    Inherits Base01

    Private _failureReason As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim success As Boolean = True
        _failureReason = String.Empty
        Dim myDefaults As New ECommerceModuleDefaults
        Try

            Dim searchService As QuickAddress = NewQuickAddress()
            searchService.Engine = QuickAddress.EngineTypes.SingleLine

            Dim sLayout As String = String.Empty
            Dim defaults As ECommerceModuleDefaults.DefaultValues
            Dim defs As New ECommerceModuleDefaults
            defaults = defs.GetDefaults
            sLayout = defaults.AddressingLayout.Trim

            'Is automatic address capture available for this country & layout
            Dim newSearch As com.qas.proweb.CanSearch
            newSearch = searchService.CanSearch("GBR", sLayout)
            If newSearch.IsOk Then
                success = True
            Else
                success = False
                _failureReason = newSearch.ErrorMessage
            End If

        Catch ex As Exception
            success = False
            _failureReason = ex.Message
        End Try

        '---------------------
        ' Write response label
        '---------------------
        If success = True Then
            lblResult.Text = "ADDRESSING CHECKED OK"
        Else
            lblResult.Text = "ADDRESSING CHECK FAILED - " & _failureReason
        End If

    End Sub
    Protected Function NewQuickAddress() As QuickAddress

        'Retrieve server URL from web.config
        Dim sServerURL As String = System.Configuration.ConfigurationManager.AppSettings(com.qas.prowebintegration.Constants.KEY_SERVER_URL)
        'Create QuickAddress search object
        Return New QuickAddress(sServerURL)

    End Function

End Class
