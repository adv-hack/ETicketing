<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OrderHeader.aspx.vb" Inherits="Orders_OrderHeader" MasterPageFile="~/MasterPages/MasterPage.master" %>
<%@ Register Src="../UserControls/OrderEnquiry.ascx" TagName="OrderEnquiry" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="server">
    <uc1:OrderEnquiry id="OrderEnquiry1" runat="server" />
</asp:Content>