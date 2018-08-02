<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DirectDebitMandate.ascx.vb" Inherits="UserControls_DirectDebitMandate" ViewStateMode="Disabled" %>
<h2><asp:Literal ID="TitleLabel" runat="server" /></h2>
<div class="row ebiz-dd-mandate-wrap">
    <div class="large-6 columns ebiz-dd-mandate-column-one">
        <div class="ebiz-dd-data ebiz-dd-address-data">
            <asp:PlaceHolder ID="plhAddressLine1" runat="server"><span><asp:Literal ID="AdressLine1" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine2" runat="server"><span><asp:Literal ID="AdressLine2" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine3" runat="server"><span><asp:Literal ID="AdressLine3" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine4" runat="server"><span><asp:Literal ID="AdressLine4" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine5" runat="server"><span><asp:Literal ID="AdressLine5" runat="server" /></span></asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAddressLine6" runat="server"><span><asp:Literal ID="AdressLine6" runat="server" /></span></asp:PlaceHolder>
        </div>
        <div class="ebiz-dd-label ebiz-dd-account-name-label">
            <asp:Label ID="AccountNameLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-account-name-data">
            <asp:Literal ID="AccountName" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-account-number-label">
            <asp:Label ID="AccountNumberLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-number-box-wrap ebiz-dd-account-number-data">
            <asp:Label ID="AccountNumber0" runat="server" />
            <asp:Label ID="AccountNumber1" runat="server" />
            <asp:Label ID="AccountNumber2" runat="server" />
            <asp:Label ID="AccountNumber3" runat="server" />
            <asp:Label ID="AccountNumber4" runat="server" />
            <asp:Label ID="AccountNumber5" runat="server" />
            <asp:Label ID="AccountNumber6" runat="server" />
            <asp:Label ID="AccountNumber7" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-sort-code-label">
            <asp:Label ID="SortCodeLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-number-box-wrap ebiz-dd-sort-code-data">
            <asp:Label ID="SortCode0" runat="server" />
            <asp:Label ID="SortCode1" runat="server" />
            <asp:Label ID="SortCode2" runat="server" />
            <asp:Label ID="SortCode3" runat="server" />
            <asp:Label ID="SortCode4" runat="server" />
            <asp:Label ID="SortCode5" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-bank-name-label">
            <asp:Label ID="BankNameLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-bank-name-data">
            <asp:Label ID="BankName" runat="server" />
        </div>
    </div>
    <div class="large-6 columns ebiz-dd-mandate-column-two">
        <asp:Image ID="DirectDebitImage" runat="server" CssClass="ebiz-dd-image" />
        <div class="ebiz-dd-instructions-title">
            <asp:Literal ID="InstructionTitleLabel" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-originator-label">
            <asp:Literal ID="OriginatorLabel" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-originator-text-label">
            <asp:Literal ID="OriginatorText" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-number-box-wrap ebiz-dd-originator-numbers-data">
            <asp:Label ID="OriginatorNumber0" runat="server" />
            <asp:Label ID="OriginatorNumber1" runat="server" />
            <asp:Label ID="OriginatorNumber2" runat="server" />
            <asp:Label ID="OriginatorNumber3" runat="server" />
            <asp:Label ID="OriginatorNumber4" runat="server" />
            <asp:Label ID="OriginatorNumber5" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-reference-label">
            <asp:Literal ID="ReferenceNumberLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-number-box-wrap ebiz-dd-reference-numbers">
            <asp:Label ID="ReferenceNumber0" runat="server" />
            <asp:Label ID="ReferenceNumber1" runat="server" />
            <asp:Label ID="ReferenceNumber2" runat="server" />
            <asp:Label ID="ReferenceNumber3" runat="server" />
            <asp:Label ID="ReferenceNumber4" runat="server" />
            <asp:Label ID="ReferenceNumber5" runat="server" />
            <asp:Label ID="ReferenceNumber6" runat="server" />
            <asp:Label ID="ReferenceNumber7" runat="server" />
            <asp:Label ID="ReferenceNumber8" runat="server" />
            <asp:Label ID="ReferenceNumber9" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-instructions-text-label">
            <asp:Literal ID="InstructionTextLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-instructions-data">
            <asp:Literal ID="InstructionText" runat="server" />
        </div>
        <div class="ebiz-dd-label ebiz-dd-date-label">
            <asp:Literal ID="TodaysDateLabel" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-date-data">
            <asp:Literal ID="TodaysDate" runat="server" />
        </div>
        <div class="ebiz-dd-data ebiz-dd-addtional-text">
            <asp:Literal ID="AdditionalText" runat="server" />
        </div>
    </div>
</div>