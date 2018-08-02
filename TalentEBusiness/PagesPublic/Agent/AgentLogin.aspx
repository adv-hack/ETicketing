<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AgentLogin.aspx.vb" Inherits="PagesPublic_Agent_AgentLogin" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:Panel ID="pnlLoginWrapper" CssClass="panel ebiz-agent-login" runat="server" DefaultButton="btnLogin">
        <asp:PlaceHolder ID="plhAgentLogin" runat="server">
            <img src="../../App_Themes/BUI/img/Advanced.svg" alt="Advanced" class="ebiz-advanced-logo">
            <asp:PlaceHolder ID="plhHeader" runat="server">
                <h2><asp:Literal ID="ltlHeader" runat="server" /></h2>
            </asp:PlaceHolder>
            <asp:ValidationSummary ID="vlsAgentLogin" runat="server" ValidationGroup="AgentLogin" CssClass="alert-box alert" />
            <asp:PlaceHolder ID="plhErrorMessage" runat="server">
                <div class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></div>
            </asp:PlaceHolder>
            <fieldset>
                <legend>
                    <asp:Literal ID="ltlFieldsetLegend" runat="server" />
                </legend>
                <asp:PlaceHolder ID="plhUserName" runat="server">
                    <div class="ebiz-user-name">
                        <asp:Label ID="lblUsername" runat="server" AssociatedControlID="txtUserName" />
                        <asp:TextBox ID="txtUserName" runat="server" ValidationGroup="AgentLogin" MaxLength="10" />
                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ControlToValidate="txtUserName" ValidationGroup="AgentLogin" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPassword" runat="server">
                    <div class="ebiz-password">
                        <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" />
                        <asp:TextBox ID="txtPassword" runat="server" ValidationGroup="AgentLogin" TextMode="Password" MaxLength="10" />
                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" ValidationGroup="AgentLogin" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhDepartment" runat="server">
                    <div class="ebiz-department">
                            <asp:Label ID="lblDepartment" runat="server" AssociatedControlID="ddlDepartment" />
                            <asp:DropDownList ID="ddlDepartment" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvDepartment" runat="server" ControlToValidate="ddlDepartment" ValidationGroup="AgentLogin" Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" EnableClientScript="true" InitialValue="-1"  />
                    </div>
                </asp:PlaceHolder>
               
                <div class="large expanded button-group ebiz-login-in-button">
                    <asp:Button ID="btnBackToLogin" runat="server" CssClass="button ebiz-back" />
                    <asp:Button ID="btnLogin" runat="server" CssClass="button ebiz-primary-action ebiz-login" CausesValidation="true" ValidationGroup="AgentLogin" />
                    <asp:Button ID="btnLogoutLogin" runat="server" CssClass="button ebiz-primary-action ebiz-logout" ValidationGroup="AgentLogin" />
                </div>
            </fieldset>
        </asp:PlaceHolder>
    </asp:Panel>
    <asp:PlaceHolder ID="plhAgentAlreadyLoggedIn" runat="server">
        <div class="panel ebiz-agent-login ebiz-already-logged-in">
            <h1>TALENT</h1>
            <h2><asp:Literal ID="ltlAlreadyLoggedIn" runat="server" /></h2>
            <div class="ebiz-log-out">
                <asp:Button ID="btnLogout" runat="server" CssClass="button ebiz-primary-action" />
            </div>
        </div>
    </asp:PlaceHolder>
</asp:content>
