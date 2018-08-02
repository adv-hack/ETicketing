var selectedOptions = [];
var maxPricesDDLValues = [];
var minPricesDDLValues = [];
var priceBreakSelectionDDLValues = [];
var standDDLValues = [];
var areaDDLValues = [];
var priceBandValues = [];
var selectedMinimumPrice;
var selectedMaximumPrice;
var selectedPriceBreak;
var selectedStand;
var selectedArea;
var standAndAreaCodeForFilters;
var isPriceAndAreaSelection = false;


//Layout options for the various drop down lists
$(document).ready(function () {
    if (renderStandAreaDDLOptions) {
        var showPricingOptionsAsDropDown = true;
        var showStandAreaOptionsAsDropDown = true;
        var showPriceBandListAsDropDown = true;
        var showTicketExchangeAsDropDown = true;

        if (document.getElementById("hdfShowPricingOptionsAsDropDown") != null) {
            showPricingOptionsAsDropDown = (document.getElementById("hdfShowPricingOptionsAsDropDown").value === "true");
        }
        if (document.getElementById("hdfShowStandAreaOptionsAsDropDown") != null) {
            showStandAreaOptionsAsDropDown = (document.getElementById("hdfShowStandAreaOptionsAsDropDown").value === "true");
        }
        if (document.getElementById("hdfShowPriceBandListAsDropDown") != null) {
            showPriceBandListAsDropDown = (document.getElementById("hdfShowPriceBandListAsDropDown").value === "true");
        }
        if (document.getElementById("hdfShowTicketExchangeAsDropDown") != null) {
            showTicketExchangeAsDropDown = (document.getElementById("hdfShowTicketExchangeAsDropDown").value === "true");
        }

        //Rendering the layout for min/max and price preak drop down lists and foundation drop down elements
        if (showPricingOptionsAsDropDown) {
            $("[data-toggle='js-price-band-filter']").show();
            $("#js-price-band-filter").addClass("large dropdown-pane left dropdown-pane--no-triangle");
            var elem1 = new Foundation.Dropdown($('#js-price-band-filter'), {
                hover: true,
                hoverPane: true
            });
        }
        //Rendering the layout for best available drop down lists and foundation drop down elements
        if (showStandAreaOptionsAsDropDown) {
            $("[data-toggle='js-price-band-best-available']").show();
            $("#js-price-band-best-available").addClass("large dropdown-pane left dropdown-pane--no-triangle");
            var elem2 = new Foundation.Dropdown($('#js-price-band-best-available'), {
                hover: true,
                hoverPane: true
            });
        }
        //Rendering the layout for price bands repeater and foundation drop down elements
        if (showPriceBandListAsDropDown) {
            $("[data-toggle='js-price-band-concession']").show();
            $("#js-price-band-concession").addClass("large dropdown-pane left dropdown-pane--no-triangle");
            var elem3 = new Foundation.Dropdown($('#js-price-band-concession'), {
                hover: true,
                hoverPane: true
            });
        }

        //Rendering the layout for ticket exchange options
        if (showTicketExchangeAsDropDown) {
            $("[data-toggle='js-ticket-exchange']").show();
            $("#js-ticket-exchange").addClass("large dropdown-pane left dropdown-pane--no-triangle");
            var elem4 = new Foundation.Dropdown($('#js-ticket-exchange'), {
                hover: true,
                hoverPane: true
            });
        }
    }
    if (document.getElementById("hdfPriceAndAreaSelection") != null) {
        isPriceAndAreaSelection = (document.getElementById("hdfPriceAndAreaSelection").value === "true");
    }
});


//Rendering the upto date availability when on stand and area selection page where there is no SVG layout
function RetrieveDynamicSeatSelectionOptions(selectingPriceBreakId, selectingMinPrice, selectingMaxPrice, selectingStand, selectingArea) {
    var catMode = document.getElementById('hdfCATMode').defaultValue;
    var productType = document.getElementById('hdfProductType').defaultValue;
    var productCode = document.getElementById('hdfProductCode').defaultValue;
    var stadiumCode = document.getElementById('hdfStadiumCode').defaultValue;
    var campaignCode = document.getElementById('hdfCampaignCode').defaultValue;
    var callId = document.getElementById('hdfCallId').defaultValue;
    var standAndAreaOptions = GetAPISettingsFromStandAndAreaOptions(selectingPriceBreakId, selectingMinPrice, selectingMaxPrice, selectingStand, selectingArea, false, false);

    $.ajax({
        type: "POST",
        url: "StandAndAreaSelection.aspx/RetrieveDynamicStandAndAreaOptions",
        cache: false,
        data: '{data: "' + standAndAreaOptions.SelectedPriceBreak + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","productType":"' + productType + '","campaignCode":"' + campaignCode + '","catMode":"' + catMode + '","callId":"' + callId + '","includeTicketExchangeSeats":"' + standAndAreaOptions.IncludeTicketExchangeSeats + '","minimumPrice":"' + standAndAreaOptions.SelectedMinimumPrice + '","maximumPrice":"' + standAndAreaOptions.SelectedMaximumPrice + '","selectedStand":"' + standAndAreaOptions.SelectedStand + '","selectedArea":"' + standAndAreaOptions.SelectedArea + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var returnValue = msg.d;
            var viewModel = JSON.parse(returnValue);
            if (returnValue != "error") {
                GetSelectedValues();
                SetupDynamicStandAndAreaOptions(viewModel, standAndAreaOptions, selectingStand, selectingArea, selectingPriceBreakId, undefined, false, true);
                GetDynamicOptionValuesAndText();
            }
        },
        error: function (xhr, status) {
            alert(status + '  ' + xhr.responseText);
        }
    });
}


//Get the options to use when calling the availablilty routines. Build a list of selected options based on the hierarchy of what has been selected and return a formatted JSON object.
//selectingPriceBreakId - The user has selected a different price break
//selectingMinPrice - The user has selected a different min price
//selectingMaxPrice - The user has selected a different max price
//selectingStand - The user has changed the stand
//selectingArea - The yser has changed the area
//newAreaForSeats - The user has clicked a different area on the graphical SVG
//selectingReset - The user has clicked the reset button
function GetAPISettingsFromStandAndAreaOptions(selectingPriceBreakId, selectingMinPrice, selectingMaxPrice, selectingStand, selectingArea, newAreaForSeats, selectingReset) {
    var refreshPriceBreakList = true;
    var refreshMinimumPriceList = true;
    var refreshMaximumPriceList = true;
    var refreshAreaList = true;
    var refreshStandList = true;
    var includeTicketExchangeSeats = false;
    var selectedStand;
    var selectedArea;
    var selectedMinimumPrice;
    var selectedMaximumPrice
    var selectedDropdownList;
    var selectedPriceBreak = 0;

    // Retrieve all selected values from drop down lists for API calls
    if (isPriceAndAreaSelection) {
        var selectedStandAndArea = document.getElementById("ddlCombinedStandAndArea").value.split("-");
        selectedStand = selectedStandAndArea[0];
        selectedArea = selectedStandAndArea[1];
        if (selectingStand && selectingArea) {
            refreshStandList = false;//stand and areas are combined so this value cannot be set to true when selecting an area
        }
        if (selectedStandAndArea.length <= 1) {
            selectedArea = "";
        }
    } else {
        selectedStand = document.getElementById("standDropDown").value;
        selectedArea = document.getElementById("areaDropDownList").value;
    }
    if (document.getElementById("ddlMinimumPrice") != null) {
        selectedMinimumPrice = document.getElementById("ddlMinimumPrice").value;
    }
    if (document.getElementById("ddlMaximumPrice") != null) {
        selectedMaximumPrice = document.getElementById("ddlMaximumPrice").value;
    }

    if (document.getElementById("ddlPriceBreakSelection") != undefined) {
        selectedPriceBreak = document.getElementById("ddlPriceBreakSelection").value;
    }

    if (selectingArea) {
        selectedDropdownList = "selectingArea";
    } else if (selectingStand) {
        selectedDropdownList = "selectedStand";
    } else if (selectingMaxPrice) {
        selectedDropdownList = "selectingMaxPrice";
    } else if (selectingMinPrice) {
        selectedDropdownList = "selectingMinPrice";
    } else if (selectingPriceBreakId) {
        selectedDropdownList = "selectingPriceBreakId";
    }

    var found = false;
    for (var i = 0; i < selectedOptions.length; i++) {
        if (found) {
            selectedOptions[i] = "";
        }
        if (selectedOptions[i] == selectedDropdownList) {
            found = true;
        }
    }
    if (!found && selectedDropdownList != undefined) {
        selectedOptions.push(selectedDropdownList);
    }

    if (newAreaForSeats) {
        //user has clicked a graphical area
        refreshMaximumPriceList = true;
        refreshMinimumPriceList = true;
        refreshPriceBreakList = true;
    } else if (selectingReset) {
        //user has clicked the reset option
        refreshPriceBreakList = true;
        refreshMinimumPriceList = true;
        refreshMaximumPriceList = true;
        refreshAreaList = true;
        refreshStandList = true;
        selectedOptions = [];
        selectedMinimumPrice = '0';
        selectedMaximumPrice = '0';
        selectedPriceBreak = '0';
        selectedStand = '';
        selectedArea = '';
    } else {
        //user has clicked one of the drop down list filters
        for (var j = 0; j < selectedOptions.length; j++) {
            switch (selectedOptions[j]) {
                case 'selectingArea':
                    refreshAreaList = false;
                    break;
                case 'selectedStand':
                    refreshStandList = false;
                    break;
                case 'selectingMaxPrice':
                    refreshMaximumPriceList = false;
                    break;
                case 'selectingMinPrice':
                    refreshMinimumPriceList = false;
                    break;
                case 'selectingPriceBreakId':
                    refreshPriceBreakList = false;
                    break;
            }
        }
    }

    if (document.getElementById("ticketExchangeOption") != undefined) {
        if (document.getElementById("ticketExchangeOption").checked) {
            includeTicketExchangeSeats = true;
        }
    }

    //Zeroise the min/max prices when they are being refreshed so that the previous selected values aren't kept
    if (refreshMinimumPriceList && refreshMaximumPriceList) {
        selectedMinimumPrice = "0";
        selectedMaximumPrice = "0";
    }

    var standAndAreaAPIOptions = {
        SelectedMinimumPrice: selectedMinimumPrice,
        SelectedMaximumPrice: selectedMaximumPrice,
        SelectedPriceBreak: selectedPriceBreak,
        SelectedStand: selectedStand,
        SelectedArea: selectedArea,
        RefreshMinimumPriceList: refreshMinimumPriceList,
        RefreshMaximumPriceList: refreshMaximumPriceList,
        RefreshAreaList: refreshAreaList,
        RefreshStandList: refreshStandList,
        RefreshPriceBreakList: refreshPriceBreakList,
        IncludeTicketExchangeSeats: includeTicketExchangeSeats,
        NewAreaForSeats: newAreaForSeats
    }
    return standAndAreaAPIOptions;
}


//Set all the stand/area price break drop down lists based on what the user has selected from the other lists and the availability given from the view model.
function SetupDynamicStandAndAreaOptions(viewModel, standAndAreaAPIOptions, selectingStand, selectingArea, selectingPriceBreakId, priceBreakList, populateFromSavedLists, isStadiumExpaned) {
    if (priceBreakList == undefined) {
        
        //Handle the stand/area options
        setStandAreaOptions(standAndAreaAPIOptions, viewModel, populateFromSavedLists);

        //ebiz-price-band-list clear then hide all elements where they are in the viewMode.PriceBandPricesList.
        //If the price is >= 0 then display the price, if the price is string.Empty then do not show it at all.
        var priceBands = viewModel.PriceBandPricesList;
        $(".ebiz-priceband").each(function () { $(this).hide(); });
        if (isStadiumExpaned && priceBands.length > 0) {
            for (band in viewModel.PriceBandPricesList) {
                var priceBandClass = ".ebiz-priceband-" + priceBands[band].PriceBand;
                $(priceBandClass + " .ebiz-price-band-quantity-box").val("");
                if ($(priceBandClass) != undefined) {
                    $(priceBandClass).show();
                    var priceContainer = priceBandClass + " .ebiz-priceband-price";
                    if (priceBands[band].PriceBandPrice == "") {
                        $(priceContainer).hide();
                    } else {
                        var priceLabel = priceBandClass + " .ebiz-priceband-price-label";
                        $(priceLabel).text(DecodeHTMLString(priceBands[band].PriceBandPrice));
                        $(priceContainer).show();
                    }
                }
            }
        }

        //Rebuild the price break drop down list. Clear the list, unless selected or value = 0.
        if (standAndAreaAPIOptions.RefreshPriceBreakList) {
            var selectedPriceBreak = "0";
            $("#ddlPriceBreakSelection > option").each(function () {
                if ($(this).val() != "0") {
                    $(this).remove();
                } else {
                    $(this).context.selected = true;
                }
            });
            for (var i = 0; i < viewModel.PriceBreakAvailabilityList.length; i++) {
                var option = $("<option>");
                option.attr("value", viewModel.PriceBreakAvailabilityList[i].PriceBreakId);
                option.html(viewModel.PriceBreakAvailabilityList[i].PriceBreakDescription);
                $("#ddlPriceBreakSelection").append(option);
            }
            if (standAndAreaAPIOptions.NewAreaForSeats || populateFromSavedLists) {
                $("#ddlPriceBreakSelection").val(standAndAreaAPIOptions.SelectedPriceBreak);
            }
        }
        

        //Rebuild the min/max drop down list. Clear all the elements and reset the selected values based on smallest number and largest number if a new price break Id is selected.
        if (standAndAreaAPIOptions.RefreshMinimumPriceList) {
            $("#ddlMinimumPrice > option").each(function () { $(this).remove(); });
            for (var i = 0; i < viewModel.PriceBreakPrices.length; i++) {
                var optionMinPrice = $("<option>");
                optionMinPrice.attr("value", viewModel.PriceBreakPrices[i].Price);
                optionMinPrice.html(DecodeHTMLString(viewModel.PriceBreakPrices[i].DisplayPrice));
                if (selectingPriceBreakId && i == 0) {
                    optionMinPrice.attr("selected", "selected");
                } else {
                    if (standAndAreaAPIOptions.SelectedMinimumPrice == viewModel.PriceBreakPrices[i].Price) {
                        optionMinPrice.attr("selected", "selected");
                    }
                }
                if (standAndAreaAPIOptions.SelectedMaximumPrice == "0" || parseFloat(viewModel.PriceBreakPrices[i].Price) <= parseFloat(standAndAreaAPIOptions.SelectedMaximumPrice)) {
                    $("#ddlMinimumPrice").append(optionMinPrice);
                }
            }
        }
        if (standAndAreaAPIOptions.RefreshMaximumPriceList) {
            var maxPriceBreakPrices = [];
            if (populateFromSavedLists) {
                maxPriceBreakPrices = maxPricesDDLValues;
            } else {
                maxPriceBreakPrices = viewModel.PriceBreakPrices;
            }
            $("#ddlMaximumPrice > option").each(function () { $(this).remove(); });
            var optionSelected = false;
            for (var i = 0; i < maxPriceBreakPrices.length; i++) {
                var optionMaxPrice = $("<option>");
                optionMaxPrice.attr("value", maxPriceBreakPrices[i].Price);
                optionMaxPrice.html(DecodeHTMLString(maxPriceBreakPrices[i].DisplayPrice));
                if (standAndAreaAPIOptions.SelectedMaximumPrice == maxPriceBreakPrices[i]) {
                    optionMaxPrice.attr("selected", "selected");
                    optionSelected = true;
                }
                if (parseFloat(maxPriceBreakPrices[i].Price) >= parseFloat(standAndAreaAPIOptions.SelectedMinimumPrice)) {
                    $("#ddlMaximumPrice").append(optionMaxPrice);
                }
            }
            if (!optionSelected) {
                $("#ddlMaximumPrice option:last").attr("selected", "selected");
            }
        }

    } else {

        //When there is no price break
        var selectedPriceBreak = "0";
        $("#ddlPriceBreakSelection > option").each(function () {
            if ($(this).val() != "0") {
                $(this).remove();
            } else {
                $(this).context.selected = true;
            }
        });
        for (var i = 0; i < priceBreakList.length; i++) {
            var option = $("<option>");
            option.attr("value", priceBreakList[i].PriceBreakId);
            option.html(priceBreakList[i].PriceBreakDescription);
            $("#ddlPriceBreakSelection").append(option);
        }
        $("#ddlPriceBreakSelection").val(standAndAreaAPIOptions.SelectedPriceBreak);

        //Rebuild the min/max price drop down lists based on the PriceBreakList
        $("#ddlMinimumPrice > option").each(function () { $(this).remove(); });
        for (var i = 0; i < priceBreakList.length; i++) {
            var optionExists = false;
            if (i > 0) {
                optionExists = ($("#ddlMinimumPrice option[value='" + priceBreakList[i].PriceBreakDefaultPriceBandPrice + "']").length > 0);
            }
            if (!optionExists) {
                var optionMinPrice = $("<option>");
                optionMinPrice.attr("value", priceBreakList[i].PriceBreakDefaultPriceBandPrice);
                optionMinPrice.html(DecodeHTMLString(priceBreakList[i].DisplayPrice));
                if (standAndAreaAPIOptions.SelectedMinimumPrice == priceBreakList[i].PriceBreakDefaultPriceBandPrice) {
                    optionMinPrice.attr("selected", "selected");
                }
                if (standAndAreaAPIOptions.SelectedMaximumPrice == "0" || parseFloat(priceBreakList[i].PriceBreakDefaultPriceBandPrice) <= parseFloat(standAndAreaAPIOptions.SelectedMaximumPrice)) {
                    $("#ddlMinimumPrice").append(optionMinPrice);
                }
            }
        }
        $("#ddlMaximumPrice > option").each(function () { $(this).remove(); });
        var optionSelected = false;
        for (var i = 0; i < priceBreakList.length; i++) {
            var optionExists = false;
            if (i > 0) {
                optionExists = ($("#ddlMaximumPrice option[value='" + priceBreakList[i].PriceBreakDefaultPriceBandPrice + "']").length > 0);
            }
            if (!optionExists) {
                var optionMaxPrice = $("<option>");
                optionMaxPrice.attr("value", priceBreakList[i].PriceBreakDefaultPriceBandPrice);
                optionMaxPrice.html(DecodeHTMLString(priceBreakList[i].DisplayPrice));
                if (standAndAreaAPIOptions.SelectedMaximumPrice == priceBreakList[i].PriceBreakDefaultPriceBandPrice) {
                    optionMaxPrice.attr("selected", "selected");
                    optionSelected = true;
                }
                if (parseFloat(priceBreakList[i].PriceBreakDefaultPriceBandPrice) >= parseFloat(standAndAreaAPIOptions.SelectedMinimumPrice)) {
                    $("#ddlMaximumPrice").append(optionMaxPrice);
                }
            }
        }
        if (!optionSelected) {
            $("#ddlMaximumPrice option:last").attr("selected", "selected");
        }
    }
}


//Get the selected values in the drop down lists
function GetSelectedValues() {
    selectedMaximumPrice = $('#ddlMaximumPrice :selected').val();
    selectedMinimumPrice = $('#ddlMinimumPrice :selected').val();
    selectedPriceBreak = $('#ddlPriceBreakSelection :selected').val();
    selectedStand = "";
    selectedArea = "";
    if (isPriceAndAreaSelection) {
        if ($('.ebiz-combined-stand-area-drop-down :selected').val() != "") {
            var selectedStandAndArea = $('.ebiz-combined-stand-area-drop-down :selected').val().split("-");
            selectedStand = selectedStandAndArea[0];
            if (selectedStandAndArea.length > 1) {
                selectedArea = selectedStandAndArea[1];
            }
        }
    } else {
        selectedStand = $('#standDropDown :selected').val();
        selectedArea = $('#areaDropDownList :selected').val();
    }
}


//Get the values from all the drop down lists so that the selected values can be set again. Used when navigating between stand view and seat view
function GetDynamicOptionValuesAndText() {
    var maximumPriceDDLValues = [];
    var minimumPriceDDLValues = [];
    priceBreakSelectionDDLValues = [];
    standDDLValues = [];
    areaDDLValues = [];
    priceBandValues = [];
    minPricesDDLValues = [];
    maxPricesDDLValues = [];

    $('#ddlMinimumPrice > option').each(function () {
        var minimumPrice = $(this);
        if (minimumPrice.val() != '') {
            minPricesDDLValues.push({ Price: minimumPrice.val(), DisplayPrice: minimumPrice.text() });
        }
    });
    $('#ddlMaximumPrice > option').each(function () {
        var maximumPrice = $(this);
        if (maximumPrice.val() != '') {
            maxPricesDDLValues.push({ Price: maximumPrice.val(), DisplayPrice: maximumPrice.text() });
        }
    });
    $('#ddlPriceBreakSelection > option').each(function () {
        var priceBreakItem = $(this);
        if (priceBreakItem.val() != '0') {
            priceBreakSelectionDDLValues.push({ PriceBreakId: priceBreakItem.val(), PriceBreakDescription: priceBreakItem.text() });
        }
    });
    if (isPriceAndAreaSelection) {
        $('.ebiz-combined-stand-area-drop-down > option').each(function () {
            var standAndArea = $(this);
            if (standAndArea.val() != '') {
                var tempString = standAndArea.val().split("-");
                var addStand = true;
                var addArea = true;
                for (var s = 0; s < standDDLValues.length > 0; s++) {
                    if (standDDLValues[s].StandCode == tempString[0]) {
                        addStand = false;
                        break;
                    }
                }
                for (var a = 0; a < areaDDLValues.length > 0; a++) {
                    if (areaDDLValues[a].AreaCode == tempString[1]) {
                        addArea = false;
                        break;
                    }
                }
                if (addStand) {
                    var standObject = ({ StandCode: tempString[0] });
                    standDDLValues.push(standObject);
                }
                if (addArea) {
                    var areaObject = ({ AreaCode: tempString[1] });
                    areaDDLValues.push(areaObject);
                }
            }
        });
    } else {
        $('#standDropDown > option').each(function () {
            var stand = $(this);
            if (stand.val() != '') {
                standDDLValues.push({ StandCode: stand.val() });
            }
        });
        $('#areaDropDownList > option').each(function () {
            var area = $(this);
            if (area.val() != '') {
                areaDDLValues.push({ AreaCode: area.val() });
            }
        });
    }
}

//Set all the values into a mocked view model and all the searching options. Used when navigating between stand view and seat view
function SetDynamicOptionValuesAndText() {
    if (defaultToSeatSelection) {
        //At this point the user has landed on this page via an external page and navigated straight to the seat selection, 
        //therefore the stadium needs resetting and this should only happen once since they will have clicked "back to stadium view"
        ResetStadium();
        defaultToSeatSelection = false;
    }
    var viewModel = ({
        AvailableStands: standDDLValues,
        AvailableAreas: areaDDLValues,
        PriceBreakAvailabilityList: priceBreakSelectionDDLValues,
        PriceBandPricesList: priceBandValues,
        PriceBreakPrices: minPricesDDLValues

    });
    var standAndAreaAPIOptions = ({
        SelectedMinimumPrice: selectedMinimumPrice,
        SelectedMaximumPrice: selectedMaximumPrice,
        SelectedPriceBreak: selectedPriceBreak,
        SelectedStand: selectedStand,
        SelectedArea: selectedArea,
        RefreshMinimumPriceList: true,
        RefreshMaximumPriceList: true,
        RefreshAreaList: false,
        RefreshStandList: false,
        RefreshPriceBreakList: true,
        IncludeTicketExchangeSeats: false,
        NewAreaForSeats: false
    });
    SetupDynamicStandAndAreaOptions(viewModel, standAndAreaAPIOptions, false, false, false, undefined, true, false);
}


//Handle the stand/area or combined drop down lists options based on the view model and the API options
function setStandAreaOptions(standAndAreaAPIOptions, viewModel, populateFromSavedLists) {
    if (isPriceAndAreaSelection) {

        if (standAndAreaAPIOptions.RefreshStandList) {
            $(".ebiz-combined-stand-area-drop-down > option").each(function () {
                if ($(this).val() != "") {
                    $(this).hide();
                } else {
                    //set the "Any Area" or "Please select" option to be selected
                    $(this).context.selected = true;
                }
            });
            //Then show the stands/areas that are available
            if (viewModel.AvailableStands.length > 0) {
                for (var s = 0; s < viewModel.AvailableStands.length; s++) {
                    for (var a = 0; a < viewModel.AvailableAreas.length; a++) {
                        $(".ebiz-combined-stand-area-drop-down > option").each(function () {
                            if ($(this).val() == viewModel.AvailableStands[s].StandCode + "-" + viewModel.AvailableAreas[a].AreaCode) {
                                $(this).show();
                            }
                        });
                    }
                }
            }
        }
        if (viewModel.AvailableStands.length > 0 && populateFromSavedLists) {
            if (standAndAreaAPIOptions.SelectedStand == "" && standAndAreaAPIOptions.SelectedArea == "") {
                $(".ebiz-combined-stand-area-drop-down").val("");
            } else {
                $(".ebiz-combined-stand-area-drop-down").val(standAndAreaAPIOptions.SelectedStand + "-" + standAndAreaAPIOptions.SelectedArea);
            }
        }
        if (populateFromSavedLists) {
            if (standAndAreaAPIOptions.SelectedStand == "" && standAndAreaAPIOptions.SelectedArea == "") {
                $(".ebiz-combined-stand-area-drop-down").val("");
            } else {
                $(".ebiz-combined-stand-area-drop-down").val(standAndAreaAPIOptions.SelectedStand + "-" + standAndAreaAPIOptions.SelectedArea);
            }
        }
        
    } else {

        if (standAndAreaAPIOptions.RefreshStandList) {
            //Make all stands/areas invisible first
            $(".ebiz-stand-drop-down > option").each(function () {
                if ($(this).val() != "") {
                    $(this).hide();
                } else {
                    //set the "Any stand" or "Please select" option to be selected
                    $(this).context.selected = true;
                }
            });
            //Then show the stands that are available
            if (viewModel.AvailableStands.length > 0) {
                for (var i = 0; i < viewModel.AvailableStands.length; i++) {
                    $(".ebiz-stand-drop-down > option").each(function () {
                        if ($(this).val() == viewModel.AvailableStands[i].StandCode) {
                            $(this).show();
                        }
                    });
                }
            }
        }
        if (viewModel.AvailableStands.length > 0 && populateFromSavedLists) {
            $("#standDropDown").val(standAndAreaAPIOptions.SelectedStand);
        }
        if (populateFromSavedLists) {
            $("#areaDropDownList").val(standAndAreaAPIOptions.SelectedArea);
        }
        //Sort out the selectable areas if they need to be setup
        availableAreaCodes = viewModel.AvailableAreas;
        //hide all the items in the list first
        $(".ebiz-area-drop-down > option").each(function () { $(this).hide(); });
        if (standAndAreaAPIOptions.SelectedStand != "") {
            //loop through the available areas and then show them if they are part of this stand
            if (availableAreaCodes.length > 0) {
                for (var i = 0; i < availableAreaCodes.length; i++) {
                    $(".ebiz-area-drop-down > option").each(function () {
                        if ($(this).val() != "") {
                            if ($(this).val() == availableAreaCodes[i].AreaCode) {
                                $(this).show();
                            }
                        } else {
                            $(this).show();
                        }
                    });
                }
            }
        } else {
            //show only the "please select option" if no stand is selected
            $(".ebiz-area-drop-down > option").each(function () {
                if ($(this).val() == "") {
                    $(this).show();
                    $(this).context.selected = true;
                }
            });
        }

    }
}

//filter areas drop down list based on price break selected
function filterAreaDDL() {
    //hide all the items in the list first
    $(".ebiz-area-drop-down > option").each(function () { $(this).hide(); });

    //loop through the available areas and then show them if they are part of this stand
    if (availableAreaCodes.length > 0) {
        for (var i = 0; i < availableAreaCodes.length; i++) {
            $(".ebiz-area-drop-down > option").each(function () {
                if ($(this).val() != "") {
                    if ($(this).val() == availableAreaCodes[i]) {
                        $(this).show();
                    }
                } else {
                    $(this).show();
                }
            });
        }
    }
}


//Get the querystring value based on the parameter name
function GetParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var queryParams = location.search.toUpperCase();
    name = name.toUpperCase();
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
		results = regex.exec(queryParams);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
};

// Reset the  Ticket Exchange Panel and slider on stand click.
function resetTicketExchangePanel() {
    $(".c-ticket-exchange").hide();
    $(".slider").addClass("disabled");
    if (selectedStandAreaTEAllowed) {
        $(".c-ticket-exchange").show();
        if (selectedStandAreaTEMinPrice && selectedStandAreaTEMaxPrice && $.isNumeric(selectedStandAreaTEMinPrice) && $.isNumeric(selectedStandAreaTEMaxPrice)) {

            $(".ebiz-slider-wrap").show();
            var sliderID = $('.slider')[0].id
            $('#' + sliderID).foundation('destroy');

            // Reset slider Min & Max position
            $("#slider-handle-min").css('left', '0%');
            $("#slider-handle-max").css('left', '100%');

            $options = {
                "start": Math.floor(selectedStandAreaTEMinPrice),
                "end": Math.ceil(selectedStandAreaTEMaxPrice),
                "initialStart": Math.floor(selectedStandAreaTEMinPrice),
                "initialEnd": Math.ceil(selectedStandAreaTEMaxPrice),
                "decimal": 2
            };
            var mySlider = new Foundation.Slider($('#' + sliderID), $options);

            var isDown = false;
            $(document).mouseup(function (e) {
                if (isDown) {            
                    isDown = false;
                    if ($('.slider').hasClass("disabled")) {
                        return false;
                    } else {
                        $(".slider").addClass("disabled");
                        ticketExchangeAndSilding = true;
                        createSeatSelection(stand, undefined);
                    }
                    e.preventDefault();
                }
                
            });

            $('[data-slider]').on('mousedown.zf.slider', function () {    
                isDown = true;      
            });
        }
    }
}

//Handle javascript side HTML Decoding
function DecodeHTMLString(valueToDecode) {
    var decodedString;
    decodedString = $('<textarea />').html(valueToDecode).text();
    return decodedString;
}
