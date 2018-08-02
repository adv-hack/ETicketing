<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SessionStatus.ascx.vb" Inherits="UserControls_SessionStatus" ViewStateMode="Disabled" %>

<asp:placeholder id="plhSeassionStatus" visible="True" runat="server">
    <div class="alert-box warning ebiz-session-status">
        <h2>
        	
            	<asp:Label ID="statusHeader" runat="server" Text="Label"></asp:Label>
        	
    	</h2>
        <p>
        	<asp:Label ID="statusContent" runat="server" Text="Label"></asp:Label>
    	</p>
    </div>
</asp:placeholder>