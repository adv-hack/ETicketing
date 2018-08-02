<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AmendPPSEnrolment.aspx.vb" Inherits="PagesPublic_ProductBrowse_AmendPPSEnrolment" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductDetail.ascx" TagName="ProductDetail" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PaymentDetails.ascx" TagName="PaymentDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebit.ascx" TagName="DirectDebit" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/DirectDebitMandate.ascx" TagName="DirectDebitMandate" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/TicketingPPS.ascx" TagName="TicketingPPS" TagPrefix="Talent" %>
<%@ Register Assembly="System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:UpdatePanel ID="updErrorList" runat="server">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlPaymentOptions" />
            <asp:AsyncPostBackTrigger ControlID="btnSubmitPaymentOptions" />
            <asp:AsyncPostBackTrigger ControlID="btnSubmitDirectDebitMandate" />
        </Triggers>
        <ContentTemplate>
            <asp:PlaceHolder ID="plhErrorList" runat="server">
                <div class="alert-box alert">
                    <asp:BulletedList ID="ErrorList" runat="server" />
                </div>
            </asp:PlaceHolder>
        </ContentTemplate>
    </asp:UpdatePanel>

    <asp:PlaceHolder ID="plhSuccessMessage" runat="server" Visible="false">
        <div class="alert-box success">
            <asp:Literal ID="ltlSuccessMessage" runat="server" />
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhAmendPPSList" runat="server" Visible="true">
        <div class="ebiz-pps-list">
            <Talent:ProductDetail ID="ProductDetail1" runat="server" ProductType="P" PPSType="*" AmendPPSEnrolmentInUse="True" />
        </div>
        <asp:PlaceHolder ID="plhRegiesteredPostOption" runat="server" Visible="false">
            <div class="alert-box info ebiz-amend-pps-reg-post-wrap">
                <asp:CheckBox ID="chkRegisteredPost" runat="server" />
                <label for="<%=chkRegisteredPost.ClientID %>"><asp:Literal ID="ltlRegistedPost" runat="server" /></label>
            </div>
        </asp:PlaceHolder>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhPayment" runat="server" Visible="true">
        <asp:UpdatePanel ID="updPaymentOptions" runat="server" UpdateMode="Conditional" Visible="false">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlPaymentOptions" />
                <asp:AsyncPostBackTrigger ControlID="btnSubmitPaymentOptions" />
                <asp:AsyncPostBackTrigger ControlID="btnSubmitDirectDebitMandate" />
            </Triggers>
            <ContentTemplate>
                <div class="panel ebiz-pps-payment-wrap">
                    <div class="row ebiz-pps-payment">
                        <div class="medium-3 columns">
                            <label for="<%=ddlPaymentOptions.ClientID %>"><asp:Literal ID="ltlPaymentOptions" runat="server" /></label>
                        </div>
                        <div class="medium-9 columns">
                            <asp:DropDownList CssClass="select" ID="ddlPaymentOptions" runat="server" AutoPostBack="true" />
                            <noscript><asp:Button ID="btnSubmitPaymentSelection" runat="server" CssClass="button" /></noscript>
                        </div>
                    </div>
                    <asp:PlaceHolder ID="plhCCPaymentOptions" runat="server" Visible="false">
                        <Talent:PaymentDetails ID="PaymentDetailsCreditCardForm" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhDDPaymentOptions" runat="server" Visible="false">
                        <Talent:DirectDebit ID="PaymentDetailsDirectDebitForm" runat="server" UsePaymentDaysDDL="false" />
                        <Talent:DirectDebitMandate ID="PaymentDetailsDirectDebitMandate" runat="server" Visible="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhSumbitButton" runat="server" Visible="false">
                        <div class="ebiz-submit-payment-options-wrap">
                            <asp:Button ID="btnSubmitPaymentOptions" runat="server" CssClass="button" />
                            <asp:Button ID="btnSubmitDirectDebitMandate" runat="server" Visible="false" Text="Confirm" CssClass="button" />
                        </div>
                    </asp:PlaceHolder>
                    <asp:HiddenField ID="hidPosition" runat="server" Value="FALSE" />
                </div>
                <asp:UpdateProgress ID="updProgress" runat="server">
                    <ProgressTemplate>
                        <div class="ebiz-loading-default ebiz-pps-payment-loading">
                            <!--<%=LoadingText %>-->
                            <i class="fa fa-spinner fa-pulse"></i>
                        </div>
                    </ProgressTemplate>
                </asp:UpdateProgress>
            </ContentTemplate>
        </asp:UpdatePanel>
    </asp:PlaceHolder>
</asp:Content>
