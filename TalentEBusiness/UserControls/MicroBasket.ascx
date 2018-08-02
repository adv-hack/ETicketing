<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MicroBasket.ascx.vb" Inherits="UserControls_MicroBasket" %>
<asp:UpdatePanel ID="updMicroBasket" RenderMode="Inline" runat="server" class="ebiz-micro-basket-wrap">
	<ContentTemplate>
	    <asp:HyperLink ID="hplBasket" runat="server" NavigateUrl="~/PagesPublic/Basket/Basket.aspx" Target="_top" />
	</ContentTemplate>
</asp:UpdatePanel>