//#################################################################################
//Activities Module Javascript code
//#################################################################################
var activity_sessionId = "";
var activity_customerNumber = "";
var activity_talentAPIUrl = "";
var activity_customerActivityHeaderId = "";
var activity_selectedAgentName = "";
var activity_selectedTemplateId = "";
var activity_editingTemplateId = "";
var activity_selectedDate = "";
var activity_selectedStatus = "";
var activity_subjectText = "";
var activity_commentText = "";
var activity_maxFileUploadSize = "";
var activity_allowedFileTypes = [];
var activity_fileItemIndex = "";
var activity_invalidFileMessage = "";
var activity_commentItemIndex = "";

var datatables_lengthMenu = "";
var datatables_zeroRecords = "";
var datatables_info = "";
var datatables_infoEmpty = "";
var datatables_infoFiltered = "";
var datatables_previousPage = "";
var datatables_nextPage = "";
var datepicker_clearDateText = "";

var alertify_title = "";
var alertify_message = "";
var alertify_okText = "";
var alertify_cancelText = "";


// Page Function
$(document).ready(function () {
    retrieveActivities();

    $(document).on("mouseenter", ".ebiz-edit-activity-comment-text", function() {
        $(this).parent().addClass("hover");
        $(this).nextAll().eq(0).toggle();
    });

    $(document).on("mouseleave", ".ebiz-edit-activity-comment-text", function() {
        $(this).parent().removeClass("hover");
        $(this).nextAll().eq(0).toggle();
    });

    $(document).on("focus",".ebiz-edit-activity-comment-text", function() {
        $(this).parent().addClass("focus");
        $(this).nextAll().eq(1).show();
    }).on("blur",".ebiz-edit-activity-comment-text", function() {
        $(this).parent().removeClass("focus");
        $(this).nextAll().eq(1).hide(0);
    });
    
    window.onload = createUploader;
});


// Set Date Picker
$(function () {
    $(".datepicker").datepicker({
        changeYear: true,
        yearRange: "-99:+99",
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        showButtonPanel: true,
        closeText: '<i class="fa fa-minus-circle" aria-hidden="true"></i>' + datepicker_clearDateText,
        beforeShow: function( input ) {
            setTimeout(function () {
                $(input).datepicker("widget").find(".ui-datepicker-current").hide();
                var clearButton = $(input).datepicker("widget").find(".ui-datepicker-close");
                clearButton.unbind("click").bind("click",function(){$.datepicker._clearDate(input);});
            }, 1 );
        }
    });
});


//Load the activities local variables used when retrieving activities
function loadVariables() {
    if (document.getElementById("hdfSessionID") != null) { activity_sessionId = document.getElementById("hdfSessionID").value; }
    if (document.getElementById("hdfCustomerNumber") != null) { activity_customerNumber = document.getElementById("hdfCustomerNumber").value; }
    if (document.getElementById("hdfTalentAPIUrl") != null) { activity_talentAPIUrl = document.getElementById("hdfTalentAPIUrl").value; }
    if (document.getElementById("hdfTemplateID") != null) { activity_editingTemplateId = document.getElementById("hdfTemplateID").value; }
    if (document.getElementById("hdfCustomerActivityUniqueID") != null) { activity_customerActivityHeaderId = document.getElementById("hdfCustomerActivityUniqueID").value; }
    if (document.getElementById("txtDate") != null) { activity_selectedDate = document.getElementById("txtDate").value; }

    if (document.getElementById("hdfDataTablesLengthMenu") != null) { datatables_lengthMenu = document.getElementById("hdfDataTablesLengthMenu").value; }
    if (document.getElementById("hdfDataTablesZeroRecords") != null) { datatables_zeroRecords = document.getElementById("hdfDataTablesZeroRecords").value; }
    if (document.getElementById("hdfDataTablesInfo") != null) { datatables_info = document.getElementById("hdfDataTablesInfo").value; }
    if (document.getElementById("hdfDataTablesInfoEmpty") != null) { datatables_infoEmpty = document.getElementById("hdfDataTablesInfoEmpty").value; }
    if (document.getElementById("hdfDataTablesInfoFiltered") != null) { datatables_infoFiltered = document.getElementById("hdfDataTablesInfoFiltered").value; }
    if (document.getElementById("hdfDataTablesPreviousPage") != null) { datatables_previousPage = document.getElementById("hdfDataTablesPreviousPage").value; }
    if (document.getElementById("hdfDataTablesNextPage") != null) { datatables_nextPage = document.getElementById("hdfDataTablesNextPage").value; }
    if (document.getElementById("hdfAlertifyOK") != null) { alertify_okText = document.getElementById("hdfAlertifyOK").value; }
    if (document.getElementById("hdfAlertifyCancel") != null) { alertify_cancelText = document.getElementById("hdfAlertifyCancel").value; }
    if (document.getElementById("txtAddComment") != null) { activity_commentText = document.getElementById("txtAddComment").value; }
    if (document.getElementById("hdfDatePickerClearDateText") != null) { datepicker_clearDateText = document.getElementById("hdfDatePickerClearDateText").value; }
    if (document.getElementById("hdfMaxFileSize") != null) { activity_maxFileUploadSize = document.getElementById("hdfMaxFileSize").value; }
    if (document.getElementById("hdfActivityFileItemIndex") != null) { activity_fileItemIndex = document.getElementById("hdfActivityFileItemIndex").value; }
    if (document.getElementById("hdfInvalidFileErrorMessage") != null) { activity_invalidFileMessage = document.getElementById("hdfInvalidFileErrorMessage").value; }
    if (document.getElementById("hdfActivityCommentItemIndex") != null) { activity_commentItemIndex = document.getElementById("hdfActivityCommentItemIndex").value; }
    if (document.getElementById("hdfAllowedFileTypes") != null) {
        activity_allowedFileTypes = document.getElementById("hdfAllowedFileTypes").value.split(",");
    }

    if (document.getElementById("ddlUser") != null) {
        if ($("#ddlUser").val() != "") {
            activity_selectedAgentName = $("#ddlUser").val();
        } else {
            activity_selectedAgentName = "";
        }
    }
    if (document.getElementById("ddlActivityTemplate") != null) {
        if ($("#ddlActivityTemplate").val() != "0") {
            activity_selectedTemplateId = $("#ddlActivityTemplate").val();
        } else {
            activity_selectedTemplateId = "";
        }
    }
    if (document.getElementById("ddlStatus") != null) {
        if ($("#ddlStatus").val() != "") {
            activity_selectedStatus = $("#ddlStatus ").val();
        } else {
            activity_selectedStatus = "";
        }
    }
    if (document.getElementById("txtSubject") != null) {
        activity_subjectText = document.getElementById("txtSubject").value;
    } else {
        activity_subjectText = "";
    }
    if (document.getElementById("ebiz-create-activity-warning") != null) {
        $("#ebiz-create-activity-warning").hide();
    }
    if (document.getElementById("clientside-success-wrapper") != null) {
        $("#clientside-success-wrapper").hide();
    }
}


//Load the activities datatable based on the search options
function retrieveActivities() {
    loadVariables();

    if ($(".ebiz-activities-list").length > 0) {
        $("#loading-image").show();
    }
    var activitiesTable = $(".ebiz-activities-list").DataTable({
        bFilter: false,
        destroy: true,
        processing: false,
        serverSide: true,
        paging: true,
        sDom: "lptip",
        lengthMenu: [ 20, 50, 100 ],
        language: {
            lengthMenu: datatables_lengthMenu,
            zeroRecords: datatables_zeroRecords,
            info: datatables_info,
            infoEmpty: datatables_infoEmpty,
            infoFiltered: datatables_infoFiltered,
            paginate: {
                previous: datatables_previousPage,
                next: datatables_nextPage
            }
        },
        ajax: {
            type: "POST",
            url: activity_talentAPIUrl + "/ActivitiesList",
            data: {
                "CustomerActivitiesHeaderID": "",
                "CustomerNumber": activity_customerNumber,
                "TemplateID": activity_selectedTemplateId,
                "ActivityUserName": activity_selectedAgentName,
                "ActivityDate": activity_selectedDate,
                "ActivitySubject": activity_subjectText,
                "ActivityStatus": activity_selectedStatus,
                "SessionID": activity_sessionId
            },
            error: function (error) {
                handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
                $("#loading-image").hide();
            },
            dataSrc: function (msg) {
                var viewModel = msg.dataList;
                if (viewModel != undefined) {
                    $(".ebiz-activities-list").show();
                } else {
                    $(".ebiz-activities-list").hide();
                }
                $("#loading-image").hide();
                return viewModel;
            }
        },
        columns: [
            { "data": 'CustomerActivitiesHeaderID', visible: false, className: "ebiz-unique-id" },
            {
                "data": "CustomerNumber",
                "mRender": function (data, type, row){
                    return row.CustomerNumber + " " + row.CustomerName;
                }
            },
            { "data": "TemplateID", visible: false, className: "ebiz-activity-template-id" },
            {
                "data": "ActivityUserName",
                "mRender": function (data, type, row) {
                    return row.DescriptiveUserName;
                }
            },
            { "data": "ActivityStatus" },
            { 
                "data": "ActivityDate",
                "mRender": function (data, type, row){
                    return row.FormattedDate; 
                }
            },
            { "data": "ActivitySubject" },
            {
                "mRender": function (data, type, row) {
                    var editLink = "<a href='ActivitiesEdit.aspx?TemplateId=" + row.TemplateID + "&Id=" + row.CustomerActivitiesHeaderID + "'><i class='fa fa-pencil' aria-hidden='true' title='Edit'></i></a>";
                    var deleteLink = "<a href='#' onclick='deleteActivity(" + row.CustomerActivitiesHeaderID + ")'><i class='fa fa-times' aria-hidden='true' title='Delete'></i></a>";
                    return editLink + " " + deleteLink;
                }, "orderable": false
            }
        ],
        initComplete: function (settings, json) {
            //once the table has initialised set the data-title attribute against each td tag.
            $(".ebiz-activities-list thead tr th").each(function (index, element) {
                index += 1;
                $(".ebiz-activities-list tbody tr td:nth-child(" + index + ")").attr("data-title", $(this).attr("data-title"));
            });
        }
    }); 
}


//Show the "cannot create activity" warning box
function cannotCreateActivity() {
    if (document.getElementById("ebiz-create-activity-warning") != null) {
        $("#ebiz-create-activity-warning").show();
    }
    return false;
}


//Delete the activity
function deleteActivity(CustomerActivitiesHeaderID) {
    loadVariables();

    if (document.getElementById("hdfAlertifyDeleteActivityTitle") != null) { alertify_title = document.getElementById("hdfAlertifyDeleteActivityTitle").value; }
    if (document.getElementById("hdfAlertifyDeleteActivityMessage") != null) { alertify_message = document.getElementById("hdfAlertifyDeleteActivityMessage").value; }

    alertify.confirm(alertify_title, alertify_message,
        function () {
            $("#loading-image").show();
            var activity = {
                SessionID: activity_sessionId,
                CustomerActivitiesHeaderID: CustomerActivitiesHeaderID
            };
            $.ajax({
                url: activity_talentAPIUrl + "/ActivitiesList?" + $.param(activity),
                type: "DELETE",
                dataType: "json",
                cache: false,
                success: function () {
                    retrieveActivities();
                    $("#clientside-success-wrapper").show();
                    $("#loading-image").hide();
                },
                error: function (error) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
                    $("#clientside-success-wrapper").hide();
                    $("#loading-image").hide();
                }
            });
        },
        function () { $("#loading-image").hide(); })
    .set('labels', { ok: alertify_okText, cancel: alertify_cancelText });
}


//Delete an activity file on an existing activity based on the given activity file attachment id, template id and file name
//Remove the HTML element based on the ItemIndex value
function deleteActivityFile(FileAttachmentID, TemplateID, FileName, ItemIndex) {
    loadVariables();

    if (document.getElementById("hdfAlertifyDeleteFileTitle") != null) { alertify_title = document.getElementById("hdfAlertifyDeleteFileTitle").value; }
    if (document.getElementById("hdfAlertifyDeleteFileMessage") != null) { alertify_message = document.getElementById("hdfAlertifyDeleteFileMessage").value; }

    alertify.confirm(alertify_title, alertify_message,
        function () {
            $("#loading-image").show();
            var fileAttachment = {
                TemplateID: TemplateID,
                SessionID: activity_sessionId,
                ActivityFileAttachmentID: FileAttachmentID,
                ActivityFileName: FileName
            };
            $.ajax({
                url: activity_talentAPIUrl + "/ActivitiesFile/DeleteFileAttachment?" + $.param(fileAttachment),
                type: "DELETE",
                dataType: "json",
                cache: false,
                success: function () {
                    $("#ebiz-activity-file-details-wrapper-" + ItemIndex).remove();
                    $("#loading-image").hide();
                },
                error: function (error) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
                    $("#clientside-success-wrapper").show();
                    $("#loading-image").hide();

                }
            });
        },
        function () { $("#loading-image").hide(); })
    .set('labels', { ok: alertify_okText, cancel: alertify_cancelText });
}


//Add an activity comment to an existing activity based on the given activity header id
function createActivityComment(CustomerActivitiesHeaderID) {
    loadVariables();

    var wrapperElement = "#ebiz-edit-activity-comment-wrapper-" + activity_commentItemIndex;
    $("#loading-image").show();
    var comment = {
        SessionID: activity_sessionId,
        CustomerActivitiesHeaderID: CustomerActivitiesHeaderID,
        ActivityCommentText: activity_commentText,
        ActivityCommentItemIndex: activity_commentItemIndex
    };
    $.ajax({
        url: activity_talentAPIUrl + "/ActivitiesEdit/CreateComment",
        type: "POST",
        dataType: "html",
        data: $.param(comment),
        cache: false,
        success: function (data) {
            $(".ebiz-edit-activity-comment-wrap").append(data);
            document.getElementById("hdfActivityFileItemIndex").value = parseInt(activity_commentItemIndex) + 1;
            document.getElementById("txtAddComment").value = "";
            $(wrapperElement).addClass("ebiz-comment-highlight");
            $(wrapperElement).on("animationend", function () {
                $(wrapperElement).removeClass("ebiz-comment-highlight");
            });
            $("#loading-image").hide();
        },
        error: function (error) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
            $("#clientside-success-wrapper").show();
            $("#loading-image").hide();
        }
    });
}


//Update an activity comment on an existing activity based on the given activity header id and comment id
//Use the view model to alter the visible HTML element
function updateActivityComment(CustomerActivitiesHeaderID, CommentID, ItemIndex) {
    loadVariables();

    var wrapperElement = "#ebiz-edit-activity-comment-wrapper-" + ItemIndex;
    var commentText = "";
    if ($(".ebiz-edit-activity-comment-text-" + CommentID) != null) { commentText = $(".ebiz-edit-activity-comment-text-" + CommentID).text(); }
    var comment = {
        SessionID: activity_sessionId,
        CustomerActivitiesHeaderID: CustomerActivitiesHeaderID,
        ActivityCommentID : CommentID,
        ActivityCommentText: commentText
    };
    $("#loading-image").show();
    $.ajax({
        url: activity_talentAPIUrl + "/ActivitiesEdit/UpdateComment",
        type: "POST",
        dataType: "json",
        data: $.param(comment),
        cache: false,
        success: function (data) {
            //Return view model with updated user, time and date
            $(wrapperElement + " .ebiz-edit-activity-comment-agent").text(data.ActivityDescriptiveUserName);
            $(wrapperElement + " .ebiz-edit-activity-comment-date").text(data.ActivityCommentUpdatedDate);
            $(wrapperElement + " .ebiz-edit-activity-comment-time").text(data.ActivityCommentUpdatedTime);
            $(wrapperElement + " .ebiz-edit-activity-comment-hidden-text").text(commentText);
            $(wrapperElement + " .ebiz-edit-activity-comment-blurb").text(data.ActivityCommentBlurb);
            $(wrapperElement).addClass('ebiz-comment-highlight');
            $(wrapperElement).on('animationend', function() {
                $(wrapperElement).removeClass('ebiz-comment-highlight');
            });
            $("#loading-image").hide();
        },
        error: function (error) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
            $("#clientside-success-wrapper").show();
            $("#loading-image").hide();
        }
    });
}


//Delete an activity comment on an existing activity based on the given comment id
//Remove the HTML element based on the ItemIndex value
function deleteActivityComment(CommentID, ItemIndex) {
    loadVariables();

    if (document.getElementById("hdfAlertifyDeleteCommentTitle") != null) { alertify_title = document.getElementById("hdfAlertifyDeleteCommentTitle").value; }
    if (document.getElementById("hdfAlertifyDeleteCommentMessage") != null) { alertify_message = document.getElementById("hdfAlertifyDeleteCommentMessage").value; }

    alertify.confirm(alertify_title, alertify_message,
        function () {
            $("#loading-image").show();
            var comment = {
                SessionID: activity_sessionId,
                ActivityCommentID: CommentID
            };
            $.ajax({
                url: activity_talentAPIUrl + "/ActivitiesEdit/DeleteComment?" + $.param(comment),
                type: "DELETE",
                dataType: "json",
                cache: false,
                success: function () {
                    $("#ebiz-edit-activity-comment-wrapper-" + ItemIndex).remove();
                    $("#loading-image").hide();
                },
                error: function (error) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
                    $("#clientside-success-wrapper").show();
                    $("#loading-image").hide();
                }
            });
        },
        function () { $("#loading-image").hide(); })
    .set('labels', { ok: alertify_okText, cancel: alertify_cancelText });
}


//Undo the edited changes the user has made to the comment based on the item index
function undoActivityCommentChanges(ItemIndex) {
    var wrapperElement = "#ebiz-edit-activity-comment-wrapper-" + ItemIndex;
    var originalComement = $(wrapperElement + " .ebiz-edit-activity-comment-hidden-text").text();
    $(wrapperElement + " .ebiz-edit-activity-comment-text").text(originalComement);
}


//Fine-Uploader code to upload file attachments
function createUploader() {
    var uploader = new qq.FineUploader({
        element: document.getElementById('fine-uploader'),
        debug: true,
        request: {
            endpoint: activity_talentAPIUrl + "/ActivitiesFile"
        },
        cors: {
            expected: true
        },
        multiple: true,
        validation: {
            allowedExtensions: activity_allowedFileTypes,
            sizeLimit: parseInt(activity_maxFileUploadSize)
        },
        showMessage: function (message) {
            return alert(activity_invalidFileMessage);
        },
        callbacks: {
            onSubmit: function (id, fileName) {
                var params = {
                    SessionID: activity_sessionId,
                    ActivityFileDescription: document.getElementById("txtFileDescription").value,
                    TemplateID: activity_editingTemplateId,
                    CustomerActivitiesHeaderID: activity_customerActivityHeaderId,
                    ActivityFileItemIndex: activity_fileItemIndex
                };
                this.setParams(params);
            },
            onComplete: function (id, fileName, responseJSON) {
                if (responseJSON.success == true) {
                    var wrapperElement = "#ebiz-activity-file-details-wrapper-" + activity_fileItemIndex;
                    document.getElementById("hdfActivityFileItemIndex").value = parseInt(activity_fileItemIndex) + 1;
                    document.getElementById("txtFileDescription").value = "";
                    $(".ebiz-activity-file-details-outer-wrapper").append(responseJSON.HTMLContent);
                    $(wrapperElement).addClass("ebiz-comment-highlight");
                    $(wrapperElement).on("animationend", function () {
                        $(wrapperElement).removeClass("ebiz-comment-highlight");
                    });
                }
            },
            onError: function (id, name, errorReason, xhrOrXdr) {
                alert(qq.format("Error on file number {} - {}.  Reason: {}", id, name, errorReason));
            }
        }
    });
}