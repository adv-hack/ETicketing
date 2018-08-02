// Include the Hammer.JS library.
// Apply the code the smallest breakpoint only -- !important the breakpoint is configurable!

if ( $(window).width() < 640) {      

    // Add pinch and pan functionality to the stadium and seat selection SVGs.

    function stadiumSeatingAreaPanAndPinch(querySelector, elementIdentifier)
    {

        // https://gist.github.com/synthecypher/f778e4f5a559268a874e

        var webpage = document.querySelector(querySelector);
        var image = document.getElementById(elementIdentifier);

        var mc = new Hammer.Manager(image);

        var pinch = new Hammer.Pinch();
        var pan = new Hammer.Pan();

        pinch.recognizeWith(pan);

        mc.add([pinch, pan]);
        // mc.add([pan]);

        var adjustScale = 1;
        var adjustDeltaX = 0;
        var adjustDeltaY = 0;

        var currentScale = null;
        var currentDeltaX = null;
        var currentDeltaY = null;

        // Prevent long press saving on mobiles.
        // webpage.addEventListener('touchstart', function (e) {
            // e.preventDefault()
        // });

        // Handles pinch and pan events/transforming at the same time;
        mc.on("pinch pan", function (ev) {

            var transforms = [];

            // Adjusting the current pinch/pan event properties using the previous ones set when they finished touching
            currentScale = adjustScale * ev.scale;
            currentDeltaX = adjustDeltaX + (ev.deltaX / currentScale);
            currentDeltaY = adjustDeltaY + (ev.deltaY / currentScale);

            // Concatinating and applying parameters.
            transforms.push('scale(' + currentScale + ')');
            transforms.push('translate(' + currentDeltaX + 'px,' + currentDeltaY + 'px)');
            webpage.style.webkitTransform = transforms.join(' ');
            // webpage.style.MozTransform = transforms.join(' ');
            // webpage.style.msTransform = transforms.join(' ');
            // webpage.style.OTransform = transforms.join(' ');
            webpage.style.transform = transforms.join(' ');
        });

        mc.on("panend pinchend", function (ev) {

            // Saving the final transforms for adjustment next time the user interacts.
            adjustScale = currentScale;
            adjustDeltaX = currentDeltaX;
            adjustDeltaY = currentDeltaY;

        });
    }

    (function(){
        stadiumSeatingAreaPanAndPinch('.ebiz-stadium', 'stadium-canvas');
        stadiumSeatingAreaPanAndPinch('.ebiz-seat-selection-inner-container', 'select');
    })();


    // Scroll to the seat list panel if the markup in the seat list panel changes.
    // https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver

    // select the target node
    var target = document.getElementById('detailed-seat-list-panel');
     
    // create an observer instance
    var observer = new MutationObserver(function(mutations) {
      mutations.forEach(function(mutation) {
        // console.log(mutation.type);
        // $('#detailed-seat-list-panel')[0].scrollIntoView(false); 
      });    
    });

    // configuration of the observer:
    var config = { attributes: true, subtree: true };

    // pass in the target node, as well as the observer options
    observer.observe(target, config);

    $(function(){
        $("#loading-image").html("<i class='fa fa-spinner fa-pulse fa-fw'></i><h2 style='color:#fff;'>Loading Seats&hellip;</h2>");
    });


    // Toggle the visibility of the visual seat selection SVGs.
    // Include '<a href="#" class="button toggleStadium">Show Visual Seat Selection</a>' on VisualSeatSelection.aspx.

    $(function(){
        $(".toggleStadium").insertAfter(".ebiz-seat-selection.ebiz-information").hide();
        $("#quick-buy-option").insertBefore(".ebiz-stadium");
    });
	
    $(".toggleStadium").on("click", function(){
        if($(this).text()=="Show Stadium Seating Plan"){
            $(this).text("Hide Stadium Seating Plan");
        } else {
            $(this).text("Show Stadium Seating Plan");
        }
        $("body").toggleClass("hideStadium"); 
        return false;
    });
}

 $(window).bind("load", function () {
  $(".toggleStadium").css("display","block");
});



