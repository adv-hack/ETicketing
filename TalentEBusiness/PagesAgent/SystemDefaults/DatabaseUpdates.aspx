<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DatabaseUpdates.aspx.vb" Inherits="DatabaseUpdates" MasterPageFile="~/MasterPages/BoxOffice/1Column.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="../../JavaScript/systemDefaults.js"></script>
	<div class="row ebiz-database-updates-wrap">
        <div class="large-12 columns">
	        <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
		        <div class="alert-box alert">
			        <asp:BulletedList ID="blErrorMessages" runat="server" />
		        </div>
	        </asp:PlaceHolder>
	        <div id="container" runat="server"></div>
        </div>
    </div>
</asp:Content>
