<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/QASContent.master" enableViewState="False" AutoEventWireup="false" CodeFile="FlatSearch.aspx.vb" Inherits="PagesPublic_FlatSearch" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	<script type="text/javascript">
	    /* Set the focus to the first input line */
	    function init()
	    {
	    }
	    /* Hyperlink clicked: select the appropriate radio button, click the Next button */
	    function submitCommand(iIndex)
	    {
	        document.getElementsByName("<%= com.qas.prowebintegration.Constants.FIELD_MONIKER %>")[iIndex].checked = true;
		    document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_MONIKER %>.selectedIndex = iIndex;
		    document.forms[0].<%= GetUniqID() %>.click();
		}
		/* Next button clicked: ensure radio button selected, and pick up private data */
		function submitForm()
		{
		    var aItems = document.getElementsByName("<%= com.qas.prowebintegration.Constants.FIELD_MONIKER %>");
		    for (var i=0; i < aItems.length; i++)
		    {
		        if (aItems[i].checked == true)
		        {
		            return true;
		        }
		    }
		    alert("<%=GetPageText("validationErrorText")%>");				
			return false;
        }			
	</script>
	<form id="Form1" method="post" runat="server" onsubmit="return submitForm();">
        <div id="qas">
            <h3><%=GetPageText("titleText")%></h3>
        	<%=GetPageText("headerText")%>
            <asp:PlaceHolder runat="server" id="plcQAS">
			<table>
				<%
				    Dim atItems As com.qas.proweb.PicklistItem() = m_Picklist.Items
				    Dim i As Integer
				    For i = 0 To atItems.Length - 1
				%>
				<tr>
					<th scope="row">
						<input type="radio" name="<%= com.qas.prowebintegration.Constants.FIELD_MONIKER %>" value="<%= atItems(i).Moniker %>" /><a href="javascript:submitCommand('<%= i %>');"><%= atItems(i).Text%></a>
					</th>
					<td>
						<%= atItems(i).Postcode %>
					</td>
				</tr>
				<%Next%>
			</table>
            </asp:PlaceHolder>
            <asp:PlaceHolder runat="server" ID="plcQASOnDemand">
			<table>
				<%
				    Dim atItems2 As com.qas.prowebondemand.PicklistItem() = m_PicklistOnDemand.Items
				    Dim i2 As Integer
				    For i2 = 0 To atItems2.Length - 1
				%>
				<tr>
					<th scope="row">
						<input type="radio" name="<%= com.qas.prowebintegration.Constants.FIELD_MONIKER %>" value="<%= atItems2(i2).Moniker %>" /><a href="javascript:submitCommand('<%= i2 %>');"><%= atItems2(i2).Text%></a>
					</th>
					<td>
						<%= atItems2(i2).Postcode%>
					</td>
				</tr>
				<%Next%>
			</table>
            </asp:PlaceHolder>
			<%=GetPageText("footerText")%>
			
			<%
			    ' carry through values from earlier pages
			    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_DATA_ID)
			    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME)
			    RenderRequestArray(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)
			    RenderRequestString(FIELD_PROMPTSET)
			    RenderRequestString(FIELD_PICKLIST_MONIKER)
			    ' hidden field to be populated by client JavaScript, picked out from form PrivateData
			    RenderHiddenField(FIELD_MUST_REFINE, "")
			%>
    		<asp:LinkButton ID="ButtonBack" Visible="false"  runat="server"><%= GetPageText("backButtonText")%></asp:LinkButton>
			<asp:LinkButton ID="ButtonNext"  runat="server"><%= GetPageText("nextButtonText")%></asp:LinkButton>
            <input type="submit" value="" style="display:none;" id="ButtonNext1" runat="server" visible="true" name="ButtonNext1" size="1" />
        </div>
	</form>
</asp:Content>