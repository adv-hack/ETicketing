<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketingPrePayments.aspx.vb" Inherits="PagesPublic_ProductBrowse_TicketingPrePayments" MaintainScrollPositionOnPostback="true"  %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/TicketingBasketDetails.ascx" TagName="TicketingBasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductDetail.ascx" TagName="ProductDetail" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/TicketingPPS.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    
    <asp:BulletedList ID="ErrorList" runat="server" CssClass="alert-box alert" />
    <asp:PlaceHolder ID="plhInstructionsText" runat="server">
        <p><asp:Literal ID="InstructionText" runat="server" /></p>
    </asp:PlaceHolder>

    <Talent:TicketingBasketDetails ID="TicketingBasketDetails1" runat="server" Usage="ORDER" SelectProductType="S" />
    <Talent:ProductDetail ID="ProductDetail1" runat="server" ProductType="P" />

    <asp:PlaceHolder ID="plhRegisteredPost" runat="server">
        <div class="ebiz-registered-post">
            <asp:CheckBox ID="RegisteredPost" runat="server" />
        </div>
    </asp:PlaceHolder>
    <div class="ebiz-ticketing-pre-payments-next-wrap">
        <asp:Button ID="ContinueButton" runat="server" CssClass="button ebiz-primary-action" />
    </div>
</asp:Content>