<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UserEdit.ascx.vb" Inherits="UserControls_UserEdit" %>
<asp:SqlDataSource ID="InsertUserSource" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT *  FROM tbl_authorized_users  WHERE AUTH_USERS_ID = @AUTH_USERS_ID"
    DeleteCommand="DELETE FROM tbl_authorized_users WHERE AUTH_USERS_ID = @AUTH_USERS_ID"
    InsertCommand="INSERT INTO tbl_authorized_users (BUSINESS_UNIT, PARTNER, LOGINID, PASSWORD, 
                        AUTO_PROCESS_DEFAULT_USER, IS_APPROVED, IS_LOCKED_OUT, 
                        CREATED_DATE, LAST_LOGIN_DATE, LAST_PASSWORD_CHANGED_DATE, LAST_LOCKED_OUT_DATE ) VALUES (
                        @BUSINESS_UNIT, @PARTNER, @LOGINID, @PASSWORD, @AUTO_PROCESS_DEFAULT_USER, 
                        @IS_APPROVED, @IS_LOCKED_OUT, GETDATE(), GETDATE(), GETDATE(), GETDATE())"
    UpdateCommand="UPDATE tbl_authorized_users SET 
                        BUSINESS_UNIT = @BUSINESS_UNIT, 
                        PARTNER = @PARTNER, 
                        LOGINID = @LOGINID, 
                        PASSWORD = @PASSWORD, 
                        AUTO_PROCESS_DEFAULT_USER = @AUTO_PROCESS_DEFAULT_USER, 
                        IS_APPROVED = @IS_APPROVED, 
                        IS_LOCKED_OUT = @IS_LOCKED_OUT
                        WHERE AUTH_USERS_ID = @AUTH_USERS_ID">
    <SelectParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="AUTH_USERS_ID" QueryStringField="ID"
            Type="Int64" />
    </SelectParameters>
    <DeleteParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="AUTH_USERS_ID" QueryStringField="PID"
            Type="Int64" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="AUTH_USERS_ID" QueryStringField="ID"
            Type="Int64" />
        <asp:Parameter Name="BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="PARTNER" Type="String" />
        <asp:Parameter Name="LOGINID" Type="String" />
        <asp:Parameter Name="PASSWORD" Type="String" />
        <asp:Parameter Name="AUTO_PROCESS_DEFAULT_USER" Type="Boolean" />
        <asp:Parameter Name="IS_APPROVED" Type="Boolean" />
        <asp:Parameter Name="IS_LOCKED_OUT" Type="Boolean" />
        <asp:Parameter Name="AUTH_USERS_ID" Type="Int64" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="PARTNER" Type="String" />
        <asp:Parameter Name="LOGINID" Type="String" />
        <asp:Parameter Name="PASSWORD" Type="String" />
        <asp:Parameter Name="AUTO_PROCESS_DEFAULT_USER" Type="Boolean" />
        <asp:Parameter Name="IS_APPROVED" Type="Boolean" />
        <asp:Parameter Name="IS_LOCKED_OUT" Type="Boolean" />
    </InsertParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_bu ORDER BY BUSINESS_UNIT">
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_partner ORDER BY PARTNER">
</asp:SqlDataSource>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Maintenance" />
</p>
<div id="Maintenance">
    <asp:FormView ID="frmDataTable" runat="server" DataKeyNames="AUTH_USERS_ID" DataSourceID="InsertUserSource">
        <EditItemTemplate>
            <div id="BusinessUnit" class="UnitRow" runat="server">
                <label for="BusinessUnit">
                    <asp:Label ID="lblBusinessUnit" runat="server">Business Unit</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtBusinessUnit" CssClass="input-l" runat="server"
                    AutoPostBack="true" Text='<%# Bind("BUSINESS_UNIT") %>' OnDataBinding="txtBusinessUnit_TextChanged"
                    OnTextChanged="txtBusinessUnit_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboBusinessUnit" CssClass="select" runat="server" OnInit="cboBusinessUnit_Load"
                    AutoPostBack="true" OnTextChanged="cboBusinessUnit_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtBusinessUnit" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtPartner" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("PARTNER") %>' OnDataBinding="txtPartner_TextChanged" OnTextChanged="txtPartner_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboPartner" CssClass="select" runat="server" OnInit="cboPartner_Load"
                    AutoPostBack="true" OnTextChanged="cboPartner_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="LoginId" class="LoginIdRow" runat="server">
                <label for="LoginId">
                    <asp:Label ID="lblLoginId" runat="server">LoginId</asp:Label></label>
                <asp:TextBox ID="txtLoginId" CssClass="input-l" runat="server" Text='<%# Bind("LOGINID") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtLoginId" ID="RequiredFieldValidator4"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Password" class="PasswordRow" runat="server">
                <label for="Password">
                    <asp:Label ID="lblPassword" runat="server">Password</asp:Label></label>
                <asp:TextBox ID="txtPassword" CssClass="input-l" runat="server" Text='<%# Bind("PASSWORD") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPassword" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Auto" class="AutoRow" runat="server">
                <label for="Auto">
                    <asp:Label ID="lblAuto" runat="server">Auto Process Default User</asp:Label></label>
                <asp:CheckBox ID="chkAuto" runat="server" Checked='<%# Bind("AUTO_PROCESS_DEFAULT_USER") %>' />
            </div>
            <div id="Approved" class="ApprovedRow" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblApproved" runat="server">Approved</asp:Label></label>
                <asp:CheckBox ID="chkApproved" runat="server" Checked='<%# Bind("IS_APPROVED")  %>' />
            </div>
            <div id="Locked" class="LockedRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblLocked" runat="server">Locked Out</asp:Label></label>
                <asp:CheckBox ID="chkLocked" runat="server" Checked='<%#  Bind("IS_LOCKED_OUT")  %>' />
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
            <div id="BusinessUnit" class="UnitRow" runat="server">
                <label for="BusinessUnit">
                    <asp:Label ID="lblBusinessUnit" runat="server">Business Unit</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtBusinessUnit" CssClass="input-l" runat="server"
                    AutoPostBack="true" Text='<%# Bind("BUSINESS_UNIT") %>' OnDataBinding="txtBusinessUnit_TextChanged"
                    OnTextChanged="txtBusinessUnit_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboBusinessUnit" CssClass="select" runat="server" OnInit="cboBusinessUnit_Load"
                    AutoPostBack="true" OnTextChanged="cboBusinessUnit_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtBusinessUnit" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtPartner" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("PARTNER") %>' OnDataBinding="txtPartner_TextChanged" OnTextChanged="txtPartner_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboPartner" CssClass="select" runat="server" OnInit="cboPartner_Load"
                    AutoPostBack="true" OnTextChanged="cboPartner_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="LoginId" class="LoginIdRow" runat="server">
                <label for="LoginId">
                    <asp:Label ID="lblLoginId" runat="server">LoginId</asp:Label></label>
                <asp:TextBox ID="txtLoginId" CssClass="input-l" runat="server" Text='<%# Bind("LOGINID") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtLoginId" ID="RequiredFieldValidator4"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Password" class="PasswordRow" runat="server">
                <label for="Password">
                    <asp:Label ID="lblPassword" runat="server">Password</asp:Label></label>
                <asp:TextBox ID="txtPassword" CssClass="input-l" runat="server" Text='<%# Bind("PASSWORD") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPassword" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Auto" class="AutoRow" runat="server">
                <label for="Auto">
                    <asp:Label ID="lblAuto" runat="server">Auto Process Default User</asp:Label></label>
                <asp:CheckBox ID="chkAuto" runat="server" Checked='<%# Bind("AUTO_PROCESS_DEFAULT_USER")  %>' />
            </div>
            <div id="Approved" class="ApprovedRow" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblApproved" runat="server">Approved</asp:Label></label>
                <asp:CheckBox ID="chkApproved" runat="server" Checked='<%# Bind("IS_APPROVED")  %>' />
            </div>
            <div id="Locked" class="LockedRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblLocked" runat="server">Locked Out</asp:Label></label>
                <asp:CheckBox ID="chkLocked" runat="server" Checked='<%# Bind("IS_LOCKED_OUT")  %>' />
            </div>
            <br />
            <br />
            <br />
            <asp:LinkButton ID="InsertButton" runat="server" CausesValidation="True" ValidationGroup="Maintenance"
                CommandName="Insert" Text="Insert" OnClick="InsertButton_Click"></asp:LinkButton>
            <asp:LinkButton ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel"
                Text="Cancel"></asp:LinkButton>
        </InsertItemTemplate>
        <ItemTemplate>
            <div id="BusinessUnit" class="BusinessUnitRow" runat="server">
                <label for="Unit">
                    <asp:Label ID="lblBusinessUnit" runat="server">Unit</asp:Label></label>
                <asp:TextBox ID="txtBusinessUnit" CssClass="input-l" runat="server" Text='<%# Bind("BUSINESS_UNIT") %>'></asp:TextBox>
            </div>
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox ID="txtPartner" CssClass="input-l" runat="server" ReadOnly="True" Text='<%# Bind("PARTNER") %>'></asp:TextBox>
            </div>
            <div id="LoginId" class="LoginIdRow" runat="server">
                <label for="LoginId">
                    <asp:Label ID="lblLoginId" runat="server">LoginId</asp:Label></label>
                <asp:TextBox ID="txtLoginId" CssClass="input-l" runat="server" ReadOnly="True" Text='<%# Bind("LOGINID") %>'></asp:TextBox>
            </div>
            <div id="Password" class="PasswordRow" runat="server">
                <label for="Password">
                    <asp:Label ID="lblPassword" runat="server">Password</asp:Label></label>
                <asp:TextBox ID="txtPassword" CssClass="input-l" runat="server" ReadOnly="True" Text='<%# Bind("PASSWORD") %>'></asp:TextBox>
            </div>
            <div id="Auto" class="AutoRow" runat="server">
                <label for="Auto">
                    <asp:Label ID="lblAuto" runat="server">Auto Process Default User</asp:Label></label>
                <asp:CheckBox ID="chkAuto" runat="server" Enabled='False' Checked='<%# Bind("AUTO_PROCESS_DEFAULT_USER")  %>' />
            </div>
            <div id="Approved" class="ApprovedRow" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblApproved" runat="server">Approved</asp:Label></label>
                <asp:CheckBox ID="chkApproved" runat="server" Enabled='False' Checked='<%# Bind("IS_APPROVED")  %>' />
            </div>
            <div id="Locked" class="LockedRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblLocked" runat="server">Locked Out</asp:Label></label>
                <asp:CheckBox ID="chkLocked" runat="server" Enabled='False' Checked='<%# Bind("IS_LOCKED_OUT")  %>' />
            </div>
            <br />
            <br />
            <br />
            <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit"
                Text="Edit"></asp:LinkButton>
             <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False" CommandName="Delete" Text="Delete"
                 OnClientClick="return confirm('Are you certain you want to delete this record?');"></asp:LinkButton>
            <asp:LinkButton ID="NewButton" runat="server" CausesValidation="False" CommandName="New"
                Text="Insert"></asp:LinkButton>
        </ItemTemplate>
    </asp:FormView>
</div>
