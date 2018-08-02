<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CompanyContacts.aspx.vb" Inherits="PagesPublic_CRM_CompanyContacts" title="Untitled Page" %>
<%@ Register Src="~/UserControls/Company/CustomerCompany.ascx" TagPrefix="Talent" TagName="CustomerCompany" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">  
    <script>
        $(document).ready(function () {
            $("#clientside-errors-wrapper").hide();
            var allowDelete = document.getElementById('hdfAllowDelete').value;
            var companyContactTable = $('.ebiz-company-contacts-results').DataTable({
                rowCallback: function (row, data, index) {
                    $('td:eq(4)', row).html('<a onClick="selectCustomer(\'' + data[0] + '\');">Select</a>'); // Customer select
                    if (allowDelete == "true") {
                        $('td:eq(5)', row).html('<a onClick="removeCustomerCompanyAssociation(\'' + data[0] + '\');">Delete</a>'); // Remove customer company association
                    }
                }
            });
            if (allowDelete != "true") {
                companyContactTable.columns('.column-show-delete').visible(false); 
            } 
        });

        function removeCustomerCompanyAssociation(customerNumber)
        {
            var contact = {
                SessionID: document.getElementById('hdfSessionID').value,
                CustomerNumber: customerNumber,
                CompanyNumber: document.getElementById('hdfCompanyNumber').value,
                AgentName: document.getElementById('hdfAgentName').value
            };
           
            $.ajax({
                type: "DELETE",
                url: "<%=TalentAPIAddress%>/CustomerCompany/DeleteContact" + '?' + $.param(contact),
                dataType: "json",
                cache: false,
                success: function (viewModel) {
                    if (viewModel.Error && viewModel.Error.HasError)
                    {
                        handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", viewModel.Error);
                    }
                    else
                    {
                        location.reload();
                    }
                    
                    //var test = result;
                },
                error: function (xhr, status) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", xhr.status + '  ' + xhr.statusText);
                }
            });
        }
        function selectCustomer(customerNumber)
        {
            var parameters = {
                SessionID: document.getElementById('hdfSessionID').value,
                customerNumber: customerNumber
            };
            $.ajax({
                type: "POST",
                url: "CompanyContacts.aspx/GetRedirectUrl",
                data: JSON.stringify(parameters),
                contentType: "application/json",
                dataType: "json",
                cache: false,
                success: function (result) {
                    window.location.replace(document.getElementById('hdfRootURL').value + result.d);
                },
                error: function (xhr, status) {
                    handleError("#clientside-errors", "#clientside-errors-wrapper", "#errorList", status + '  ' + xhr.responseText);    
                }


            });
        }
    </script>
    <asp:PlaceHolder ID="plhErrorList" runat="server"  visible="False">
        <div class="alert-box alert">
            <asp:BulletedList ID="errorList" runat="server" ClientIDMode="Static" />
        </div>
    </asp:PlaceHolder>
    <div class="alert-box alert" id ="clientside-errors-wrapper">
        <ul id="clientside-errors"></ul>
    </div>
    <div class="button-group ebiz-company-actions-wrap">
        <asp:Hyperlink ID="hplSearch" runat="server" CssClass="button ebiz-primary-action ebiz-back-source" Visible ="true"/>
    </div>
    <Talent:CustomerCompany runat="server" id="CustomerCompany" />
    <asp:HiddenField ID="hdfCompanyNumber" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAgentName" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hdfAllowDelete" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ClientIDMode="Static" ID="hdfRootURL" runat="server" /> 
    <asp:Repeater ID="rptCompanyContactResults" runat="server">
        <HeaderTemplate>
            <div class="panel ebiz-company-contacts-results-wrap">
                <table class="ebiz-company-contacts-results ebiz-responsive-table">
                    <thead>
                        <tr>
                            <th scope="col" class="ebiz-name"><%=CustomerNumberHeader%></th>
                            <th scope="col" class="ebiz-forename"><%=ForenameHeader%></th>
                            <th scope="col" class="ebiz-surname"><%=SurnameHeader%></th>
                            <th scope="col" class="ebiz-telephone"><%=TelephoneHeader%></th>
                            <th scope="col" class="ebiz-select"><%=SelectHeader%></th>
                            <th scope="col" class="ebiz-delete column-show-delete"><%=DeleteHeader%></th>
                        </tr>
                    </thead>
                    <tbody>
        </HeaderTemplate>
         <ItemTemplate>
             <tr>
                 <td class="ebiz-name" data-title="<%=CustomerNumberHeader%>"><%# Eval("CustomerNumber")%></td>
                 <td class="ebiz-forename" data-title="<%=ForenameHeader%>"><%# Eval("Forename")%></td>
                 <td class="ebiz-surname" data-title="<%=SurnameHeader%>"><%# Eval("Surname")%></td>
                 <td class="ebiz-telephone" data-title="<%=TelephoneHeader%>"><%# Eval("Telephone")%></td>
                 <td class="ebiz-select"></td>
                 <td class="ebiz-delete"></td>
             </tr>
         </ItemTemplate>
        <FooterTemplate>
                    </tbody>
                </table>
            </div>
        </FooterTemplate>
    </asp:Repeater>
    <div class="button-group ebiz-company-contacts-actions-wrap">
        <asp:Hyperlink ID="hplAddExistingCustomer" runat="server" CssClass="button ebiz-primary-action ebiz-existing-customer" Visible ="false"/>
        <asp:Button ID="btnRegisterAndAddContact" runat="server" CssClass="button ebiz-primary-action" />
        <asp:Button ID="btnAddLoggedInCustomer" runat="server" CssClass="button ebiz-primary-action" Visible="False" />
    </div>
    <script type="text/javascript" src="../../JavaScript/Application/Status/error-handling.js"></script>
</asp:Content>

