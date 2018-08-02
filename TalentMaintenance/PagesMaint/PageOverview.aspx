<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="PageOverview.aspx.vb" Inherits="PageOverview" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <div id="page-overview">
        <p class="title"><asp:Label ID="titleLabel" runat="server" Text="PageLabel" /></p>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" Text="PageLabel" /></p>
        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <p class="error"><asp:Label ID="ErrorLabel" runat="server" /></p>
        </asp:PlaceHolder>

        <div class="page-overview-wrapper">
            <table cellspacing="0" class="page-overview vertical">
                <tbody>
                    <tr class="business-unit">
                        <th scope="row"><asp:Label ID="businessunitLabel" runat="server" /></th>
                        <td><asp:DropDownList CssClass="select" ID="businessunitDropDownList" runat="server" AutoPostBack="True" /></td>
                    </tr>
                    <tr class="page-type">
                        <th><asp:Label ID="lblPageTypeLabel" runat="server" /></th>
                        <td>
                            <asp:RadioButton ID="rdoAllPages" runat="server" GroupName="PageType" AutoPostBack="true" />
                            <asp:RadioButton ID="rdoStandardPages" runat="server" GroupName="PageType" AutoPostBack="true" />
                            <asp:RadioButton ID="rdoUserDefinedPages" runat="server" GroupName="PageType" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr class="page">
                        <th><asp:Label ID="pageLabel" runat="server" /></th>
                        <td><asp:DropDownList CssClass="select" ID="PageDDL" runat="server" AutoPostBack="true" /></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <asp:PlaceHolder ID="plhPageGridView" runat="server" Visible="False">
            <div class="pageOverview3">
                <asp:GridView ID="PageGridView" CssClass="defaultTable" AllowPaging="True" Width="100%"
                    PagerStyle-HorizontalAlign="Left" GridLines="None" runat="server" AutoGenerateColumns="False"
                    DataSourceID="ObjectDataSource1" PagerSettings-Mode="Numeric" EmptyDataText="No Record Found">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID">
                            <HeaderStyle CssClass="Hide" />
                            <ItemStyle CssClass="Hide" VerticalAlign="Middle" />
                        </asp:BoundField>
                        <asp:BoundField DataField="PAGE_CODE" HeaderText="Page">
                            <HeaderStyle />
                            <ItemStyle CssClass="row" />
                        </asp:BoundField>
                        <asp:BoundField DataField="DESCRIPTION" HeaderText="Description">
                            <HeaderStyle />
                            <ItemStyle CssClass="row" />
                        </asp:BoundField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton CssClass="button" ID="DeleteLinkButton" CommandArgument='<%#Eval("PAGE_CODE")%>'
                                    CommandName='<%#Eval("ID")%>' runat="server" OnClick="RowDelete" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <PagerStyle />
                </asp:GridView>
                <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="" TypeName="PageDataSetTableAdapters.PageDateTableAdapter">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                        <asp:QueryStringParameter Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                    </SelectParameters>
                </asp:ObjectDataSource>
            </div>
        </asp:PlaceHolder>

        <div class="page-overview-buttons button-bar">
            <asp:Button CssClass="add-new-page button" ID="AddNewPageButton" runat="server" />
            <asp:Button CssClass="edit-controls button" ID="EditControlsButton" runat="server" />
        </div>
    </div>
</asp:Content>
