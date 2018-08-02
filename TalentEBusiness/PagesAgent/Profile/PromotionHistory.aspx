<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromotionHistory.aspx.vb" Inherits="PagesLogin_Profile_PromotionHistory" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />
        <div class="panel ebiz-promotion-history-wrap">
            <asp:Repeater ID="rptPromotionHistory" runat="server">
                <HeaderTemplate>
                    <table>
                        <thead>
                            <tr>
                                <th scope="col" class="ebiz-product"><%=ProductTypeHeaderText%></th>
                                <th scope="col" class="ebiz-priority"><%=PriorityHeaderText%></th>
                                <th scope="col" class="ebiz-description"><%=DescriptionHeaderText%></th>
                                <th scope="col" class="ebiz-prerequisite"><%=PreRequisiteHeaderText%></th>
                                <th scope="col" class="ebiz-discounts"><%=MaxNumberOfDiscountPromotionsHeaderText%></th>
                                <th scope="col" class="ebiz-products"><%=MaxNumberOfDiscountProductsHeaderText%></th>
                                <th scope="col" class="ebiz-allocation"><%=CustomerAllocationHeaderText%></th>
                                <th scope="col" class="ebiz-details">&nbsp;</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td data-title="<%=ProductTypeHeaderText%>" class="ebiz-product"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "ProductType"))%></span></td>
                        <td data-title="<%=PriorityHeaderText%>" class="ebiz-priority"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "Priority"))%></span></td>
                        <td data-title="<%=DescriptionHeaderText%>" class="ebiz-description"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "PromotionDescription"))%></span></td>
                        <td data-title="<%=PreRequisiteHeaderText%>" class="ebiz-prerequisite"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "PreRequisiteProduct"))%></span></td>
                        <td data-title="<%=MaxNumberOfDiscountPromotionsHeaderText%>" class="ebiz-discounts"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "MaxDiscountPromotions"))%></span></td>
                        <td data-title="<%=MaxNumberOfDiscountProductsHeaderText%>" class="ebiz-products"><span><%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "MaxProductPromotions"))%></span></td>
                        <td data-title="<%=CustomerAllocationHeaderText%>" class="allocation"><span><asp:Literal ID="ltlCustomerAllocation" runat="server" Text='<%# trim(Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "CustomerAllocation")))%>' /></span></td>
                        <td data-title="&nbsp;" class="ebiz-details"><asp:HyperLink ID="hplPromotionDetails" runat="server" NavigateUrl='<%#"~/PagesAgent/Profile/PromotionHistoryDetail.aspx?promotionid=" & Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "PromotionID"))%>'>Details</asp:HyperLink></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
            <div class="ebiz-back-button-wrap">
                <asp:Button ID="btnBack" runat="server" CssClass="button ebiz-back" />
            </div>
        </div>

</asp:Content>