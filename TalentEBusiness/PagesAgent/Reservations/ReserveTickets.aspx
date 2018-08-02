<%@ Page Language="VB"  AutoEventWireup="false" CodeFile="ReserveTickets.aspx.vb" Inherits="PagesAgent_Reservations_ReserveTickets" EnableViewState="false" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<asp:Content ID="cphHead" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".datetimepicker").datetimepicker({ dateFormat: 'dd/mm/yy', numberOfMonths: 1 });
        });
    </script>
</asp:content>
<asp:Content ID="cphBody" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder runat="server" ID="plhErrorMessage" Visible="false">
        <div class="alert-box alert ebiz-reservation-error-message">
            <asp:BulletedList id="blErrorDetails" runat="server"></asp:BulletedList>
        </div>
    </asp:PlaceHolder> 
    <asp:ValidationSummary ID="vlsReserveTickets" runat="server" ShowSummary="true" EnableClientScript="true" DisplayMode="BulletList" CssClass="alert-box alert" />
    <asp:PlaceHolder ID="plhReserveTickets" runat="server" Visible="false">
        <div class="row ebiz-reserved-by">
            <div class="medium-3 columns">
                <asp:Label ID="lblReservedBy" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:Label ID="lblAgentName" runat="server" />
            </div>
        </div>
        <div class="row ebiz-expiry-date">
            <div class="medium-3 columns">
                <asp:Label ID="lblExpiryDate" AssociatedControlID="txtExpiryDate" runat="server" CssClass="middle" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtExpiryDate" runat="server"  CssClass="datetimepicker" TabIndex="1" />
                <asp:RequiredFieldValidator ID="rfvExpiryDate" runat="server" ControlToValidate="txtExpiryDate" CssClass="error" ValidationGroup="ReserveTickets" Display="Static"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revExpiryDate" runat="server" ControlToValidate="txtExpiryDate" CssClass="error" ValidationGroup="ReserveTickets" Display="Static" Enabled="false"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row ebiz-comment">
            <div class="medium-3 columns">
                <asp:Label ID="lblComment" AssociatedControlID="txtComment" runat="server" CssClass="middle" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtComment" runat="server" TabIndex="2" />
            </div>
        </div>
        <div class="row ebiz-sale-or-return">
            <div class="medium-3 columns">
                <asp:Label ID="lblSaleOrReturn" AssociatedControlID="chkSaleOrReturn" runat="server" CssClass="middle"  />
            </div>
            <div class="medium-9 columns">
                <asp:Checkbox ID="chkSaleOrReturn" runat="server" TabIndex="3" AutoPostBack="True" />
            </div>
        </div>
        <ul class="button-group">
		  	<li><asp:button ID="btnReserve" runat="server" CssClass="button ebiz-reserve" ValidationGroup="ReserveTickets" TabIndex="4" /></li>
		</ul>
    </asp:PlaceHolder>
</asp:Content>

