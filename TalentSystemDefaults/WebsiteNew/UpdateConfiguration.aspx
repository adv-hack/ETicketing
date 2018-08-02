<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UpdateConfiguration.aspx.vb" Inherits="UpdateConfiguration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
        <title></title>
    <script src="JavaScript/jquery.js"></script>
	<link href="Style/foundation.min.css" rel="stylesheet" />
	<script src="JavaScript/foundation.min.js"></script>
	<script src="JavaScript/modernizr.js"></script>
	<link href="Style/layout.css" rel="stylesheet" />
	
</head>
<body>
    <form id="form1" runat="server">
    <div class="mediunm-12 columns">
        <br />
        <br />
        <div class="row">
            <div class="medium-6 column ebiz-left">
                <asp:Label ID="lblConfigurationId" runat="server">
                    Enter Configuration ID to update:
                </asp:Label>
            </div>
            <div class="medium-6 column">
                <asp:TextBox ID="txtConfigId" runat="server">
                </asp:TextBox>
            </div>
        </div>
        <asp:PlaceHolder ID="plhNewConfigId" runat="server" Visible="false">
            <div class="row">
                <div class="medium-6 column ebiz-left">
                    <asp:Label ID="lblNewConfigurationId" runat="server">
                        New Configuration ID:
                     </asp:Label>
                </div>
                <div class="medium-6 column">
                    <asp:Label ID="lblNewConfigId" runat="server">
                    </asp:Label>
                </div>
            </div>
        </asp:PlaceHolder>
        </div>
        <div class="row">
           <div class="ebiz-left">
                    <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red">
                    </asp:Label>
          </div>
        </div>
        <div class="row">
            <div class="medium-6 column"></div>
            <div class="medium-6 column">
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button tiny">
                </asp:Button>
            </div>
        </div>
    </form>
</body>
</html>
