<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MicroBasket.aspx.vb" Inherits="PagesPublic_Basket_MicroBasket" MasterPageFile="~/MasterPages/Shared/Blank.master" %>
<%@ Register Src="~/UserControls/MicroBasket.ascx" TagName="MicroBasket" TagPrefix="Talent" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" ID="Content" runat="server">
    <asp:PlaceHolder ID="plhPersonalisation" runat="server">
        <div class="ebiz-personalisation"><asp:Literal ID="ltlPersonalisation" runat="server" /></div>
    </asp:PlaceHolder>
    <Talent:MicroBasket ID="uscMicroBasket" runat="server" />
</asp:Content>