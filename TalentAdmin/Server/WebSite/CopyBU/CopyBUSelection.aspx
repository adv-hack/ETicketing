<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="CopyBUSelection.aspx.vb" Inherits="CopyBU_CopyBUSelection" %>

<%@ Register Src="~/UserControls/CopyBUSelection.ascx" TagName="CopyBUSelection" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <uc1:CopyBUSelection ID="CopyBUSelection1" runat="server" />
</asp:Content>