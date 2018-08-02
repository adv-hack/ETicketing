Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    User Controls - Banners
'
'       Date                        01/11/07
'
'       Author                       
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
Partial Class UserControls_Banners
    Inherits ControlBase
    Private _sequence As String
    Private _usage As String


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Dim moduleDefaults As New ECommerceModuleDefaults
        Dim def As ECommerceModuleDefaults.DefaultValues = moduleDefaults.GetDefaults()

        If def.enableBannerControl Then
            '-----------
            ' Set banner
            '-----------
            Dim group1 As String = String.Empty
            If Request.QueryString("group1") Is Nothing Then
                Response.Redirect("~/default.aspx")
            Else
                group1 = Request.QueryString("group1")
            End If
            Dim groups As New TalentGroupInformationTableAdapters.tbl_group_level_01TableAdapter
            Dim dt As Data.DataTable = groups.GetDataByBU_Partner_Group(TalentCache.GetBusinessUnit, TalentCache.GetPartner(Profile), group1)
            
            If dt.Rows.Count > 0 Then
                Ban1.Location = dt.Rows(0)("GROUP_L01_DESCRIPTION_1").ToString.Trim
            Else
                '------------
                ' try for all partners
                '------------
                dt = groups.GetDataByBU_Partner_Group(TalentCache.GetBusinessUnit, "*ALL", group1)
                If dt.Rows.Count > 0 Then
                    Ban1.Location = dt.Rows(0)("GROUP_L01_DESCRIPTION_1").ToString.Trim
                Else
                    '------------
                    ' try for all business units
                    '------------
                    dt = groups.GetDataByBU_Partner_Group(Talent.Common.Utilities.GetAllString, TalentCache.GetPartner(Profile), group1)
                    If dt.Rows.Count > 0 Then
                        Ban1.Location = dt.Rows(0)("GROUP_L01_DESCRIPTION_1").ToString.Trim
                    Else
                        '------------
                        ' try for all business and partners
                        '------------
                        dt = groups.GetDataByBU_Partner_Group(Talent.Common.Utilities.GetAllString, Talent.Common.Utilities.GetAllString, group1)
                    End If
   
                End If
                If dt.Rows.Count > 0 Then
                    Ban1.Location = dt.Rows(0)("GROUP_L01_DESCRIPTION_1").ToString.Trim
                End If
            End If
        Else
            Ban1.Enabled = False
            Me.Controls.Remove(Ban1)
        End If



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
