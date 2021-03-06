<%@ Page Language="VB" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" CodeFile="productHome.aspx.vb" Inherits="PagesPublic_productHome"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductDetail.ascx" TagName="ProductDetail" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
    <script src="https://a248.e.akamai.net/images.talentarena.co.uk/Global/Lib/ticketing-products-date-picker.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 

    <asp:PlaceHolder ID="plhMultiStadiumText" runat="server">
        <div class="ebiz-multi-stadium-text"><asp:Literal ID="multiStadiumMainTextLabel" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:BulletedList ID="ErrorList" runat="server" CssClass="alert-box alert" />
    <Talent:ProductDetail ID="ProductDetail1" runat="server"  ProductType="H" />

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>