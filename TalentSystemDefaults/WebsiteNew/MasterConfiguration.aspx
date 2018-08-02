<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MasterConfiguration.aspx.vb" Inherits="MasterConfiguration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SetupMasterConfig</title>
     <link href="Style/layout.css" rel="stylesheet" />
</head>
<body>
    <form id="masterConfigForm" runat="server">
        <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrorMessages" runat="server"/>
            </div>
        </asp:PlaceHolder>
        <table>
            <tr>
                <td>
                    <asp:Label ID="lblTableName" runat="server" Text="TableName"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlTableName" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <asp:PlaceHolder ID="plhBusinessUnitSelector" runat="server" Visible="false">
            <tr>
                <td>
                    <input type="radio" id="rbUK" name="rbBusinessUnit" value="unitedkingdom" title="UNITEDKINGDOM" checked="true" runat="server"/>UNITEDKINGDOM
                </td>
                <td>
                    <input type="radio" id="rbBoxoffice" name="rbBusinessUnit" value="boxoffice" title="BOXOFFICE" runat="server" />BOXOFFICE
                </td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhCompanyCode" runat="server" Visible="false">
            <tr>
                <td>
                    <asp:Label ID="lblCompanyCode" runat="server" Text="Company Code"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCompanyCode" runat="server" Text="IPS"></asp:TextBox>
                </td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhColumns" runat="server" Visible="false">
            <tr>
                <td>
                    <asp:Label ID="lblColumns" runat="server" Text="Columns"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtColumns" runat="server" Text="*" Lines="2" TextMode="multiline"></asp:TextBox>
                </td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDefaultKey1" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblDefaultKey1" runat="server" Text="Default Key1"></asp:Label></td>
                <td><asp:TextBox ID="txtDefaultKey1" runat="server" ></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDefaultKey2" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblDefaultKey2" runat="server" Text="Default Key2"></asp:Label></td>
                <td><asp:TextBox ID="txtDefaultKey2" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDefaultKey3" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblDefaultKey3" runat="server" Text="Default Key3"></asp:Label></td>
                <td><asp:TextBox ID="txtDefaultKey3" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhDefaultKey4" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblDefaultKey4" runat="server" Text="Default Key4"></asp:Label></td>
                <td><asp:TextBox ID="txtDefaultKey4" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
			<asp:PlaceHolder ID="plhDefaultName" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblDefaultName" runat="server" Text="Default Name"></asp:Label></td>
                <td><asp:TextBox ID="txtDefaultName" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhVariableKey1" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblVariableKey1" runat="server" Text="Variable Key1"></asp:Label></td>
                <td><asp:TextBox ID="txtVariableKey1" runat="server" ></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhVariableKey2" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblVariableKey2" runat="server" Text="Variable Key2"></asp:Label></td>
                <td><asp:TextBox ID="txtVariableKey2" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhVariableKey3" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblVariableKey3" runat="server" Text="Variable Key3"></asp:Label></td>
                <td><asp:TextBox ID="txtVariableKey3" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhVariableKey4" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblVariableKey4" runat="server" Text="Variable Key4"></asp:Label></td>
                <td><asp:TextBox ID="txtVariableKey4" runat="server"></asp:TextBox></td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhBaseDefinition" runat="server" Visible="false">
            <tr>
                <td><asp:Label ID="lblBaseDefinition" runat="server" Text="Base Definition"></asp:Label></td>
                <td>
                   <asp:RadioButton GroupName="gpBaseDefinition" ID="rbAll" runat="server" Text="*ALL" Checked="true"></asp:RadioButton>
                   <asp:RadioButton GroupName="gpBaseDefinition" ID="rbVariableKey1" runat="server" Text="VariableKey1"></asp:RadioButton>
                </td>
            </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhClassDetails" runat="server" Visible="false">
                <asp:PlaceHolder ID="plhClassName" runat="server">
                <tr>
                    <td><asp:Label ID="lblClassName" runat="server" Text="Class Name"></asp:Label></td>
                    <td><asp:TextBox ID="txtClassName" runat="server"></asp:TextBox></td>
                </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhShowAsModule" runat="server">
                <tr>
                    <td><asp:Label ID="lblShowAsModule" runat="server" Text="Show As Module"></asp:Label></td>
                    <td>
                       <asp:RadioButton GroupName="gpShowAsModule" ID="rbShowAsModuleYes" runat="server" Text="Yes" OnCheckedChanged="rbShowAsModuleYes_CheckedChanged" AutoPostBack="true"></asp:RadioButton>
                       <asp:RadioButton GroupName="gpShowAsModule" ID="rbShowAsModuleNo" runat="server" Text="No" OnCheckedChanged="rbShowAsModuleNo_CheckedChanged" AutoPostBack="true"></asp:RadioButton>
                    </td>
                </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhModuleTitle" runat="server" Visible="false">
                <tr>
                    <td><asp:Label ID="lblModuleTitle" runat="server" Text="Module Title"></asp:Label></td>
                    <td><asp:TextBox ID="txtModuleTitle" runat="server"></asp:TextBox></td>
                </tr>
                </asp:PlaceHolder>
            </asp:PlaceHolder>
         </table>
        <asp:PlaceHolder ID="plhLoad" runat="server" Visible="false">
            <div style="padding-bottom:10px">
                <asp:Button ID="btnLoad" runat="server" Text="Load Content From DB" />
                <asp:Button ID="btnClear" runat="server" Text="Clear List" />
            </div>
        </asp:PlaceHolder>
        <div>
            <asp:GridView ID="gridConfigData" runat="server" AutoGenerateColumns="False" style="margin-right: 0px" OnRowDataBound="gridConfigData_RowDataBound">
                <Columns>
                    <asp:TemplateField  HeaderText="">
                        <ItemTemplate>
                            <asp:CheckBox ID="cbSelect" runat="server" OnCheckedChanged="cbSelect_CheckedChanged" AutoPostBack="true"></asp:CheckBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField  HeaderText="Table Name">
                        <ItemTemplate>
                            <asp:TextBox ID="txtTableName" runat="server" Text='<%#Bind("TableName")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Master Configuration">
                        <ItemTemplate>
                            <asp:TextBox ID="txtMasterConfigurationId" runat="server" Text='<%#Bind("MasterConfigId")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DefaultKey1">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultKey1" runat="server" Text='<%#Bind("DefaultKey1")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DefaultKey2">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultKey2" runat="server" Text='<%#Bind("DefaultKey2")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DefaultKey3">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultKey3" runat="server" Text='<%#Bind("DefaultKey3")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="DefaultKey4">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultKey4" runat="server" Text='<%#Bind("DefaultKey4")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="VariableKey1">
                        <ItemTemplate>
                            <asp:TextBox ID="txtVariableKey1" runat="server" Text='<%#Bind("VariableKey1")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="VariableKey2">
                        <ItemTemplate>
                            <asp:TextBox ID="txtVariableKey2" runat="server" Text='<%#Bind("VariableKey2")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="VariableKey3">
                        <ItemTemplate>
                            <asp:TextBox ID="txtVariableKey3" runat="server" Text='<%#Bind("VariableKey3")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="VariableKey4">
                        <ItemTemplate>
                            <asp:TextBox ID="txtVariableKey4" runat="server" Text='<%#Bind("VariableKey4")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Display Name">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDisplayName" runat="server" Text='<%#Bind("DisplayName")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Default Name">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultName" runat="server" Text='<%#Bind("DefaultName")%>'></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Default Value">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDefaultValue" runat="server" Text='<%#Bind("DefaultValue")%>'>></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Allowed Values">
                        <ItemTemplate>
                            <asp:TextBox ID="txtAllowedValues" runat="server" Text='<%#Bind("AllowedValues")%>'>></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Allowed Place Holders">
                        <ItemTemplate>
                            <asp:TextBox ID="txtAllowedPlaceHolders" runat="server" Text='<%#Bind("AllowedPlaceHolders") %>'>></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>
                
                    <asp:TemplateField HeaderText="Description">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDescription" runat="server" Text='<%#Bind("Description") %>'>></asp:TextBox>
                        </ItemTemplate>
                    </asp:TemplateField>

                     <asp:TemplateField HeaderText="DataType">
                         <ItemTemplate>
                             <asp:DropDownList ID="ddlDataType" runat="server">
                             </asp:DropDownList>
                         </ItemTemplate>
                     </asp:TemplateField>

                    <asp:TemplateField HeaderText="Display Tab">
                        <ItemTemplate>
                            <asp:DropDownList ID="ddlDisplayTab" runat="server">
                             </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>               

                     <asp:TemplateField HeaderText="Mandatory">
                         <ItemTemplate>
                             <center><asp:CheckBox ID="cbMandatory" runat="server" /></center>
                         </ItemTemplate>
                     </asp:TemplateField>

                     <asp:TemplateField HeaderText="MinLength">
                         <ItemTemplate>
                             <center><asp:CheckBox ID="cbMinLength" runat="server"/></center>
                         </ItemTemplate>
                     </asp:TemplateField>

                     <asp:TemplateField HeaderText="MaxLength">
                         <ItemTemplate>
                             <center><asp:CheckBox ID="cbMaxLength" runat="server"/></center>
                         </ItemTemplate>
                     </asp:TemplateField>

                    <asp:TemplateField HeaderText="MinLength Value">
                         <ItemTemplate>
                              <asp:TextBox ID="txtMinLength" runat="server" Width="50px" />
                         </ItemTemplate>
                     </asp:TemplateField>

                    <asp:TemplateField HeaderText="MaxLength Value">
                         <ItemTemplate>
                             <asp:TextBox ID="txtMaxLength" runat="server" Width="50px" />
                         </ItemTemplate>
                     </asp:TemplateField> 
                </Columns>
            </asp:GridView>
         </div>
        <asp:PlaceHolder ID="plhButtons" runat="server" Visible="false">
            <div style="padding-top:10px">
                <table>
                    <tr>
                        <td><asp:Button ID="btnSave" runat="server" Text="Generate Code" OnClick="btnSave_Click" /></td>
                        <td><asp:Button ID="btnDelete" runat="server" Text="Delete Selected" OnClick="btnDelete_Click" /></td>
                    </tr>
                </table>
            </div>
        </asp:PlaceHolder>
    </form>
</body>
</html>