<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CompanySearch.ascx.vb" Inherits="UserControls_CompanySearch" %>
<asp:Panel ID="pnlCompanySearch" runat="server" DefaultButton="btnPerformCompanySearch" cssclass="panel ebiz-company-search">
    <h2 id="h2CompanySearchFormHeader" runat="server"><asp:Literal ID="ltlCompanySearchFormHeader" runat="server" /> <span><asp:Literal ID="ltlParentCompanyName" runat="server" /></span></h2>
    <div class="alert-box warning" id="customer-search-warning"><span id="customer-search-warning-message"></span></div>
    <div class="row">   
        <div class="column ebiz-company-search-item ebiz-company-name">
            <asp:Label ID="ltlCompanyName" runat="server" AssociatedControlID="txtCompanyName" />
            <asp:TextBox ID="txtCompanyName" runat="server" ClientIDMode="Static" CssClass="company-details"/>
        </div>
        <div class="column ebiz-company-search-item ebiz-company-name">
            <asp:Label ID="ltlAddressLine1" runat="server" AssociatedControlID="txtCompanyAddressLine1" />
            <asp:TextBox ID="txtCompanyAddressLine1" runat="server" ClientIDMode="Static" CssClass="company-details"/>
        </div>
        <div class="column ebiz-company-search-item ebiz-company-address-line1">
            <asp:Label ID="ltlPostCode" runat="server" AssociatedControlID="txtCompanyPostCode" />
            <asp:TextBox ID="txtCompanyPostCode" runat="server" ClientIDMode="Static" CssClass="company-details"/>
        </div>
        <div class="column ebiz-company-search-item ebiz-company-web-address">
            <asp:Label ID="ltlWebAddress" runat="server" AssociatedControlID="txtCompanyWebAddress" />
            <asp:TextBox ID="txtCompanyWebAddress" runat="server" ClientIDMode="Static" CssClass="company-details"/>
        </div>
        <div class="column ebiz-company-search-item ebiz-company-telephone-number">
            <asp:Label ID="ltlTelephoneNumber" runat="server" AssociatedControlID="txtCompanyTelephoneNumber" />
            <asp:TextBox ID="txtCompanyTelephoneNumber" runat="server" ClientIDMode="Static" CssClass="company-details" />
        </div>
    </div>
    <div class="button-group ebiz-button-group-icons ebiz-company-search-actions-wrap">
        <asp:Hyperlink ID="hplBack" runat="server" CssClass="has-tip button ebiz-muted-action ebiz-back" Visible="false" data-tooltip aria-haspopup="true" data-disable-hover="false" data-hover-delay="600" data-v-offset="7"><i class="fa fa-arrow-left fa-fw" aria-hidden="true"></i></asp:Hyperlink>  
        <asp:LinkButton ID="btnPerformCompanySearch" runat="server" CssClass="has-tip button ebiz-primary-action" OnClientClick="processCompanySearch(); return false;" data-tooltip aria-haspopup="true" data-disable-hover="false" data-hover-delay="600" data-v-offset="7"><i class="fa fa-search fa-fw" aria-hidden="true"></i></asp:LinkButton> 
        <asp:Hyperlink ID="hplAdd" runat="server" Visible="true" CssClass="has-tip button ebiz-primary-action ebiz-add" data-tooltip aria-haspopup="true" data-disable-hover="false" data-hover-delay="600" data-v-offset="7"><i class="fa fa-plus fa-fw" aria-hidden="true"></i></asp:Hyperlink>
    </div>
</asp:Panel>

