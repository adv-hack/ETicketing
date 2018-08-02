<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CustomerText.aspx.vb" Inherits="PagesAgent_Profile_CustomerText" validateRequest="false" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:PlaceHolder ID="plhCustomerText" runat="server">
    <div class="panel ebiz-customer-textbox">
        <h2>
            <asp:Literal ID="ltlCustomerTextBoxHeading" runat="server" /></h2>
        <asp:TextBox ID="txtCustomerText" runat="server" TextMode="Multiline" rows="16" />
        <div class="ebiz-upate-cutomer-text-wrap">
        	<asp:Button ID="updateBtn" runat="server" CausesValidation="true"  CssClass="button ebiz-primary-action" />
    	</div>
    </div> 
    </asp:PlaceHolder>
</asp:Content>

