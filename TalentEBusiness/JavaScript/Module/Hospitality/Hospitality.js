//#################################################################################
//Hospitality Product/Package Selection Javascript code
//#################################################################################
var hospitality_buttonFilters = {};
var hospitality_buttonFilter = '.c-hosp-isotope__fixs';
var hospitality_rangeFilters = {};
var $hospitality_grid;
var $hospitality_groupSlider;
var $hospitality_budgetSlider;
var $hospitality_dateSlider
var $hospitality_anyButtons = $('.c-hosp-isotope__btn-fltr[data-filter=""]');
var $hospitality_buttons = $('.c-hosp-isotope__btn-fltr');

var hospitality_seasonProductCount = 0;
var hospitality_minDate = "";
var hospitality_maxDate = "";
var hospitality_minBudget = 0;
var hospitality_maxBudget = 0;
var hospitality_minGroupSize = 0;
var hospitality_maxGroupSize = 0;
var hospitality_minBudgetHome = 0;
var hospitality_maxBudgetHome = 0;
var hospitality_minGroupSizeHome = 0;
var hospitality_maxGroupSizeHome = 0;
var hospitality_minBudgetSeason = 0;
var hospitality_maxBudgetSeason = 0;
var hospitality_minGroupSizeSeason = 0;
var hospitality_maxGroupSizeSeason = 0;
var hospitality_currencySymbol = "";

//default filters
var hospitality_buttonFilter_type = "";
var hospitality_buttonFilter_subtype = "";
var hospitality_buttonFilter_competition = "";
var hospitality_buttonFilter_opposition = "";
var hospitality_defaultMinGroupSize = 0;
var hospitality_defaultMaxGroupSize = 0;
var hospitality_defaultMinBudget = 0;
var hospitality_defaultMaxBudget = 0;
var hospitality_defaultMinDate = 0;
var hospitality_defaultMaxDate = 0;




//Runs on load
$(function () {
    $(".c-hosp-isotope__no-results").hide();
    loadVariables();
    initaliseGrid();
    initaliseSliders();

    if (hospitality_buttonFilter_type == "pak") {
        $(".c-hosp-isotope__btn-fltr-pak").click();
    }
    else if (hospitality_buttonFilter_type == "fixs") {
        $(".c-hosp-isotope__btn-fltr-fixs").click();
    }
    else if (hospitality_buttonFilter_type == "pakSeason" || hospitality_buttonFilter_type == "fixsSeason") {
        if (hospitality_seasonProductCount == 1) {
            $(".c-hosp-isotope__btn-fltr-pakSeason").click();
        }
        if (hospitality_seasonProductCount > 1) {
            $(".c-hosp-isotope__btn-fltr-fixsSeason").click();
        }
    }
    else {
        //Default Filter as fixture

        $(".c-hosp-isotope__btn-fltr-fixs").click();
    }
    
});


//Load the hospitality local variables used when retrieving activities
function loadVariables() {
    if (document.getElementById("hdfSeasonProductCount") != null) { hospitality_seasonProductCount = parseInt(document.getElementById("hdfSeasonProductCount").value); }
    if (document.getElementById("hdfMinDate") != null) { hospitality_minDate = document.getElementById("hdfMinDate").value; }
    if (document.getElementById("hdfMaxDate") != null) { hospitality_maxDate = document.getElementById("hdfMaxDate").value; }
    if (document.getElementById("hdfMinBudgetHome") != null) { hospitality_minBudgetHome = parseInt(document.getElementById("hdfMinBudgetHome").value); }
    if (document.getElementById("hdfMaxBudgetHome") != null) { hospitality_maxBudgetHome = parseInt(document.getElementById("hdfMaxBudgetHome").value); }
    if (document.getElementById("hdfMinGroupSizeHome") != null) { hospitality_minGroupSizeHome = parseInt(document.getElementById("hdfMinGroupSizeHome").value); }
    if (document.getElementById("hdfMaxGroupSizeHome") != null) { hospitality_maxGroupSizeHome = parseInt(document.getElementById("hdfMaxGroupSizeHome").value); }
    if (document.getElementById("hdfMinBudgetSeason") != null) { hospitality_minBudgetSeason = parseInt(document.getElementById("hdfMinBudgetSeason").value); }
    if (document.getElementById("hdfMaxBudgetSeason") != null) { hospitality_maxBudgetSeason = parseInt(document.getElementById("hdfMaxBudgetSeason").value); }
    if (document.getElementById("hdfMinGroupSizeSeason") != null) { hospitality_minGroupSizeSeason = parseInt(document.getElementById("hdfMinGroupSizeSeason").value); }
    if (document.getElementById("hdfMaxGroupSizeSeason") != null) { hospitality_maxGroupSizeSeason = parseInt(document.getElementById("hdfMaxGroupSizeSeason").value); }
    if (document.getElementById('hdfCurrencySymbol')) { hospitality_currencySymbol = document.getElementById('hdfCurrencySymbol').value; }
    if (document.getElementById('hdfFilterGroupType') != null) { hospitality_buttonFilter_type = document.getElementById('hdfFilterGroupType').value; }
    if (document.getElementById('hdfFilterGroupsubType') != null) { hospitality_buttonFilter_subtype = document.getElementById('hdfFilterGroupsubType').value; }
    if (document.getElementById('hdfFilterGroupCompetition') != null) { hospitality_buttonFilter_competition = document.getElementById('hdfFilterGroupCompetition').value; }
    if (document.getElementById('hdfFilterGroupOpposition') != null) { hospitality_buttonFilter_opposition = document.getElementById('hdfFilterGroupOpposition').value; }
    if (document.getElementById('hdfRangeGroupMin') != null) { hospitality_defaultMinGroupSize = document.getElementById('hdfRangeGroupMin').value; }
    if (document.getElementById('hdfRangeGroupMax') != null) { hospitality_defaultMaxGroupSize = document.getElementById('hdfRangeGroupMax').value; }
    if (document.getElementById('hdfRangeBudgetMin') != null) { hospitality_defaultMinBudget = document.getElementById('hdfRangeBudgetMin').value; }
    if (document.getElementById('hdfRangeBudgetMax') != null) { hospitality_defaultMaxBudget = document.getElementById('hdfRangeBudgetMax').value; }
    if (document.getElementById('hdfRangeDateMin') != null) { hospitality_defaultMinDate = document.getElementById('hdfRangeDateMin').value; }
    if (document.getElementById('hdfRangeDateMax') != null) { hospitality_defaultMaxDate = document.getElementById('hdfRangeDateMax').value; }

    //Default button filters
    setDefaultButtonFilters();
}

//set default button filters
function setDefaultButtonFilters() {
    if (hospitality_buttonFilter_type) {
        var filterValue = ".c-hosp-isotope__" + hospitality_buttonFilter_type;
        
        var $hospitality_Button = $('.c-hosp-isotope__btn-fltr[data-filter="type"]'.replace("type", filterValue));
        if ($hospitality_Button.length > 0) {
            hospitality_buttonFilters["type"] = filterValue;
            $hospitality_Button.addClass('is-checked');
        }
        
    }
    if (hospitality_buttonFilter_subtype) {
        var filterValue = ".c-hosp-isotope__" + hospitality_buttonFilter_subtype;
        
        var $hospitality_Button = $('.c-hosp-isotope__btn-fltr[data-filter="subtype"]'.replace("subtype", filterValue));
        if ($hospitality_Button.length > 0) {
            hospitality_buttonFilters["subtype"] = filterValue;
            $hospitality_Button.addClass('is-checked');
            $("#subTypeAny").removeClass('is-checked');
        }
    }
    if (hospitality_buttonFilter_competition) {
        var filterValue = ".c-hosp-isotope__" + hospitality_buttonFilter_competition;
       
        var $hospitality_Button = $('.c-hosp-isotope__btn-fltr[data-filter="subtype"]'.replace("subtype", filterValue));
        if ($hospitality_Button.length > 0) {
            hospitality_buttonFilters["competition"] = filterValue;
            $hospitality_Button.addClass('is-checked');
            $("#competitionAny").removeClass('is-checked');
        }       
    }
    if (hospitality_buttonFilter_opposition) {
        var filterValue = ".c-hosp-isotope__" + hospitality_buttonFilter_opposition;       
        var $hospitality_Button = $('.c-hosp-isotope__btn-fltr[data-filter="opposition"]'.replace("opposition", filterValue));
        if ($hospitality_Button.length > 0) {
            hospitality_buttonFilters["opposition"] = filterValue;
            $hospitality_Button.addClass('is-checked');
            $("#oppositionAny").removeClass('is-checked');
        }
       
    }
}

//Intialise the isotope grid
function initaliseGrid() {
    //There are two season filters. one for product and one for package
    //If there is only 1 season ticket then we only show the linked packages
    //$(".button.c-hosp-isotope__btn-fltr-fixsSeason").hide();
    //$(".button.c-hosp-isotope__btn-fltr-pakSeason").hide();
    if (hospitality_seasonProductCount == 1) {
        $(".button.c-hosp-isotope__btn-fltr-pakSeason").show();
        $(".button.c-hosp-isotope__btn-fltr-fixsSeason").hide();
    }
    if (hospitality_seasonProductCount > 1 || hospitality_seasonProductCount == 0) {
        // debugger;
        $(".button.c-hosp-isotope__btn-fltr-fixsSeason").show();
        $(".button.c-hosp-isotope__btn-fltr-pakSeason").hide();
    }

    //Set the different range filters
    hospitality_rangeFilters = {
        'budget': {
            'min': hospitality_minBudget,
            'max': hospitality_maxBudget
        },
        'group': {
            'min': hospitality_minGroupSize,
            'max': hospitality_maxGroupSize
        },
        'dateInNumbers': {
            'min': new Date(hospitality_minDate).getTime() / 1000,
            'max': new Date(hospitality_maxDate).getTime() / 1000
        }
    };

    //Setup the isotop grid
    $hospitality_grid = $('.c-hosp-isotope__g').isotope({
        itemSelector: '.c-hosp-isotope__i',
        layout: 'masonry',

        //Set filter function
        filter: function () {
            var isInBudgetRange = false;
            var isInGroupRange = false;
            var isInDateRange = true;
            var budget = $(this).attr('data-budget');
            var group = $(this).attr('data-group');
            var maxLevelGroup;
            var minLevelGroup;
            var groupLevels = "";
            var productDate = $(this).attr('data-date');
            var dateInNumbers = new Date(productDate).getTime() / 1000;

            if (budget == '') {
                isInBudgetRange = true;
            } else {
                isInBudgetRange = (hospitality_rangeFilters['budget'].min <= budget && hospitality_rangeFilters['budget'].max >= budget);
            }
            if (group.indexOf(",") !== -1) { 
                groupLevels = group.split(",");
                maxLevelGroup = groupLevels[0];
                minLevelGroup = groupLevels[1];
            } else {
                minLevelGroup = group;
                maxLevelGroup = group;
            }
            if (group == '') {
                isInGroupRange = true;
            } else {
                isInGroupRange = (hospitality_rangeFilters['group'].min <= minLevelGroup && hospitality_rangeFilters['group'].max >= maxLevelGroup);
            }
            if (productDate == '') {
                var isInDateRange = true;
            } else {
                var isInDateRange = (hospitality_rangeFilters['dateInNumbers'].min <= dateInNumbers && hospitality_rangeFilters['dateInNumbers'].max >= dateInNumbers);
            }
            return $(this).is(hospitality_buttonFilter) && (isInBudgetRange && isInGroupRange && isInDateRange);
        }
    });
}


//Initialise the isotope sliders
function initaliseSliders() {
    if (hospitality_defaultMinGroupSize >= hospitality_minGroupSize && hospitality_defaultMaxGroupSize != 0) {

        //set default group range filter
        $hospitality_groupSlider = $("#filter-group").slider({
            range: true,
            min: hospitality_minGroupSize,
            max: hospitality_maxGroupSize,
            values: [hospitality_defaultMinGroupSize, hospitality_defaultMaxGroupSize],
            slide: function (event, ui) {
                $("#filter-group-amount").text(ui.values[0] + " - " + ui.values[1]);
            }
        });

        //Trigger Isotope
        updateDefaultRangeSlider($hospitality_groupSlider);
    }
    else {
        //Set the slider function range values and slide functions
        $hospitality_groupSlider = $("#filter-group").slider({
            range: true,
            min: hospitality_minGroupSize,
            max: hospitality_maxGroupSize,
            values: [hospitality_minGroupSize, hospitality_maxGroupSize],
            slide: function (event, ui) {
                $("#filter-group-amount").text(ui.values[0] + " - " + ui.values[1]);
            }
        });
    }


    if (hospitality_defaultMinBudget >= hospitality_minBudget && hospitality_defaultMaxBudget != 0) {
        //set default budget range filter
        $hospitality_budgetSlider = $("#filter-budget").slider({
            range: true,
            min: hospitality_minBudget,
            max: hospitality_maxBudget,
            values: [hospitality_defaultMinBudget, hospitality_defaultMaxBudget],
            slide: function (event, ui) {
                $("#filter-budget-amount").html("<span>" + hospitality_currencySymbol + ui.values[0] + " - " + hospitality_currencySymbol + ui.values[1] + "</span>");
            }
        });

        //Trigger Isotope
        updateDefaultRangeSlider($hospitality_budgetSlider);
    }
    else {
        $hospitality_budgetSlider = $("#filter-budget").slider({
            range: true,
            min: hospitality_minBudget,
            max: hospitality_maxBudget,
            values: [hospitality_minBudget, hospitality_maxBudget],
            slide: function (event, ui) {
                $("#filter-budget-amount").html("<span>" + hospitality_currencySymbol + ui.values[0] + " - " + hospitality_currencySymbol + ui.values[1] + "</span>");
            }
        });
    }

    if (hospitality_defaultMinDate != "" && hospitality_defaultMaxDate != "") {
        $hospitality_dateSlider = $("#filter-date").slider({
            range: true,
            min: new Date(hospitality_minDate).getTime() / 1000,
            max: new Date(hospitality_maxDate).getTime() / 1000,
            step: 86400,
            values: [
                new Date(hospitality_defaultMinDate).getTime() / 1000,
                new Date(hospitality_defaultMaxDate).getTime() / 1000
            ],
            slide: function (event, ui) {
                $("#filter-date-amount").text((new Date(ui.values[0] * 1000).toDateString()) + " - " + (new Date(ui.values[1] * 1000)).toDateString());
            }
        });

        //Trigger Isotope
        updateDefaultRangeSlider($hospitality_dateSlider);
    }
    else {
        $hospitality_dateSlider = $("#filter-date").slider({
            range: true,
            min: new Date(hospitality_minDate).getTime() / 1000,
            max: new Date(hospitality_maxDate).getTime() / 1000,
            step: 86400,
            values: [
                new Date(hospitality_minDate).getTime() / 1000,
                new Date(hospitality_maxDate).getTime() / 1000
            ],
            slide: function (event, ui) {
                $("#filter-date-amount").text((new Date(ui.values[0] * 1000).toDateString()) + " - " + (new Date(ui.values[1] * 1000)).toDateString());
            }
        });
    }


    //Set initial text label values based on the highest and lowest slider values
    $("#filter-group-amount").text($("#filter-group").slider("values", 0) + " - " + $("#filter-group").slider("values", 1));
    $("#filter-budget-amount").html("<span>" + hospitality_currencySymbol + $("#filter-budget").slider("values", 0) + " - " + hospitality_currencySymbol + $("#filter-budget").slider("values", 1) + "</span>");
    $("#filter-date-amount").text((new Date($("#filter-date").slider("values", 0) * 1000).toDateString()) + " - " + (new Date($("#filter-date").slider("values", 1) * 1000)).toDateString());

    // Trigger Isotope Filter when slider drag has stopped
    $hospitality_budgetSlider.on('slide slidestop', function (slideEvt) {
        var $this = $(this);
        updateRangeSlider($this, slideEvt);
    });
    $hospitality_groupSlider.on('slide slidestop', function (slideEvt) {
        var $this = $(this);
        updateRangeSlider($this, slideEvt);
    });
    $hospitality_dateSlider.on('slide slidestop', function (slideEvt) {
        var $this = $(this);
        updateRangeSlider($this, slideEvt);
    });
}



//Refresh the isotope grid if a select list change has been triggered
$('.c-hosp-isotope__fltr').on('change', function () {
    var filterValue = this.value;
    var filterGroup = $(this).attr('data-filter-group');
    hospitality_buttonFilters[filterGroup] = filterValue;
    hospitality_buttonFilter = concatValues(hospitality_buttonFilters) || '.c-hosp-isotope__fixs';
    $hospitality_grid.isotope();
});


//Refresh the isotop grid if a range slider has moved
function updateRangeSlider(slider, slideEvt) {
    // +$ is retreiving the value and converting to a numeric 
    var sldmin = +$(slider).slider("values", 0);
    var sldmax = +$(slider).slider("values", 1);
    // Find which filter group this slider is in (in this case it will be either budget or group)
    // This can be changed by modifying the data-filter-group="age" attribute on the slider HTML
    var filterGroup = slider.attr('data-filter-group');

    // Set min and max values for current selection to current selection
    // If no values are found set min to 0 and max to 1000
    // Store min/max values in hospitality_rangeFilters array in the relevant filter group
    // E.g. hospitality_rangeFilters['budget'].min and hospitality_rangeFilters['budget'].max
    hospitality_rangeFilters[filterGroup] = {
        min: sldmin || 0,
        max: sldmax || 1000
    };
    $hospitality_grid.isotope();
    if (!$hospitality_grid.data('isotope').filteredItems.length) {
        $(".c-hosp-isotope__no-results").show();
    } else {
        $(".c-hosp-isotope__no-results").hide();
    }
}

function updateDefaultRangeSlider(slider) {
    // +$ is retreiving the value and converting to a numeric 
    var sldmin = +$(slider).slider("values", 0);
    var sldmax = +$(slider).slider("values", 1);
    // Find which filter group this slider is in (in this case it will be either budget or group)
    // This can be changed by modifying the data-filter-group="age" attribute on the slider HTML
    var filterGroup = slider.attr('data-filter-group');

    // Set min and max values for current selection to current selection
    // If no values are found set min to 0 and max to 1000
    // Store min/max values in hospitality_rangeFilters array in the relevant filter group
    // E.g. hospitality_rangeFilters['budget'].min and hospitality_rangeFilters['budget'].max
    hospitality_rangeFilters[filterGroup] = {
        min: sldmin || 0,
        max: sldmax || 1000
    };
    $hospitality_grid.isotope();
}


//Flatten object by concatting values
function concatValues(obj) {
    var value = '';
    for (var prop in obj) {
        value += obj[prop];
    }
    return value;
}


//Filter buttons click
//Look inside element with .filters class for any clicks on elements with .c-hosp-isotope__btn-fltr
$('.hosp-isotope').on('click', '.c-hosp-isotope__btn-fltr', function () {
    var $this = $(this);
    var $buttonGroup = $this.parents('.c-hosp-isotope__btn-grp');// Get group key from parent btn-group (e.g. data-filter-group="color")
    var filterGroup = $buttonGroup.attr('data-filter-group');
    var alertBoxItems = $(".alert-box.info");

    hospitality_buttonFilters[filterGroup] = $this.attr('data-filter');// set filter for group
    hospitality_buttonFilter = concatValues(hospitality_buttonFilters) || '*';// Combine filters or set the value to * if buttonFilters
    $hospitality_grid.isotope();// Trigger isotope again to refresh layout
    if (!$hospitality_grid.data('isotope').filteredItems.length) {
        var alertBoxVisible = false;
        alertBoxItems.each(function (i, val) {
            var alertBox = $(val);
            if (alertBox.is(":visible")) {
                alertBoxVisible = true;
            } 
        });
        if (alertBoxVisible) {
            $(".c-hosp-isotope__no-results").hide();
        } else {
            $(".c-hosp-isotope__no-results").show();
        }
    } else {
        $(".c-hosp-isotope__no-results").hide();
    }
});


//Show the status message on basis of the availability of products and packages
function showStatusMessage(element) {
    //Hide all 4 status messages
    $(".alert-box.info").hide();

    var btnClassName = element.className.split(' ')[2];
    btnClassName = ".alert-box.info." + btnClassName;
    if ($(btnClassName).html().trim() != '') {
        $(btnClassName).show();
        $(".c-hosp-isotope__no-results").hide();
    }
}


// change is-checked class on btn-filter to toggle which one is active
$('.c-hosp-isotope__btn-grp').each(function (i, buttonGroup) {
    var $buttonGroup = $(buttonGroup);
    $buttonGroup.on('click', '.c-hosp-isotope__btn-fltr', function () {
        $buttonGroup.find('.is-checked').removeClass('is-checked');
        $(this).addClass('is-checked');
    });
});


//Reset button click
$('.c-hosp-isotope__btn-rset').on('click', function () {
    // reset filters
    $(".c-hosp-isotope__pak-sliders").hide();
    $(".c-hosp-isotope__comp-fltr-cont, .c-hosp-isotope__opp-fltr-cont, .c-hosp-isotope__fixs-slider, c-hosp-isotope__fixs-slider, .c-hosp-isotope__subtype-fltr-cont").show();
    hospitality_buttonFilters = {};
    hospitality_buttonFilter = '.c-hosp-isotope__fixs';
    $(".c-hosp-isotope__slider").slider("destroy");
    initaliseGrid();
    initaliseSliders();
    $hospitality_grid.isotope();
    // reset buttons
    $hospitality_buttons.removeClass('is-checked');
    $hospitality_anyButtons.addClass('is-checked');
    $(".c-hosp-isotope__btn-fltr-fixs").addClass("is-checked");
    $(".c-hosp-isotope__fltr").val($(".c-hosp-isotope__fltr option:first").val());
    $(".c-hosp-isotope__btn-fltr-fixs").click();
    $('.c-hosp-isotope__btn-rset').removeClass("ebiz-primary-action");
    $('.c-hosp-isotope__btn-rset').addClass("ebiz-muted-action");
});


//Home Fixtures button click
$(".button.c-hosp-isotope__btn-fltr-fixs").click(function () {
    showStatusMessage(this);

    hospitality_minBudget = hospitality_minBudgetHome;
    hospitality_maxBudget = hospitality_maxBudgetHome;
    hospitality_minGroupSize = hospitality_minGroupSizeHome;
    hospitality_maxGroupSize = hospitality_maxGroupSizeHome;
    $(".c-hosp-isotope__pak-sliders").hide();
    $(".c-hosp-isotope__fixs-slider").show();
    $(".c-hosp-isotope__comp-fltr-cont").show();
    $(".c-hosp-isotope__opp-fltr-cont").show();
    $(".c-hosp-isotope__subtype-fltr-cont").show();
    $(".c-hosp-isotope__comp-fltr-opp").show();
});


//Season Fixtures button click
$(".button.c-hosp-isotope__btn-fltr-fixsSeason").click(function () {
    showStatusMessage(this);

    hospitality_minBudget = hospitality_minBudgetSeason;
    hospitality_maxBudget = hospitality_maxBudgetSeason;
    hospitality_minGroupSize = hospitality_minGroupSizeSeason;
    hospitality_maxGroupSize = hospitality_maxGroupSizeSeason;
    $(".c-hosp-isotope__pak-sliders").hide();
    $(".c-hosp-isotope__comp-fltr-cont").hide();
    $(".c-hosp-isotope__opp-fltr-cont").hide();
    $(".c-hosp-isotope__comp-fltr-opp").hide();
    $(".c-hosp-isotope__fixs-slider").hide();
    $(".c-hosp-isotope__subtype-fltr-cont").hide();
    $('.c-hosp-isotope__btn-rset').removeClass("ebiz-muted-action");
    $('.c-hosp-isotope__btn-rset').addClass("ebiz-primary-action");
});


//Home Packages button click
$(".button.c-hosp-isotope__btn-fltr-pak").click(function () {
    showStatusMessage(this);

    hospitality_minBudget = hospitality_minBudgetHome;      
    hospitality_maxBudget = hospitality_maxBudgetHome;
    hospitality_minGroupSize = hospitality_minGroupSizeHome;
    hospitality_maxGroupSize = hospitality_maxGroupSizeHome;
    $(".c-hosp-isotope__slider").slider("destroy");
    initaliseGrid();
    initaliseSliders();
    $hospitality_grid.isotope();
    $(".c-hosp-isotope__comp-fltr-cont").show();
    $(".c-hosp-isotope__opp-fltr-cont").show();
    $(".c-hosp-isotope__subtype-fltr-cont").show();
    $(".c-hosp-isotope__comp-fltr-opp").show();
    $(".c-hosp-isotope__pak-sliders").show();
    $(".c-hosp-isotope__fixs-slider").hide();
    $('.c-hosp-isotope__btn-rset').removeClass("ebiz-muted-action");
    $('.c-hosp-isotope__btn-rset').addClass("ebiz-primary-action");
});


//Season Packages button click
$(".button.c-hosp-isotope__btn-fltr-pakSeason").click(function () {
    showStatusMessage(this);

    hospitality_minBudget = hospitality_minBudgetSeason;
    hospitality_maxBudget = hospitality_maxBudgetSeason;
    hospitality_minGroupSize = hospitality_minGroupSizeSeason;
    hospitality_maxGroupSize = hospitality_maxGroupSizeSeason;
    hospitality_buttonFilters = {};
    $(".c-hosp-isotope__slider").slider("destroy");
    initaliseGrid();
    initaliseSliders();
    $hospitality_grid.isotope();
    // reset buttons
    $hospitality_buttons.removeClass('is-checked');
    $hospitality_anyButtons.addClass('is-checked');
    $(".c-hosp-isotope__btn-fltr-pakSeason").addClass("is-checked");
    $(".c-hosp-isotope__fltr").val($(".c-hosp-isotope__fltr option:first").val());
    //
    $(".c-hosp-isotope__pak-sliders").show();
    $(".c-hosp-isotope__comp-fltr-opp").hide();
    $(".c-hosp-isotope__comp-fltr-cont").hide();
    $(".c-hosp-isotope__opp-fltr-cont").hide();
    $(".c-hosp-isotope__fixs-slider").hide();
    $(".c-hosp-isotope__subtype-fltr-cont").hide();
    $('.c-hosp-isotope__btn-rset').removeClass("ebiz-muted-action");
    $('.c-hosp-isotope__btn-rset').addClass("ebiz-primary-action");
});

//Product Group button click
$(".button.c-hosp-isotope__btn-fltr-bundles").click(function () {
    showStatusMessage(this);

    hospitality_minBudget = hospitality_minBudgetSeason;
    hospitality_maxBudget = hospitality_maxBudgetSeason;
    hospitality_minGroupSize = hospitality_minGroupSizeSeason;
    hospitality_maxGroupSize = hospitality_maxGroupSizeSeason;
    $(".c-hosp-isotope__pak-sliders").hide();
    $(".c-hosp-isotope__comp-fltr-cont").hide();
    $(".c-hosp-isotope__opp-fltr-cont").hide();
    $(".c-hosp-isotope__comp-fltr-opp").hide();
    $(".c-hosp-isotope__fixs-slider").hide();
    $(".c-hosp-isotope__subtype-fltr-cont").hide();
    $('.c-hosp-isotope__btn-rset').removeClass("ebiz-muted-action");
    $('.c-hosp-isotope__btn-rset').addClass("ebiz-primary-action");
});
