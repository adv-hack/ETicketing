<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Basket.aspx.vb" Inherits="PagesPublic_Basket" %>
<%@ Register Src="~/UserControls/BasketDetails.ascx" TagName="BasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketButtons.ascx" TagName="BasketButtons" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductAssociations.ascx" TagName="ProductAssociations" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ReserveTickets.ascx" TagPrefix="Talent" TagName="ReserveTickets" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagPrefix="Talent" TagName="CustomerProgressBar" %>
<%@ Register Src="~/UserControls/UnreserveTickets.ascx" TagPrefix="Talent" TagName="UnreserveTickets" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="Basket_HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>
    <asp:Label ID="BasketAlreadyPaidLabel" Visible="false" runat="server" CssClass="alert-box alert" />
    <Talent:BasketDetails ID="BasketDetails1" runat="server" Usage="BASKET" DisplaySummaryTotals="false" />
    <Talent:BasketButtons id="BasketButtons1" runat="server"></Talent:BasketButtons>
    <Talent:ProductAssociations ID="uscProductAssociations" runat="server" PagePosition="2" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
    <Talent:ReserveTickets runat="server" id="ReserveTickets" Visible="False" />
    <Talent:UnreserveTickets runat="server" id="UnreserveTickets" Visible="False" />
    <asp:hiddenfield ID="hdfNoneReservationErrorCount" runat="server"></asp:hiddenfield>
    <asp:Hiddenfield ID ="hdfSearchNewCustomer" runat="server" ClientIDMode="Static"></asp:Hiddenfield>
    <asp:Hiddenfield ID ="hdfNewCustomerNumber" runat="server" ClientIDMode="Static"></asp:Hiddenfield>
    <asp:Hiddenfield ID ="hdfSelectedBasketDetailId" runat="server" ClientIDMode="Static"></asp:Hiddenfield>
</asp:Content>