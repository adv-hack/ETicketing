<%@ Page Language="VB" AutoEventWireup="false" CodeFile="updateProfileConfirmation.aspx.vb" Inherits="PagesLogin_updateProfileConfirmation" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/UpdateDetailsConfirmation.ascx" TagName="UpdateDetailsConfirmation" TagPrefix="uc1" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:Literal ID="MessageLabel" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1"/>
    <uc1:UpdateDetailsConfirmation runat="server" id="uclUpdateDetailConfirmation"/>
</asp:Content>