<%@ Page Language="VB" AutoEventWireup="false" CodeFile="browse07.aspx.vb" Inherits="PagesPublic_browse07" title="Untitled Page" EnableEventValidation="false" %>
<%@ Register Src="~/UserControls/ProductList.ascx" TagName="ProductList" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductList2.ascx" TagName="ProductList2" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductListGraphical.ascx" TagName="ProductListGraphical" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductListGraphical2.ascx" TagName="ProductListGraphical2" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/GroupGraphical.ascx" TagName="GroupGraphical" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Banners.ascx" TagName="Banners" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
        <div class="alert-box alert">
            <asp:Literal ID="ltlError" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="product" CssClass="alert-box alert" />
    <asp:PlaceHolder ID="plhProductList" runat="server">
        <Talent:ProductList id="ProductList" runat="server"></Talent:ProductList>
        <Talent:ProductList2 id="ProductList2" runat="server"></Talent:ProductList2>
        <Talent:ProductListGraphical id="ProductListGraphical" runat="server"></Talent:ProductListGraphical>
        <Talent:ProductListGraphical2 id="ProductListGraphical2" runat="server"></Talent:ProductListGraphical2>
        <Talent:GroupGraphical id="GroupGraphical" runat="server"></Talent:GroupGraphical>
    </asp:PlaceHolder>
</asp:Content>