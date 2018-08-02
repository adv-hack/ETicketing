<%@ Page Language="VB" AutoEventWireup="false" CodeFile="error.aspx.vb" Inherits="PagesPublic_error" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />

    <asp:PlaceHolder ID="plhError" runat="server">
        <p><asp:label ID="lblErrorCode" runat="server" CssClass="alert-box alert" /></p>
    </asp:PlaceHolder>
    
    <asp:PlaceHolder ID="plhBodyLabel" runat="server">
        <p><asp:Literal ID="bodyLabel1" runat="server" /></p>
    </asp:PlaceHolder>

    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
</asp:Content>
