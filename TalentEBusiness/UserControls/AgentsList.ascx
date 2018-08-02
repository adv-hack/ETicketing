<%@ Control Language="VB" AutoEventWireup="false" CodeFile="AgentsList.ascx.vb" Inherits="UserControls_AgentsList" %>
<asp:PlaceHolder ID="plhAgent" runat="server">
	<div class="row">
		<div class="medium-3 columns">
			<asp:Label runat="server" ID="lblAgent" AssociatedControlID="ddlAgents" />
		</div>
		<div class="medium-9 columns">
			<asp:DropDownList ID="ddlAgents" runat="server" AutoPostBack="true" />
		</div>
	</div> 
</asp:PlaceHolder>