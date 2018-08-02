// Foundation
$(document).foundation();

// FastClick
/*$(function() {
    FastClick.attach(document.body);
});*/

// Previous Modal Functionality
//$(document).on('closed.zf.reveal', '[data-reveal]', function(e){           
//    var modal = $(this);
//    if (modal.hasClass("ebiz-reveal-ajax")) {
//        modal.empty();
//    }
//});

//=========================================================================
// Modal Function
// Insert the reveal modal container inside the form tag
// http://foundation.zurb.com/forum/posts/1187-aspnetfoundation-reveal-modal

/* $(function() {
    $(".reveal-overlay").appendTo("form");
}); */

$(document).on('open.zf.reveal', '[data-reveal]', function () {
    $(this).parent().appendTo($("form"));
});


//Open an external page using AJAX with a reveal
$(".ebiz-open-modal").bind("click", function (e) {
    e.preventDefault();
    var url = ($(this).attr("href"));
    var modal = "#" + ($(this).attr("data-open"));
    $.ajax(url).done(function (data) {
        $(modal).html(data).foundation();
    });
});

//Close inline reveal modal
$(document).on('closed.zf.reveal', function (e) {
    var modalDiv = $(e.target)[0];
    if (modalDiv.classList.contains("ebiz-reveal-ajax")) {
        modalDiv.innerHTML = "";
    }
});


//Close inline reveal modal
$(document).on('closed.fndtn.reveal', '[data-reveal]', function (e) {
    if (e.namespace !== 'fndtn.reveal') {
        return;
    }

    // handle namespaced event               
    var modal = $(this);
    if (!modal.hasClass(".ebiz-inline-modal")) {
        modal.empty();
    }
});
    //=========================================================================

    // Hide Empty TD Tags - Small and Medium Only
    $(window).on("load resize",function(e){
        var emptyTdTag = Foundation.MediaQuery.current;
        if(emptyTdTag == "small" || emptyTdTag == "medium") {
            $("td:empty").hide();
        } else {
            $("td:empty").show();
        }
    });

// Select2 - Select Box Replacement
    $(".select2").select2();
//Select2 - select box on template override
    $(document).ready(function () {
      $('.js-select2--default').select2();
      $('.js-select2--tags-no-creation').select2({
          tags: true,
          createTag: function (params) {
              return undefined;
          }
      });
  });
 
    // Calculate Maximum Widths
    (function ( $ ) {
        $.fn.maxWidth = function( options ) {
            var settings = $.extend({
                breakpoint: ''
            }, options);
            var maxWidth = Math.max.apply(null, $(this).map(function(){
                return $(this).outerWidth(true);
            }).get());
            if (Foundation.MediaQuery.atLeast(settings.breakpoint)) {
                $(this).width(maxWidth).css("min-height", "1px");
            } else {
                $(this).css('width', 'auto');
            }
        };
    }( jQuery ));


    //Alerts Open
    function OpenAlertsWindow() {
        if (Foundation.MediaQuery.atLeast('medium')) {
            $('#ebiz-alert-reveal').foundation('open');
        }
    }

    /*=============================================
    =           Page Specific Snippets            =
    =============================================*/

    // browseXX.aspx
    $(function() {
        $(".ebiz-graphical-product-item .panel").matchHeight();
        $(".ebiz-graphical-product-item h2").matchHeight();
        $(".ebiz-graphical-product-item .ebiz-long-description").matchHeight();
    });


    // DespatchNoteGeneration.aspx
    // Despatch Note Generation Search Scroll To Top
    $(function() {
        $(".ebiz-despatch-note-generation-search .button").click(function(event){
            $("html, body, .off-canvas-wrapper").animate({
                scrollTop: ($(".off-canvas-content").offset().top)
            },500);
        });
    });


    // ProductSubTypes.aspx
    $(function() {
        $(".ebiz-product-sub-types .panel").matchHeight();
        $(".ebiz-product-sub-types h2").matchHeight();
        $(".ebiz-product-sub-types .ebiz-subtype-image").matchHeight();
    });


    // PurchaseDetails.aspx
    // Dynamically set a TD colspan to the same number of header columns
    var ebizOrderDetailsColumns = $(".ebiz-order-details thead tr").children().length;
    $(".ebiz-order-details .ebiz-actions").attr("colspan", ebizOrderDetailsColumns);


    // ComponentSelection.aspx
    // Dynamically set a TD colspan to the same number of header columns
    var ebizOrderDetailsColumns = $(".ebiz-component-seats thead tr").children().length;
    $(".ebiz-component-seats .ebiz-ebiz-component-seats-container").attr("colspan", ebizOrderDetailsColumns);


    /*=================================
    =           Components            =
    ===================================*/

    // Price band selection
    $(function() {
        $(".c-price-band-selection .columns").matchHeight();
    });

// Fancy Checkboxes and Radio Buttons
    var nAgt = navigator.userAgent;
    var browserName = "";
    var verOffset;
    if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
        browserName = "Firefox";
    }
    if (browserName.toUpperCase() != "FIREFOX") {
        $(function () {
            try {
                $(":not(.switch,.js-no-icheck) > input, input[disabled]").iCheck({
                    checkboxClass: "icheckbox_square",
                    radioClass: "iradio_square"
                });
            } catch (e) {

            }
        }).trigger('icheck');
    }
    try {
        $('.ebiz-accordion').find('input').iCheck();
        } catch (e) {
    }
    


    // DespatchProcess.aspx Print All
    // https://github.com/fronteed/icheck/issues/58
    $(function () {
        $('#chkPrintAll')
          .on('ifChecked', function (event) {
              $('.ebiz-print-checkbox input').iCheck('check');
          })
          .on('ifUnchecked', function () {
              $('.ebiz-print-checkbox input').iCheck('uncheck');
          });
    });


    // DespatchNoteGeneration.aspx & generic PostBack container class
    function pageLoad(sender, args) {
        try {
            $(".ebiz-despatch-note-generation-results input, .ebiz-adjustment-type-wrap input, .js-icheck-postback-container, .ebiz-accordion input").iCheck({
                checkboxClass: "icheckbox_square",
                radioClass: "iradio_square"
            });
        } catch (e) {

        }
        $('input[type="checkbox"], input[type="radio"]').on('ifChanged', function (e) {
            $(this).trigger("onclick", e);
        });
    }


    // Does the iCheck trigger click and fire postback
    // https://github.com/fronteed/icheck/issues/158
    $('.u-radio-postback__container input').each(function () {
        $onclick = $(this).attr("onclick");
        $iCheckName = $(this).attr("name");
        $buttonTrigger = 'input[name="' + $iCheckName + '"';
        var $this = $(this);
        if ($onclick != undefined) {
            if ($onclick.length > 0) {
                $($buttonTrigger).on('ifChecked', function (event) {
                    $(this).trigger("click");
                });
            }
        }
    });


    // jQuery UI Touch Punch is a small hack that enables the use of touch events on sites using the jQuery UI user interface library.
    $(".ui-slider-handle").draggable();