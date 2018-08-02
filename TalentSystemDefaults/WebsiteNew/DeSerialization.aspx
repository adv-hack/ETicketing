<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DeSerialization.aspx.vb" Inherits="DeSerialization" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<script src="JavaScript/jquery.js"></script>
	<link href="Style/foundation.min.css" rel="stylesheet" />
	<script src="JavaScript/foundation.min.js"></script>
	<script src="JavaScript/modernizr.js"></script>
	<link href="Style/layout.css" rel="stylesheet" />
	<link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/font-awesome/4.4.0/css/font-awesome.min.css" />
</head>
<body>
	<form id="form1" runat="server">
		<br />
		<br />
		<div class="medium-12 columns">
			<div class="row">
				<div class="medium-6 column ebiz-right">
					Repeat the transaction for Payment Type Code: 
				</div>
				<div class="medium-6 column ebiz-left">
					<asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
				</div>
			</div>
			<div class="row">
				<div class="medium-6 column ebiz-right">
					Connection String:
				</div>
				<div class="medium-6 column ebiz-left">
					<asp:DropDownList ID="DropDownList1" runat="server">
						<asp:ListItem Value="1">TalentEBusinessDBTalentDev</asp:ListItem>
						<asp:ListItem Value="2">TalentConfiguration</asp:ListItem>
					</asp:DropDownList>
				</div>
			</div>
			<div class="row">
				<div class="medium-6 column"></div>
				<div class="medium-6 column">
					<asp:Button ID="Button1" runat="server" Text="Proceed" CssClass="button tiny" />
					<br />
					<asp:Label ID="lblMsg" runat="server"></asp:Label>
				</div>
			</div>
		</div>
	</form>
</body>
</html>
