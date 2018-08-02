

(function ($){
    $(function(){
    		   
			 
			 
	init();
	
	function init(){
		
		if(!$('html').hasClass('lt-ie9')){
		
		/*!
		 * enquire.js v2.1.0 - Awesome Media Queries in JavaScript
		 * Copyright (c) 2014 Nick Williams - http://wicky.nillia.ms/enquire.js
		 * License: MIT (http://www.opensource.org/licenses/mit-license.php)
		 */
		
		!function(a,b,c){var d=window.matchMedia;"undefined"!=typeof module&&module.exports?module.exports=c(d):"function"==typeof define&&define.amd?define(function(){return b[a]=c(d)}):b[a]=c(d)}("enquire",this,function(a){"use strict";function b(a,b){var c,d=0,e=a.length;for(d;e>d&&(c=b(a[d],d),c!==!1);d++);}function c(a){return"[object Array]"===Object.prototype.toString.apply(a)}function d(a){return"function"==typeof a}function e(a){this.options=a,!a.deferSetup&&this.setup()}function f(b,c){this.query=b,this.isUnconditional=c,this.handlers=[],this.mql=a(b);var d=this;this.listener=function(a){d.mql=a,d.assess()},this.mql.addListener(this.listener)}function g(){if(!a)throw new Error("matchMedia not present, legacy browsers require a polyfill");this.queries={},this.browserIsIncapable=!a("only all").matches}return e.prototype={setup:function(){this.options.setup&&this.options.setup(),this.initialised=!0},on:function(){!this.initialised&&this.setup(),this.options.match&&this.options.match()},off:function(){this.options.unmatch&&this.options.unmatch()},destroy:function(){this.options.destroy?this.options.destroy():this.off()},equals:function(a){return this.options===a||this.options.match===a}},f.prototype={addHandler:function(a){var b=new e(a);this.handlers.push(b),this.matches()&&b.on()},removeHandler:function(a){var c=this.handlers;b(c,function(b,d){return b.equals(a)?(b.destroy(),!c.splice(d,1)):void 0})},matches:function(){return this.mql.matches||this.isUnconditional},clear:function(){b(this.handlers,function(a){a.destroy()}),this.mql.removeListener(this.listener),this.handlers.length=0},assess:function(){var a=this.matches()?"on":"off";b(this.handlers,function(b){b[a]()})}},g.prototype={register:function(a,e,g){var h=this.queries,i=g&&this.browserIsIncapable;return h[a]||(h[a]=new f(a,i)),d(e)&&(e={match:e}),c(e)||(e=[e]),b(e,function(b){h[a].addHandler(b)}),this},unregister:function(a,b){var c=this.queries[a];return c&&(b?c.removeHandler(b):(c.clear(),delete this.queries[a])),this}},new g});
		
		}
	
	}
			
			
    		$('ul#menu-mainmenu').superfish();
    		$('.g1 ul').superfish();		 
    		$('#primary #menu-main-menu').superfish();
    		   
			 if(!$('html').hasClass('lt-ie9')){
				 
  
    		enquire.register("screen and (max-width:64em)", {
    			setup : function() {
    			 			 
    			},
    			match : function() {
    				
    				$('ul#menu-mainmenu').superfish('destroy');
    				$('.g1 ul').superfish('destroy');
    				
    				var menu_button = '<div class="menu_button"><img src="/App_Themes/Responsive-Goodwood/img/mobile_menu1.png"/></div>';
    				$('.menu-mainmenu-container').prepend(menu_button);
    			 	$('#menu-mainmenu').children().not(':first').wrapAll( "<div class='mobilemenu_wrapper' />");
    			 	
    				 $('.menu_button').on('click', function(){
    					
    					
    					if($('.mobilemenu_wrapper').is(':visible'))
    	 				{
    						$('.mobilemenu_wrapper').slideUp();
    					}
    					else
    					{
    						$('.mobilemenu_wrapper').slideDown();
    					}
    				});
    				
					$('nav#site-navigation ul#menu-mainmenu li').show();

				
    			},
    			unmatch : function() {
    		
    				$('ul#menu-mainmenu').superfish('init');
    				$('.g1 ul').superfish('init');
    				
    			 	$('.menu_button').remove();
    				var cnt = $(".mobilemenu_wrapper").contents();
    				$(".mobilemenu_wrapper").replaceWith(cnt);
    			  
    			}
    		});
			
			/*enquire.register("screen and (min-width:64.063em)", {
    			setup : function() {
    			 			 
    			},
    			match : function() {
    				
				//homepage parallax
				
				jQuery('#firstImage .fullWidthPromoBlock-bg').parallax('0%', '0.1');
				jQuery('#secondImage .fullWidthPromoBlock-bg').parallax('0%', '0.1');
				jQuery('#thirdImage .fullWidthPromoBlock-bg').parallax('0%', '0.05');
					
    				
    			}
    		});*/
    
    	 //functions
    	 
			 } //if ie8
    	 
    		$('.mobilemenu_wrapper li.open').on('click', function(e)
			{
				console.log('yes');
				$(this).find('ul.sub-menu').slideUp('fast');
			});
			
    		$('.mobilemenu_wrapper li a').on('click', function(e)
    		{
    			e.preventDefault();
    			
    			var url = $(this).attr('href');
				$(this).parent().addClass('open');
    			var submenu = $(this).parent().find('ul.sub-menu').first();
    			
				var click_x = ((e.pageX / $(window).width()) * 100);
				
				if(submenu.length == 0 || submenu.is(':visible'))
    			{
					if(click_x > 75)
					{
						$('li').removeClass('open');
						$('ul.sub-menu').slideUp('fast');
					}
					else 
					{
						window.location.href = url;	
					}
    			}
    			else
    			{
					$('li').not($(this).parent()).removeClass('open');
    				$('ul.sub-menu').not(submenu.parents()).slideUp('fast');
    				submenu.slideDown('fast');
    			}
    		
    		});
			
						
			
			// grab the initial top offset of the navigation 
			sticky_navigation_offset_top = $('#site-navigation').offset().top;
			wpdepth = $('#wpadminbar').height();
			sticky_navigation_depth 	 = $('#wpadminbar').length ? wpdepth : 0;


			function sticky_navigation(){
				
				var scroll_top = $(window).scrollTop(); // our current vertical position from the top
			
				// if we've scrolled more than the navigation, change its position to fixed to stick to top, otherwise change it back to relative
				if (scroll_top > sticky_navigation_depth) { 
					$('#site-navigation').css({ 'position': 'fixed', 'top':sticky_navigation_depth, 'left':0 });
					$('body').css('padding-top', ((45+wpdepth)+sticky_navigation_depth)+'px');
					$('.g1').hide();
				} else {
					$('#site-navigation').css({ 'position': 'relative', 'top': 0, 'left':0}); 
					$('body').css('padding-top', '0px');
					$('.g1').show();
				}   
			}
			
			
			// run our function on load
			
			// and run it again every time you scroll
			$(window).scroll(function() {
				 sticky_navigation();
			});
			
			
			
    
    });
}(jQuery));


