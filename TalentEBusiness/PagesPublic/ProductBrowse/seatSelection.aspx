<%@ Page Language="VB" AutoEventWireup="false" CodeFile="seatSelection.aspx.vb" Inherits="PagesPublic_ProductBrowse_seatSelection" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/SeatSelection.ascx" TagName="SeatSelection" TagPrefix="Talent" %>

<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script type="text/javascript" src="../../JavaScript/jquery.hammer.js"></script>
    <script type="text/javascript" src="../../JavaScript/hammer.js"></script>
    <script type="text/javascript" src="../../JavaScript/Raphael.js"></script>
    <script>
        var renderStandAreaDDLOptions = false;
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhBackButton" runat="server"><div class="back-button"><asp:HyperLink ID="hplBack" runat="server" CssClass="button" /></div></asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessages" runat="server">
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error"></asp:BulletedList>
    </asp:PlaceHolder>

    <Talent:SeatSelection ID="uscSeatSelection" runat="server" IsSeatSelectionOnly="true" />
    <asp:HiddenField ID="hdfPriceAndAreaSelection" runat="server" ClientIDMode="Static" />

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>