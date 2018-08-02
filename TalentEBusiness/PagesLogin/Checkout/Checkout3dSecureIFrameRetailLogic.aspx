<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Checkout3dSecureIFrameRetailLogic.aspx.vb" Inherits="PagesLogin_Checkout3dSecureIFrameRetailLogic" title="Retail Logic" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>
<script type="text/javascript">
    <!--
    function SubMe(){
     document.getElementById("pasform").style.display = 'none';
     document.forms[0].submit();
    }
    //-->
</script>

</head>

<body onload="SubMe();" class="iframe-body">
    <asp:PlaceHolder ID="plhFormToPostToTicketingGateway" runat="server" Visible="false">
        <asp:Literal ID="ltlPleaseWait" runat="server" />
        <asp:Image ID="imgWaitingIcon" runat="server" />
    </asp:PlaceHolder>
    <form visible="false" id="pasform" action="<%=formActionUrl %>" method="POST"
        enctype="application/x-www-form-urlencoded">
        <asp:PlaceHolder ID="plhFormToPostToRetailLogic" runat="server">
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
        </asp:PlaceHolder>
        <noscript>
            <asp:Literal ID="ltlNoJavascriptMessage" runat="server" />
            <input type="submit" name="submit" value="Submit">
        </noscript>
    </form>
</body>
</html>
