<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false"
    CodeFile="Default.aspx.vb" Inherits="_Default" Title="Untitled Page" EnableSessionState="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div class="title">
        <asp:Label ID="lblHeader" runat="server"></asp:Label>
    </div>
    <div class="details">
        <div class="item">
            <asp:Label ID="lblDetail" runat="server">Website Type:</asp:Label>
        </div>
    </div>
</asp:Content>

