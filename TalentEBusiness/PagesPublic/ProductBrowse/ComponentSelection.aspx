<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ComponentSelection.aspx.vb" Inherits="PagesPublic_ProductBrowse_ComponentSelection" ViewStateMode="Disabled" %>
<%@ Register Src="~/UserControls/Package/ComponentSeats.ascx" TagName="ComponentSeatsDetails" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Package/TravelAndAccommodationGroup.ascx" TagName="TravelAndAccommodationGroup" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/Package/PackageSummary.ascx" TagName="PackageSummary" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/CustomerProgressBar.ascx" TagName="CustomerProgressBar" TagPrefix="Talent" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<Talent:CustomerProgressBar ID="ProgressBar1" runat="server"></Talent:CustomerProgressBar>
<div class="row">
    <div class="large-12 columns">
        <h1><asp:literal ID="ltlProductHeader" runat="server" /></h1>
        <h2><asp:literal ID="ltlPackageHeader" runat="server" /></h2>
        
        <asp:BulletedList ID="ErrorList" runat="server" CssClass="alert-box alert" Visible="false"></asp:BulletedList>
        
        <asp:Literal ID="ltlSpecificContent1" runat="server" />
       
        <asp:Repeater ID="rptComponents" runat="server">
            <ItemTemplate>
                <div class="panel ebiz-component-wrap">
                    <a runat="server" ID="lnkStage" />
   
                        <div class="row">
                            <div class="medium-6 columns ebiz-component"><h2><asp:literal ID="ltlComponentHeader" runat="server" /></h2></div>
                            <div class="medium-6 columns ebiz-completed"><h2><asp:literal ID="ltlCompleted" runat="server" /> <i class="fa fa-check-square-o" id="icnCompleted" runat="server"></i> <i class="fa fa-square-o" id="icnNotCompleted" runat="server"></i></h2></div>
                        </div>
                                       
                        <Talent:ComponentSeatsDetails ID="uscComponentSeatsDetails" runat="server" display="false" />
                        <Talent:TravelAndAccommodationGroup ID="uscTravelAndAccommodationGroup" runat="server" display="false" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
        
        <Talent:PackageSummary ID="uscPackageSummary" runat="server" display="true" />
    </div>
</div>
<script type="text/javascript" src="../../JavaScript/Module/TicketingPackage/component-selection.js?a=<%=GetVersion()%>"></script>
</asp:Content>
