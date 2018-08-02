// Apply max height function
(function($) {
    $.fn.applyMaxHeight = function() {
        // Get an array of all element widths
        var elementHeights = this.map(function() {
            return $(this).height();
        }).get();
        // Math.max takes a variable number of arguments
        // `apply` is equivalent to passing each width as an argument
        var maxHeight = Math.max.apply(null, elementHeights);
        // Set each width to the max width
        this.height(maxHeight);
        return this;
    };
}(jQuery));



// Apply max width function
(function($) {
    $.fn.applyMaxWidth = function() {
        // Get an array of all element widths
        var elementWidths = this.map(function() {
            return $(this).width();
        }).get();
        // Math.max takes a variable number of arguments
        // `apply` is equivalent to passing each width as an argument
        var maxWidth = Math.max.apply(null, elementWidths);
        // Set each width to the max width
        this.width(maxWidth);
        return this;
    };
}(jQuery));



// Foundation

$(document).foundation();



// FastClick

$(function() {
    FastClick.attach(document.body);
});

// browseXX.aspx

$(function() {
    $('.ebiz-graphical-product-item .panel').matchHeight();
    $('.ebiz-graphical-product-item h2').matchHeight();
    $('.ebiz-graphical-product-item .ebiz-long-description').matchHeight();
});



// Previous Modal Functionality

$(".ebiz-inline-modal").on("opened.fndtn.reveal", function () {
    $(this).appendTo($("form:first"));
});

$(document).on('closed.fndtn.reveal', '[data-reveal]', function (e){
    if (e.namespace !== 'fndtn.reveal') {
        return;
    }
    // handle namespaced event               
    var modal = $(this);
    if (!modal.hasClass("ebiz-inline-modal")) {
        modal.empty();
    }
});



// Modal Function

// Insert the reveal modal container inside the form tag
// http://foundation.zurb.com/forum/posts/1187-aspnetfoundation-reveal-modal

$(function() {
    $(".reveal-overlay").appendTo("form");
});

$(".ebiz-open-modal").bind("click",function(e){
    e.preventDefault();
    var url = ($(this).attr("href"));
    var modal = "#" + ($(this).attr("data-open"));
    $.ajax(url).done(function(data) {
        $(modal).html(data).foundation();
    });
}); 



// PurchaseDetails.aspx

// Dynamically set a TD colspan to the same number of header columns
var ebizOrderDetailsColumns = $(".ebiz-order-details thead tr").children().length;
$(".ebiz-order-details .ebiz-actions").attr("colspan", ebizOrderDetailsColumns);



// Fix Foundation 6 Dropdown Menu
// http://foundation.zurb.com/forum/posts/37499-trouble-with-events-on-f6-dropdown-menus

$(document).on("show.zf.dropdownmenu", function(ev, $el) {
  // console.log("dropdown menu show "+ev.target.id+" / "+$el.attr('class'));
  $(".submenu").removeClass("js-dropdown-active");
  $el.addClass("js-dropdown-active");
});



// Fullscreen

/*$('.toggle-fullscreen').click(function () {
    if (screenfull.enabled) {
        // We can use `this` since we want the clicked element
        screenfull.toggle();
    }
});
*/


// Favourites

/*$(function() {
    var drilldownHeight = $('.ebiz-drilldown-favourites-wrap .is-drilldown').height();
    $('.ebiz-drilldown-favourites-append-to-wrap').css('min-height', drilldownHeight + 'px');
    $(".ebiz-drilldown-favourites-wrap .is-drilldown").appendTo(".ebiz-drilldown-favourites-append-to-wrap");
    $(".ebiz-drilldown-favourites-wrap").hide();
});
*/


// Hide Mask After Page Load

/*$(window).bind('beforeunload', function(e) {
    console.log("beforeunload");
    setTimeout(function(){
        $(".ebiz-mask").show();
    }, 2000);
});

$(function() {
    $(".ebiz-mask").hide();
});
*/


// Hide Empty TD Tags - Small and Medium Only

$(window).on("load resize",function(e){
    var emptyTdTag = Foundation.MediaQuery.current;
    if(emptyTdTag == "small" || emptyTdTag == "medium") {
        $("td:empty").hide();
    } else {
        $("td:empty").show();
    }
});



// Despatch Note Generation Search Scroll To Top

$(function() {
    $('.ebiz-despatch-note-generation-search .button').click(function(event){
        $('html, body, .off-canvas-wrapper').animate({
            scrollTop: ($('.off-canvas-content').offset().top)
        },500);
    });
});



// perfect-scrollbar

$(function() {
    $('.no-touchevents .off-canvas-wrapper').perfectScrollbar(); 
    $('.no-touchevents .off-canvas').perfectScrollbar(); 
});


// Product Hot List

$(document).ready(function(){
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


// Select2 - Select Box Replacement

$(".select2").select2()