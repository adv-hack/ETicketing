<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FavouriteSeat.aspx.vb" Inherits="PagesLogin_Profile_FavouriteSeat" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    
    <asp:PlaceHolder ID="plhFavouriteSeatDetails" runat="server" Visible="false">
        <div class="panel favourite-seat-details-wrap">
            <asp:PlaceHolder ID="plhFavouriteSeatIntro" runat="server" Visible="false">
                <h2><asp:Literal ID="ltlFavouriteSeatIntro" runat="server" /></h2>
            </asp:PlaceHolder>
            <div class="row small-up-1 medium-up-4">
                <div class="column ebiz-amendpps-stand">
                    <span class="ebiz-label"><asp:Literal ID="ltlFavouriteStandLabel" runat="server" /></span>
                    <span class="ebiz-data"><asp:Literal ID="ltlFavouriteStandValue" runat="server" /></span>
                </div>
                <div class="column ebiz-amendpps-area">
                    <span class="ebiz-label"><asp:Literal ID="ltlFavouriteAreaLabel" runat="server" /></span>
                    <span class="ebiz-data"><asp:Literal ID="ltlFavouriteAreaValue" runat="server" /></span>
                </div>
                <div class="column ebiz-amendpps-row">
                    <span class="ebiz-label"><asp:Literal ID="ltlFavouriteRowLabel" runat="server" /></span>
                    <span class="ebiz-data"><asp:Literal ID="ltlFavouriteRowValue" runat="server" /></span>
                </div>
                <div class="column ebiz-amendpps-seat">
                    <span class="ebiz-label"><asp:Literal ID="ltlFavouriteSeatLabel" runat="server" /></span>
                    <span class="ebiz-data"><asp:Literal ID="ltlFavouriteSeatValue" runat="server" /></span>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="plhNoFavouriteSeatDetails" runat="server">
        <div class="alert-box warning">
            <asp:Literal ID="ltlNoFavouriteSeatDetailsMessage" runat="server" />
        </div>
    </asp:PlaceHolder>
    
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>