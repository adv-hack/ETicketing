<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Templateselection.aspx.vb"
    Inherits="Templateselction" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Visual Template Selection</title>

    <script type="text/javascript" src="../javascript/tableruler.js"></script>

    <script type="text/javascript">
    	window.onload=function(){tableruler();} 
    
    </script>



</head>
<body>
    <form id="form1" runat="server">
        <div id="visualSelection">
            <table cellspacing="0">
                <tbody>
                    <tr>
                            <asp:DataList RepeatColumns="3" RepeatDirection="Horizontal" ID="Repeater1" runat="server"
                                DataSourceID="ObjectDataSource1">
                                <ItemTemplate>
                                         <td>
                                        <p>
                                        <table cellpadding="0" cellspacing="0" border="0">
                                        <tr>
                                            <td align="center">
                                            <a href="#" onclick="window.opener.document.getElementById('ctl00_Content1_TemplateDropDownList').value='<%#Eval("TEMPLATE_NAME")%>';window.opener.document.getElementById('ctl00_Content1_TemplateImage').src='http://'+window.location.host+'/Maintenance/images/<%#Eval("TEMPLATE_NAME")%>'+'.gif';self.close();">
                                            <img  runat="server" id="TemplateImage" src='<%# "~/images/"+Eval("TEMPLATE_NAME")+".gif" %>' width="240" height="180" class="template-image" /></a>
                                            </td>
                                        </tr>
                                        <tr>
                                          <td align="center">
                                            <a href="#" onclick="window.opener.document.getElementById('ctl00_Content1_TemplateDropDownList').value='<%#Eval("TEMPLATE_NAME")%>';window.opener.document.getElementById('ctl00_Content1_TemplateImage').src='http://'+window.location.host+'/Maintenance/images/<%#Eval("TEMPLATE_NAME")%>'+'.gif';self.close();"><asp:Label ID="label1" runat="server" Text=' <%#Eval("TEMPLATE_NAME")%> ' /></a>
                                            </td>
                                        </tr>
                                        </table>
                                            </p>
                                    </td>
                                </ItemTemplate>
                            </asp:DataList>
                            <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OldValuesParameterFormatString="original_{0}"
                                SelectMethod="GetData" TypeName="pageselectionTableAdapters.PageTemplateTableAdapter">
                            </asp:ObjectDataSource>
                    
                    </tr>
                </tbody>
            </table>
        </div>
    </form>
</body>
</html>
