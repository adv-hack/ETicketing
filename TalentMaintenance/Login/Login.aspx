<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login_Login" %>

<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>TALENT Sport &ndash; Maintenance</title>
</head>
<body class="MaintenanceMasterPage Login">
    <form id="form1" runat="server">
		<div id="container">
			<header>
				<img src="/Maintenance/App_Themes/Maintenance/images/logo.png" alt="" />
			</header>
			<div id="login-form">
				<asp:PlaceHolder ID="plhLoginError" runat="server" Visible="false">
					<div class="login-error">
						<span><asp:Literal ID="ltlLoginError" runat="server" /></span>
					</div>
				</asp:PlaceHolder>
				<asp:Login ID="Login1" runat="server" DisplayRememberMe="false" />
			</div>
			<footer>
			</footer>
		</div>
    </form>
</body>
</html>