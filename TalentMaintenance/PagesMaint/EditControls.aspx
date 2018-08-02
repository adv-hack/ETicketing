<%--IMPORTANT!! VALIDATE REQUEST IS TURNED OFF!! This is to allow HTML and JScript to be put in the form fields--%>
<%@ Page 
    Language="VB" 
    MasterPageFile="~/MasterPages/PageMaintenanceMasterPage.master" 
    AutoEventWireup="false" 
    CodeFile="EditControls.aspx.vb" 
    Inherits="PagesMaint_EditControls" 
    validateRequest="False" 
    title="Edit Page Controls" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" Runat="Server">
    
    <div id="edit_controls_wrapper">
    
        <p class="title"><asp:Literal ID="ltlEditControlsTitle" runat="server" /></p>
        <asp:BulletedList ID="lstErrorMessages" runat="server" CssClass="error" />
        
        <fieldset>
            <legend><asp:Literal ID="ltlEditControlsLegend" runat="server" /></legend>
            <ul>
                <li>
                    <label for="<%= txtBusinessUnit.ClientID %>"><asp:Literal ID="ltlBusinessUnit" runat="server" /></label>
                    <asp:TextBox ID="txtBusinessUnit" runat="server" ReadOnly="true" CssClass="input-l" MaxLength="50" />
                </li>
                <li>
                    <label for="<%= txtPartner.ClientID %>"><asp:Literal ID="ltlPartner" runat="server" /></label>
                    <asp:TextBox ID="txtPartner" runat="server" ReadOnly="true" CssClass="input-l" MaxLength="50" />
                </li>
                <li runat="server" id="liPageName" visible="false">
                    <label for="<%= txtPageName.ClientID %>"><asp:Literal ID="ltlPageName" runat="server" /></label>
                    <asp:TextBox ID="txtPageName" runat="server" ReadOnly="true" CssClass="input-l" MaxLength="50" />
                </li>
                <li runat="server" id="liDescription" visible="false">
                    <label for="<%= txtDescription.ClientID %>"><asp:Literal ID="ltlDescription" runat="server" /></label>
                    <asp:TextBox ID="txtDescription" runat="server" ReadOnly="true" CssClass="input-l" MaxLength="500" />
                </li>
                <li runat="server" id="liControl">
                    <label for="<%= ddlControl.ClientID %>"><asp:Literal ID="ltlControl" runat="server" /></label>
                    <asp:DropDownList ID="ddlControl" runat="server" CssClass="select" AutoPostBack="True" OnSelectedIndexChanged="ddlControl_Select" />
                </li>
                <li runat="server" id="liLanguage" visible="false">
                    <label for="<%= ddlLanguage.ClientID %>"><asp:Literal ID="ltlLanguage" runat="server" /></label>
                    <asp:DropDownList ID="ddlLanguage" runat="server" CssClass="select" AutoPostBack="True" OnSelectedIndexChanged="ddlLanguage_Select" />
                </li>
            </ul>
        </fieldset>
        
        <asp:PlaceHolder ID="plhControlTextLang" runat="server" Visible="False">
            <fieldset id="edit_controls_text_lang_fieldset">
                <legend><asp:Literal ID="ltlEditControlTextLangLegend" runat="server" /></legend>
                <asp:PlaceHolder ID="plhControlTextLangChangesSaved" runat="server" Visible="False">
                    <p class="error"><asp:Literal ID="ltlControlTextLangChangesSaved" runat="server" /></p>
                </asp:PlaceHolder>
                <asp:Repeater 
                    ID="rptControlTextLang" 
                    OnItemDataBound="rptControlTextLang_OnItemDataBound"
                    runat="server">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:Literal ID="ltlControlTextLangTextCode" runat="server" />
                            <asp:TextBox ID="txtControlTextLangControlContent" runat="server" />
                            <asp:HiddenField ID="hdfControlTextLangPageCode" runat="server" />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        <asp:Button ID="btnSaveControlTextLang" runat="server" OnClick="btnSaveControlTextLangChanges" CssClass="save_button" />
                    </FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="plhNoControlTextLangRows" runat="server" Visible="False">
                    <p class="error"><asp:Literal ID="ltlNoControlTextLangRows" runat="server" /></p>
                </asp:PlaceHolder>
            </fieldset>
        </asp:PlaceHolder>
        
        <asp:PlaceHolder ID="plhControlAttributes" runat="server" Visible="False">
            <fieldset id="edit_controls_attribute_fieldset">
                <legend><asp:Literal ID="ltlEditControlAttributesLegend" runat="server" /></legend>
                <asp:PlaceHolder ID="plhControlAttributesChangesSaved" runat="server" Visible="False">
                    <p class="error"><asp:Literal ID="ltlControlAttributesChangesSaved" runat="server" /></p>
                </asp:PlaceHolder>
                <asp:Repeater 
                    ID="rptControlAttributes" 
                    OnItemDataBound="rptControlAttributes_OnItemDataBound"
                    runat="server">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <asp:Literal ID="ltlControlAttributeName" runat="server" />
                            <asp:TextBox ID="txtControlAttributeValue" runat="server" CssClass="textbox" Visible="False" />
                            <asp:CheckBox ID="chkControlAttrubuteValue" runat="server" Visible="False" CssClass="checkbox" />
                            <asp:HiddenField ID="hdfControlAttributePageCode" runat="server" />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                        <asp:Button ID="btnSaveAttributes" runat="server" OnClick="btnSaveControlAttributesChanges" CssClass="save_button" />
                    </FooterTemplate>
                </asp:Repeater>
                <asp:PlaceHolder ID="plhNoControlTextAttributes" runat="server" Visible="False">
                    <p class="error"><asp:Literal ID="ltlNoControlTextAttributes" runat="server" /></p>
                </asp:PlaceHolder>
            </fieldset>
        </asp:PlaceHolder>
        
    </div>    
    
    <div class="pageDetailButtons">
        <asp:Button ID="btnReturnToPageOverview" runat="server" CssClass="buttonPage" CausesValidation="False" OnClick="btnReturnToPageOverview_Click" />
        <asp:Button ID="btnReturnToPageDetails" runat="server" CssClass="buttonPage" CausesValidation="False" Visible="False" OnClick="btnReturnToPageDetails_Click" />
    </div>
    
</asp:Content>