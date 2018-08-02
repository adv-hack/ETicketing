<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AdHocRefund.aspx.vb" Inherits="PagesAgent_Orders_AdHocRefund" ViewStateMode="Disabled" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />

    <asp:BulletedList ID="blErrorMessage" runat="server" CssClass="alert-box alert" />
    <asp:ValidationSummary ID="vlsErrorMessage" runat="server" CssClass="alert-box alert" ValidationGroup="AdHocRefund" />
    
   
    <asp:Panel ID="pnlAdHocRefund" runat="server" DefaultButton="btnAdHocRefund" CssClass="panel ebiz-adhocrefund-top-up">
        <asp:PlaceHolder ID="plhAdHocRefundHeader" runat="server"><h2><asp:Literal ID="ltlAdHocRefundHeader" runat="server" /></h2></asp:PlaceHolder>
         <fieldset>
            <legend><asp:Literal ID="ltlAdHocRefundLegend" runat="server" /></legend>
            <asp:PlaceHolder ID="plhAdHocRefundTextBox" runat="server">
            <div class="row ebiz-adhocrefund-amount">
                <div class="large-3 columns">
                    <asp:Label ID="lblAdHocRefundAmountText" runat="server" AssociatedControlID="txtAdHocRefundAmount"/>
               </div>
               <div class="large-9 columns">
                   <asp:TextBox ID="txtAdHocRefundAmount" runat="server" MaxLength="8" />
              <asp:RegularExpressionValidator ID="rgxadHocRefund" runat="server" ValidationGroup="AdHocRefund" ControlToValidate="txtAdHocRefundAmount" 
             Display="None" CssClass="error  ebiz-validator-error" SetFocusOnError="true" />
                  </div>
            </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhAdHocRefundProductDropDownList" runat="server">
            <div class="row ebiz-adhocrefund-product">
                <div class="large-3 columns">
                    <asp:Label ID="lblAdHocRefundProductDropDownList" runat="server" AssociatedControlID="ddlAdHocRefundProduct" />
                </div>
                <div class="large-9 columns">
                    <asp:DropDownList ID="ddlAdHocRefundProduct" runat="server" ViewStateMode="Enabled" />
                </div>
            </div>
            </asp:PlaceHolder>
            <asp:Button ID="btnAdHocRefund" runat="server" CausesValidation="true" ValidationGroup="AdHocRefund" CssClass="button" />
        </fieldset>
    </asp:Panel>

    <Talent:HTMLInclude ID="uscHTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:content>
