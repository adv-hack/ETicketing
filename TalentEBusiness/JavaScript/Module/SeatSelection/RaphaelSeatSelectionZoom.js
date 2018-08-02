var moveTest = 0;
var yPOSVIEW = 0;
var xPOSVIEW = 0;
var intervalId;
var zoomWidth = 0;
var zoomHeight = 0;
var seatingCanvasID = "#select";
var viewBoxHeight;
var viewBoxWidth;
var viewBox;
var oX;
var dX, dY;
var mousedown;
var startX, startY;
var vB;
var c;
var miniMat;
$.ajaxSetup({ cache: false });
function goReset() {
    var sliderValue = calcZoomFactor();
    moveTest = 0;
    yPOSVIEW = 0;
    xPOSVIEW = 0;
    zoomWidth = sliderValue;
    zoomHeight = sliderValue;
    seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight, false);
    if (multiSelectOn === true)
        mat.attr({ width: multiSelectRectWidth, height: multiSelectRectHeight, x: xPOSVIEW, y: yPOSVIEW });
    zoomScaleFactor = zoomHeight / originalZoom;
    miniMat.attr({ width: (400 * zoomScaleFactor), height: (300 * zoomScaleFactor), x: xPOSVIEW, y: yPOSVIEW });
    resetSlider(true);
    disableDirectionArrows();
}
var prevCanvasWidth;
var prevCanvasHeight;
var zoomScaleFactor;

function zoom(iValue, toZoom) {
    var sliderValue = iValue;
    if (multiSelectOn === true) {
        multiSelectOn = false;
        document.getElementById("btnMultiSelect").value = multiSelectOnText;
    }
    var xxx = document.getElementById("sliderVal");
    xxx.value = iValue;
    zoomWidth = sliderValue;
    zoomHeight = sliderValue;
    if (toZoom && (sliderValue < sliderMax - 200) && (sliderValue >= 200)) {
        seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, (zoomHeight), false);
        if (multiSelectOn === true)
            mat.attr({width: multiSelectRectWidth, height: multiSelectRectHeight });
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        miniMat.attr({ width: (400 * zoomScaleFactor), height: (300 * zoomScaleFactor) });
    }
}

function goDown() {
    if (multiSelectOn === false) {
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        yPOSVIEW = yPOSVIEW + (100 * maxZoomFactorY);
        disableDirectionArrows();
        seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight, false);

        miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });
    }
}
function goUp() {
    if (multiSelectOn === false) {
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        yPOSVIEW = yPOSVIEW - (100 * maxZoomFactorY);
        disableDirectionArrows();
        seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight, false);

        miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });
    }
}
function goLeft() {
    if (multiSelectOn === false) {
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        xPOSVIEW = xPOSVIEW - (100 * maxZoomFactorX);
        disableDirectionArrows();
        seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight, false);

        miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });
    }
}
function goRight() {
    if (multiSelectOn === false) {
        zoomScaleFactor = zoomHeight / prevCanvasHeight;
        xPOSVIEW = xPOSVIEW + (100 * maxZoomFactorX);
        disableDirectionArrows();
        seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight, false);

        miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });
    }
}
function goZoomIn() {
    var sliderVal = zoomWidth + 100;
    zoom(sliderVal, true);
    $('#zoomSlider').slider('value', (sliderMax - sliderVal));

}
function goZoomOut() {
    var sliderVal = zoomWidth - 100;
    if (sliderVal < sliderMax) {
        zoom(sliderVal, true);
        $('#zoomSlider').slider('value', (sliderMax - sliderVal));
    }
}

$(function () {
    $(document).scroll(function (e) {
        if ($(this).scrollLeft() > 1) {
            e.preventDefault();
            $(this).scrollLeft(0);
        }
    });
});

var sliderMax = 2500;

$(function () {
    $("#zoomSlider").slider({
        orientation: "horizontal",
        range: "max",
        min: 200,
        max: sliderMax - 200,
        value: (sliderMax / 2),
        stop: function (event, ui) {
            zoom(sliderMax - ui.value, true);
        }
    });
});



$(function () {
    if (("ontouchstart" in document.documentElement)) {
        var element = document.getElementById("zoomSlider");
        Hammer(element, {
            transform_always_block: true,
            drag_block_horizontal: true,
            drag_block_vertical: true,
            drag_min_distance: 0
        });

        var hammertime = Hammer(element).on('drag dragend', function (ev) {
            ev.preventDefault();
            switch (ev.type) {
                case 'drag':
                    if (ev.gesture.direction === "left") {
                        zoomWidth *= 0.97;
                        zoomHeight *= 0.97;
                    }
                    else if (ev.gesture.direction === "right") {
                        zoomWidth *= 1.03;
                        zoomHeight *= 1.03;
                    }
                    $('#zoomSlider').slider({ value: (zoomHeight) });
                    zoomScaleFactor = zoomHeight / prevCanvasHeight;
                    miniMat.attr({ width: (400 * zoomScaleFactor), height: (300 * zoomScaleFactor), x: xPOSVIEW, y: yPOSVIEW });
                    if (multiSelectOn === true)
                        mat.attr({ width: multiSelectRectWidth, height: multiSelectRectHeight, x: xPOSVIEW, y: yPOSVIEW });
                    break;
                case 'dragend':
                    seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, (zoomHeight), false);
                    break;
            }
        });
    }
});

$(function () {
    if (("ontouchstart" in document.documentElement) && (multiSelectOn === true)) {
        var element = document.getElementById("select");
        var sliderValue1 = calcZoomFactor();
        var hammertime = Hammer(element, {
            transform_always_block: true,
            transform_min_scale: 1,
            drag_block_horizontal: true,
            drag_block_vertical: true,
            drag_min_distance: 0
        });
        var hammertime = Hammer(element).on('transform dragstart drag dragend transformend transformstart', function (ev) {

            manageMultitouch(ev);
            $('#zoomSlider').slider('value', (sliderMax - sliderValue1));
        });
    }
});

var posX = 0;
var posY = 0;
var last_scale = 0;
var scale = 1;
var dragon = false;
var transbreak = true;
var transCount = 0;
var seatingAreaImage;
var gesturedImg;
var timerStart;
var timerEnd;

function manageMultitouch(ev) {//, gesturedImg) {
    ev.preventDefault();
    switch (ev.type) {
        case 'transformstart':
            timerStart = new Date().getTime();
            transCount = 0;
            break;
        case 'dragstart':
            if (seatingAreaCanvas.getElementByPoint(ev.gesture.deltaX, ev.gesture.deltaY) != null) { return; }
            if (multiSelectOn === true) { return; }
            dragon = true;
            startX = ev.gesture.deltaX;
            startY = ev.gesture.deltaY;

            break;
        case 'drag':
            if (dragon === false) { return; }

            dX = startX - ev.gesture.deltaX;
            dY = startY - ev.gesture.deltaY;

            var x = viewBoxWidth / seatingAreaCanvas.width;
            var y = viewBoxHeight / seatingAreaCanvas.height;

            dX *= x;
            dY *= y;
            xPOSVIEW = viewBox.X + dX;
            yPOSVIEW = viewBox.Y + dY;
            break;

        case 'transform':
            var zscale;
            if (ev.gesture.scale > 1) {
                zoomWidth *= 0.97;
                zoomHeight *= 0.97;
                zscale = 0.99;

            } else {
                zoomWidth *= 1.03;
                zoomHeight *= 1.03;
                zscale = 1.01;
            }
            break;

        case 'dragend':
            if (dragon === false) return;
            viewBox.X += dX;
            viewBox.Y += dY;
            if (isNaN(parseFloat(viewBox.X)) || isNaN(parseFloat(viewBox.Y))) {
                viewBox.X = xPOSVIEW;
                viewBox.Y = yPOSVIEW;
            }
            seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, (zoomHeight), false);
            zoomScaleFactor = zoomHeight / prevCanvasHeight;
            miniMat.attr({ width: (400 * zoomScaleFactor), height: (300 * zoomScaleFactor), x: xPOSVIEW, y: yPOSVIEW });
            
            dragon = false;
            break;
        case 'transformend':

            seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, (zoomHeight), false);
            zoomScaleFactor = zoomHeight / prevCanvasHeight;
            miniMat.attr({ width: (400 * zoomScaleFactor), height: (300 * zoomScaleFactor), x: xPOSVIEW, y: yPOSVIEW });
            if (multiSelectOn === true)
                mat.attr({ width: multiSelectRectWidth, height: multiSelectRectHeight, x: xPOSVIEW, y: yPOSVIEW });

            break;
    }
}

var maxMatWidth;
var maxMatHeight;
var originalZoom;
var callCount = 0;
function resetSlider(reset) {
    var sliderValue = calcZoomFactor();
    zoomWidth = sliderValue;
    zoomHeight = sliderValue;
    yPOSVIEW = 0;
    xPOSVIEW = 0;
    prevCanvasWidth = seatingAreaCanvas._viewBox[2];
    prevCanvasHeight = seatingAreaCanvas._viewBox[3];
    if (!reset && callCount == 0) {
        originalZoom = prevCanvasHeight;
        //Chrome and safari calls createSeatingArea twice
        callCount++;
    }
    seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, (zoomHeight), false);
    if (multiSelectOn === true)
        mat.attr({ width: multiSelectRectWidth, height: multiSelectRectHeight });
    maxMatWidth = parseFloat(document.getElementById("maxX").value);
    maxMatHeight = parseFloat(document.getElementById("maxY").value);
    $('#zoomSlider').slider('value', (sliderMax - sliderValue));
    zoomScaleFactor = zoomHeight / prevCanvasHeight;

    maxZoomFactorX = document.getElementById("maxX").value / prevCanvasHeight;
    maxZoomFactorY = document.getElementById("maxY").value / prevCanvasHeight;

}

var maxZoomFactorX;
var maxZoomFactorY;
var xFactor;
var yFactor;
function calcZoomFactor() {
    var reducePcent = 0;
    var increasePcent = 0;

    var maxX = document.getElementById("maxX");
    var maxY = document.getElementById("maxY");

    xFactor = seatingAreaCanvasWidth / maxX.value;
    yFactor = seatingAreaCanvasHeight / maxY.value;

    var sliderValue = sliderMax / 2;
    if (zoomOnLoad) {
        if (xFactor <= yFactor) {
            // Base on width
            var theFactor = maxX.value;
        } else {
            // Base on Height
            var theFactor = maxY.value;
        }
        var rowSeatFactor = 1;
        if (isRowSeatOnSVG) {
            rowSeatFactor = 0.5;
        }
        if (theFactor < 50) {
            sliderValue = 200 * rowSeatFactor;
        } else if (theFactor >= 51 && theFactor <= 100) {
            sliderValue = 250 * rowSeatFactor;
        } else if (theFactor >= 101 && theFactor <= 200) {
            sliderValue = 300 * rowSeatFactor;
        } else if (theFactor >= 201 && theFactor <= 300) {
            sliderValue = 350 * rowSeatFactor;
        } else if (theFactor >= 301 && theFactor <= 400) {
            sliderValue = 400 * rowSeatFactor;
        } else if (theFactor >= 401 && theFactor <= 500) {
            sliderValue = 450 * rowSeatFactor;
        } else if (theFactor >= 501 && theFactor <= 600) {
            sliderValue = 570 * rowSeatFactor;
        } else if (theFactor >= 601 && theFactor <= 700) {
            sliderValue = 600 * rowSeatFactor;
        } else if (theFactor >= 701 && theFactor <= 800) {
            sliderValue = 700 * rowSeatFactor;
        } else if (theFactor >= 801) {
            sliderValue = 900 * rowSeatFactor;
        } 
    }
    return sliderValue;
}

$(function () {
    $(seatingCanvasID).mousedown(function (e) {
        if (over) { return; }
        if (multiSelectOn == false) {
            mousedown = true;
            startX = e.pageX;
            startY = e.pageY;
            e.stopPropagation();
        }
    });

    $(seatingCanvasID).mousemove(function (e) {
        if (multiSelectOn == false) {
            if (mousedown == false) { return; }
            dX = (startX - e.pageX) * zoomScaleFactor;
            dY = (startY - e.pageY) * zoomScaleFactor;

            if (isNaN(parseFloat(viewBox.X)) || isNaN(parseFloat(viewBox.Y))) {
                viewBox.X = xPOSVIEW;
                viewBox.Y = yPOSVIEW;
            }
            xPOSVIEW = viewBox.X + dX;
            yPOSVIEW = viewBox.Y + dY;
            disableDirectionArrows();
            seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight);
            miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });

        }

    })

    $(document).mouseup(function (e) {
        if (multiSelectOn == false && isStadiumExpaned == false) {
            if (mousedown == false) return;
            viewBox.X += dX;
            viewBox.Y += dY;
            mousedown = false;
        }

    });
});

var ox = 0;
var oy = 0;

function minidragstart(x, y) {
}

function minidragmove(dx, dy, posx, posy) {
    var zoomWeightWidth = (maxMatWidth / 400);
    var zoomWeightHeight = (maxMatHeight / 300);
    var zoomWeight;
    if (maxMatWidth > maxMatHeight) {
        zoomWeight = zoomWeightWidth;
    } else {
        zoomWeight = zoomWeightHeight;
    }
    disableDirectionArrows();
    xPOSVIEW = xPOSVIEW + (((dx - ox) * zoomScaleFactor) * zoomWeight);
    yPOSVIEW = yPOSVIEW + (((dy - oy) * zoomScaleFactor) * zoomWeight);

    miniMat.attr({ x: xPOSVIEW, y: yPOSVIEW });

    ox = dx;
    oy = dy;
}

function minidragend(e) {
    seatingAreaCanvas.setViewBox(xPOSVIEW, yPOSVIEW, zoomWidth, zoomHeight);
    if (multiSelectOn === true)
        mat.attr({ x: xPOSVIEW, y: yPOSVIEW });
    ox = 0;
    oy = 0;
    viewBox.X = xPOSVIEW;
    viewBox.Y = yPOSVIEW;
}

function disableDirectionArrows() {
    if (xPOSVIEW < 0) {
        $(".button.ebiz-left").removeClass("ebiz-direction-enabled");
        $(".button.ebiz-left").addClass("ebiz-direction-disabled");
    } else {
        $(".button.ebiz-left").removeClass("ebiz-direction-disabled");
        $(".button.ebiz-left").addClass("ebiz-direction-enabled");
    }

    if (yPOSVIEW < 0) {
        $(".button.ebiz-up").removeClass("ebiz-direction-enabled");
        $(".button.ebiz-up").addClass("ebiz-direction-disabled");
    } else {
        $(".button.ebiz-up").removeClass("ebiz-direction-disabled");
        $(".button.ebiz-up").addClass("ebiz-direction-enabled");
    }

    if ((xPOSVIEW + seatingAreaCanvasWidth) > maxMatWidth) {
        $(".button.ebiz-right").removeClass("ebiz-direction-enabled");
        $(".button.ebiz-right").addClass("ebiz-direction-disabled");
    } else {
        $(".button.ebiz-right").removeClass("ebiz-direction-disabled");
        $(".button.ebiz-right").addClass("ebiz-direction-enabled");
    }
}