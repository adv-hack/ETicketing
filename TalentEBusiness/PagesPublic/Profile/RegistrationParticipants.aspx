<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RegistrationParticipants.aspx.vb" Inherits="PagesPublic_Profile_RegistrationParticipants" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationParticipants.ascx" TagName="RegistrationParticipants" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:registrationparticipants id="RegistrationParticipants1" runat="server" />
</asp:Content>