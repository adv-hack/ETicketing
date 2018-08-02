<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductSummary.ascx.vb" Inherits="UserControls_ProductSummary" %>
<div class="row ebiz-product-summary">
    <asp:PlaceHolder ID="plhProductSummaryError" runat="server">
        <div class="alert-box alert">
            <asp:Literal ID="ltlProductSummaryError" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:HiddenField ID="hdfReturnUrl" runat="server" />
    <asp:PlaceHolder ID="plhProductSummary" runat="server">
        <div class="row">
            <asp:PlaceHolder ID="plhProductSummaryImage" runat="server">
                <div class="large-4 columns">
                    <asp:Image ID="imgProductSummary" runat="server" CssClass="ProductSummary-Image" />
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhProductSummaryHtml" runat="server">
                <div class="large-8 columns">
                    <asp:Literal ID="ltlProductSummaryHtml" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhPageReturn" runat="server">
        <noscript>
            <asp:Button ID="btnPageReturn" runat="server" CssClass="button" />
        </noscript>
    </asp:PlaceHolder>
</div>