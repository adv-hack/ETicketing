<%@ Control 
Language="VB" 
AutoEventWireup="False" 
CodeFile="CopyBU.ascx.vb"
Inherits="UserControls_CopyBU" %>

<div id="copyBU">
    <div class='controls'>
        
        <div class='error'><asp:Label ID="lblError" runat="server"></asp:Label></div>
        
        <div class='label'><asp:Label ID="lblCopyFrom" runat="server" Text="Site to Copy From:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtCopyFrom" runat="server" Enabled="false"></asp:TextBox></div>
        
        <div class='label'><asp:Label ID="lblCopyTo" runat="server" Text="Site to Copy To:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtCopyTo" runat="server" MaxLength="50"></asp:TextBox></div>
  
        <div class='label'><asp:Label ID="lblBU" runat="server" Text="Business Unit:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtBU" runat="server" MaxLength="50"></asp:TextBox></div>

        <div class='label'><asp:Label ID="lblStoredProc" runat="server" Text="Stored Procedure Group:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtSPG" runat="server" Width="500"></asp:TextBox></div>                   
                
        <div class='label'><asp:Label ID="lblStadiumCode" runat="server" Text="Stadium Code:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtStadiumCode" runat="server" MaxLength="50"></asp:TextBox></div>
        
        <div class='label'><asp:Label ID="lblNoiseURLs" runat="server" Text="Noise URLs:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtNoiseURLs" runat="server" Width="500"></asp:TextBox></div>
        
        <div class='label'><asp:Label ID="lblNoiseKey" runat="server" Text="Noise Applicaiton Key:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtNoiseKey" runat="server" Width="500"></asp:TextBox></div>
                
        <div class='label'><asp:Label ID="lblNoiseThresh" runat="server" Text="Noise Threshold:"></asp:Label></div>
        <div class='textbox'><asp:TextBox ID="txtNoiseThres" runat="server" Width="500"></asp:TextBox></div>                
                
        <div class='label'><asp:Label ID="lblResult" runat="server"></asp:Label></div>
        
    </div>
    
    <asp:Button CssClass="button" ID="btnCopy" runat="server" Text="Copy BU" />
    <asp:Button ID="btnBack" runat="server" CssClass="button" PostBackUrl="~/Server/WebSite/CopyBU/CopyBUSelection.aspx" Text="Back" />
    <asp:Button ID="btnHomeLink" runat="server" CssClass="button" PostBackUrl="~/Default.aspx" Text="Return Home" />
    
</div>