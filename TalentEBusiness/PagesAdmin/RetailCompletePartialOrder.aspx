<%@ Page Language="VB" AutoEventWireup="false" CodeFile="RetailCompletePartialOrder.aspx.vb" Inherits="PagesAdmin_RetailCompletePartialOrder" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    Login Verification : <asp:TextBox runat="server" id="txtCanOveride"  /> 
    <br />
    Order Id : <asp:TextBox runat="server" id="txtTempOrderID" />
    <br />
    <asp:Button runat="server" id="btnCompletePartialOrder" text="Process Merchandise In Backend" />
</asp:Content>
