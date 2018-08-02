<%@ Page Language="VB" ValidateRequest="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    AutoEventWireup="false" CodeFile="PageHeadersAndTitles.aspx.vb" Inherits="PagesMaint_PageHeaderAndTitles"
    Title="pageHeadersAndTitles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="PageHeadersAndTitles1">
        <p class="title">
            <asp:Label ID="TitleLabel" runat="server"></asp:Label></p>
        <p class="instructions">
            <asp:Label ID="instructionsLabel" runat="server"></asp:Label></p>
        <div class="pageHeadersAndTitles2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th>
                            <asp:Label ID="BusinessUnitlbl" runat="server" /></th>
                        <td>
                            <asp:Label ID="BusinessUnitlbl1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="Partnerlbl" runat="server" /></th>
                        <td>
                            <asp:Label ID="Partnerlbl1" runat="server" /></td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="PageNamelbl" runat="server" /></th>
                        <td>
                            <asp:Label ID="PageNamelbl1" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="Descriptionlbl" runat="server" /></th>
                        <td>
                            <asp:Label ID="Descriptionlbl1" runat="server" /></td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div class="pageHeadersAndTitles3">
            <asp:GridView ID="Gridview1" CssClass="defaultTable" runat="server" AutoGenerateColumns="False" DataKeyNames="LANGUAGE_CODE,PAGE_CODE,PARTNER_CODE"
                DataSourceID="ObjectDataSource1" AllowPaging="True" EmptyDataText="No Record Found">
                <Columns>
                    <asp:TemplateField HeaderText="Language" SortExpression="LANGUAGE_CODE">
                        <ItemTemplate>
                            <asp:Button CssClass="button" ID="Button2" runat="server" Text='<%#Eval("LANGUAGE_CODE")%>'
                                CommandName='<%#Eval("ID") %>' OnClick="Editdata" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TITLE" HeaderText="Page Title" SortExpression="TITLE" />
                    <asp:BoundField DataField="PAGE_HEADER" HeaderText="Page Header" SortExpression="PAGE_HEADER" />
                    <asp:TemplateField SortExpression="PAGE_HTML_ID">
                        <ItemTemplate>
                            <asp:Button CssClass="button" ID="PAGE_HTML_ID" runat="server" CommandName='<%#Eval("ID") %>'
                                OnClick="RowDelete" Text="Delete Page Header" />
                            <asp:Button CssClass="button" ID="Button1" runat="server" Text="Edit Language Specific Text"
                                CommandName='<%#Eval("ID") %>' OnClick="Editdata2" />
                            <asp:Button CssClass="button" ID="Button3" runat="server" Text="Edit Attributes"
                                CommandName='<%#Eval("ID") %>' OnClick="Editdata3" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                SelectMethod="GetData" TypeName="PageHeaderAndTitlesTableAdapters.tbl_page_langTableAdapter">
                <SelectParameters>
                    <asp:QueryStringParameter Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                    <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                    <asp:QueryStringParameter Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <div class="pageHeadersAndTitlesButtons">
                <asp:ObjectDataSource ID="SearchDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                    SelectMethod="GetLanguageNotInData" TypeName="PageAddHeadersAndTitlesDatasetTableAdapters.tbl_languageTableAdapter"
                    DeleteMethod="Delete" InsertMethod="Insert" UpdateMethod="Update">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                        <asp:QueryStringParameter DefaultValue="" Name="PARTNER_CODE" QueryStringField="Partner"
                            Type="String" />
                        <asp:QueryStringParameter DefaultValue="" Name="PAGE_CODE" QueryStringField="PageName"
                            Type="String" />
                    </SelectParameters>
                    <DeleteParameters>
                        <asp:Parameter Name="Original_LANGUAGE_CODE" Type="String" />
                    </DeleteParameters>
                    <UpdateParameters>
                        <asp:Parameter Name="LANGUAGE_CODE" Type="String" />
                        <asp:Parameter Name="LANGUAGE_DESCRIPTION" Type="String" />
                        <asp:Parameter Name="Original_LANGUAGE_CODE" Type="String" />
                    </UpdateParameters>
                    <InsertParameters>
                        <asp:Parameter Name="LANGUAGE_CODE" Type="String" />
                        <asp:Parameter Name="LANGUAGE_DESCRIPTION" Type="String" />
                    </InsertParameters>
                </asp:ObjectDataSource>
                <asp:Button ID="Add_Page_Header_and_Titlebtn" runat="server" CssClass="button" />
                <asp:Button ID="Define_HTML_Includesbtn" runat="server" CssClass="button" />
                <asp:Button ID="Return_To_Page_Detailbtn" runat="server" CssClass="button" />
                <asp:Button ID="Return_To_Pagesbtn" runat="server" CssClass="button" />
            </div>
        </div>
    </div>
</asp:Content>
