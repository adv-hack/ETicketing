<%@ Page Language="VB"  AutoEventWireup="false"
    CodeFile="miscellaneous10.aspx.vb" Inherits="PagesPublic_miscellaneous10"
    Title="Untitled Page" %>

<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
  
    <a href="#" class="js-printer-confirm"><i class="fa fa-print" aria-hidden="true"></i></a>
  

    <script>
      $(function () {
        var text = '<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p><label for="">Ticket Printer Default</label><select name="" id="JaneDoe"><option value="none">Please select a new ticket printer default...</option><option value="BOCAPRT" selected="selected">BOCAPRT</option><option value= "QPRINT" > qprint</option></select >';
        $(".js-printer-confirm").on('click', function (e) {
          alertify.confirm('Confirm Title', text, function () { alertify.success('Sent to ' + $('#JaneDoe').val()) }, function () { alertify.error('Cancel') });
        });
      });
    </script>

</asp:Content>
