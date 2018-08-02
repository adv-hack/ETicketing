<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CheckoutOrderConfirmation.aspx.vb" Inherits="PagesLogin_checkoutOrderConfirmation"%>
<%@ Reference Control="~/UserControls/PaymentDetails.ascx"  %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/BasketDetails.ascx" TagName="BasketDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebitSummary.ascx" TagName="DirectDebitSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CreditFinanceSummary.ascx"  TagName="CreditFinanceSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/SmartcardOrderDetails.ascx" TagName="SmartcardOrderDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductAssociations.ascx" TagName="ProductAssociations" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/NotificationOptions.ascx" TagName="NotificationOptions" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DespatchProcess.ascx" TagName="DespatchProcess" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagPrefix="Talent" TagName="CustomerProgressBar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    
    <asp:PlaceHolder ID="plhConfirmationMessage" runat="server">
        <div class="alert-box success ebiz-confirmation-message-wrap">
            <div class="ebiz-confirmation-details">
                <asp:Literal ID="ltlConfirmationDetails" runat="server" />
                <asp:Literal ID="PayPalReferenceLabel" runat="server" Visible="false" />
            </div>
            <Talent:NotificationOptions ID="NotificationOptions" runat="server" />
            <div class="ebiz-confirmation-message">
                <asp:Literal id="ltlConfirmationMessage" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" visible="False">
       <div class="alert-box success">
           <asp:BulletedList ID="blSuccessMessages" runat="server" />
       </div>
        <div class="ebiz-next-sale">
            <asp:Button ID="btnNextSale" runat="server" CssClass="button ebiz-primary-action"></asp:Button>
        </div>
    </asp:PlaceHolder>
    
    <asp:Panel ID="pnlConfirmOrder" runat="server" DefaultButton="btnConfirm">
        <Talent:BasketDetails ID="Confirmation_BasketDetails" runat="server" Usage="ORDER" DisplaySummaryTotals="false" />
        <Talent:DirectDebitSummary ID="DirectDebitSummary1" runat="server" />
        <Talent:CreditFinanceSummary ID="CreditFinanceSummary1" runat="server" />
        <Talent:SmartcardOrderDetails ID="SmartcardOrderDetails" runat="server" />
        <Talent:DespatchProcess ID="uscDespatchProcess" runat="server" Usage="ORDERCONFIRMATION" />
    
        <asp:PlaceHolder ID="plhConfirm" runat="server" Visible="false">
            <div class="ebiz-confirm text-right">
                <asp:Button ID="btnConfirm" runat="server" CssClass="button ebiz-primary-action"></asp:Button>
            </div>
        </asp:PlaceHolder>
    </asp:Panel>
    
    <Talent:ProductAssociations ID="uscProductAssociations" runat="server" PagePosition="2" />
 
    <script type="text/javascript">
        //-- Return the payload in the post-order html response page
     
        var discountIFValue = '<%= discountIF %>';
        if (discountIFValue == 'True') {
            var cashbackif_payload = {
                order: '<%= DiscountIFValues(0) %>',
                amount: '<%= DiscountIFValues(1) %>',
                sign: '<%= DiscountIFValues(2) %>'
            };
        }
    </script>
</asp:Content>

