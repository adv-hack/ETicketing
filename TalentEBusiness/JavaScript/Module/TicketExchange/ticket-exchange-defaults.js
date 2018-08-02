
$(document).ready(function () {
    $(".ebiz-ticket-exchange-defaults").DataTable(
           {
               "bSort": false,
               "bPaginate": false
               // Don't turn paginate on - updates dont work!!!  
           }
        );   
});

function validateProductMinMaxPrice(sender, args) {
    var minPrice = parseFloat($("#txtProductMinPrice").val());
    var maxPrice = parseFloat($("#txtProductmaxPrice").val());
    var ValidateMinPriceMsg = $("#hdfValidateProductMinPriceMsg").val();
    var ValidateMaxPriceMsg = $("#hdfValidateProductMaxPriceMsg").val();

    var ValidatorProductMinPrice = $("#cvValidateProductMinPrice");
    var ValidatorProductMaxPrice = $("#cvValidateProductMaxPrice");

    args.IsValid = true;

    if (minPrice > maxPrice)
    {
        args.IsValid = false;
    }
 

    if (isNaN(minPrice)||isNaN(maxPrice))
    {
        args.IsValid = false;
    }


    if (args.IsValid == false)
    {
        ValidatorProductMinPrice.html(ValidateMinPriceMsg);
        ValidatorProductMaxPrice.html(ValidateMaxPriceMsg);
    }
    else {
        ValidatorProductMinPrice.html("");
        ValidatorProductMaxPrice.html("");
    }


}