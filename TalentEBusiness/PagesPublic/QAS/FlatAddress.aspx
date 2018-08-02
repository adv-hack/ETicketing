<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/QASContent.master" AutoEventWireup="false" CodeFile="FlatAddress.aspx.vb" Inherits="PagesPublic_FlatAddress" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
    
    <%-----
		<script type="text/javascript">
		function init() {
		}
		function DoOpener() {
		    
		    var NotFound = document.forms[0].ctl00$ContentPlaceHolder1$NotFound.value;
		    if (NotFound == '') {
    		    var AdrLine0 = document.forms[0].AdrLine0.value;
        	    var AdrLine1 = document.forms[0].AdrLine1.value;
	            var AdrLine2 = document.forms[0].AdrLine2.value;
	            var AdrLine3 = document.forms[0].AdrLine3.value;
    	        var AdrLine4 = document.forms[0].AdrLine4.value;
    	        var AdrLine5 = document.forms[0].AdrCountry.value;
            }
            else {
       		    var AdrLine0 = document.forms[0].AdrLine0.value;
        	    var AdrLine1 = document.forms[0].AdrLine1.value;
	            var AdrLine2 = document.forms[0].AdrLine2.value; 
	            var AdrLine2a = document.forms[0].AdrLine3.value;
	            var AdrLine3 = document.forms[0].AdrLine4.value;
       		    var AdrLine4 = document.forms[0].AdrLine5.value;
        	    var AdrLine5 = document.forms[0].AdrCountry.value;
        	    if (AdrLine2a != '') {
                    if (AdrLine2 == '') {
                        AdrLine2 = AdrLine2a;
                    }
                    else {
                        AdrLine2 = AdrLine2 + ' ' + AdrLine2a;
                    }        	       
        	    }
       		}

	    
	        if (window.opener && !window.opener.closed) {
                window.opener.document.forms[0].hiddenAdr0.value = AdrLine0;
                window.opener.document.forms[0].hiddenAdr1.value = AdrLine1;
                window.opener.document.forms[0].hiddenAdr2.value = AdrLine2;
                window.opener.document.forms[0].hiddenAdr3.value = AdrLine3;
                window.opener.document.forms[0].hiddenAdr4.value = AdrLine4;
                window.opener.document.forms[0].hiddenAdr5.value = AdrLine5;
                window.opener.UpdateAddressFields();
            }
               
            window.close();
		}
	</script>		
	-----%>

	<form id="Form1" method="post" runat="server">
        <div id="qas">
        
            <h3><%=GetPageText("titleText")%></h3>
        
        	<%=GetPageText("headerText")%>					
		
			<P><asp:Literal id="LiteralMessage" runat="server" EnableViewState="False"></asp:Literal></P>
        	
			<table>
				<%
				    If not m_asAddressLines is Nothing
				    Dim i As Integer
				    For i = 0 To m_asAddressLines.Length - 1
				%>
				<tr>
					<th scope="row">
						<%= m_asAddressLabels(i) %>
					</th>
					<td>
						<input type="hidden" name="<%= com.qas.prowebintegration.Constants.FIELD_ADDRESS_LINES %>" value="<%= HttpUtility.HtmlEncode(m_asAddressLines(i)) %>" class="input-l" />
						<input type="text" name="AdrLine<%= i %>" value="<%= HttpUtility.HtmlEncode(m_asAddressLines(i)) %>" />
					</td>
				</tr>
				<%
				  Next 
				  End if
				 %>
				<tr>
					<th scope="row">Country</th>
					<td><input type="text" name="<%= com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME %>" value="<%= GetCountryName() %>" readonly class="readonly input-l" tabindex="-1" /><input type="hidden" name="AdrCountry" value="<%= GetCountryName() %>" /></td>
				</tr>
				<% If Not GetRoute().Equals(com.qas.prowebintegration.Constants.Routes.Okay) Then %>
				<tr class="debug">
					<td colspan="2">
							<p class="debug"><%= GetRoute().ToString() %>									
							<% If Not GetErrorInfo() Is Nothing Then
							        Response.Write("<br />" + GetErrorInfo())
							    End If
							%>
							</p>
					</td>
				</tr>
				<%	End If%>
			</table>

            <%=GetPageText("footerText")%>

			<%
			    ' carry through values from earlier pages
			    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_DATA_ID)
			    RenderRequestArray(com.qas.prowebintegration.Constants.FIELD_INPUT_LINES)
			    RenderRequestString(FIELD_PROMPTSET)
			    RenderRequestString(FIELD_PICKLIST_MONIKER)
			    RenderRequestString(FIELD_REFINE_MONIKER)
			    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_MONIKER)
			    RenderHiddenField(com.qas.prowebintegration.Constants.FIELD_ROUTE, GetRoute().ToString())
			%>
            <asp:HiddenField ID="NotFound" runat="server" Value="" />
            <input type="hidden" id="CountryCode" name="CountryCode" value="<%= GetCountryCode() %>" class="input-l" />
       		<asp:LinkButton ID="ButtonBack"  runat="server"><%= GetPageText("backButtonText")%></asp:LinkButton>
	    	<asp:LinkButton ID="ButtonAccept"  runat="server" OnClientClick="Javascript:DoOpener();"><%= GetPageText("nextButtonText")%></asp:LinkButton>
	    	
       </div>
	</form>
	
    <% CreateQASJavascript()%>
    
</asp:Content>