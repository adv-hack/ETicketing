<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PartnerList.ascx.vb"
    Inherits="UserControls_PartnerList" %>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_partner ORDER BY PARTNER_DESC "></asp:SqlDataSource>
<table>
    <tr>
        <td>
            <div id="Maintenance">
                <asp:GridView ID="grdDataTable" DataKeyNames="PARTNER_ID" DataSourceID="SqlDataSource1"
                    AutoGenerateColumns="False" runat="server" AllowPaging="True" AllowSorting="True"
                    PageSize="20" PagerSettings-Mode="NextPreviousFirstLast" PagerSettings-Position="Bottom"
                    PagerStyle-HorizontalAlign="Right" Caption="<h2>Partner Maintenance</h2>" CaptionAlign="Left"
                    SelectedRowStyle-BackColor="#eeeeee" CellPadding="1" CellSpacing="1" HorizontalAlign="Left"
                    AutoGenerateEditButton="false" AutoGenerateDeleteButton="false" AutoGenerateSelectButton="false">
                    <Columns>
                        <asp:CommandField ShowSelectButton="True" />
                        <asp:BoundField SortExpression="PARTNER_ID" DataField="PARTNER_ID" ReadOnly="True"
                            HeaderText=" ID " />
                        <asp:BoundField SortExpression="PARTNER" DataField="PARTNER" HeaderText=" Partner " />
                        <asp:BoundField SortExpression="PARTNER_DESC" DataField="PARTNER_DESC" HeaderText=" Description " />
                        <asp:BoundField SortExpression="EMAIL" DataField="EMAIL" HeaderText=" Email " />
                        <asp:BoundField SortExpression="TELEPHONE_NUMBER" DataField="TELEPHONE_NUMBER" HeaderText=" Telephone" />
                        <asp:BoundField SortExpression="FAX_NUMBER" DataField="FAX_NUMBER" HeaderText=" Fax " />
                    </Columns>
                </asp:GridView>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <br />
            <br />
            <br />
            <div id="Partner2" class="Partner box2" runat="server">
                <asp:Button ID="cmdEdit" runat="server" Text="Edit" />
                <asp:Button ID="cmdDelete" runat="server" Text="Delete" />
                <asp:Button ID="cmdInsert" runat="server" Text="Insert" />
            </div>
        </td>
    </tr>
</table>
