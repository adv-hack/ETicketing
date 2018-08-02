<%@ Control Language="VB" AutoEventWireup="false" CodeFile="personalisationBar.ascx.vb" Inherits="UserControls_PersonalisationBar" ViewStateMode="Disabled" %>
<asp:LoginView ID="LoginView" runat="server">
    <AnonymousTemplate>
        <span class="ebiz-personalisation-anonymous-wrap">
            <span class="ebiz-sign-in"><asp:Literal ID="ltlAnonymous" runat="server" /></span>
            <span class="ebiz-register"><asp:Literal ID="ltlRegister" runat="server" /></span>
        </span>
    </AnonymousTemplate>
    <LoggedInTemplate>
        <span class="ebiz-personalisation-signed-in-wrap">
            <span class="ebiz-signed-in"><asp:Literal ID="ltlLoggedIn" runat="server" /></span>
            <span class="ebiz-sign-out">
                <asp:HyperLink ID="hplLogout" runat="server" />
            </span>
        </span>
    </LoggedInTemplate>
</asp:LoginView>