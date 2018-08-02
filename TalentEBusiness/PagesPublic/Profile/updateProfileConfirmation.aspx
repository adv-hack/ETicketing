<%@ Page Language="VB" AutoEventWireup="false" CodeFile="updateProfileConfirmation.aspx.vb" Inherits="PagesLogin_updateProfileConfirmation" title="Untitled Page" %>

<%@ Register Src="../../UserControls/UpdateDetailsConfirmation.ascx"
    TagName="UpdateDetailsConfirmation" TagPrefix="uc1" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <uc1:UpdateDetailsConfirmation ID="UpdateDetailsConfirmation"
        runat="server" />
</asp:Content>

