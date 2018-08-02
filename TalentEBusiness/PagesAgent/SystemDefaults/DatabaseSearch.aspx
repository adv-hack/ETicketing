<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DatabaseSearch.aspx.vb" Inherits="DatabaseSearch" ViewStateMode="Disabled" MasterPageFile="~/MasterPages/BoxOffice/1Column.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
	<div class="row ebiz-database-search-wrap">
		<div class="large-12 columns">
			<asp:placeholder id="plhErrorList" runat="server" visible="false">
				<div class="alert-box alert">
					<asp:BulletedList ID="blErrorMessages" runat="server" />
				</div>
			</asp:placeholder>
			<div id="container" runat="server"></div>
		</div>
	</div>
</asp:Content>
