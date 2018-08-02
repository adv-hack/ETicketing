Imports System.Data.SqlClient
Imports System.Data
Imports System.Collections.Generic
Imports Talent.eCommerce

Partial Class UserControls_advancedSearchType4
    Inherits AbstractTalentUserControl

    Private Const SQL2005 As String = "SqlServer2005"
    Private criteriaDropDownLists As List(Of DropDownList)
    Private _queryString As String
    Public Property QueryString() As String
        Get
            Return _queryString
        End Get
        Set(ByVal value As String)
            _queryString = value
        End Set
    End Property

    Protected Sub DropDownListGroup1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownListGroup1.SelectedIndexChanged
        Dim newURL As StringBuilder = New StringBuilder()
        Dim oldURl As String = Request.Url.ToString
        oldURl = removeRequestParameter(oldURl, "Group1")
        oldURl = removeRequestParameter(oldURl, "Group2")
        newURL.Append(oldURl)
        If Not newURL.ToString.Contains("?") Then
            newURL.Append("?Group1=" & DropDownListGroup1.SelectedValue)
        Else
            newURL.Append("&Group1=" & DropDownListGroup1.SelectedValue)
        End If
        Response.Redirect(newURL.ToString)
    End Sub

    Protected Sub SetupGroup1DropDownList()
        Dim selectStatement As String = "SELECT DISTINCT tbl_group.GROUP_DESCRIPTION_1, tbl_group.GROUP_NAME FROM tbl_beko_advanced_search INNER JOIN tbl_group ON tbl_beko_advanced_search.GROUP1 = tbl_group.GROUP_NAME"
        Dim conTalent As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
        conTalent.Open()
        Dim command As SqlCommand = New SqlCommand(selectStatement, conTalent)
        Dim reader As SqlDataReader = command.ExecuteReader()
        Dim dt As New DataTable()
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("GroupName", GetType(String)))
        dt.Columns.Add(New DataColumn("GroupDescription", GetType(String)))
        dr = dt.NewRow()
        dr(0) = Talent.Common.Utilities.GetAllString()
        dr(1) = "All"
        dt.Rows.Add(dr)
        If (reader.HasRows) Then
            While (reader.Read())
                dr = dt.NewRow()
                dr(0) = reader.Item(1)
                dr(1) = reader.Item(0)
                dt.Rows.Add(dr)
            End While
        End If
        DropDownListGroup1.DataSource = New DataView(dt)
        DropDownListGroup1.DataTextField = "GroupDescription"
        DropDownListGroup1.DataValueField = "GroupName"
        DropDownListGroup1.DataBind()
        If Not Request("Group1") Is Nothing Then
            DropDownListGroup1.SelectedValue = Request("Group1")
        End If
    End Sub

    Protected Sub SetupGroup2DropDownList()
        Dim selectStatement As String = "SELECT DISTINCT tbl_group.GROUP_DESCRIPTION_2, tbl_beko_advanced_search.GROUP2 FROM tbl_beko_advanced_search INNER JOIN tbl_group ON tbl_beko_advanced_search.GROUP2 = tbl_group.GROUP_NAME WHERE (tbl_beko_advanced_search.GROUP1 = @GROUP_NAME)"
        Dim conTalent As SqlConnection = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
        conTalent.Open()
        Dim command As SqlCommand = New SqlCommand(selectStatement, conTalent)
        command.Parameters.Add("@GROUP_NAME", SqlDbType.NChar)
        command.Parameters("@GROUP_NAME").Value = DropDownListGroup1.SelectedValue
        Dim reader As SqlDataReader = command.ExecuteReader()
        Dim dt As New DataTable()
        Dim dr As DataRow
        dt.Columns.Add(New DataColumn("GroupName", GetType(String)))
        dt.Columns.Add(New DataColumn("GroupDescription", GetType(String)))
        dr = dt.NewRow()
        dr(0) = Talent.Common.Utilities.GetAllString()
        dr(1) = "All"
        dt.Rows.Add(dr)
        If (reader.HasRows) Then
            While (reader.Read())
                dr = dt.NewRow()
                dr(0) = reader.Item(1)
                dr(1) = reader.Item(0)
                dt.Rows.Add(dr)
            End While
        End If
        DropDownListGroup2.DataSource = New DataView(dt)
        DropDownListGroup2.DataTextField = "GroupDescription"
        DropDownListGroup2.DataValueField = "GroupName"
        DropDownListGroup2.DataBind()
        If Not Request("Group2") Is Nothing Then
            DropDownListGroup2.SelectedValue = Request("Group2")
        End If
    End Sub

    Protected Overrides Sub setupUCR()

    End Sub

    Protected Sub page_load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        criteriaDropDownLists = New List(Of DropDownList)
        If Not Request("Group1") Is Nothing Then
            DropDownListGroup2.Visible = True
            If Not Request("Group2") Is Nothing Then
                LoadDropDownBoxes()
            End If
        Else
            DropDownListGroup2.Visible = False
        End If
    End Sub

    Protected Sub LoadDropDownBoxes()
        Dim selectStatement As String = "SELECT * FROM tbl_beko_advanced_search WHERE GROUP1 = @GROUP1 and GROUP2 = @GROUP2"
        Dim parm1 As String = "@GROUP1"
        Dim parm2 As String = "@GROUP2"

        Dim conTalent As SqlConnection = Nothing
        Dim searchList As List(Of String) = New List(Of String)
        Dim s = ConfigurationManager.ConnectionStrings(SQL2005)
        Try
            conTalent = New SqlConnection(ConfigurationManager.ConnectionStrings(SQL2005).ConnectionString)
            conTalent.Open()
            Dim command As SqlCommand = New SqlCommand(selectStatement, conTalent)
            command.Parameters.Add(New SqlParameter(parm1, SqlDbType.Char, 50))
            command.Parameters(parm1).Value = Request("Group1")
            command.Parameters.Add(New SqlParameter(parm2, SqlDbType.Char, 50))
            command.Parameters(parm2).Value = Request("Group2")

            Dim reader As SqlDataReader = command.ExecuteReader()
            If (reader.HasRows) Then
                While (reader.Read())
                    searchList.Add(reader.Item("CONTAINS_DATA_FROM"))
                End While
            End If

            Repeater1.DataSource = searchList
            Repeater1.DataBind()

        Catch ex As Exception
            'Should handle the exception here?
        End Try

    End Sub

    Protected Sub Pre_Render(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        SetupGroup1DropDownList()
        If Not Request("Group1") Is Nothing Then
            DropDownListGroup1.SelectedValue = Request("Group1")
            SetupGroup2DropDownList()
        End If
        If Not Request("Group2") Is Nothing Then
            DropDownListGroup2.SelectedValue = Request("Group2")
        End If
    End Sub

    Private Function removeRequestParameter(ByVal url As String, ByVal paramKey As String) As String
        Dim newUrl As StringBuilder = New StringBuilder()
        Dim oldurl As String = url
        If Not Request.QueryString(paramKey) Is Nothing Then
            Dim index As Integer
            If url.LastIndexOf("&" & paramKey & "=") < 0 Then
                index = oldURl.LastIndexOf("?" & paramKey & "=")
            Else
                index = oldURl.LastIndexOf("&" & paramKey & "=")
            End If
            Dim base As String = oldurl.Remove(index, oldurl.Length - index)
            oldurl = oldurl.Remove(0, index + 1)
            newUrl.Append(base)
            If oldurl.Contains("&") Then
                oldurl = oldurl.Remove(0, oldurl.IndexOf("&"))
                newUrl.Append(oldurl)
            End If
        Else
            newUrl.Append(url)
        End If

        Return newUrl.ToString()
    End Function

    Protected Sub DropDownListGroup2_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DropDownListGroup2.SelectedIndexChanged
        Dim newURL As StringBuilder = New StringBuilder()
        Dim oldURl As String = Request.Url.ToString
        oldURl = removeRequestParameter(oldURl, "Group1")
        oldURl = removeRequestParameter(oldURl, "Group2")
        newURL.Append(oldURl)
        If Not newURL.ToString.Contains("?") Then
            newURL.Append("?Group1=" & DropDownListGroup1.SelectedValue)
        Else
            newURL.Append("&Group1=" & DropDownListGroup1.SelectedValue)
        End If
        newURL.Append("&Group2=" & DropDownListGroup2.SelectedValue)
        LoadDropDownBoxes()
        Response.Redirect(newURL.ToString)
    End Sub


    Protected Sub Repeater1_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles Repeater1.ItemDataBound
        Dim dropdownlist3 As DropDownList = CType(e.Item.FindControl("DropDownList3"), DropDownList)
        Dim s As String = CType(e.Item.DataItem, String)
        dropdownlist3.ID = s
        Dim ps As ProductSearch = New ProductSearch
        Dim number As String = s.Replace("PRODUCT_SEARCH_CRITERIA_", "")
        dropdownlist3.DataSource = ps.GetCriteria(_businessUnit, Talent.Common.Utilities.GetAllString, Request("Group1"), Integer.Parse(number))
        If dropdownlist3.Items.Count > 0 Then
            criteriaDropDownLists.Add(dropdownlist3)
        Else
            dropdownlist3.Visible = False
        End If
    End Sub

    Public Function BuildQueryString() As String

        If Not DropDownListGroup1.SelectedValue.Equals(String.Empty) Then
            QueryString &= AppendQueryItem("group01", DropDownListGroup1.SelectedValue)
        End If
        If Not DropDownListGroup2.SelectedValue.Equals(String.Empty) Then
            QueryString &= AppendQueryItem("group02", DropDownListGroup2.SelectedValue)
        End If

        For Each criteria As DropDownList In criteriaDropDownLists
            Dim s As String = criteria.ID
            s = s.Replace("PRODUCT_SEARCH_CRITERIA_", "")
            If Integer.Parse(s) < 10 Then
                s = "0" + s
            End If
            If Not criteria.SelectedValue Is Nothing AndAlso Not criteria.SelectedValue.Equals("") Then
                QueryString &= AppendQueryItem("criteria" & s, criteria.SelectedValue)
            End If
        Next

        Return QueryString()
    End Function

    Private Function AppendQueryItem(ByVal name As String, ByVal value As String) As String
        Dim item As String = String.Empty
        item &= "&" & name & "=" & value
        Return item
    End Function
End Class
