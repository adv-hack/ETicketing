<%@ Page Language="VB" AutoEventWireup="false" CodeFile="changePassword.aspx.vb" Inherits="PagesPublic_changePassword" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ChangePassword.ascx" TagName="ChangePassword"  TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhMessage" runat="server">
        <div class="alert-box warning"><asp:Literal ID="MessageLabel" runat="server" /></div>
    </asp:PlaceHolder>
    <Talent:ChangePassword ID="uscChangePassword" runat="server" />
</asp:Content>