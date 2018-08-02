function processCompanySearch() {
    try {
        //add parent number to search for subsidaries
        var parentCompanyNumber;
        var showSubsidaries = Boolean(document.getElementById('hdfShowSubsidaries').value);
        if (showSubsidaries) {
            parentCompanyNumber = document.getElementById('hdfParentCompanyNumber').value;
        }
        // errors if all fields and parentCompanyNumber is empty
        var fieldPopulated = false;
        $('.company-details').each(function (index) {
            if ($(this).val().trim().length != 0) {
                fieldPopulated = true;
            }
        });
        if (!fieldPopulated && (parentCompanyNumber == undefined || parentCompanyNumber.length == 0))
        {
            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", document.getElementById('hdfNoSearchCriteria').value);
            return false;
        }
        // destroy customer table to remove pagination buttons & information
        var customerTable = $(".ebiz-customer-search-results").DataTable();
        customerTable.destroy();
        $(".ebiz-customer-search-results").hide();
        var colDisplay = document.getElementById('hdfColumnDisplay').value;
        var companySearchTable = $(".ebiz-company-search-results").DataTable({
            bFilter: false,
            destroy: true,
            processing: true,
            serverSide: true,
            info: false,
            bLengthChange: document.getElementById('hdfCompanySearchChangePageSize').value,
            iDisplayLength: document.getElementById('hdfCompanySearchPageSize').value,
            lengthMenu: [10, 25, 50],
            language: {
                lengthMenu: document.getElementById('hdfCompanySearchLengthMenuText').value,
                infoFiltered: ""
            },
            columnDefs: [{ 
                orderable: false, 
                targets: [3,5,6,7],
                }],
            ajax: {
                type: "POST",
                url: document.getElementById('hdfTalentAPIAddress').value + "/CompanySearch/Post",
                data: {
                    "ParentCompanyNumber": parentCompanyNumber,
                    "ChildCompanyNumber": (document.getElementById('hdfChildCompanyNumber') == null ? '' : document.getElementById('hdfChildCompanyNumber').value), //not searchable
                    "SearchType": (document.getElementById('hdfSearchType') == null ? '' : document.getElementById('hdfSearchType').value), // not searchable
                    "CompanyName": (document.getElementById('txtCompanyName') == null ? '' : document.getElementById('txtCompanyName').value),
                    "AddressLine1": (document.getElementById('txtCompanyAddressLine1') == null ? '' : document.getElementById('txtCompanyAddressLine1').value),
                    "PostCode": (document.getElementById('txtCompanyPostCode') == null ? '' : document.getElementById('txtCompanyPostCode').value),
                    "WebAddress": (document.getElementById('txtCompanyWebAddress') == null ? '' : document.getElementById('txtCompanyWebAddress').value),
                    "TelephoneNumber1": (document.getElementById('txtCompanyTelephoneNumber') == null ? '' : document.getElementById('txtCompanyTelephoneNumber').value),
                    "SessionID": (document.getElementById('hdfSessionID') == null ? '' : document.getElementById('hdfSessionID').value)
                },
                error: function (error) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", error.responseText);
                },
                dataSrc: function (msg) {
                    var viewModel = msg.dataList[0];
                    if (viewModel != undefined) {
                        if (viewModel.Error && viewModel.Error.HasError) {
                            handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", viewModel.Error);
                            $(".ebiz-company-search-results").hide();
                        } else {
                            $(document).scrollTop($(".ebiz-customer-selection-results-wrap").offset().top);
                            //Store the record count for each new search
                            if (msg.draw == 1) {
                                document.getElementById('hdfRecordsFiltered').value = msg.recordsFiltered;
                            } else {
                                msg.recordsFiltered = document.getElementById('hdfRecordsFiltered').value;
                            }

                            $(".ebiz-company-search-results").show();
                        }
                    }
                    return viewModel.Companies;
                }
            },
            rowCallback: function (row, data, index) {
                if (data != undefined) {
                    // ColDisplay = 1 (Select Column is Visible)
                    // ColDisplay = 2 (Update and Contacts Columns are Visible)
                    // ColDisplay = 3 (Select and Contacts Columns are Visible)
                    if (colDisplay == 1) {
                        companySearchTable.columns('.column-show-update').visible(false);
                        companySearchTable.columns('.column-show-contacts').visible(false);
                        var returnToUrl = document.getElementById('hdfReturnToAddress').value
                        if (returnToUrl == "") {
                            var showParentCompanySearch = Boolean(document.getElementById('hdfShowParentCompanySearch').value);
                            if (showParentCompanySearch) {
                                var addCompanyToNull = Boolean(document.getElementById('hdfAddCompanyToNull').value);
                                if (addCompanyToNull) {
                                    $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?ParentCompanyNumber=' + data.CompanyNumber + '&ParentCompanyName=' + data.CRMCompanyName + '&CompanyUpdatePageMode=Add&AddType=AddToNull&source=ParentSearch"> Select</a>'); // Customer select
                                } else {
                                    var addSubsidiaries = Boolean(document.getElementById('hdfAddSubsidiaries').value);
                                    if (addSubsidiaries)
                                    {
                                        var parentCompanyNumber = document.getElementById('hdfParentCompanyNumber').value;
                                        var parentCompanyName = document.getElementById('hdfParentCompanyName').value;
                                        $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?CompanyUpdatePageMode=Update&AddType=AddToNullSubsidiaries&source=ParentSearch&SubsidiaryCompanyNumber=' + data.CompanyNumber + '&SubsidiaryCompanyName=' + data.CRMCompanyName + '"> Select</a>'); // Customer select
                                    }
                                    else
                                    {
                                        $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?ParentCompanyNumber=' + data.CompanyNumber + '&CompanyUpdatePageMode=Update&AddType=ExistingParent&source=ParentSearch"> Select</a>'); // Customer select
                                    }
                                }

                            }
                            else {
                                $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=' + data.CompanyNumber + '&CompanyUpdatePageMode=Update&Source=companysearch"> Select</a>'); // Customer select
                            }
                        }
                        else {
                            $('td:eq(5)', row).html('<a href="' + returnToUrl + '?CompanyNumber=' + data.CompanyNumber + '"> Select</a>'); // Customer select                           
                        }
                    }

                    if (colDisplay == 2) {
                        companySearchTable.columns('.column-show-select').visible(false);
                        $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=' + data.CompanyNumber + '&CompanyUpdatePageMode=Update&Source=companysearch"> Update</a>'); // Customer update
                        $('td:eq(6)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyContacts.aspx?CompanyNumber=' + data.CompanyNumber + '&source=companysearch"> Contacts</a>'); // Customer contacts
                    }
                    if (colDisplay == 3) {
                        companySearchTable.columns('.column-show-update').visible(false);

                        $('td:eq(5)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyContacts.aspx?CompanyNumber=' + data.CompanyNumber + '&source=companysearch"> Contacts</a>'); // Customer contacts
                        $('td:eq(6)', row).html('<a href="' + document.getElementById('hdfRootURL').value + 'PagesPublic/CRM/CompanyUpdate.aspx?CompanyNumber=' + data.CompanyNumber + '&CompanyUpdatePageMode=Update&Source=companysearch"> Select</a>'); // Customer select 
                    }

                }
            },
            columns: [
                { "data": 'CompanyName' },
                { "data": 'AddressLine1' },
                { "data": 'PostCode' },
                { "data": 'Telephone' },
                { "data": 'WebAddress' },
                { "data": null },
                { "data": null },
                { "data": null }
            ]
        });
    }
    catch (err) {
        handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", err.message);
    }
}
