var currencySymbol;
var CurrentlyOffSaleText, CurrentlyOnSaleText, TakingOffSaleText, PlacingOnSaleText, PriceChangeText, SoldText;

//initialization variable, used so that the sliders aren't set onload, only when the user moves the slider. The moved function is called when the page loads.
var initialisation = true;
var mainSliderInit = true;

$(window).load(function () {
    initialisation = false;

    var colAry = $(".ebiz-status-label");
    var arrayLength = colAry.length;
    for (var i = 0; i < arrayLength; i++) {
        setRowClass(colAry[i].id);
    }
});

$(document).ready(function () {
    $(".ebiz-ticket-exchange-products-results").DataTable(
        {
            "bSort" : false
        } 
     );
    
    if (document.getElementById('hdfCurrencySymbol')) {
        currencySymbol = document.getElementById('hdfCurrencySymbol').value;
    }
    if (document.getElementById('hdfCurrentlyOffSaleText')) {
        CurrentlyOffSaleText = document.getElementById('hdfCurrentlyOffSaleText').value;
    }
    if (document.getElementById('hdfCurrentlyOnSaleText')) {
        CurrentlyOnSaleText = document.getElementById('hdfCurrentlyOnSaleText').value;
    }
    if (document.getElementById('hdfTakingOffSaleText')) {
        TakingOffSaleText = document.getElementById('hdfTakingOffSaleText').value;
    }
    if (document.getElementById('hdfPlacingOnSaleText')) {
        PlacingOnSaleText = document.getElementById('hdfPlacingOnSaleText').value;
    }
    if (document.getElementById('hdfPriceChangeText')) {
        PriceChangeText = document.getElementById('hdfPriceChangeText').value;
    }
    if (document.getElementById('hdfSoldText')) {
        SoldText = document.getElementById('hdfSoldText').value;
    }
});


function flagChange(lblStatusID)
{
    if (!initialisation) {
        if (document.getElementById(lblStatusID).innerText == CurrentlyOffSaleText) {
            document.getElementById(lblStatusID).innerText = PlacingOnSaleText;
        } else if (document.getElementById(lblStatusID).innerText == CurrentlyOnSaleText) {
            document.getElementById(lblStatusID).innerText = TakingOffSaleText;
        } else if (document.getElementById(lblStatusID).innerText == PlacingOnSaleText) {
            document.getElementById(lblStatusID).innerText = CurrentlyOffSaleText;
        } else if (document.getElementById(lblStatusID).innerText == TakingOffSaleText) {
            document.getElementById(lblStatusID).innerText = CurrentlyOnSaleText;
        } else if (document.getElementById(lblStatusID).innerText == PriceChangeText) {
            document.getElementById(lblStatusID).innerText = TakingOffSaleText;
        }
        // Onsale status may be a price change.
        priceChange(lblStatusID)      
    }
    // Set the class name for teh row based on the new status
    setRowClass(lblStatusID)
}

// Price change slider
function priceChange(lblStatusID)
{
    if (!initialisation) {
        if (document.getElementById(lblStatusID).innerText == CurrentlyOnSaleText) {
            var repeaterPosition = lblStatusID.split("_");
            var hdfOriginalResaleValue = $("#" + generateFullId(repeaterPosition, "hdfOriginalResaleValue"));
            var txtResaleSlider = $("#" + generateFullId(repeaterPosition, "txtResaleSlider"));

            if (txtResaleSlider.val() && !(hdfOriginalResaleValue.val() == txtResaleSlider.val())) {
                document.getElementById(lblStatusID).innerText = PriceChangeText
                setRowClass(lblStatusID)
            }
        }
    }
}

// Set the class name for teh row based on the new status
function setRowClass(lblStatusID) {

    var className;
    if (document.getElementById(lblStatusID).innerText == CurrentlyOffSaleText) {
        className = "ebiz-row-status-off-sale"
    } else if (document.getElementById(lblStatusID).innerText == CurrentlyOnSaleText) {
        className = "ebiz-row-status-on-sale"
    } else if (document.getElementById(lblStatusID).innerText == TakingOffSaleText) {
        className = "ebiz-row-status-taking-off-sale"
    } else if (document.getElementById(lblStatusID).innerText == PlacingOnSaleText) {
        className = "ebiz-row-status-placing-on-sale"
    } else if (document.getElementById(lblStatusID).innerText == PriceChangeText) {
        className = "ebiz-row-status-price-change"
    } else if (document.getElementById(lblStatusID).innerText == SoldText) {
        className = "ebiz-row-status-sold"
    }

    $('#' + lblStatusID).closest("tr").removeClass();
    $('#' + lblStatusID).closest("tr").addClass(className);
}
    
$(".ebiz-main-slider").on("moved.zf.slider", function () {
        
        var value1;
        var value2;
        value1 = parseFloat($("#ebiz-main-slider-value").val()).toFixed(2);
        $("#ebiz-main-slider-value").val(value1);
        var first = false
        if (!initialisation && !mainSliderInit) {
            $(".ebiz-price-slider").each(function (index) {
                //This is a way of setting the sliders dynamically in the repeater
                var repeaterPosition = $(this)[0].id.split("_");
                var lblStatus = document.getElementById(generateFullId(repeaterPosition, "lblStatus"));
                if (!(lblStatus.innerText == SoldText)) {
                    if ($(".ebiz-price-slider_" + index).length > 0) {
                        mySlider = new Foundation.Slider($(".ebiz-price-slider_" + index), { initialStart: value1 })
                    }
                }
            });
        
            $(".ebiz-price-slider-value").each(function (index) {
                value2 = parseFloat($(".ebiz-price-slider-value_" + index).val()).toFixed(2);
                $(".ebiz-price-slider-value_" + index).val(value2);
            }); 
        }
        mainSliderInit = false;
    });

    
    // Highlight "YOU WILL EARN" value on slide
    $(function(){

        var isDown = false;
        $(".ebiz-ticket-exchange-select-tickets-wrap .slider-handle").mousedown(function(e){
            $("td.ebiz-you-will-earn").removeClass("ebiz-moving-handle");
            
            var repeaterPosition = $(this)[0].id.split("_");
            var lblStatus = document.getElementById(generateFullId(repeaterPosition, "lblStatus"));
            if (!(lblStatus.innerText == SoldText)) {
                $(this).closest("tr").find("td.ebiz-you-will-earn").addClass("ebiz-moving-handle");
            }
            isDown = true;
            e.preventDefault();
        });

        $(document).mouseup(function(){
            if(isDown){
                $("td.ebiz-you-will-earn").removeClass("ebiz-moving-handle");
                $(".ebiz-ticket-exchange-set-all-tickets-wrap .ebiz-price-slider-value").blur();
                isDown = false;
            }
        }); 

        $(".ebiz-price-slider-value").on("blur", function(){
            $("td.ebiz-you-will-earn").removeClass("ebiz-moving-handle");
        }).on("focus", function () {
            var repeaterPosition = $(this)[0].id.split("_");
            var lblStatus = document.getElementById(generateFullId(repeaterPosition, "lblStatus"));
            if (!(lblStatus.innerText == SoldText)) {
                $(this).closest("tr").find("td.ebiz-you-will-earn").addClass("ebiz-moving-handle");

            }
        });

        $(".ebiz-ticket-exchange-set-all-tickets-wrap .slider-handle").mousedown(function(e){
            var PotEarningAry = $("td.ebiz-you-will-earn");
            var arrayLength = PotEarningAry.length;
            for (var i = 0; i < arrayLength; i++) {
                if ($(PotEarningAry[i]).closest("tr").find('.ebiz-resale-label').length == 0) {
                    PotEarningAry[i].className += " ebiz-moving-handle";
                }
            }
            isDown = true;
            e.preventDefault();
        });

        $(".ebiz-ticket-exchange-set-all-tickets-wrap .ebiz-price-slider-value").on("blur", function(){
            $("td.ebiz-you-will-earn").removeClass("ebiz-moving-handle");
        }).on("focus", function () {
            var PotEarningAry = $("td.ebiz-you-will-earn");
            var arrayLength = PotEarningAry.length;
            for (var i = 0; i < arrayLength; i++) {
                if ($(PotEarningAry[i]).closest("tr").find('.ebiz-resale-label').length == 0) {
                    PotEarningAry[i].className += " ebiz-moving-handle";
                }
            }
        });

    });


    $(".ebiz-price-slider").on("moved.zf.slider", function () {
        var repeaterPosition = $(this)[0].id.split("_");

        var lblStatus = document.getElementById(generateFullId(repeaterPosition, "lblStatus"));
        if (!(lblStatus.innerText == SoldText)) {

            var selectedResalePrice = $(this).children('.slider-handle').attr('aria-valuenow');

            // Retrieve the club fee, fee type, original sale and face values for this item.
            var hdfOriginalPrice = $("#" + generateFullId(repeaterPosition, "hdfOriginalPrice"));
            var hdfFaceValue = $("#" + generateFullId(repeaterPosition, "hdfFaceValue"));
            var hdfProductFeeValue = $("#" + generateFullId(repeaterPosition, "hdfProductFeeValue"));
            var hdfProductFeeType = $("#" + generateFullId(repeaterPosition, "hdfProductFeeType"));
            var lblStatusID = generateFullId(repeaterPosition, "lblStatus");

            var calculatedFee;
            var calculatedEarning;

            if (hdfProductFeeType.val() == "P") {
                calculatedFee = (selectedResalePrice * hdfProductFeeValue.val() / 100);
                calculatedFee = calculatedFee.toFixed(2);
            } else {
                calculatedFee = hdfProductFeeValue.val();
            }

            var concessionDiff = parseFloat(hdfFaceValue.val()).toFixed(2) - parseFloat(hdfOriginalPrice.val()).toFixed(2);
            if (!concessionDiff || concessionDiff < 0) {
                concessionDiff = 0
            }

            calculatedEarning = selectedResalePrice - concessionDiff - calculatedFee;
            calculatedEarning = Math.max(0, calculatedEarning);
            calculatedEarning = calculatedEarning.toFixed(2);

            // Set the corresponding columns with selected values.
            var value = $(this).closest('td').find(':input[type="number"]').val();
            $(this).closest('td').find(':input[type="number"]').val(parseFloat(value).toFixed(2));
            $(this).closest('tr').find('td.ebiz-club-fee').html("<span>" + currencySymbol + calculatedFee + "</span>");
            $(this).closest('tr').find('td.ebiz-you-will-earn').html("<span>" + currencySymbol + calculatedEarning + "</span>");

            // Price changed status if the item is currently on sale.
            priceChange(lblStatusID, true)
        }
    });

    $(".ebiz-price-slider-value").on('change', function () {
        if (!$(this).val() || $(this).val() == "Nan" || $(this).val() == undefined || $(this).val() < 0) {
            $(this).val(0);
        }
    });
    var checkboxList;
    function validateStatusCheckbox(sender, args)
    {
        var isStatusValid = false;
        $(".ebiz-exchange-checkbox").each(function (index) {
            if ($(this).checked) {
                isStatusValid = true;             
            }
            else
            {
                var checkBoxId = $(this)[0].childNodes[0].id;
                var checkBoxIdSplit = checkBoxId.split("_");
                var hdfOriginalCheckedObject = $("#" + generateFullId(checkBoxIdSplit, "hdfOriginalChecked"));
                var hdfOriginalStatusObject = $("#" + generateFullId(checkBoxIdSplit, "hdfOriginalStatus"));
                var txtResaleSliderObject = $("#" + generateFullId(checkBoxIdSplit, "txtResaleSlider"));
                var hdfOriginalResaleValueObject = $("#" + generateFullId(checkBoxIdSplit, "hdfOriginalResaleValue"));

                if (hdfOriginalCheckedObject[0].value == "False")
                {
                    if (hdfOriginalStatusObject[0].value != 0 && txtResaleSliderObject[0].value == hdfOriginalResaleValueObject[0].value)
                    {
                        isStatusValid = true;
                    }
                }
            }
        });
        if (isStatusValid) {
            args.IsValid = true;
            $("#clientside-errors-wrapper").visible = true;
        }
        else
        {
            args.IsValid = false;
        }
    }

    function generateFullId(elementId, toElement)
    {
        var fullID = elementId[0];
        for (x = 1; x < elementId.length; x++)
        {
            if (x == elementId.length - 2) {
                fullID += "_" + toElement;
            } else { 
                fullID += "_" + elementId[x];
            }
        }
        return fullID;
    }
    function updateStageOnQueryString(thisStage, nextStage) {
        if (isACheckboxChecked() || thisStage > nextStage) {
            var el = document.getElementById('form1');
            thisStage = "stage=" + thisStage;
            nextStage = "stage=" + nextStage;
            el.onsubmit = function (evt) {
                var url = $(this).attr('action');
                url = url.replace(thisStage, nextStage);
                $(this).attr('action', url);
            };
        }
       
    }

    function isACheckboxChecked() {
        var checkFound = false;
        $('input[type=checkbox]').each(function () {
            if (this.checked) {
                checkFound = true;
            }
        });
        return checkFound;
    }