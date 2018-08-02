<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CreditFinanceMandate.ascx.vb" Inherits="UserControls_CreditFinanceMandate" %>
<div class="row ebiz-cf-mandate-details">
    <div class="row">
        <div class="large-12 columns">
            <h2><asp:Literal ID="TitleLabel" runat="server" /></h2>
            <div class="row">
            
                <div class="large-6 columns ebiz-cf-mandate-column-1">
                    <div class="row ebiz-cf-address">
                        <div class="large-6 columns">
                            <asp:PlaceHolder ID="plhAddressLine1" runat="server"><p><asp:Literal ID="AddressLine1" runat="server" /></p></asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAddressLine2" runat="server"><p><asp:Literal ID="AddressLine2" runat="server" /></p></asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAddressLine3" runat="server"><p><asp:Literal ID="AddressLine3" runat="server" /></p></asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAddressLine4" runat="server"><p><asp:Literal ID="AddressLine4" runat="server" /></p></asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAddressLine5" runat="server"><p><asp:Literal ID="AddressLine5" runat="server" /></p></asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhAddressPostCode" runat="server"><p><asp:Literal ID="AddressPostCode" runat="server" /></p></asp:PlaceHolder>
                        </div>
                    </div>
                    <div class="row ebiz-cf-years-at-address">
                        <div class="large-3 columns">
                            <asp:Label ID="AddressYearsLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Literal ID="AddressYearsText" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-account-name">
                        <div class="large-3 columns">
                            <asp:Label ID="AccountNameLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Literal ID="AccountName" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-account-number">
                        <div class="large-3 columns">
                            <asp:Label ID="AccountNumberLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Label ID="AccountNumber0" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber1" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber2" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber3" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber4" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber5" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber6" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="AccountNumber7" runat="server" CssClass="ebiz-dd-number-box" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-sort-code">
                        <div class="large-3 columns">
                            <asp:Label ID="SortCodeLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Label ID="SortCode0" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="SortCode1" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="SortCode2" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="SortCode3" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="SortCode4" runat="server" CssClass="ebiz-dd-number-box" />
                            <asp:Label ID="SortCode5" runat="server" CssClass="ebiz-dd-number-box" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-bank-name">
                        <div class="large-3 columns">
                            <asp:Label ID="BankNameLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Label ID="BankName" runat="server" CssClass="ebiz-dd-bankname-box" />
                        </div>
                    </div>
                </div>

                <div class="large-6 columns ebiz-cf-mandate-column-2">
                    <div class="row ebiz-cf-image">
                        <div class="large-6 columns">
                            <asp:Image ID="CreditFinanceImage" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-instructions-title">
                        <div class="large-6 columns">
                            <asp:Literal ID="InstructionTitleLabel" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-installment-plan">
                        <div class="large-3 columns">
                            <asp:Label ID="InstallmentPlanLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Label ID="InstallmentPlanText" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-cf-installment-plan-example">
                        <div class="large-3 columns">
                            <asp:Label ID="InstallmentPlanExampleLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Literal ID="InstallmentPlanExampleText" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-dd-instructions-text-label">
                        <div class="large-6 columns">
                            <asp:Literal ID="InstructionTextLabel" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-dd-instructions-text">
                        <div class="large-6 columns">
                            <asp:Literal ID="InstructionText" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-dd-date-label">
                        <div class="large-3 columns">
                            <asp:Label ID="TodaysDateLabel" runat="server" CssClass="middle" />
                        </div>
                        <div class="large-3 columns">
                            <asp:Literal ID="TodaysDate" runat="server" />
                        </div>
                    </div>
                    <div class="row ebiz-dd-addtional-text">
                        <div class="large-6 columns">
                            <asp:Literal ID="AdditionalText" runat="server" />
                        </div>
                    </div>
                </div>
                
            </div>
        </div>
    </div>
</div>