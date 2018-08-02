<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master"
    CodeFile="CopyBU.aspx.vb" Inherits="BU_CopyBU" %>

<%@ Register Src="../UserControls/BusinessUnitCopy.ascx" TagName="BusinessUnitCopy"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:BusinessUnitCopy ID="BusinessUnitCopy1" runat="server" />
</asp:Content>
