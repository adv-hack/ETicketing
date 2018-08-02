<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromotionHistoryDetail.aspx.vb" Inherits="PagesLogin_Profile_PromotionHistoryDetail" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <div class="panel promotion-history-details-wrap">
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />
        <div class="row">
            <div class="columns ebiz-product-type">
                <span class="ebiz-label"><asp:Literal ID="ltlProductTypeLabel" runat="server" /></span>
                <span class="ebiz-value"><asp:Literal ID="ltlProductTypeValue" runat="server" /></span>
            </div>
            <div class="columns ebiz-priority">
                <span class="ebiz-label"><asp:Literal ID="ltlPriorityLabel" runat="server" /></span>
                <span class="ebiz-value"><asp:Literal ID="ltlPriorityValue" runat="server" /></span>
            </div>
            <div class="columns ebiz-price-code">
                <span class="ebiz-label"><asp:Literal ID="ltlPriceCodeLabel" runat="server" /></span>
                <span class="ebiz-value"><asp:Literal ID="ltlPriceCodeValue" runat="server" /></span>
            </div>
            <div class="columns ebiz-price-band">
                <span class="ebiz-label"><asp:Literal ID="ltlPriceBandLabel" runat="server" /></span>
                <span class="ebiz-value"><asp:Literal ID="ltlPriceBandValue" runat="server" /></span>
            </div>
        </div>

        <asp:Repeater ID="rptPromotionHistoryDetail" runat="server">
            <HeaderTemplate>
                <table class="ebiz-responsive-table">
                    <thead>
                        <tr>
                            <th scope="col" class="product"><%=ProductTypeHeaderText%></th>
                            <th scope="col" class="description"><%=DescriptionHeaderText%></th>
                            <th scope="col" class="seat"><%=SeatHeaderText%></th>
                            <th scope="col" class="price-code"><%=PriceCodeHeaderText%></th>
                            <th scope="col" class="price-band"><%=PriceBandHeaderText%></th>
                            <th scope="col" class="sold-date"><%=SoldDateHeaderText%></th>
                        </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="product"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ProductType"))%></td>
                    <td class="description"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ProductDescription"))%></td>
                    <td class="seat"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "seat"))%></td>
                    <td class="price-code"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "pricecode"))%></td>
                    <td class="price-band"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "PriceBand"))%></td>
                    <td class="sold-date"><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "SoldDate"))%></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        <div class="ebiz-back-button-wrap">
            <asp:Button ID="btnBack" runat="server" CssClass="button" />
        </div>
    </div>
</asp:Content>
