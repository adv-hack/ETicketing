<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ActivateSmartcard.aspx.vb" Inherits="PagesLogin_Smartcard_ActivateSmartcard" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server" ViewStateMode="Enabled">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:PlaceHolder ID="plhErrorMessage" runat="server" Visible="false">
        <p class="alert-box alert"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhCurrentSmartcard" runat="server" Visible="false">
        <div class="panel ebiz-current-card-details">
            <h2><asp:Literal ID="ltlCurrentCardDetailsH2" runat="server" /></h2>
            <div class="row ebiz-current-smartcard">
                <div class="large-4 columns"><asp:Literal ID="ltlCurrentSmartcardLabel" runat="server" /></div>
                <div class="large-8 columns"><asp:Literal ID="ltlCurrentSmartcardValue" runat="server" /></div>
            </div>
            <div class="row ebiz-current-fan-id">
                <div class="large-4 columns"><asp:Literal ID="ltlCurrentFanIdLabel" runat="server" /></div>
                <div class="large-8 columns"><asp:Literal ID="ltlCurrentFanIdValue" runat="server" /></div>
            </div>
            <div class="row ebiz-current-4digits">
                <div class="large-4 columns"><asp:Literal ID="ltlCurrent4DigitsLabel" runat="server" /></div>
                <div class="large-8 columns"><asp:Literal ID="ltlCurrent4DigitsValue" runat="server" /></div>
            </div>
            <div class="row ebiz-current-Issue">
                <div class="large-4 columns"><asp:Literal ID="ltlCurrentIssueLabel" runat="server" /></div>
                <div class="large-8 columns"><asp:Literal ID="ltlCurrentIssueValue" runat="server" /></div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhFanIdDescription" runat="server">
        <div class="ebiz-fan-id-description"><asp:Literal ID="ltlFanIdDescription" runat="server" /></div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhFanIdList" runat="server">
        <div class="panel ebiz-fan-id-list">
        <fieldset>
            <legend><asp:Literal ID="ltlFanIdListLegend" runat="server" /></legend>
            <asp:Repeater ID="rptFanIdList" runat="server">
                <HeaderTemplate><table class="stack">
                    <tr>
                        <th></th>
                        <th><asp:Literal ID="ltlFanIdLabel" runat="server" Text='<%# AvailableFanIdLabel %>' /></th>
                        <th><asp:Literal ID="ltlLastFourNumbersLabel" runat="server" Text='<%# FanIdLastFourNumbersLabel %>' /></th>
                        <th><asp:Literal ID="ltlIssueNumber" runat="server" Text='<%# FanIdIssueNumberLabel %>' /></th>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:PlaceHolder ID="plhItemRow" runat="server" Visible="false">
                        <tr>
                            <td><asp:RadioButton ID="rbtnFanId" runat="server" GroupName="FanId" /></td>
                            <td data-title="<%# AvailableFanIdLabel %>"><asp:Label ID="lblAvailableFanId" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "AvailableFanId") %>' /></td>
                            <td data-title="<%# FanIdLastFourNumbersLabel %>"><asp:Label ID="lblAvailable4digits" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "LastFourCardDigits")%>' /></td>
                            <td data-title="<%# FanIdIssueNumberLabel %>"><asp:Label ID="lblAvailableIssueNumber" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CardIssueNumber") %>' /></td>
                        </tr>
                    </asp:PlaceHolder>
                </ItemTemplate>
                <FooterTemplate></table></FooterTemplate>
            </asp:Repeater>
        </fieldset>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhUpdateButton" runat="server">
        <div class="ebiz-update-fan-id">
            <asp:Button ID="btnUpdateFanId" runat="server" CssClass="button" />
        </div>
    </asp:PlaceHolder>

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>