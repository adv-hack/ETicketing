<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ReserveTickets.ascx.vb" Inherits="UserControls_ReserveTickets" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/BasketDetails.ascx" %>

<div id="reserve-button" class="reveal ebiz-reserve-ticket-wrap" data-reveal>
    <script type="text/javascript">
        $(function () {
            $(".datetimepicker").datetimepicker({ dateFormat: 'dd/mm/yy', numberOfMonths: 1 });
            document.getElementById('txtExpiryDate').value = '';
            document.getElementById('txtComment').value = '';
            document.getElementById('hdfExpiryDate').value = '';
        });

        $('#txtComment').focus();

        function toggleSalesOrReturn(chkSaleOrReturn) {
            var hdfExpiryDate = document.getElementById('hdfExpiryDate');
            var txtExpiryDate = document.getElementById('txtExpiryDate');
            var rfvExpiryDate = document.getElementById('rfvExpiryDate');
        
            if (chkSaleOrReturn.checked) {
                rfvExpiryDate.enabled = false;
                txtExpiryDate.disabled = true;
                hdfExpiryDate.value = txtExpiryDate.value;
                txtExpiryDate.value = '';
            } else {
                rfvExpiryDate.enabled = false;
                txtExpiryDate.disabled = false;
                txtExpiryDate.value = hdfExpiryDate.value;
            }
        }

        function reserveButtonClick() {
            if (txtExpiryDate.value != "")
            {
                $("#reserve-button").foundation('close');
            }         
        }

        function cancelReserve(){      
            document.getElementById('hdfExpiryDate').value='';
            document.getElementById('txtExpiryDate').value='';
            document.getElementById('txtComment').value = '';
        }
    </script>


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
                <asp:Label ID="lblExpiryDate" AssociatedControlID="txtExpiryDate" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtExpiryDate" runat="server"  CssClass="datetimepicker" TabIndex="1" ClientIDMode="Static" />
                <asp:HiddenField ID="hdfExpiryDate" runat="server" ClientIDMode="Static"  />
                <asp:RequiredFieldValidator ID="rfvExpiryDate" runat="server" ControlToValidate="txtExpiryDate" CssClass="error" ValidationGroup="ReserveTickets" Display="Static" ClientIDMode="Static"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revExpiryDate" runat="server" ControlToValidate="txtExpiryDate" CssClass="error" ValidationGroup="ReserveTickets" Display="Static" Enabled="false"></asp:RegularExpressionValidator>
            </div>
        </div>
        <div class="row ebiz-comment">
            <div class="medium-3 columns">
                <asp:Label ID="lblComment" AssociatedControlID="txtComment" runat="server" />
            </div>
            <div class="medium-9 columns">
                <asp:TextBox ID="txtComment" runat="server" TabIndex="2" ClientIDMode="Static" />
            </div>
        </div>
        <asp:PlaceHolder ID="plhSaleOrReturn" runat="server">
          <div class="ebiz-sale-or-return-wrap">
            <asp:Checkbox ID="chkSaleOrReturn" runat="server" TabIndex="3" onclick="toggleSalesOrReturn(this);" ClientIDMode="Static" />
            <asp:Label ID="lblSaleOrReturn" AssociatedControlID="chkSaleOrReturn" runat="server"  />
          </div>
        </asp:PlaceHolder>   
        <div class="expanded button-group">
            <button  data-close aria-label="Close reveal" type="button" class="button ebiz-muted-action" OnClick="cancelReserve()"><%=CancelButtonText%></button>         
            <asp:button ID="btnReserve" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="ReserveTickets" TabIndex="4" OnClientClick="reserveButtonClick()" />
        </div>
    </asp:PlaceHolder>
    <button class="close-button" data-close aria-label="Close reveal" type="button" OnClick="cancelReserve()">
        <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
    </button>
</div>

<script>
    $(document).on('open.zf.reveal', '[data-reveal]', function () {
        $(".datetimepicker").first().focus();
    });
</script> 