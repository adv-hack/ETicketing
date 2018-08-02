<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="ClubDetails.aspx.vb" Inherits="Clubs_ClubDetails" %>
<%@ Register Src="../UserControls/ClubDetails.ascx" TagName="ClubDetails" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
<uc1:ClubDetails ID="ClubDetailsControl" runat="server" />
</asp:Content>

