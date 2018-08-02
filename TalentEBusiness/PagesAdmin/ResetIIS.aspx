<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResetIIS.aspx.vb" Inherits="PagesAdmin_ResetIIS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Reset IIS</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <asp:Button ID="loadIIS" runat="server" Text="Load IIS" />
    <asp:Button ID="resetIIS" runat="server" Text="Reset IIS" Visible="false" />
    <asp:Label ID="lblWebConfig" runat="server" Text="Click the 'Load IIS' Button to begin."/>
    </div>
    </form>
</body>
</html>
