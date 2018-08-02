<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BusinessUnitEdit.ascx.vb"
    Inherits="UserControls_BusinessUnitEditl" %>
<asp:SqlDataSource ID="InsertBusinessUnitSource" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT BUSINESS_UNIT_ID, BUSINESS_UNIT, BUSINESS_UNIT_DESC FROM tbl_bu WHERE BUSINESS_UNIT_ID = @BUSINESS_UNIT_ID"
    DeleteCommand="DELETE FROM tbl_bu WHERE BUSINESS_UNIT_ID = @BUSINESS_UNIT_ID"
    InsertCommand="INSERT INTO tbl_bu (BUSINESS_UNIT, BUSINESS_UNIT_DESC ) VALUES ( @BUSINESS_UNIT, @BUSINESS_UNIT_DESC )"
    UpdateCommand="UPDATE tbl_bu SET  BUSINESS_UNIT = @BUSINESS_UNIT, BUSINESS_UNIT_DESC = @BUSINESS_UNIT_DESC WHERE BUSINESS_UNIT_ID = @BUSINESS_UNIT_ID">
    <SelectParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="BUSINESS_UNIT_ID" QueryStringField="ID"
            Type="Int64" />
    </SelectParameters>
    <DeleteParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="BUSINESS_UNIT_ID" QueryStringField="ID"
            Type="Int64" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="BUSINESS_UNIT_ID" QueryStringField="ID"
            Type="Int64" />
        <asp:Parameter Name="BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="BUSINESS_UNIT_DESC" Type="String" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="BUSINESS_UNIT_DESC" Type="String" />
    </InsertParameters>
</asp:SqlDataSource>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Maintenance" />
</p>
<div id="Maintenance">
    <asp:FormView ID="InsertBusinessUnitForm" runat="server" DataKeyNames="BUSINESS_UNIT_ID"
        DataSourceID="InsertBusinessUnitSource">
        <EditItemTemplate>
            <div id="BusinessUnit" class="BusinessUnitRow" runat="server">
                <label for="BUSINESS_UNIT">
                    <asp:Label ID="lblBUSINESS_UNIT" runat="server">Business Unit</asp:Label></label>
                <asp:TextBox ID="txtBUSINESS_UNIT" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtBUSINESS_UNIT" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT_DESC") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtDescription" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <br />
            <br />
            <br />
            <asp:LinkButton ID="UpdateButton" runat="server" CausesValidation="True" ValidationGroup="Maintenance"
                CommandName="Update" Text="Update"> </asp:LinkButton>
            <asp:LinkButton ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                Text="Cancel"> </asp:LinkButton>
        </EditItemTemplate>
        <InsertItemTemplate>
            <div id="BUSINESS_UNIT" class="BusinessUnitRow" runat="server">
                <label for="BUSINESS_UNIT">
                    <asp:Label ID="lblBUSINESS_UNIT" runat="server">Business Unit</asp:Label></label>
                <asp:TextBox ID="txtBUSINESS_UNIT" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtBUSINESS_UNIT" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT_DESC") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtDescription" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <br />
            <br />
            <br />
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" ValidationGroup="Maintenance"
                CommandName="Insert" Text="Insert"></asp:LinkButton>
            <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                Text="Cancel"></asp:LinkButton>
        </InsertItemTemplate>
        <ItemTemplate>
            <div id="BUSINESS_UNIT" class="BusinessUnitRow" runat="server">
                <label for="BUSINESS_UNIT">
                    <asp:Label ID="lblBUSINESS_UNIT" runat="server">Business Unit</asp:Label></label>
                <asp:TextBox ID="txtBUSINESS_UNIT" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT") %>'></asp:TextBox>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT_DESC") %>'></asp:TextBox>
            </div>
            <br />
            <br />
            <br />
            <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit"
                Text="Edit"></asp:LinkButton>
            <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete"
                Text="Delete" OnClientClick="return confirm('Are you certain you want to delete this record?');"></asp:LinkButton>
            <asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New"
                Text="Insert"></asp:LinkButton>
        </ItemTemplate>
    </asp:FormView>
</div>
