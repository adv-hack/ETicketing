<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="DatabaseDetails.aspx.vb" Inherits="Databases_DatabaseDetails" %>

<%@ Register Src="../UserControls/DatabaseDetails.ascx" TagName="DatabaseDetails" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <uc1:DatabaseDetails ID="DatabaseDetails1" runat="server" />
</asp:Content>
