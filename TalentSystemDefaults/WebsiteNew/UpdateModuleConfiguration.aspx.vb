Imports TalentSystemDefaults
Imports TalentSystemDefaults.DataEntities
Imports TalentSystemDefaults.DataAccess.ConfigObjects

Partial Class UpdateModuleConfiguration
    Inherits System.Web.UI.Page
    Const NEWLINE As String = "<br>"
    Const TAB As String = "&nbsp;&nbsp;&nbsp;&nbsp;"

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If (Not IsPostBack) Then
            ddlModules.DataSource = DMFactory.GetModuleNames()
            ddlModules.DataTextField = "Name"
            ddlModules.DataValueField = "Value"
            ddlModules.DataBind()
        Else
            If Request.Params.Get("__EVENTTARGET") = ddlModules.UniqueID Then
                Dim data As ConfigurationEntity() = DMFactory.GetDMConfigurations(GetDESettings(), ddlModules.SelectedValue)
                gridConfigData.DataSource = data
                Session(ddlModules.SelectedValue) = data
                gridConfigData.DataBind()
                plhModuleConfigurations.Visible = True
                dvContent.InnerHtml = String.Empty
            End If
        End If
    End Sub

    Private Function GetDESettings() As DESettings
        Dim settings As New DESettings
        With settings
            .BusinessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit")
            .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("TalentEBusinessDBConnectionString").ToString
            .DestinationDatabase = DestinationDatabase.SQL2005.ToString
        End With

        Return settings
    End Function

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        SaveData()
    End Sub

    Private Function GetValue(ByRef item As GridViewRow, ByVal i As Integer) As Object
        Dim value As Object = Nothing
        Dim control As Control = item.Cells(i).Controls(1)
        If TypeOf control Is TextBox Then
            value = CType(control, TextBox).Text.Trim
        End If

        Return value.ToString.Trim
    End Function

    Protected Sub btnGenerate_Click(sender As Object, e As EventArgs)
        SaveData()
        dvContent.InnerHtml = GenerateConstants()
    End Sub

    Private Function GenerateConstants() As String
        Dim entities As ConfigurationEntity() = Session(ddlModules.SelectedValue)
        Dim content As New StringBuilder
        For Each entity As ConfigurationEntity In entities
            content.Append(NEWLINE & TAB & TAB)
            content.Append(String.Format("public const string {0} = ""{1}"";", entity.DefaultName, If(entity.NewConfigurationId <> String.Empty, entity.NewConfigurationId, entity.ConfigurationId)))
        Next
        Return content.ToString
    End Function

    Private Sub SaveData()
        Dim entities As New List(Of ConfigurationEntity)
        Dim dataSource As ConfigurationEntity() = Session(ddlModules.SelectedValue)
        For Each item As GridViewRow In gridConfigData.Rows
            Dim length = item.Cells.Count
            Dim values(length) As String

            For i As Integer = 0 To (length - 1)
                values(i) = GetValue(item, i)
            Next
            Dim entity As ConfigurationEntity = dataSource(item.DataItemIndex)
            entity.DisplayName = values(1)
            entity.DefaultValue = values(3)
            entity.AllowedValues = values(4)
            entity.AllowedPlaceHolders = values(5)
            entity.Description = values(6)

            entities.Add(entity)
        Next
        Dim configDetail As New tbl_config_detail(GetDESettings())
        configDetail.UpdateAll(dataSource, ddlModules.SelectedValue)
        gridConfigData.DataSource = dataSource
        gridConfigData.DataBind()
    End Sub

End Class
