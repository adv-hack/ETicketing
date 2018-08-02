<%@ Page Language="VB" ValidateRequest="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="PageHtmlIncludes.aspx.vb" Inherits="_Default"
    Title="PageHtmlIncludes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="page-html-includes">
        <p class="title"><asp:Label ID="TitleLabel" runat="server" /></p>
        <p class="instructions"><asp:Label ID="InstructionsLabel" runat="server" Text="Label" /></p>

        <div class="page-html-includes-summary-wrapper">
            <table cellspacing="0" class="page-html-includes-summary vertical">
                <tbody>
                    <tr class="business-unit">
                        <th><asp:Label ID="BusinessUnitlbl" runat="server" /></th>
                        <td><asp:Label ID="BusinessUnitlbl1" runat="server" /></td>
                    </tr>
                    <tr class="partner">
                        <th><asp:Label ID="Partnerlbl" runat="server" /></th>
                        <td><asp:Label ID="Partnerlbl1" runat="server" /></td>
                    </tr>
                    <tr class="page-name">
                        <th><asp:Label ID="PageNamelbl" runat="server" /></th>
                        <td><asp:Label ID="PageNamelbl1" runat="server" /></td>
                    </tr>
                    <tr class="description">
                        <th><asp:Label ID="Descriptionlbl" runat="server" /></th>
                        <td><asp:Label ID="Descriptionlbl1" runat="server" /></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="page-html-includes-data-wrapper">
            <asp:GridView ID="GridView1" runat="server" CssClass="page-html-includes-data horizontal"
                summary="tbl_page list" AutoGenerateColumns="False" AllowPaging="True" DataSourceID="Page_Html_Includes_DataSource" EmptyDataText="No Record Found">
                <Columns>
                    <asp:TemplateField HeaderText="Location" SortExpression="SECTION" ItemStyle-CssClass="location" HeaderStyle-CssClass="location">
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%#Eval("SECTION")%>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SEQUENCE" HeaderText="Sequence" SortExpression="SEQUENCE" ItemStyle-CssClass="sequence" HeaderStyle-CssClass="sequence" />
                    <asp:BoundField DataField="PAGE_QUERYSTRING" HeaderText="Query String" SortExpression="PAGE_QUERYSTRING" ItemStyle-CssClass="query-string" HeaderStyle-CssClass="query-string" />
                    <asp:BoundField DataField="HTML_LOCATION" HeaderText="HTML Include Name" SortExpression="HTML_LOCATION" ItemStyle-CssClass="html-include-name" HeaderStyle-CssClass="html-include-name" />
                    <asp:TemplateField SortExpression="PAGE_HTML_ID" ItemStyle-CssClass="buttons" HeaderStyle-CssClass="buttons">
                        <ItemTemplate>
                            <asp:Button Enabled="false" ID="PAGE_HTML_EDIT" CssClass="edit button" runat="server" CommandName='<%#Eval("PAGE_HTML_ID") %>' OnClick="Editdata" Text="Edit" />
                            <asp:Button Enabled="false" ID="PAGE_HTML_DELETE" CssClass="delete button" runat="server" CommandName='<%#Eval("PAGE_HTML_ID") %>' OnClick="RowDelete" Text="Delete" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="Page_Html_Includes_DataSource" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetData" TypeName="testdatasetTableAdapters.tbl_page_htmlTableAdapter">
                <SelectParameters>
                    <asp:QueryStringParameter Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                    <asp:QueryStringParameter DefaultValue="" Name="Partner" QueryStringField="Partner" Type="String" />
                    <asp:QueryStringParameter DefaultValue="" Name="BusinessUnit" QueryStringField="BusinessUnit" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>

            <div class="page-html-includes-buttons button-bar">
                <asp:Button ID="Add_HTML_Includebtn" runat="server" CssClass="add-html-include button" />
                <asp:Button ID="Page_Headers_and_Titlesbtn" runat="server" CssClass="page-headers-and-titles button" />
                <asp:Button ID="Return_To_Page_Detailbtn" runat="server" CssClass="return-to-page-details button" />
                <asp:Button ID="Return_To_Pagesbtn" runat="server" CssClass="return-to-pages button" />
            </div>
        </div>
    </div>
</asp:Content>
