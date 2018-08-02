<%@ Page Language="VB" AutoEventWireup="false" CodeFile="registration.aspx.vb" Inherits="PagesPublic_registration" EnableEventValidation="false" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Agent.ascx" TagName="Agent" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm.ascx" TagName="RegistrationForm" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm2.ascx" TagName="RegistrationForm2" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm3.ascx" TagName="RegistrationForm3" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Company/CustomerCompany.ascx" TagPrefix="Talent" TagName="CustomerCompany" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhAgent" runat="server">
        <Talent:Agent ID="agent1" runat="server" />
    </asp:PlaceHolder>
    <Talent:CustomerCompany runat="server" id="CustomerCompany" />
    <asp:PlaceHolder ID="plhRegistrationForm1" runat="server">
        <Talent:RegistrationForm ID="registrationForm1" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhRegistrationForm2" runat="server">
        <Talent:RegistrationForm2 ID="registrationForm2" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhRegistrationForm3" runat="server">
        <Talent:RegistrationForm3 ID="registrationForm3" runat="server" />
    </asp:PlaceHolder>
</asp:Content>
