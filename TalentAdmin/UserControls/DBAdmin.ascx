<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DBAdmin.ascx.vb" Inherits="UserControls_DBAdmin" %>
<div class="dbadmin-wrap">
    <div class="title">
        <asp:Label ID="titleLabel" runat="server">Database Admin</asp:Label>
    </div>
    <div class="content">
        <div class="error-wrap">
            <asp:BulletedList ID="feedbackList" runat="server">
            </asp:BulletedList>
        </div>
        <div class="line-wrap">
            <asp:Label id="xmlFolderLabel" runat="server" CssClass="line-label">XML Folder Path</asp:Label>
            <asp:TextBox ID="xmlPathBox" runat="server" CssClass="line-input"></asp:TextBox>&nbsp;
            <asp:Button ID="btnXmlFileSelect" runat="server" Text="Select A Single File" />
        </div>
        <asp:PlaceHolder ID="plhXmlFileSelection" runat="server" Visible="False">
            <div class="line-wrap">
                <asp:Label ID="xmlFileLabel" runat="server" CssClass="line-label">XML File</asp:Label>
                <asp:DropDownList ID="ddlXmlFile" runat="server" CssClass="line-input" />
            </div>
        </asp:PlaceHolder>
        <div class="line-wrap">
            <asp:Label id="clientNameLabel" runat="server" CssClass="line-label">Select Client</asp:Label>
            <asp:DropDownList ID="clientNameDDL" runat="server" CssClass="line-input" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="line-wrap">
            <asp:Label id="Label1" runat="server" CssClass="line-label">Type</asp:Label>
            <asp:DropDownList ID="liveOrTestDDL" runat="server" CssClass="line-input" AutoPostBack="true"></asp:DropDownList>
        </div>
        <div class="line-wrap">
            <asp:Label id="buLabel" runat="server" CssClass="line-label">Business Unit</asp:Label>
            <asp:TextBox ID="buBox" runat="server" CssClass="line-input">UNITEDKINGDOM</asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="partnerLabel" runat="server" CssClass="line-label">Partner</asp:Label>
            <asp:TextBox ID="partnerBox" runat="server" CssClass="line-input">PUBLIC</asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="langLabel" runat="server" CssClass="line-label">Language</asp:Label>
            <asp:TextBox ID="LangBox" runat="server" CssClass="line-input">ENG</asp:TextBox>
        </div>
         <div class="line-wrap">
            <asp:Label id="verLabel" runat="server" CssClass="line-label">Version</asp:Label>
            <asp:TextBox ID="verBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="subVerLabel" runat="server" CssClass="line-label">Sub Version</asp:Label>
            <asp:TextBox ID="subVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="ptfVerLabel" runat="server" CssClass="line-label">PTF Version</asp:Label>
            <asp:TextBox ID="ptfVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="clientVerLabel" runat="server" CssClass="line-label">Client Specific</asp:Label>
            <asp:TextBox ID="clientVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Repeater ID="ClientServersRepeater1" runat="server">
                <HeaderTemplate>
                    <table>
                        <tr>
                            <th>Server</th>
                            <th>Update?</th>
                            <th>Connection String</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Label ID="csLabel" runat="server"></asp:Label></td>
                        <td>
                            <asp:CheckBox ID="csCheck" runat="server" /></td>
                        <td>
                            <asp:Label ID="csConnStr" runat="server"></asp:Label></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        <div class="line-wrap">
            <asp:Label id="newVerLabel" runat="server" CssClass="line-label">Version</asp:Label>
            <asp:TextBox ID="newVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="newSubVerLabel" runat="server" CssClass="line-label">Sub Version</asp:Label>
            <asp:TextBox ID="newSubVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="newPtfVerLabel" runat="server" CssClass="line-label">PTF Version</asp:Label>
            <asp:TextBox ID="newPtfVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Label id="newClientVerLabel" runat="server" CssClass="line-label">Client Specific</asp:Label>
            <asp:TextBox ID="newClientVerBox" runat="server" CssClass="line-input"></asp:TextBox>
        </div>
        <div class="line-wrap">
            <asp:Button ID="goButton" runat="server" Text="Submit" />
            <asp:Button ID="upgradeNotesBtn" runat="server" Text="View Notes" Visible="False" />
        </div>
        <div class="upgrade-notes">
            <asp:Repeater ID="upgradeNotesRepeater" runat="server" Visible="False">
                <HeaderTemplate>
                    <table>
                </HeaderTemplate>
                <ItemTemplate>
                        <tr>
                            <th>Version</th><td><asp:Label ID="versionLabel" runat="server"></asp:Label></td>
                            <th>Sub Version</th><td><asp:Label ID="subVersionLabel" runat="server"></asp:Label></td>
                            <th>PTF</th><td><asp:Label ID="PTFLabel" runat="server"></asp:Label></td>
                            <th>Client</th><td><asp:Label ID="clientLabel" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Table</th>
                            <td colspan="7"><asp:Label ID="tableNameLabel" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Description</th>
                            <td colspan="7"><asp:Label ID="descLabel" runat="server"></asp:Label></td>
                        </tr>
                        <tr>
                            <th>Action</th>
                            <td colspan="7"><asp:Label ID="actionLabel" runat="server"></asp:Label></td>
                        </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        
        </div>
    </div>
</div>