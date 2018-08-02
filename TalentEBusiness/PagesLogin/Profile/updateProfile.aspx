<%@ Page Language="VB" AutoEventWireup="false" CodeFile="updateProfile.aspx.vb" Inherits="PagesLogin_updateProfile" title="Untitled Page" EnableEventValidation="false" %>

    <%@ Register Src="../../UserControls/UpdateDetailsForm.ascx" TagName="UpdateDetailsForm"
    TagPrefix="Talent" %>
    <%@ Register Src="../../UserControls/UpdateDetailsForm2.ascx" TagName="UpdateDetailsForm2"
    TagPrefix="Talent" %>
    <%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText"
    TagPrefix="Talent" %>
    <%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
    <%@ Register Src="~/UserControls/Company/CustomerCompany.ascx" TagPrefix="Talent" TagName="CustomerCompany" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
   <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
     <Talent:CustomerCompany runat="server" id="CustomerCompany" />
    <asp:PlaceHolder ID="plhUpdateDetailsForm1" runat="server">
        <Talent:UpdateDetailsForm ID="UpdateDetailsForm1" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="plhUpdateDetailsForm2" runat="server">
        <Talent:UpdateDetailsForm2 ID="UpdateDetailsForm2" runat="server" />
    </asp:PlaceHolder>
</asp:Content>

