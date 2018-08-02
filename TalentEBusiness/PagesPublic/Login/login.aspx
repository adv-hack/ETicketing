<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="PagesPublic_login" EnableEventValidation="false" %>
<%@ Register Src="~/UserControls/ActivateAccount.ascx" TagName="ActivateAccount" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/LoginBox.ascx" TagName="LoginBox" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegisterAccount.ascx" TagName="RegisterAccount" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm.ascx" TagName="RegistrationForm1" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm2.ascx" TagName="RegistrationForm2" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/RegistrationForm3.ascx" TagName="RegistrationForm3" TagPrefix="Talent" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
   <asp:Literal ID="MessageLabel" runat="server" />
   <asp:Literal ID="LoginStatus" runat="server" />
   <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
   <Talent:LoginBox ID="MainLoginBox" runat="server" />
   <Talent:ActivateAccount ID="ActivateAccount1" runat="server" />
   <Talent:RegisterAccount ID="RegisterAccount1" runat="server" /> 
   <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
   <Talent:RegistrationForm1 ID="RegistrationForm1" runat="server" /> 
   <Talent:RegistrationForm2 ID="RegistrationForm2" runat="server" /> 
   <Talent:RegistrationForm3 ID="RegistrationForm3" runat="server" /> 
</asp:Content>