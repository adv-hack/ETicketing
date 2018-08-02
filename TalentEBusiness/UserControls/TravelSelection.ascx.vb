Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports Talent.Common
Imports Talent.eCommerce
Imports Talent.eCommerce.Utilities
Imports System.Xml
Imports System.Globalization
Imports DataSetHelperTravel
'Talent.Test added for testing
'Imports Talent.Test

'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Travel Selection
'
'       Date                        6 Aug '07
'
'       Author                      Swapnil
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Partial Class UserControls_TravelSelection
    Inherits ControlBase

    'changed for Testing
    Dim product As New Talent.Common.TalentProduct
    Dim settings As New Talent.Common.DESettings
    Dim err As New Talent.Common.ErrorObj
    'Dim product As New Talent.Test.TalentProduct
    'Dim settings As New Talent.Test.DESettings
    'Dim err As New Talent.Test.ErrorObj
    Public ucr As New Talent.Common.UserControlResource

    Dim depd As New Talent.Common.DEProductDetails
    Private _languageCode As String = Talent.Common.Utilities.GetDefaultLanguage

#Region " Public Propertys"
    Public _ValueID As String
    Public _ProductCode As String
    Public _ProductType As String

    Const _cProductTable = 1
    Private _Enabled As Boolean

    Public Property Enabled() As Boolean
        Get
            Return _Enabled
        End Get
        Set(ByVal value As Boolean)
            _Enabled = value
        End Set
    End Property
    Public Property ProductCode() As String
        Get
            Return _ProductCode
        End Get
        Set(ByVal value As String)
            _ProductCode = value
        End Set
    End Property
    Public Property ValueID() As String
        Get
            Return _ValueID
        End Get
        Set(ByVal value As String)
            _ValueID = value
        End Set
    End Property
    Public Property ProductType() As String
        Get
            Return _ProductType
        End Get
        Set(ByVal value As String)
            _ProductType = value
        End Set
    End Property

#End Region

    Public Sub setTravelDetails()
        Dim dsHelper As DataSetHelperTravel
        Dim ds1 As New DataSet()
        Dim ds As New DataSet()
        Try



            qtyLabel.Text = ucr.Content("qtyLabel", _languageCode, True)
            buyButton.Text = ucr.Content("buyButton", _languageCode, True)

        Catch ex As Exception
        End Try

        Try
            settings.FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            settings.BusinessUnit = TalentCache.GetBusinessUnit()
            depd.ProductCode = _ProductCode
            product.Settings() = settings

            'DEProductDetails' is not a member of 'Talent.Test.TalentProduct'
            'product.DEProductDetails() = depd
            err = product.AvailableTravel()
            ds = product.ResultDataSet
            'ds.Tables(_cProductTable).DefaultView.RowFilter = "ProductType='" + _ProductType + "'"
            ViewState("dsTest") = ds


            'If Not err.HasError Then
            If ds.Tables(_cProductTable).Rows.Count <> 0 Then
                TransportDropDown.Visible = True

                dsHelper = New DataSetHelperTravel(ds1)
                With TransportDropDown
                    .ToolTip = _ValueID
                    .DataSource = ds.Tables(_cProductTable).DefaultView 'ds.Tables(_cProductTable) 'dsHelper.SelectDistinct(ds.Tables(_cProductTable).TableName, ds.Tables(_cProductTable), "TravelDescription", "TravelDescription") ', "StandCode", "StandDescription")
                    .DataTextField = "TravelDescription" '"StandDescription"
                    .DataValueField = "TravelDescription" '"StandCode"
                    .DataBind()
                End With
            End If
            'Else
            '    If ds.Tables(0).Rows(0).Item("ErrorOccurred").ToString = "E" Then
            '        showError(ds.Tables(0).Rows(0).Item("ReturnCode").ToString())
            '    End If
            'End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub showError(ByVal errCode As String)
        Dim eerlbl As New Label
        eerlbl = CType(Page.FindControl("errorLabel"), Label)
        If errCode = "NS" Then
            eerlbl.Text = getErrText(errCode)
        Else
            eerlbl.Text = getErrText("XX")
        End If
    End Sub
    Protected Function getErrText(ByVal pCode As String) As String
        Dim wfrPage As New WebFormResource

        With wfrPage
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = Talent.eCommerce.Utilities.GetCurrentPageName
            .PageCode = Talent.eCommerce.Utilities.GetCurrentPageName
        End With
        Dim s As String
        s = wfrPage.Content(pCode, _languageCode, True)
        Return s

    End Function
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        With ucr
            .BusinessUnit = TalentCache.GetBusinessUnit()
            .PageCode = String.Empty
            .PartnerCode = TalentCache.GetPartner(HttpContext.Current.Profile)
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .KeyCode = "TravelSelection.ascx"
        End With
    End Sub

    Protected Sub buyButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles buyButton.Click
        Response.Redirect("~/Redirect/TicketingGateway.aspx?page=ProductTravel.aspx&function=AddToBasket&product=ProductCode&coach=CoachCode&quantity=Quantity")
    End Sub

    Public def As ECommerceModuleDefaults.DefaultValues

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim moduleDefaults As New ECommerceModuleDefaults
        def = moduleDefaults.GetDefaults()
        ' Should only be used for memberships
        'qtyTextBox.Text = def.DefaultMembershipQuantity
    End Sub
End Class

Public Class DataSetHelperTravel
    Public ds As DataSet
    Public Sub New(ByRef DataSet As DataSet)
        ds = DataSet
    End Sub
    Public Sub New()
        ds = Nothing
    End Sub
    Private Function ColumnEqual(ByVal A As Object, ByVal B As Object) As Boolean

        ' Compares two values to see if they are equal. Also compares DBNULL.Value.
        ' Note: If your DataTable contains object fields, then you must extend this
        ' function to handle them in a meaningful way if you intend to group on them.

        If A.Equals(DBNull.Value) AndAlso B.Equals(DBNull.Value) Then
            Return True
            '  both are DBNull.Value
        End If
        If A.Equals(DBNull.Value) OrElse B.Equals(DBNull.Value) Then
            Return False
            '  only one is DBNull.Value
        End If
        Return (A.Equals(B))
        ' value type standard comparison
    End Function
    Public Function SelectDistinct(ByVal TableName As String, ByVal SourceTable As DataTable, ByVal FieldName As String, ByVal FieldName2 As String) As DataTable
        Dim dt As New DataTable(TableName)
        dt.Columns.Add(FieldName, SourceTable.Columns(FieldName).DataType)
        dt.Columns.Add(FieldName2, SourceTable.Columns(FieldName2).DataType)
        Dim LastValue As Object = Nothing
        For Each dr As DataRow In SourceTable.[Select]("", FieldName)
            If LastValue Is Nothing OrElse Not (ColumnEqual(LastValue, dr(FieldName))) Then
                LastValue = dr(FieldName)
                dt.Rows.Add(LastValue, dr(FieldName2))
            End If
        Next
        If ds IsNot Nothing Then
            ds.Tables.Add(dt)
        End If
        Return dt
    End Function
End Class
