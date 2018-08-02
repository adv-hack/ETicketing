<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TalentEmail.aspx.vb" Inherits="PagesAdmin_TalentEmail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Button ID="btnTicketingEmail" runat="server" Text="Ticketing Email" />
    <br /><br />
    <asp:Button ID="btnRetailEmail" runat="server" Text="Retail Email" />
    </div>
    <br /><br />
    <asp:TextBox ID="txtEmail" runat="server" />
    <asp:RegularExpressionValidator ID="regExEmail" runat="server" ControlToValidate="txtEmail" ValidationExpression="^(?!.*websales@wolves\.co\.uk.*).+@[^\.].*\.[A-Za-z]{2,}$" Text="err" />
    <!-- ^.+@[^\.].*\.[A-Za-z]{2,}$ -->
    </form>
</body>
</html>
