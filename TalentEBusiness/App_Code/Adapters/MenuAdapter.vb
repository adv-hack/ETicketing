Imports System
Imports System.IO
Imports System.Web
Imports System.Web.Configuration
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.HtmlControls

Namespace CSSFriendly
    Public Class MenuAdapter
        Inherits System.Web.UI.WebControls.Adapters.MenuAdapter

        Protected This As WebControlAdapterExtender = Nothing
        Private _extender As WebControlAdapterExtender = Nothing
        Private ReadOnly Property Extender() As WebControlAdapterExtender
            Get
                If ((IsNothing(_extender) AndAlso (Not IsNothing(Control))) OrElse _
                    ((Not IsNothing(_extender)) AndAlso (Not Control.Equals(_extender.AdaptedControl)))) Then
                    _extender = New WebControlAdapterExtender(Control)
                End If

                System.Diagnostics.Debug.Assert(Not IsNothing(_extender), "CSS Friendly adapters internal error", "Null extender instance")
                Return _extender
            End Get
        End Property

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)

            If (Extender.AdapterEnabled) Then
                RegisterScripts()
            End If
        End Sub

        Private Sub RegisterScripts()
            Extender.RegisterScripts()

            'Dim folderPath As String = WebConfigurationManager.AppSettings.Get("CSSFriendly-JavaScript-Path")
            'If (String.IsNullOrEmpty(folderPath)) Then
            '    folderPath = "~/JavaScript"
            'End If
            Dim folderPath As String = Talent.eCommerce.Utilities.GetStaticContentPath("Javascript", Nothing)

            Dim filePath As String = IIf(folderPath.EndsWith("/"), folderPath & "MenuAdapter.js", folderPath & "/MenuAdapter.js")
            Page.ClientScript.RegisterClientScriptInclude(Me.GetType(), Me.GetType().ToString(), Page.ResolveUrl(filePath))
        End Sub

        Protected Overrides Sub RenderBeginTag(ByVal writer As HtmlTextWriter)
            If (Extender.AdapterEnabled) Then
                If ((Not IsNothing(Control)) AndAlso (Not String.IsNullOrEmpty(Control.Attributes.Item("CssSelectorClass")))) Then
                    writer.WriteLine()
                    writer.WriteBeginTag("div")
                    writer.WriteAttribute("class", Control.Attributes("CssSelectorClass"))
                    writer.Write(HtmlTextWriter.TagRightChar)
                    writer.Indent = writer.Indent + 1
                End If

                writer.WriteLine()
                writer.WriteBeginTag("div")
                writer.WriteAttribute("class", "AspNet-Menu-" + Control.Orientation.ToString())
                writer.Write(HtmlTextWriter.TagRightChar)
            Else
                MyBase.RenderBeginTag(writer)
            End If
        End Sub

        Protected Overrides Sub RenderEndTag(ByVal writer As HtmlTextWriter)
            If (Extender.AdapterEnabled) Then
                writer.WriteEndTag("div")

                If ((Not IsNothing(Control)) AndAlso (Not String.IsNullOrEmpty(Control.Attributes.Item("CssSelectorClass")))) Then
                    writer.Indent = writer.Indent - 1
                    writer.WriteLine()
                    writer.WriteEndTag("div")
                End If

                writer.WriteLine()
            Else
                MyBase.RenderEndTag(writer)
            End If
        End Sub

        Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
            If (Extender.AdapterEnabled) Then
                writer.Indent = writer.Indent + 1
                BuildItems(Control.Items, True, writer)
                writer.Indent = writer.Indent - 1
                writer.WriteLine()
            Else
                MyBase.RenderContents(writer)
            End If
        End Sub

        Private Sub BuildItems(ByVal items As MenuItemCollection, ByVal isRoot As Boolean, ByVal writer As HtmlTextWriter)
            If (items.Count > 0) Then
                writer.WriteLine()

                writer.WriteBeginTag("ul")
                If isRoot Then
                    writer.WriteAttribute("class", "AspNet-Menu")
                End If
                writer.Write(HtmlTextWriter.TagRightChar)
                writer.Indent = writer.Indent + 1

                Dim item As MenuItem
                For Each item In items
                    BuildItem(item, writer)
                Next

                writer.Indent = writer.Indent - 1
                writer.WriteLine()
                writer.WriteEndTag("ul")
            End If
        End Sub

        Private Sub BuildItem(ByVal item As MenuItem, ByVal writer As HtmlTextWriter)
            Dim menu As Menu = Control
            If ((Not IsNothing(menu)) AndAlso (Not IsNothing(item)) AndAlso (Not IsNothing(writer))) Then
                writer.WriteLine()
                writer.WriteBeginTag("li")

                Dim theClass As String = IIf(item.ChildItems.Count > 0, "AspNet-Menu-WithChildren", "AspNet-Menu-Leaf")
                Dim selectedStatusClass As String = GetSelectStatusClass(item)
                If (Not String.IsNullOrEmpty(selectedStatusClass)) Then
                    theClass &= " " & selectedStatusClass
                End If

                writer.WriteAttribute("class", theClass)
                writer.Write(HtmlTextWriter.TagRightChar)
                writer.Indent = writer.Indent + 1
                writer.WriteLine()

                If (((item.Depth < menu.StaticDisplayLevels) AndAlso (Not IsNothing(menu.StaticItemTemplate))) OrElse _
                    ((item.Depth >= menu.StaticDisplayLevels) AndAlso (Not IsNothing(menu.DynamicItemTemplate)))) Then
                    writer.WriteBeginTag("div")
                    writer.WriteAttribute("class", GetItemClass(menu, item))
                    writer.Write(HtmlTextWriter.TagRightChar)
                    writer.Indent = writer.Indent + 1
                    writer.WriteLine()

                    Dim container As MenuItemTemplateContainer = New MenuItemTemplateContainer(menu.Items.IndexOf(item), item)
                    If ((item.Depth < menu.StaticDisplayLevels) AndAlso (Not IsNothing(menu.StaticItemTemplate))) Then
                        menu.StaticItemTemplate.InstantiateIn(container)
                    Else
                        menu.DynamicItemTemplate.InstantiateIn(container)
                    End If
                    container.DataBind()
                    container.RenderControl(writer)

                    writer.Indent = writer.Indent - 1
                    writer.WriteLine()
                    writer.WriteEndTag("div")
                Else
                    If (IsLink(item)) Then
                        writer.WriteBeginTag("a")
                        If (Not String.IsNullOrEmpty(item.NavigateUrl)) Then
                            writer.WriteAttribute("href", Page.Server.HtmlEncode(menu.ResolveClientUrl(item.NavigateUrl)))
                        Else
                            writer.WriteAttribute("href", Page.ClientScript.GetPostBackClientHyperlink(menu, "b" & item.ValuePath.Replace(menu.PathSeparator.ToString(), "\"), True))
                        End If


                        writer.WriteAttribute("class", GetItemClass(menu, item))
                        WebControlAdapterExtender.WriteTargetAttribute(writer, item.Target)

                        If (Not String.IsNullOrEmpty(item.ToolTip)) Then
                            writer.WriteAttribute("title", item.ToolTip)
                        ElseIf (Not String.IsNullOrEmpty(menu.ToolTip)) Then
                            writer.WriteAttribute("title", menu.ToolTip)
                        End If
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1
                        writer.WriteLine()
                    Else
                        writer.WriteBeginTag("span")
                        writer.WriteAttribute("class", GetItemClass(menu, item))
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1
                        writer.WriteLine()
                    End If

                    If (Not String.IsNullOrEmpty(item.ImageUrl)) Then
                        writer.WriteBeginTag("img")
                        writer.WriteAttribute("src", menu.ResolveClientUrl(item.ImageUrl))
                        writer.WriteAttribute("alt", IIf(Not String.IsNullOrEmpty(item.ToolTip), item.ToolTip, IIf(Not String.IsNullOrEmpty(menu.ToolTip), menu.ToolTip, item.Text)))
                        writer.Write(HtmlTextWriter.SelfClosingTagEnd)
                    End If

                    writer.Write(item.Text)

                    If (IsLink(item)) Then
                        writer.Indent = writer.Indent - 1
                        writer.WriteEndTag("a")
                    Else
                        writer.Indent = writer.Indent - 1
                        writer.WriteEndTag("span")
                    End If

                End If

                If (Not IsNothing(item.ChildItems) AndAlso (item.ChildItems.Count > 0)) Then
                    BuildItems(item.ChildItems, False, writer)
                End If

                writer.Indent = writer.Indent - 1
                writer.WriteLine()
                writer.WriteEndTag("li")
            End If
        End Sub

        Private Function IsLink(ByVal item As MenuItem) As Boolean
            Return (Not IsNothing(item)) AndAlso item.Enabled AndAlso ((Not String.IsNullOrEmpty(item.NavigateUrl)) OrElse item.Selectable)
        End Function

        Private Function GetItemClass(ByVal menu As Menu, ByVal item As MenuItem) As String
            Dim value As String = "AspNet-Menu-NonLink"
            If Not item Is Nothing Then
                If (((item.Depth < menu.StaticDisplayLevels) AndAlso (Not IsNothing(menu.StaticItemTemplate))) OrElse _
                    (item.Depth >= menu.StaticDisplayLevels) AndAlso (Not IsNothing(menu.DynamicItemTemplate))) Then
                    value = "AspNet-Menu-Template"
                ElseIf (IsLink(item)) Then
                    value = "AspNet-Menu-Link"
                End If
                Dim selectedStatusClass As String = GetSelectStatusClass(item)
                If (Not String.IsNullOrEmpty(selectedStatusClass)) Then
                    value &= " " & selectedStatusClass
                End If
            End If
            Return value
        End Function

        Private Function GetSelectStatusClass(ByVal item As MenuItem) As String
            Dim value As String = ""
            If item.Selected Then
                value = "AspNet-Menu-Selected"
            ElseIf IsChildItemSelected(item) Then
                value = "AspNet-Menu-ChildSelected"
            ElseIf IsParentItemSelected(item) Then
                value = "AspNet-Menu-ParentSelected"
            End If
            Return value
        End Function

        Private Function IsChildItemSelected(ByVal item As MenuItem) As Boolean
            Dim bRet As Boolean = False

            If ((Not IsNothing(item)) AndAlso (Not IsNothing(item.ChildItems))) Then
                bRet = IsChildItemSelected(item.ChildItems)
            End If

            Return bRet
        End Function

        Private Function IsChildItemSelected(ByVal items As MenuItemCollection) As Boolean
            Dim bRet As Boolean = False

            If Not items Is Nothing Then
                Dim item As MenuItem
                For Each item In items
                    If (item.Selected OrElse IsChildItemSelected(item.ChildItems)) Then
                        bRet = True
                        Exit For
                    End If
                Next
            End If

            Return bRet
        End Function

        Private Function IsParentItemSelected(ByVal item As MenuItem) As Boolean
            Dim bRet As Boolean = False

            If ((Not IsNothing(item)) AndAlso (Not IsNothing(item.Parent))) Then
                If item.Parent.Selected Then
                    bRet = True
                Else
                    bRet = IsParentItemSelected(item.Parent)
                End If
            End If

            Return bRet
        End Function
    End Class
End Namespace
