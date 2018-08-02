<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddressChange.aspx.vb" Inherits="PagesAgent_Profile_AddressChange" Title="Untitled Page" %>
<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
   
     <script language="javascript" type="text/javascript">

            function SelectAll()
            {
                $(".ebiz-select :checkbox").each(function (index)    {
                    this.setAttribute("checked", "checked");
                    this.checked = true;
            });
                               
        }

        function DeSelectAll()
            {
              $(".ebiz-select :checkbox").each(function (index) {
                  this.setAttribute("checked", ""); // For IE
                  this.removeAttribute("checked"); // For other browsers
                  this.checked = false;
            });
        }
      
    </script>

    <asp:BulletedList ID="ErrorList" runat="server" CssClass="error" />
     <h2>
       <asp:Literal ID="ltlAddressChangeInstructions"  runat="server" />
     </h2>
    <div class="row ebiz-address-changes-wrap">
        <div class="large-6 columns ebiz-old-address-wrap">
            <div class="panel">
                <asp:PlaceHolder ID="plholdAddressLine1" runat="server">
                    <div class="row ebiz-address-line-1-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="AddressLine1Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldAddressLine1" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdAddressLine2" runat="server">
                    <div class="row ebiz-address-line-2-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="AddressLine2Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldAddressLine2" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdAddressLine3" runat="server">
                    <div class="row ebiz-address-line-3-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="AddressLine3Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldAddressLine3" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdAddressLine4" runat="server">
                    <div class="row ebiz-address-county-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="AddressLine4Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldAddressLine4" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdAddressLine5" runat="server">
                    <div class="row ebiz-address-country-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="AddressLine5Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldAddressLine5" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdCountry" runat="server">
                    <div class="row ebiz-address-country-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="CountryLabel" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldCountry" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plholdPostcode" runat="server">
                    <div class="row ebiz-address-postcode-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="PostcodeLabel" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="oldPostcode" runat="server"  /> 
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        </div>
        <div class="large-6 columns ebiz-new-address-wrap">
            <div class="panel">
                      
            
                <asp:PlaceHolder ID="plhnewAddressLine1" runat="server">
                    <div class="row ebiz-address-line-1-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine1Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine1" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhnewAddressLine2" runat="server">
                    <div class="row ebiz-address-line-2-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine2Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine2" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhnewAddressLine3" runat="server">
                    <div class="row ebiz-address-line-3-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine3Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine3" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhnewAddressLine4" runat="server">
                    <div class="row ebiz-address-county-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine4Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine4" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhnewAddressLine5" runat="server">
                    <div class="row ebiz-address-country-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine5Label" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newAddressLine5" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                  <asp:PlaceHolder ID="plhnewCountry" runat="server">
                    <div class="row ebiz-address-country-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newCountryLabel" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newCountry" runat="server"  />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhnewPostcode" runat="server">
                    <div class="row ebiz-address-postcode-wrap">
                        <div class="medium-6 columns">
                            <asp:Literal ID="newPostCodeLabel" runat="server"  />
                        </div>
                        <div class="medium-6 columns">
                            <asp:Literal ID="newPostcode" runat="server"  /> 
                        </div>
                    </div>
                </asp:PlaceHolder>
            </div>
        
       
        </div>
    </div>

    <asp:Button ID="btnUpdate" runat="server" CssClass="button ebiz-update" />    

    <asp:Literal ID="ltlErrorMessage" runat="server" />
    <asp:Repeater ID="rptAddressChangeResults" runat="server">
        <HeaderTemplate>
            <table class="ebiz-address-change-results ebiz-reponsive-table">
                <thead>
                    <tr>
                        <asp:placeholder id ="plhchkSelectAll" runat ="server">    
                            <th scope="col" class="ebiz-select" >
                               <asp:Checkbox ID="chkSelectAll"  runat="server" ClientIDMode="Static" OnClick="selectAll(this.checked, '.ebiz-item-select');" /> 
                            </th>   
                        </asp:placeholder> 
                        <th scope="col" class="ebiz-customernumber"><%=CustomerNumberHeader%></th>
                        <th scope="col" class="ebiz-forname"><%=ForenameHeader%></th>
                        <th scope="col" class="ebiz-surname"><%=SurnameHeader%></th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
        <asp:placeholder id="plhchkSelectedItem" runat="server"  >
            <td class="ebiz-item-select" data-title="<%=SelectHeader%>">
                <asp:CheckBox ID="chkSelectedItem" runat="server" OnClick="validateSelAllChkBox(this.checked, '#chkSelectAll', '.ebiz-item-select');"></asp:Checkbox>
            </td>
        </asp:placeholder>
        <td class="ebiz-customernumber"><asp:Label ID="lblCustomerNumber" runat="server" Text='<%# Talent.Common.Utilities.CheckForDBNull_String(DataBinder.Eval(Container.DataItem, "CustomerNumber"))%>' /></td> 
        <td class="ebiz-forename" data-title="<%=ForenameHeader%>"><%# Eval("Forename")%></td>
        <td class="ebiz-surname" data-title="<%=SurnameHeader%>"><%# Eval("Surname")%></td>
        </tr>
        </ItemTemplate>
        <FooterTemplate>
        </tbody>
        </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:content>