<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BasketFees.ascx.vb" Inherits="UserControls_BasketFees" %>
<%@ Reference Control="~/UserControls/BasketSummary.ascx" %>

<asp:PlaceHolder ID="plhBasketFees" runat="server" Visible="true">
    <asp:PlaceHolder ID="plhFeesAccordion" runat="server">
        
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhFeesCharity" runat="server" Visible="false">
        <div class="panel ebiz-fee-wrap ebiz-fees-charity-wrap">
            <h2><asp:Literal ID="ltlFeesCharityHeaderLabel" runat="server" /></h2> 
            <asp:Repeater ID="rptFeesCharityDetail" runat="server">
                <ItemTemplate>
                    <div class="ebiz-charity-option-wrap">
                        <asp:CheckBox ID="chkFeesCharity" AutoPostBack="true" runat="server" OnCheckedChanged="ChkFeesCharity_OnCheckedChanged" />
                        <asp:Label ID="lblFeesCharityLabel" runat="server" AssociatedControlID="chkFeesCharity" />
                        <asp:HiddenField ID="hidFeesCharityCode" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="plhGiftAid" runat="server" Visible="false">
                <div class="ebiz-charity-gift-aid-wrap">
                    <asp:CheckBox ID="chkGiftAid" runat="server" Enabled="false" ViewStateMode="Enabled" CssClass="ebiz-checkbox-label" />
                    <asp:Label ID="lblGiftAid" runat="server" AssociatedControlID="chkGiftAid" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhFeesAdhoc" runat="server" Visible="false">
        <div class="panel ebiz-fee-wrap ebiz-fees-adhoc-wrap">
            <h2><asp:Literal ID="ltlFeesAdhocHeaderLabel" runat="server" /></h2>
            <asp:Repeater ID="rptFeesAdhocDetail" runat="server">
                <ItemTemplate>
                    <div class="ebiz-adhoc-option-wrap">
                        <asp:CheckBox ID="chkFeesAdhoc" AutoPostBack="true" runat="server" OnCheckedChanged="ChkFeesAdhoc_OnCheckedChanged" />
                        <asp:Label ID="lblFeesAdhocLabel" runat="server" AssociatedControlID="chkFeesAdhoc" />
                        <asp:HiddenField ID="hidFeesAdhocCode" runat="server" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhFeesVariable" runat="server" Visible="false">
        <div class="panel ebiz-fee-wrap ebiz-fees-variable-wrap">
            <h2><asp:Literal ID="ltlFeesVariableHeaderLabel" runat="server" /></h2>  
            <asp:Repeater ID="rptFeesVariableDetail" runat="server">
                <ItemTemplate>
                    <div class="ebiz-variable-option-wrap">
                        <asp:HiddenField ID="hidFeesVariableCode" runat="server" />
                        <asp:CheckBox ID="chkFeesVariable" AutoPostBack="true" runat="server" OnCheckedChanged="ChkFeesVariable_OnCheckedChanged" />
                        <asp:Label ID="lblFeesVariableLabel" runat="server" AssociatedControlID="chkFeesVariable" />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:PlaceHolder ID="plhVariableFeesAvail" runat="server">
                <div class="ebiz-variable-select-wrap">
                    <asp:Label ID="lblVariableFeeDDLLabel" runat="server" AssociatedControlID="ddlVariableFeesAvail" />
                    <asp:DropDownList ID="ddlVariableFeesAvail" runat="server" />
                </div>
                <div class="ebiz-variable-enter-wrap">
                    <asp:Label ID="lblVariableFeeValLabel" runat="server" AssociatedControlID="txtVariableFeeValue" />
                    <asp:TextBox ID="txtVariableFeeValue" runat="server" />
                    <asp:RegularExpressionValidator ID="rgxVariableFee" runat="server" ValidationGroup="VariableFee" ControlToValidate="txtVariableFeeValue" 
                        Display="Static" CssClass="error" SetFocusOnError="true" />
                </div>
                <div class="ebiz-add-variable-fee-button-wrap">
                    <asp:Button ID="btnVariableFeeAdd" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="VariableFee" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

</asp:PlaceHolder>