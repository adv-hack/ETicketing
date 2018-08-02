<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProfileMemberships.ascx.vb" 
Inherits="UserControls_ProfileMemberships" %>

<asp:PlaceHolder ID="plhErrorList" runat="server" Visible="false">
	<div class="alert-box alert">
		<asp:BulletedList ID="blErrorMessages" runat="server" />
	</div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhMembershipRepeater" runat="server">
	<div class="panel ebiz-profile-memberships-wrap">
		<asp:Repeater ID="rptMembership" runat="server">
			<HeaderTemplate>
				<table class="ebiz-responsive-table">
					<thead>
						<tr>
							<asp:PlaceHolder ID="plhMembership" runat="server">
							   <th scope="col" class="ebiz-membership"> <%=ColumnHeaderText_Membership%></th>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhNumber" runat="server"  OnPreRender="CanDisplayThisColumn">
							   <th scope="col" class="ebiz-number"> <%=ColumnHeaderText_Number%></th>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhPurchasedDate" runat="server" OnPreRender="CanDisplayThisColumn">
							   <th scope="col" class="ebiz-purchaseddate"> <%=ColumnHeaderText_PurchasedDate%></th>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhExpiryDate" runat="server" OnPreRender="CanDisplayThisColumn">
							   <th scope="col" class="ebiz-expirydate"> <%=ColumnHeaderText_ExpiryDate%></th>
							</asp:PlaceHolder>
							<asp:PlaceHolder ID="plhLoyalty" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-loyalty"> <%=ColumnHeaderText_Loyalty%></th>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhMembershipSinceDate" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-membership-sincedate"> <%=ColumnHeaderText_MembershipSinceDate%></th>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPriceCode" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-price-code"> <%=ColumnHeaderText_PriceCode%></th>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPriceCodeDescription" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-price-code-description"> <%=ColumnHeaderText_PriceCodeDescription%></th>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPriceBand" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-price-code"> <%=ColumnHeaderText_PriceBand%></th>
							</asp:PlaceHolder>
                            <asp:PlaceHolder ID="plhPriceBandDescription" runat="server" OnPreRender="CanDisplayThisColumn">
								<th scope="col" class="ebiz-price-code-description"> <%=ColumnHeaderText_PriceBandDescription%></th>
							</asp:PlaceHolder>
						</tr>
                    </thead>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<asp:PlaceHolder ID="plhMembership" runat="server">
						<td class="ebiz-membership" data-title="<%=ColumnHeaderText_Membership%>"><%# Container.DataItem("MembershipDesc").ToString().Trim()%></td>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="plhNumber" runat="server" OnPreRender="CanDisplayThisColumn">
						<td class="ebiz-number" data-title="<%=ColumnHeaderText_Number%>"><%# Container.DataItem("MembershipNumber").ToString().Trim()%></td>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="plhPurchasedDate" runat="server" OnPreRender="CanDisplayThisColumn">
						<td class="ebiz-purchaseddate" data-title="<%=ColumnHeaderText_PurchasedDate%>"><%# GetFormattedDate(Container.DataItem("PurchaseDate").ToString().Trim())%></td>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="plhExpiryDate" runat="server" OnPreRender="CanDisplayThisColumn">
						<td class="ebiz-expireddate" data-title="<%=ColumnHeaderText_ExpiryDate%>"><%# GetFormattedDate(Container.DataItem("ExpiryDate").ToString().Trim())%></td>
					</asp:PlaceHolder>
					<asp:PlaceHolder ID="plhLoyalty" runat="server" OnPreRender="CanDisplayThisColumn">
						<td class="ebiz-loyalty" data-title="<%=ColumnHeaderText_Loyalty%>"><%# CInt(Container.DataItem("Loyalty").ToString().Trim())%></td>
					</asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhMembershipSinceDate" runat="server" OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-membership-sincedate" data-title="<%=ColumnHeaderText_MembershipSinceDate%>"><%# GetFormattedDate(Container.DataItem("MembershipSinceDate").ToString().Trim())%></td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPriceCode" runat="server" OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-price-code" data-title="<%=ColumnHeaderText_PriceCode%>"><%# Container.DataItem("PriceCode").ToString().Trim()%></td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPriceCodeDescription" runat="server" OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-price-code-description" data-title="<%=ColumnHeaderText_PriceCodeDescription%>"><%# Container.DataItem("PriceCodeDescription").ToString().Trim()%></td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPriceBand" runat="server" OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-price-code" data-title="<%=ColumnHeaderText_PriceBand%>"><%# Container.DataItem("PriceCode").ToString().Trim()%></td>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhPriceBandDescription" runat="server" OnPreRender="CanDisplayThisColumn">
                        <td class="ebiz-price-band-description" data-title="<%=ColumnHeaderText_PriceBand%>"><%# Container.DataItem("PriceBandDescription").ToString().Trim()%></td>
                    </asp:PlaceHolder>
				</tr>
			</ItemTemplate>
			<FooterTemplate>
				</table>
			</FooterTemplate>
		</asp:Repeater>

	</div>
</asp:PlaceHolder>

