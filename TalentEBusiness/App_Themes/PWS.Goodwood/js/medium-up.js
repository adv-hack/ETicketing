$(function() {
  setTimeout(function() {
  $('.owl-carousel').owlCarousel({
    autoplay:true,
    autoplayTimeout:5000,
    autoplayHoverPause:true,
    loop:true,
    nav:true,
    responsive:true,
    items:1,
    margin:0,
    navText: [
      "<i class='fa fa-chevron-circle-left fa-4x'></i>",
      "<i class='fa fa-chevron-circle-right fa-4x'></i>"
    ]
  }) }
  , 200);
});