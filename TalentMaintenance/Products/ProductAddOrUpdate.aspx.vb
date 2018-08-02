Imports System.Data
Imports System.Collections.Generic
Imports Talent.Common
Partial Class Products_ProductAddOrUpdate
    Inherits System.Web.UI.Page

    Private _wfrPage As New Talent.Common.WebFormResource
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage()

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With _wfrPage
            .BusinessUnit = GlobalConstants.MAINTENANCEBUSINESSUNIT
            .PartnerCode = GlobalConstants.STARALLPARTNER
            .PageCode = "ProductAddOrUpdate.aspx"
            .KeyCode = "ProductAddOrUpdate.aspx"
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        End With
        Dim master As MasterPage1 = CType(Me.Master, MasterPage1)
        master.HeaderText = _wfrPage.Content("HeaderText", _languageCode, True)
    End Sub

#Region "Web Service Methods"
    Private Shared Function GetOptionDefinitions() As DataTable
        Dim dtOptionDefinitions As New DataTable
        Dim TalentDataObj = New Talent.Common.TalentDataObjects()
        Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
        settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
        settings.DestinationDatabase = "SQL2005"
        TalentDataObj.Settings = settings
        dtOptionDefinitions = TalentDataObj.ProductsSettings.TblProductOptionDefinition.GetAllOptionCode(False)
        TalentDataObj = Nothing
        Return dtOptionDefinitions
    End Function
    <System.Web.Script.Services.ScriptMethod(), _
System.Web.Services.WebMethod()> _
    Public Shared Function GetOptionCodeList(ByVal prefixText As String, ByVal count As Integer) As List(Of String)
        Dim optionCodeList As List(Of String) = New List(Of String)
        Dim dtOptionDefinitions As DataTable = Nothing
        If HttpContext.Current.Session("DtOptionDefinitions") IsNot Nothing Then
            dtOptionDefinitions = CType(HttpContext.Current.Session("DtOptionDefinitions"), DataTable)
        Else
            dtOptionDefinitions = GetOptionDefinitions()
            HttpContext.Current.Session("DtOptionDefinitions") = dtOptionDefinitions
        End If

        If dtOptionDefinitions IsNot Nothing _
            AndAlso dtOptionDefinitions.Rows.Count > 0 Then
            Dim matchedRows() As DataRow = Nothing
            matchedRows = dtOptionDefinitions.Select("OPTION_CODE LIKE '" & prefixText & "%'")
            If matchedRows.Length > 0 Then
                For rowIndex As Integer = 0 To matchedRows.Length - 1
                    optionCodeList.Add("""" & matchedRows(rowIndex)("OPTION_CODE").ToString() & """")
                    If rowIndex = (count - 1) Then Exit For
                Next
            End If
        End If
        Return optionCodeList
    End Function
#End Region

End Class
