<%@ Control Language="VB" AutoEventWireup="false" CodeFile="UserAddressEdit.ascx.vb"
    Inherits="UserControls_UserAddressEdit" %>
<asp:SqlDataSource ID="InsertUserSource" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_address WHERE ADDRESS_ID = @ADDRESS_ID" DeleteCommand="DELETE FROM  tbl_address WHERE ADDRESS_ID = @ADDRESS_ID "
    InsertCommand="INSERT INTO  tbl_address (PARTNER, LOGINID, TYPE, REFERENCE, SEQUENCE, DEFAULT_ADDRESS, 
                        ADDRESS_LINE_1, ADDRESS_LINE_2, ADDRESS_LINE_3, ADDRESS_LINE_4, ADDRESS_LINE_5, 
                        POST_CODE, COUNTRY ) VALUES (@PARTNER, @LOGINID, @TYPE, @REFERENCE, 
                        @SEQUENCE, @DEFAULT_ADDRESS, 
                        @ADDRESS_LINE_1, @ADDRESS_LINE_2, @ADDRESS_LINE_3, @ADDRESS_LINE_4, @ADDRESS_LINE_5, 
                        @POST_CODE, @COUNTRY" UpdateCommand="UPDATE tbl_address SET 
                        TYPE = @TYPE, 
                        REFERENCE = @REFERENCE, 
                        SEQUENCE = @SEQUENCE, 
                        DEFAULT_ADDRESS = @DEFAULT_ADDRESS,
                        ADDRESS_LINE_1 = @ADDRESS_LINE_1,
                        ADDRESS_LINE_2 = @ADDRESS_LINE_2,
                        ADDRESS_LINE_3 = @ADDRESS_LINE_3,
                        ADDRESS_LINE_4 = @ADDRESS_LINE_4,
                        ADDRESS_LINE_5 = @ADDRESS_LINE_5,
                        POST_CODE = @POST_CODE,
                        COUNTRY = @COUNTRY
                        WHERE ADDRESS_ID = @ADDRESS_ID ">
    <SelectParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="ADDRESS_ID" QueryStringField="ID"
            Type="Int64" />
    </SelectParameters>
    <DeleteParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="ADDRESS_ID" QueryStringField="ID"
            Type="Int64" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="ADDRESS_ID" QueryStringField="ID"
            Type="Int64" />
        <asp:Parameter Name="TYPE" Type="String" />
        <asp:Parameter Name="REFERENCE" Type="String" />
        <asp:Parameter Name="SEQUENCE" Type="String" />
        <asp:Parameter Name="DEFAULT_ADDRESS" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_1" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_2" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_3" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_4" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_5" Type="String" />
        <asp:Parameter Name="POST_CODE" Type="String" />
        <asp:Parameter Name="COUNTRY" Type="String" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="PARTNER" Type="String" />
        <asp:Parameter Name="LOGINID" Type="String" />
        <asp:Parameter Name="TYPE" Type="String" />
        <asp:Parameter Name="REFERENCE" Type="String" />
        <asp:Parameter Name="SEQUENCE" Type="String" />
        <asp:Parameter Name="DEFAULT_ADDRESS" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_1" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_2" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_3" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_4" Type="String" />
        <asp:Parameter Name="ADDRESS_LINE_5" Type="String" />
        <asp:Parameter Name="POST_CODE" Type="String" />
        <asp:Parameter Name="COUNTRY" Type="String" />
    </InsertParameters>
</asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_country ORDER BY COUNTRY_DESCRIPTION"></asp:SqlDataSource>
<asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT * FROM tbl_partner ORDER BY PARTNER"></asp:SqlDataSource>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Maintenance" />
</p>
<div id="Maintenance">
    <asp:FormView ID="frmDataTable" runat="server" DataKeyNames="ADDRESS_ID" DataSourceID="InsertUserSource">
        <EditItemTemplate>
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtPartner" CssClass="input-l" runat="server"  Text='<%# Bind("PARTNER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="LoginId" class="LoginIdRow" runat="server">
                <label for="LoginId">
                    <asp:Label ID="lblLoginId" runat="server">LoginId</asp:Label></label>
                <asp:TextBox ID="txtLoginId" CssClass="input-l" runat="server" ReadOnly="True" Text='<%# Bind("LOGINID") %>'></asp:TextBox>
            </div>
            <div id="TYPE" class="TypeRow" runat="server">
                <label for="TYPE">
                    <asp:Label ID="lblType" runat="server">Type</asp:Label></label>
                <asp:TextBox ID="txtTYPE" CssClass="input-l" runat="server" Text='<%# Bind("TYPE") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtType" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Reference" class="ReferenceRow" runat="server">
                <label for="Reference">
                    <asp:Label ID="lblReference" runat="server">Reference</asp:Label></label>
                <asp:TextBox ID="txtReference" runat="server" Text='<%# Bind("REFERENCE") %>' />
            </div>
            <div id="Sequence" class="SequenceRow" runat="server">
                <label for="Sequence">
                    <asp:Label ID="lblSequence" runat="server">Sequence</asp:Label></label>
                <asp:TextBox ID="txtSequence" runat="server" Text='<%# Bind("SEQUENCE") %>' />
            </div>
            <div id="Default_Address" class="Default_AddressRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblDefaultAddress" runat="server">Default Address</asp:Label></label>
                <asp:CheckBox ID="chkDefaultAddress" runat="server" Checked='<%#  Bind("DEFAULT_ADDRESS")  %>' />
            </div>
            <div id="AddressLine1" class="ReferenceRow" runat="server">
                <label for="AddressLine1">
                    <asp:Label ID="lblAddressLine1" runat="server">Address line 1</asp:Label></label>
                <asp:TextBox ID="txtAddressLine1" runat="server" Text='<%# Bind("ADDRESS_LINE_1") %>' />
            </div>
            <div id="AddressLine2" class="AddressLine2Row" runat="server">
                <label for="AddressLine2">
                    <asp:Label ID="lblAddressLine2" runat="server">Address line 2</asp:Label></label>
                <asp:TextBox ID="txtAddressLine2" runat="server" Text='<%# Bind("ADDRESS_LINE_2") %>' />
            </div>
            <div id="AddressLine3" class="AddressLine3Row" runat="server">
                <label for="AddressLine3">
                    <asp:Label ID="lblAddressLine3" runat="server">Address line 3</asp:Label></label>
                <asp:TextBox ID="txtAddressLine3" runat="server" Text='<%# Bind("ADDRESS_LINE_3") %>' />
            </div>
            <div id="AddressLine4" class="AddressLine4Row" runat="server">
                <label for="AddressLine4">
                    <asp:Label ID="lblAddressLine4" runat="server">Address line 4</asp:Label></label>
                <asp:TextBox ID="txtAddressLine4" runat="server" Text='<%# Bind("ADDRESS_LINE_4") %>' />
            </div>
            <div id="AddressLine5" class="AddressLine5Row" runat="server">
                <label for="AddressLine5">
                    <asp:Label ID="lblAddressLine5" runat="server">Address line 5</asp:Label></label>
                <asp:TextBox ID="txtAddressLine5" runat="server" Text='<%# Bind("ADDRESS_LINE_5") %>' />
            </div>
            <div id="PostCode" class="PostCodeRow" runat="server">
                <label for="PostCode">
                    <asp:Label ID="lblPostCode" runat="server">PostCode</asp:Label></label>
                <asp:TextBox ID="txtPostCode" runat="server" Text='<%# Bind("POST_CODE") %>' />
            </div>
            <div id="Country" class="CountryRow" runat="server">
                <label for="Country">
                    <asp:Label ID="lblCountry" runat="server">Country</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtCountry" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("COUNTRY") %>' OnDataBinding="txtCountry_TextChanged" OnTextChanged="txtCountry_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboCountry" CssClass="select" runat="server" OnInit="cboCountry_Load"
                    AutoPostBack="true" OnTextChanged="cboCountry_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtCountry" ID="RequiredFieldValidator3"
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
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtPartner" CssClass="input-l" runat="server"  Text='<%# Bind("PARTNER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator1"
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
            <div id="Type" class="TypeRow" runat="server">
                <label for="Type">
                    <asp:Label ID="lblType" runat="server">Type</asp:Label></label>
                <asp:TextBox ID="txtTYPE" CssClass="input-l" runat="server" Text='<%# Bind("TYPE") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtType" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Reference" class="ReferenceRow" runat="server">
                <label for="Reference">
                    <asp:Label ID="lblReference" runat="server">Reference</asp:Label></label>
                <asp:TextBox ID="txtReference" runat="server" Text='<%# Bind("REFERENCE") %>' />
            </div>
            <div id="Sequence" class="SequenceRow" runat="server">
                <label for="Sequence">
                    <asp:Label ID="lblSequence" runat="server">Sequence</asp:Label></label>
                <asp:TextBox ID="txtSequence" runat="server" Text='<%# Bind("SEQUENCE") %>' />
            </div>
            <div id="Default_Address" class="Default_AddressRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblDefaultAddress" runat="server">Default Address</asp:Label></label>
                <asp:CheckBox ID="chkDefaultAddress" runat="server" Checked='<%#  Bind("DEFAULT_ADDRESS")  %>' />
            </div>
            <div id="AddressLine1" class="ReferenceRow" runat="server">
                <label for="AddressLine1">
                    <asp:Label ID="lblAddressLine1" runat="server">Address line 1</asp:Label></label>
                <asp:TextBox ID="txtAddressLine1" runat="server" Text='<%# Bind("ADDRESS_LINE_1") %>' />
            </div>
            <div id="AddressLine2" class="AddressLine2Row" runat="server">
                <label for="AddressLine2">
                    <asp:Label ID="lblAddressLine2" runat="server">Address line 2</asp:Label></label>
                <asp:TextBox ID="txtAddressLine2" runat="server" Text='<%# Bind("ADDRESS_LINE_2") %>' />
            </div>
            <div id="AddressLine3" class="AddressLine3Row" runat="server">
                <label for="AddressLine3">
                    <asp:Label ID="lblAddressLine3" runat="server">Address line 3</asp:Label></label>
                <asp:TextBox ID="txtAddressLine3" runat="server" Text='<%# Bind("ADDRESS_LINE_3") %>' />
            </div>
            <div id="AddressLine4" class="AddressLine4Row" runat="server">
                <label for="AddressLine4">
                    <asp:Label ID="lblAddressLine4" runat="server">Address line 4</asp:Label></label>
                <asp:TextBox ID="txtAddressLine4" runat="server" Text='<%# Bind("ADDRESS_LINE_4") %>' />
            </div>
            <div id="AddressLine5" class="AddressLine5Row" runat="server">
                <label for="AddressLine5">
                    <asp:Label ID="lblAddressLine5" runat="server">Address line 5</asp:Label></label>
                <asp:TextBox ID="txtAddressLine5" runat="server" Text='<%# Bind("ADDRESS_LINE_5") %>' />
            </div>
            <div id="PostCode" class="PostCodeRow" runat="server">
                <label for="PostCode">
                    <asp:Label ID="lblPostCode" runat="server">PostCode</asp:Label></label>
                <asp:TextBox ID="txtPostCode" runat="server" Text='<%# Bind("POST_CODE") %>' />
            </div>
            <div id="Country" class="CountryRow" runat="server">
                <label for="Country">
                    <asp:Label ID="lblCountry" runat="server">Country</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtCountry" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("COUNTRY") %>' OnDataBinding="txtCountry_TextChanged" OnTextChanged="txtCountry_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="DropDownList1" CssClass="select" runat="server" OnInit="cboCountry_Load"
                    AutoPostBack="true" OnTextChanged="cboCountry_Load">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ControlToValidate="txtCountry" ID="RequiredFieldValidator3"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
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
            <div id="TYPE" class="TypeRow" runat="server">
                <label for="TYPE">
                    <asp:Label ID="lblType" runat="server">Type</asp:Label></label>
                <asp:TextBox ID="txtTYPE" CssClass="input-l" runat="server" ReadOnly="True" Text='<%# Bind("TYPE") %>'></asp:TextBox>
            </div>
            <div id="Reference" class="ReferenceRow" runat="server">
                <label for="Reference">
                    <asp:Label ID="lblReference" runat="server">Reference</asp:Label></label>
                <asp:TextBox ID="txtReference" runat="server" ReadOnly="True" Text='<%# Bind("REFERENCE") %>' />
            </div>
            <div id="Sequence" class="SequenceRow" runat="server">
                <label for="Sequence">
                    <asp:Label ID="lblSequence" runat="server">Sequence</asp:Label></label>
                <asp:TextBox ID="txtSequence" runat="server" ReadOnly="True" Text='<%# Bind("SEQUENCE") %>' />
            </div>
            <div id="Default_Address" class="Default_AddressRow" runat="server">
                <label for="Locked">
                    <asp:Label ID="lblDefaultAddress" runat="server">Default Address</asp:Label></label>
                <asp:CheckBox ID="chkDefaultAddress" runat="server" Enabled="False" Checked='<%#  Bind("DEFAULT_ADDRESS")  %>' />
            </div>
            <div id="AddressLine1" class="ReferenceRow" runat="server">
                <label for="AddressLine1">
                    <asp:Label ID="lblAddressLine1" runat="server">Address line 1</asp:Label></label>
                <asp:TextBox ID="txtAddressLine1" runat="server" ReadOnly="True" Text='<%# Bind("ADDRESS_LINE_1") %>' />
            </div>
            <div id="AddressLine2" class="AddressLine2Row" runat="server">
                <label for="AddressLine2">
                    <asp:Label ID="lblAddressLine2" runat="server">Address line 2</asp:Label></label>
                <asp:TextBox ID="txtAddressLine2" runat="server" ReadOnly="True" Text='<%# Bind("ADDRESS_LINE_2") %>' />
            </div>
            <div id="AddressLine3" class="AddressLine3Row" runat="server">
                <label for="AddressLine3">
                    <asp:Label ID="lblAddressLine3" runat="server">Address line 3</asp:Label></label>
                <asp:TextBox ID="txtAddressLine3" runat="server" ReadOnly="True" Text='<%# Bind("ADDRESS_LINE_3") %>' />
            </div>
            <div id="AddressLine4" class="AddressLine4Row" runat="server">
                <label for="AddressLine4">
                    <asp:Label ID="lblAddressLine4" runat="server">Address line 4</asp:Label></label>
                <asp:TextBox ID="txtAddressLine4" runat="server" ReadOnly="True" Text='<%# Bind("ADDRESS_LINE_4") %>' />
            </div>
            <div id="AddressLine5" class="AddressLine5Row" runat="server">
                <label for="AddressLine5">
                    <asp:Label ID="lblAddressLine5" runat="server">Address line 5</asp:Label></label>
                <asp:TextBox ID="txtAddressLine5" runat="server" ReadOnly="True" Text='<%# Bind("ADDRESS_LINE_5") %>' />
            </div>
            <div id="PostCode" class="PostCodeRow" runat="server">
                <label for="PostCode">
                    <asp:Label ID="lblPostCode" runat="server">PostCode</asp:Label></label>
                <asp:TextBox ID="txtPostCode" runat="server" ReadOnly="True" Text='<%# Bind("POST_CODE") %>' />
            </div>
            <div id="Country" class="CountryRow" runat="server">
                <label for="Country">
                    <asp:Label ID="lblCountry" runat="server">Country</asp:Label></label>
                <asp:TextBox ID="txtCountry" runat="server" ReadOnly="True" Text='<%# Bind("COUNTRY") %>' />
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
