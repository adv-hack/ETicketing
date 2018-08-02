<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PaymentCardDetails.ascx.vb" Inherits="UserControls_PaymentCardDetails" %>
<%@ Reference Control="~/UserControls/CheckoutPartPayments.ascx" %>
<script type="text/javascript">

    var VGCardCapturePage = '<%:VGCardCapturePage%>';
    var CCType = '<%:BasketCardTypeCode%>';
    var currentYear = '<%= Date.Now.Year%>';
    var currentMonth = '<%= Date.Now.ToString("MM")%>';
    var ValidateCardType = <%= ValidateCardType.ToString().ToLower()%>
    
    $(document).ready(function ()
    {        
        InitialiseTheControls();
    });

    function InitialiseTheControls() {
        $("#chkSaveTheseCardDetails").bind("click", CheckSaveCardDetailsChangeEvent);
        PopulateExpiryYear();
        PopulateStartYear();
    }

    function PopulateExpiryYear()
    {     
        var year = currentYear
        var ddlExpiry = document.getElementById("expiryyear");

        for (i = 0; i <= 20; i++)
        {
            var option = document.createElement("option");
            option.text = year;
            option.value = year.toString().substring(2);
            year++;
            ddlExpiry.add(option);
        }
    }

    function PopulateStartYear()
    {        
        var year = currentYear
        var ddlStart = document.getElementById("startyear");
        var option = document.createElement("option");
        option.text = "--";
        option.value = "--";
        ddlStart.add(option);
        for (i = 0; i <= 20; i++)
        {
            var option = document.createElement("option");
            option.text = year;
            option.value = year.toString().substring(2);
            year--;
            ddlStart.add(option);
        }
    }

    function ValidateCardDetails()
    {
        var valid = "false";
        var cardNumber;
        try
        {
            resetPaymentCardDetailsErrors();
            cardNumber = document.getElementById('pan').value;

            // check to see if there is a card type selected in PPS mode 
            if (document.getElementById("ddlCardTypes"))
            {
                if (document.getElementById("ddlCardTypes").value.trim() == "--")
                {
                    addErrorsToValidationSummary('<%:RFVCreditCardType%>', "ddlCardTypes", "errCreditCardType");
                    return false;
                }
            }

            if (cardNumber.length == 0)
            {
                addErrorsToValidationSummary('<%:RFVCreditCardNumberMsg%>', "pan", "errPAN");
                return false;
            }
            var valid = ValidateCardNumber(cardNumber);
            if (!valid)
            {
                addErrorsToValidationSummary("<%:CFVCreditCardNumberInvalidMsg%>", "pan", "errPAN");
                return false;
            }
            else
            {
                if (!ValidateCardNumberWithType(cardNumber))
                {
                    addErrorsToValidationSummary("<%:CFVCreditCardRangeInvalidMsg%>", "pan", "errPAN");
                    return false;
                }
            }

            if (document.getElementById('expirymonth').value == '--')
            {
                addErrorsToValidationSummary("<%:RFVExpiryDateMsg%>", "expirymonth", "errExpiryMonth");
                return false;
            }

            valid = ValidateDates()
            if (!valid)
            {
                return false;
            }

            valid = ValidateCVVNumber(document.getElementById('csc').value);
            if (!valid)
            {
                addErrorsToValidationSummary("<%:CFVSecurityInvalidMsg%>", "csc", "errCSC");
                return false;
            }

            if (!document.getElementById('chkCardTAndC').checked){
                addErrorsToValidationSummary("<%:RFVNoTermsConditionsChecked%>", "chkCardTAndC", "errChkCardTAndC");
                return false;
            }
            return true;
        }
        catch (err)
        {
            return false;
        }
    }

    function ValidateCardNumber(cardNumber)
    {

        var sum = 0;
        var i, val, length, parity
        var strCardNumber = cardNumber.toString();
        length = cardNumber.length
        parity = length % 2

        for (i = 0; i < length; i++)
        {

            val = parseInt(strCardNumber.charAt(i))
            if (i % 2 == parity)
            {
                val = val * 2;
            }

            if (val > 9)
            {
                val = val - 9
            }

            sum = sum + val
        }

        if (sum % 10 == 0)
            return true;
        else
            return false;
    }

    /* THIS FUNCTION IS TO PROVIDE TRUE IF CREDIT CARD NUMBER IS MATCHED WITH RANGES RECEIVED FROM THE DATABASE. */
    function ValidateCardNumberWithType(cardNumber)
    {
        if (ValidateCardType == true)
        {
            var i, ccTypeIdx = -1, rangeMin, rangeMax, range;
            var arrRanges = [];
            if (document.getElementById("ddlCardTypes")) {
                CCType = document.getElementById("ddlCardTypes").value.toUpperCase();
            }
            ccTypeIdx = CCTypeCodes.indexOf(CCType);
            if (ccTypeIdx > -1) {
                for (i = 0; i < CCTypeRanges[ccTypeIdx].length; i++) {
                    arrRanges[i] = CCTypeRanges[ccTypeIdx][i];
                }
                if (arrRanges.length > 0) {
                    // check against the different ranges
                    for (i = 0; i < arrRanges.length; i++) {
                        range = arrRanges[i].split("-");
                        rangeMin = parseInt(range[0]);
                        rangeMax = parseInt(range[1]);
                        if (cardNumber.startsWith(rangeMin.toString()) || cardNumber.startsWith(rangeMax.toString())) {
                            return true;
                        }
                        else if (parseInt(cardNumber.substring(0, rangeMin.toString().length)) > rangeMin && parseInt(cardNumber.substring(0, rangeMax.toString().length)) < rangeMax) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    /* STARTSWITH FUNCTION RETURNS TRUE IF STRING STARTS WITH THE SPECIFIED ARGUMENT  */
    if (typeof String.prototype.startsWith != 'function')
    {

        String.prototype.startsWith = function (str)
        {
            return this.indexOf(str) == 0;
        };
    }

    function ValidateDates()
    {
        if (document.getElementById("expiryyear").value == currentYear.toString().substring(2) && document.getElementById('expirymonth').value < parseInt(currentMonth))
        {
            addErrorsToValidationSummary("<%:CFVExpiryDateInPastMsg%>", "expirymonth", "errExpiryMonth");
            return false;
        }

        if (document.getElementById("startmonth").value == "--")
        {
            return true;
        }
        else
        {
            if (document.getElementById("startyear").value == currentYear.toString().substring(2) && document.getElementById("startmonth").value > parseInt(currentMonth))
            {
                addErrorsToValidationSummary("<%:CFVStartDateInFutureMsg%>", "startmonth", "errStartMonth");
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    function ValidateCVVNumber(cvvNumber)
    {
        var len = cvvNumber.length
        if (len == 3 || len == 4)
            return true;
        else
            return false;
    }

    function addErrorsToValidationSummary(objErrorMsg,objControl,objErrorContainer)
    {
        // add error to validation summary 
        addNewErrorInValidationSummary(objErrorMsg);
        $("#vlsPaymentCardDetailsErrors").show();

        if ($("#" + objControl))
        {
            $("#" + objControl).focus(); // have focus on the control having a problem 
        }
        if ($("#" + objErrorContainer))
        {
            $("#" + objErrorContainer).text(objErrorMsg);
            $("#" + objErrorContainer).show(); // display the error 
        }
    }

    function addNewErrorInValidationSummary(objErrorMsg)
    {
        var ul = document.getElementById("PaymentCardDetailsErrors");
        var li = document.createElement("li");
        li.appendChild(document.createTextNode(objErrorMsg));
        ul.appendChild(li);
    }
    function resetPaymentCardDetailsErrors()
    {
        $("#vlsPaymentCardDetailsErrors").hide(); //available in checkout.aspx  
        $("#PaymentCardDetailsErrors li").remove(); //available in checkout.aspx 
        $("span[vg='pcd']").text('');
        $("span[vg='pcd']").hide();
    }
    function CheckSaveCardDetailsChangeEvent()
    {

        checkoutPageURL = "Checkout.aspx/SetSaveCardFlag";
        var basketPaymentID = document.getElementById("basketpaymentid").value;

        $.ajax({
            type: "POST",
            url: checkoutPageURL,
            data: '{basketPaymentID: "' + basketPaymentID + '","canSaveCard":"' + document.getElementById("chkSaveTheseCardDetails").checked + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (msg)
            {
                // successfully saved don’t show any error message

            },
            error: function (xhr, status)
            {
                alert(status + '  ' + xhr.responseText);
            }
        });
    }

    function submitToVanguard()
    {
        try
        {
            if (ValidateCardDetails())
            {
                var vgForm = "";
                vgForm = vgForm.concat('<form id="vgccform"  name="vgccform" action="'+ VGCardCapturePage +'" method="post">');
                vgForm = vgForm.concat('<input type="hidden" name="sessionguid" id="sessionguid" value="' + GetFormattedFieldValue("sessionguid") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="sessionpasscode" id="sessionpasscode" value="' + GetFormattedFieldValue("sessionpasscode") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="processingdb" id="processingdb" value="' + GetFormattedFieldValue("processingdb") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="address1" id="address1" value="' + GetFormattedFieldValue("address1") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="postcode" id="postcode" value="' + GetFormattedFieldValue("postcode") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="country" id="country" value="' + GetFormattedFieldValue("country") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="paymentAmount" id="paymentAmount" value="' + GetFormattedFieldValue("paymentAmount") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="pan" id="pan" value="' + GetFormattedFieldValue("pan") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="expirymonth" id="expirymonth" value="' + GetFormattedFieldValue("expirymonth") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="expiryyear" id="expiryyear" value="' + GetFormattedFieldValue("expiryyear") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="startmonth" id="startmonth" value="' + GetFormattedFieldValue("startmonth") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="startyear" id="startyear" value="' + GetFormattedFieldValue("startyear") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="issuenumber" id="issuenumber" value="' + GetFormattedFieldValue("issuenumber") + '">');
                vgForm = vgForm.concat('<input type="hidden" name="csc" id="csc" value="' + GetFormattedFieldValue("csc") + '">');
                vgForm = vgForm.concat(GetFormattedFieldValue("pleaseWaitText").replace(/\[\[/g, "<").replace(/\]\]/g, ">"));
                vgForm = vgForm.concat('</form>');
                vgForm = vgForm.concat('<script type="text/javascript">document.getElementById("vgccform").submit();<' + '/script>');
                document.write(vgForm);
                document.getElementById("vgccform").submit();
            }
        }
        catch (ex)
        {
            // WE NEED TO CONFIRM THE ERROR MESSAGE 
        }
    }
    function GetFormattedFieldValue(elementID)
    {
        var formattedFieldValue = "";
        if (document.getElementById(elementID) != null)
        {
            formattedFieldValue = document.getElementById(elementID).value;
            formattedFieldValue = formattedFieldValue.replace("'", "");
            formattedFieldValue = formattedFieldValue.replace('"', '');
        }
        else
        {
            formattedFieldValue = "";
        }
        return formattedFieldValue;
    }
</script>
<asp:PlaceHolder ID="plhCardTypes" runat="server">
    <div class="row ebiz-card-type">
        <div class="medium-3 columns">
            <asp:Label ID="lblCardTypes" runat="server" AssociatedControlID="ddlCardTypes"  />
        </div>
        <div class="medium-9 columns">
            <select id="ddlCardTypes" runat="server"></select>
            <span id="errCreditCardType" class="error" style="display:none;" vg="pcd"></span>
        </div>
    </div>
</asp:PlaceHolder>

<div class="row ebiz-card-number">
    <div class="medium-3 columns">
        <asp:Literal ID="ltlHiddenFields" runat="server"></asp:Literal>
        <asp:Label ID="lblCardNumber" runat="server"  />
    </div>
    <div class="medium-9 columns">
        <input type="number" name="pan" id="pan" min="0" autocomplete="off"/>
        <span id="errPAN" class="error" style="display:none;" vg="pcd"></span>
    </div>
</div>


<div class="row ebiz-expiry-date">
    <div class="medium-3 columns">
        <asp:Label ID="lblExpiryDate" runat="server"  />
    </div>
    
    <div class="medium-2 columns">
        <select id="expirymonth" name="expirymonth">
            <option value="--">--</option>
            <option value="01">01</option>
            <option value="02">02</option>
            <option value="03">03</option>
            <option value="04">04</option>
            <option value="05">05</option>
            <option value="06">06</option>
            <option value="07">07</option>
            <option value="08">08</option>
            <option value="09">09</option>
            <option value="10">10</option>
            <option value="11">11</option>
            <option value="12">12</option>
        </select>
        
    </div>
    <div class="medium-2 columns">
        <select id="expiryyear" name="expiryyear"></select>
    </div>
    <div class="medium-9 columns">
        <span id="errExpiryMonth" class="error" style="display:none;" vg="pcd"></span>
    </div>
</div>

<div class="row ebiz-start-date">
    <div class="medium-3 columns">
        <asp:Label ID="lblStartDate" runat="server" />
    </div>
    <div class="medium-2 columns">
        <select id="startmonth" name="startmonth">
            <option value="--">--</option>
            <option value="01">01</option>
            <option value="02">02</option>
            <option value="03">03</option>
            <option value="04">04</option>
            <option value="05">05</option>
            <option value="06">06</option>
            <option value="07">07</option>
            <option value="08">08</option>
            <option value="09">09</option>
            <option value="10">10</option>
            <option value="11">11</option>
            <option value="12">12</option>
        </select>
    </div>
    <div class="medium-2 columns">
        <select id="startyear" name="startyear"></select>
    </div>
    <div class="medium-9 columns">
        <span id="errStartMonth" class="error" style="display:none;" vg="pcd"></span>
    </div>
</div>

<asp:PlaceHolder ID="plhIssueNumber" runat="server">
    <div class="row ebiz-issue-number">
        <div class="medium-3 columns">
            <asp:Label ID="lblIssueNumber" runat="server"  />
        </div>
        <div class="medium-2 columns last">
            <input type="number" name="issuenumber" id="issuenumber" autocomplete="off" min="0" max="4" />
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhSecurityNumber" runat="server">
    <div class="row ebiz-security-number">
        <div class="medium-3 columns">
            <asp:Label ID="lblSecurityNumber" runat="server"  />
        </div>
        <div class="medium-2 columns">
            <input type="number" name="csc" id="csc" autocomplete="off" min="0" max="4" />
            <span id="errCSC" class="error" style="display:none;" vg="pcd"></span>
        </div>
        <div class="medium-7 columns">
            <asp:Image ID="SecurityImage" SkinID="CV2" runat="server" />
        </div>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhCardHolderName" runat="server" Visible="false">
    <div class="row ebiz-card-holder-name">
        <div class="medium-3 columns">
            <asp:Label ID="lblCardHolderName" runat="server" AssociatedControlID="cardholdername"  />
        </div>
        <div class="medium-9 columns">
            <input type="text" id="cardholdername" name="cardholdername" maxlength="50" runat="server" />
        </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="plhSaveTheseCardDetails" runat="server" Visible="true">
    <p class="ebiz-save-these-card-details">
        <input type="checkbox" id="chkSaveTheseCardDetails" />
        <asp:Label ID="lblSaveTheseCardDetails" runat="server" />
    </p>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhCreditCardTAndC" runat="server" Visible="true">
    <div class="ebiz-terms-and-contitions">
        <input type="checkbox" id="chkCardTAndC" />
        <asp:Label ID="lblCardTAndC" runat="server" CssClass="tandc-label" />
        <span id="errChkCardTAndC" class="error" style="display:none;" vg="pcd"></span>        
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="plhCustomerPresentCC" runat="server" Visible="False">
    <div class="ebiz-customer-present">
        <input type="checkbox" runat="server" id="chkCustomerPresentCC" />

        <asp:Label ID="lblCustomerPresentCC" runat="server" AssociatedControlID="chkCustomerPresentCC" CssClass="custpres-label" />
    </div>

</asp:PlaceHolder>


