<%@ Page Language="VB" AutoEventWireup="false" MasterPageFile="~/MasterPages/MasterPage.master" CodeFile="AddPartnersToBU.aspx.vb" Inherits="Partners_AddPartnersToBu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="Server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
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

    <div id="addPartnersToBU1">
        <p class="title">
            <asp:Label ID="lblHeader" runat="server"></asp:Label></p>
        <asp:GridView ID="grvPartners" DataSourceID="SqlPartnersNotInBU" CssClass="defaultTable"
            runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
            DataKeyNames="PARTNER_ID" PagerSettings-FirstPageText="First" PagerSettings-LastPageText="Last"
            PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous" PageSize="20">
            <Columns>
                <asp:BoundField DataField="PARTNER" HeaderText="Partner" InsertVisible="False" ReadOnly="True"
                    SortExpression="PARTNER" />
                <asp:BoundField DataField="PARTNER_DESC" HeaderText="Partner Description" InsertVisible="False"
                    ReadOnly="True" SortExpression="PARTNER_DESC" />
                <asp:TemplateField HeaderText="Select">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server" />
                    </ItemTemplate>
                    <HeaderTemplate>
                        <%-- <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server"
                                type="checkbox" />--%>
                    </HeaderTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                No Data to display</EmptyDataTemplate>
        </asp:GridView>
        <div class="addPartnersToBUBtn1">
            <asp:Button CssClass="button" ID="btnAdd" runat="server" Text="Add Partners to Business Unit" /></div>
        <asp:SqlDataSource ID="SqlPartnersNotInBU" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>">
        </asp:SqlDataSource>
        <p class="title">
            <asp:Label ID="lblHeader2" runat="server"></asp:Label></p>
        <div class="addPartnersToBU2">
            <asp:GridView ID="grvRemovePartners" DataSourceID="SqlRemovePartners" CssClass="defaultTable"
                runat="server" AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False"
                DataKeyNames="PARTNER_ID" PagerSettings-FirstPageText="First" PagerSettings-LastPageText="Last"
                PagerSettings-NextPageText="Next" PagerSettings-PreviousPageText="Previous" PageSize="20">
                <Columns>
                    <asp:HyperLinkField DataNavigateUrlFields="PARTNER_ID,BUSINESS_UNIT" DataNavigateUrlFormatString="~/Users/AddUsersToBu.aspx?PartnerID={0}&bu={1}"
                        Text="Users" />
                    <asp:BoundField DataField="PARTNER" HeaderText="Partner" InsertVisible="False" ReadOnly="True"
                        SortExpression="PARTNER" />
                    <asp:BoundField DataField="PARTNER_DESC" HeaderText="Partner Description" InsertVisible="False"
                        ReadOnly="True" SortExpression="PARTNER_DESC" />
                    <asp:TemplateField HeaderText="Select">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelect" runat="server" />
                        </ItemTemplate>
                        <HeaderTemplate>
                            <%--<input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server"
                                type="checkbox" />--%>
                        </HeaderTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    No Data to display</EmptyDataTemplate>
            </asp:GridView>
        </div>
        <div class="addPartnersToBUBtn2">
            <asp:Button CssClass="button" ID="btnRemove" runat="server" Text="Remove Partners from Business Unit" /></div>
        <asp:SqlDataSource ID="SqlRemovePartners" runat="server" SelectCommand="" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>">
        </asp:SqlDataSource>
        <p class="home">
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Default.aspx">Home</asp:HyperLink>
        </p>
    </div>
</asp:Content>
