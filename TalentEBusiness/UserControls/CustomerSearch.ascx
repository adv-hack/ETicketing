<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CustomerSearch.ascx.vb" Inherits="UserControls_CustomerSearch" %>
<asp:Panel ID="pnlCustomerSearchForm" runat="server" DefaultButton="btnPerformCustomerSearch" CssClass="panel ebiz-customer-search">
    <h2 id="h2CustomerSearchFormHeader" runat="server"><asp:Literal ID="ltlCustomerSearchFormHeader" runat="server" /></h2>
    <div class="row">
        <asp:PlaceHolder ID="plhCustomerNumber" runat="server" Visible="false">
            <div class="column ebiz-customer-search-item ebiz-customer-number">
                <label for="<%=txtContactNumber.ClientID%>"><asp:Literal ID="ltlContactNumber" runat="server" /></label>
                <asp:TextBox ID="txtContactNumber" runat="server" ClientIDMode="Static" CssClass="customer-details" />
            </div>
        </asp:PlaceHolder>
        <div class="column ebiz-customer-search-item ebiz-first-name">
            <label for="<%=txtForename.ClientID %>"><asp:Literal ID="ltlForenameLabel" runat="server" /></label>
            <asp:TextBox ID="txtForename" runat="server" ClientIDMode="Static" CssClass="customer-details" />
        </div>
        <div class="column ebiz-customer-search-item ebiz-last-name">
            <label for="<%=txtSurname.ClientID %>"><asp:Literal ID="ltlSurnameLabel" runat="server" /></label>
            <asp:TextBox ID="txtSurname" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
        </div>
        <asp:PlaceHolder ID="plhPassportNumber" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-passport-number">
                <label for="<%=txtPassportNumber.ClientID%>"><asp:Literal ID="ltlPassportNumberLabel" runat="server" /></label>
                <asp:TextBox ID="txtPassportNumber" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine1Row" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-building-name">
                <label for="<%=txtAddressLine1.ClientID %>"><asp:Literal ID="ltlAddressLine1Label" runat="server" /></label>
                <asp:TextBox ID="txtAddressLine1" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine2Row" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-street">
                <label for="<%=txtAddressLine2.ClientID %>"><asp:Literal ID="ltlAddressLine2Label" runat="server" /></label>
                <asp:TextBox ID="txtAddressLine2" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine3Row" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-town">
                <label for="<%=txtAddressLine3.ClientID %>"><asp:Literal ID="ltlAddressLine3Label" runat="server" /></label>
                <asp:TextBox ID="txtAddressLine3" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressLine4Row" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-city">
                <label for="<%=txtAddressLine4.ClientID %>"><asp:Literal ID="ltlAddressLine4Label" runat="server" /></label>
                <asp:TextBox ID="txtAddressLine4" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhAddressPostCodeRow" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-postcode">
                <label for="<%=txtAddressPostCode.ClientID %>"><asp:Literal ID="ltlAddressPostCodeLabel" runat="server" /></label>
                <asp:TextBox ID="txtAddressPostCode" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
        <div class="column ebiz-customer-search-item ebiz-address-postcode">
            <label for="<%=txtEmail.ClientID %>"><asp:Literal ID="ltlEmailLabel" runat="server" /></label>
            <asp:TextBox ID="txtEmail" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
        </div>
        <asp:PlaceHolder ID="plhPhoneNumberRow" runat="server" Visible="True">
            <div class="column ebiz-customer-search-item ebiz-address-phonenumber"> 
                <label for="<%=txtPhoneNumber.ClientID %>" class="inline"><asp:Literal ID="ltlPhoneNumber" runat="server" /></label>   
                <asp:TextBox ID="txtPhoneNumber" runat="server" ClientIDMode="Static" CssClass="customer-details"/>
            </div>
        </asp:PlaceHolder>
    </div>
    <div class="button-group ebiz-button-group-icons ebiz-customer-search-actions-wrap">
        <asp:PlaceHolder ID="plhBackButton" runat="server" Visible="True">
           <asp:LinkButton ID="btnBack" runat="server" CssClass="has-tip button ebiz-muted-action" OnClick="btnBack_Click" data-tooltip aria-haspopup="true" data-disable-hover="false" data-hover-delay="600" data-v-offset="7"><i class="fa fa-arrow-left fa-fw" aria-hidden="true"></i></asp:LinkButton>
        </asp:PlaceHolder>
        <asp:LinkButton ID="btnPerformCustomerSearch" runat="server" CssClass="has-tip button ebiz-primary-action" OnClientClick="ProcessCustomerSearch(); return false;" data-tooltip aria-haspopup="true" data-disable-hover="false" data-hover-delay="600" data-v-offset="7"><i class="fa fa-search fa-fw" aria-hidden="true"></i></asp:LinkButton>
    </div>
    
    
</asp:Panel>