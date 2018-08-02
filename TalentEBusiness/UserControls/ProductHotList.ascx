<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductHotList.ascx.vb" Inherits="UserControls_ProductHotList" %>
<asp:PlaceHolder ID="plhProductHotList" runat="server" Visible="false">
    <div class="ebiz-product-hot-list">
        <asp:Repeater ID="rptProductHotList" runat="server">
            <HeaderTemplate>
                <div class="owl-carousel owl-theme">
            </HeaderTemplate>
            <ItemTemplate>
                <div>
                    <asp:HyperLink ID="hplVisualSeatSelection" runat="server">
                        <asp:Image ID="OppositionImage" runat="server" ImageUrl='<%# GetImageURL("PRODTICKETING",Container.DataItem("ProductOppositionCode")) %>' CssClass="ebiz-opposition" />
                        <asp:Image ID="CompetitionImage" runat="server" ImageUrl='<%# GetImageURL("PRODCOMPETITION",Container.DataItem("ProductCompetitionCode")) %>' CssClass="ebiz-competition" />
                        <span class="ebiz-information"><span class="ebiz-description">
                            <%#DataBinder.Eval(Container.DataItem, "ProductDescription").ToString()%></span> <span class="ebiz-date">
                                <%# DataBinder.Eval(Container.DataItem, "ProductDate").ToString()%></span> <span class="ebiz-type">
                                    <%# GetProductTypeDescription(DataBinder.Eval(Container.DataItem, "ProductType").ToString())%></span> </span>
                    </asp:HyperLink>
                    <asp:HiddenField ID="hvfProductDate" Value='<%# DataBinder.Eval(Container.DataItem, "ProductDate").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfProductType" Value='<%# DataBinder.Eval(Container.DataItem, "ProductType").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfProductStadium" Value='<%#DataBinder.Eval(Container.DataItem, "ProductStadium").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfCampaignCode" Value='<%#DataBinder.Eval(Container.DataItem, "CampaignCode").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfProductCode" Value='<%#DataBinder.Eval(Container.DataItem, "ProductCode").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfProductHomeAsAway" Value='<%#DataBinder.Eval(Container.DataItem, "ProductHomeAsAway").ToString()%>' runat="server" Visible="false" />
                    <asp:HiddenField ID="hvfProductSubType" Value='<%#DataBinder.Eval(Container.DataItem, "ProductSubType").ToString()%>' runat="server" Visible="false" />
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>
