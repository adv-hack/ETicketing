<%@ Control Language="VB" AutoEventWireup="false" CodeFile="RegistrationParticipants.ascx.vb" Inherits="UserControls_RegistrationParticipants" %>

<asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="RegParticipants" CssClass="alert-box alert" />
    
<asp:PlaceHolder ID="phError" runat="server">
    <div class="alert-box alert">
        <asp:Literal ID="lblError" runat="server" />
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="phHiddenFields" runat="server">
    <asp:HiddenField ID="hdfProductCode" runat="server" />
    <asp:HiddenField ID="hdfProductType" runat="server" />
    <asp:HiddenField ID="hdfProductSubType" runat="server" />
    <asp:HiddenField ID="hdfProductStadium" runat="server" />
    <asp:HiddenField ID="hdfStandCode" runat="server" />
    <asp:HiddenField ID="hdfAreaCode" runat="server" />
    <asp:HiddenField ID="hdfCampaignCode" runat="server" />
    <asp:HiddenField ID="hdfQuantity" runat="server" />
    <asp:HiddenField ID="hdfUpdateMode" runat="server" />
</asp:PlaceHolder>

<asp:Repeater ID="rptParticipantsForm" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
        <div class="panel ebiz-registration-participants-form">
            <div class="row ebiz-registration-participants-customer">
                <div class="large-3 columns">
                    <asp:Label ID="lblMember" runat="server" AssociatedControlID="ddlMember" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlMember" runat="server" CausesValidation="false" OnSelectedIndexChanged="ddlMember_SelectedIndexChanged" AutoPostBack="true" />
                    <asp:RequiredFieldValidator ID="rfvMember" runat="server" ControlToValidate="ddlMember" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue="" />
                    <asp:RegularExpressionValidator ID="revMember" ControlToValidate="ddlMember" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <noscript>
                        <asp:Button ID="btnFetchMember" runat="server" CssClass="button" CommandName="FetchMember" />
                    </noscript>
                </div>
            </div>
            <div class="row ebiz-registration-participants-forename">
                <div class="large-3 columns">
                    <asp:Label ID="lblForename" runat="server" AssociatedControlID="txtForename" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtForename" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvForename" runat="server" ControlToValidate="txtForename" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revForename" ControlToValidate="txtForename" CssClass="error"
                        runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />  
                </div>
            </div>
            <div class="row ebiz-registration-participants-surname">
                <div class="large-3 columns">
                    <asp:Label ID="lblSurname" runat="server" AssociatedControlID="txtSurname" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtSurname" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvSurname" runat="server" ControlToValidate="txtSurname" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revSurname" ControlToValidate="txtSurname" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-dob">
                <div class="large-3 columns">
                    <asp:Label ID="lblDOB" runat="server" AssociatedControlID="ddlDate" CssClass="middle" />
                </div>
                <div class="large-3 columns">
                    <asp:DropDownList ID="ddlDate" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="ddlDate" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue="" />
                    <asp:RequiredFieldValidator ID="rfvMonth" runat="server" ControlToValidate="ddlMonth" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue="" />
                </div>
                <div class="large-3 columns">
                    <asp:DropDownList ID="ddlMonth" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvYear" runat="server" ControlToValidate="ddlYear" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue="" />
                    <asp:RegularExpressionValidator ID="revDate" ControlToValidate="ddlDate" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
                <div class="large-3 columns">
                    <asp:DropDownList ID="ddlYear" runat="server" />
                    <asp:RegularExpressionValidator ID="revMonth" ControlToValidate="ddlMonth" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revYear" ControlToValidate="ddlYear" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-sex">
                <div class="large-3 columns">
                    <asp:Label ID="lblSex" runat="server" AssociatedControlID="ddlSex" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlSex" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvSex" runat="server" ControlToValidate="ddlSex" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue="" />
                    <asp:RegularExpressionValidator ID="revSex" ControlToValidate="ddlSex" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-email">
                <div class="large-3 columns">
                    <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtEmail" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revEmail" ControlToValidate="txtEmail" runat="server" CssClass="error"
                        OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <asp:PlaceHolder ID="phAddressDetails" runat="server">
                <div class="row ebiz-registration-participants-address1">
                    <div class="large-3 columns">
                        <asp:Label ID="lblAddressLine1" runat="server" AssociatedControlID="txtAddressLine1" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:TextBox ID="txtAddressLine1" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server" ControlToValidate="txtAddressLine1" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                        <asp:RegularExpressionValidator ID="revAddressLine1" ControlToValidate="txtAddressLine1" CssClass="error"
                            runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
                <div class="row ebiz-registration-participants-address2">
                    <div class="large-3 columns">
                        <asp:Label ID="lblAddressLine2" runat="server" AssociatedControlID="txtAddressLine2" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:TextBox ID="txtAddressLine2" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvAddressLine2" runat="server" ControlToValidate="txtAddressLine2" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                        <asp:RegularExpressionValidator ID="revAddressLine2" ControlToValidate="txtAddressLine2" CssClass="error"
                            runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
                <div class="row ebiz-registration-participants-address3">
                    <div class="large-3 columns">
                        <asp:Label ID="lblCity" runat="server" AssociatedControlID="txtCity" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:TextBox ID="txtCity" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvCity" runat="server" ControlToValidate="txtCity" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                        <asp:RegularExpressionValidator ID="revCity" ControlToValidate="txtCity" runat="server" CssClass="error"
                            OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
                <div class="row ebiz-registration-participants-county">
                    <div class="large-3 columns">
                        <asp:Label ID="lblCounty" runat="server" AssociatedControlID="txtCounty" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:TextBox ID="txtCounty" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvCounty" runat="server" ControlToValidate="txtCounty" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                        <asp:RegularExpressionValidator ID="revCounty" ControlToValidate="txtCounty" runat="server" CssClass="error"
                            OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
                <div class="row ebiz-registration-participants-postcode">
                    <div class="large-3 columns">
                        <asp:Label ID="lblPostcode" runat="server" AssociatedControlID="txtPostcode" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:TextBox ID="txtPostcode" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvPostcode" runat="server" ControlToValidate="txtPostcode" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                        <asp:RegularExpressionValidator ID="revPostcode" ControlToValidate="txtPostcode" CssClass="error"
                            runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
                <div class="row ebiz-registration-participants-country">
                    <div class="large-3 columns">
                        <asp:Label ID="lblCountry" runat="server" AssociatedControlID="ddlCountry" CssClass="middle" />
                    </div>
                    <div class="large-9 columns">
                        <asp:DropDownList ID="ddlCountry" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="ddlCountry" CssClass="error"
                            OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" InitialValue=" -- " />
                        <asp:RegularExpressionValidator ID="revCountry" ControlToValidate="ddlCountry" runat="server" CssClass="error"
                            OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="row ebiz-registration-participants-emergency-name">
                <div class="large-3 columns">
                    <asp:Label ID="lblEmergencyContactName" runat="server" AssociatedControlID="txtEmergencyContactName" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtEmergencyContactName" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvEmergencyContactName" runat="server" ControlToValidate="txtEmergencyContactName" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revEmergencyContactName" ControlToValidate="txtEmergencyContactName" CssClass="error"
                        runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-emergency-number">
                <div class="large-3 columns">
                    <asp:Label ID="lblEmergencyContactNo" runat="server" AssociatedControlID="txtEmergencyContactNo" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                    <asp:TextBox ID="txtEmergencyContactNo" runat="server" />
                    <asp:RequiredFieldValidator ID="rfvEmergencyContactNo" runat="server" ControlToValidate="txtEmergencyContactNo" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revEmergencyContactNo" ControlToValidate="txtEmergencyContactNo" CssClass="error"
                        runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-emergency-fan">
                <div class="large-3 columns">
                    <asp:Label ID="lblFan" runat="server" AssociatedControlID="chkFan" CssClass="middle" />
                </div>
                <div class="large-9 columns ebiz-checkbox-column-wrap">
                    <asp:CheckBox ID="chkFan" runat="server" />
                </div>
            </div>
            <div class="row ebiz-registration-participants-emergency-medical-info">
                <div class="large-3 columns">
                    <asp:Label ID="lblMedicalInfo" runat="server" AssociatedControlID="txtMedicalInfo" CssClass="middle" />
                </div>
                <div class="large-9 columns">
                <asp:TextBox ID="txtMedicalInfo" runat="server" TextMode="MultiLine" />
                    <asp:RequiredFieldValidator ID="rfvMedicalInfo" runat="server" ControlToValidate="txtMedicalInfo" CssClass="error"
                        OnInit="SetupRequiredValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:RegularExpressionValidator ID="revMedicalInfo" ControlToValidate="txtMedicalInfo" CssClass="error"
                        runat="server" OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" />
                    <asp:CustomValidator ID="cvMedicalInfo" ControlToValidate="txtMedicalInfo" runat="server" CssClass="error"
                        SetFocusOnError="true" Visible="true" ValidationGroup="RegParticipants" Display="Static" Enabled="true" OnServerValidate="MedicalInfoValidation" ClientValidationFunction="validateMedicalInfoLength" />
                </div>
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>

<div class="stacked-for-small button-group ebiz-registration-participants-buttons-wrap">   
    <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-back" />
    <asp:Button ID="btnRegister" CausesValidation="true" ValidationGroup="RegParticipants" runat="server" CssClass="button ebiz-register" />
</div>