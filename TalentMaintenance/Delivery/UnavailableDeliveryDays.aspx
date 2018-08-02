<%@ Page Language="VB" MasterPageFile="~/MasterPages/MasterPage.master" AutoEventWireup="false" CodeFile="UnavailableDeliveryDays.aspx.vb" Inherits="Delivery_UnavailableDeliveryDays" %>

<asp:Content ID="ContentArea1" runat="server" ContentPlaceHolderID="Content1"></asp:Content>

<asp:Content ID="ContentArea2" runat="server" ContentPlaceHolderID="Content2">
<link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.2/themes/base/jquery-ui.css" type="text/css" media="all" />
<script src="https://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js" type="text/javascript"></script>
<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.8.2/jquery-ui.min.js" type="text/javascript"></script>
<link href="../Css/DateAndTimePicker.css" rel="stylesheet" type="text/css" />
<script src="../javascript/DateHelper.js" type="text/javascript"></script>
<script src="../javascript/DateAndTimePicker.js" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
    $(document).ready(function () { $(".datetimepicker").datetimepicker({ dateFormat: 'dd/mm/yy', numberOfMonths: 1 }); });
</script>

    <div class="unavailable-days-wrapper">
        <div class="unavailable-days-detail">
            <p class="title"><asp:Literal ID="ltlPageTitle" runat="server" /></p>
            <p class="instructions"><asp:Literal ID="ltlPageInstructions" runat="server" /></p>
        </div>

        <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <p class="error"><asp:Literal ID="ltlErrorMessage" runat="server" /></p>
        </asp:PlaceHolder>

        <fieldset class="unavailable-days-add-date">
            <ul>
                <li class="label"><asp:Label ID="lblAddDate" runat="server" AssociatedControlID="txtAddDate" /></li>
                    <li class="textbox"><asp:Label ID="lblCarrier" runat="server" /><asp:TextBox ID="txtCarrier" runat="server" /></li>
                <li class="textbox"><asp:Label ID="lblDate" runat="server" /><asp:TextBox ID="txtAddDate" runat="server" CssClass="input-s datetimepicker" /></li>
                <li class="btn"><asp:Button ID="btnAddDate" runat="server" CssClass="button" /></li>
            </ul>
        </fieldset>

        <asp:PlaceHolder ID="plhCurrentDates" runat="server">
        <div class="unavailable-days-current-days">
            <p><asp:Literal ID="ltlCurrentDates" runat="server" /></p>
            <asp:Repeater ID="rptCurrentDates" runat="server">
                <HeaderTemplate><ul></HeaderTemplate>
                <ItemTemplate>
                    <li class="odd">
                      <span class="carrier-code"><%# DataBinder.Eval(Container.DataItem, "CARRIER_CODE").ToString()%></span>
                        <span class="date"><%# DataBinder.Eval(Container.DataItem, "DATE").ToString()%></span>
                        <asp:Button ID="btnDelete" runat="server" Text='<%# DeleteButtonText %>'
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID").ToString()%>' CommandName="Delete" />
                    </li>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <li class="even">
                    <span class="carrier-code"><%# DataBinder.Eval(Container.DataItem, "CARRIER_CODE").ToString()%></span>
                        <span class="date"><%# DataBinder.Eval(Container.DataItem, "DATE").ToString()%></span>
                        <asp:Button ID="btnDelete" runat="server" Text='<%# DeleteButtonText %>'
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID").ToString()%>' CommandName="Delete" />
                    </li>
                </AlternatingItemTemplate>
                <FooterTemplate></ul></FooterTemplate>
            </asp:Repeater>
        </div>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhNoDates" runat="server">
            <div class="unavailable-days-no-days"><p><asp:Literal ID="ltlNoDatesAdded" runat="server" /></p></div>
        </asp:PlaceHolder>
    </div>
</asp:Content>