<%@ Control Language="VB" AutoEventWireup="false" CodeFile="TransactionEnquiry.ascx.vb" Inherits="UserControls_TransactionEnquiry" %>


    <script language="javascript" type="text/javascript">
        $(document).ready(function () { $(".datepicker").datepicker({ dateFormat: 'dd/mm/yy' }); });
    </script>

    <asp:PlaceHolder ID="plhError" runat="server">
        <asp:Label ID="lblError" runat="server" CssClass="alert-box alert ebiz-transaction-enquiry-error-wrap" />
    </asp:PlaceHolder>

    <div class="row">
        <div class="large-6 columns ebiz-transaction-enquiry-filter">
    <asp:Panel ID="pnlTransactionEnquiryFilter" runat="server" DefaultButton="filterButton" CssClass="panel">
        
        <h2>
            
                <asp:Label ID="InstructionsLabel" runat="server" />
            
        </h2>
        <asp:PlaceHolder ID="plhClubCode" runat="server">
            <div class="row ebiz-club-code">
                <div class="large-3 columns">
                    <asp:Label ID="lblClubCodeLabel" runat="server" AssociatedControlID="ddlClubCode" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlClubCode" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhPaymentReferenceFilter" runat="server" Visible="false">
            <div class="row ebiz-payment-reference-filter">
                <div class="large-3 columns">
                    <asp:Label ID="PayRefLabel" runat="server" AssociatedControlID="PaymentReference" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="PaymentReference" runat="server" MaxLength="15" />
                    <asp:RegularExpressionValidator ID="rgxPaymentReference" runat="server" ControlToValidate="PaymentReference"
                        Display="Static" CssClass="error" SetFocusOnError="true" ValidationExpression="^[0-9]{1,15}$" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="row ebiz-from-date-filter">
            <div class="large-3 columns">
                <asp:Label ID="FromDateLabel" runat="server" AssociatedControlID="FromDate" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="FromDate" CssClass="datepicker" runat="server" />
                <asp:CompareValidator id="cvdFromDate" CssClass="error" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="FromDate" />
                <asp:label ID="lblFromDateValue" runat="server" Visible="false" />
            </div>
        </div>
        <div class="row ebiz-to-date-filter">
            <div class="large-3 columns">
                <asp:Label ID="ToDateLabel" runat="server" AssociatedControlID="ToDate" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="ToDate" CssClass="datepicker" runat="server" />
                <asp:CompareValidator id="cvdToDate" CssClass="error" runat="server" Type="Date" Operator="DataTypeCheck" ControlToValidate="ToDate" />
                <asp:label ID="lblToDateValue" runat="server" Visible="false" />
            </div>
        </div>
        <asp:PlaceHolder ID="plhAgentFilter" runat="server">
            <div class="row ebiz-agent-filter">
                <div class="large-3 columns">
                    <asp:Label ID="AgentLabel" runat="server" AssociatedControlID="ddlAgent" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlAgent" runat="server">
                    </asp:DropDownList>
                    <asp:label ID="lblAgentValue" runat="server" Visible="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhCustomerFilter" runat="server">
            <div class="row ebiz-customer-filter">
                <div class="large-3 columns">
                    <asp:Label ID="CustomerLabel" runat="server" AssociatedControlID="txtCustomer" />
                </div>
                <div class="large-9 columns">
                    <div class="input-group">
                        <asp:TextBox ID="txtCustomer" runat="server" CssClass="input-group-field" />
                        <div class="input-group-button">
                            <asp:Button ID="btnCustomerSelect" runat="server" CssClass="button postfix" />
                        </div>
                    </div>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="ebiz-transaction-enquiry-button-wrap">
            <asp:Button ID="filterButton" CssClass="button" runat="server" />
        </div>

    </asp:Panel>
    </div>
    <div class="large-6 columns ebiz-transaction-enquiry-search">

    <asp:Panel ID="pnlTransactionEnquirySearch" runat="server" Visible="false" DefaultButton="btnSearch" CssClass="panel">
        <h2>
            <asp:Literal ID="ltlTranEnqSearchTitle" runat="server" />
        </h2>
        <asp:PlaceHolder ID="plhPaymentReferenceSearch" Visible="true" runat="server">
            <div class="row ebiz-payment-reference">
                <div class="large-3 columns">
                    <asp:Label ID="lblPayRefSearch" runat="server" AssociatedControlID="txtPaymentReferenceSearch" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtPaymentReferenceSearch" runat="server" MaxLength="15" ValidationGroup="TrxEnqSearchGroup" />
                    <asp:RegularExpressionValidator ID="rgxPaymentReferenceSearch" runat="server" ControlToValidate="txtPaymentReferenceSearch"
                        Display="Static" CssClass="error" SetFocusOnError="true" ValidationExpression="^[0-9]{1,15}$" ValidationGroup="TrxEnqSearchGroup" />
                 </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhPackageIDSearch" runat="server">
        <div class="row ebiz-package-id">
            <div class="large-3 columns">
                <asp:Label ID="lblPackageIDSearch" runat="server" AssociatedControlID="txtPackageIDSearch" />
            </div>
            <div class="large-9 columns">
                <asp:TextBox ID="txtPackageIDSearch" runat="server" MaxLength="13" ValidationGroup="TrxEnqSearchGroup" />
                <asp:RegularExpressionValidator ID="rgxPackageIDSearch" runat="server" ControlToValidate="txtPackageIDSearch"
                    Display="Static" CssClass="error" SetFocusOnError="true" ValidationExpression="^[0-9]{1,13}$" ValidationGroup="TrxEnqSearchGroup" />
            </div>
        </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhCustomerNumberSearch" Visible="true" runat="server">
            <div class="row ebiz-customer-number-search">
                <div class="large-3 columns">
                    <asp:Label ID="lblCustomerNumberSearch" runat="server" AssociatedControlID="txtCustomerNumberSearch" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtCustomerNumberSearch" runat="server" MaxLength="12" ValidationGroup="TrxEnqSearchGroup" />
                    <asp:RegularExpressionValidator ID="rgxCustomerNumberSearch" runat="server" ControlToValidate="txtCustomerNumberSearch"
                    Display="Static" CssClass="error" SetFocusOnError="true" ValidationExpression="^[0-9]{1,12}$" ValidationGroup="TrxEnqSearchGroup"/>
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="ebiz-enquiry-search-button-wrap">
            <asp:Button ID="btnSearch" CssClass="button" runat="server" ValidationGroup="TrxEnqSearchGroup"/>
        </div>
    </asp:Panel>
    </div>
    </div>

<asp:PlaceHolder ID="plhEnquiryResults" runat="server" Visible="false">
    <div class="panel ebiz-enquiry-results-wrap">
        <asp:PlaceHolder ID="plhPagerTop" runat="server">
        <div class="ebiz-pager ebiz-pager-top">
            <div class="row">
                <div class="large-6 columns">
                    <p class="ebiz-pager-display">
                        <asp:Label ID="CurrentResultsDisplaying" runat="server" />
                    </p>
                </div>
                <div class="large-6 columns">
                    <ul class="pagination right">
                        <li><asp:LinkButton ID="FirstTop" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav1Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav2Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav3Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav4Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav5Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav6Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav7Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav8Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav9Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="Nav10Top" runat="server" OnClick="ChangePage" /></li>
                        <li><asp:LinkButton ID="LastTop" runat="server" OnClick="ChangePage" /></li>
                    </ul>
                </div>
            </div>
        </div>
        </asp:PlaceHolder>

        <asp:Repeater ID="TransactionHistoryView" runat="Server" Visible ="false">
            <HeaderTemplate>
                <table class="ebiz-responsive-table">
                    <thead>
                        <tr>
                        <th class="ebiz-payment-reference" scope="col">
                            <%# GetText("PaymentReferenceHeader")%>
                        </th>
                        <th class="ebiz-date" scope="col">
                            <%# GetText("DateHeader")%>
                        </th>
                        <th class="ebiz-member-number" scope="col">
                            <%# GetText("MemberNumberHeader")%>
                        </th>
                        <th class="ebiz-value" scope="col">
                            <%# GetText("ValueHeader")%>
                        </th>
                        <th class="ebiz-lines" scope="col">
                            <%# GetText("LinesHeader")%>
                        </th>
                        <th class="ebiz-payment-type" scope="col">
                            <%# GetText("PaymentTypeHeader")%>
                        </th>
                        <th class="ebiz-club-code" scope="col">
                            <%# GetText("ClubCodeHeader")%>
                        </th>
                        <th class="ebiz-package" scope="col">
                            <%# GetText("PackageId")%>
                        </th>
                        <th class="ebiz-action" scope="col">
                            <%# GetText("ViewDetailsLinkHeader")%>
                        </th>
                    </tr>
                </thead>
                <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr>
                        <td class="ebiz-payment-reference" data-title='<%#getText("PaymentReferenceHeader") %>'>
                            <asp:Label ID="PaymentReference" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("PaymentReference")).TrimStart(GlobalConstants.LEADING_ZEROS)%>' />
                        </td>
                        <td class="ebiz-date" data-title='<%#getText("DateHeader") %>'>
                            <asp:Label ID="Date" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("Date")) %>' />
                        </td>
                        <td class="ebiz-member-number" data-title='<%#getText("MemberNumberHeader") %>'>
                            <asp:Label ID="MemberNumber" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("Member")).TrimStart(GlobalConstants.LEADING_ZEROS)%>' />
                        </td>
                        <td class="ebiz-value" data-title='<%#getText("ValueHeader") %>'>
                            <asp:Label ID="Value" runat="server" Text='<%# FormatCurrency(Talent.eCommerce.Utilities.RoundToValue(Talent.eCommerce.Utilities.CheckForDBNull_Decimal(Container.DataItem("Value")), 0.01, False))%>' />
                            <asp:Label ID="hdValue" Visible="false" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_Decimal(Container.DataItem("Value")) %>' />
                        </td>
                        <td class="ebiz-lines" data-title='<%#getText("LinesHeader") %>'>
                            <asp:Label ID="Lines" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("NumberOfItems")).TrimStart(GlobalConstants.LEADING_ZEROS) %>' />
                        </td>
                        <td class="ebiz-payment-type" data-title='<%#getText("PaymentTypeHeader") %>'>
                            <asp:Label ID="PaymentType" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("PaymentType")) %>' />
                        </td>
                        <td class="ebiz-club-code" data-title='<%#getText("ClubCodeHeader") %>'>
                            <asp:Label ID="ClubCode" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("ClubCode")) %>' />
                        </td>
                        <td class="ebiz-package" data-title='<%#getText("PackageId") %>'>
                            <asp:Label ID="lblPackageId" runat="server" Text='<%# Talent.eCommerce.Utilities.CheckForDBNull_String(Container.DataItem("CallId")) %>' />
                        </td>
                        <td class="ebiz-action" data-title='<%#getText("ViewDetailsLinkHeader") %>'>
                            <asp:HyperLink ID="hplViewDetails" runat="server" CssClass="ebiz-view-details" data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="View Details"><i class="fa fa-info"></i></asp:HyperLink>
                            <asp:HyperLink ID="hplViewDespatchDetails" runat="server" CssClass="ebiz-view-purchase-details" data-tooltip aria-haspopup="true" class="has-tip" data-disable-hover="false" title="View Despatch Details"><i class="fa fa-chevron-right"></i></asp:HyperLink>
                        </td>
                    </tr>
            </ItemTemplate>
            <FooterTemplate>
            </tbody>
                </table>
            </FooterTemplate>
        </asp:Repeater>

        <asp:PlaceHolder ID="plhPagerBottom" runat="server">
        <div class="ebiz-pager ebiz-pager-bottom">
            <ul class="pagination right">
                <li><asp:LinkButton ID="FirstBottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav1Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav2Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav3Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav4Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav5Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav6Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav7Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav8Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav9Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="Nav10Bottom" runat="server" OnClick="ChangePage" /></li>
                <li><asp:LinkButton ID="LastBottom" runat="server" OnClick="ChangePage" /></li>
            </ul>
        </div>
        </asp:PlaceHolder>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhTransactionSummary" runat="server" Visible="false">

        <asp:Repeater ID="rptTransactionSummary" runat="server" Visible="false">
            <HeaderTemplate>
                <div class="panel ebiz-transaction-summary-wrap">
                    <table class="ebiz-responsive-table">
                    <thead>
                        <tr>
                            <th class="ebiz-paytype" scope="col"><asp:Literal ID="ltlTrnxSumPayTypeLabel" runat="server" /></th>
                            <th class="ebiz-payamount" scope="col"><asp:Literal ID="ltlTrnxSumPayAmountLabel" runat="server" /></th>
                        </tr>
                    </thead>
                    <tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td data-title="<%#getText("TrnxSummaryPayTypeLabel") %>" class="ebiz-paytype"><asp:Literal ID="ltlTrnxSumPayTypeValue" runat="server" /></td>
                    <td data-title="<%#getText("TrnxSummaryPayAmountLabel") %>" class="ebiz-payamount"><asp:Literal ID="ltlTrnxSumPayAmountValue" runat="server" /></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate>
                    <tr>
                        <td data-title="<%#getText("TrnxSummaryPayTypeLabel") %>" class="ebiz-paytype"><asp:Literal ID="ltlTrnxSumTotalLabel" runat="server" /></td>
                        <td data-title="<%#getText("TrnxSummaryPayAmountLabel") %>" class="ebiz-payamount"><asp:Literal ID="ltlTrnxSumTotalValue" runat="server" /></td>
                    </tr>
                    </tbody>
                    </table>
                </div>
            </FooterTemplate>
        </asp:Repeater>

</asp:PlaceHolder>

<asp:PlaceHolder ID="plhNoPaymentDetails" runat="server" Visible="false">
    <div class="alert-box warning ebiz-no-payment-details"><asp:Label ID="ltlNoPaymentDetails" runat="server" /></div>
</asp:PlaceHolder>