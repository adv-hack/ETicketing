<%@ Page Language="VB" AutoEventWireup="false"  MasterPageFile="~/MasterPages/MasterPage.master" CodeFile="PartnerDetails.aspx.vb" Inherits="Partners_PartnerDetails" %>
<%@ Register Src="../UserControls/PartnerDetail.ascx" TagName="PartnerDetail" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
    <uc1:PartnerDetail ID="PartnerDetail1" runat="server" />
</asp:Content>

