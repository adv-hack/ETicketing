<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CompanyUpdate.aspx.vb" Inherits="PagesPublic_Company_CompanyUpdate" title="Untitled Page" ViewStateMode="Disabled" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:PlaceHolder ID="plhErrorList" runat="server" visible="False">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorList" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhSuccess" runat="server" visible="False">
        <div class="alert-box success">
            <asp:Literal ID="ltlSuccess" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhWarningList" runat="server" visible="False">
        <div class="alert-box warning">
            <asp:Literal ID="lblWarning" runat="server" />
        </div>
    </asp:PlaceHolder>
    <div class="button-group ebiz-company-actions-wrap">
        <asp:Hyperlink ID="hplBack" runat="server" CssClass="button ebiz-primary-action ebiz-back" Visible="false" />
        <asp:Hyperlink ID="hplAdd" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible="false"/>
        <asp:Hyperlink ID="hplSearch" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible="false"/>
        <asp:Hyperlink ID="hplContacts" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible="false"/>
        <asp:Hyperlink ID="hplSubsidiary" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible="false"/>
        <asp:Hyperlink ID="hplAddSubsidiary" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible="false"/>
    </div>
    <asp:PlaceHolder ID="plhAddSubsidiary" runat="server" visible="False">
        <div class="panel ebiz-add-subsidiary-wrap">
            <span><asp:Literal ID="ltlAddSubsidiaryHeader" runat="server" /></span><asp:Literal ID="ltlAddSubsidiaryName" runat="server" />
        </div>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhCompanyDetailsForm" runat="server" visible="True">
        <asp:PlaceHolder ID="plhParentCompany" runat="server" visible="True">
            <div class="panel ebiz-parent-customer-company-wrap">
                <h2>
                    <span><asp:Literal ID="ltlParentCompanyHeader" runat="server" /></span> <span><asp:Literal ID="ltlParentCompanyName" runat="server" /></span>
                </h2>
                <div class="button-group ebiz-cutomer-company-actions-wrap">
                    <asp:Hyperlink ID="hplParentAdd" runat="server" Visible="false" CssClass="button" />
                    <asp:Hyperlink ID="hplParentChange" runat="server" Visible="false" CssClass="button" />
                    <asp:Hyperlink ID="hplParentSearch" runat="server" Visible="false" CssClass="button" />       
                    <asp:Button ID="btnParentRemove" runat ="server"  CssClass="button" Visible="false"/>
                    <asp:Hyperlink ID="hplParentSubsidiaries" runat="server" Visible="false" CssClass="button" />
                    <asp:Hyperlink ID="hplParentDetails" runat="server" Visible="false" CssClass="button" />  
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="panel ebiz-company-details">
            <h2>
                <asp:Literal ID="ltlCompanyDetailsFormHeader" runat="server" />
            </h2>
            <asp:PlaceHolder ID="plhCompanyName" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-company-name">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlCompanyName" AssociatedControlID="txtCompanyName" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtCompanyName" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtCompanyName" ID="rfvCompanyName" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtCompanyName" ID="rgxCompanyName" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>   
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhWebAddress" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-web-address ebiz-contact-preference">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlWebAddress" AssociatedControlID="txtWebAddress" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtWebAddress" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvWebAddress" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error"  ControlToValidate="txtWebAddress"/>
                        <asp:RegularExpressionValidator ControlToValidate="txtWebAddress" ID="rgxWebAddress" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                    <div class="medium-9 large-offset-3 columns">
                        <asp:Checkbox ID="ckContactByWebAddress" runat="server" />
                        <asp:Label ID="lblContactByWebAddress" AssociatedControlID="ckContactByWebAddress" runat="server" />
                    </div>
                </div>
              </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhTelephoneNumber1" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-telephone-number-1 ebiz-contact-preference">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlTelephoneNumber1" AssociatedControlID="txtTelephoneNumber1" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtTelephoneNumber1" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtTelephoneNumber1" ID="rfvTelephoneNumber1" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtTelephoneNumber1" ID="rgxTelephoneNumber1" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                    <div class="medium-9 large-offset-3 columns">
                        <asp:Checkbox ID="ckContactByTelephoneNumber1" runat="server" />
                        <asp:Label ID="lblContactByTelephoneNumber1" AssociatedControlID="ckContactByTelephoneNumber1" runat="server" /> 
                    </div>
                </div>
            </asp:PlaceHolder>
        
             <asp:PlaceHolder ID="plhTelephoneNumber2" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-telephone-number-2 ebiz-contact-preference">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlTelephoneNumber2" AssociatedControlID="txtTelephoneNumber2" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtTelephoneNumber2" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtTelephoneNumber2" ID="rfvTelephoneNumber2" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtTelephoneNumber2" ID="rgxTelephoneNumber2" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                    <div class="medium-9 large-offset-3 columns">
                        <asp:Checkbox ID="ckContactByTelephoneNumber2" runat="server" />
                        <asp:Label ID="lblContactByTelephoneNumber2" AssociatedControlID="ckContactByTelephoneNumber2" runat="server" />
                    </div>
                </div>
             </asp:PlaceHolder>
        
            <asp:PlaceHolder ID="plhTelephoneNumber3" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-telephone-number-3 ebiz-contact-preference">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlTelephoneNumber3" AssociatedControlID="txtTelephoneNumber3" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtTelephoneNumber3" runat="server" />
                         <asp:RequiredFieldValidator ControlToValidate="txtTelephoneNumber3" ID="rfvTelephoneNumber3" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtTelephoneNumber3" ID="rgxTelephoneNumber3" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                    <div class="medium-9 large-offset-3 columns">
                        <asp:Checkbox ID="ckContactByTelephoneNumber3" runat="server" />
                        <asp:Label ID="lblContactByTelephoneNumber3" AssociatedControlID="ckContactByTelephoneNumber3" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhVATCode" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-vat-codes">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlVATCode" AssociatedControlID="ddlVATCodes" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:DropDownList ID="ddlVATCodes" runat="server" ViewStateMode="Enabled"></asp:DropDownList>
                        <asp:RequiredFieldValidator ControlToValidate="ddlVATCodes" ID="rfvVATCode" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" Control />
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:PlaceHolder ID="plhSalesLedgerCode" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-sales-ledger-code">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlSalesLedgerCode" AssociatedControlID="txtSalesLedgerCode" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtSalesLedgerCode" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtSalesLedgerCode" ID="rfvSalesLedgerCode" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
        
            <asp:PlaceHolder ID="plhOwningAgent" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                 <div class="row ebiz-owning-agent">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlOwningAgent" AssociatedControlID="ddlOwningAgent" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:DropDownList ID="ddlOwningAgent" runat="server" ViewStateMode="Enabled" class="select2"> </asp:DropDownList>
                        <asp:RequiredFieldValidator ControlToValidate="ddlOwningAgent" ID="rfvOwningAgent" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" Control />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>



        <div class="panel ebiz-address-information">
            <h2 id="h1" runat="server">
                <asp:Literal ID="ltlCompanyAddressFormHeader" runat="server" />
            </h2>
            <% CreateAddressingJavascript()%>
            <% CreateAddressingHiddenFields()%>
            <asp:PlaceHolder ID="plhFindAddressButtonRow" runat="server">
                <div class="ebiz-find-address">
                    <a id="AddressingLinkButton" name="AddressingLinkButtton" href="Javascript:addressingPopup();"><%=GetAddressingLinkText()%></a>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine1" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-address-line-1">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlAddressLine1" AssociatedControlID="txtAddressLine1" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtAddressLine1" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtAddressLine1" ID="rfvAddressLine1" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtAddressLine1" ID="rgxAddressLine1" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine2" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">     
                 <div class="row ebiz-address-line-2">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlAddressLine2" AssociatedControlID="txtAddressLine2" runat="server" />                
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtAddressLine2" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtAddressLine2" ID="rfvAddressLine2" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtAddressLine2" ID="rgxAddressLine2" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div> 
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine3" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-address-line-3">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlAddressLine3" AssociatedControlID="txtAddressLine3" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtAddressLine3" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtAddressLine3" ID="rfvAddressLine3" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtAddressLine3" ID="rgxAddressLine3" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCounty" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                <div class="row ebiz-county">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlCounty" AssociatedControlID="txtCounty" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtCounty" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtCounty" ID="rfvCounty" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtCounty" ID="rgxCounty" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
             <asp:PlaceHolder ID="plhPostCode" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                 <div class="row ebiz-post-code">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlPostCode" AssociatedControlID="txtPostCode" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtPostCode" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtPostCode" ID="rfvPostCode" runat="server" OnPreRender="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="CompanyValidation"
                                Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                        <asp:RegularExpressionValidator ControlToValidate="txtPostCode" ID="rgxPostCode" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true"
                                ValidationGroup="CompanyValidation" Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCountry" runat="server" OnPreRender="ShowPlaceHolder" ClientIDMode="Static">
                 <div class="row ebiz-country">
                    <div class="medium-3 columns">
                        <asp:Label ID="ltlCountry" AssociatedControlID="ddlCountry" runat="server" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:DropDownList ID="ddlCountry" runat="server" ViewStateMode="Enabled" class="select2" /> 
                        <asp:RegularExpressionValidator ControlToValidate="ddlCountry" ID="rgxCountry" runat="server" OnPreRender="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Registration"
                            Display="Static" Enabled="true" CssClass="error ebiz-validator-error" />
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
        <div class="button-group ebiz-company-update-actions-wrap">
            <asp:Button ID="btnAdd" runat="server" CssClass="button  ebiz-primary-action ebiz-add" Visible="False" ValidationGroup="CompanyValidation"/>
            <asp:Button ID="btnUpdate" runat="server" CssClass="button  ebiz-primary-action ebiz-upate" Visible="False" ValidationGroup="CompanyValidation"/>
        </div>
    </asp:PlaceHolder>
</asp:Content>

