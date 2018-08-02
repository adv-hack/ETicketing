<%@ Page Language="VB" ValidateRequest="false" MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master" Inherits="PagesMaint_PageTextLang" AutoEventWireup="false" CodeFile="PageTextLang.aspx.vb"  Title="PageTextLang"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <!-- tinyMCE -->

    <script language="javascript" type="text/javascript" src="mce/jscripts/tiny_mce/tiny_mce.js"></script>

    <script language="javascript" type="text/javascript">
	// Notice: The simple theme does not use all options some of them are limited to the advanced theme
	tinyMCE.init({
		theme : "advanced",
		mode : "textareas",
		plugins : "preview",
		theme_advanced_buttons3_add : "preview"
	});
    </script>

    <!-- /tinyMCE -->
    
    <div id="PageTextLang">
        <h1><asp:Label ID="TitleLabel" CssClass="Black" runat="server" ></asp:Label></h1>
        <p><asp:Label id="instructionsLabel" runat="server" ></asp:Label></p>
        <div class="PageTextLang1">
            <table cellspacing="0" class="defaultTable" summary="Page Filter">
              <tbody>
                <tr>
                  <th><asp:Label ID="BusinessUnitlbl" runat="server" /></th>
                  <td><asp:Label ID="BusinessUnitlbl1" runat="server" /> </td>
               </tr>
               <tr>
                  <th><asp:Label ID="Partnerlbl" runat="server" /></th>
                  <td><asp:Label ID="Partnerlbl1" runat="server" /></td>
               </tr>
               <tr>
                  <th><asp:Label ID="PageNamelbl" runat="server" /></th>
                  <td><asp:Label ID="PageNamelbl1" runat="server" /> </td>
               </tr>
               <tr>
                  <th> <asp:Label ID="Descriptionlbl" runat="server" /></th>
                  <td><asp:Label ID="Descriptionlbl1" runat="server" /></td>
               </tr>
              </tbody>
            </table>
        </div>
        <div class="PageTextLang2">    
            <asp:GridView 
                ID="gvEditPageTextLang" 
                runat="server" 
                AutoGenerateColumns="False"
                DataKeyNames="ID" 
                DataSourceID="SqlDataSource1" 
                CssClass="defaultTable" 
                OnRowCommand="gvEditPageTextLang_RowCommand"
                EmptyDataText="There is no language specific text to edit">
                <Columns>
                    <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True" SortExpression="ID" />
                    <asp:BoundField DataField="BUSINESS_UNIT" HeaderText="BUSINESS UNIT" ReadOnly="True" SortExpression="BUSINESS_UNIT" />
                    <asp:BoundField DataField="PARTNER_CODE" HeaderText="PARTNER CODE" ReadOnly="True" SortExpression="PARTNER_CODE" />
                    <asp:BoundField DataField="PAGE_CODE" HeaderText="PAGE CODE" ReadOnly="True" SortExpression="PAGE_CODE" />
                    <asp:BoundField DataField="TEXT_CODE" HeaderText="TEXT CODE" ReadOnly="True" SortExpression="TEXT_CODE" />
                    <asp:BoundField DataField="LANGUAGE_CODE" HeaderText="LANGUAGE CODE" ReadOnly="True" SortExpression="LANGUAGE_CODE" />
                    <asp:TemplateField HeaderText="TEXT CONTENT" SortExpression="TEXT_CONTENT">
                        <%--
                        <EditItemTemplate>
                            <asp:TextBox ID="TextBox1"  runat="server" name="elm1" Rows="30" Columns="80" Text='<%# Bind("TEXT_CONTENT") %>' TextMode="MultiLine"></asp:TextBox>
                        </EditItemTemplate>
                        --%>
                        <ItemTemplate>
                            <asp:Label ID="lblTextContent" runat="server" Text='<%# Bind("TEXT_CONTENT") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="button" CommandArgument='<%# Bind("ID") %>' CommandName="Edit" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            
            <asp:PlaceHolder ID="plhHtmlEditor" runat="server" Visible="False">
                <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
                    SelectCommand="SELECT * FROM tbl_page_text_lang 
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
                <asp:TextBox ID="txtHtmlEditor" runat="server" name="elm1" Rows="30" Columns="80" TextMode="MultiLine"></asp:TextBox>
                <br />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="button" />&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="button" />
            </asp:PlaceHolder>            
            
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
                SelectCommand="SELECT * FROM tbl_page_text_lang WHERE BUSINESS_UNIT = @BUSINESS_UNIT AND PARTNER_CODE = @PARTNER_CODE AND PAGE_CODE = @PAGE_CODE"
                UpdateCommand="UPDATE tbl_page_text_lang SET TEXT_CONTENT = @TEXT_CONTENT WHERE ID = @ID">
                <SelectParameters>
                    <asp:QueryStringParameter Name="BUSINESS_UNIT" QueryStringField="BusinessUnit" Type="String" />
                    <asp:QueryStringParameter Name="PARTNER_CODE" QueryStringField="Partner" Type="String" />
                    <asp:QueryStringParameter Name="PAGE_CODE" QueryStringField="PageName" Type="String" />
                </SelectParameters>
                <UpdateParameters>
                    <asp:ControlParameter ControlID="hdfRowID" PropertyName="Value" Name="ID" />
                    <asp:ControlParameter ControlID="txtHtmlEditor" PropertyName="Text" Name="TEXT_CONTENT" />
                </UpdateParameters>
            </asp:SqlDataSource>
            
            <div class="pageHeadersAndTitlesButtons">
                <asp:Button ID="Add_Page_Header_and_Titlebtn" runat="server"  CssClass="buttonPage"/>
                <asp:Button ID="Define_HTML_Includesbtn" runat="server"  CssClass="buttonPage"/>
                <asp:Button ID="Return_To_Page_Detailbtn" runat="server"   CssClass="buttonPage"/>
                <asp:Button ID="Return_To_Pagesbtn" runat="server"   CssClass="buttonPage"/>    
            </div>
        </div>
    </div>
    
</asp:Content>