<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UnreserveTickets.ascx.vb" Inherits="UserControls_UnreserveTickets" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/BasketDetails.ascx" %>

<div id="unreserve-button" class="reveal ebiz-unreserve-ticket-wrap" data-reveal>
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <Talent:HTMLInclude ID="uscHTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder runat="server" ID="plhErrorMessage" Visible="false">
        <div class="alert-box alert ebiz-reservation-error-message">
            <asp:BulletedList id="blErrorDetails" runat="server"></asp:BulletedList>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhUnreserveTickets" runat="server" Visible="false">
        <div class="row">
            <div class="medium-3 columns">
                <asp:Label ID="lblComment" AssociatedControlID="txtComment" runat="server" />
            </div>
           <div class="medium-9 columns">
                <asp:TextBox ID="txtComment" runat="server" CssClass="ebiz-comment" />
               <asp:RequiredFieldValidator runat="server" ID="rfvComment" ControlToValidate="txtComment" CssClass="error" ValidationGroup="UnreserveTickets" Display="Static" ClientIDMode="Static" Enabled="false"></asp:RequiredFieldValidator>
           </div>
        </div>
        <div class="expanded button-group">
            <asp:hyperlink ID="btnCancel" runat="server" CssClass="button ebiz-muted-action" data-close aria-label="Close reveal" />
            <asp:button ID="btnUnreserve" runat="server" CssClass="button ebiz-primary-action" ValidationGroup="UnreserveTickets" />
        </div>
    </asp:PlaceHolder>
    <button class="close-button" data-close aria-label="Close reveal" type="button">
        <span aria-hidden="true"><i class="fa fa-times" aria-hidden="true"></i></span>
    </button>
</div>

<script>
    $(document).on('open.zf.reveal', '[data-reveal]', function () {
        $("input.ebiz-comment").first().focus();
    });
</script> 