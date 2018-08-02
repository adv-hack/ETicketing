<%@ Control Language="VB" AutoEventWireup="false" CodeFile="LoyaltySummary.ascx.vb" Inherits="UserControls_Loyalty" %>

<asp:Panel ID="LoyaltySummaryPanelView" runat="server" class="alert-box ebiz-loyalty-summary">
     <h2>
     	
     		<asp:Label ID="LoyaltyHeaderLabel" runat="server" OnPreRender="GetText"></asp:Label>
            
        
    </h2>
    <p>
    	<asp:Label ID="TotalLabel" runat="server" OnPreRender="GetText"></asp:Label> <span class="ebiz-loyalty-total"><asp:Label ID="Total" runat="server" OnPreRender="GetText"></asp:Label></span>
	</p>
</asp:Panel>


