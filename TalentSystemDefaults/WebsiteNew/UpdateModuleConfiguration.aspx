<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UpdateModuleConfiguration.aspx.vb" Inherits="UpdateModuleConfiguration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    Select Module
                </td>
                <td>
                    <asp:DropDownList ID="ddlModules" runat="server" AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
        </table>
    </div>
        <br />
       <asp:PlaceHolder ID="plhModuleConfigurations" runat="server" Visible="false">
        <div>
            <asp:GridView ID="gridConfigData" runat="server" AutoGenerateColumns="False" style="margin-right: 0px">
                <Columns>
                    <asp:TemplateField HeaderText="Old GUID">
                        <ItemTemplate>
                            <asp:TextBox ID="txtOldGUID" runat="server" Text='<%#Bind("ConfigurationId")%>' ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Display Name">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDisplayName" runat="server" Text='<%#Bind("DisplayName")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Formatted Display Name">
                        <ItemTemplate>
                            <asp:TextBox ID="txtFormattedDisplayName" runat="server" Text='<%#Bind("FormattedDisplayName")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Default Value">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultValue" runat="server" Text='<%#Bind("DefaultValue")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Allowed Values">
                        <ItemTemplate>
                            <asp:TextBox ID="txtAllowedValue" runat="server" Text='<%#Bind("AllowedValues")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Allowed Place Holders">
                        <ItemTemplate>
                            <asp:TextBox ID="txtAllowedPlaceHolders" runat="server" Text='<%#Bind("AllowedPlaceHolders")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server" Text='<%#Bind("Description")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="New GUID">
                        <ItemTemplate>
                            <asp:TextBox ID="txtNewGUID" runat="server" Text='<%#Bind("NewConfigurationId")%>' ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
        <br />
        <div>
            <asp:Button ID="btnSubmit" Text="Submit" runat="server" OnClick="btnSubmit_Click" /> &nbsp;
            <asp:Button ID="btnGenerate" Text="Generate Constants" runat="server" OnClick="btnGenerate_Click" />
         </div>
        <div id="dvContent" runat="server">
    
        </div>
    </asp:PlaceHolder>
    </form>
</body>
</html>
