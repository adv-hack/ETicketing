<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ActivitiesEdit.aspx.vb" Inherits="PagesAgent_CRM_ActivitiesEdit" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/EditProfileActivity.ascx" TagPrefix="Talent" TagName="EditProfileActivity" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:EditProfileActivity ID="uscEditProfileActivity" runat="server" />
</asp:Content>
