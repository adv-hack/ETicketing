<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MatchDayHospitalitySummary.ascx.vb"
    Inherits="UserControls_MatchDayHospitalitySummary" %>
<div class="MDHSummary">
    <asp:PlaceHolder ID="plhMDHSummaryError" runat="server">
        <div class="MDHSummaryError">
            <asp:Label ID="lblMDHSummaryError" runat="server" CssClass="MDHSummary-Error"></asp:Label>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhMDHSummary" runat="server">
        <asp:PlaceHolder ID="plhMDHSummaryImage" runat="server">
            <div class="MDHSummaryImage">
                <asp:Image ID="imgMDHSummary" runat="server" CssClass="MDHSummary-Image" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhMDHSummaryHtml" runat="server">
            <div class="MDHSummaryHtml">
                <asp:Literal ID="ltlMDHSummaryHtml" runat="server" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhPackage" runat="server">
            <div class="package">
                <div class="packageHeader">
                    <asp:Literal ID="ltlPackageHeader" runat="server" />
                </div>
                <div class="packageDetail">
                    <span class="packageDescription"><asp:Literal ID="ltlPackageDescription" runat="server" /></span>
                    <span class="packageComments"><asp:Literal ID="ltlPackageComments" runat="server" /></span>
                </div>
            </div>
            <div class="component">
                <div class="componentHeader">
                    <asp:Literal ID="ltlComponentHeader" runat="server" />
                </div>
                <div class="componentDetail">
                    <asp:Repeater ID="rptComponents" runat="server">
                        <HeaderTemplate>
                            <ul>
                        </HeaderTemplate>
                        <ItemTemplate>
                                <li>
                                    <span class="compQuantity"><asp:Literal ID="ltlCompQuantity" runat="server" /></span>
                                    <span class="compDescription"><asp:Literal ID="ltlCompDescription" runat="server" /></span>
                                    <span class="compComments"><asp:Literal ID="ltlCompComments" runat="server" /></span>
                                </li>
                        </ItemTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhPageReturn" runat="server">
        <noscript>
            <div id="divPageReturn">
                <asp:Button ID="btnPageReturn" runat="server" CssClass="button" />
            </div>
        </noscript>
    </asp:PlaceHolder>
</div>