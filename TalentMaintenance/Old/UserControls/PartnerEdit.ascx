<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PartnerEdit.ascx.vb"
    Inherits="UserControls_PartnerEdit" %>
<asp:SqlDataSource ID="InsertPartnerSource" runat="server" ConnectionString="<%$ ConnectionStrings:TalentEBusinessDBConnectionString %>"
    SelectCommand="SELECT PARTNER_ID, PARTNER, PARTNER_DESC, DESTINATION_DATABASE, EMAIL, 
                     ACCOUNT_NO_5, ACCOUNT_NO_4, ACCOUNT_NO_3, ACCOUNT_NO_2, ACCOUNT_NO_1, 
                     TELEPHONE_NUMBER, FAX_NUMBER, PARTNER_URL, PARTNER_NUMBER, ORIGINATING_BUSINESS_UNIT, 
                     STORE_XML, LOGGING_ENABLED, CACHE_TIME_MINUTES, CACHEING_ENABLED  FROM tbl_partner  
                     WHERE PARTNER_ID = @PARTNER_ID"
    DeleteCommand="DELETE FROM tbl_partner WHERE PARTNER_ID = @PARTNER_ID" InsertCommand="INSERT INTO tbl_partner (PARTNER, PARTNER_DESC, DESTINATION_DATABASE, EMAIL, 
                     ACCOUNT_NO_5, ACCOUNT_NO_4, ACCOUNT_NO_3, ACCOUNT_NO_2, ACCOUNT_NO_1, 
                     TELEPHONE_NUMBER, FAX_NUMBER, PARTNER_URL, PARTNER_NUMBER, ORIGINATING_BUSINESS_UNIT, 
                     STORE_XML, LOGGING_ENABLED, CACHE_TIME_MINUTES, CACHEING_ENABLED 
                     ) VALUES (
                     @PARTNER, @PARTNER_DESC, @DESTINATION_DATABASE, @EMAIL, 
                     @ACCOUNT_NO_5, @ACCOUNT_NO_4, @ACCOUNT_NO_3, @ACCOUNT_NO_2, @ACCOUNT_NO_1, 
                     @TELEPHONE_NUMBER, @FAX_NUMBER, @PARTNER_URL, @PARTNER_NUMBER, @ORIGINATING_BUSINESS_UNIT, 
                     @STORE_XML, @LOGGING_ENABLED, @CACHE_TIME_MINUTES, @CACHEING_ENABLED)"
    UpdateCommand="UPDATE tbl_partner SET 
                     PARTNER = @PARTNER, 
                     PARTNER_DESC = @PARTNER_DESC, 
                     DESTINATION_DATABASE = @DESTINATION_DATABASE, 
                     EMAIL = @EMAIL, 
                     ACCOUNT_NO_5 = @ACCOUNT_NO_5, 
                     ACCOUNT_NO_4 = @ACCOUNT_NO_4, 
                     ACCOUNT_NO_3 = @ACCOUNT_NO_3, 
                     ACCOUNT_NO_2 = @ACCOUNT_NO_2, 
                     ACCOUNT_NO_1 = @ACCOUNT_NO_1, 
                     TELEPHONE_NUMBER =@TELEPHONE_NUMBER, 
                     FAX_NUMBER = @FAX_NUMBER, 
                     PARTNER_URL = @PARTNER_URL, 
                     PARTNER_NUMBER =@PARTNER_NUMBER, 
                     ORIGINATING_BUSINESS_UNIT = @ORIGINATING_BUSINESS_UNIT, 
                     STORE_XML = @STORE_XML, 
                     LOGGING_ENABLED = @LOGGING_ENABLED, 
                     CACHE_TIME_MINUTES = @CACHE_TIME_MINUTES, 
                     CACHEING_ENABLED = @CACHEING_ENABLED
                     WHERE PARTNER_ID = @PARTNER_ID">
    <SelectParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="PARTNER_ID" QueryStringField="ID"
            Type="Int64" />
    </SelectParameters>
    <DeleteParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="PARTNER_ID" QueryStringField="ID"
            Type="Int64" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:QueryStringParameter DefaultValue="0" Name="PARTNER_ID" QueryStringField="ID"
            Type="Int64" />
        <asp:Parameter Name="PARTNER" Type="String" />
        <asp:Parameter Name="PARTNER_DESC" Type="String" />
        <asp:Parameter Name="DESTINATION_DATABASE" Type="String" />
        <asp:Parameter Name="EMAIL" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_5" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_4" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_3" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_2" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_1" Type="String" />
        <asp:Parameter Name="TELEPHONE_NUMBER" Type="String" />
        <asp:Parameter Name="FAX_NUMBER" Type="String" />
        <asp:Parameter Name="PARTNER_URL" Type="String" />
        <asp:Parameter Name="PARTNER_NUMBER" Type="Int64" />
        <asp:Parameter Name="ORIGINATING_BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="STORE_XML" Type="Boolean" />
        <asp:Parameter Name="LOGGING_ENABLED" Type="Boolean" />
        <asp:Parameter Name="CACHE_TIME_MINUTES" Type="Int64" />
        <asp:Parameter Name="CACHEING_ENABLED" Type="Boolean" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="PARTNER" Type="String" />
        <asp:Parameter Name="PARTNER_DESC" Type="String" />
        <asp:Parameter Name="DESTINATION_DATABASE" Type="String" />
        <asp:Parameter Name="EMAIL" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_5" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_4" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_3" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_2" Type="String" />
        <asp:Parameter Name="ACCOUNT_NO_1" Type="String" />
        <asp:Parameter Name="TELEPHONE_NUMBER" Type="String" />
        <asp:Parameter Name="FAX_NUMBER" Type="String" />
        <asp:Parameter Name="PARTNER_URL" Type="String" />
        <asp:Parameter Name="PARTNER_NUMBER" Type="Int64" />
        <asp:Parameter Name="ORIGINATING_BUSINESS_UNIT" Type="String" />
        <asp:Parameter Name="STORE_XML" Type="Boolean" />
        <asp:Parameter Name="LOGGING_ENABLED" Type="Boolean" />
        <asp:Parameter Name="CACHE_TIME_MINUTES" Type="Int64" />
        <asp:Parameter Name="CACHEING_ENABLED" Type="Boolean" />
    </InsertParameters>
</asp:SqlDataSource>
<p class="error">
    <asp:Label ID="ErrorLabel" runat="server" CssClass="error"></asp:Label>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList"
        ValidationGroup="Maintenance" />
</p>
<div id="Maintenance">
    <asp:FormView ID="frmDataTable" runat="server" DataKeyNames="PARTNER_ID" DataSourceID="InsertPartnerSource">
        <EditItemTemplate>
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox ID="txtPartner" CssClass="input-l" runat="server" Text='<%# Bind("Partner") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_DESC") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtDescription" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Database" class="UnitRow" runat="server">
                <label for="Database">
                    <asp:Label ID="lblDatabase" runat="server">Database</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtDatabase" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("DESTINATION_DATABASE") %>' OnDataBinding="txtDatabase_TextChanged"
                    OnTextChanged="txtDatabase_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboDatabase" CssClass="select" runat="server" OnInit="cboDatabase_Load"
                    AutoPostBack="true" OnTextChanged="cboDatabase_Load">
                    <asp:ListItem>System21</asp:ListItem>
                    <asp:ListItem>Sql2005</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div id="emailRow" class="emailRow" runat="server">
                <label for="EMail">
                    <asp:Label ID="lblEmail" runat="server">EMail</asp:Label></label>
                <asp:TextBox ID="txtEmail" CssClass="input-l" runat="server" Text='<%# Bind("EMAIL") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtEmail" ID="emailRFV" runat="server"
                    OnInit="SetupRequiredFieldValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Maintenance"
                    Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtEmail" ID="emailRegEx" runat="server"
                    OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Maintenance"
                    Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Telephone" class="TelephoneRow" runat="server">
                <label for="Telephone">
                    <asp:Label ID="lblTelephone" runat="server">Telephone</asp:Label></label>
                <asp:TextBox ID="txtTelephone" CssClass="input-l" runat="server" Text='<%# Bind("TELEPHONE_NUMBER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtTelephone" ID="RequiredFieldValidator4"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtTelephone" ID="RegularExpressionValidator1"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Fax" class="FaxRow" runat="server">
                <label for="Fax">
                    <asp:Label ID="lblFax" runat="server">Fax</asp:Label></label>
                <asp:TextBox ID="txtFax" CssClass="input-l" runat="server" Text='<%# Bind("FAX_NUMBER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtFax" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtFax" ID="RegularExpressionValidator2"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Account1" class="Account1Row" runat="server">
                <label for="Account1">
                    <asp:Label ID="lblAccount1" runat="server">Account 1</asp:Label></label>
                <asp:TextBox ID="txtAccount1" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_1") %>'></asp:TextBox>
            </div>
            <div id="Account2" class="Account1Row" runat="server">
                <label for="Account2">
                    <asp:Label ID="lblAccount2" runat="server">Account 2</asp:Label></label>
                <asp:TextBox ID="txtAccount2" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_2") %>'></asp:TextBox>
            </div>
            <div id="Account3" class="Account1Row" runat="server">
                <label for="Account3">
                    <asp:Label ID="lblAccount3" runat="server">Account 3</asp:Label></label>
                <asp:TextBox ID="txtAccount3" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_3") %>'></asp:TextBox>
            </div>
            <div id="Account4" class="Account1Row" runat="server">
                <label for="Account4">
                    <asp:Label ID="lblAccount4" runat="server">Account 4</asp:Label></label>
                <asp:TextBox ID="txtAccount4" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_4") %>'></asp:TextBox>
            </div>
            <div id="Account5" class="Account1Row" runat="server">
                <label for="Account5">
                    <asp:Label ID="lblAccount5" runat="server">Account 5</asp:Label></label>
                <asp:TextBox ID="txtAccount5" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_5") %>'></asp:TextBox>
            </div>
            <div id="PartnerUrl" class="Account1Row" runat="server">
                <label for="PartnerUrl">
                    <asp:Label ID="lblPartnerUrl" runat="server">Partner Url</asp:Label></label>
                <asp:TextBox ID="txtPartnerUrl" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_URL") %>'></asp:TextBox>
            </div>
            <div id="PartnerNumber" class="Account1Row" runat="server">
                <label for="PartnerNumber">
                    <asp:Label ID="lblPartnerNumber" runat="server">Partner Number</asp:Label></label>
                <asp:TextBox ID="txtPartnerNumber" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_NUMBER") %>'></asp:TextBox>
            </div>
            <div id="StoreXML" class="Account1Row" runat="server">
                <label for="StoreXML">
                    <asp:Label ID="lblStoreXML" runat="server">Store XML</asp:Label></label>
                <asp:CheckBox ID="chkStoreXML" runat="server" Checked='<%# Bind("STORE_XML")  %>' />
            </div>
            <div id="Logging" class="Account1Row" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblLogging" runat="server">Logging Enabled</asp:Label></label>
                <asp:CheckBox ID="chkLogging" runat="server" Checked='<%# Bind("LOGGING_ENABLED")  %>' />
            </div>
            <div id="CacheTime" class="Account1Row" runat="server">
                <label for="CacheTime">
                    <asp:Label ID="lblCacheTime" runat="server">Cache Time (Minutes)</asp:Label></label>
                <asp:TextBox ID="txtCacheTime" CssClass="input-l" runat="server" Text='<%# Bind("CACHE_TIME_MINUTES")  %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtCacheTime" ID="RequiredFieldValidator6"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtCacheTime" ID="RegularExpressionValidator3"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
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
                <asp:TextBox ID="txtPartner" CssClass="input-l" runat="server" Text='<%# Bind("Partner") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtPartner" ID="RequiredFieldValidator1"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_DESC") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtDescription" ID="RequiredFieldValidator2"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
            </div>
            <div id="Database" class="UnitRow" runat="server">
                <label for="Database">
                    <asp:Label ID="lblDatabase" runat="server">Database</asp:Label></label>
                <asp:TextBox Visible="False" ID="txtDatabase" CssClass="input-l" runat="server" AutoPostBack="true"
                    Text='<%# Bind("DESTINATION_DATABASE") %>' OnDataBinding="txtDatabase_TextChanged"
                    OnTextChanged="txtDatabase_TextChanged"></asp:TextBox>
                <asp:DropDownList ID="cboDatabase" CssClass="select" runat="server" OnInit="cboDatabase_Load"
                    AutoPostBack="true" OnTextChanged="cboDatabase_Load">
                    <asp:ListItem>System21</asp:ListItem>
                    <asp:ListItem>Sql2005</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div id="emailRow" class="emailRow" runat="server">
                <label for="EMail">
                    <asp:Label ID="lblEmail" runat="server">EMail</asp:Label></label>
                <asp:TextBox ID="txtEmail" CssClass="input-l" runat="server" Text='<%# Bind("EMAIL") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtEmail" ID="emailRFV" runat="server"
                    OnInit="SetupRequiredFieldValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Maintenance"
                    Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtEmail" ID="emailRegEx" runat="server"
                    OnInit="SetupRegExValidator" SetFocusOnError="true" Visible="true" ValidationGroup="Maintenance"
                    Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Telephone" class="TelephoneRow" runat="server">
                <label for="Telephone">
                    <asp:Label ID="lblTelephone" runat="server">Telephone</asp:Label></label>
                <asp:TextBox ID="txtTelephone" CssClass="input-l" runat="server" Text='<%# Bind("TELEPHONE_NUMBER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtTelephone" ID="RequiredFieldValidator4"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtTelephone" ID="RegularExpressionValidator1"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Fax" class="FaxRow" runat="server">
                <label for="Fax">
                    <asp:Label ID="lblFax" runat="server">Fax</asp:Label></label>
                <asp:TextBox ID="txtFax" CssClass="input-l" runat="server" Text='<%# Bind("FAX_NUMBER") %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtFax" ID="RequiredFieldValidator5"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtFax" ID="RegularExpressionValidator2"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
            </div>
            <div id="Account1" class="Account1Row" runat="server">
                <label for="Account1">
                    <asp:Label ID="lblAccount1" runat="server">Account 1</asp:Label></label>
                <asp:TextBox ID="txtAccount1" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_1") %>'></asp:TextBox>
            </div>
            <div id="Account2" class="Account1Row" runat="server">
                <label for="Account2">
                    <asp:Label ID="lblAccount2" runat="server">Account 2</asp:Label></label>
                <asp:TextBox ID="txtAccount2" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_2") %>'></asp:TextBox>
            </div>
            <div id="Account3" class="Account1Row" runat="server">
                <label for="Account3">
                    <asp:Label ID="lblAccount3" runat="server">Account 3</asp:Label></label>
                <asp:TextBox ID="txtAccount3" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_3") %>'></asp:TextBox>
            </div>
            <div id="Account4" class="Account1Row" runat="server">
                <label for="Account4">
                    <asp:Label ID="lblAccount4" runat="server">Account 4</asp:Label></label>
                <asp:TextBox ID="txtAccount4" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_4") %>'></asp:TextBox>
            </div>
            <div id="Account5" class="Account1Row" runat="server">
                <label for="Account5">
                    <asp:Label ID="lblAccount5" runat="server">Account 5</asp:Label></label>
                <asp:TextBox ID="txtAccount5" CssClass="input-l" runat="server" Text='<%# Bind("ACCOUNT_NO_5") %>'></asp:TextBox>
            </div>
            <div id="PartnerUrl" class="Account1Row" runat="server">
                <label for="PartnerUrl">
                    <asp:Label ID="lblPartnerUrl" runat="server">Partner Url</asp:Label></label>
                <asp:TextBox ID="txtPartnerUrl" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_URL") %>'></asp:TextBox>
            </div>
            <div id="PartnerNumber" class="Account1Row" runat="server">
                <label for="PartnerNumber">
                    <asp:Label ID="lblPartnerNumber" runat="server">Partner Number</asp:Label></label>
                <asp:TextBox ID="txtPartnerNumber" CssClass="input-l" runat="server" Text='<%# Bind("PARTNER_NUMBER") %>'></asp:TextBox>
            </div>
            <div id="StoreXML" class="Account1Row" runat="server">
                <label for="StoreXML">
                    <asp:Label ID="lblStoreXML" runat="server">Store XML</asp:Label></label>
                <asp:CheckBox ID="chkStoreXML" CssClass="input-l" runat="server" Checked='false'
                    Text='<%# Bind("STORE_XML") %>' />
            </div>
            <div id="Logging" class="Account1Row" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblLogging" runat="server">Logging Enabled</asp:Label></label>
                <asp:CheckBox ID="chkLogging" CssClass="input-l" runat="server" Checked='false' Text='<%# Bind("LOGGING_ENABLED") %>' />
            </div>
            <div id="CacheTime" class="Account1Row" runat="server">
                <label for="CacheTime">
                    <asp:Label ID="lblCacheTime" runat="server">Cache Time (Minutes)</asp:Label></label>
                <asp:TextBox ID="txtCacheTime" CssClass="input-l" runat="server" Text='<%# Bind("CACHE_TIME_MINUTES")  %>'></asp:TextBox>
                <asp:RequiredFieldValidator ControlToValidate="txtCacheTime" ID="RequiredFieldValidator6"
                    OnInit="SetupRequiredFieldValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*" Width="10px"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ControlToValidate="txtCacheTime" ID="RegularExpressionValidator3"
                    OnInit="SetupRegExValidator" runat="server" SetFocusOnError="true" Visible="true"
                    ValidationGroup="Maintenance" Display="Dynamic" Enabled="true" Text="*"></asp:RegularExpressionValidator>
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
            <div id="Partner" class="PartnerRow" runat="server">
                <label for="Partner">
                    <asp:Label ID="lblPartner" runat="server">Partner</asp:Label></label>
                <asp:TextBox ID="txtPartner" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("Partner") %>'></asp:TextBox>
            </div>
            <div id="Description" class="DescriptionRow" runat="server">
                <label for="Description">
                    <asp:Label ID="lblDescription" runat="server">Description</asp:Label></label>
                <asp:TextBox ID="txtDescription" CssClass="input-l" runat="server" ReadOnly='True'
                    Text='<%# Bind("PARTNER_DESC") %>'></asp:TextBox>
            </div>
            <div id="Database" class="DescriptionRow" runat="server">
                <label for="Database">
                    <asp:Label ID="lblDatabase" runat="server">Database</asp:Label></label>
                <asp:TextBox ID="txtDatabase" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("DESTINATION_DATABASE") %>'></asp:TextBox>
            </div>
            <div id="emailRow" class="emailRow" runat="server">
                <label for="emailRow">
                    <asp:Label ID="lblEmail" runat="server">EMail</asp:Label></label>
                <asp:TextBox ID="txtEmail" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("EMAIL") %>'></asp:TextBox>
            </div>
            <div id="Telephone" class="TelephoneRow" runat="server">
                <label for="Telephone">
                    <asp:Label ID="lblTelephone" runat="server">Telephone</asp:Label></label>
                <asp:TextBox ID="txtTelephone" CssClass="input-l" runat="server" ReadOnly='True'
                    Text='<%# Bind("TELEPHONE_NUMBER") %>'></asp:TextBox>
            </div>
            <div id="Fax" class="FaxRow" runat="server">
                <label for="Fax">
                    <asp:Label ID="lblFax" runat="server">Fax</asp:Label></label>
                <asp:TextBox ID="txtFax" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("FAX_NUMBER") %>'></asp:TextBox>
            </div>
            <div id="Account1" class="Account1Row" runat="server">
                <label for="Account1">
                    <asp:Label ID="lblAccount1" runat="server">Account 1</asp:Label></label>
                <asp:TextBox ID="txtAccount1" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("ACCOUNT_NO_1") %>'></asp:TextBox>
            </div>
            <div id="Account2" class="Account1Row" runat="server">
                <label for="Account2">
                    <asp:Label ID="lblAccount2" runat="server">Account 2</asp:Label></label>
                <asp:TextBox ID="txtAccount2" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("ACCOUNT_NO_2") %>'></asp:TextBox>
            </div>
            <div id="Account3" class="Account1Row" runat="server">
                <label for="Account3">
                    <asp:Label ID="lblAccount3" runat="server">Account 3</asp:Label></label>
                <asp:TextBox ID="txtAccount3" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("ACCOUNT_NO_3") %>'></asp:TextBox>
            </div>
            <div id="Account4" class="Account1Row" runat="server">
                <label for="Account4">
                    <asp:Label ID="lblAccount4" runat="server">Account 4</asp:Label></label>
                <asp:TextBox ID="txtAccount4" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("ACCOUNT_NO_4") %>'></asp:TextBox>
            </div>
            <div id="Account5" class="Account1Row" runat="server">
                <label for="Account5">
                    <asp:Label ID="lblAccount5" runat="server">Account 5</asp:Label></label>
                <asp:TextBox ID="txtAccount5" CssClass="input-l" runat="server" ReadOnly='True' Text='<%# Bind("ACCOUNT_NO_5") %>'></asp:TextBox>
            </div>
            <div id="PartnerUrl" class="Account1Row" runat="server">
                <label for="PartnerUrl">
                    <asp:Label ID="lblPartnerUrl" runat="server">Partner Url</asp:Label></label>
                <asp:TextBox ID="txtPartnerUrl" CssClass="input-l" runat="server" ReadOnly='True'
                    Text='<%# Bind("PARTNER_URL") %>'></asp:TextBox>
            </div>
            <div id="PartnerNumber" class="Account1Row" runat="server">
                <label for="PartnerNumber">
                    <asp:Label ID="lblPartnerNumber" runat="server">Partner Number</asp:Label></label>
                <asp:TextBox ID="txtPartnerNumber" CssClass="input-l" runat="server" ReadOnly="True"
                    Text='<%# Bind("PARTNER_NUMBER") %>'></asp:TextBox>
            </div>
            <div id="StoreXML" class="Account1Row" runat="server">
                <label for="StoreXML">
                    <asp:Label ID="lblStoreXML" runat="server">Store XML</asp:Label></label>
                <asp:CheckBox ID="chkStoreXML" runat="server" Checked='<%# Bind("STORE_XML")  %>' />
            </div>
            <div id="Logging" class="Account1Row" runat="server">
                <label for="Logging">
                    <asp:Label ID="lblLogging" runat="server">Logging Enabled</asp:Label></label>
                <asp:CheckBox ID="chkLogging" runat="server" Checked='<%# Bind("LOGGING_ENABLED")  %>' />
            </div>
            <div id="CacheTime" class="Account1Row" runat="server">
                <label for="CacheTime">
                    <asp:Label ID="lblCacheTime" runat="server">Cache Time (Minutes)</asp:Label></label>
                <asp:TextBox ID="txtCacheTime" CssClass="input-l" runat="server" ReadOnly='True'
                    Text='<%# Bind("CACHE_TIME_MINUTES") %>'></asp:TextBox>
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
