<%@ Page Language="VB" AutoEventWireup="false" CodeFile="registrationConfirmation.aspx.vb" Inherits="PagesPublic_registrationConfirmation" title="Untitled Page" %>
<%@ Register Src="../../UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/RegistrationConfirmationUser.ascx"
    TagName="RegistrationConfirmationUser" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/RegistrationConfirmationPartner.ascx"
    TagName="RegistrationConfirmationPartner" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1"/>
    <Talent:RegistrationConfirmationUser ID="RegistrationConfirmationUser1" runat="server" />
    <Talent:RegistrationConfirmationPartner ID="RegistrationConfirmationPartner1" runat="server" />
</asp:Content>