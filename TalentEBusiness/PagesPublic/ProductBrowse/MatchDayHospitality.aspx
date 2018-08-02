<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MatchDayHospitality.aspx.vb" Inherits="PagesPublic_ProductBrowse_MatchDayHospitality" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/ProductDetail.ascx" TagName="ProductDetail" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script src="https://a248.e.akamai.net/images.talentarena.co.uk/Global/Lib/ticketing-products-date-picker.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <div id="multiStadiumText" runat="server"><asp:Label ID="multiStadiumMainTextLabel" runat="server" Text=""></asp:Label></div>
    <asp:BulletedList ID="ErrorList" runat="server" CssClass="error"></asp:BulletedList>
    <Talent:ProductDetail ID="ProductDetail1" runat="server"  ProductType='CH' />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />

</asp:Content>