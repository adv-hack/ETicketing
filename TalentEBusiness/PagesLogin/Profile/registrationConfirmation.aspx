<%@ Page Language="VB" AutoEventWireup="false" CodeFile="registrationConfirmation.aspx.vb" Inherits="PagesPublic_registrationConfirmation" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:Literal ID="MessageLabel" runat="server" />

    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1"/>
    
    <asp:Panel ID="CameraButtonPanel" runat="server">
        <input type="button" class="button" runat="server" id="capturePhotoBtn" />
    </asp:Panel>

</asp:Content>