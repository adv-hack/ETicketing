<%@ Page Language="VB" AutoEventWireup="false" CodeFile="VisualSeatSelection.aspx.vb" Inherits="PagesPublic_ProductBrowse_VisualSeatSelection" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/SeatSelection.ascx" TagName="SeatSelection" TagPrefix="Talent" %>
<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0" />
    
    <script type="text/javascript" src="../../JavaScript/jquery.hammer.js"></script>
    <script type="text/javascript" src="../../JavaScript/hammer.js"></script>
    <script type="text/javascript" src="../../JavaScript/Raphael.js"></script>
    <asp:PlaceHolder ID="plhTicketing3DSeatView" runat="server">
        <script src="//ticketing3d-uk.deployment-mmc.com/apis/tk3d.js"></script>
    </asp:PlaceHolder>
    <script language="Javascript" type="text/javascript">
        function removeAllOptions(ddl) {
            var i;
            for (i = ddl.options.length - 1; i >= 0; i--) {
                ddl.remove(i);
            }
        }
        function addOption(selectbox, value, text) {
            var optn = document.createElement("OPTION");
            optn.text = text;
            optn.value = value;
            selectbox.options.add(optn);
        }
    </script>
</asp:Content>

<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:ValidationSummary ID="vlsErrorMessages" runat="server" DisplayMode="BulletList" ShowSummary="true" CssClass="alert-box alert" ValidationGroup="Quantity"  />
    <asp:ValidationSummary ID="vlsFavSeatErrorMessages" runat="server" DisplayMode="BulletList" ShowSummary="true" CssClass="alert-box alert" ValidationGroup="FavSeatQuantity" />
    <Talent:SeatSelection ID="uscSeatSelection" runat="server" IsSeatSelectionOnly="false"  />
    <asp:HiddenField ID="hdfPriceAndAreaSelection" runat="server" ClientIDMode="Static" />
    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>