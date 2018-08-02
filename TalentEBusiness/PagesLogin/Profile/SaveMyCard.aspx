<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SaveMyCard.aspx.vb" Inherits="PagesLogin_Profile_SaveMyCard" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentDetails.ascx" TagName="PaymentDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/MySavedCards.ascx" TagName="MySavedCards" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentCardDetails.ascx" TagName="PaymentCardDetails" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" Visible="false">
        <p class="alert-box success"><asp:Literal ID="ltlSuccessMessage" runat="server" /></p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
        <p class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessageList" runat="server" Visible="false">
        <asp:BulletedList ID="blErrorMessageList" runat="server" CssClass="alert-box alert" />
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="vlsCheckoutErrors" runat="server" ValidationGroup="Checkout" ShowSummary="true" CssClass="alert-box alert ebiz-checkout-eErrors" />
    <div id="vlsPaymentCardDetailsErrors" class="alert-box alert" style="display:none;">
            <ul id="PaymentCardDetailsErrors">
            </ul>
	    </div>

    <Talent:MySavedCards ID="savedCardsList" runat="server" ShowDeleteButton="True" ShowDefaultButton="True" />

    <asp:PlaceHolder ID="plhAddACard" runat="server">
    	<div class="panel">
	        <asp:PlaceHolder ID="plhAddACardIntroText" runat="server">
	            <p class="ebiz-add-a-card-intro-text"><asp:Literal ID="ltlAddACardIntroText" runat="server" /></p>
	        </asp:PlaceHolder>
	        <asp:Placeholder id="plhCardDetail" runat="server">
	            <Talent:PaymentDetails ID="payDetails" runat="server" />
	            <asp:Button ID="btnSaveThisCard" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" />
	        </asp:Placeholder>
	        <asp:Placeholder id="plhCardDetailToVG" runat="server">
	            <Talent:PaymentCardDetails ID="PaymentCardDetails1" runat="server" Usage="SAVEMYCARD"/>
	            <asp:Button ID="btnSaveThisCardVG" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="Checkout" OnClientClick="submitToVanguard(); return false;" />
	        </asp:Placeholder>
	    </div>
    </asp:PlaceHolder>
</asp:content>
