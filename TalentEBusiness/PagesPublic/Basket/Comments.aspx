<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Comments.aspx.vb" Inherits="PagesPublic_Basket_Comments" Theme="" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <talent:pageheadertext id="PageHeaderText1" runat="server" />
    <talent:htmlinclude id="HTMLInclude1" runat="server" usage="2" sequence="1" />
    <div class="panel ebiz-comments">
        <asp:Label runat="server" AssociatedControlID="txtComments" ID="lblComments" />
        <asp:RegularExpressionValidator ID="rgxComments" runat="server" ControlToValidate="txtComments" CssClass="error" Display="Static" />
        <asp:TextBox TextMode="MultiLine" ID="txtComments" runat="server" CssClass="ebiz-comments-text-box" />
        <noscript>              
            <asp:Button ID="btApplyCommentsNoScript" runat="server" CssClass="button ebiz-comments-apply" />
        </noscript>
        <asp:Button ID="btApplyComments" runat="server" CssClass="button apply comments-apply" />
        <script type="text/javascript">               
            document.getElementById('<%=btApplyComments.ClientID %>').style.display = 'inline';
        </script>    
    </div>
    <button class="close-button" data-close aria-label="Close modal" type="button">
      <span aria-hidden="true"><i class="fa fa-times"></i></span>
    </button>
</asp:Content>
 