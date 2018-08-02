<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PageLeftProductNav.ascx.vb" Inherits="UserControls_PageLeftProductNav" %>

<asp:TreeView ID="trvProductNav" OnSelectedNodeChanged="trvProductNav_SelectedNodeChanged" OnAdaptedSelectedNodeChanged="trvProductNav_SelectedNodeChanged" runat="server" ShowExpandCollapse="False" />