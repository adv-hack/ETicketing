Imports System
Imports System.Data
Imports System.Collections
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

Namespace CSSFriendly
    Public Class DataListAdapter
        Inherits System.Web.UI.WebControls.Adapters.WebControlAdapter

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

        Private ReadOnly Property RepeatColumns() As Integer
            Get
                Dim dataList As DataList = Control
                Dim nRet As Integer = 1
                If (Not IsNothing(dataList)) Then
                    If dataList.RepeatColumns = 0 Then
                        If dataList.RepeatDirection = RepeatDirection.Horizontal Then
                            nRet = dataList.Items.Count
                        End If
                    Else
                        nRet = dataList.RepeatColumns
                    End If
                End If
                Return nRet
            End Get
        End Property

        '/ ///////////////////////////////////////////////////////////////////////////////
        '/ PROTECTED        

        Protected Overrides Sub OnInit(ByVal e As EventArgs)
            MyBase.OnInit(e)

            If (Extender.AdapterEnabled) Then
                RegisterScripts()
            End If
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
                writer.WriteAttribute("class", "AspNet-DataList")
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
                Dim dataList As DataList = Control
                If Not IsNothing(dataList) Then
                    writer.Indent = writer.Indent + 1
                    writer.WriteLine()
                    writer.WriteBeginTag("table")
                    writer.WriteAttribute("cellpadding", "0")
                    writer.WriteAttribute("cellspacing", "0")
                    writer.WriteAttribute("summary", Control.ToolTip)
                    writer.Write(HtmlTextWriter.TagRightChar)
                    writer.Indent = writer.Indent + 1

                    If Not IsNothing(dataList.HeaderTemplate) Then
                        Dim container As PlaceHolder = New PlaceHolder()
                        dataList.HeaderTemplate.InstantiateIn(container)
                        container.DataBind()

                        If (container.Controls.Count = 1) AndAlso GetType(LiteralControl).IsInstanceOfType(container.Controls.Item(0)) Then
                            writer.WriteLine()
                            writer.WriteBeginTag("caption")
                            writer.Write(HtmlTextWriter.TagRightChar)

                            Dim literalControl As LiteralControl = CType(container.Controls.Item(0), LiteralControl)
                            writer.Write(literalControl.Text.Trim())
                            writer.WriteEndTag("caption")
                        Else
                            writer.WriteLine()
                            writer.WriteBeginTag("thead")
                            writer.Write(HtmlTextWriter.TagRightChar)
                            writer.Indent = writer.Indent + 1

                            writer.WriteLine()
                            writer.WriteBeginTag("tr")
                            writer.Write(HtmlTextWriter.TagRightChar)
                            writer.Indent = writer.Indent + 1

                            writer.WriteLine()
                            writer.WriteBeginTag("th")
                            writer.WriteAttribute("colspan", RepeatColumns.ToString())
                            writer.Write(HtmlTextWriter.TagRightChar)
                            writer.Indent = writer.Indent + 1

                            container.RenderControl(writer)

                            writer.Indent = writer.Indent - 1
                            writer.WriteLine()
                            writer.WriteEndTag("th")

                            writer.Indent = writer.Indent - 1
                            writer.WriteLine()
                            writer.WriteEndTag("tr")

                            writer.Indent = writer.Indent - 1
                            writer.WriteLine()
                            writer.WriteEndTag("thead")
                        End If
                    End If

                    If Not IsNothing(dataList.FooterTemplate) Then
                        writer.WriteLine()
                        writer.WriteBeginTag("tfoot")
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1

                        writer.WriteLine()
                        writer.WriteBeginTag("tr")
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1

                        writer.WriteLine()
                        writer.WriteBeginTag("td")
                        writer.WriteAttribute("colspan", RepeatColumns.ToString())
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1

                        Dim container As PlaceHolder = New PlaceHolder()
                        dataList.FooterTemplate.InstantiateIn(container)
                        container.DataBind()
                        container.RenderControl(writer)

                        writer.Indent = writer.Indent - 1
                        writer.WriteLine()
                        writer.WriteEndTag("td")

                        writer.Indent = writer.Indent - 1
                        writer.WriteLine()
                        writer.WriteEndTag("tr")

                        writer.Indent = writer.Indent - 1
                        writer.WriteLine()
                        writer.WriteEndTag("tfoot")
                    End If

                    If Not IsNothing(dataList.ItemTemplate) Then
                        writer.WriteLine()
                        writer.WriteBeginTag("tbody")
                        writer.Write(HtmlTextWriter.TagRightChar)
                        writer.Indent = writer.Indent + 1

                        Dim nItemsInColumn As Integer = CType(Math.Ceiling((CType(dataList.Items.Count, Double)) / (CType(RepeatColumns, Double))), Integer)
                        Dim iItem As Integer
                        For iItem = 0 To dataList.Items.Count - 1
                            Dim nRow As Integer = iItem \ RepeatColumns
                            Dim nCol As Integer = Decimal.Remainder(iItem, RepeatColumns)
                            Dim nDesiredIndex As Integer = iItem
                            If dataList.RepeatDirection = RepeatDirection.Vertical Then
                                nDesiredIndex = (nCol * nItemsInColumn) + nRow
                            End If

                            If (Decimal.Remainder(iItem, RepeatColumns) = 0) Then
                                writer.WriteLine()
                                writer.WriteBeginTag("tr")
                                writer.Write(HtmlTextWriter.TagRightChar)
                                writer.Indent = writer.Indent + 1
                            End If

                            writer.WriteLine()
                            writer.WriteBeginTag("td")
                            writer.Write(HtmlTextWriter.TagRightChar)
                            writer.Indent = writer.Indent + 1

                            Dim itemCtrl As Control
                            For Each itemCtrl In dataList.Items(iItem).Controls
                                itemCtrl.RenderControl(writer)
                            Next

                            writer.Indent = writer.Indent - 1
                            writer.WriteLine()
                            writer.WriteEndTag("td")

                            If (Decimal.Remainder((iItem + 1), RepeatColumns) = 0) Then
                                writer.Indent = writer.Indent - 1
                                writer.WriteLine()
                                writer.WriteEndTag("tr")
                            End If
                        Next

                        If (Decimal.Remainder(dataList.Items.Count, RepeatColumns) <> 0) Then
                            writer.Indent = writer.Indent - 1
                            writer.WriteLine()
                            writer.WriteEndTag("tr")
                        End If

                        writer.Indent = writer.Indent - 1
                        writer.WriteLine()
                        writer.WriteEndTag("tbody")
                    End If

                    writer.Indent = writer.Indent - 1
                    writer.WriteLine()
                    writer.WriteEndTag("table")

                    writer.Indent = writer.Indent - 1
                    writer.WriteLine()
                End If
            Else
                MyBase.RenderContents(writer)
            End If
        End Sub

        '/ ///////////////////////////////////////////////////////////////////////////////
        '/ PRIVATE        

        Private Sub RegisterScripts()
        End Sub
    End Class
End Namespace
