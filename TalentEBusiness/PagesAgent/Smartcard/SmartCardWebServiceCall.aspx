<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/Blank.master" AutoEventWireup="false" CodeFile="SmartcardWebServiceCall.aspx.vb" Inherits="PagesAgent_Smartcard_SmartCardWebServiceCall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<div class="smartcard-wrapper">
    <h1>Red/White List Processing</h1>
    <p>Please enter a product code in the box below and click Process Request to generate a red and white list. You will then be able to save them locally or to a portable device.</p>
    <asp:TextBox ID="txtProductCode" runat="server" /><asp:Button ID="btnProcess" runat="server" Text="Process Request" />
    <asp:PlaceHolder ID="plhResponse" runat="server" Visible="false">
    <div class="response">
        <p><asp:Label ID="lblResponseMessage" runat="server" /></p>
    </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhFileSaveOption" runat="server" Visible="false">
    <div class="file-save-option">
        <p>The lists have been created successfully.</p>
        <p>Please select the desired list to view its content, or <strong>right click</strong> and <strong>Save As</strong> to save it to a local drive.</p>
        <p><asp:HyperLink ID="hplWhiteList" runat="server" Text="White list" Target="_blank" /></p>
        <p><asp:HyperLink ID="hplRedList" runat="server" Text="Red list" Target="_blank" /></p>
    </div>
    </asp:PlaceHolder>
</div>
</asp:Content>