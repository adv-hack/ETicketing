<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SeasonTicketWaitList.ascx.vb" Inherits="UserControls_SeasonTicketWaitList" %>
<div class="wait-list ">
    <asp:HiddenField ID="hfProductCode" runat="server" />
    <div class="list-wrap default-form">
        <h2><asp:Label ID="titleLabel" runat="server"></asp:Label></h2>
    <p class="content">
        <asp:Label ID="contentLabel" runat="server"></asp:Label>
    </p>
    <div class="error-list">
        <asp:BulletedList ID="errorList" runat="server"></asp:BulletedList>
        <asp:ValidationSummary ID="validationErrorList" runat="server" ValidationGroup="WaitListValidation" />
    </div>
    
        <div class="quantity-row">
            <asp:Label ID="quantityLabel" runat="server" AssociatedControlID="quantityDDL"></asp:Label>
            <asp:DropDownList ID="quantityDDL" runat="server" AutoPostBack="true" />
        </div>
        <asp:Panel ID="waitListDetailsPanel" runat="server" Visible="false" CssClass="waitListDetailsPanel">
            <div class="customers-row">
                <asp:Repeater ID="customersRepeater" runat="server">
                    <HeaderTemplate>
                        <asp:Label ID="customersLabel" runat="server" OnLoad="GetText"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:DropDownList ID="customerDDL" runat="server"></asp:DropDownList>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="preferred-stand-row">
                <asp:Label ID="prefStand1Label" runat="server" AssociatedControlID="prefStand1DDL"></asp:Label>
                <asp:DropDownList ID="prefStand1DDL" runat="server" OnChange="<%# GetJavascriptStringStand1Change()%>" />
                <asp:Button ID="standLoadButton1" CssClass="button" runat="server" />
                <asp:HiddenField ID="hfStand1Selected" runat="server" />
            </div>
            <div class="preferred-area-row" id="paRow1" runat="server">
                <asp:Label ID="prefArea1Label" runat="server" AssociatedControlID="prefArea1DDL"></asp:Label>
                <asp:DropDownList ID="prefArea1DDL" runat="server" OnChange="Javascript: PopulateDDL1Hidden();" />
                <asp:HiddenField ID="hfArea1Selected" runat="server" />
            </div>
            <div class="preferred-stand-row">
                <asp:Label ID="prefStand2Label" runat="server" AssociatedControlID="prefStand2DDL"></asp:Label>
                <asp:DropDownList ID="prefStand2DDL" runat="server" OnChange="<%# GetJavascriptStringStand2Change()%>" />
                <asp:Button ID="standLoadButton2" CssClass="button" runat="server" />
                <asp:HiddenField ID="hfStand2Selected" runat="server" />
            </div>
            <div class="preferred-area-row" id="paRow2" runat="server">
                <asp:Label ID="prefArea2Label" runat="server" AssociatedControlID="prefArea2DDL"></asp:Label>
                <asp:DropDownList ID="prefArea2DDL" runat="server" OnChange="Javascript: PopulateDDL2Hidden();" />
                <asp:HiddenField ID="hfArea2Selected" runat="server" />
            </div>
            <div class="preferred-stand-row">
                <asp:Label ID="prefStand3Label" runat="server" AssociatedControlID="prefStand3DDL"></asp:Label>
                <asp:DropDownList ID="prefStand3DDL" runat="server" OnChange="<%# GetJavascriptStringStand3Change()%>" />
                <asp:Button ID="standLoadButton3" CssClass="button" runat="server" />
                <asp:HiddenField ID="hfStand3Selected" runat="server" />
            </div>
            <div class="preferred-area-row" id="paRow3" runat="server">
                <asp:Label ID="prefArea3Label" runat="server" AssociatedControlID="prefArea3DDL"></asp:Label>
                <asp:DropDownList ID="prefArea3DDL" runat="server" OnChange="Javascript: PopulateDDL3Hidden();" />
                <asp:HiddenField ID="hfArea3Selected" runat="server" />
            </div>
            <div class="email-row" id="emailRow" runat="server">
                <asp:Label ID="emailLabel" runat="server" AssociatedControlID="emailBox"></asp:Label>
                <asp:TextBox ID="emailBox" runat="server" />
                <asp:RegularExpressionValidator ID="emailRegEx" runat="server" Display="Static" ValidationGroup="WaitListValidation" Text="*" ControlToValidate="emailBox"></asp:RegularExpressionValidator>
            </div>
            <div class="phone-row" id="dPhoneRow" runat="server">
                <asp:Label ID="daytimePhoneLabel" runat="server" AssociatedControlID="daytimePhoneBox"></asp:Label>
                <asp:TextBox ID="daytimePhoneBox" runat="server" />
                <asp:RegularExpressionValidator ID="dPhoneRegEx" runat="server" ControlToValidate="daytimePhoneBox" Display="Static" ValidationGroup="WaitListValidation" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div class="phone-row" id="ePhoneRow" runat="server">
                <asp:Label ID="eveningPhoneLabel" runat="server" AssociatedControlID="eveningPhoneBox"></asp:Label>
                <asp:TextBox ID="eveningPhoneBox" runat="server" />
                <asp:RegularExpressionValidator ID="ePhoneRegEx" runat="server" ControlToValidate="eveningPhoneBox" Display="Static" ValidationGroup="WaitListValidation" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div class="phone-row" id="mPhoneRow" runat="server">
                <asp:Label ID="mobilePhoneLabel" runat="server" AssociatedControlID="mobilePhoneBox"></asp:Label>
                <asp:TextBox ID="mobilePhoneBox" runat="server" />
                <asp:RegularExpressionValidator ID="mPhoneRegEx" runat="server" Display="Static" ValidationGroup="WaitListValidation" Text="*" ControlToValidate="mobilePhoneBox"></asp:RegularExpressionValidator>
            </div>
            <div class="comments-row">
                <asp:Label ID="commentLabel" runat="server" AssociatedControlID="commentBox1"></asp:Label>
                <span class="comment-inputs"><asp:TextBox ID="commentBox1" runat="server" MaxLength="60" />
                <asp:TextBox ID="commentBox2" runat="server" MaxLength="60" /></span>
            </div>
            <div class="data-protected-row" runat="server" id="dataProtectionRow">
                <asp:Label ID="dataProtectionLabel" runat="server"></asp:Label>
                <asp:CheckBox ID="dataProtectionCheck" runat="server" />
            </div>
            <div class="buttons-row">
                <asp:Button ID="BackButton" runat="server" CssClass="button"/>
                <asp:Button ID="nextButton" runat="server" CssClass="button" CausesValidation="true" ValidationGroup="WaitListValidation" />
            </div>
        </asp:Panel>
    </div>
    <asp:Label ID="javascriptForStandDDLs" runat="server"></asp:Label>
<%--<% WriteDDLJavascript() %>--%>
</div>