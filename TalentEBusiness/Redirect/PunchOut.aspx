<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PunchOut.aspx.vb" Inherits="Redirect_Punchout" Title="Checkout" %>


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<script type="text/javascript">
    <!--
//    function SubMe() {
//        document.forms[0].submit();
//    }
    //-->
</script>

</head>

<body onload="SubMe();">
    <form visible="<%= formVisible %>"  id="pasform" style="<%= formStyle %>"  action="<%=formActionUrl %>" method="POST"
        enctype="application/x-www-form-urlencoded" target="<%=formTarget %>">
        <% Response.Write(WriteForm())%>
        
     </form>
</body>
</html>
