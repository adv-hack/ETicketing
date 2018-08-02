<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TicketExchangeSelection.aspx.vb" Inherits="PagesLogin_Orders_TicketExchangeSelection" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagName="CustomerProgressBar" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/TicketExchangeSeatList.ascx" TagName="TicketExchangeSeatList" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Import Namespace="Talent.Common" %>

<asp:content id="cphMain" contentplaceholderid="ContentPlaceHolder1" runat="Server"> 
    <Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>

    <asp:PlaceHolder ID="plhErrorList" runat="server" visible="False">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorList" runat="server" ClientIDMode="Static" />
        </div>
    </asp:PlaceHolder>

    <div class="alert-box alert" id="clientside-errors-wrapper" style="display:none">
        <asp:CustomValidator ID="cvCheckBoxes" runat="server" ErrorMessage="Required" ClientValidationFunction = "validateStatusCheckbox" ValidationGroup="TicketExcahangeValidation"></asp:CustomValidator>
        <ul id="clientside-errors"></ul>
    </div>

    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" visible="False">
        <div class="alert-box success">
            <asp:Literal ID="ltlSuccessMessage" runat="server" />
        </div>
    </asp:PlaceHolder>
    
    <div class="ebiz-ticket-exchange-summary-wrap">
        <h2><asp:Literal ID="ltlTicketingExchangeSelection" runat="server" /></h2>
    </div>
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="1" Sequence="1" />

    <Talent:TicketExchangeSeatList ID="TicketExchangeSelection" runat="server" ViewStateMode="Enabled" Visible="False" ListType="Selection" />
    <Talent:TicketExchangeSeatList ID="TicketExchangePriceChange" runat="server" ViewStateMode="Enabled" Visible="False" ListType="PriceChange" />
    <Talent:TicketExchangeSeatList ID="TicketExchangePlaceSeatsOnTE" runat="server" ViewStateMode="Enabled" Visible="False" ListType="PlaceOnSale" />
    <Talent:TicketExchangeSeatList ID="TicketExchangeTakeSeatsOffTE" runat="server" ViewStateMode="Enabled" Visible="False" ListType="TakeOffSale" />
    
    <asp:PlaceHolder ID="plhButtonGroup" runat="server">
        <div class="ebiz-ticket-exchange-buttons button-group">
            <asp:Button ID="btnCancel" runat="server" CssClass="button"/>
            <asp:Button ID="btnReset" runat="server" CssClass="button"/>
            <asp:Button ID="btnPrevious" runat="server" CssClass="button"/>
            <asp:Button ID="btnNext" runat="server" CssClass="button ebiz-primary-action"/>
            <asp:Button ID="btnConfirm" runat="server" CssClass="button ebiz-primary-action"/>
            <asp:Button ID="btnFinished" runat="server" CssClass="button ebiz-primary-action"/>
        </div>
    </asp:PlaceHolder>
</asp:content>
