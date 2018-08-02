<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OnAccountAdjustment.aspx.vb" Inherits="PagesLogin_Profile_OnAccountAdjustment"  ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/MySavedCards.ascx" TagName="MySavedCards" TagPrefix="Talent" %>
<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:UpdatePanel ID="updActAdjustment" runat="server" ViewStateMode="Enabled">
        <ContentTemplate>
            <script type="text/javascript">             
                Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(function (evt, args) {
                    $(".datepicker").datepicker({
                        dateFormat: 'dd/mm/yy',
                        onSelect: function (date) {
                
                            var selectedDate = new Date(date);
                            var dateObjecta = $.datepicker.parseDate('dd/mm/yy', date);
                
                            var currDate = new Date();
                            currDate.setHours(0, 0, 0, 0);
                
                            //alert('difference = ' + ((dateObjecta - currDate) / 1000 / 60 / 60 / 24));
                
                            if (((dateObjecta - currDate) / 1000 / 60 / 60 / 24) > 0) {
                                $(".datepickerFuture").datepicker("option", "minDate", dateObjecta.getDate() + 1);
                            }
                            else {
                                $(".datepickerFuture").datepicker("option", "minDate", 1);
                            }
                        }
                    });
                    $(".datepickerFuture").datepicker({
                        dateFormat: 'dd/mm/yy',
                        minDate: 1,
                        onSelect: function (date) {
                
                            var selectedDate = new Date(date);
                            $(".datepicker").datepicker("option", "maxDate", date);
                
                        }
                    });
                });
                
            </script>
            <asp:ValidationSummary ID="vlsActAdjustmentErrors" runat="server" ValidationGroup="ActAdjustment" ShowSummary="true" CssClass="alert-box alert" />
            <asp:Panel ID="pnlAccountAdjustment" runat="server" DefaultButton="btnOkay" class="panel ebiz-on-account-adjustment-wrap">
                <asp:PlaceHolder ID="plhErrorList" runat="server">
                    <div class="alert-box alert">
                        <asp:BulletedList ID="errorList" runat="server" CssClass="error" />
                    </div>
                </asp:PlaceHolder>

                <asp:PlaceHolder ID="plhRefundToCardHeading" runat="server">
                    <h2><asp:Label ID="ltlrefundtocard" runat="server" /></h2>
                    <div class="row ebiz-totalonaccount">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblTotalOnAccount" runat="server"/>
                        </div>
                        <div class="medium-9 columns">
                            <asp:Label ID="TotalOnAccount" runat="server"/>
                        </div>
                    </div>
                        <div class="row ebiz-refundableonaccount">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblRefundableOnAccount" runat="server"/>
                        </div>
                        <div class="medium-9 columns">
                            <asp:Label ID="RefundableOnAccount" runat="server"/>
     		            </div>
                    </div>
                 </asp:PlaceHolder> 
        
                <div class="row ebiz-amount-wrap">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblAmount" runat="server" AssociatedControlID="txtAmount" CssClass="medium-middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtAmount" TabIndex="1" CssClass="input-l" runat="server"/>
                        <asp:RequiredFieldValidator ControlToValidate="txtAmount" ID="rfvAmount" runat="server" OnLoad="SetErrorMessageAmount" CssClass="error"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="rgxAmount" runat="server" ValidationGroup="Checkout" CssClass="error" ControlToValidate="txtAmount"  OnLoad="SetRegex"/>
                    </div>
                </div>
                <asp:PlaceHolder ID="plhAdjustmentType" runat="server">
                <div class="row ebiz-adjustment-type-wrap">
                    <div class="medium-3 columns u-margin-bottom-1-small-only">
                        <asp:Label ID="lblAdjustmentType" runat="server" CssClass="medium-middle" />
                    </div>
                    <div class="medium-9 columns">
                        <div class="row">
                            <div class="column">
                                <asp:RadioButton runat="server" ID="rdbPositive"  TabIndex="2"  GroupName="AdjustmentType" OnCheckedChanged="RdbAdjustmentType_OnCheckedChanged"   ViewStateMode="Enabled" AutoPostback="True"  Checked="True" />
                            </div>
                            <div class="column">
                                <asp:RadioButton runat="server" ID="rdbNegative"  TabIndex="3"  GroupName="AdjustmentType" OnCheckedChanged="RdbAdjustmentType_OnCheckedChanged"   ViewStateMode="Enabled" AutoPostback="True" />
                            </div>
                        </div>
                    </div>
                </div>
                </asp:PlaceHolder>  
                <asp:PlaceHolder ID="plhReason" runat="server"> 
                <div class="row ebiz-reason-wrap">
                    <div class="medium-3 columns">
                        <asp:Label ID="lblReason" runat="server" AssociatedControlID="txtReason" CssClass="medium-middle" />
                    </div>
                    <div class="medium-9 columns">
                        <asp:TextBox ID="txtReason"  TabIndex="4" CssClass="input-l" maxlength="50" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtReason" ID="rfvReason" runat="server" OnLoad="SetErrorMessageReason" CssClass="error"></asp:RequiredFieldValidator>
                    </div>
                </div>
                </asp:PlaceHolder>  
                <asp:PlaceHolder ID="plhActivationDate" runat="server">
                    <div class="row ebiz-activation-date-wrap">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblActivationDate" runat="server" AssociatedControlID="txtActivationDate" CssClass="medium-middle" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:TextBox ID="txtActivationDate" TabIndex="5" CssClass="datepicker" runat="server" ValidationGroup="ActAdjustment" />
                            <asp:CustomValidator id="cvActDate" ControlToValidate="txtActivationDate" OnServerValidate="ValidateActivationDate" ValidationGroup="ActAdjustment" Display="Static" runat="server" SetFocusOnError="true" CssClass="error" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhExpiryDetails" runat="server">
                    <div class="row ebiz-expiry-or-wrap">
                        <div class="medium-3 columns u-margin-bottom-1-small-only">
                            <asp:Label ID="lblExpiryOr" runat="server" CssClass="medium-middle" />
                        </div>
                        <div class="medium-9 columns">
                            <div class="row js-icheck-postback-container">
                                <div class="column">
                                    <asp:RadioButton runat="server" ID="rdoNoExpiry" TabIndex="6" GroupName="Expiry" ViewStateMode="Enabled" Checked="True" />
                                </div>
                                <div class="column">
                                    <asp:RadioButton runat="server" ID="rdoRewardPeriodEnd" GroupName="Expiry" ViewStateMode="Enabled" />
                                </div>
                                <div class="column">
                                    <asp:RadioButton runat="server" ID="rdoSpecificDate" GroupName="Expiry" ViewStateMode="Enabled" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row ebiz-expiry-date-wrap">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblExpiryDate" runat="server" AssociatedControlID="txtExpiryDate" CssClass="medium-middle" />
                        </div>
                        <div class="medium-9 columns">
                            <asp:TextBox ID="txtExpiryDate" CssClass='datepickerFuture' runat="server" ValidationGroup="ActAdjustment" ></asp:TextBox>
                            <asp:CustomValidator id="cvExpiry" ControlToValidate="txtExpiryDate" OnServerValidate="ValidateExpiryDate" ValidationGroup="ActAdjustment" Display="Static" runat="server" SetFocusOnError="true"  CssClass="error" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhConfirmAllowance" runat="server">
                    <div class="row ebiz-confirm-allowance-wrap">
                        <div class="medium-3 columns">
                            <asp:Label ID="lblConfirmAllowance" runat="server" AssociatedControlID="chkConfirmAllowance" CssClass="medium-middle" />
                        </div>
                        <div class="medium-9 columns js-icheck-postback-container">
                            <asp:CheckBox ID="chkConfirmAllowance" runat="server"></asp:CheckBox>
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhDDLSavedCard" runat="server" visible="false">
                    <Talent:MySavedCards ID="uscSavedCards" runat="server" />
                </asp:PlaceHolder>
            </asp:Panel>
            <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="2" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="stacked-for-small button-group">
        <asp:PlaceHolder ID="plhBackButton" runat="server">
            <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-back" OnClick="btnBack_Click" CausesValidation="false" />
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plhOkButton" runat="server">
            <asp:Button ID="btnOkay" runat="server" CssClass="button ebiz-primary-action ebiz-okay" CausesValidation="true"/>
        </asp:PlaceHolder>
    </div>
</asp:Content>