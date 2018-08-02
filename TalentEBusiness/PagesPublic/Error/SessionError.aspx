<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SessionError.aspx.vb" Inherits="PagesPublic_Error_SessionError" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    
    <asp:PlaceHolder ID="plhSessionErrorType" runat="server">
        <div class="alert-box alert">
            <asp:Literal ID="ltlSessionErrTypeMess" runat="server" />
        </div>
    </asp:PlaceHolder>

</asp:Content>