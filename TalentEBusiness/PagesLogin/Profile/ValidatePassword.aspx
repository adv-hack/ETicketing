<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ValidatePassword.aspx.vb" Inherits="PagesLogin_Profile_ValidatePassword" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhErrorMessages" runat="server">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:Panel ID="pnlValidatePassword" runat="server" DefaultButton="btnValidatePassword" CssClass="panel ebiz-validate-password">
        <asp:PlaceHolder ID="plhTitle" runat="server"><h2><asp:Literal ID="ltlTitle" runat="server" /></h2></asp:PlaceHolder>
        <fieldset>
            <legend><asp:Literal ID="ltlValidatePasswordLegend" runat="server" /></legend>
            <div class="row">
                <div class="large-3 columns">
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtPassword" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtPassword" CssClass="input-l" runat="server" TextMode="Password" />
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" Display="Static" CssClass="error" SetFocusOnError="true" />
                </div>
            </div>
            <asp:Button ID="btnValidatePassword" runat="server" CssClass="button" />
        </fieldset>
    </asp:Panel>
</asp:Content>