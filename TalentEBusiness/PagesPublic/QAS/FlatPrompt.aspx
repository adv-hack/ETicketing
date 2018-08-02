<%@ Page Language="VB" MasterPageFile="~/MasterPages/Shared/QASContent.master" AutoEventWireup="false" CodeFile="FlatPrompt.aspx.vb" Inherits="PagesPublic_FlatPrompt" title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
		<script type="text/javascript">
		    /* Set the focus to the first input line */
		    function init()
		    {
		        document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_INPUT_LINES %>[0].focus();
			}
			
			/* Ensure at least one address line has been entered */
			function validate()
			{
			    var aUserInput = document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_INPUT_LINES %>;
			    for (var i=0; i < aUserInput.length; i++)
			    {
			        if (aUserInput[i].value != "")
			        {
			            return true;
			        }
			    }
			    document.forms[0].<%= com.qas.prowebintegration.Constants.FIELD_INPUT_LINES %>[0].focus();
				alert("<%=GetPageText("validationErrorText")%>");
			    return false;
			}
		</script>
		<form id="Form1" method="post" runat="server">
            <div id="qas">
            
                <h3><%=GetPageText("titleText")%></h3>
        		<%=GetPageText("headerText")%>
			<asp:PlaceHolder runat="server" ID="plcQAS">
				<table>
					<%	
					    Dim asInputLines As String()
					    asInputLines = GetInputLines()
					    Dim i As Integer
					  
					        For i = 0 To m_atPromptLines.Length - 1
					            Dim sValue As String = ""
					            If i < asInputLines.Length Then
					                sValue = asInputLines(i)
					            End If
					%>
					<tr>
						<th scope="row"><%= m_atPromptLines(i).Prompt %></th>
						<td>
						    <input class="input-l" type="text" size="<%= m_atPromptLines(i).SuggestedInputLength %>" value="<%= sValue %>" name="<%= com.qas.prowebintegration.Constants.FIELD_INPUT_LINES %>" />&nbsp;&nbsp;<i>(e.g.<%= m_atPromptLines(i).Example %>)</i>
						</td>
					</tr>
					<%	Next i%>
					<tr>
						<td colspan="2">
							<p><asp:linkbutton id="HyperlinkAlternate" runat="server" EnableViewState="False"><%=GetPageText("alternativeLinkText")%></asp:linkbutton></p>
   						</td>
					</tr>
				</table>
                </asp:PlaceHolder>
                	<asp:PlaceHolder runat="server" ID="plcQasOndemand">
                	<table>
					<%	
					    Dim asInputLines2 As String()
					    asInputLines2 = GetInputLines()
					    Dim i2 As Integer
					  
					    For i2 = 0 To m_atPromptLinesOnDemand.Length - 1
					        Dim sValue As String = ""
					        If i2 < asInputLines2.Length Then
					            sValue = asInputLines2(i2)
					        End If
					        If m_atPromptLinesOnDemand(i2).Prompt = "Postcode" Then
					            If Not Request("postCode") Is Nothing Then
					                sValue = Request("postCode").ToString
					            End If
					        End If
					%>
					<tr>
						<th scope="row"><%= m_atPromptLinesOnDemand(i2).Prompt%></th>
						<td>
						    <input class="input-l" type="text" size="<%= m_atPromptLinesOnDemand(i2).SuggestedInputLength %>" value="<%= sValue %>" name="<%= com.qas.prowebintegration.Constants.FIELD_INPUT_LINES %>" />&nbsp;&nbsp;<i>(e.g.<%= m_atPromptLinesOnDemand(i2).Example%>)</i>
						</td>
					</tr>
					<% Next i2%>
					<tr>
						<td colspan="2">
							<p><asp:linkbutton id="lnkAlternate" runat="server" EnableViewState="False"><%=GetPageText("alternativeLinkText")%></asp:linkbutton></p>
   						</td>
					</tr>
				</table>
    		</asp:PlaceHolder>
    			<%=GetPageText("footerText")%>
			
				<%
				    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_DATA_ID)
				    RenderRequestString(com.qas.prowebintegration.Constants.FIELD_COUNTRY_NAME)
				%>
				<input id="HiddenPromptSet" type="hidden" runat="server" NAME="HiddenPromptSet" />
				<asp:LinkButton ID="ButtonBack"  Visible="false" runat="server"><%= GetPageText("backButtonText")%></asp:LinkButton>
				<asp:LinkButton ID="ButtonNext"  runat="server" OnClientClick="return validate();"><%= GetPageText("nextButtonText")%></asp:LinkButton>
				
			</div>
		</form>

</asp:Content>