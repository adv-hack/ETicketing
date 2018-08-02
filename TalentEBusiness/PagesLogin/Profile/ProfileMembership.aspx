<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ProfileMembership.aspx.vb" Inherits="PagesLogin_Profile_ProfileMembership" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProfileMemberships.ascx" TagName="ProfileMemberships" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="1" />
    <Talent:ProfileMemberships ID="ProfileMembership" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
</asp:Content>