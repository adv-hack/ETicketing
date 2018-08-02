Public Class DropDownListAdapter
    Inherits System.Web.UI.WebControls.Adapters.WebControlAdapter

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        Dim list As DropDownList = Me.Control
        Dim currentOptionGroup As String
        Dim renderedOptionGroups As New Generic.List(Of String)

        For Each item As ListItem In list.Items
            Page.ClientScript.RegisterForEventValidation(list.UniqueID, item.Value)
            If item.Attributes("OptionGroup") IsNot Nothing Then
                'The item is part of an option group
                currentOptionGroup = item.Attributes("OptionGroup")
                If Not renderedOptionGroups.Contains(currentOptionGroup) Then
                    'the header was not written- do that first
                    If (renderedOptionGroups.Count > 0) Then
                        RenderOptionGroupEndTag(writer) 'need to close previous group
                    End If
                    RenderOptionGroupBeginTag(currentOptionGroup, writer)
                    renderedOptionGroups.Add(currentOptionGroup)
                End If
                RenderListItem(item, writer)
            ElseIf item.Text = "--" Then 'simple separator
                RenderOptionGroupBeginTag("--", writer)
                RenderOptionGroupEndTag(writer)
            Else
                'default behavior: render the list item as normal
                RenderListItem(item, writer)
            End If
        Next item

        If renderedOptionGroups.Count > 0 Then
            RenderOptionGroupEndTag(writer)
        End If
    End Sub

    Private Sub RenderOptionGroupBeginTag(ByVal name As String, ByVal writer As HtmlTextWriter)
        writer.WriteBeginTag("optgroup")
        writer.WriteAttribute("label", name)
        writer.Write(HtmlTextWriter.TagRightChar)
        writer.WriteLine()
    End Sub

    Private Sub RenderOptionGroupEndTag(ByVal writer As HtmlTextWriter)
        writer.WriteEndTag("optgroup")
        writer.WriteLine()
    End Sub

    Private Sub RenderListItem(ByVal item As ListItem, ByVal writer As HtmlTextWriter)
        writer.WriteBeginTag("option")
        writer.WriteAttribute("value", item.Value, True)
        If item.Selected Then
            writer.WriteAttribute("selected", "selected", False)
        End If

        For Each key As String In item.Attributes.Keys
            writer.WriteAttribute(key, item.Attributes(key))
        Next key

        writer.Write(HtmlTextWriter.TagRightChar)
        HttpUtility.HtmlEncode(item.Text, writer)
        writer.WriteEndTag("option")
        writer.WriteLine()
    End Sub
End Class