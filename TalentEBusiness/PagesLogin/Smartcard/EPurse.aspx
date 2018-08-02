<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EPurse.aspx.vb" Inherits="PagesLogin_Smartcard_EPurse" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:BulletedList ID="blErrorMessage" runat="server" CssClass="alert-box alert" />
    <asp:ValidationSummary ID="vlsErrorMessage" runat="server" CssClass="alert-box alert" ValidationGroup="EPurse" />
    
    <div class="panel ebiz-epurse-balance">
       <asp:PlaceHolder ID="plhEPurseCardSelectionandValues" runat="server">
       <asp:PlaceHolder ID="plhEPurseBalanceHeader" runat="server"><h2><asp:Literal ID="ltlEPurseBalanceHeader" runat="server" /></h2></asp:PlaceHolder>
        <div class="row ebiz-epurse-card-number">
            <div class="large-4 columns"><asp:Literal ID="ltlPleaseChooseCard" runat="server" /></div>
            <div class="large-4 columns"><asp:Literal ID="ltlEPurseCardNumberLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:DropDownList ID="ddlSelectCard" runat="server" ClientIDMode="Static" AutoPostBack="True"  ViewStateMode="Enabled" /></div>
        </div>
        <div class="row ebiz-epurse-card-number">
            <div class="large-4 columns"><asp:Literal ID="ltlEPurseCardAmountLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlEPurseCardAmountValue" runat="server" /></div>
        </div>
        <asp:PlaceHolder ID="plhEPurseCardAmountToSpend" runat="server">
        <div class="row ebiz-epurse-card-amount-to-spend">
            <div class="large-4 columns"><asp:Literal ID="ltlEPurseCardAmountToSpendLabel" runat="server" /></div>
            <div class="large-8 columns"><asp:Literal ID="ltlEPurseCardAmountToSpendValue" runat="server" /></div>
        </div>
        </asp:PlaceHolder>
          </asp:PlaceHolder>
    </div>

    <asp:Panel ID="pnlEPurseTopUp" runat="server" DefaultButton="btnEPurseTopUp" CssClass="panel ebiz-epurse-top-up">
        <asp:PlaceHolder ID="plhEPurseTopUpHeader" runat="server"><h2><asp:Literal ID="ltlEPurseTopUpHeader" runat="server" /></h2></asp:PlaceHolder>
        <asp:PlaceHolder ID="plhEPurseTopUpInstructions" runat="server">
            <p><asp:Literal ID="ltlEPurseTopUpInstructions" runat="server" /></p>
        </asp:PlaceHolder>
        <fieldset>
            <legend><asp:Literal ID="ltlEPurseTopUpLegend" runat="server" /></legend>
            <asp:PlaceHolder ID="plhEPurseTopUpTextBox" runat="server">
            <div class="row epurse-top-up-amount">
                <div class="large-3 columns">
                    <asp:Label ID="lblEPurseTopUpAmountText" runat="server" AssociatedControlID="txtEPurseTopUpAmount" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtEPurseTopUpAmount" runat="server" MaxLength="8" />
                    <asp:RegularExpressionValidator ID="rgxEPurseTopUpAmount" runat="server" ControlToValidate="txtEPurseTopUpAmount"
                        Display="None" SetFocusOnError="true" ValidationGroup="EPurse" CssClass="error" />
                    <asp:RequiredFieldValidator ID="rfvEPurseTopUpAmount" runat="server" ControlToValidate="txtEPurseTopUpAmount"
                        Display="None" SetFocusOnError="true" ValidationGroup="EPurse" CssClass="error" />
                </div>
            </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhEPurseTopUpDropDownList" runat="server">
            <div class="row epurse-top-up-amount">
                <div class="large-3 columns">
                    <asp:Label ID="lblEPurseTopUpAmountDropDownList" runat="server" AssociatedControlID="ddlEPurseTopUpAmount" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlEPurseTopUpAmount" runat="server" ViewStateMode="Enabled" />
                </div>
            </div>
            </asp:PlaceHolder>
            <asp:Button ID="btnEPurseTopUp" runat="server" CausesValidation="true" ValidationGroup="EPurse" CssClass="button" />
        </fieldset>
    </asp:Panel>

    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>