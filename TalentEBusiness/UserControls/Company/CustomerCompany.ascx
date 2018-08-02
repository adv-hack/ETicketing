<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CustomerCompany.ascx.vb" Inherits="UserControls_CustomerCompany" %>
<div class="panel ebiz-customer-company-wrap">
    <h2>
        <span><asp:Literal ID="ltlCompanyHeader" runat="server" /></span> <span><asp:Literal ID="ltlCompanyName" runat="server" /></span>
    </h2>
    <asp:Literal ID="ltlCompanyReference" runat="server" Visible="False"/>
    <div class="button-group ebiz-cutomer-company-actions-wrap">
        <asp:Hyperlink ID="hplChange" runat="server" Visible="false" CssClass="button" />
        <asp:Hyperlink ID="hplDetails" runat="server" Visible="false" CssClass="button" />
        <asp:Hyperlink ID="hplAdd" runat="server" NavigateUrl="~/PagesPublic/Profile/CustomerSelection.aspx?displayMode=ShowCompanySearch" Visible="false" CssClass="button" />
        <asp:Button ID="btnRemove" runat ="server"  CssClass="button" Visible="false"/>
    </div>
    <asp:HiddenField ID="hdfCustomerNumber" ClientIDMode="Static"  runat="server" />
    <asp:HiddenField ID="hdfCompanyID" ClientIDMode="Static"  runat="server" />
    <asp:HiddenField ID="hdfOldCompanyID" ClientIDMode="Static"  runat="server" />
</div>
