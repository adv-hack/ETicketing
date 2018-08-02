<%@ Page Language="VB" AutoEventWireup="false" CodeFile="HopewiserPostcodeAndCountry.aspx.vb" Inherits="PagesPublic_HopewiserPostcodeAndCountry" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />

    <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert-box alert ebiz-hopewiser-error" />
    <div class="panel ebiz-hopewiser-wrap">
        <h2><%= GetPageText("headerText")%></h2>
        <div class="row ebiz-hopewiser-postcode-wrap">
            <div class="columns medium-6">
                <asp:Label ID="lblPostCode" runat="server" AssociatedControlID="postcode" />
            </div>
            <div class="columns medium-6">
                <asp:TextBox ID="postcode" MaxLength="9" runat="server" />
            </div>
        </div>
        <div class="row ebiz-hopewiser-country-wrap">
            <div class="columns medium-6">
                <asp:Label ID="lblCountry" runat="server" AssociatedControlID="country" />
            </div>
            <div class="columns medium-6">
                <asp:DropDownList ID="country" runat="server" />
            </div>
        </div>
        <%= GetPageText("footerText")%>
        <div class="button-group ebiz-hopewiser-buttons-wrap">
            <asp:LinkButton ID="ButtonClose" runat="server" CssClass="button ebiz-close" OnClientClick="Javascript: window.close();"><%= GetPageText("closeButtonText")%></asp:LinkButton>
            <asp:LinkButton ID="ButtonNext" runat="server" CssClass="button ebiz-primary-action ebiz-next" OnClientClick="Javascript:return submitForm();"><%= GetPageText("nextButtonText")%></asp:LinkButton>
        </div>
        <input type="hidden" name="hiddenCountry" />
    </div>
    <% CreateJavascript() %>
</asp:Content>