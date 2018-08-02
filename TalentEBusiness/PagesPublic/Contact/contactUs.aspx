<%@ Page Language="VB" AutoEventWireup="false" CodeFile="contactUs.aspx.vb" Inherits="PagesPublic_contactUs" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ContactUsForm.ascx" TagName="ContactUsForm" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />   
    <Talent:ContactUsForm ID="contactUsForm1" runat="server" />
</asp:Content>