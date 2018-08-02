<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master"
    CodeFile="AddModulesToPartner.aspx.vb" Inherits="Modules_AddModulesToPartner" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Content2" runat="Server">

    <script language="javascript" type="text/javascript">

 function SelectAllCheckboxes(spanChk){

   // Added as ASPX uses SPAN for checkbox
   var oItem = spanChk.children;
   var theBox= (spanChk.type=="checkbox") ? 
        spanChk : spanChk.children.item[0];
   xState=theBox.checked;
   elm=theBox.form.elements;

   for(i=0;i<elm.length;i++)
     if(elm[i].type=="checkbox" && 
              elm[i].id!=theBox.id)
     {
       //elm[i].click();
       if(elm[i].checked!=xState)
         elm[i].click();
       //elm[i].checked=xState;
     }
 }
    </script>

    <div id="addModulesToPartners1">
        <p class="title">
            <asp:Label ID="lblHeader" runat="server"></asp:Label></p>
        <asp:GridView ID="grvModules" CssClass="defaultTable" DataSourceID="SqlModulesNotForPartner"
            runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
            DataKeyNames="MODULE" PagerSettings-FirstPageText="First" PagerSettings-LastPageText="Last"
            PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous" PageSize="20">
            <Columns>
                <asp:BoundField DataField="MODULE" HeaderText="Modules" InsertVisible="False" ReadOnly="True"
                    SortExpression="PARTNER" />
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                    <HeaderTemplate>
                        <%--       <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server"
                                type="checkbox" />--%>
                    </HeaderTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No Data to display</EmptyDataTemplate>
        </asp:GridView>
        <div class="addModulesToPartners2">
            <asp:Button CssClass="button" ID="btnAdd" runat="server" Text="Add Modules to Partner" /></div>
        <asp:SqlDataSource ID="SqlModulesNotForPartner" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>">
        </asp:SqlDataSource>
        <p class="title">
            <asp:Label ID="lblHeader2" runat="server"></asp:Label></p>
        <asp:GridView ID="grvRemoveModules" CssClass="defaultTable" DataSourceID="SqlRemoveModules"
            runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
            DataKeyNames="SUPPLYNET_MP_ID" PagerSettings-FirstPageText="First" PagerSettings-LastPageText="Last"
            PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous" PageSize="20"
            PagerSettings-Mode="NumericFirstLast">
            <Columns>
                <asp:HyperLinkField DataNavigateUrlFields="SUPPLYNET_MP_ID, PARTNER_ID" DataNavigateUrlFormatString="~/Modules/EditPartnerModule.aspx?ID={0}&PartnerID={1}"
                    Text="Edit" />
                <asp:BoundField DataField="MODULE" HeaderText="Module" InsertVisible="False" ReadOnly="True"
                    SortExpression="MODULE" />
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                    <HeaderTemplate>
                        <%--  <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server"
                                type="checkbox" />--%>
                    </HeaderTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataRowStyle />
            <EmptyDataTemplate>
                No Data to display</EmptyDataTemplate>
        </asp:GridView>
        <div class="addModulesToPartners3">
            <asp:Button CssClass="button" ID="btnRemove" runat="server" Text="Remove Modules from Partner" /></div>
        <asp:SqlDataSource ID="SqlRemoveModules" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>">
        </asp:SqlDataSource>
        <p class="home">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Home</asp:HyperLink>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/Modules/SelectPartner.aspx">Cancel</asp:HyperLink>
        </p>
    </div>
</asp:Content>
