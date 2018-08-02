<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProfileAttributes.aspx.vb" Inherits="PagesLogin_Profile_ProfileAttributes" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProfileAttributes.ascx" TagName="ProfileAttributes" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="1" />
    <Talent:ProfileAttributes ID="ProfileAttributes" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
</asp:Content>

