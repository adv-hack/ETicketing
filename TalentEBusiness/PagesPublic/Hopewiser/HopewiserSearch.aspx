<%@ Page Language="VB" AutoEventWireup="false" CodeFile="HopewiserSearch.aspx.vb" Inherits="PagesPublic_HopewiserSearch" %>
<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <Talent:PageHeaderText ID="uscPageHeaderText" runat="server" />
    <% DoHopewiserSearch()%>

    <asp:Label ID="lblErrorMessage" runat="server" CssClass="alert-box alert ebiz-hopewiser-error" />
    <div class="panel ebiz-hopewiser-wrap">
        <h2><%= GetPageText("headerText")%></h2>
        <% =SearchResults%>
        <%= GetPageText("footerText")%>
    	<asp:HiddenField ID="NotFound" runat="server" Value="" />
        <div class="button-group ebiz-hopewiser-buttons-wrap ebiz-hopewiser-buttons-search-wrap">
            <asp:HyperLink ID="ButtonPrev" CssClass="button ebiz-close" runat="server"><%= GetPageText("prevButtonText")%></asp:HyperLink>
		  <asp:LinkButton ID="ButtonNext" CssClass="button ebiz-primary-action ebiz-next" OnClientClick="Javascript:return submitForm();" runat="server"><%= GetPageText("nextButtonText")%></asp:LinkButton>
        </div>
    </div>

    <script language="javascript" type="text/javascript">
        function submitForm() {
			var aItems = document.getElementsByName("address");
			var fields = new Array;
			var value  = "";
    		for (var i=0; i < aItems.length; i++){
    			if (aItems[i].checked == true){
    				value = aItems[i].value;
    				fields = value.split(',');
    				DoOpener(fields[0], fields[1], fields[2], fields[3], fields[4], fields[5], '<%= CountryString%>');
    				return true;
    			}
    		}
            ShowSelectAddressMessage();
	    	return false;
    	}	    		
        function DoOpener(sCompany, sAddress1, sAddress2, sTown, sCounty, sPostcode, sCountry) {
            window.opener.document.forms[0].hiddenAdr0.value = sCompany;
            window.opener.document.forms[0].hiddenAdr1.value = sAddress1;
            window.opener.document.forms[0].hiddenAdr2.value = sAddress2;
            window.opener.document.forms[0].hiddenAdr3.value = sTown;
            window.opener.document.forms[0].hiddenAdr4.value = sCounty;
            window.opener.document.forms[0].hiddenAdr5.value = sPostcode;
            window.opener.document.forms[0].hiddenAdr6.value = sCountry;
            window.opener.UpdateAddressFields();
            window.close();
        }
    </script>
    <% CreateJavascript() %>

</asp:Content>