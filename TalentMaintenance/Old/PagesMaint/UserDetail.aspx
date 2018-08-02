<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="UserDetail.aspx.vb" Inherits="PagesMaint_UserDetail" Title="PagesMaint_UserDetail" %>

<%@ Register Src="../UserControls/UserEdit.ascx" TagName="UserEdit" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc2:UserEdit ID="UserEdit1" runat="server" />
</asp:Content>
