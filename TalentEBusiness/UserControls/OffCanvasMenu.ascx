<%@ Control Language="VB" AutoEventWireup="false" CodeFile="OffCanvasMenu.ascx.vb" Inherits="UserControls_OffCanvasMenu" %>
<div class="panel ebiz-off-canvas-fan-search">
    <asp:Panel runat="server" ID="pnlFanCard" DefaultButton="btnSubmit">
        <div class="ebiz-fan-card">
            <asp:Label ID="fancardLabel" runat="server" AssociatedControlID="fancardBox" />
            <asp:TextBox ID="fancardBox" runat="server" MaxLength="12" Type="number" />
        </div>
        <asp:PlaceHolder ID="plhMembershipNumber" runat="server">
            <div class="ebiz-membership-number">
                <asp:Label ID="lblMembershipNumber" runat="server" AssociatedControlID="txtMembershipNumber" />
                <asp:TextBox ID="txtMembershipNumber" runat="server" />
            </div>
        </asp:PlaceHolder>
        <div class="ebiz-fan-search-wrap">
            <asp:Button ID="btnSubmit" runat="server" CssClass="button ebiz-primary-action" />
        </div>
        <asp:PlaceHolder ID="plhLastNCustomerLogins" runat="server">
        <asp:Label ID="lblLastNCustomersUsed" CssClass="ebiz-customers-used" runat="server" />	
    	    <asp:Repeater ID="rptLastNCustomerLogins" runat="server">
    		    <HeaderTemplate>
    			    <ul class="no-bullet ebiz-customer-logins-list">
    		    </HeaderTemplate>
    		    <ItemTemplate>
    			    <li><asp:HyperLink ID="hplLastCustomerLogin" runat="server" /></li>
    		    </ItemTemplate>
    		    <FooterTemplate>
    			    </ul>
    		    </FooterTemplate>
    	    </asp:Repeater>
        </asp:PlaceHolder>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlCustomerSearch" DefaultButton="btnPerformCustomerSearch">
        <h2><asp:Literal ID="ltlCustomerSearchFormHeader" runat="server" /></h2>
        <div class="ebiz-forename">
            <label for="<%=txtForename.ClientID %>"><asp:Literal ID="ltlForenameLabel" runat="server" /></label>
            <asp:TextBox ID="txtForename" runat="server" />
        </div>
        <div class="ebiz-surname">
            <label for="<%=txtSurname.ClientID %>"><asp:Literal ID="ltlSurnameLabel" runat="server" /></label>
            <asp:TextBox ID="txtSurname" runat="server" />
        </div>
        <div class="ebiz-address-building-name">
            <label for="<%=txtAddressLine1.ClientID %>"><asp:Literal ID="ltlAddressLine1Label" runat="server" /></label>
            <asp:TextBox ID="txtAddressLine1" runat="server" />
        </div>
        <div class="ebiz-address-street">
            <label for="<%=txtAddressLine2.ClientID %>"><asp:Literal ID="ltlAddressLine2Label" runat="server" /></label>
            <asp:TextBox ID="txtAddressLine2" runat="server" />
        </div>
        <div class="ebiz-address-town">
            <label for="<%=txtAddressLine3.ClientID %>"><asp:Literal ID="ltlAddressLine3Label" runat="server" /></label>
            <asp:TextBox ID="txtAddressLine3" runat="server" />
        </div>
        <div class="ebiz-address-city">
            <label for="<%=txtAddressLine4.ClientID %>"><asp:Literal ID="ltlAddressLine4Label" runat="server" /></label>
            <asp:TextBox ID="txtAddressLine4" runat="server" />
        </div>
        <div class="ebiz-address-postcode">
            <label for="<%=txtAddressPostCode.ClientID %>"><asp:Literal ID="ltlAddressPostCodeLabel" runat="server" /></label>
            <asp:TextBox ID="txtAddressPostCode" runat="server" />
        </div>
        <div class="row ebiz-address-email">
            <label for="<%=txtEmail.ClientID %>"><asp:Literal ID="ltlEmailLabel" runat="server" /></label>
            <asp:TextBox ID="txtEmail" runat="server" />
        </div>
        <asp:Button ID="btnPerformCustomerSearch" runat="server" CssClass="button ebiz-primary-action" />
    </asp:Panel>
</div>
