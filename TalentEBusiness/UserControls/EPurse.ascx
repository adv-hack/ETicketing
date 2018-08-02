<%@ Control Language="VB" AutoEventWireup="false" CodeFile="EPurse.ascx.vb" Inherits="UserControls_EPurse" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp1" %>

<asp:PlaceHolder ID="plhEPurse" runat="server" Visible="false">
    <asp:HiddenField ID="hdfDisplay" runat="server" Value="false" />
    <asp:PlaceHolder ID="plhEPFirst" runat="server" Visible="True">
        <div class="panel ebiz-checkout-payment-details-EP-First">
            <div class="row">
                <div class="large-12 columns">
                    <asp:PlaceHolder ID="plhManualEnter" runat="server" Visible="True">
                        <asp:Panel ID="panelItems" runat="server" CssClass="row ebiz-smartcard-list">
                            <div class="large-12 columns">
                                <asp:Repeater ID="rptSmartcards" runat="server">
                                    <HeaderTemplate>
                                        <ul id="epForm_blSmartcards">
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <li><asp:LinkButton ID="lbtnSmartcardListItem" runat="server" /></li>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </ul>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </asp:Panel>
                        <div class="row ebiz-teamcard-number">
                            <div class="large-3 columns">
                                <label class="middle" for='<%=txtEPurseCard.ClientID %>'><asp:Literal ID="ltlEPurseCard" runat="server" /></label>
                            </div>
                            <div class="large-9 columns">
                                <asp:TextBox ID="txtEPurseCard" runat="server" />
                                <asp1:dropdownextender id="ddeEPurseCard" runat="server" dynamicservicepath="" enabled="True"
                                    dropdowncontrolid="panelItems" targetcontrolid="txtEPurseCard" />
                                <asp:RequiredFieldValidator ID="rfvEPurseCard" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                                    ValidationGroup="Checkout" ControlToValidate="txtEPurseCard" />
                                <asp:RegularExpressionValidator ID="regExEPurseCard" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                                    ValidationGroup="Checkout" ControlToValidate="txtEPurseCard" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDDLSelect" runat="server" Visible="True">
                        <div class="row ebiz-teamcard-number">
                            <div class="large-3 columns">
                                <asp:Label ID="ltlEPurseCard1" runat="server" AssociatedControlID="ddlSelectCard" />
                            </div>
                            <div class="large-9 columns">
                                <asp:DropDownList ID="ddlSelectCard" runat="server" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:Button ID="btnRetrieveBalance" runat="server" CssClass="button"/>
                </div>
            </div>
        </div>
    </asp:PlaceHolder>

    <asp:Placeholder ID="plhGiftCard" runat="server" Visible="false">
        <div class="row ebiz-gift-card">
            <div class="medium-3 columns">
                <asp:Label ID="lblGiftCard" runat="server" Text="Gift Card" AssociatedControlID="txtGiftCard"></asp:Label>
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtGiftCard" runat="server" />
            </div>
        </div>
        <div class="row ebiz-pin">
            <div class="medium-3 columns">
                <asp:Label ID="lblPin" runat="server" Text="Pin" AssociatedControlID="txtPin"></asp:Label>
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtPin" runat="server" />
            </div>
        </div>
        <div class="ebiz-retrieve-gift-card-balance-wrap">
            <asp:Button ID="btnRetrieveGiftCardBalance" runat="server" CssClass="button"  Text="Retrieve Balance"  />
        </div>
    </asp:Placeholder>

    <asp:PlaceHolder ID="plhEPSecond" runat="server" Visible="False">
        <div class="panel ebiz-checkout-payment-details-EP-Second">
            <div class="row ebiz-part-payment-first">
                <div class="large-3 columns ebiz-e-purse-summary-title"><asp:Literal ID="ltlEPurseSummaryTitle" runat="server" /></div>
                <div class="large-9 columns ebiz-e-purse-description"><asp:Literal ID="ltlEPurseDescription" runat="server" /></div>
            </div>
            <div class="row ebiz-teamcard-number">
                <div class="large-3 columns"><asp:Literal ID="ltlEPurseCard2" runat="server" /></div>
                <div class="large-9 columns"><asp:Literal ID="ltlEPurseCardValue" runat="server" /></div>
            </div>
            <div class="row ebiz-total-available">
                <div class="large-3 columns"><asp:Literal ID="ltlTotalAvailable" runat="server" /></div>
                <div class="large-9 columns"><asp:Literal ID="ltlTotalAvailableValue" runat="server" /></div>
                <asp:HiddenField ID="hdfTotalAvailableValue" runat="server" />
            </div>
            <div class="row ebiz-teamcard-points">
                <div class="large-3 columns"><asp:Literal ID="ltlTotalPoints" runat="server" /></div>
                <div class="large-9 columns"><asp:Literal ID="ltlTotalPointsValue" runat="server" /></div>
            </div>

            <asp:PlaceHolder ID="plhNextCard" runat="server">
                <asp:Button ID="btnNextCard" runat="server" CssClass="button" />
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhPartPayment" runat="server" Visible="True">
                <div class="ebiz-part-payment-second">
                    <div class="row part-payment-title">
                        <div class="large-12 columns">
                            <asp:Literal ID="ltlPartPaymentTitle" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-part-payment-description">
                        <div class="large-12 columns">
                            <asp:Literal ID="ltlPartPaymentDescription" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-spend-points">
                        <div class="large-3 columns">
                            <asp:Label ID="ltlPaymentTotal" runat="server" AssociatedControlID="txtPaymentAmount" />
                        </div>
                        <div class="large-9 columns">
                            <asp:TextBox ID="txtPaymentAmount" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvPaymentAmount" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                                ValidationGroup="Checkout" ControlToValidate="txtPaymentAmount" />
                            <asp:RegularExpressionValidator ID="regExPaymentAmount" runat="server"  Display="Static" CssClass="error" SetFocusOnError="true"
                                ValidationGroup="Checkout" ControlToValidate="txtPaymentAmount" />
                        </div>
                    </div>
                    <asp:HiddenField ID="hdfEPursePWPoints" runat="server" />
                    <asp:HiddenField ID="hdfEPursePWValue" runat="server" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhNoBalance" runat="server" Visible="false">
        <div class="panel ebiz-no-balance">
            <div class="ebiz-epurse-no-balance"><asp:Literal ID="ltlNoBalance" runat="server" /></div>
            <div class="ebiz-epurse-no-balance-link"><asp:HyperLink ID="hplNoBalance" runat="server" /></div>
        </div>
    </asp:PlaceHolder>

</asp:PlaceHolder>