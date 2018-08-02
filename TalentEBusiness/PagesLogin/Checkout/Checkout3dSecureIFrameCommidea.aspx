<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Checkout3dSecureIFrameCommidea.aspx.vb" Inherits="PagesLogin_Checkout3dSecureIFrameCommidea" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<script type="text/javascript">
    <!--
    function SubMe(){
     document.forms[0].submit();
    }
    //-->
</script>

</head>

<body onload="SubMe();">
<%--<body>--%>
    <form visible="false"  id="pasform" style="display:none;"  action="<%=formActionUrl %>" method="POST"
        enctype="application/x-www-form-urlencoded">
        <% if requestOk then response.Write(WriteForm())%>
        
        <input type="hidden" id="hdfRedirect" value="<%= hdfRedirectValue %>" />
        <input type="hidden" id="hdfRedirectLink" value="<%= hdfRedirectUrl %>" />
        <script type="text/javascript">
            <!-- //Redirect if IFrame doesnt function correctly
            var hdfRedirect = document.getElementById("hdfRedirect").value;
            var hdfRedirectLink = document.getElementById("hdfRedirectLink").value;

            if (hdfRedirect == "True")
            {
                window.open(hdfRedirectLink, "_parent");
            }
            //-->
        </script>
        <noscript>
            Javascript is disabled in your browser. In order to process your transaction please
            click the ‘Submit’ button below.<br />
            <input type="submit" name="submit" value="Submit"/>
        </noscript>
     </form>
</body>
</html>
