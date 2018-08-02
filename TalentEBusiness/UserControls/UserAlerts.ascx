<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UserAlerts.ascx.vb" Inherits="UserControls_UserAlerts" %>

<asp:PlaceHolder ID="plhUserAlerts" runat="server">
    <div class="show-for-medium-up ebiz-user-alerts">
        <div id="alerts_with_js_enabled" class="alerts_summary" style="display:none">
            <asp:HyperLink ID="hlkAlertsRedirect" runat="server" data-open="alerts-redirect-modal" CssClass="ebiz-open-modal">
                <i class="fa fa-comments-o"></i><span><asp:Literal ID="ltlAlertsText02" runat="server" /></span>
            </asp:HyperLink>   
            <div id="alerts-redirect-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>      
        </div>
    </div>

    <asp:Literal ID="ltlJavaScriptString" runat="server" />
    <script type="text/javascript" lang="javascript">
        document.getElementById('alerts_with_js_enabled').style.display = 'block';
    </script>
</asp:PlaceHolder>