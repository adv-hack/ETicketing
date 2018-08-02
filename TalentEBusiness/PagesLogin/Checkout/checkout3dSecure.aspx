<%@ Page Language="VB" AutoEventWireup="false" CodeFile="checkout3dSecure.aspx.vb" Inherits="PagesLogin_checkout3dSecure" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <div class="ebiz-3d-secure-wrap">
    	<iframe runat="server" id="iframe3dSecure" class="ebiz-checkout-3dsecure-iframe"></iframe>
	</div>
</asp:Content>