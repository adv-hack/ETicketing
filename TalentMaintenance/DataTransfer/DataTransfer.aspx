<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DataTransfer.aspx.vb" MasterPageFile="~/MasterPages/MasterPage.master" Inherits="DataTransfer_DataTransfer" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="Content1" runat="server">
    <div id="pageMaintenanceTopNavigation">
        <ul>
            <li><a href="../Default.aspx"><asp:Literal ID="ltlHomeLink" runat="server" /></a></li>
        </ul>
    </div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Content2" Runat="Server">

    <div class="data-transfer-wrapper">
        <h2 class="title">Data Transfer Status</h2>
    
        <asp:PlaceHolder ID="plhStatusMessages" runat="server">
            <div class="status-messages">
                <p><asp:Literal ID="ltlStatusMessage" runat="server" /></p>
            </div>
        </asp:PlaceHolder>
        
        <asp:PlaceHolder ID="plhTableKey" runat="server">
            <script>
                <!--
                var limit="0:15"

                if (document.images){
                    var parselimit=limit.split(":")
                    parselimit=parselimit[0]*60+parselimit[1]*1
                }
                
                function beginrefresh(){
                    if (!document.images)
                        return
                    if (parselimit==1){
                        var url = window.location.href;
                        window.location.href = url;
                        }
                    else{ 
                        parselimit-=1
                        curmin=Math.floor(parselimit/60)
                        cursec=parselimit%60
                        if (curmin!=0)
                            curtime=curmin+" minutes and "+cursec+" seconds left until page refresh!"
                        else
                            curtime=cursec+" seconds left until page refresh!"
                            window.status=curtime
                            setTimeout("beginrefresh()",1000)
                        }
                    }
                window.onload=beginrefresh
                //-->
            </script>
            <div class="table-key">
                <ul>
                    <li><span id="records-in-progress"></span>In Progress (<%=InProgressRecords.ToString%>)</li>
                    <li><span id="records-success"></span>Success (<%=SuccessRecords.ToString%>)</li>
                    <li><span id="records-failed"></span>Failed (<%=FailedRecords.ToString%>)</li>
                </ul>
            </div>
        </asp:PlaceHolder>
        
        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
            <div class="delete-buttons">
                <ul>
                    <li><asp:Button ID="Button1" runat="server" Text="Delete All Success Records" /></li>
                    <li><asp:Button ID="Button2" runat="server" Text="Delete All Failed Records" /></li>
                    <li><asp:Button ID="Button3" runat="server" Text="Delete All Records" /></li>
                    <li><asp:Button ID="Button4" runat="server" Text="Delete Selected Records" /></li>
                </ul>
            </div>
        </asp:PlaceHolder>

        <asp:Repeater ID="rptDataTransfer" runat="server">
            <HeaderTemplate>
                <table class="data-transfer defaultTable" summary="data transfer results">
                    <tr>
                        <th class="id" scope="col">ID</th>
                        <th class="filename" scope="col">Filename</th>
                        <th class="start-time" scope="col">Start Time</th>
                        <th class="end-time" scope="col">End Time</th>
                        <th class="success" scope="col">Success</th>
                        <th class="number-of-records" scope="col">Number of records</th>
                        <th class="status" scope="col">Status</th>
                        <th class="message" scope="col">Message</th>
                        <th class="delete-checkbox" scope="col">Delete</th>
                    </tr>
            </HeaderTemplate>
            <ItemTemplate>
                    <tr class="odd<%#ShowCssClassBasedOnStatus(DataBinder.Eval(Container, "DataItem.STATUS"))%>">
                        <td class="id"><%#DataBinder.Eval(Container, "DataItem.ID")%></td>
                        <td class="filename"><%#DataBinder.Eval(Container, "DataItem.FILENAME")%></td>
                        <td class="start-time"><%#DataBinder.Eval(Container, "DataItem.START_TIME")%></td>
                        <td class="end-time"><%#DataBinder.Eval(Container, "DataItem.END_TIME")%></td>
                        <td class="success"><%#DataBinder.Eval(Container, "DataItem.SUCCESS")%></td>
                        <td class="number-of-records"><%#DataBinder.Eval(Container, "DataItem.NUMBER_OF_RECORDS")%></td>
                        <td class="status"><%#DataBinder.Eval(Container, "DataItem.STATUS")%></td>
                        <td class="message"><%#DataBinder.Eval(Container, "DataItem.MESSAGE")%></td>
                        <td class="delete-checkbox">
                            <asp:CheckBox ID="chkDeleteRecord" runat="server" />
                            <asp:HiddenField ID="hdfID" runat="server" Value='<%#DataBinder.Eval(Container, "DataItem.ID")%>' />
                        </td>
                    </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                    <tr class="even<%#ShowCssClassBasedOnStatus(DataBinder.Eval(Container, "DataItem.STATUS"))%>">
                        <td class="id"><%#DataBinder.Eval(Container, "DataItem.ID")%></td>
                        <td class="filename"><%#DataBinder.Eval(Container, "DataItem.FILENAME")%></td>
                        <td class="start-time"><%#DataBinder.Eval(Container, "DataItem.START_TIME")%></td>
                        <td class="end-time"><%#DataBinder.Eval(Container, "DataItem.END_TIME")%></td>
                        <td class="success"><%#DataBinder.Eval(Container, "DataItem.SUCCESS")%></td>
                        <td class="number-of-records"><%#DataBinder.Eval(Container, "DataItem.NUMBER_OF_RECORDS")%></td>
                        <td class="status"><%#DataBinder.Eval(Container, "DataItem.STATUS")%></td>
                        <td class="message"><%#DataBinder.Eval(Container, "DataItem.MESSAGE")%></td>
                        <td class="delete-checkbox">
                            <asp:CheckBox ID="chkDeleteRecord" runat="server" />
                            <asp:HiddenField ID="hdfID" runat="server" Value='<%#DataBinder.Eval(Container, "DataItem.ID")%>' />
                        </td>
                    </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </table>
            </FooterTemplate>
        </asp:Repeater>
        
        <asp:PlaceHolder ID="plhDeleteButtons" runat="server">
            <div class="delete-buttons">
                <ul>
                    <li><asp:Button ID="btnDeleteAllSuccessRecords" runat="server" Text="Delete All Success Records" /></li>
                    <li><asp:Button ID="btnDeleteAllFailedRecords" runat="server" Text="Delete All Failed Records" /></li>
                    <li><asp:Button ID="btnDeleteAllRecords" runat="server" Text="Delete All Records" /></li>
                    <li><asp:Button ID="btnDeleteSelectedRecords" runat="server" Text="Delete Selected Records" /></li>
                </ul>
            </div>
        </asp:PlaceHolder>
    
    </div>
</asp:Content>