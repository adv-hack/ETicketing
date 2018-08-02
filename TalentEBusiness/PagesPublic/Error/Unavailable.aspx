<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Unavailable.aspx.vb" Inherits="PagesPublic_Error_Unavailable" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:PlaceHolder ID="plhUnavailable" runat="server">
        <div class="alert alert-box"><asp:Literal ID="ltlUnavailableDescription" runat="server" /></div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhReturnLink" runat="server">
        <p><asp:HyperLink ID="hlnkUnavailableReturnLink" runat="server" /></p>
    </asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
</asp:Content>
