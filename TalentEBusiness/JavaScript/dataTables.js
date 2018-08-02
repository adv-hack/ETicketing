/*		DATACOLUMNS array format - ["index","column name","type","editable","visible","sortable","headerText","isPrimaryKey"]
		------------------------------------------------------------------------------------------------------------------------------
		1. index: table cell index
		2. column name: column name coming from DB
		3. type: type of table cell content --> possibilities are: text,checkbox,dropdown
		4. editable: if table cell is editable or not. making cell editable will allow you to double click on the table-cell and open it in the editable mode
		5. visible: if column is visible to user or not
		6. sortable: if sorting is allowed on column
		7. headerText: what to display in the header
		8. isPrimaryKey: if the column is a part of primary key
*/

var dataColumns, dataTables, jsonData, dataCols, columnNames, arrPageSizeNames, arrPageSizeValues, arrPageSize, pkIdx, rowCount;
/*	Note: (tableID)
 	Please take a note that we are currently having a single variable in the library which has a table name reference stored in it.
	Having said that we have a limitation at this point that library functions will only work for single dataTable grid(HTML table) avaialble on the page.
	We will use this ID at many places in the library further.
*/
var tableID;
var jsErrorMsg = "error occurred: ";
var jsValidatonErrMsg = "validation error";
var jsNoDataChangedMsg = "no data has been changed";
var jsUpdateErrorMsg = "Problem occurred while updating data to the server.Please contact administrator.";
var jsUpdateSuccessMsg = "Data updated successfully";
var APIUrl, APIUpdateURL;
var isCellDblClick;


$(document).ready(function () {
    try {
        init();
    }
    catch (ex) {
        alert('Error occurred when initializing the data grid: ' + ex.message);
    }
});

function formClicked() {
    closeAllTextBoxes("2");
}

function init() {
    try {
        // add a form click event as an alternate of onblur event
        $("form").click(function () { formClicked(); });
        isCellDblClick = false;

        if ($("#hdnJSErrMsg")) {
            jsErrorMsg = $("#hdnJSErrMsg").val();
        }
        if ($("#hdnJSValidationErrMsg")) {
            jsValidatonErrMsg = $("#hdnJSValidationErrMsg").val();
        }
        if ($("#hdnNoDataChanged")) {
            jsNoDataChangedMsg = $("#hdnNoDataChanged").val();
        }
        if ($("#hdnJSUpdateErrorMsg")) {
            jsUpdateErrorMsg = $("#hdnJSUpdateErrorMsg").val();
        }
        if ($("#hdnJSUpdateSuccessMsg")) {
            jsUpdateSuccessMsg = $("#hdnJSUpdateSuccessMsg").val();
        }
        rowCount = 0;
    }
    catch (ex) {
        alert(jsErrorMsg + ex.message);
    }
};

function prepareDBTablesAndColumns(objTables, objColumns) {
    var arrPair, colName, isEditable, isVisible, arrIdx, columnType, allowSorting, headerRow, headerText, isPrimaryKey;
    try {
        // reset the table
        resetConfigurations();

        // 1. Fill out dataTables object
        dataTables = new Array(objTables.length);
        for (i = 0; i < objTables.length; i++) {
            /* Retrieve attributes from hidden fields */
            arrIdx = i;
            // prepare an array content
            arrPair = new Array();
            arrPair.push(objTables[i].idx);
            arrPair.push(objTables[i].table);
            // add
            dataTables[i] = arrPair;
        }

        // 2. Fill out dataColumns object
        dataColumns = new Array(objColumns.length);
        dataCols = new Array(objColumns.length);
        columnNames = "";
        headerRow = "<thead><tr>";
        pkIdx = new Array();

        for (i = 0; i < objColumns.length; i++) {
            /* Retrieve attributes from hidden fields */
            arrIdx = i;
            colName = objColumns[i].dbColumn;
            headerText = objColumns[i].header;
            columnType = objColumns[i].colType;
            isEditable = (objColumns[i].edit == "1" ? true : false);
            isVisible = (objColumns[i].show == "1" ? true : false);
            allowSorting = (objColumns[i].sort == "1" ? true : false);
            isPrimaryKey = (objColumns[i].pk == "1" ? true : false);
            if (isPrimaryKey) {
                pkIdx.push(arrIdx);
            }
            // prepare array
            arrPair = new Array();
            arrPair.push(arrIdx);
            arrPair.push(colName);
            arrPair.push(columnType);
            arrPair.push(isEditable);
            arrPair.push(isVisible);
            arrPair.push(isVisible);
            arrPair.push(allowSorting);
            arrPair.push(headerText);
            arrPair.push(isPrimaryKey);

            dataColumns[i] = arrPair;
            dataCols[i] = { data: colName, orderable: allowSorting };
            columnNames = columnNames + colName + ",";
            headerRow += "<th class='" + (isVisible ? '' : 'hide') + "'>" + headerText + "</th>";
        }
        columnNames = columnNames.substring(0, columnNames.length - 1);

        //3. add header into the data table
        headerRow += "</tr></thead>";
        $("#" + tableID).html(headerRow);

        // 4. page size options
        arrPageSizeValues = [5, 10, 20, 50, 100, -1];
        arrPageSizeNames = [5, 10, 20, 50, 100, "All"];
        arrPageSize = [arrPageSizeValues, arrPageSizeNames];
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function resetConfigurations() {
    try {
        $("#" + tableID).html("");
        dataColumns = null;
        dataTables = null;
        jsonData = null;
        dataCols = null;
        columnNames = null;
        dataTables = null;
        arrPageSize = null;
        arrPageSizeValues = null;
        arrPageSizeNames = null;
        pkIdx = null;
        rowCount = 0;
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function getData() {
    var searchText, searchType;
    try {
        // clear the table
        $("#" + tableID + " tbody tr").remove();

        searchText = $.trim($("#txtSearch").val());
        searchType = $("#ddlSearchType").val();

        $('#' + tableID).dataTable({
            bFilter: false, // hide grid`s search bar
            destroy: true, // will destroy existing table(if any); allows you to re-click on the "search" button !!
            processing: true, //will display the text "processing when table-HTML is getting rendered"
            serverSide: true,
            pagingType: "full",// there are other possible options too !!
            lengthMenu: arrPageSize,
            ajax: {
                url: APIUrl,
                type: 'POST',
                data: {
                    "columnnames": columnNames,
                    "searchtext": searchText,
                    "searchtype": searchType
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert(jsErrorMsg + errorThrown);
                    return;
                }
            },
            "fnCreatedRow": fnPostRender,
            "fnDrawCallback": fnRemoveUnwantedColums,
            "fnInitComplete": fnInitComplete,
            columns: dataCols
        });
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function fnInitComplete(settings, json) {
    var searchText, records;
    try {
        records = this.fnGetData().length;
        if (records > 0) {
            $("#btnLocalReplace").attr("disabled", false);
            searchText = $.trim($("#txtSearch").val());
            $('#hdnSearchedText').val(searchText);
        }
        else {
            $("#btnLocalReplace").attr("disabled", "disabled");
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
}

function fnPostRender(nRow, aData, iDisplayIndex) {
    var idx;
    try {
        // set a unique id to each row
        nRow.setAttribute("rID", rowCount++);

        // Fix - double of click issue
        //$(nRow).attr("style", "height:100px");

        //1. add "primary key" to TR
        for (i = 0; i < pkIdx.length; i++) {
            nRow.setAttribute(dataColumns[pkIdx[i]][1], aData[dataColumns[pkIdx[i]][1]]);
        }

        //2.  add tab attribute(DB Table Name) to TR
        nRow.setAttribute("dbTable", aData["TABLE_NAME"]);
        //3.  add id attribute(column index) to TD
        for (i = 0; i < dataColumns.length; i++) {
            if (dataColumns[i][3]) //isEditable ??
            {
                idx = dataColumns[i][0];
                $('td:eq(' + idx + ')', nRow).attr("id", dataColumns[i][0]);

                //4. bind a function on table cell`s double click event
                $('td:eq(' + idx + ')', nRow).attr("ondblclick", "dblClicked(this);");
                $('td:eq(' + idx + ')', nRow).attr("onmousedown", "isCellDblClick = true;");
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function fnRemoveUnwantedColums() {
    var idx;
    try {
        // remove unwanted columns (we can later move this part out of this function
        for (i = 0; i < dataColumns.length; i++) {
            idx = parseInt(dataColumns[i][0]) + 1;
            if (!dataColumns[i][4]) //not visible
            {
                $("#" + tableID + " tr td:nth-child(" + idx + ")").remove();
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function closeAllTextBoxes(target) {
    var arrTxt, arrTxtArea;
    try {
        if (!target) {
            target = "1";
        }
        arrTxt = $("#" + tableID + " input[type='text'][rowid][colid]");
        if (arrTxt) {
            for (i = 0; i < arrTxt.length; i++) {
                fnFocusOff(arrTxt[i], false, target);
            }
        }
        arrTxtArea = $("#" + tableID + " textarea[rowid][colid]");
        if (arrTxtArea) {
            for (i = 0; i < arrTxtArea.length; i++) {
                fnFocusOff(arrTxtArea[i], false, target);
            }
        }
        isCellDblClick = false;
    }
    catch (ex) {
        showErrorMessage(ex);
    }
}

function dblClicked(tableCell) {
    var rowID, colID;
    var htnlElementValue, htnlElementID;
    try {
        // close any open textboxes
        closeAllTextBoxes();

        rowID = tableCell.parentNode.getAttribute("rID");
        colID = tableCell.getAttribute("id");
        htnlElementID = "txt_" + rowID + colID;
        htnlElementValue = $(tableCell).text();
        //add textbox/textarea
        element = getHtmlTextBoxOrTextArea(htnlElementValue);
        element.setAttribute("id", htnlElementID);
        element.setAttribute("rowid", rowID);
        element.setAttribute("colid", colID);
        element.setAttribute("style", "width:100%;");
        element.setAttribute("onkeydown", "fnKeyPressed(event,this);");
        if (!tableCell.getAttribute("orig")) {
            tableCell.setAttribute("orig", htnlElementValue);
        }
        tableCell.setAttribute("prev", htnlElementValue);
        $(tableCell).html(element);
        $("#txt_" + rowID + colID).focus();
        $("#msgBox").hide();
        isCellDblClick = false;
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function getHtmlTextBoxOrTextArea(value) {
    var element, elementType, inputType, textAreaRows;
    elementType = "input";
    inputType = "text";
    try {
        // Content has more than 100 characters then display textarea otherwise display a textbox
        if (value.length > 100) {
            //add textarea
            textAreaRows = value.length / 40; // This is based on assumption that single row can display around 40 characters, based on that we can estimate number of rows assigned to the textarea element as an attribute
            elementType = "textarea";
            element = document.createElement(elementType);
            element.setAttribute("rows", textAreaRows);
        }
        else {
            //add textbox
            element = document.createElement(elementType);
            element.setAttribute("type", inputType);
        }
        // set the value
        $(element).val(value);
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return element;
}

function fnKeyPressed(e, txtBox) {
    try {
        // if enter key is pressed
        if (e.keyCode == 13 || e.keyCode == 9) // enter(13) or tab(9)
        {
            fnFocusOff(txtBox, false, "2");
        }
        else if (e.keyCode == 27) // escape
        {
            fnFocusOff(txtBox, true);
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function fnFocusOff(txtBox, reset, target) {
    var rowID, colID, td;
    rowID = $(txtBox).attr("rowid");
    colID = $(txtBox).attr("colid");
    try {
        if (!isCellDblClick || target == "1") {
            td = $("#" + tableID + " tr[rID='" + rowID + "'] td[id='" + colID + "']");
            $(td).show();
            if (reset) {
                $(td).text($(td).attr("prev"));
                if ($(td).attr("orig") != $(td).attr("prev")) {
                    $(td).addClass("tc");
                }
                else {
                    $(td).removeClass("tc");
                }
            }
            else {
                $(td).text(txtBox.value);
                if ($(td).attr("prev") != txtBox.value && $(td).attr("orig") != txtBox.value) {
                    $(td).addClass("tc");
                }
                else if ($(td).attr("orig") == txtBox.value) {
                    $(td).removeClass("tc");
                }
            }
            $(txtBox).remove();
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function validate() {
    var retVal = true;
    try {

    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};

function save() {
    var arrTR, arrTD, pervRowID, rowID, dbTableName, oldVal, newVal, colName, colId, jsonItem, hasChanges = false, hasRowChanges, pKeyCol;
    var listOfElementsChanged = "";
    try {
        if (validate()) {
            // check for any open editable cells to close
            formClicked();

            // reset
            jsonData = [];

            arrTR = $("#" + tableID + " td[id][orig]").parent();

            for (i = 0; i < arrTR.length; i++) {
                rowID = arrTR[i].getAttribute("rID");
                dbTableName = arrTR[i].getAttribute("dbTable");

                arrTD = $("#" + tableID + " tr[rID='" + rowID + "'] td[orig]");

                if (arrTD) {
                    jsonItem = {};
                    //add PK dynamically
                    for (j = 0; j < pkIdx.length; j++) {
                        pKeyCol = dataColumns[pkIdx[j]][1];
                        jsonItem[pKeyCol] = $(arrTR[i]).attr(pKeyCol);
                    }
                    jsonItem["dbtable"] = dbTableName;
                }
                hasRowChanges = false;
                for (j = 0; j < arrTD.length; j++) {
                    oldVal = arrTD[j].getAttribute("orig");
                    colId = parseInt(arrTD[j].getAttribute("id"));
                    colName = dataColumns[colId][1];
                    newVal = $(arrTD[j]).text();
                    if (oldVal != newVal) {
                        jsonItem[colName] = newVal;
                        hasRowChanges = true;
                    }
                }
                if (hasRowChanges) {
                    hasChanges = true;
                    jsonData.push(jsonItem);
                }
            }

            if (hasChanges) {
                //send the data
                var response = $.ajax({
                    type: "POST",
                    async: false,
                    url: APIUpdateURL,
                    data: JSON.stringify(jsonData),
                    contentType: 'application/json; charset=utf-8',
                    done: showMessage(jsUpdateSuccessMsg,1),
                });

                if (response.status == "200") {
                    // refresh grid data
                    if ($.trim($("#txtReplace").val()) != "") {
                        $("#txtSearch").val($.trim($("#txtReplace").val()));
                        $("#hdnSearchedText").val($.trim($("#txtReplace").val()));
                        $("#txtReplace").val("");
                    }
                    getData();
                }
                else {
                    showMessage(jsUpdateErrorMsg, 2);
                }
            }
            else {
                alert(jsNoDataChangedMsg);
            }
        }
        else {
            alert(jsValidatonErrMsg);
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

/* 
    msg:    Error/Success message 
    type:   1: Success 2: Error 3: Warning
*/
function showMessage(msg, type) {
    $("#msgBox").show();
    $("#msgBox").html(msg);
    // reset 
    $("#msgBox").removeClass("success");
    $("#msgBox").removeClass("alert");
    $("#msgBox").removeClass("warning");
    // set style 
    if (type == 1) {
        $("#msgBox").addClass("success");
    }
    else if (type == 2) {
        $("#msgBox").addClass("alert");
    }
    else if (type == 3) {
        $("#msgBox").addClass("warning");
    }
};

function replaceTableCells(searchText, replaceText) {
    var arrTD;
    try {
        arrTD = $("#" + tableID + " td[id]:contains(" + searchText + ")");
        for (j = 0; j < arrTD.length; j++) {
            if (arrTD[j].textContent != replaceText) {
                if (!$(arrTD[j]).attr("orig")) {
                    $(arrTD[j]).attr("orig", arrTD[j].textContent);
                }
            }
        }
        $(arrTD).replaceText(eval("/" + getSearchText(searchText) + "/g"), replaceText);
        $(arrTD).each(function () {
            if ($(this).text() == $(this).attr("orig")) {
                $(this).removeClass("tc");
            }
            else {
                $(this).addClass("tc");
            }
        });
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

function onReplaceClicked() {
    var searchText, replaceText, searchTextOrig, replaceTextOrig;
    var arrSearches = [];
    try {
        //reset
        $("#msgBox").hide();
        searchTextOrig = $("#txtSearch").val().trim();
        replaceTextOrig = $("#txtReplace").val().trim();

        if (searchTextOrig == replaceTextOrig) {
            alert("Replace text cannot be same as search text.");
            $("#txtReplace").focus();
        }
        else {
            // 1. First character is in uppercase & rest in lowercase
            searchText = searchTextOrig.substring(0, 1).toUpperCase() + searchTextOrig.substring(1, searchTextOrig.length).toLowerCase();
            replaceText = replaceTextOrig.substring(0, 1).toUpperCase() + replaceTextOrig.substring(1, replaceTextOrig.length).toLowerCase();
            replaceTableCells(searchText, replaceText);
            arrSearches.push(searchText);

            // 2. All characters are in uppercase
            searchText = searchTextOrig.toUpperCase();
            replaceText = replaceTextOrig.toUpperCase();
            replaceTableCells(searchText, replaceText);
            arrSearches.push(searchText);

            // 3. All characters are in lowercase
            searchText = searchTextOrig.toLowerCase();
            replaceText = replaceTextOrig.toLowerCase();
            replaceTableCells(searchText, replaceText);
            arrSearches.push(searchText);

            // 4. Exact match case
            searchText = searchTextOrig;
            if (arrSearches.indexOf(searchText) == -1) {
                replaceText = replaceTextOrig;
                if (searchText.substring(0, 1) == searchText.substring(0, 1).toUpperCase() && replaceText.substring(0, 1) == replaceText.substring(0, 1).toLowerCase()) {
                    replaceText = replaceText.substring(0, 1).toUpperCase() + replaceText.substring(1, replaceText.length);
                }
                replaceTableCells(searchText, replaceText);
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
};

// returns the string value having special characters prefixed with backslash "\"
function getSearchText(text) {
    var arrChars, arrRetVal;
    try {
        arrChars = text.split("");
        arrRetVal = "";
        for (i = 0; i < arrChars.length; i++) {
            if (isSpecialChar(arrChars[i])) {
                arrRetVal += "\\";
            }
            arrRetVal += arrChars[i];
        }
    }
    catch (ex) {
        showErrorMessage(ex);
        arrRetVal = "";
    }
    return arrRetVal;
};

function isSpecialChar(objChar) {
    var retVal = true;
    try {
        retVal = (/^[a-zA-Z0-9- ]*$/.test(objChar)) ? false : true;
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};

$.fn.replaceText = function (search, replace, text_only) {
    return this.each(function () {
        var node = this.firstChild, val, new_val, remove = []; // Elements to be removed at the end.
        var arrWords, word, newWord;
        var replaceCS;

        // Only continue if firstChild exists.
        if (node) {
            // Loop over all childNodes.
            do {
                // The original node value.
                val = $(node).text();

                if (val.indexOf(">>") == -1) {
                    // Only process text nodes.
                    if (node.nodeType === 3) {
                        // The new value.
                        new_val = val.replace(search, replace);
                        // Only replace text if the new value is actually different!
                        if (new_val !== val) {
                            if (!text_only && /</.test(new_val)) {
                                // The new value contains HTML, set it in a slower but far more
                                // robust way.
                                $(node).before(new_val);

                                // Don't remove the node yet, or the loop will lose its place.
                                remove.push(node);
                            }
                            else {
                                // The new value contains no HTML, so it can be set in this
                                // very fast, simple way.
                                node.nodeValue = new_val;
                            }
                        }
                    }
                }
                else {
                    // The original node value.
                    val = $(node).text();
                    arrWords = val.split(' ');
                    new_val = "";

                    for (i = 0; i < arrWords.length; i++) {
                        word = arrWords[i];
                        newWord = word;
                        replaceCS = replace;

                        // if word is not a placeholder
                        if (arrWords[i].indexOf("<<") == -1 && arrWords[i].indexOf(">>") == -1) {
                            if (search.toString().substring(1, 2) == search.toString().substring(1, 2).toUpperCase() && replaceCS.substring(0, 1) == replaceCS.substring(0, 1).toLowerCase()) {
                                replaceCS = replaceCS.substring(0, 1).toUpperCase() + replaceCS.substring(1, replaceCS.length);
                            }
                            newWord = word.replace(search, replaceCS);
                        }
                        new_val += newWord + " "; //add a space after a word
                    }
                    new_val = new_val.substring(0, new_val.length - 1);
                    node.textContent = new_val;
                }
            } while (node = node.nextSibling);
        }
        // Time to remove those elements!
        remove.length && $(remove).remove();
    });
};

function showErrorMessage(ex) {
    alert(jsErrorMsg + ex.message);
};

/*******************************************************
			Unused functions
*******************************************************/

function getDBTableIdByName(name) {
    var retVal = "";
    try {
        for (i = 0; i < dataTables.length; i++) {
            if (name == dataTables[i][1]) {
                retVal = dataTables[i][0];
                break;
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};

function getDBTableNameById(id) {
    var retVal = "";
    try {
        for (i = 0; i < dataTables.length; i++) {
            if (id == dataTables[i][0]) {
                retVal = dataTables[i][1];
                break;
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};

function getDBColumnIdByName(name) {
    var retVal = "";
    try {
        for (i = 0; i < dataColumns.length; i++) {
            if (name == dataColumns[i][1]) {
                retVal = dataColumns[i][0];
                break;
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};

function getDBColumnNameById(id) {
    var retVal = "";
    try {
        for (i = 0; i < dataColumns.length; i++) {
            if (id == dataColumns[i][0]) {
                retVal = dataColumns[i][1];
                break;
            }
        }
    }
    catch (ex) {
        showErrorMessage(ex);
    }
    return retVal;
};
