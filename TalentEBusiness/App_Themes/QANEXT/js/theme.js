// Fix Foundation 6 Dropdown Menu
// http://foundation.zurb.com/forum/posts/37499-trouble-with-events-on-f6-dropdown-menus
$(document).on("show.zf.dropdownmenu", function(ev, $el) {
  // console.log("dropdown menu show "+ev.target.id+" / "+$el.attr('class'));
  $(".submenu").removeClass("js-dropdown-active");
  $el.addClass("js-dropdown-active");
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