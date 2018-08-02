<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataAnnotation.aspx.vb" Inherits="Examples_DataAnnotation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:PlaceHolder ID="plhErrorList" runat="server">
    	<div class="alert-box alert">
    		<asp:BulletedList ID="blErrorMessages" runat="server" />
    	</div>
    </asp:PlaceHolder>
    <div>
            <table>
            <tr>
                <td>First Name: </td>
                <td><asp:TextBox ID="txtFirstName" runat="server" /></td>
            </tr>
            <tr>
                <td>Last Name: </td>
                <td><asp:TextBox ID="txtLastName" runat="server" /></td>
            </tr>
            <tr>
                <td>Tickets quantity: </td>
                <td><asp:TextBox ID="txtQuantity" runat="server" /></td>
            </tr>
            <tr>
                <td><asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click"/></td>
                <td></td>
            </tr>                            
        </table>
    </div>
    </form>
</body>
</html>
