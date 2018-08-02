// Fix Foundation 6 Dropdown Menu
// http://foundation.zurb.com/forum/posts/37499-trouble-with-events-on-f6-dropdown-menus
$(document).on("show.zf.dropdownmenu", function(ev, $el) {
  // console.log("dropdown menu show "+ev.target.id+" / "+$el.attr('class'));
  $(".submenu").removeClass("js-dropdown-active");
  $el.addClass("js-dropdown-active");
});

$(function() {
    $(".inc-next-matches .columns").matchHeight();
});

$(".hamburger").click(function(){
    $(this).toggleClass('is-active');
});

$(".js-off-canvas-overlay").click(function(){
    $("#ebiz-burger-menu").removeClass("is-active");
});
