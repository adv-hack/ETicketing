//Page Load - perform the customer search if there is a "search" querystring.
//This usually comes from the off-canvas searching.
$(document).ready(function () {
    var qs = GetQueryStringParams("Type");
    if (qs == "Search") {
        ProcessCustomerSearch();
    }
});


//Get the given querystring value of the current URL
function GetQueryStringParams(sParam) {
    var sPageURL = window.location.search.substring(1);
    var sURLVariables = sPageURL.split('&');
    for (var i = 0; i < sURLVariables.length; i++) {
        var sParameterName = sURLVariables[i].split('=');
        if (sParameterName[0] == sParam) {
            return sParameterName[1];
        }
    }
}

function ProcessCustomerSearch() {
    try {        // destroy company table to remove pagination buttons & information
        var companyTable = $(".ebiz-company-search-results").DataTable();
        companyTable.destroy();
        $(".ebiz-company-search-results").hide();
        /// errors if all fields and parentCompanyNumber is empty
        var fieldPopulated = false;
        $('.customer-details').each(function (index) {
            if ($(this).val().trim().length != 0) {
                fieldPopulated = true;
            }
        });
        if (!fieldPopulated) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", document.getElementById('hdfNoSearchCriteria').value);
            return false;
        }
        $("#loading-image").show();
        var customerTable = $('.ebiz-customer-search-results').DataTable({
            bFilter: false,
            destroy: true,
            processing: true,
            serverSide: true,
            info: false,
            bLengthChange: document.getElementById('hdfCustomerSearchChangePageSize').value,
            iDisplayLength: 10,
            lengthMenu: [10, 25, 50],
            language: {
                lengthMenu: document.getElementById('hdfCustomerSearchLengthMenuText').value,
                infoFiltered: ""
            },
            columnDefs: [{
                orderable: false,
                targets: [9, 10]
            }],
            rowCallback: function (row, data, index)
            {
                var searchType = document.getElementById('hdfSearchType').value;
                if (searchType == "Basket") {
                    $('td:eq(0)', row).html('<a onClick="updateCustomerOnBasket(\'' + data.CustomerNo + '\');">' + data.CustomerNo + '</a>');
                    $('td:eq(9)', row).html('<a onClick="updateCustomerOnBasket(\'' + data.CustomerNo + '\');">Update</a>');
                } else if (searchType == "Contacts") {
                    $('td:eq(0)', row).html('<a onClick="AddCompanyContact(\'' + data.CustomerNo + '\');">' + data.CustomerNo + '</a>');
                    $('td:eq(9)', row).html('<a onClick="AddCompanyContact(\'' + data.CustomerNo + '\');">Select</a>');
                } else {
                    $('td:eq(0)', row).html('<a onClick="LoginCustomer(\'' + data.CustomerNo + '\', false);">' + data.CustomerNo + '</a>');
                    $('td:eq(9)', row).html('<a onClick="LoginCustomer(\'' + data.CustomerNo + '\', true);">Update</a>');
                }

                $('td:eq(1)', row).html(data.ContactForename + ' ' + data.ContactSurname); // customer name
                $('td:eq(2)', row).html(formatAddress(data.AddressLine1, data.AddressLine2, data.AddressLine3, data.AddressLine4)); // address

                $('td:eq(5)', row).html(data.DateBirth);

                //phone numbers
                var phoneNumber1 = data.PhoneNumber1.toString().trim()
                var phoneNumber2 = data.PhoneNumber2.toString().trim()
                var phoneNumber3 = data.PhoneNumber3.toString().trim()

                //Remove duplicates and repostion
                if (phoneNumber1 == phoneNumber2) {
                    phoneNumber2 = "";
                }
                if (phoneNumber1 == phoneNumber3 || phoneNumber2 == phoneNumber3) {
                    phoneNumber3 = "";
                }
                if (phoneNumber1.trim().length == 0) {
                    if (phoneNumber2.trim().length > 0) {
                        phoneNumber1 = phoneNumber2;
                        phoneNumber2 = "";
                    } else if (phoneNumber3.trim().length > 0) {
                        phoneNumber1 = phoneNumber3;
                        phoneNumber3 = "";
                    }
                }
                if (phoneNumber2.trim().length == 0) {
                    if (phoneNumber3.trim().length > 0) {
                        phoneNumber2 = phoneNumber3;
                        phoneNumber3 = "";
                    }
                }
                //Multiple Phone Numbers display in prompt
                if (phoneNumber2.trim().length > 0) {
                    var phoneNumberString = "";
                    phoneNumberString = phoneNumberString + '<li>' + phoneNumber1 + '</li>';
                    phoneNumberString = phoneNumberString + '<li>' + phoneNumber2 + '</li>';
                    if (phoneNumber3.trim().length > 0) {
                        phoneNumberString = phoneNumberString + '<li>' + phoneNumber3 + '</li>';
                    }
                    $('td:eq(4)', row).html(phoneNumber1 + '<a data-toggle="phone-dd-' + data.CustomerNo + '"><i class="fa fa-eye" aria-hidden="true"></i></a> ' +
                        '<div id="phone-dd-' + data.CustomerNo + '" class="dropdown-pane" data-dropdown data-auto-focus="true"><ul class="no-bullet">' + phoneNumberString + '</ul></div>');
                } else {
                    // Single unique phone number
                    $('td:eq(4)', row).html(phoneNumber1);
                }

                //memberships
                var memberships = data.MembershipNumber.split(",");
                if (memberships.length > 1) {
                    var index;
                    var membershipsString = "";
                    for (index = 0, len = memberships.length; index < len; ++index) {
                        membershipsString = membershipsString + '<li>' + memberships[index].trim() + '</li>';
                    }

                    //format memberships
                    $('td:eq(6)', row).html('<a data-toggle="membership-dd-' + data.CustomerNo + '"><i class="fa fa-eye" aria-hidden="true"></i> View</a> ' +
                        '<div id="membership-dd-' + data.CustomerNo + '" class="dropdown-pane" data-dropdown data-auto-focus="true"><ul class="no-bullet">' + membershipsString + '</ul></div>');
                }


                if (searchType == "ALL") {
                    $('td:eq(10)', row).html('<a onClick="printAddressLabel(\'' + data.CustomerNo + '\');"><i class="' + document.getElementById('hdfPrintAddressLabelItem').value + '"></i></a>'); // print address label
                }
                else
                {
                    $('td:eq(10)', row).html('&nbsp');
                }
            },
            ajax: {
                type: "POST",
                url: document.getElementById('hdfTalentAPIAddress').value + "/CustomerSearch/Post",
                data: {
                    "ContactNumber": (document.getElementById('txtContactNumber') == null ? '' : document.getElementById('txtContactNumber').value),
                    "ContactForename": (document.getElementById('txtForename') == null ? '' : document.getElementById('txtForename').value),
                    "ContactSurname": (document.getElementById('txtSurname') == null ? '' : document.getElementById('txtSurname').value),
                    "PassportNumber": (document.getElementById('txtPassportNumber') == null ? '' : document.getElementById('txtPassportNumber').value),
                    "AddressLine1": (document.getElementById('txtAddressLine1') == null ? '' : document.getElementById('txtAddressLine1').value),
                    "AddressLine2": (document.getElementById('txtAddressLine2') == null ? '' : document.getElementById('txtAddressLine2').value),
                    "AddressLine3": (document.getElementById('txtAddressLine3') == null ? '' : document.getElementById('txtAddressLine3').value),
                    "AddressLine4": (document.getElementById('txtAddressLine4') == null ? '' : document.getElementById('txtAddressLine4').value),
                    "PostCode": (document.getElementById('txtAddressPostCode') == null ? '' : document.getElementById('txtAddressPostCode').value),
                    "PhoneNumber": (document.getElementById('txtPhoneNumber') == null ? '' : document.getElementById('txtPhoneNumber').value),
                    "WebAddress": (document.getElementById('txtEmail') == null ? '' : document.getElementById('txtEmail').value),
                    "AgentType": (document.getElementById('hdfAgentType') == null ? '' : document.getElementById('hdfAgentType').value),
                    "AgentLoginID": (document.getElementById('hdfAgentLoginID') == null ? '' : document.getElementById('hdfAgentLoginID').value),
                    "SearchResultLimit": (document.getElementById('hdfCustomerSearchLimit') == null ? '' : document.getElementById('hdfCustomerSearchLimit').value),
                    "SessionID": (document.getElementById('hdfSessionID') == null ? '' : document.getElementById('hdfSessionID').value)
                },
                error: function (error) {
                    var err = error.message;
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.message);
                    $("#loading-image").hide();
                },
                dataSrc: function (msg) {
                    var viewModel = msg.dataList[0];
                    if (viewModel) {
                        if (viewModel.Error && viewModel.Error.HasError) {
                            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", viewModel.Error);
                            $(".ebiz-customer-search-results").hide();
                        } else {
                            talentClubs = viewModel.Clubs;
                            $(".ebiz-customer-search-results").show();
                            $(document).scrollTop($(".ebiz-customer-selection-results-wrap").offset().top);
                            //Store the record count for each new search
                            if (msg.draw == 1) {
                                document.getElementById('hdfRecordsFiltered').value = msg.recordsFiltered;
                            } else {
                                msg.recordsFiltered = document.getElementById('hdfRecordsFiltered').value;
                            }
                        }
                    }
                    $("#loading-image").hide();
                    return viewModel.Customers;
                }
            },
            initComplete: function (settings, json) {
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityName')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityName').value.toString().toLowerCase() =="false") {
                    customerTable.column(1).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityAddress')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityAddress').value.toString().toLowerCase() == "false") {
                    customerTable.column(2).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPostcode')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPostcode').value.toString().toLowerCase() == "false") {
                    customerTable.column(3).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPhoneNumber')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPhoneNumber').value.toString().toLowerCase() == "false") {
                    customerTable.column(4).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityDOB')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityDOB').value.toString().toLowerCase() == "false") {
                    customerTable.column(5).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityMembershipNo')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityMembershipNo').value.toString().toLowerCase() == "false") {
                    customerTable.column(6).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPassport')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityPassport').value.toString().toLowerCase() == "false") {
                    customerTable.column(7).visible(false);
                }
                if ((document.getElementById('hdfCustomerSearchCustomerColumnVisibilityEmail')) && document.getElementById('hdfCustomerSearchCustomerColumnVisibilityEmail').value.toString().toLowerCase() == "false") {
                    customerTable.column(8).visible(false);
                }
            },
            columns: [
                { "data": 'CustomerNo' },
                { "data": 'ContactSurname' },
                { "data": 'AddressLine1' },
                { "data": 'PostCode' },
                { "data": 'PhoneNumber1' },
                { "data": 'DateBirth' },
                { "data": 'MembershipNumber' },
                { "data": 'PassportNumber' },
                { "data": 'EmailAddress' },
                { "data": null },
                { "data": null }
            ]
        });
    }
    catch (error) {
        handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.message);
        $("#loading-image").hide();
    }
}

function updateCustomerOnBasket(customerNumber) {
    // from basket
    var parameters = {
        SessionID: getSessionID(),
        BasketID: document.getElementById('hdfBasketDetailID').value,
        ProductType: document.getElementById('hdfProductType').value,
        ProductCode: document.getElementById('hdfProductCode').value,
        ProductSubType: document.getElementById('hdfProductSubType').value,
        PackageId: document.getElementById('hdfPackageId').value,
        PriceCode: document.getElementById('hdfPriceCode').value,
        PriceBand: document.getElementById('hdfPriceBand').value,
        FulfilmentMethod: document.getElementById('hdfFulfilmentMethod').value,
        Seat: document.getElementById('hdfSeat').value,
        BulkSalesId: document.getElementById('hdfBulkSalesId').value,
        BulkSalesQuantity: document.getElementById('hdfBulkSalesQuantity').value,
        ReturnURL: document.getElementById('hdfReturnURL').value,
        NewUser: customerNumber,
        OriginalUser: document.getElementById('hdfOriginalUser').value
    };
    $.ajax({
        type: "POST",
        url: "CustomerSelection.aspx/UpdateCustomerBasket",
        data: JSON.stringify(parameters),
        contentType: "application/json",
        dataType: "json",
        cache: false,
        success: function (result) {
            forwardUrl = result.d;
            window.location = document.getElementById('hdfRootURL').value + result.d;
        },
        error: function (xhr, status) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", status + '  ' + xhr.responseText);
            $("#loading-image").hide();
        }
    });
}

function LoginCustomer(customerNumber, doUpdate) {
    var club = getClubByClubBusinessUnit();
    var returnUrl = GetQueryStringParams("ReturnUrl");
    returnUrl = returnUrl != undefined ? returnUrl : "";
    var parameters = {
        SessionID: getSessionID(),
        CustomerNo: customerNumber,
        Update: doUpdate,
        ReturnUrl: returnUrl
    };
    $.ajax({
        type: "POST",
        url: "CustomerSelection.aspx/GetEncodedQueryStringParameters",
        data: JSON.stringify(parameters),
        contentType: "application/json",
        dataType: "json",
        cache: false,
        success: function (result) {
            window.location = document.getElementById('hdfRootURL').value + result.d;
        },
        error: function (xhr, status) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", status + '  ' + xhr.responseText);
            $("#loading-image").hide();
        }
    });
}


function AddCompanyContact(customerNumber) {
    var contact = {
        SessionID: document.getElementById('hdfSessionID').value,
        CustomerNumber: customerNumber,
        CompanyNumber: document.getElementById('hdfCompanyNumber').value,
        AgentName: document.getElementById('hdfAgentLoginID').value
    };
    contact = $.param(contact);

    $.ajax({
        type: "POST",
        url: document.getElementById('hdfTalentAPIAddress').value + "/CustomerCompany/AddContact",
        dataType: "json",
        data: contact,
        cache: false,
        success: function (viewModel) {
            if (viewModel.Error && viewModel.Error.HasError) {
                handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", viewModel.Error);
                $("#loading-image").hide();
            }
            else {
                window.location = document.getElementById('hdfRootURL').value +  "PagesPublic/CRM/CompanyContacts.aspx?source=companysearch&CompanyNumber=" + document.getElementById('hdfCompanyNumber').value;
            }
        },
        error: function (xhr, status) {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", xhr.status + '  ' + xhr.statusText);
            $("#loading-image").hide();
        }
    });
}

function formatAddress(address1, address2, address3, address4) {
    var address = '';
    if (address1.trim().length > 0)
        address = address + address1.trim() + ' ';
    if (address2.trim().length > 0)
        address = address + address2.trim() + ' ';
    if (address3.trim().length > 0)
        address = address + address3.trim() + ' ';
    if (address4.trim().length > 0)
        address = address + address4.trim() + ' ';

    return address;
}

function getClubByClubBusinessUnit() {
    var club;
    jQuery.each(talentClubs, function () {
        if (this.CLUB_CODE == document.getElementById('hdfClubBusinessUnit').value) {
            club = this;
        }
    });

    return club;
}

function printAddressLabel(CustomerNumber) {
    var printInput = {
        CustomerNumber: CustomerNumber,
        SessionID: getSessionID()
    };
    //web api call
    $.ajax({
        type: "POST",
        url: document.getElementById('hdfTalentAPIAddress').value + "/AddressLabel",
        dataType: "json",
        data: $.param(printInput),
        cache: false,
        success: function (msg) {
            var status = msg.Status;
            if (status.HasError) {
                handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", status.ErrorMessage + '  (' + status.ErrorFlag + ')');
                $("#loading-image").hide();
            }
            else {
                alert("Print successful");
            }
        },
        error: function (xhr, status) {
            alert('Print failed: ' + status + '  ' + xhr.responseText);
        }

    });
}

$(document).ajaxComplete(function () {
    $(document).foundation();
});



