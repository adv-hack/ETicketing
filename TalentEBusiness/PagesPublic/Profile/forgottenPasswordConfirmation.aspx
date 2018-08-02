<%@ Page Language="VB" AutoEventWireup="false" CodeFile="forgottenPasswordConfirmation.aspx.vb" Inherits="PagesLogin_forgottenPasswordConfirmation" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:PlaceHolder ID="plhMessage" runat="server">
        <p><asp:Literal ID="MessageLabel" runat="server" /></p>
    </asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
</asp:Content>
