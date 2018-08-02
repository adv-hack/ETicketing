<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CustomerSelection.aspx.vb" Inherits="PagesPublic_Profile_CustomerSelection" Title="Untitled Page" %>
<%@ Register Src="~/UserControls/CustomerSearch.ascx" TagPrefix="Talent" TagName="CustomerSearch" %>
<%@ Register Src="~/UserControls/CompanySearch.ascx" TagPrefix="Talent" TagName="CompanySearch" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <asp:PlaceHolder ID="plhPageHeader" runat="server" Visible="false">
        <h1>
               <%=CustomerSelectionSearchHeader%>
        </h1>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorList" runat="server"  visible="False">
        <div class="alert-box alert">
            <asp:BulletedList ID="errorList" runat="server" ClientIDMode="Static" />
        </div>
    </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhSuccessList" runat="server" visible="False">
        <div class="alert-box success">
            <asp:BulletedList ID="successList" runat="server" />
        </div>
    </asp:PlaceHolder>
    <div class="alert-box alert" id ="clientside-errors-wrapper">
        <ul id="clientside-errors"></ul>
    </div>
    <%-- Single customer selection panel --%>
    <asp:Panel ID="pnlSingleCustomerSelection" runat="server" DefaultButton="btnSelectCustomer">
       
        <div class="panel customer-selection-search-wrap">
            <h2><%=CustomerSelectionSearchHeader%></h2>
            <div class="row customer-selection-search">

                <asp:PlaceHolder ID="plhClubDDL" runat="server">
                    <div class="column customer-selection-search-item ebiz-club">
                        <asp:Label ID="clubLabel" runat="server" AssociatedControlID="clubDDL" />
                        <asp:DropDownList ID="clubDDL" runat="server" AutoPostBack="true" />
                    </div>
                </asp:PlaceHolder>
                <div class="column customer-selection-search-item ebiz-customer-number">
                    <asp:Label ID="fancardLabel" runat="server" AssociatedControlID="fancardBox" />
                    <asp:TextBox ID="fancardBox" runat="server" MaxLength="12" Type="number" />
                </div>
                <asp:PlaceHolder ID="plhMembershipNumber" runat="server">
                    <div class="column customer-selection-search-item ebiz-membership-number">
                        <asp:Label ID="lblMembershipNumber" runat="server" AssociatedControlID="txtMembershipNumber" />
                        <asp:TextBox ID="txtMembershipNumber" runat="server" ClientIDMode="Static"/>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhPaymentReference" runat="server">
                    <div class="column customer-selection-search-item ebiz-payment-reference">
                        <asp:Label ID="lblPaymentReference" runat="server" AssociatedControlID="txtPaymentReference" />
                        <asp:TextBox ID="txtPaymentReference" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhCorporateSaleID" runat="server">
                    <div class="column customer-selection-search-item ebiz-corporate-sale-id">
                        <asp:Label ID="lblCorporateSaleID" runat="server" AssociatedControlID="txtCorporateSaleID" />
                        <asp:TextBox ID="txtCorporateSaleID" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhSubmitButton" runat="server">
                    <div class="column customer-selection-search-item ebiz-action">
                        <asp:Button ID="btnSelectCustomer" runat="server" CssClass="button ebiz-primary-action" />
                    </div>
                </asp:PlaceHolder>
            </div>

            <asp:PlaceHolder ID="plhLastNCustomerLogins" runat="server">
                <div class="row ebiz-last-customer-logins-wrap">
	                <div class="column ebiz-customer-logins">
		                <asp:Label ID="lblLastNCustomersUsed" runat="server" />
	                </div>
	                <div class="column ebiz-customer-logins-list">
		                <asp:Repeater ID="rptLastNCustomerLogins" runat="server">
			                <HeaderTemplate>
				                <div class="row">
			                </HeaderTemplate>
			                <ItemTemplate>
				                <div class="column ebiz-last-customer-login-item"><asp:HyperLink ID="hplLastCustomerLogin" runat="server" /></div>
			                </ItemTemplate>
			                <FooterTemplate>
				                </div>
			                </FooterTemplate>
		                </asp:Repeater>
	                </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddCustomer" runat="server" Visible="false">
                <div class="ebiz-add-customer-wrap">
                    <asp:HyperLink ID="hplAddCustomer" runat="server" CssClass="button ebiz-add-customer" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <asp:HiddenField ClientIDMode="Static" ID="hdfTalentAPIAddress" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchLimit" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfAgentLoginID" runat="server" />  
    <asp:HiddenField ClientIDMode="Static" ID="hdfAgentType" runat="server" />  
    <asp:HiddenField ClientIDMode="Static" ID="hdfPrintAddressLabelItem" runat="server" /> 
    <asp:HiddenField ClientIDMode="Static" ID="hdfQueryStringValidation" runat="server" /> 
    <asp:HiddenField ClientIDMode="Static" ID="hdfRootURL" runat="server" /> 
    <asp:HiddenField ClientIDMode="Static" ID="hdfClubBusinessUnit" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfPerformAgentWatchListCheck" runat="server" />
    
    <asp:HiddenField ClientIDMode="Static" ID="hdfBasketDetailID" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfProductType" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfProductCode" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfProductSubType" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfPackageId" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfPriceCode" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfPriceBand" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfFulfilmentMethod" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfSeat" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfBulkSalesId" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfBulkSalesQuantity" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfReturnURL" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfOriginalUser" runat="server" />

    <asp:HiddenField ClientIDMode="Static" ID="hdfNewCustomer" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfShowCompanySearch" runat="server" Value=""/>
    <asp:HiddenField ClientIDMode="Static" ID="hdfShowParentCompanySearch" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfShowSubsidaries" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfAddCompanyToNull" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfAddSubsidiaries" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfParentCompanyNumber" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfParentCompanyName" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanyNumber" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfSearchType" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfColumnDisplay" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfReturnToAddress" runat="server" Value="" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfSessionID" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanySearchChangePageSize" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanySearchPageSize" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanySearchChangePageSizeSelection" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanySearchLengthMenuText" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCompanySearchNonSortableColumnArray" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchChangePageSize" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchPageSize" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchChangePageSizeSelection" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchLengthMenuText" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchNonSortableColumnArray" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfNoSearchCriteria" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityName" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityAddress" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityPostcode" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityPhoneNumber" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityDOB" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityMembershipNo" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityPassport" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfCustomerSearchCustomerColumnVisibilityEmail" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfRecordsFiltered" runat="server" />    
    <asp:HiddenField ClientIDMode="Static" ID="hdfChildCompanyNumber" runat="server" />  
    
    <ul class="tabs ebiz-customer-selection-tabs" data-tabs id="search-tabs">
        <li class="tabs-title customer-selection"><a href="#customer" aria-selected="true"><%=CustomerSearchTabTitle%></a></li>
        <li class="tabs-title company-allocation"><a href="#company"><%=CompanySearchTabTitle%></a></li>
    </ul>
    <div class="tabs-content ebiz-customer-search-tabs" data-tabs-content="search-tabs">
      <div class="tabs-panel" id="customer">
        <Talent:CustomerSearch runat="server" id="uscCustomerSearch" />
      </div>
      <div class="tabs-panel" id="company">
        <Talent:CompanySearch runat="server" ID="uscCompanySearch" />
      </div>
    </div>

    <div class="panel ebiz-customer-selection-results-wrap">
        <table class="display ebiz-customer-search-results ebiz-responsive-table">
            <thead>
                <tr>
                    <th scope="col" class="ebiz-customer"><%=CustomerNumberHeader%></th> 
                    <th scope="col" class="ebiz-name"><%=CustomerNameHeader%></th>
                    <th scope="col" class="ebiz-address"><%=AddressHeader%></th>
                    <th scope="col" class="ebiz-post-code"><%=PostCodeHeader%></th>
                    <th scope="col" class="ebiz-telephone"><%=TelephoneHeader%></th>
                    <th scope="col" class="ebiz-dob"><%=DateOfBirthHeader%></th>
                    <th scope="col" class="ebiz-membership"><%=MembershipHeader%></th>
                    <th scope="col" class="ebiz-passport-number"><%=PassportNumber%></th>
                    <th scope="col" class="ebiz-email"><%=EMailAddressHeader%></th>
                    <th scope="col" class="ebiz-update"><%=UpdateLinkHeader%></th>
                    <th scope="col" class="ebiz-print"><%=PrintAddressLabelHeader%></th>
                </tr>
            </thead>
        </table>

        <table class="display ebiz-company-search-results ebiz-responsive-table">
            <thead>
                <tr>
                    <th scope="col" class="ebiz-company-name"><%=CompanyNameHeader%></th> 
                    <th scope="col" class="ebiz-address-line1"><%=AddressHeader%></th>
                    <th scope="col" class="ebiz-post-code"><%=PostCodeHeader%></th>
                    <th scope="col" class="ebiz-telephone"><%=TelephoneHeader%></th>
                    <th scope="col" class="ebiz-email"><%=EMailAddressHeader%></th>
                    <th scope="col" class="ebiz-update column-show-update"><%=UpdateHeader%></th>
                    <th scope="col" class="ebiz-Contact column-show-contacts"><%=ContactsHeader%></th>
                    <th scope="col" class="ebiz-Select column-show-select"><%=SelectHeader%></th>
                </tr>
            </thead>     
        </table>
    </div>
    <script type="text/javascript" src="../../JavaScript/Module/Customer/customer-selection.js"></script>

</asp:content>

