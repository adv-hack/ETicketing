<%@ Page Language="VB" 
MasterPageFile="~/MasterPages/masterpage.master" 
AutoEventWireup="false" 
CodeFile="AdhocGroupMaintenance.aspx.vb" 
Inherits="Navigation_AdhocGroupMaintenance" title="Untitled Page" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">
    <div id="pageMaintenanceTopNavigation">
       <asp:BulletedList ID="navigationOptions" runat="server" DisplayMode="HyperLink">
            <asp:listItem Text="Maintenance Portal" Value="../MaintenancePortal.aspx" />
       </asp:BulletedList>
    </div>

    <div id="pageOverview1">
        <p class="title"><asp:Label ID="titleLabel" runat="server" Text="PageLabel" /></p>
        <p class="instructions"><asp:Label ID="instructionsLabel" runat="server" Text="" /></p>
        <p class="error"><asp:Label ID="ErrorLabel" runat="server" /></p>
        
        <div class="pageOverview2">
            <table cellspacing="0" class="defaultTable">
                <tbody>
                    <tr>
                        <th class="label" scope="row">
                            <asp:Label ID="pageLabel" runat="server" Text="Select Group" />
                        </th>
                        <td class="element">
                            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="" TypeName="NavigationDataSetTableAdapters.tbl_groupTableAdapter">
                                <SelectParameters></SelectParameters>
                            </asp:ObjectDataSource>
                            <asp:DropDownList CssClass="select" ID="PageDDL" runat="server" AutoPostBack="True" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
            
        <div class="pageOverviewButtons">
            <asp:Button CssClass="button" ID="CancelButton" runat="server" Text="Return To Group Navigation" />
            <asp:Button CssClass="button" ID="AddNewPageButton" runat="server" Text="Add A New Group" />
        </div>
    </div>
</asp:Content>