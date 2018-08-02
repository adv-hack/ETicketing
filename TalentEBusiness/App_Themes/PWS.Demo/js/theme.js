// Fix Foundation 6 Dropdown Menu
// http://foundation.zurb.com/forum/posts/37499-trouble-with-events-on-f6-dropdown-menus
$(document).on("show.zf.dropdownmenu", function(ev, $el) {
  // console.log("dropdown menu show "+ev.target.id+" / "+$el.attr('class'));
  $(".submenu").removeClass("js-dropdown-active");
  $el.addClass("js-dropdown-active");
});

// Animation fix on hamburger menu

$(".hamburger").click(function(){
    $(this).toggleClass('is-active');
});

$(".js-off-canvas-overlay").click(function(){
    $(".hamburger").removeClass("is-active");
});

// Header transition on page scroll  

var prev = 0;
var $window = $(window);
var div = $('.scrollhide-nav');

$window.on('scroll', function(){
  var scrollTop = $window.scrollTop();
  div.toggleClass('hidden', scrollTop > prev);
  prev = scrollTop;
});




