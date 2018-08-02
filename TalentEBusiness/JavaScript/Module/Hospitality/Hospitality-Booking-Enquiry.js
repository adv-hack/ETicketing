//#################################################################################
//Hospitality Booking Enquiry Javascript code
//#################################################################################
var alertify_title = "";
var alertify_message = "";
var alertify_okText = "";
var alertify_cancelText = "";
var datatables_emptyTableText = "";
var datatables_clearTableState = "";
var alertify_title_qAndAReminder = "";
var alertify_message_qAndAReminder = "";
var alertify_okText_qAndAReminder = "";
var alertify_cancelText_qAndAReminder = "";
var qAndA_reminder_disabled_message = "";
var alertify_title_printAll = "";
var alertify_message_printAll = "";
var alertify_okText_printAll = "";
var alertify_cancelText_printAll = "";
var alertify_title_printSingleBooking = "";
var alertify_message_printSingleBooking = "";
var alertify_message_printSingleBookingMerged = "";
var alertify_okText_printSingleBooking = "";
var alertify_cancelText_printSingleBooking = "";
var alertify_title_createSingleBookingDocument = "";
var alertify_message_createSingleBookingDocument = "";
var alertify_message_createSingleBookingDocumentMerged = "";
var alertify_okText_createSingleBookingDocument = "";
var alertify_cancelText_createSingleBookingDocument = "";
var merged_document_path = "";
var printAll_disabled_message = "";
var print_status_not_printed = "";
var print_status_partially_printed = "";
var print_status_fully_printed = "";
var booking_status_sold = "";
var callIdList = new Array();
var callIdListForPrint = new Array();
var alertify_title_pdf = "";
var alertify_message_pdf = "";
var alertify_okText_pdf = "";
var alertify_cancelText_pdf = "";

//Execute when page is ready
$(document).ready(function () {
    $(".datepicker").datepicker({
        dateFormat: "dd/mm/yy",
    });
    loadVariables();
    openMergedDocument();
    drawTheDataTable();
    if (datatables_clearTableState == "true") {
        var table = $(".datatable").DataTable();
        table.state.clear();
        datatables_clearTableState = "";
        document.getElementById("hdfClearDataTableState").value = "";
        //the data table will need to be redrawn at this point
        table.destroy();
        drawTheDataTable(); 
    }
});

//Load the hospitality booking local variables
function loadVariables() {
    if (document.getElementById("hdfDataTablesInfoEmpty") != null) { datatables_emptyTableText = document.getElementById("hdfDataTablesInfoEmpty").value; }
    if (document.getElementById("hdfClearDataTableState") != null) { datatables_clearTableState = document.getElementById("hdfClearDataTableState").value; }

    if (document.getElementById("hdfAlertifyTitle") != null) { alertify_title = document.getElementById("hdfAlertifyTitle").value; }
    if (document.getElementById("hdfAlertifyMessage") != null) { alertify_message = document.getElementById("hdfAlertifyMessage").value; }
    if (document.getElementById("hdfAlertifyOK") != null) { alertify_okText = document.getElementById("hdfAlertifyOK").value; }
    if (document.getElementById("hdfAlertifyCancel") != null) { alertify_cancelText = document.getElementById("hdfAlertifyCancel").value; }

    if (document.getElementById("hdfAlertifyTitleForSingleBookingDocument") != null) { alertify_title_createSingleBookingDocument = document.getElementById("hdfAlertifyTitleForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyMessageForSingleBookingDocument") != null) { alertify_message_createSingleBookingDocument = document.getElementById("hdfAlertifyMessageForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyOKForSingleBookingDocument") != null) { alertify_okText_createSingleBookingDocument = document.getElementById("hdfAlertifyOKForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyCancelForSingleBookingDocument") != null) { alertify_cancelText_createSingleBookingDocument = document.getElementById("hdfAlertifyCancelForSingleBookingDocument").value; }

    if (document.getElementById("hdfAlertifyTitleForQAndAReminder") != null) { alertify_title_qAndAReminder = document.getElementById("hdfAlertifyTitleForQAndAReminder").value; }
    if (document.getElementById("hdfAlertifyMessageForQAndAReminder") != null) { alertify_message_qAndAReminder = document.getElementById("hdfAlertifyMessageForQAndAReminder").value; }
    if (document.getElementById("hdfAlertifyOKForQAndAReminder") != null) { alertify_okText_qAndAReminder = document.getElementById("hdfAlertifyOKForQAndAReminder").value; }
    if (document.getElementById("hdfAlertifyCancelForQAndAReminder") != null) { alertify_cancelText_qAndAReminder = document.getElementById("hdfAlertifyCancelForQAndAReminder").value; }
    if (document.getElementById("hdfQAndAReminderDisabledMessage") != null) { qAndA_reminder_disabled_message = document.getElementById("hdfQAndAReminderDisabledMessage").value; }

    if (document.getElementById("hdfAlertifyTitleForPrintAll") != null) { alertify_title_printAll = document.getElementById("hdfAlertifyTitleForPrintAll").value; }
    if (document.getElementById("hdfAlertifyMessageForPrintAll") != null) { alertify_message_printAll = document.getElementById("hdfAlertifyMessageForPrintAll").value; }
    if (document.getElementById("hdfAlertifyOKForPrintAll") != null) { alertify_okText_printAll = document.getElementById("hdfAlertifyOKForPrintAll").value; }
    if (document.getElementById("hdfAlertifyCancelForPrintAll") != null) { alertify_cancelText_printAll = document.getElementById("hdfAlertifyCancelForPrintAll").value; }
    if (document.getElementById("hdfPrintAllDisabledMessage") != null) { printAll_disabled_message = document.getElementById("hdfPrintAllDisabledMessage").value; }
  
    if (document.getElementById("hdfAlertifyTitleForPrintSingleBooking") != null) { alertify_title_printSingleBooking = document.getElementById("hdfAlertifyTitleForPrintSingleBooking").value; }
    if (document.getElementById("hdfAlertifyMessageForPrintSingleBooking") != null) { alertify_message_printSingleBooking = document.getElementById("hdfAlertifyMessageForPrintSingleBooking").value; }
    if (document.getElementById("hdfAlertifyOKForPrintSingleBooking") != null) { alertify_okText_printSingleBooking = document.getElementById("hdfAlertifyOKForPrintSingleBooking").value; }
    if (document.getElementById("hdfAlertifyCancelForPrintSingleBooking") != null) { alertify_cancelText_printSingleBooking = document.getElementById("hdfAlertifyCancelForPrintSingleBooking").value; }

    if (document.getElementById("hdfPrintStatusNotPrinted") != null) { print_status_not_printed = document.getElementById("hdfPrintStatusNotPrinted").value; }
    if (document.getElementById("hdfPrintStatusPartiallyPrinted") != null) { print_status_partially_printed = document.getElementById("hdfPrintStatusPartiallyPrinted").value; }
    if (document.getElementById("hdfPrintStatusFullyPrinted") != null) { print_status_fully_printed = document.getElementById("hdfPrintStatusFullyPrinted").value; }
    if (document.getElementById("hdfBookingStatus") != null) { booking_status_sold = document.getElementById("hdfBookingStatus").value; }

    if (document.getElementById("hdfAlertifyTitleForSingleBookingDocument") != null) { alertify_title_createSingleBookingDocument = document.getElementById("hdfAlertifyTitleForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyMessageForSingleBookingDocument") != null) { alertify_message_createSingleBookingDocument = document.getElementById("hdfAlertifyMessageForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyOKForSingleBookingDocument") != null) { alertify_okText_createSingleBookingDocument = document.getElementById("hdfAlertifyOKForSingleBookingDocument").value; }
    if (document.getElementById("hdfAlertifyCancelForSingleBookingDocument") != null) { alertify_cancelText_createSingleBookingDocument = document.getElementById("hdfAlertifyCancelForSingleBookingDocument").value; }    
    if (document.getElementById("hdfMergedDocumentPath") != null) { merged_document_path = document.getElementById("hdfMergedDocumentPath").value; }

    if (document.getElementById("hdfAlertifyTitleForCreatePDF") != null) { alertify_title_pdf = document.getElementById("hdfAlertifyTitleForCreatePDF").value; }
    if (document.getElementById("hdfAlertifyMessageForCreatePDF") != null) { alertify_message_pdf = document.getElementById("hdfAlertifyMessageForCreatePDF").value; }
    if (document.getElementById("hdfAlertifyOKForCreatePDF") != null) { alertify_okText_pdf = document.getElementById("hdfAlertifyOKForCreatePDF").value; }
    if (document.getElementById("hdfAlertifyCancelForCreatePDF") != null) { alertify_cancelText_pdf = document.getElementById("hdfAlertifyCancelForCreatePDF").value; }
}


//Draw the datatable
function drawTheDataTable() {
    var hidecolumns = document.getElementById("hdfHideColumns").value;
    var hideColAry = hidecolumns.split(",").map(Number);
    $(".datatable").DataTable({

        "columnDefs": [
            { "targets": [9], "orderable": false },
            { "targets": [4], "orderData": [9] },
            { "targets": hideColAry, "visible": false, "searchable": false }
        ],
        "language": { "emptyTable": datatables_emptyTableText },
        "order": [[0, "desc"]],
        "stateSave": true,
        "stateSaveParams": function (settings, data) {
            for (var i = 0; i < data.columns.length; i++) {
                delete data.columns[i].visible;
            }
        },
    });

    var table = $(".datatable").DataTable();
    var cells = [];
    var data = table.rows().data();    
    data.each(function (value, index) {
        var splitRowObject = value.toString().split(',');
        if (splitRowObject.length > 0) {
            if (splitRowObject[splitRowObject.length-1] == "True")
                callIdList.push(splitRowObject[0]);
            if (splitRowObject[6] == booking_status_sold && (splitRowObject[8] ==print_status_not_printed || splitRowObject[8] == print_status_partially_printed || splitRowObject[8] == print_status_fully_printed))
                callIdListForPrint.push(splitRowObject[0]);
        }
        
    });
    document.getElementById("hdfListOfCallIds").value = callIdList.join(',')
    if (callIdList.length > 0) {
        $(".ebiz-send-qa-reminder").removeClass("has-tip top").addClass("ebiz-primary-action");
    }
    else {      
        $(".ebiz-send-qa-reminder").removeClass("has-tip top").addClass("ebiz-muted-action");
        $(".ebiz-send-qa-reminder").attr("title", qAndA_reminder_disabled_message);
        $(".ebiz-send-qa-reminder").removeAttr("data-open");
        var elem = new Foundation.Tooltip($(".ebiz-send-qa-reminder"));
    }
    if (callIdListForPrint.length > 0) {      
        $(".ebiz-print-all-tickets").removeClass("has-tip top").addClass("ebiz-primary-action");
    }
    else {
        $(".ebiz-print-all-tickets").removeClass("has-tip top").addClass("ebiz-muted-action");
        $(".ebiz-print-all-tickets").attr("title", printAll_disabled_message);
        $(".ebiz-print-all-tickets").removeAttr("data-open");
        var elem = new Foundation.Tooltip($(".ebiz-print-all-tickets"));
    }
}


//Details booking link confirmation
function DetailsButtonClick(itemForwardingUrl, requiresLogin) {
    if (requiresLogin == "True") {
        alertify.confirm(alertify_title, alertify_message,
            function () {
                document.getElementById("hdfForwardingUrl").value = itemForwardingUrl;
                $("#form1").submit();
            },
            function () {
            })
        .set('labels', { ok: alertify_okText, cancel: alertify_cancelText });
    } else {
        document.getElementById("hdfForwardingUrl").value = itemForwardingUrl;
        $("#form1").submit();
    }
}


//Clear the state of the datatable (page number, sorting, etc) when the user clicks the clear button
function ClearDataTableState() {
    var table = $(".datatable").DataTable();
    table.state.clear();
    return true;
}


//Send email reminder for incomplete Q&A
function SendQAndAReminderButtonClick() {
    if (callIdList.length > 0) {
        alertify.confirm(alertify_title_qAndAReminder, alertify_message_qAndAReminder,
            function () {
               
                document.getElementById("hdfSendQAndAReminder").value = "True";
                $("#form1").submit();
            },
            function () {
                //cancel clicked
            })
        .set('labels', { ok: alertify_okText_qAndAReminder, cancel: alertify_cancelText_qAndAReminder });
    }
    else
    {
        alertify.notify(qAndA_reminder_disabled_message, '-default', 4);
        return false;
    }
}


//Print all filterd hospitality bookings.
function PrintBookingsClick() {
    if (callIdListForPrint.length > 0) {
        alertify.confirm(alertify_title_printAll, alertify_message_printAll,
            function () {        
                document.getElementById("hdfPrintBookings").value = "True";
                $("#form1").submit();
            },
            function () {
                //cancel clicked
            })
        .set('labels', { ok: alertify_okText_printAll, cancel: alertify_cancelText_printAll });
    }
    else {
        alertify.notify(printAll_disabled_message, '-default', 4);
        return false;
    }
}


//Print Single Booking Tickets Function
function PrintSingleBookingClick(callid, numberOfTickets)
{    
    alertify_message_printSingleBookingMerged = alertify_message_printSingleBooking.replace("xxNumberOfTicketsToPrintxx", numberOfTickets.bold());
    alertify.confirm(alertify_title_printSingleBooking, alertify_message_printSingleBookingMerged,
        function ()
        {
            document.getElementById("hdfPrintSingleBooking").value = "True";
            document.getElementById("hdfSelectedCallIdToBePrinted").value = callid;
            document.getElementById("hdfNumberOfTicketsInBooking").value = numberOfTickets;
            $("#form1").submit();
        },
        function ()
        {
            //cancel clicked
        })
        .set('labels', { ok: alertify_okText_printSingleBooking, cancel: alertify_cancelText_printSingleBooking });
}


//Generate Document For Single Booking
function GenerateDocumentForBookingClick(callid)
{
    alertify_message_createSingleBookingDocumentMerged = alertify_message_createSingleBookingDocument.replace("xxCallIdxx", callid.bold());
    alertify.confirm(alertify_title_createSingleBookingDocument, alertify_message_createSingleBookingDocumentMerged,
        function ()
        {
            document.getElementById("hdfGenerateBookingDocument").value = "True";
            document.getElementById("hdfCallIdForDocumentProduction").value = callid;
            $("#form1").submit();
        },
        function ()
        {
            //cancel clicked
        })
        .set('labels', { ok: alertify_okText_createSingleBookingDocument, cancel: alertify_cancelText_createSingleBookingDocument});
}


//Create a PDF for the booking
function CreatePDFForBookingClick(itemForwardingUrl, requiresLogin)
{
    if (requiresLogin == "True") {
        alertify.confirm(alertify_title_pdf, alertify_message_pdf,
            function () {
                document.getElementById("hdfForwardingUrl").value = itemForwardingUrl;
                $("#form1").submit();
            },
            function () {
            })
        .set('labels', { ok: alertify_okText_pdf, cancel: alertify_cancelText_pdf });
    } else {
        document.getElementById("hdfCreatePDFForBooking").value = "True";
    }
}


//Open the merged document
function openMergedDocument()
{
    if (merged_document_path != '')
    {
        //first line is for testing purposes and second line needs to be uncommented once documentation is completed
        //window.open('../../Assets/Documents/HospMergeDocs/SampleDoc.docx');
        //window.open(merged_document_path);
    }        
}
