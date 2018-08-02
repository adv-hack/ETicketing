<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master"
    CodeFile="ModuleDetails.aspx.vb" Inherits="Modules_ModuleDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">
   
<asp:ValidationSummary ID="vldValidationSummary" runat="server" />
<asp:DetailsView ID="DetailsView1" runat="server" Height="50px" Width="125px" enableinserting=>
</asp:DetailsView>

</asp:Content>
