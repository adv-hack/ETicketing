<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProfileAttributes.ascx.vb" 
    Inherits="UserControls_ProfileAttributes" %>

<div id="divProfileAttributes">
    <asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
        <div class="alert-box alert">
            <asp:BulletedList ID="blErrorMessages" runat="server"/>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhCustomerAttributeAdd" runat="server" >
        <div class="row ebiz-profile-attributes-wrap">
            <div class="large-3 columns ebiz-profile-attributes-filters-wrap"  id="divCustomerAttributeAdd" runat="server">
                <div class="panel">
                    <asp:UpdatePanel ID="upCustomerAttributeAdd" runat="server">
                        <ContentTemplate>
                            <div class="row ebiz-status">
                                <div class="large-12 medium-3 columns">
                                    <asp:Label ID="lblCategoryDropdown" runat="server" AssociatedControlID="ddlCategory" />
                                </div>
                                <div class="large-12 medium-9 columns">
                                    <asp:DropDownList ID="ddlCategory" runat="server" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" AutoPostBack="true"/>
                                </div>
                            </div>
                            <div class="row ebiz-status">
                                <div class="large-12 medium-3 columns">
                                    <asp:Label ID="lblAttributeDropdown" runat="server" AssociatedControlID="ddlAttribute" />
                                </div>
                                <div class="large-12 medium-9 columns">
                                    <asp:DropDownList ID="ddlAttribute" runat="server"/>
                                </div>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlCategory" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div class="ebiz-add-wrap">
                        <asp:Button ID="btnAdd" runat="server" CssClass="button" OnClick="btnAdd_Click"/>
                    </div>
                </div>
            </div>
       
    
            <div class="large-9 columns ebiz-profile-attributes-results-wrap" runat="server" id="divCustomerAttributesRepeater">
            <asp:Repeater ID="rptCustomerAttributes" runat="server" OnItemDataBound="rptCustomerAttributes_ItemDataBound">
                <HeaderTemplate>
                    <div class="panel">
                        <table>
                            <thead>
                                <tr>
                                    <asp:PlaceHolder ID="plhHeaderCategory" runat="server">
                                        <th scope="col" class="ebiz-category"><%=ColumnHeaderText_Category%></th>
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="plhHeaderAttribute" runat="server">
                                        <th scope="col" class="ebiz-attribute"><%=ColumnHeaderText_Attribute%></th>
                                    </asp:PlaceHolder>
                                </tr>
                            </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <asp:PlaceHolder ID="plhItemCategory" runat="server">
                            <td class="ebiz-category" data-title="<%=ColumnHeaderText_Category%>">
		                       <%# Container.DataItem("CategoryDescription").ToString().Trim()%>
                               <asp:Label ID="lblErrorMessage" cssClass="alert-box alert" Visible="false" runat="server"/>
                            </td>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhItemAttribute" runat="server">
                            <td class="ebiz-attribute" data-title="<%=ColumnHeaderText_Attribute%>">
                                <div class="row">
                                    <asp:Repeater ID="rptAttribute" runat="server" OnItemDataBound='rptAttribute_ItemDataBound' OnItemCommand="rptAttribute_ItemCommand">                   
                                                                          
                                         <ItemTemplate>
                                            <div class="columns ebiz-attribute-item"><span><%#Container.DataItem("AttributeDescription").ToString().Trim()%></span>
                                                <asp:HiddenField ID="hidCategoryCode" runat="server" Value='<%#Container.DataItem("CategoryCode").ToString().Trim()%>' />
                                                <asp:HiddenField ID="hidAttributeCode" runat="server" Value='<%#Container.DataItem("AttributeCode").ToString().Trim()%>' />
                                                <asp:HiddenField ID="hidAttributeID" runat="server" Value='<%#Container.DataItem("AttributeKey").ToString().Trim()%>' />
                                                <asp:PlaceHolder ID="plhDelete" runat="server">
                                                    <asp:LinkButton ID="lnkButtonDelete" Visible="true" runat="server" CommandName="ProcessDelete">                                        
                                                        <i id="iconForDelete" runat="server" class="fa fa-times"></i>
                                                    </asp:LinkButton>
                                                </asp:PlaceHolder>
                                            </div>
                                       </ItemTemplate>
                                      
                                    </asp:Repeater>
                                    </div>
                            </td>
                        </asp:PlaceHolder>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </table>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
             </div>
    </asp:PlaceHolder>
</div>


