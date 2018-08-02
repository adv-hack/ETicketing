<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CheckoutInvoiceProcess.aspx.vb" Inherits="PagesLogin_Checkout_CheckoutInvoiceProcess" %>

<%@ Register Src="../../UserControls/CheckoutInvoice.ascx" TagName="CheckoutInvoice"
    TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <uc1:CheckoutInvoice ID="CheckoutInvoice1" runat="server" />
    
    </div>
    </form>
</body>
</html>
