<%@ Page Language="VB" AutoEventWireup="false" CodeFile="standAndAreaSelection.aspx.vb" Inherits="PagesPublic_ProductBrowse_standAndAreaSelection" title="Untitled Page" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/StandAndAreaSelection.ascx" TagName="StandAndAreaSelection" TagPrefix="Talent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <asp:PlaceHolder ID="plhProductTitle" runat="server"><h1><span class="ebiz-product-title"><asp:Literal ID="ltlProductTitle" runat="server" /></span></h1></asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />
    <asp:PlaceHolder ID="plhBackButton" runat="server"><div class="back-button"><asp:HyperLink ID="hplBack" runat="server" CssClass="button" /></div></asp:PlaceHolder>
    <asp:PlaceHolder ID="plhErrorMessages" runat="server">
        <div class="alert-box alert">
            <asp:BulletedList ID="ErrorList" runat="server" />    
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="plhAdditionalInformation" runat="server">
        <div class="ebiz-more-info">
            <asp:HyperLink ID="hlkAdditionalInformation" runat="server" data-open="additional-information-modal" CssClass="ebiz-open-modal"> <asp:Literal ID="ltlMoreInfoText" runat="server" />  <i class="fa fa-info-circle"></i></asp:HyperLink>
             <div id="additional-information-modal" class="reveal ebiz-reveal-ajax" data-reveal></div>    
        </div>
    </asp:PlaceHolder>
    
    <div class="ebiz-selection-outer-wrapper">
        <asp:PlaceHolder ID="plhSeatingPlanImage" runat="server">
        <div class="seat-plan-image">
            <asp:Image ID="imgSeatingPlan" runat="server" />
        </div>
        </asp:PlaceHolder>
        <div class="ebiz-selection-inner-wrapper">
            <Talent:StandAndAreaSelection ID="uscStandAndAreaSelection" runat="server" />
            <script language="Javascript" type="text/javascript">
                function removeAllOptions(ddl) {
                    var i;
                    for (i = ddl.options.length - 1; i >= 0; i--) {
                        ddl.remove(i);
                    }
                }
                function addOption(selectbox, value, text) {
                    var optn = document.createElement("OPTION");
                    optn.text = text;
                    optn.value = value;
                    selectbox.options.add(optn);
                }
            </script>
        </div>
    </div>

    <asp:HiddenField ID="hdfCATMode" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfProductType" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfProductCode" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfStadiumCode" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfCampaignCode" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfCallId" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="hdfPriceAndAreaSelection" runat="server" ClientIDMode="Static" />

    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" />
</asp:Content>