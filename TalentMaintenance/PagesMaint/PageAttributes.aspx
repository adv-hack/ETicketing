<%@ Page Language="VB" ValidateRequest="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master"
    Inherits="PagesMaint_PageAttributes" AutoEventWireup="false" CodeFile="PageAttributes.aspx.vb"
    Title="PageAttributes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">

    <div id="PageAttributes">
        <h1>
            <asp:Label ID="TitleLabel" CssClass="Black" runat="server"></asp:Label></h1>
        <p>
            <asp:Label ID="instructionsLabel" runat="server"></asp:Label></p>
            
        <div class="PageTextLang1">
            <table cellspacing="0" class="defaultTable" summary="Page Filter">
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
        
        <div class="PageAttributes2">
            <asp:GridView ID="gvEditPageAttributes" runat="server" AutoGenerateColumns="False"
                DataKeyNames="ID" DataSourceID="SqlDataSource1" CssClass="defaultTable" OnRowCommand="gvEditPageAttributes_RowCommand"
                EmptyDataText="There are no attributes to edit">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                        SortExpression="ID" />
                    <asp:BoundField DataField="BUSINESS_UNIT" HeaderText="BUSINESS UNIT" ReadOnly="True"
                        SortExpression="BUSINESS_UNIT" />
                    <asp:BoundField DataField="PARTNER_CODE" HeaderText="PARTNER CODE" ReadOnly="True"
                        SortExpression="PARTNER_CODE" />
                    <asp:BoundField DataField="ATTR_NAME" HeaderText="ATTRIBUT NAME" ReadOnly="True"
                        SortExpression="ATTR_NAME" />
                    <asp:TemplateField HeaderText="ATTR_VALUE" SortExpression="ATTR_VALUE">
                        <ItemTemplate>
                            <asp:Label ID="lblTextContent" runat="server" Text='<%# Bind("ATTR_VALUE") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="button" CommandArgument='<%# Bind("ID") %>'
                                CommandName="Edit" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            
             <asp:PlaceHolder ID="plhTextEditor" runat="server" Visible="False">
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
                    SelectCommand="SELECT * FROM tbl_page_attribute
                    WHERE BUSINESS_UNIT = @BUSINESS_UNIT 
                    AND PARTNER_CODE = @PARTNER_CODE 
                    AND PAGE_CODE = @PAGE_CODE
                    AND ID = @ID">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                        <asp:QueryStringParameter Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                        <asp:QueryStringParameter Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                        <asp:ControlParameter ControlID="hdfRowID" PropertyName="Value" Name="ID" />
                    </SelectParameters>
                </asp:SqlDataSource>
                <asp:HiddenField ID="hdfRowID" runat="server" />
                <asp:TextBox ID="txtEditor" runat="server" Rows="30" Columns="80" TextMode="MultiLine"></asp:TextBox>
                <br />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="button" />
            </asp:PlaceHolder>                        
            
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
                SelectCommand="SELECT * FROM tbl_page_attribute WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE AND PAGE_CODE = @PAGE_CODE"
                UpdateCommand="UPDATE tbl_page_attribute SET ATTR_VALUE = @ATTR_VALUE WHERE ID = @ID">
                <SelectParameters>
                    <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                    <asp:QueryStringParameter Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                    <asp:QueryStringParameter Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="hdfRowID" PropertyName="Value" Name="ID" />
                    <asp:ControlParameter ControlID="txtEditor" PropertyName="Text" Name="ATTR_VALUE" />
                </UpdateParameters>
            </asp:SqlDataSource>
            
            <div class="pageHeadersAndTitlesButtons">
                <asp:Button ID="Add_Page_Header_and_Titlebtn" runat="server" CssClass="buttonPage" />
                <asp:Button ID="Define_HTML_Includesbtn" runat="server" CssClass="buttonPage" />
                <asp:Button ID="Return_To_Page_Detailbtn" runat="server" CssClass="buttonPage" />
                <asp:Button ID="Return_To_Pagesbtn" runat="server" CssClass="buttonPage" />
            </div>
            
        </div>
    </div>
</asp:Content>
 