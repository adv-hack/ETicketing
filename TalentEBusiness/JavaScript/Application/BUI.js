// Fix Foundation 6 Dropdown Menu
// http://foundation.zurb.com/forum/posts/37499-trouble-with-events-on-f6-dropdown-menus

/*$(document).on("show.zf.dropdownmenu", function(ev, $el) {
  // console.log("dropdown menu show "+ev.target.id+" / "+$el.attr('class'));
  $(".submenu").removeClass("js-dropdown-active");
  $el.addClass("js-dropdown-active");
});*/


// Favourites

$(function() {
    var drilldownHeight = $('.ebiz-drilldown-favourites-wrap .is-drilldown').height();
    $('.ebiz-drilldown-favourites-append-to-wrap').css('min-height', drilldownHeight + 'px');
    $(".ebiz-drilldown-favourites-wrap .is-drilldown").appendTo(".ebiz-drilldown-favourites-append-to-wrap");
    $(".ebiz-drilldown-favourites-wrap").hide();
});


// Hide Mask After Page Load

$(window).bind('beforeunload', function(e) {
    setTimeout(function(){
        $(".ebiz-mask").show();
    }, 4000);
});

$(function() {
    $(".ebiz-mask").hide();
});


// Product Hot List

$(function() {
  $(".ebiz-hot-list-carousel .owl-carousel").owlCarousel({
    loop:true,
    margin:14,
    responsiveClass:true,
    responsive:{
        0:{
            items:1,
            nav:false
        },
        640:{
            items:2,
            nav:false
        },
        1024:{
            items:4,
            nav:false
        }
    }
  });
});


// Slide Toggle Product Hot List

$(function() {
    $(".ebiz-hot-list-carousel-wrap").hide();
    $(".ebiz-hot-list-carousel-toggle").click(function(){
        $(this).prev().slideToggle("fast").toggleClass("open closed");
        $(this).html() == '<i class="fa fa-times" aria-hidden="true"></i>' ? $(this).html('<i class="fa fa-angle-down" aria-hidden="true"></i>') : $(this).html('<i class="fa fa-times" aria-hidden="true"></i>');
    });
});


// Calculate Maximum Width

$(function(){
    /*maxWidth();*/
    $('.ebiz-ticketing-products .ebiz-product-logos').maxWidth({ breakpoint: 'medium' });
});

$(window).on('changed.zf.mediaquery', function() {
    /*maxWidth();*/
    $('.ebiz-ticketing-products .ebiz-product-logos').maxWidth({ breakpoint: 'medium' });
});


/*function maxWidth(){
    var maxWidth = Math.max.apply(null, $(".ebiz-ticketing-products .ebiz-product-logos").map(function(){
        return $(this).outerWidth(true);
    }).get());
    if (Foundation.MediaQuery.atLeast('medium')) {
        $(".ebiz-ticketing-products .ebiz-product-logos").width(maxWidth).css("min-height", "1px");
    }
    else {
        $(".ebiz-ticketing-products .ebiz-product-logos").css('width', 'auto');
    }
}*/
