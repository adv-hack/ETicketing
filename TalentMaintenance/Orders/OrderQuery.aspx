<%@ Page Language="VB" AutoEventWireup="false" CodeFile="OrderQuery.aspx.vb" Inherits="Orders_OrderQuery" 
    MasterPageFile="~/MasterPages/MaintenanceMasterPage.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">

    <div id="order-query-wrapper">
        <h1><asp:Literal ID="ltlPageHeader" runat="server" /></h1>
        <asp:ValidationSummary ID="vlsDateValidationSummary" runat="server" ValidationGroup="OrderQuery" />

        <ul class="order-from-date">
            <li class="date">
                <asp:Label ID="lblFromDate" runat="server" AssociatedControlID="txtFromDate" />
                <asp:TextBox ID="txtFromDate" runat="server" MaxLength="2" />
                <asp:RegularExpressionValidator ID="regFromDate" runat="server" ValidationExpression="^[0-9]{2}$" 
                    ControlToValidate="txtFromDate" ValidationGroup="OrderQuery" Display="None" />
                <asp:RequiredFieldValidator ID="rfvFromDate" runat="server" ControlToValidate="txtFromDate" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
            <li class="month">
                <asp:Label ID="lblFromMonth" runat="server" AssociatedControlID="txtFromMonth" />
                <asp:TextBox ID="txtFromMonth" runat="server" MaxLength="2" />
                <asp:RegularExpressionValidator ID="regFromMonth" runat="server" ValidationExpression="^[0-9]{2}$" 
                    ControlToValidate="txtFromMonth" ValidationGroup="OrderQuery"  Display="None" />
                <asp:RequiredFieldValidator ID="rfvFromMonth" runat="server" ControlToValidate="txtFromMonth" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
            <li class="date">
                <asp:Label ID="lblFromYear" runat="server" AssociatedControlID="txtFromYear" />
                <asp:TextBox ID="txtFromYear" runat="server" MaxLength="4" />
                <asp:RegularExpressionValidator ID="regFromYear" runat="server" ValidationExpression="^[0-9]{4}$" 
                    ControlToValidate="txtFromYear" ValidationGroup="OrderQuery" Display="None" />
                <asp:RequiredFieldValidator ID="rfvFromYear" runat="server" ControlToValidate="txtFromYear" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
        </ul>

        <ul class="order-to-date">
            <li class="date">
                <asp:Label ID="lblToDate" runat="server" AssociatedControlID="txtToDate" />
                <asp:TextBox ID="txtToDate" runat="server" MaxLength="2" />
                <asp:RegularExpressionValidator ID="regToDate" runat="server" ValidationExpression="^[0-9]{2}$" 
                    ControlToValidate="txtToDate" ValidationGroup="OrderQuery" Display="None" />
                <asp:RequiredFieldValidator ID="rfvToDate" runat="server" ControlToValidate="txtToDate" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
            <li class="month">
                <asp:Label ID="lblToMonth" runat="server" AssociatedControlID="txtToMonth" />
                <asp:TextBox ID="txtToMonth" runat="server" MaxLength="2" />
                <asp:RegularExpressionValidator ID="regToMonth" runat="server" ValidationExpression="^[0-9]{2}$" 
                    ControlToValidate="txtToMonth" ValidationGroup="OrderQuery" Display="None" />
                <asp:RequiredFieldValidator ID="rfvToMonth" runat="server" ControlToValidate="txtToMonth" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
            <li class="date">
                <asp:Label ID="lblToYear" runat="server" AssociatedControlID="txtToYear" />
                <asp:TextBox ID="txtToYear" runat="server" MaxLength="4" />
                <asp:RegularExpressionValidator ID="regToYear" runat="server" ValidationExpression="^[0-9]{4}$" 
                    ControlToValidate="txtToYear" ValidationGroup="OrderQuery" Display="None" />
                <asp:RequiredFieldValidator ID="rfvToYear" runat="server" ControlToValidate="txtToYear" 
                    ValidationGroup="OrderQuery" Display="None" />
            </li>
        </ul>

        <div class="submit-button">
            <asp:Button ID="btnGetOrders" runat="server" ValidationGroup="OrderQuery" CausesValidation="true" />
        </div>

        <asp:PlaceHolder ID="plhResults" runat="server" Visible="false">
        <div class="order-query-results">
            <asp:Label ID="lblResults" runat="server" />
        </div>
        </asp:PlaceHolder>
    </div>

</asp:Content>