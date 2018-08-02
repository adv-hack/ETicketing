<%@ Page Language="VB" AutoEventWireup="false" CodeFile="product.aspx.vb" Inherits="PagesPublic_product" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/usercontrols/productrelationsgraphical.ascx" TagName="ProductRelationsGraphical" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductControls.ascx" TagName="ProductControls" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/PriceBreakTable.ascx" TagName="PriceBreakTable" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/ProductMagicZoomAdditionalImages.ascx" TagName="ProductMagicZoomAdditionalImages" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>
<%@ Reference Control="~/UserControls/ProductOptions.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder3" runat="server">
    <script type="text/javascript" src="/JavaScript/vendor/magiczoomplus/magiczoomplus.js"></script>
</asp:Content>


<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" /> 
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <asp:PlaceHolder ID="plhProductDescription2" runat="server">
        <div class="ebiz-product-description-2">
            <asp:Literal ID="ltlProductDescription2" runat="server" />
        </div>
    </asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude2" runat="server" Usage="2" Sequence="2" /> 
    <asp:PlaceHolder ID="plhErrorMessage" runat="server">
            <div class="alert-box alert">
                <asp:BulletedList ID="blErrorMessages" runat="server" />
            </div>
    </asp:PlaceHolder>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" DisplayMode="BulletList" ValidationGroup="product" CssClass="alert-box alert" />
    <div class="row ebiz-MagicZoom-product-controls-wrap">
        <div class="columns ebiz-product-image-inner-container">
            <asp:PlaceHolder ID="plhProductCode" runat="server">
                <p class="ebiz-product-code"><asp:Literal runat="server" ID="ltlProductCodeText" /><strong><asp:Literal runat="server" ID="ltlProductCode" /></strong></p>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plhProductImage" runat="server">
                <asp:Image ID="imgProduct" runat="server" CssClass="ebiz-product-image" />
            </asp:PlaceHolder>
            <Talent:ProductMagicZoomAdditionalImages ID="ProductMagicZoomAdditionalImages" runat="server" />
            <Talent:HTMLInclude ID="HTMLInclude3" runat="server" Usage="2" Sequence="3" /> 
        </div>
        <div class="columns ebiz-product-blurb-inner-container">
            <asp:PlaceHolder ID="plhProductDescription3" runat="server">
                <div class="ebiz-product-description-3">
                    <asp:Literal ID="ltlProductDescription3" runat="server" />
                </div>
            </asp:PlaceHolder>
            <!-- HTML 7 GOES HERE -->
            <div ID="plhTabTitle7" runat="server" Visible="false">
                <label><%= ProductHTML7Title%></label>
                <div ID="plhContent7" runat="server" Visible="false">                   
                    <asp:Literal ID="lblHTML7" runat="server" />                        
                </div> 
            </div>

            <Talent:HTMLInclude ID="HTMLInclude4" runat="server" Usage="2" Sequence="4" /> 
            <Talent:PriceBreakTable runat="server" ID="ucPriceBreakTable" />
            <Talent:ProductControls ID="ProductControls1" runat="server" />
            <Talent:HTMLInclude ID="HTMLInclude5" runat="server" Usage="2" Sequence="5" /> 
            <asp:PlaceHolder ID="plhProductDescription4" runat="server">
                <div class="ebiz-product-description-4">
                    <asp:Literal ID="ltlProductDescription4" runat="server" />
                </div>
            </asp:PlaceHolder>
            <!-- HTML 8 GOES HERE -->
            <div ID="plhTabTitle8" runat="server" Visible="false">
                <label><%= ProductHTML8Title%></label>
                <div ID="plhContent8" runat="server" Visible="false">                   
                    <asp:Literal ID="lblHTML8" runat="server" />                        
                </div> 
            </div>
        </div>
    </div>
    <asp:PlaceHolder ID="plhProductDescription5" runat="server">
        <div class="row ebiz-product-description-5">
            <div class="column">
                <asp:Literal ID="ltlProductDescription5" runat="server" />
            </div>
        </div>
    </asp:PlaceHolder>
    <Talent:HTMLInclude ID="HTMLInclude6" runat="server" Usage="2" Sequence="6" /> 
    <div class="row ebiz-product-tabs-wrap">
        <div class="large-12 columns">
            <%--To display tabs: a tab item needs to have both a tab title with content (i.e TabTitle1 and lblHTML1.Text). 
            The first tab item must be populated before other items.--%>
            <ul class="tabs" data-tabs id="product-blurb-tabs">
                <asp:PlaceHolder ID="plhTabTitle1" runat="server" Visible="false">
                    <li ID="plhTabTitle1li" runat="server" class="tabs-title"><a href="#panel-1"><%= TabTitle1%></a></li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="plhTabTitle2" runat="server" Visible="false">
                    <li ID="plhTabTitle2li" class="tabs-title" runat="server"><a href="#panel-2"><%= TabTitle2%></a></li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="plhTabTitle3" runat="server" Visible="false">
                    <li ID="plhTabTitle3li" runat="server" class="tabs-title"><a href="#panel-3"><%= TabTitle3%></a></li>
                 </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhTabTitle4" runat="server" Visible="false">
                    <li ID="plhTabTitle4li" runat="server" class="tabs-title"><a href="#panel-4"><%= TabTitle4%></a></li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="plhTabTitle5" runat="server" Visible="false">
                    <li ID="plhTabTitle5li" runat="server" class="tabs-title"><a href="#panel-5"><%= TabTitle5%></a></li>
                 </asp:PlaceHolder>
                 <asp:PlaceHolder ID="plhTabTitle6" runat="server" Visible="false">
                    <li ID="plhTabTitle6li" runat="server" class="tabs-title"><a href="#panel-6"><%= TabTitle6%></a></li>
                 </asp:PlaceHolder>                                
            </ul>

            <div class="tabs-content" data-tabs-content="product-blurb-tabs">
                <asp:PlaceHolder ID="plhContent1" runat="server" Visible="false">
                    <div class="tabs-panel is-active" id="panel-1">
                        <asp:Literal ID="lblHTML1" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhContent2" runat="server" Visible="false">
                    <div class="tabs-panel" id="panel-2">
                        <asp:Literal ID="lblHTML2" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhContent3" runat="server" Visible="false">
                    <div class="tabs-panel" id="panel-3">
                        <asp:Literal ID="lblHTML3" runat="server" />
                    </div> 
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhContent4" runat="server" Visible="false">
                    <div class="tabs-panel" id="panel-4">
                        <asp:Literal ID="lblHTML4" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhContent5" runat="server" Visible="false">
                    <div class="tabs-panel" id="panel-5">
                        <asp:Literal ID="lblHTML5" runat="server" />
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhContent6" runat="server" Visible="false">
                    <div class="tabs-panel" id="panel-6">
                        <asp:Literal ID="lblHTML6" runat="server" />
                    </div> 
                </asp:PlaceHolder>                
            </div>
        </div>
    </div>
    <!-- HTML 9 GOES HERE -->
    <div ID="plhTabTitle9" runat="server" Visible="false">
        <label><%= ProductHTML9Title%></label>
        <div ID="plhContent9" runat="server" Visible="false">                   
            <asp:Literal ID="lblHTML9" runat="server" />                        
        </div> 
    </div>

    <Talent:HTMLInclude ID="HTMLInclude7" runat="server" Usage="2" Sequence="7" /> 

</asp:content>
