//#################################################################################
//Hospitality Booking Javascript code
//#################################################################################
var alertify_title = "";
var alertify_message = "";
var alertify_okText = "";
var alertify_cancelText = "";
var updatePackageUsed = false;
var updatePackageMessageText = "";
var datepicker_clearDateText = "";
var alertify_title_printSingleSeat = "";
var alertify_message_printSingleSeat = "";
var alertify_okText_printSingleSeat = "";
var alertify_cancelText_printSingleSeat = "";
var alertify_message_printSingleSeatMerged = "";

var alertify_title_printBooking = "";
var alertify_message_printBooking = "";
var alertify_okText_printBooking = "";
var alertify_cancelText_printBooking = "";
var alertify_message_printBookingMerged = "";

var alertify_title_createBookingDocument = "";
var alertify_message_createBookingDocument = "";
var alertify_okText_createBookingDocument = "";
var alertify_cancelText_createBookingDocument = "";
var alertify_message_createBookingDocumentMerged = "";
var merged_word_document_path = "";

//Load the hospitality booking local variables
function loadVariables() {
    if (document.getElementById("hdfAlertifyTitle") != null) { alertify_title = document.getElementById("hdfAlertifyTitle").value; }
    if (document.getElementById("hdfAlertifyMessage") != null) { alertify_message = document.getElementById("hdfAlertifyMessage").value; }
    if (document.getElementById("hdfAlertifyOK") != null) { alertify_okText = document.getElementById("hdfAlertifyOK").value; }
    if (document.getElementById("hdfAlertifyCancel") != null) { alertify_cancelText = document.getElementById("hdfAlertifyCancel").value; }
    if (document.getElementById("hdfUpdatePackageMessageText") != null) { updatePackageMessageText = document.getElementById("hdfUpdatePackageMessageText").value; }
    if (document.getElementById("hdfDatePickerClearDateText") != null) { datepicker_clearDateText = document.getElementById("hdfDatePickerClearDateText").value; }   
    if (document.getElementById("hdfAlertifyTitleForPrintSingleSeat") != null) { alertify_title_printSingleSeat = document.getElementById("hdfAlertifyTitleForPrintSingleSeat").value; }
    if (document.getElementById("hdfAlertifyMessageForPrintSingleSeat") != null) { alertify_message_printSingleSeat = document.getElementById("hdfAlertifyMessageForPrintSingleSeat").value; }
    if (document.getElementById("hdfAlertifyOKForPrintSingleSeat") != null) { alertify_okText_printSingleSeat = document.getElementById("hdfAlertifyOKForPrintSingleSeat").value; }
    if (document.getElementById("hdfAlertifyCancelForPrintSingleSeat") != null) { alertify_cancelText_printSingleSeat = document.getElementById("hdfAlertifyCancelForPrintSingleSeat").value; }

    if (document.getElementById("hdfAlertifyTitleForPrintBooking") != null) { alertify_title_printBooking = document.getElementById("hdfAlertifyTitleForPrintBooking").value; }
    if (document.getElementById("hdfAlertifyMessageForPrintBooking") != null) { alertify_message_printBooking = document.getElementById("hdfAlertifyMessageForPrintBooking").value; }
    if (document.getElementById("hdfAlertifyOKForPrintBooking") != null) { alertify_okText_printBooking = document.getElementById("hdfAlertifyOKForPrintBooking").value; }
    if (document.getElementById("hdfAlertifyCancelForPrintBooking") != null) { alertify_cancelText_printBooking = document.getElementById("hdfAlertifyCancelForPrintBooking").value; }

    if (document.getElementById("hdfAlertifyTitleForSingleBookingDocument") != null) { alertify_title_createBookingDocument = document.getElementById("hdfAlertifyTitleForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyMessageForSingleBookingDocument") != null) { alertify_message_createBookingDocument = document.getElementById("hdfAlertifyMessageForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyOKForSingleBookingDocument") != null) { alertify_okText_createBookingDocument = document.getElementById("hdfAlertifyOKForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyCancelForSingleBookingDocument") != null) { alertify_cancelText_createBookingDocument = document.getElementById("hdfAlertifyCancelForSingleBookingDocument").value; }
    if (document.getElementById("hdfMergedDocumentPath") != null) { merged_word_document_path = document.getElementById("hdfMergedDocumentPath").value; }
}


//Table visibility control for responsive layout
$(document).ready(function () {
    loadVariables();
    openMergedDocument();
    if (Foundation.MediaQuery.atLeast('large')) {
        $(".js-admin-table tbody tr").hover(function () {
            $(this).find(".js-admin-table__menu").css("visibility", "visible");
        }, function () {
            $(this).find(".js-admin-table__menu").css("visibility", "hidden");
        });
    }
    if (document.getElementById("hdfReserveClick") != null && document.getElementById("hdfReserveClick").value == "true") {
        $('#reserve-button').foundation('open');
        document.getElementById("hdfReserveClick").value = "false"
    }
});

$(function () {
    $(".datepicker").datepicker({
        changeYear: true,
        yearRange: "-99:+99",
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        showButtonPanel: true,
        closeText: '<i class="fa fa-minus-circle" aria-hidden="true"></i>' + datepicker_clearDateText,
        beforeShow: function (input) {
            setTimeout(function () {
                $(input).datepicker("widget").find(".ui-datepicker-current").hide();
                var clearButton = $(input).datepicker("widget").find(".ui-datepicker-close");
                clearButton.unbind("click").bind("click", function () { $.datepicker._clearDate(input); });
            }, 1);
        }
    });
});

//Cancel booking confirmation
function CancelButtonClick() {
    alertify.confirm(alertify_title, alertify_message,
        function () {
            document.getElementById("hdfCancelBooking").value = "true";
            $("#form1").submit();
        },
        function () {
            //cancel clicked
        })
    .set('labels', { ok: alertify_okText, cancel: alertify_cancelText });
}

    
//Handle the redirection for the customer drop down list when registering a new customer or F&F
function CustomerDDLRedirect(ddlCustomerID) {
    var hdfFFRedirectURL = document.getElementById("hdfFFRedirectURL");
    var hdfNewCustomerRedirectText = document.getElementById("hdfNewCustomerRedirectText");
    var ddlCustomer = document.getElementById(ddlCustomerID);
    var ddlCustomerSelectedValue = ddlCustomer.value;
    var ddlCustomerSelectedText = ddlCustomer.options[ddlCustomer.selectedIndex].text;

    if (ddlCustomerSelectedValue != undefined) {
        if (ddlCustomerSelectedValue == hdfFFRedirectURL.value) {
            location = hdfFFRedirectURL.value;
        }
        if (ddlCustomerSelectedText == hdfNewCustomerRedirectText.value) {
            location = ddlCustomerSelectedValue;
        }
    }
}

function updatePackage() {
    if (!updatePackageUsed) {
        alertify.notify(updatePackageMessageText, '-basket-needs-updating', 5);
        updatePackageUsed = true;
    }
};

//Collapse opened non availability component 
function CollapseAccordion(element) {
    if (!element.classList.contains("A")) {
        var $expandedAccordions = $(".accordion").find(".accordion-item.is-active .accordion-content");
        $expandedAccordions.each(function (i, section) {
            var elementDiv = $(element).find(".accordion-content");
            if (section != elementDiv["0"] && !section.parentElement.classList.contains("A")) {
                $(".accordion").foundation('up', $(section));
            }
        });
    }
};

//Print single seat confirmation 
function PrintSingleSeat(seatToBePrinted, componentId, formattedSeatDetails) {
    alertify_message_printSingleSeatMerged = alertify_message_printSingleSeat.replace("XXX", formattedSeatDetails.bold());
    alertify.confirm(alertify_title_printSingleSeat, alertify_message_printSingleSeatMerged,
            function () {
                document.getElementById("hdfPrintSingleSeat").value = "true";
                document.getElementById("hdfSeatToBePrinted").value = seatToBePrinted;
                document.getElementById("hdfFormattedSeatToBePrinted").value = formattedSeatDetails;
                document.getElementById("hdfComponentId").value = componentId;
                $("#form1").submit();
            },
            function () {
                //cancel clicked
            })
        .set('labels', { ok: alertify_okText_printSingleSeat, cancel: alertify_cancelText_printSingleSeat });
};

//Print booking confirmation
function PrintBookingClick() {
    alertify_message_printBookingMerged = alertify_message_printBooking.replace("xxNumberOfTicketsToPrintxx", document.getElementById("hdfNumberOfTicketsToPrint").value.bold());
    alertify.confirm(alertify_title_printBooking, alertify_message_printBookingMerged,
        function () {
            document.getElementById("hdfPrintBooking").value = "true";
            $("#form1").submit();
        },
        function () {
            //cancel clicked
        })
    .set('labels', { ok: alertify_okText_printBooking, cancel: alertify_cancelText_printBooking });
};

//Generate document for booking confirmation click
function GenerateDocumentForBookingClick()
{
    alertify_message_createBookingDocumentMerged = alertify_message_createBookingDocument.replace("xxCallIdxx", document.getElementById("hdfCallIdForDocumentProduction").value.bold());
    alertify.confirm(alertify_title_createBookingDocument, alertify_message_createBookingDocumentMerged,
        function ()
        {
            document.getElementById("hdfGenerateBookingDocument").value = "true";
            $("#form1").submit();
        },
        function ()
        {
            //cancel clicked
        })
    .set('labels', { ok: alertify_okText_createBookingDocument, cancel: alertify_cancelText_createBookingDocument });
};

//Open the merged document
function openMergedDocument()
{
    if (merged_word_document_path != '')
    {
        //first line is for testing purposes and second line needs to be uncommented once documentation is completed
        //window.open('../../Assets/Documents/HospMergeDocs/SampleDoc.docx');
        //window.open(merged_word_document_path);
    }
}

function updateBookingAndOpenReserveModal() 
{
    document.getElementById("hdfReserveClick").value = "true";
    document.getElementById("SiteMasterBody_ContentPlaceHolder1_lbtnUpdate").click();   
}


