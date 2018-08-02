<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="PartnerPromotions.aspx.vb" Inherits="PagesPublic_ProductBrowse_PartnerPromotions" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <div class="alert-box alert">
            <asp:Literal ID="ltlErrorMessage" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:Literal ID="ltlPromotionSpecificContent" runat="server" />

    <asp:Repeater ID="rptPartnerPromotions" runat="server">
        <HeaderTemplate>
            <div class="ebiz-ticketing-products">
        </HeaderTemplate>
        <ItemTemplate>
            <asp:HyperLink ID="hplNavigateUrl" runat="server">
                <div class="panel <%# DataBinder.Eval(Container.DataItem,"ProductCode").Trim%> ebiz-product-type-<%# DataBinder.Eval(Container.DataItem,"ProductType").Trim%>">
                    <div class="ebiz-product-detail">
                        <div class="ebiz-logos-wrap">
                            <asp:Image ID="OppositionImage" runat="server" CssClass="ebiz-opposition" />
                            <asp:Image ID="CompetitionImage" runat="server" CssClass="ebiz-competition" />
                            <asp:PlaceHolder ID="plhSponsorImage" runat="server">
                                <span class="ebiz-sponsor">
                                    <asp:Label ID="SponsoredByText" runat="server" Text='<%# ProductDetail.GetSponsoredByText(Container.DataItem("ProductType"))%>' />
                                    <asp:Image ID="SponsorImage" runat="server" CssClass="ebiz-sponsor" />
                                </span>
                            </asp:PlaceHolder>
                        </div>
                        <ul class="no-bullet">
                            <asp:PlaceHolder ID="plhProductDescription" runat="server" Visible="False">
                                <li class="ebiz-description"><h2><asp:Literal ID="ltlProductDescription" runat="server" /></h2></li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhProductCompetition" runat="server" Visible="False">
                                <li class="ebiz-competition">
                                    <span class="ebiz-label"><asp:Literal ID="ltlProductCompetitionLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlProductCompetitionValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>                                
                            <asp:PlaceHolder ID="plhProductDate" runat="server" Visible="False">
                                <li class="ebiz-date">
                                    <span class="ebiz-label"><asp:Literal ID="ltlProductDateLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlProductDateValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhProductTime" runat="server" Visible="False">
                                <li class="ebiz-time">
                                    <span class="ebiz-label"><asp:Literal ID="ltlProductTimeLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlProductTimeValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhLoyaltyPoints" runat="server" Visible="False">
                                <li class="ebiz-loyalty">
                                    <span class="ebiz-label"><asp:Literal ID="ltlLoyaltyPointsLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlLoyaltyPointsValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAge" runat="server" Visible="False">
                                <li class="ebiz-age">
                                    <span class="ebiz-label"><asp:Literal ID="ltlAgeLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlAgeValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhDuration" runat="server" Visible="False">
                                <li class="ebiz-duration">
                                    <span class="ebiz-label"><asp:Literal ID="ltlDurationLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlDurationValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhLocation" runat="server" Visible="False">
                                <li class="ebiz-location">
                                    <span class="ebiz-label"><asp:Literal ID="ltlLocationLabel" runat="server" /></span>
                                    <span class="ebiz-data"><asp:Literal ID="ltlLocationValue" runat="server" /></span>
                                </li>
                            </asp:PlaceHolder>
                        </ul>
                    </div>
                    <asp:PlaceHolder ID="pnlExtendedText" runat="server">
                        <div class="alert-box info ebiz-extended-text">
                            <asp:Literal ID="ltlExtendedText1" runat="server" />
                            <asp:Literal ID="ltlExtendedText2" runat="server" />
                            <asp:Literal ID="ltlExtendedText3" runat="server" />
                            <asp:Literal ID="ltlExtendedText4" runat="server" />
                            <asp:Literal ID="ltlExtendedText5" runat="server" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </asp:HyperLink>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

</asp:Content>