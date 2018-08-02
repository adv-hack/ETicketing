<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MarketingCampaigns.ascx.vb" Inherits="UserControls_MarketingCampaigns" ViewStateMode="Disabled" %>

<asp:PlaceHolder ID="plhDisplayMarketingCampaigns" runat="server" Visible="false">
    <asp:PlaceHolder ID="plhMarketingCampaignsTopText" runat="server">
        <p><asp:Literal ID="ltlMarketingCampaignsTopText" runat="server" /></p>
    </asp:PlaceHolder>

    <div class="row">
        <div class="large-3 columns">
            <asp:Label ID="lblMarketingCampaignsSideText" runat="server" AssociatedControlID="MarketingCampaignsDDL" CssClass="middle" />
        </div>
        <div class="large-9 columns">
            <asp:DropDownList ID="MarketingCampaignsDDL" runat="server" CssClass="marketing-campaigns select" ViewStateMode="Enabled" />
            <asp:RequiredFieldValidator ID="rfvMarketingCampaign" runat="server" ControlToValidate="MarketingCampaignsDDL" ValidationGroup="Checkout"
                InitialValue="" Display="Static" CssClass="error" SetFocusOnError="true" />
        </div>
    </div>
    <input type="button" class="button" value='<%=btnContinueText %>' onclick="OpenNextItem(1);" />
</asp:PlaceHolder>