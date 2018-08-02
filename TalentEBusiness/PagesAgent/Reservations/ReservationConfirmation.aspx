<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="ReservationConfirmation.aspx.vb" Inherits="PagesAgent_Reservations_ReservationConfirmation" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhConfirmationMessage" runat="server">
        <div class="alert-box info ebiz-confirmation-message">
            <div class="alert-box success ebiz-confirmation-details">
                <asp:Literal id="ltlConfirmationDetails" runat="server"></asp:Literal>
            </div>
            <asp:Literal id="ltlConfirmationMessage" runat="server"></asp:Literal> 
        </div>
    </asp:PlaceHolder>
    <div class="ebiz-next-sale">
        <asp:Button ID="btnNextSale" runat="server" CssClass="button"></asp:Button>
    </div>
</asp:Content>

