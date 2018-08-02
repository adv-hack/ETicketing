<%@ Page Language="VB" AutoEventWireup="false" CodeFile="checkout3dSecureiFrameHSBC.aspx.vb" Inherits="PagesLogin_checkout3dSecureiFrameHSBC" title="Untitled Page" %>

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
    <form visible="false"  id="pasform" style="display:none;"  action="https://www.ccpa.hsbc.com/ccpa" method="POST"
        enctype="application/x-www-form-urlencoded">
        <%=WriteForm()%>
    </form>
</body>
</html>
