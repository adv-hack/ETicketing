<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="CopyBU.aspx.vb" Inherits="CopyBU_CopyBU" %>

<%@ Register Src="~/UserControls/CopyBU.ascx" TagName="CopyBU" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <uc1:CopyBU ID="CopyBU1" runat="server" />
</asp:Content>