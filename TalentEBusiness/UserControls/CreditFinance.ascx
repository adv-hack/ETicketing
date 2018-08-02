<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CreditFinance.ascx.vb" Inherits="UserControls_CreditFinance" %>

<asp:PlaceHolder ID="pnlCreditFinanceExample" runat="server" Visible="false">
    <div class="ebiz-cf-details-intro panel">
        <h2>
            <asp:Literal ID="ExampleTitleLabel" runat="server" /></h2>
        <p class="instructions">
            <asp:Literal ID="lblIntroduction" runat="server" />
        </p>
        <asp:Repeater ID="repExample" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="row ebiz-cf-details-example">
                    <div class="large-12 columns">
                        <asp:Literal ID="lblExample" runat="server" />
                    </div>
                </div>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="pnlCreditFinance" runat="server" Visible="false">
    <div class="ebiz-cf-payment-details panel">
        <h2>
            <asp:Literal ID="TitleLabel" runat="server" /></h2>
        <div class="row ebiz-cf-account-name">
            <div class="large-3 columns">
                <asp:Label ID="AccountNameLabel" runat="server" AssociatedControlID="AccountName" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="AccountName" runat="server" />
                <asp:RequiredFieldValidator ID="AccountNameRFV" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="AccountName" />
                <asp:RegularExpressionValidator ID="AccountNameRegEx" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="AccountName" />
            </div>
        </div>
        <div class="row ebiz-cf-sortcode">
            <div class="large-3 columns">
                <asp:Label ID="SortCodeLabel" runat="server" AssociatedControlID="SortCode1" CssClass="middle" />
            </div>
            <div class="large-3 columns">
                <asp:TextBox ID="SortCode1" runat="server" MaxLength="2" CssClass="sort-code-1" />
            </div>
            <div class="large-3 columns">
                <asp:TextBox ID="SortCode2" runat="server" MaxLength="2" CssClass="sort-code-2" />
            </div>
            <div class="large-3 columns">
                <asp:TextBox ID="SortCode3" runat="server" MaxLength="2" CssClass="sort-code-3" />
            </div>
            <div class="large-9 large-offset-3 columns">
                <asp:RegularExpressionValidator ID="SortCode1RegEx" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="SortCode1" />
                <asp:RegularExpressionValidator ID="SortCode2RegEx" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="SortCode2" />
                <asp:RegularExpressionValidator ID="SortCode3RegEx" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="SortCode3" />
                <asp:CustomValidator ID="csvSortCode" runat="server" ValidationGroup="Checkout" SetFocusOnError="true" Display="Static" CssClass="error"
                    OnServerValidate="ValidateSortCode" ClientValidationFunction="ValidateSortCode" EnableClientScript="true" />
            </div>
        </div>
        <div class="row ebiz-cf-account-number">
            <div class="large-3 columns">
                <asp:Label ID="AccountNumberLabel" runat="server" AssociatedControlID="AccountNumber" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="AccountNumber" runat="server" MaxLength="8" />
                <asp:RequiredFieldValidator ID="AccountNumberRFV" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="AccountNumber" />
                <asp:RegularExpressionValidator ID="AccountNumberRegEx" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="AccountNumber" />
            </div>
        </div>
        <div class="row ebiz-cf-installment-plan">
            <div class="large-3 columns">
                <asp:Label ID="InstallmentPlanLabel" runat="server" AssociatedControlID="InstallmentPlanDDL" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:DropDownList ID="InstallmentPlanDDL" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-current-details">
            <div class="large-12 columns">
                <asp:Localize ID="LocCurrentDetails" runat="server" />
            </div>
        </div>
        <div class="row ebiz-cf-customer-name">
            <div class="large-3 columns">
                <asp:Label ID="CustomerNameLabel" runat="server" AssociatedControlID="CustomerName" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="CustomerName" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="row ebiz-cf-mobile-number">
            <div class="large-3 columns">
                <asp:Label ID="MobileLabel" runat="server" AssociatedControlID="Mobile" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="Mobile" runat="server" ReadOnly="True" Enabled="False" />
                <asp:RequiredFieldValidator ID="MobileRFV" runat="server" Display="Static" CssClass="error" SetFocusOnError="true"
                    ValidationGroup="Checkout" ControlToValidate="Mobile" />
            </div>
        </div>
        <div class="row ebiz-cf-email">
            <div class="large-3 columns">
                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="Email" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="row ebiz-cf-address-line1">
            <div class="large-3 columns">
                <asp:Label ID="Address1Label" runat="server" AssociatedControlID="Address1" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="Address1" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="row ebiz-cf-address-line2">
            <div class="large-3 columns">
                <asp:Label ID="Address2Label" runat="server" AssociatedControlID="Address2" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="Address2" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="row ebiz-cf-address-line3">
            <div class="large-3 columns">
                <asp:Label ID="TownLabel" runat="server" AssociatedControlID="Town" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="Town" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>
        <div class="row ebiz-cf-address-line4">
            <div class="large-3 columns">
                <asp:Label ID="CountyLabel" runat="server" AssociatedControlID="County" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="County" runat="server" ReadOnly="True" Enabled="False" />
                <asp:RequiredFieldValidator ID="CountyRFV" runat="server" Display="Static" CssClass="error"
                    ValidationGroup="Checkout" ControlToValidate="County"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row ebiz-cf-address-line5">
            <div class="large-3 columns">
                <asp:Label ID="PostCodeLabel" runat="server" AssociatedControlID="PostCode" CssClass="middle" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="PostCode" runat="server" ReadOnly="True" Enabled="False" />
            </div>
        </div>


        <asp:PlaceHolder ID="PLHV12Fields" runat="server">
            <div class="row ebiz-cf-Noofyears">
                <div class="large-3 columns">
                    <asp:Label ID="NoOfYearsLabel" runat="server" AssociatedControlID="ddlNoOfYears" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlNoOfYears" runat="server" />
                    <asp:RequiredFieldValidator ID="RFVNoOfYears" runat="server" ControlToValidate="ddlNoOfYears" ValidationGroup="Checkout" InitialValue=" " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                </div>
            </div>

            <div class="row ebiz-cf-Noofmonths">
                <div class="large-3 columns">
                    <asp:Label ID="NoofmonthsLabel" runat="server" AssociatedControlID="ddlNoofmonths" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlNoofmonths" runat="server" />
                    <asp:RequiredFieldValidator ID="RVFNoofmonths" runat="server" ControlToValidate="ddlNoofmonths" ValidationGroup="Checkout" InitialValue=" " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                </div>
            </div>

            <div class="row ebiz-cf-HomeStatus">
                <div class="large-3 columns">
                    <asp:Label ID="HomeStatusLabel" runat="server" AssociatedControlID="ddlHomeStatus" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlHomeStatus" runat="server" />
                    <asp:RequiredFieldValidator ID="RVFHomeStatus" runat="server" ControlToValidate="ddlHomeStatus" ValidationGroup="Checkout" InitialValue=" " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />

                </div>
            </div>

            <div class="row ebiz-cf-Employment">
                <div class="large-3 columns">
                    <asp:Label ID="EmploymentLabel" runat="server" AssociatedControlID="ddlEmployment" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlEmployment" runat="server" />
                    <asp:RequiredFieldValidator ID="RVFEmployment" runat="server" ControlToValidate="ddlEmployment" ValidationGroup="Checkout" InitialValue=" " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                </div>
            </div>

            <div class="row ebiz-cf-Income">
                <div class="large-3 columns">
                    <asp:Label ID="IncomeLabel" runat="server" AssociatedControlID="ddlIncome" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlIncome" runat="server" />
                    <asp:RequiredFieldValidator ID="RVFIncome" runat="server" ControlToValidate="ddlIncome" ValidationGroup="Checkout" InitialValue=" " Display="Static" CssClass="error ebiz-validator-error" SetFocusOnError="true" />
                </div>
            </div>

        </asp:PlaceHolder>

    </div>
</asp:PlaceHolder>
