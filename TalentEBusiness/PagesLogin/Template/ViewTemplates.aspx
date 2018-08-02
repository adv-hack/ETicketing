<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ViewTemplates.aspx.vb" Inherits="PagesLogin_Template_ViewTemplates" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/OrderTemplatesHeader.ascx" TagName="OrderTemplatesHeader" TagPrefix="Talent" %>
<%@ Register Src="../../UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <Talent:OrderTemplatesHeader id="OrderTemplatesHeader1" runat="server" Usage="TEMPLATES" ShowOptions="true" />

    <asp:Repeater ID="rptOrderTemplatesForFandF" runat="server">
        <HeaderTemplate>
            <div class="fandf-order-templates">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="fandf-order-template">
                <p class="order-template-name"><%#OrderTemplateForText %> <%# DataBinder.Eval(Container.DataItem, "Forename").ToString() %> <%# DataBinder.Eval(Container.DataItem, "Surname").ToString() %></p>
                <Talent:OrderTemplatesHeader ID="uscOrderTemplatesHeaderForFandF" runat="server" Usage="TEMPLATES" ShowOptions="false" 
                    CustomerNumber='<%# DataBinder.Eval(Container.DataItem, "AssociatedCustomerNumber").ToString() %>' />
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>