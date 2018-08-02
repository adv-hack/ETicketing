<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromotionDetails.aspx.vb" Inherits="PagesPublic_Basket_PromotionDetails" MasterPageFile="~/MasterPages/Shared/Blank.master" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhPromotionNotFound" runat="server" Visible="false">
        <div class="alert-box warning ebiz-promotion-not-found">
            <asp:Literal ID="ltlPromotionNotFound" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhTicketingPromotionDetails" runat="server">
        <asp:Repeater ID="rptTicketingPromotionDetails" runat="server">
            <ItemTemplate>
                <div class="panel ebiz-promotional-details-wrap">
                    <div class="row ebiz-promotion-description1">
                        <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionDescription1Label" runat="server" /></div>
                        <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionDescription1Value" runat="server" /></div>
                    </div>
                    <div class="row ebiz-promotion-description2">
                        <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionDescription2Label" runat="server" /></div>
                        <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionDescription2Value" runat="server" /></div>
                    </div>
                    <div class="row ebiz-promotion-start-date">
                        <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionStartDateLabel" runat="server" /></div>
                        <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionStartDateValue" runat="server" /></div>
                    </div>
                    <div class="row ebiz-promotion-end-date">
                        <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionEndDateLabel" runat="server" /></div>
                        <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionEndDateValue" runat="server" /></div>
                    </div>
                    <asp:PlaceHolder ID="plhTicketingFeeInformation" runat="server">
                        <div class="alert-box info ebiz-ticketing-fee-information">
                            <asp:Literal ID="ltlTicketingFeeRemoved" runat="server" />
                        </div>
                    </asp:PlaceHolder>
                </div>  
            </ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="plhTicketingPriceInformation" runat="server">
            <div class="panel ebiz-promotional-prices-wrap">
                <div class="row ebiz-promotion-original-price">
                    <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionOriginalPriceLabel" runat="server" /></div>
                    <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionOriginalPriceValue" runat="server" /></div>
                </div>
                <div class="row ebiz-promotion-new-price">
                    <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionNewPriceLabel" runat="server" /></div>
                    <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionNewPriceValue" runat="server" /></div>
                </div>
                <div class="row ebiz-promotion-discount-price">
                    <div class="medium-3 columns"><asp:Literal ID="ltlTicketingPromotionDiscountPriceLabel" runat="server" /></div>
                    <div class="medium-9 columns"><asp:Literal ID="ltlTicketingPromotionDiscountPriceValue" runat="server" /></div>
                </div>
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhMerchandisePromotionDetails" runat="server">
        <div class="alert-box info ebiz-merchandise-promotions">
            <asp:BulletedList ID="blMerchandisePromotions" runat="server" />
        </div>
    </asp:PlaceHolder>  

    <button class="close-button" data-close aria-label="Close modal" type="button">
        <span aria-hidden="true"><i class="fa fa-times"></i></span>
    </button>

</asp:Content>