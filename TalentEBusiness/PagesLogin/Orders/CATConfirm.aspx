<%@ Page Title="" Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="CATConfirm.aspx.vb" Inherits="PagesLogin_Orders_CATConfirm" %>
<%@ Register Src="../../UserControls/CATBasket.ascx" TagName="CATBasket" TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:CATBasket ID="CATBasket1" runat="server" />
</asp:Content>