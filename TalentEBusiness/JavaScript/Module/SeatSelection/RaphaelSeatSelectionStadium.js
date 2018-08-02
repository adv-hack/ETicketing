var stadiumCanvas; //toggles settings for when the stadium has been shrunk or expanded
var stand;
var standTipText = "";
var over = false;
var viewFromAreaUrl;
var standTipAreaDesc;
var standTipStandDesc;
var standTipAvailibilityDesc;
var standTipPriceDesc;
var areaId;
var textId;
var standAreaId;
var isAvailibilityView = true;
var isStadiumExpaned = true;
var standPassedOrphanBenchmark = true;
var standPriceDesc = [];
var hoverColour;
var selectedColour;
var selectedTextColour;
var availableStandCodes = [];
var availableAreaCodes = [];
var selectedStandAndArea = { stand: "", area: "" };
var selectedStandAreaTEAllowed;
var selectedStandAreaTEMinPrice;
var selectedStandAreaTEMaxPrice;

/* Stadium code: Draws stadium stands and pitch, attaches tooltip descriptions to stands, adds events to stands...*/
window.onload = function () {
    $("#detailed-seat-list-panel").hide();
    $(".ebiz-slider-wrap").hide();
    $(".ebiz-svg-container").addClass(document.getElementById('hdfStadiumCode').defaultValue);
    $(".ebiz-svg-container").addClass(document.getElementById('hdfProductCode').defaultValue);
    $(".ebiz-svg-container").addClass('ebiz-product-type-' + document.getElementById('hdfProductType').defaultValue);
    if (document.getElementById("hdfSelectedSeats") != undefined && basket.length == 0) {
        document.getElementById("hdfSelectedSeats").value = "";
    }

    // retrieve CAT mode seats and populate JS basket
    var transferableBasketItems = document.getElementById("hdfTransferableBasketItems").value;
    if (transferableBasketItems) {
        addTransferableBasketItemsToSeatList(transferableBasketItems);
    }

    // isSeatSelectionOnly signals whether the page is being loaded from seatSelection.aspx or VisualSeatSelection.aspx
    if (isSeatSelectionOnly == true) {
        //seating area is loaded
        createSeatSelection(document.getElementById('hdfStandAndAreaCode').defaultValue, undefined);
    }
    else {
        //stadium is loaded
        $(".ebiz-stand-availability").show();
        $(".ebiz-stand-pricing").hide();
        createStadiumCanvas();
    }

    if (isQuickBuy && singleArea.length > 0 && singleStand.length > 0) {
        defaultStandAndAreaClick = true;
        populateStandAndAreaDropDown(singleStand, singleArea, true);
    }
    else if (defaultToSeatSelection == true) {
        // default to seat selection forces the stand/area to be loaded by default on load rather than the stadium
        createSeatSelection(document.getElementById('hdfStandAndAreaCode').defaultValue, undefined);
    } else if (defaultToQuickBuy == true) {
        populateStandAndAreaDropDown(singleStand, singleArea, false);
    }

    $(".slider").addClass("disabled");
}

function isStandQuickBuyOnly(standAndAreaCode) {
    var stadiumAreas = document.getElementById('hdfStadiumXML').defaultValue;
    var areasXMLDoc;
    var returnCode = false;
    if (window.DOMParser) {
        parser = new DOMParser();
        areasXMLDoc = parser.parseFromString(stadiumAreas, "text/xml");
    }
    else // Internet Explorer
    {
        areasXMLDoc = new ActiveXObject("Microsoft.XMLDOM");
        areasXMLDoc.async = "false";
        areasXMLDoc.loadXML(stadiumAreas);
    }
    $(areasXMLDoc).find('A').each(function () {
        var $entry = $(this);
        var areaType = $entry.attr('t');

        if ($entry.attr('id') == standAndAreaCode) {
            if ($entry.attr('ss') == 'N') {
                returnCode = true;
            }
            else {
                returnCode = false;
            }
        }
    });
    return returnCode;
}

// Retrieves and draws stadium
function createStadiumCanvas() {
    stadiumCanvas = Raphael('stadium-canvas', stadiumCanvasWidth, stadiumCanvasHeight);
    var stadiumAreas = document.getElementById('hdfStadiumXML').defaultValue;
    $("#loading-image").show();
    var areasXMLDoc;
    if (window.DOMParser) {
        parser = new DOMParser();
        areasXMLDoc = parser.parseFromString(stadiumAreas, "text/xml");
    }
    else // Internet Explorer
    {
        areasXMLDoc = new ActiveXObject("Microsoft.XMLDOM");
        areasXMLDoc.async = "false";
        areasXMLDoc.loadXML(stadiumAreas);
    }
    // retrieve general seating colours
    $(areasXMLDoc).find('AIn').each(function () {
        var $entry = $(this);
        hoverColour = $entry.attr('hvc');
        selectedColour = $entry.attr('sel');
        selectedTextColour = $entry.attr('selt');
    })

    //loop through .net generated xml to populate page with elements
    $(areasXMLDoc).find('A').each(function () {
        var $entry = $(this);
        var areaType = $entry.attr('t');
        var shapeFillColour;
        // sets stadium view colour for individual areas based on isAvailibilityView toggle
        if (isAvailibilityView)
            shapeFillColour = $entry.attr('afl');
        else
            shapeFillColour = $entry.attr('pfl');

        if (shapeFillColour == undefined)
            shapeFillColour = $entry.attr('fl');

        switch (areaType) {
            case 'rect':
                var rectDesc;
                // Retrieve CData area description when id exists
                if ($entry.attr('id'))
                    rectDesc = $entry[0].childNodes[0].data;
                // Draws rectangle
                var rectangle = CreateRect($entry.attr('id'), rectDesc, $entry.attr('x'), $entry.attr('y'),
                                        $entry.attr('w'), $entry.attr('h'), shapeFillColour, $entry.attr('str'),
                                        $entry.attr('sml'), $entry.attr('ss'), $entry.attr('flr'),
                                        $entry.attr('clr'), $entry.attr('a'), $entry.attr('sw'), $entry.attr('tr'), $entry.attr('ap'),
                                        $entry.attr('txf'), $entry.attr('isp'), $entry.attr('xsp'));
                // If the rectangle has an id, try to find if there is a text element with the same ID contained in the XML
                if ($entry.attr('id')) {
                    $(areasXMLDoc).find('A[fid|="' + $entry.attr('id') + '"]:first').each(function () {
                        var $textEntry = $(this);
                        var rectAreaDesc = $textEntry[0].childNodes[0].data;
                        // Draw text with associated stand for mouse-over events
                        CreateText($textEntry.attr('fl'), $textEntry.attr('tr'), $textEntry.attr('ff'),
					                $textEntry.attr('font'), $textEntry.attr('fs'), $textEntry.attr('tx'), rectAreaDesc,
					                rectangle, shapeFillColour, $entry.attr('ss'), $entry.attr('a'));
                    });
                    // Add the available stand, area code to an array
                    if ($entry.attr('ap')) {
                        var tempString = $entry.attr('id').split("-");
                        var standCode = tempString[0];
                        var areaCode = tempString[1];
                        if ($entry.attr('ap') != "0") {
                            availableStandCodes.push(standCode);
                            availableAreaCodes.push(areaCode);
                        }
                    }
                }
                break;
            case 'polygon':
                var polyDesc;
                // Retrieve innerXML area description when id exists
                if ($entry.attr('id'))
                    polyDesc = $entry[0].childNodes[0].data;
                // Draws polygon
                var polygon = CreatePoly($entry.attr('id'), polyDesc, $entry.attr('p'), shapeFillColour,
                                        $entry.attr('str'), $entry.attr('sml'), $entry.attr('ss'),
                                        $entry.attr('flr'), $entry.attr('clr'), $entry.attr('a'), $entry.attr('sw'),
                                        $entry.attr('tr'), $entry.attr('ap'), $entry.attr('txf'), $entry.attr('isp'), $entry.attr('xsp'));

                // If the polygon has an id, try to find if there is a text element with the same ID contained in the XML
                if ($entry.attr('id')) {
                    $(areasXMLDoc).find('A[fid|="' + $entry.attr('id') + '"]:first').each(function () {
                        var $textEntry = $(this);
                        var polyAreaDesc = $textEntry[0].childNodes[0].data;

                        // Draw text with associated stand for mouse-over events
                        CreateText($textEntry.attr('fl'), $textEntry.attr('tr'), $textEntry.attr('ff'),
				                    $textEntry.attr('font'), $textEntry.attr('fs'), $textEntry.attr('tx'), polyAreaDesc,
				                    polygon, shapeFillColour, $entry.attr('ss'), $entry.attr('a'));
                    });
                    // Add the available stand, area code to an array
                    if ($entry.attr('ap')) {
                        var tempString = $entry.attr('id').split("-");
                        var standCode = tempString[0];
                        var areaCode = tempString[1];
                        if ($entry.attr('ap') != "0") {
                            availableStandCodes.push(standCode);
                            availableAreaCodes.push(areaCode);
                        }
                    }
                }
                break;
            case 'path':
                var pathDesc;
                // Retrieve innerXML area description when id exists
                if ($entry.attr('id'))
                    pathDesc = $entry[0].childNodes[0].data;
                // Draws path
                var path = CreatePath($entry.attr('id'), pathDesc, $entry.attr('d'), shapeFillColour,
                                $entry.attr('str'), $entry.attr('sml'), $entry.attr('ss'),
                                $entry.attr('flr'), $entry.attr('clr'), $entry.attr('a'), $entry.attr('sw'),
                                $entry.attr('tr'), $entry.attr('ap'), $entry.attr('txf'), $entry.attr('isp'), $entry.attr('xsp'));

                // If the path has an id, try to find if there is a text element with the same ID contained in the XML
                if ($entry.attr('id')) {
                    $(areasXMLDoc).find('A[fid|="' + $entry.attr('id') + '"]:first').each(function () {
                        var $textEntry = $(this);
                        var pathAreaDesc = $textEntry[0].childNodes[0].data;

                        // Draw text with associated stand for mouse-over events
                        CreateText($textEntry.attr('fl'), $textEntry.attr('tr'), $textEntry.attr('ff'),
					                $textEntry.attr('font'), $textEntry.attr('fs'), $textEntry.attr('tx'), pathAreaDesc,
					                path, shapeFillColour, $entry.attr('ss'), $entry.attr('a'));
                    });
                    // Add the available stand, area code to an array
                    if ($entry.attr('ap')) {
                        var tempString = $entry.attr('id').split("-");
                        var standCode = tempString[0];
                        var areaCode = tempString[1];
                        if ($entry.attr('ap') != "0") {
                            availableStandCodes.push(standCode);
                            availableAreaCodes.push(areaCode);
                        }
                    }
                }
                break;
            case 'circle':
                CreateCircle($entry.attr('cx'), $entry.attr('cy'), $entry.attr('r'), $entry.attr('fl'),
                         $entry.attr('str'), $entry.attr('sml'), $entry.attr('sw'));
                break;
            case 'line':
                CreateLine($entry.attr('x1'), $entry.attr('y1'), $entry.attr('x2'), $entry.attr('y2'),
                        $entry.attr('fl'), $entry.attr('str'), $entry.attr('sml'), $entry.attr('sw'));
                break;
            case 'text':
                // Draws text when fid doesn't exist or when text is not associated with a shape
                if ($entry.attr('fid') == undefined) {
                    var areaDesc;
                    if ($entry.attr('id'))
                        areaDesc = $entry[0].childNodes[0].data;

                    CreateText($entry.attr('fl'), $entry.attr('tr'), $entry.attr('ff'),
                            $entry.attr('font'), $entry.attr('fs'), $entry.attr('tx'), areaDesc, undefined, undefined, undefined);
                }

                break;
            case 'polyline':
                var polylineDesc;
                // Retrieve innerXML area description when id exists
                if ($entry.attr('id'))
                    polylineDesc = $entry[0].childNodes[0].data;
                // Draws polygonline
                var polyline = CreatePolyline($entry.attr('id'), polyDesc, $entry.attr('p'), shapeFillColour,
                                $entry.attr('str'), $entry.attr('sml'), $entry.attr('ss'),
                                $entry.attr('flr'), $entry.attr('clr'), $entry.attr('a'), $entry.attr('sw'), $entry.attr('tr'), $entry.attr('ap'), $entry.attr('txf'),
                                $entry.attr('isp'), $entry.attr('xsp'));

                // If the polygonline has an id, try to find if there is a text element with the same ID contained in the XML
                if ($entry.attr('id')) {
                    $(areasXMLDoc).find('A[fid|="' + $entry.attr('id') + '"]:first').each(function () {
                        var $textEntry = $(this);
                        var polylineAreaDesc = $textEntry[0].childNodes[0].data;

                        // Draw text with associated stand for mouse-over events
                        CreateText($textEntry.attr('fl'), $textEntry.attr('tr'), $textEntry.attr('ff'),
				            $textEntry.attr('font'), $textEntry.attr('fs'), $textEntry.attr('tx'), polylineAreaDesc,
				            polyline, shapeFillColour, $entry.attr('ss'), $entry.attr('a'));

                    });
                    // Add the available stand, area code to an array
                    if ($entry.attr('ap')) {
                        var tempString = $entry.attr('id').split("-");
                        var standCode = tempString[0];
                        var areaCode = tempString[1];
                        if ($entry.attr('ap') != "0") {
                            availableStandCodes.push(standCode)
                            availableAreaCodes.push(areaCode)
                        }
                    }
                }
                break;
        }
    });
    $("#loading-image").hide();
}

// Tool tip mouse over event responsible for displaying text
$(function () {
    $('#stadium-canvas').mousemove(function (e) {
        if (isStadiumExpaned === true) {
            if (over) {
                $("#stand-tip").addClass('ebiz-stand-tip-position');
                $("#stand-tip #stand .ebiz-data").text(standTipStandDesc);
                $("#stand-tip #area .ebiz-data").text(standTipAreaDesc);
                $("#stand-tip #stand-tip-availibility .ebiz-data").text(standTipAvailibilityDesc);
                $("#stand-tip #stand-tip-price1 .ebiz-label").text("");
                $("#stand-tip #stand-tip-price1 .ebiz-data").text("");
                $("#stand-tip #stand-tip-price .ebiz-label").text("");
                $("#stand-tip #stand-tip-price .ebiz-data").text("");

                var tempString;
                if (standPriceDesc.length > 0) {
                    if (standPriceDesc[1]) {
                        tempString = standPriceDesc[1].split("@");
                        $("#stand-tip #stand-tip-price1 .ebiz-label").text(tempString[0]);
                        $("#stand-tip #stand-tip-price1 .ebiz-data").text(tempString[1]);
                    }
                    if (standPriceDesc[0]) {
                        tempString = standPriceDesc[0].split("@");
                        $("#stand-tip #stand-tip-price .ebiz-label").text(tempString[0]);
                        $("#stand-tip #stand-tip-price .ebiz-data").text(tempString[1]);
                    }
                } else {

                }
                return true;
            }
        }
    });
});

var mArea;
// Draw rectangle, create events and related handling
function CreateRect(standAndAreaCode, desc, x, y, width, height, rectFill, rectStroke, strokeMiterlimit, isSeatSelection, rectFillRule, rectClipFill, availability, rectStrokeWidth, rectTransform, availabilityPercentage, rectStandAndAreaTicketExchangeFlag, rectTEMinSliderPrice, rectTEMaxSliderPrice) {
    var area;
    // Rectangles without a standAndAreaCode are considered not to be stands
    if (standAndAreaCode) {
        //  stand with attached events
        if (!rectStrokeWidth) {
            rectStrokeWidth = 1;
        }
        area = stadiumCanvas.rect(x, y, width, height); area.attr({ x: x, y: y, fill: rectFill, 'fill-rule': rectFillRule, 'clip-rule': rectClipFill, 'stroke-width': rectStrokeWidth }).data('id', standAndAreaCode).data('originalFill', rectFill).data('originalStroke', rectStroke).data('isTicketExchangeFlag', rectStandAndAreaTicketExchangeFlag).data('TEMinSliderPrice', rectTEMinSliderPrice).data('TEMaxSliderPrice', rectTEMaxSliderPrice);
        area.transform(rectTransform);

        AddEventsToShapes(area, availability, hoverColour, rectFill, availabilityPercentage, standAndAreaCode, isSeatSelection);

    } else {
        // shape not a stand and with no events
        area = stadiumCanvas.rect(x, y, width, height);
        area.attr({ x: x, y: y, fill: rectFill, 'stroke-miterlimit': strokeMiterlimit, stroke: rectStroke, 'fill-rule': rectFillRule, 'clip-rule': rectClipFill, 'stroke-width': rectStrokeWidth, 'transform': rectTransform });
        area.transform(rectTransform);
    }

    // add description to tool tip if description exists
    if (desc)
        addStandTip(area.node, desc);
    return area;
}


// Draw polygon, create events and related handling
function CreatePoly(standAndAreaCode, desc, path, polyFill, polyStroke, strokeMiterlimit, isSeatSelection, polyFillRule, polyClipRule, availability, polyStrokeWidth, polyTransform, availabilityPercentage, polyStandAndAreaTicketExchangeFlag, polyTEMinSliderPrice, polyTEMaxSliderPrice) {
    var area;
    // Polygons without a code are considered not to be stands
    if (standAndAreaCode) {
        //stand with attached events
        if (!polyStrokeWidth) {
            polyStrokeWidth = 1;
        }
        area = stadiumCanvas.path("M " + path + " z"); area.attr({ fill: polyFill, 'fill-rule': polyFillRule, 'clip-rule': polyClipRule, 'stroke-width': polyStrokeWidth, 'transform': polyTransform }).data('id', standAndAreaCode).data('originalFill', polyFill).data('originalStroke', polyStroke).data('isTicketExchangeFlag', polyStandAndAreaTicketExchangeFlag).data('TEMinSliderPrice', polyTEMinSliderPrice).data('TEMaxSliderPrice', polyTEMaxSliderPrice);
        area.transform(polyTransform);

        AddEventsToShapes(area, availability, hoverColour, polyFill, availabilityPercentage, standAndAreaCode, isSeatSelection);

    } else {
        // not a stand with no events
        area = stadiumCanvas.path("M " + path + " z"); area.attr({ fill: polyFill, stroke: polyStroke, 'stroke-miterlimit': strokeMiterlimit, 'fill-rule': polyFillRule, 'clip-rule': polyClipRule, 'stroke-width': polyStrokeWidth, 'transform': polyTransform });
        area.transform(polyTransform);
    }

    // add description to tool tip if description exists
    if (desc)
        addStandTip(area.node, desc);

    return area;
}


// Draw path, create events and related handling
function CreatePath(standAndAreaCode, desc, path, pathFill, pathStroke, strokeMiterlimit, isSeatSelection, pathFillRule, pathClipRule, availability, pathStrokeWidth, pathTransform, availabilityPercentage, pathStandAndAreaTicketExchangeFlag, pathTEMinSliderPrice, pathTEMaxSliderPrice) {
    var area;
    // Paths without a code are considered not to be stands
    if (standAndAreaCode) {
        //stand with attached events
        if (!pathStrokeWidth) {
            pathStrokeWidth = 1;
        }
        area = stadiumCanvas.path(path); area.attr({ fill: pathFill, 'fill-rule': pathFillRule, 'clip-rule': pathClipRule, 'stroke-width': pathStrokeWidth }).data('id', standAndAreaCode).data('originalFill', pathFill).data('originalStroke', pathStroke).data('isTicketExchangeFlag', pathStandAndAreaTicketExchangeFlag).data('TEMinSliderPrice', pathTEMinSliderPrice).data('TEMaxSliderPrice', pathTEMaxSliderPrice);
        area.transform(pathTransform);


        AddEventsToShapes(area, availability, hoverColour, pathFill, availabilityPercentage, standAndAreaCode, isSeatSelection);

    } else {
        // not a stand with no events
        area = stadiumCanvas.path(path); area.attr({ fill: pathFill, stroke: pathStroke, 'stroke-miterlimit': strokeMiterlimit, 'fill-rule': pathFillRule, 'clip-rule': pathClipRule, 'stroke-width': pathStrokeWidth, 'transform': pathTransform });
        area.transform(pathTransform);
    }

    // add description to tool tip if description exists
    if (desc)
        addStandTip(area.node, desc);
    return area;
}


function CreatePolyline(standAndAreaCode, desc, path, polylineFill, polylineStroke, strokeMiterlimit, isSeatSelection, polylineFillRule, polylineClipRule, availability, polylineStrokeWidth, polylineTransform, availabilityPercentage, polylineStandAndAreaTicketExchangeFlag, polylineTEMinSliderPrice, polylineTEMaxSliderPrice) {
    var area;
    // Polyglines without a code are considered not to be stands
    if (standAndAreaCode) {
        //stand with attached events
        if (!polylineStrokeWidth) {
            polyStrokeWidth = 1;
        }
        area = stadiumCanvas.path("M " + path); area.attr({ fill: polylineFill, 'fill-rule': polylineFillRule, 'clip-rule': polylineClipRule, 'stroke-width': polylineStrokeWidth }).data('id', standAndAreaCode).data('originalFill', polylineFill).data('originalStroke', polylineStroke).data('isTicketExchangeFlag', polylineStandAndAreaTicketExchangeFlag).data('TEMinSliderPrice', polylineTEMinSliderPrice).data('TEMaxSliderPrice', polylineTEMaxSliderPrice);
        area.transform(polylineTransform);

        AddEventsToShapes(area, availability, hoverColour, polylineFill, availabilityPercentage, standAndAreaCode, isSeatSelection);

    } else {
        // not a stand with no events
        area = stadiumCanvas.path("M " + path); area.transform(polylineTransform);
        area.attr({ fill: polylineFill, stroke: polylineStroke, 'stroke-miterlimit': strokeMiterlimit, 'fill-rule': polylineFillRule, 'clip-rule': polylineClipRule, 'stroke-width': polylineStrokeWidth, 'transform': polylineTransform });
    }

    // add description to tool tip if description exists
    if (desc)
        addStandTip(area.node, desc);
    return area;
}


// Adds events to shapes8
function AddEventsToShapes(area, availability, hoverColour, fillColour, availabilityPercentage, standAndAreaCode, isSeatSelection) {

    $(area.node).on('mouseover', function () {
        area.node.style.cursor = 'pointer';
        if (isStadiumExpaned === true) {
            if (availability != "na") {
                area.attr({ fill: hoverColour });
            }
        }
    });

    $(area.node).on('mouseout', function () {
        if (isStadiumExpaned === true) {
            if (availability != "na") {
                area.attr({ fill: fillColour });
            }
        }
    });

    $(area.node).on('touchend click', function () {
        if ((availability == "na" && isAgent) || availability != "na") {
            selectedStandAreaTEAllowed = (area.data("isTicketExchangeFlag") == 'true');
            selectedStandAreaTEMinPrice = area.data("TEMinSliderPrice");
            selectedStandAreaTEMaxPrice = area.data("TEMaxSliderPrice");
            standPassedOrphanBenchmark = overOrphanSeatBenchmark(availabilityPercentage);
            ClickStand(fillColour, standAndAreaCode, area, isSeatSelection);
            if (defaultToQuickBuy) {
                splitSelectedStandAndAreaObject(selectedStandAndArea, standAndAreaCode);
                populateStandAndAreaDropDown(selectedStandAndArea.stand, selectedStandAndArea.area, true);
                $("#txtQuantity").focus();
            }
            
        }
    });
}


// Draw circle, edit its attributes
function CreateCircle(cx, cy, r, circleFill, circleStroke, strokeMiterlimit) {
    var circle = stadiumCanvas.circle(cx, cy, r);
    circle.attr({ fill: circleFill, stroke: circleStroke, 'stroke-miterlimit': strokeMiterlimit });
}


// Draw line, edit its attributes
function CreateLine(x1, y1, x2, y2, lineFill, lineStroke, strokeMiterlimit) {
    var line = stadiumCanvas.path(["M", x1, y1, "L", x2, y2]);
    line.attr({ fill: lineFill, stroke: lineStroke, 'stroke-miterlimit': strokeMiterlimit });
}


// Draw Text, create text and shape set, edit it/their attributes and add events
function CreateText(textFill, textTransform, fontFamily, font, fontSize, textValue, tipDesc, area, shapeFill, isSeatSelection, availability) {
    // if the area parameter is undefined only draw the text
    // if the area parameter contains an area, attach text to area, create a set, and attach events to the set

    var text = stadiumCanvas.text(0, 0, textValue);
    text.attr({ fill: textFill, 'font-family': fontFamily, 'font-size': fontSize });
    text.transform(textTransform);
    text.attr({ 'text-anchor': 'start' });
    if (area) {

        $(text.node).on('mouseover', function () {
            if (isStadiumExpaned === true) {
                text.attr({ fill: "#FFFFFF" });
                text.node.style.cursor = 'pointer';
                area.attr({ fill: hoverColour });
                area.node.style.cursor = 'pointer';
            }
        });

        $(text.node).on('mouseout', function () {
            if (isStadiumExpaned === true) {
                text.attr({ fill: textFill });
                area.attr({ fill: shapeFill });
            }
        });

        area.data('originalFill', shapeFill).data('originalTextFill', textFill);
        $(text.node).on('touchend click', function () {
            if ((availability == "na" && isAgent) || availability != "na") {
                ClickStand(shapeFill, area.data("id"), area, isSeatSelection);
            }
        });
    }
    if (tipDesc)
        addStandTip(text.node, tipDesc);
}


//Check to see if the availability percentage is less that the current benchmark
function overOrphanSeatBenchmark(availabilityPercentage) {
    if (orphanBenchmark < availabilityPercentage)
        return true;
    return false;
}


// based on isAvailibility toggle change the stadium view by resetting isAvailibilityView and redrawing stadium with changes
function changeStandView() {
    $("#loading-image").show();
    if (isAvailibilityView) {
        // availibility mode change to price view mode
        $(".ebiz-stand-availability").hide();
        $(".ebiz-stand-pricing").show();
        $("#btnChangeStandView").removeClass("ebiz-view-price");
        $("#btnChangeStandView").addClass("ebiz-view-availability");
        if (stadiumCanvas)
            stadiumCanvas.remove();
        isAvailibilityView = false;
        createStadiumCanvas();
        document.getElementById("btnChangeStandView").value = standAvailabilityText;
        $("#loading-image").hide();
    } else {
        // price view mode change to availibility mode
        $(".ebiz-stand-availability").show();
        $(".ebiz-stand-pricing").hide();
        $("#btnChangeStandView").removeClass("ebiz-view-availability");
        $("#btnChangeStandView").addClass("ebiz-view-price");
        if (stadiumCanvas)
            stadiumCanvas.remove();
        isAvailibilityView = true;
        createStadiumCanvas();
        document.getElementById("btnChangeStandView").value = standPriceText;
        $("#loading-image").hide();
    }
}


// toggles between seat selection and quick buy on all stands based on isQuickBuy
function ClickStand(colour, standAndAreaCode, standArea, isSeatSelection) {
    GetSelectedValues();
    GetDynamicOptionValuesAndText();
    // cross stand ophan seat validation
    splitSelectedStandAndAreaObject(selectedStandAndArea, standAndAreaCode);
    
    // Set the visibility of the ticket exchange sliders if we have a TE active area.
    resetTicketExchangePanel()
    
   
    var quickBuyCreated = false;
    var orphanValidation = true;
    if (isStadiumExpaned === false) {
        // javascript orphan seat validation
        if (orphanSeatFlag == true && standPassedOrphanBenchmark == true && !isQuickBuy) {
            seatValidationResult = {
                leftSeat1Status: true,
                leftSeat2Status: true,
                rightSeat1Status: true,
                rightSeat2Status: true,
                message: ""
            };
            var i = 0;
            orphanValidation = true;
            while (i < basket.length) {
                var tempScode = basket[i].split("/");
                $(seatingAreaXML).find('s[n|="' + tempScode[2] + '"][r|="' + tempScode[1] + '"]:first').each(function () {
                    var $seat = $(this);
                    var resultObj = validate(seatingAreaXML, $seat);
                    if (resultObj == false) {
                        orphanValidation = false;
                    }
                });
                if (orphanValidation == false) {
                    break;
                }
                i++;
            }
        }
    }
    // if no orphan seat error continue...
    if (orphanValidation) {
        if (!isQuickBuy) {
            // toggles between seat selection and quick buy on individuals stands based on DB setting isSeatSelection
            if (isSeatSelection == "Y" || isSeatSelection == "")
                createSeatSelection(standAndAreaCode, undefined);
            else
                createQuickBuy(standAndAreaCode);
        } else if (defaultToQuickBuy) {
            createQuickBuy(standAndAreaCode);
        } else {
            createQuickBuy(standAndAreaCode);
        }

        // re-colour area based on original fill and stroke
        standAreaId = standArea.id;
        if (standAreaId != undefined) {
            var standAreaLast = stadiumCanvas.getById(standAreaId);
            var originalreafill = standAreaLast.data("originalFill");
            standAreaLast.attr({ fill: originalreafill });
        }
        return true;
    } else {
        alert(orphanedSeatErrorText);
        return false;
    }
}


// Tool tip mouse over event responsible for showing/hiding the the tool tip and setting the text
function addStandTip(nodeArea, txt) {
    if (isStadiumExpaned === true) {
        var tempString = txt.split(";");
        $(nodeArea).on('mouseover', function () {
            if (isStadiumExpaned === true) {
                standTipStandDesc = tempString[0];
                standTipAreaDesc = tempString[1];
                standTipAvailibilityDesc = tempString[2];
                standTipPriceDesc = tempString[3];
                var tempJ = tempString.length - 3;
                standPriceDesc = [];
                for (j = 0; j < tempJ; j++) {
                    standPriceDesc[j] = tempString[3 + j];
                }
                $("#stand-tip").show(0);
                over = true;
            }
        });

        $(nodeArea).on('mouseout', function () {
            $("#stand-tip").hide(0);
            over = false;
        });
    }
}


// Restores "Stand and Area(Stadium) view
function backToStadium() {
    if (!isQuickBuy) {

        //orphan seat check
        if (basket.length > 0) {
            var orphanValidation = true;
            if (orphanSeatFlag == true && standPassedOrphanBenchmark == true) {
                seatValidationResult = {
                    leftSeat1Status: true,
                    leftSeat2Status: true,
                    rightSeat1Status: true,
                    rightSeat2Status: true,
                    message: ""
                };
                var i = 0;
                orphanValidation = true;
                while (i < basket.length) {
                    var tempScode = basket[i].SeatInfo.split("/");
                    $(seatingAreaXML).find('s[n|="' + tempScode[2] + '"][r|="' + tempScode[1] + '"]:first').each(function () {
                        var $seat = $(this);
                        var resultObj = validate(seatingAreaXML, $seat);
                        if (resultObj == false) {
                            orphanValidation = false;
                        }
                    });
                    if (orphanValidation == false) {
                        break;
                    }
                    i++;
                }
            }
            if (orphanValidation) {
                // return to orginal size
                stadiumExpand();
                SetDynamicOptionValuesAndText();
                return true;
            } else {
                alert(orphanedSeatErrorText);
                return false;
            }
        } else {
            // return to orginal size
            stadiumExpand();
            SetDynamicOptionValuesAndText();
        }
    }
    // redraw stadium going back to stadium view
    if (!isQuickBuy){
        ReDrawStadium(false, false, false, false, false, false);
    }
    // re-colour area based on original fill and stroke
    if (standAreaId != undefined) {
        var standArea = stadiumCanvas.getById(standAreaId);
        var originalreafill = standArea.data("originalFill");
        standArea.attr({ fill: originalreafill });
        standAreaId = undefined;
    }
    $(".ebiz-priceband").each(function () { $(this).show(); });
}


//Expand the stadium SVG graphic
function stadiumExpand() {
    if (!isStadiumExpaned) {
        isStadiumExpaned = true;
        stadiumCanvas.setSize(stadiumCanvasWidth, stadiumCanvasHeight);
        $("#pitchStands").show();
        $("#select").hide();
        $(".ebiz-teams-wrapper").show();
        $(".ebiz-match-text").show();
        $(".ebiz-zoom-options").hide();
        $("#backButton").hide();
        $(".ebiz-seating-key").hide();
        $("#btnViewFromArea").hide();
        $("#btnChangeStandView").show();
        $("#navigation-options").hide();
        $(".ebiz-seat-details").hide();
        $(".c-price-band-best-available").show();
        $(".c-ticket-exchange").show();
        $(".c-price-band-concession").show();
        $(".ebiz-restriction-details").hide();
        $("#btnResetSeating").hide();
        if (isAgent) {
            $("#btnMultiSelect").hide();
            $("#btnRowSeatCodes").hide();
        }
        $("#display").hide();
        $(".ebiz-svg-container").addClass("ebiz-area-view");
        $(".ebiz-svg-container").removeClass("ebiz-seat-view");
        $(".ebiz-slider-wrap").hide();
        $("#svg-controls").removeClass("medium-3 columns");
        $("#svg-controls").addClass("large-12 columns");
        $("#svg-wrapper").removeClass("medium-9 columns");
        $("#svg-wrapper").hide();
        if (seatingAreaCanvas)
            seatingAreaCanvas.remove();
        if (miniSeatingAreaCanvas)
            miniSeatingAreaCanvas.remove();
        set.clear();
        set = null;
        seatingAreaCanvas = null;
        if (miniSeatingAreaCanvas)
            miniSeatingAreaCanvas = null;
        if (isAvailibilityView) {
            $(".ebiz-stand-pricing").hide();
            $(".ebiz-stand-availability").show();
        }
        else {
            $(".ebiz-stand-availability").hide();
            $(".ebiz-stand-pricing").show();
        }
        $("#ticketing3DSeatView").hide();
    }
}


//Reset the stadium graphic and drop down lists
function ResetStadium() {
    if (!isStadiumExpaned) {
        stadiumExpand();
    }
    ReDrawStadium(false, false, false, false, false, true);
}


//Re-Draw the stadium canvas based on price break ID. The customer has selected a different price break and the stadium XML must be recreated
function ReDrawStadium(selectingPriceBreakId, selectingMinPrice, selectingMaxPrice, selectingStand, selectingArea, selectingReset) {
    var productType = document.getElementById('hdfProductType').defaultValue;
    var catMode = document.getElementById('hdfCATMode').defaultValue;
    var standAndAreaAPIOptions = GetAPISettingsFromStandAndAreaOptions(selectingPriceBreakId, selectingMinPrice, selectingMaxPrice, selectingStand, selectingArea, false, selectingReset);

    if (standAndAreaAPIOptions.IncludeTicketExchangeSeats)
    {
        $(".slider").removeClass("disabled");
    } else {
        $(".slider").addClass("disabled");
    }
    $.ajax({
        type: "POST",
        url: "VisualSeatSelection.aspx/GetStadiumXml",
        cache: false,
        data: '{data: "' + standAndAreaAPIOptions.SelectedPriceBreak + '","productCode":"' + productCode + '","stadiumCode":"' + stadiumCode + '","productType":"' + productType + '","campaignCode":"' + campaignCode + '","catMode":"' + catMode + '","callId":"' + callId + '","includeTicketExchangeSeats":"' + standAndAreaAPIOptions.IncludeTicketExchangeSeats + '","minimumPrice":"' + standAndAreaAPIOptions.SelectedMinimumPrice + '","maximumPrice":"' + standAndAreaAPIOptions.SelectedMaximumPrice + '","selectedStand":"' + standAndAreaAPIOptions.SelectedStand + '","selectedArea":"' + standAndAreaAPIOptions.SelectedArea + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var returnValue = msg.d;
            var viewModel = JSON.parse(returnValue);
            if (returnValue != "error") {
                availableStandCodes = [];
                availableAreaCodes = [];
                document.getElementById('hdfStadiumXML').defaultValue = viewModel.StadiumXml;
                if (isStadiumExpaned) {
                    stadiumCanvas.remove();
                    createStadiumCanvas();
                }
                GetSelectedValues();
                SetupDynamicStandAndAreaOptions(viewModel, standAndAreaAPIOptions, selectingStand, selectingArea, selectingPriceBreakId, undefined, false, isStadiumExpaned);
                GetDynamicOptionValuesAndText();
            }
        },
        error: function (xhr, status) {
            alert(status + '  ' + xhr.responseText);
        }
    });
    if (!isStadiumExpaned) {
        createSeatSelection(stand, standAndAreaAPIOptions);
    }
}

//Split the concatenated stand and area code string into correct values in the stand and area object
function splitSelectedStandAndAreaObject(standAndAreaObj, standAndAreaCode) {
    var tempString = standAndAreaCode.split("-");
    standAndAreaObj.stand = tempString[0];
    standAndAreaObj.area = tempString[1];
}


//Populate the stand and area drop down list based on the selected stand
function populateStandAndAreaDropDown(singleStand, singleArea, changeArea) {
    if (isPriceAndAreaSelection) {
        $(".ebiz-combined-stand-area-drop-down").val(singleStand + "-" + singleArea);
    } else {
        $("#standDropDown").val(singleStand);
        $('#standDropDown').trigger('onchange');
        if (changeArea) {
            $("#areaDropDownList").val(singleArea);
            $('#areaDropDownList').trigger('onchange');
        }
    }
}