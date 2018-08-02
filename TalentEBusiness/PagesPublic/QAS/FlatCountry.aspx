<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/QASContent.master" AutoEventWireup="false" CodeFile="FlatCountry.aspx.vb" Inherits="PagesPublic_FlatCountry" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<script type="text/javascript">
	    /* Reload the country selection and set the focus to the country dropdown */
	    function init()
	    {
	        var sDataID = "<%= GetDataID() %>";
		    if (sDataID != "")
		    {
		        document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_DATA_ID %>.value = sDataID;
			}
            document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_DATA_ID %>.focus();
		}
		/* Store the text of the DataID select control in the CountryName field */
		function setCountryValue(theForm)  
		{      
		    var iSelected = theForm.<%= com.qas.prowebintegration.Constants.FIELD_DATA_ID %>.selectedIndex;
		    theForm.<%= com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME %>.value = theForm.<%= com.qas.prowebintegration.Constants.FIELD_DATA_ID %>.options[iSelected].text;
		}
	</script>
	
	<form id="Form1" method="post" runat="server" onsubmit="setCountryValue(this);">
    	<div id="qas">
    	
            <h3><%= GetPageText("titleText")%></h3>
    		<%= GetPageText("headerText")%>
		
			<table>
                <asp:PlaceHolder ID="plhPostCode" runat="server">
                <tr>
                    <th scope="row"><%= GetPageText("PostCodeText")%></th>
                    <td>
                        <asp:TextBox ID="txtPostCode" runat="server" />
                    </td>
                </tr>
                </asp:PlaceHolder>
				<tr>
					<th scope="row"><%= GetPageText("countryText")%></th>
					<td>
						<select class="select" name="<%= com.qas.prowebintegration.Constants.FIELD_DATA_ID %>">
							<!-- #include virtual="countries.all.inc" -->
						</select>
					</td>
				</tr>
			</table>
		
			<%= GetPageText("footerText")%>
		
			<input type="hidden" name="<%= com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME %>" />
			<asp:LinkButton ID="ButtonNext" runat="server"><%= GetPageText("nextButtonText")%></asp:LinkButton>
		</div>	
	</form>

</asp:Content>