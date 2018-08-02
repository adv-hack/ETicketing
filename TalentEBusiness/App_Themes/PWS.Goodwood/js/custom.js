jQuery(document).ready(function() {
	var offset = 220;
	var duration = 500;
	jQuery(window).scroll(function() {
   		if (jQuery(this).scrollTop() > offset) {
   			jQuery('.back-to-top').fadeIn(duration);
    	} else {
       jQuery('.back-to-top').fadeOut(duration);
    }
	});

jQuery('.back-to-top').click(function(event) {
    event.preventDefault();
    jQuery('html, body').animate({scrollTop: 0}, duration);
    return false;
	 	})
});

$(function() {
   $(".ebiz-ticketing-products .ebiz-extended-text").prepend("<i class='fa fa-info-circle' style=' color: #0066CC; font-size: 1rem; '></i>");
});

$(window).load(function() {
 $(window).resize(function() {   
  var imgHeight = $('.ebiz-UsrDef-logo-title-wrap img').height();
    $(".ebiz-UsrDef-logo-title-wrap h2").height($(".ebiz-UsrDef-logo-title-wrap img").height());
    $(".ebiz-UsrDef-logo-title-wrap h2").css('line-height',imgHeight + 'px');
    $(".ebiz-UsrDef-logo-title-wrap h2").show();
 }).resize(); 
});

//Fix z-index youtube video embedding
$(document).ready(function (){
  $('iframe').each(function() {
    var url = $(this).attr("src");
    if ($(this).attr("src").indexOf("?") > 0) {
      $(this).attr({
        "src" : url + "&wmode=transparent",
        "wmode" : "Opaque"
      });
    }
    else {
      $(this).attr({
        "src" : url + "?wmode=transparent",
        "wmode" : "Opaque"
      });
    }
  });
});

$(window).load(function(){
  if (navigator.userAgent.match(/(iPod|iPhone|iPad)/)) {
    $('#sb-body-inner').css({
        'overflow': 'scroll', 
        '-webkit-overflow-scrolling': 'touch' 
    }); 
  }
});