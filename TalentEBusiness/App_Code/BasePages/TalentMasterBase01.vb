Imports System.Data
Imports System.IO
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.CustomControls
Imports TEBUtilities = Talent.eCommerce.Utilities
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Hospitality
Imports System.Linq
Imports TalentBusinessLogic.DataTransferObjects.Hospitality
Imports System.Collections.Generic

Public Class TalentMasterBase01
    Inherits System.Web.UI.MasterPage
    Dim ModuleDefaults As Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage
    Private _TDataObjects As Talent.Common.TalentDataObjects

    Private Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        If (Me.Page.Master.Master IsNot Nothing AndAlso Me.ToString() = Me.Page.Master.Master.ToString()) OrElse (Me.ToString().Contains("modal")) OrElse (Me.ToString().Contains("blank")) Then
            ModuleDefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
            Dim headScript As New LiteralControl
            Dim serverName As New LiteralControl
            headScript.Text = GetPageExtraDataForHead()
            Try
                serverName.Text = "<!-- " & Request.ServerVariables("LOCAL_ADDR") & " -->"
            Catch
            End Try
            Me.Page.Header.Controls.AddAt(1, serverName)
            Me.Page.Header.Controls.Add(headScript)
            If Not Page.IsPostBack Then
                GetTrackingHeaderCodesText()
            End If
            SetBodyCSSClassAttribute()
            If Me.Page.Master.Master IsNot Nothing AndAlso Me.Page.Master.Master.ToString().Contains("site") Then
                Dim hdfSessionID As HiddenField = CType(Me.Page.Master.Master.FindControl("hdfSessionID"), HiddenField)
                If hdfSessionID IsNot Nothing Then hdfSessionID.Value = Session.SessionID
            End If
        End If
    End Sub




    ''' <summary>
    ''' Get the tracking codes for the page header
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub GetTrackingHeaderCodesText()
        Dim myStr As String = TrackingCodes.DoInsertTrackingCodes("HEADER")
        If myStr.Length > 0 Then
            Dim lit As New LiteralControl
            lit.Text = myStr
            Me.Page.Header.Controls.AddAt(0, lit)
        End If
    End Sub

    ''' <summary>
    ''' Set the CSS class names for the body tag
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub SetBodyCSSClassAttribute()
        If _TDataObjects Is Nothing Then
            _TDataObjects = New Talent.Common.TalentDataObjects()
            Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.DestinationDatabase = "SQL2005"
            _TDataObjects.Settings = settings
        End If
        Dim masterBodyTag As HtmlGenericControl = Nothing
        Dim dtPage As DataTable = _TDataObjects.PageSettings.TblPage.GetByPageCode(Talent.eCommerce.Utilities.GetCurrentPageName(), TalentCache.GetBusinessUnit())
        Dim cssClassName As String = String.Empty
        If Me.Page.Master.Master IsNot Nothing Then
            masterBodyTag = CType(Me.Page.Master.Master.FindControl("MasterBodyTag"), HtmlGenericControl)
            If masterBodyTag IsNot Nothing Then
                If dtPage IsNot Nothing AndAlso dtPage.Rows.Count > 0 Then
                    cssClassName = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPage.Rows(0)("BODY_CSS_CLASS"))
                    If Talent.eCommerce.Utilities.GetCurrentPageName.ToUpper().Contains("HOSPITALITYPACKAGEDETAILS.ASPX") Then
                        Dim PackageCode As String = retrievePackageCode()
                        cssClassName = cssClassName & " packageCode-" & PackageCode
                    End If
                End If
                masterBodyTag.Attributes.Add("class", cssClassName)
            End If
        Else
            If Me.ToString().Contains("modal") Then
                masterBodyTag = CType(Me.Page.Master.FindControl("MasterBodyTag"), HtmlGenericControl)
                If dtPage IsNot Nothing AndAlso dtPage.Rows.Count > 0 Then
                    cssClassName = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPage.Rows(0)("BODY_CSS_CLASS"))
                End If
                masterBodyTag.Attributes.Add("class", cssClassName & " modal")
            ElseIf Me.ToString().Contains("blank") Then
                masterBodyTag = CType(Me.Page.Master.FindControl("MasterBodyTag"), HtmlGenericControl)
                If dtPage IsNot Nothing AndAlso dtPage.Rows.Count > 0 Then
                    cssClassName = Talent.eCommerce.Utilities.CheckForDBNull_String(dtPage.Rows(0)("BODY_CSS_CLASS"))
                End If
                masterBodyTag.Attributes.Add("class", cssClassName)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Get the software Version 
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetVersion() As String
        Return Talent.eCommerce.Utilities.getVersionQueryString
    End Function

    ''' <summary>
    ''' Get the tracking codes for the page body
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetTrackingBodyCodesText() As String
        If Not ModuleDefaults Is Nothing AndAlso ModuleDefaults.TrackingCodeInUse Then
            Return TrackingCodes.DoInsertTrackingCodes("BODY")
        Else
            Return ""
        End If
    End Function

    ''' <summary>
    ''' Get the page extra data records for the page body
    ''' </summary>
    ''' <returns>A string value of the page extra data records</returns>
    ''' <remarks></remarks>
    Public Function GetPageExtraDataForBody() As String
        Dim dtPageExtraData As DataTable = Nothing
        If ModuleDefaults Is Nothing Then
            ModuleDefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        End If
        If Not ModuleDefaults Is Nothing AndAlso ModuleDefaults.UsePageExtraDataTable Then
            If _TDataObjects Is Nothing Then
                _TDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                _TDataObjects.Settings = settings
            End If

            dtPageExtraData = _TDataObjects.PageSettings.TblPageExtraData.GetPageData(
                   TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile),
                   Talent.eCommerce.Utilities.GetCurrentPageName, _languageCode, "BODY", Talent.eCommerce.Utilities.GetCurrentPageName(True), True)
            If dtPageExtraData.Rows.Count > 0 Then
                Dim dataStringBuilder As New StringBuilder
                For Each row As DataRow In dtPageExtraData.Rows
                    If row.Table.Columns.Contains("DATA") Then
                        dataStringBuilder.Append(Talent.eCommerce.Utilities.CheckForDBNull_String(row("DATA")) & vbCrLf)
                    Else
                        For Each column As DataColumn In dtPageExtraData.Columns
                            LogWriter.WriteToLog("D:\Temp\PageExtraData\", column.ColumnName)
                        Next
                    End If
                Next
                Return dataStringBuilder.ToString
            Else
                Return String.Empty
            End If
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' Get the page extra data records for the page header
    ''' </summary>
    ''' <returns>A string value of the page extra data records</returns>
    ''' <remarks></remarks>
    Public Function GetPageExtraDataForHead() As String
        If Talent.eCommerce.Utilities.GetCurrentPageName.ToLower = "checkoutorderconfirmation.aspx" And ModuleDefaults.DiscountIF = False Then
            Return String.Empty
        End If
        If Not ModuleDefaults Is Nothing AndAlso ModuleDefaults.UsePageExtraDataTable Then
            If _TDataObjects Is Nothing Then
                _TDataObjects = New Talent.Common.TalentDataObjects()
                Dim settings As Talent.Common.DESettings = New Talent.Common.DESettings()
                settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
                settings.DestinationDatabase = "SQL2005"
                _TDataObjects.Settings = settings
            End If

            Dim dtPageExtraData As DataTable = _TDataObjects.PageSettings.TblPageExtraData.GetPageData(
                TalentCache.GetBusinessUnit, TalentCache.GetPartner(HttpContext.Current.Profile),
                Talent.eCommerce.Utilities.GetCurrentPageName, _languageCode, "HEAD", Talent.eCommerce.Utilities.GetCurrentPageName(True), True)
            If dtPageExtraData.Rows.Count > 0 Then
                Dim dataStringBuilder As New StringBuilder
                For Each row As DataRow In dtPageExtraData.Rows
                    If row.Table.Columns.Contains("DATA") Then
                        dataStringBuilder.Append(Talent.eCommerce.Utilities.CheckForDBNull_String(row("DATA")) & vbCrLf)
                    Else
                        For Each column As DataColumn In dtPageExtraData.Columns
                            LogWriter.WriteToLog("D:\Temp\PageExtraData\", column.ColumnName)
                        Next
                    End If
                Next
                Return dataStringBuilder.ToString
            Else
                Return String.Empty
            End If
        Else
            Return String.Empty
        End If
    End Function

    ''' <summary>
    ''' Return the content of a given HTML include file name in the reserved HTML includes folder
    ''' </summary>
    ''' <param name="fileName">The given filename</param>
    ''' <returns>The file content</returns>
    ''' <remarks></remarks>
    Public Function GetStaticHTMLInclude(ByVal fileName As String) As String
        Dim includeFolderName As String = "Includes\"
        Dim fileContent As String = String.Empty
        Dim filePath As String = String.Empty
        If ModuleDefaults Is Nothing Then
            ModuleDefaults = (New Talent.eCommerce.ECommerceModuleDefaults).GetDefaults
        End If
        If ModuleDefaults IsNot Nothing AndAlso ModuleDefaults.HtmlPathAbsolute IsNot Nothing Then
            If Not ModuleDefaults.HtmlPathAbsolute.EndsWith("\") Then includeFolderName = "\" & includeFolderName
            filePath = ModuleDefaults.HtmlPathAbsolute & includeFolderName & fileName
            Try
                If File.Exists(filePath) Then
                    Dim line As String = String.Empty
                    Dim fStream As New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                    Using fStream
                        Dim fReader As New StreamReader(fStream)
                        Using fReader
                            Do While fReader.Peek >= 0
                                line = line & fReader.ReadLine() & vbNewLine
                            Loop
                        End Using
                    End Using
                    fileContent = line
                End If
            Catch ex As Exception
                fileContent = String.Empty
            End Try
        End If
        Return fileContent
    End Function

    ''' <summary>
    ''' Set the HTML include file on the custom literal control.
    ''' </summary>
    ''' <param name="sender">The custom literal control</param>
    ''' <param name="e">The event arguments</param>
    ''' <remarks></remarks>
    Public Sub GetStaticHTMLInclude(ByVal sender As Object, ByVal e As EventArgs)
        Dim litHTML As Talent.CustomControls.Literal = CType(sender, Talent.CustomControls.Literal)
        Dim fileContent As String = GetStaticHTMLInclude(litHTML.HTMLFileName.ToString)
        litHTML.Text = fileContent
    End Sub

#Region "Private Function"

    ''' <summary>
    ''' Get PackageCode from the packageid passed through query string
    ''' </summary>
    ''' <remarks></remarks>
    Private Function retrievePackageCode() As String
        Dim inputModel As New HospitalityPackageListInputModel
        Dim builder As New HospitalityListBuilder
        Dim viewModelPackage As New HospitalityPackageListViewModel
        Dim packageListhome = New List(Of PackageDetails)
        Dim packageCode As String = String.Empty
        Dim packageID As String = Request.QueryString("packageid")
        If Not String.IsNullOrEmpty(packageID) Then
            viewModelPackage = builder.GetHospitalityPackageList(inputModel)
            packageListhome = viewModelPackage.PackageListDetailsHome
            
            if packageListhome.Any(Function(item) item.PackageID = packageID) Then
                packageCode = packageListhome.Where(Function(item) item.PackageID = packageID).Select(Function(item) item.PackageCode).First()
            End If    
        End If
        Return packageCode
    End Function
#End Region

End Class
