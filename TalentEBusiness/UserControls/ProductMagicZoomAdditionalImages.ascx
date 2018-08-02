<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ProductMagicZoomAdditionalImages.ascx.vb"
    Inherits="UserControls_ProductMagicZoomAdditionalImages" %>

<asp:PlaceHolder ID="pnlProductMagicZoomAdditionalImages" runat="server">
    
        <asp:PlaceHolder ID="MZDefaultImage" runat="server">
            
                <img src="<%=MZDefault%>" alt="" />
            
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="MZImages" runat="server">
            <asp:PlaceHolder ID="MZMain" runat="server">
                
                    <a href="<%=MZ1M%>" class="MagicZoom" id="Zoomer"><img src="<%=MZ1M%>" alt="" /></a>
                    <asp:PlaceHolder ID="ImageZoomLabel" runat="server">
                        <p class="ebiz-image-zoom"><asp:Literal ID="ImageZoom" runat="server"></asp:Literal></p>
                    </asp:PlaceHolder>
                
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="AlternativeImages" runat="server">
                
                    <div class="owl-carousel owl-theme js-product-carousel">
                        <asp:PlaceHolder ID="MZ1" runat="server">
                            <div class="item"><a href="<%=MZ1L%>" data-zoom-id="Zoomer" data-image="<%=MZ1M%>"><img alt="" src="<%=MZ1M%>" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ2" runat="server">
                            <div class="item"><a href="<%=MZ2L%>" data-zoom-id="Zoomer" data-image="<%=MZ2M%>"><img src="<%=MZ2M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ3" runat="server">
                            <div class="item"><a href="<%=MZ3L%>" data-zoom-id="Zoomer" data-image="<%=MZ3M%>"><img src="<%=MZ3M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ4" runat="server">
                            <div class="item"><a href="<%=MZ4L%>" data-zoom-id="Zoomer" data-image="<%=MZ4M%>"><img src="<%=MZ4M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ5" runat="server">
                            <div class="item"><a href="<%=MZ5L%>" data-zoom-id="Zoomer" data-image="<%=MZ5M%>"><img src="<%=MZ5M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ6" runat="server">
                            <div class="item"><a href="<%=MZ6L%>" data-zoom-id="Zoomer" data-image="<%=MZ6M%>"><img src="<%=MZ6M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ7" runat="server">
                            <div class="item"><a href="<%=MZ7L%>" data-zoom-id="Zoomer" data-image="<%=MZ7M%>"><img src="<%=MZ7M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ8" runat="server">
                            <div class="item"><a href="<%=MZ8L%>" data-zoom-id="Zoomer" data-image="<%=MZ8M%>"><img src="<%=MZ8M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ9" runat="server">
                            <div class="item"><a href="<%=MZ9L%>" data-zoom-id="Zoomer" data-image="<%=MZ9M%>"><img src="<%=MZ9M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ10" runat="server">
                            <div class="item"><a href="<%=MZ10L%>" data-zoom-id="Zoomer" data-image="<%=MZ10M%>"><img src="<%=MZ10M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ11" runat="server">
                            <div class="item"><a href="<%=MZ11L%>" data-zoom-id="Zoomer" data-image="<%=MZ11M%>"><img src="<%=MZ11M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="MZ12" runat="server">
                            <div class="item"><a href="<%=MZ12L%>" data-zoom-id="Zoomer" data-image="<%=MZ12M%>"><img src="<%=MZ12M%>" alt="" /></a></div>
                        </asp:PlaceHolder>
                    </div>
                    
                    <asp:PlaceHolder ID="AlternativeViewLabel" runat="server">
                        <p class="ebiz-alternative-view"><asp:Literal ID="AlternativeView" runat="server"></asp:Literal></p>
                    </asp:PlaceHolder>
                
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    
</asp:PlaceHolder>

<script>
    $(document).ready(function() {
        var w = document.body.clientWidth;
            if (w < 1024) {
                MagicZoom.options = {
                'disable-zoom': true
            };
        };
    });
</script>
