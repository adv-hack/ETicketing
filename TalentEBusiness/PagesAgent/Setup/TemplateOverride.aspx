<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TemplateOverride.aspx.vb" Inherits="PagesAgent_Setup_TemplateOverride" validateRequest="false" %>

<%@ Register Src="~/UserControls/PageHeaderText.ascx" TagName="PageHeaderText" TagPrefix="Talent" %>
<%@ Register Src="~/UserControls/HTMLInclude.ascx" TagName="HTMLInclude" TagPrefix="Talent" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
    <Talent:PageHeaderText ID="PageHeaderText1" runat="server" />
    <Talent:HTMLInclude ID="HTMLInclude1" runat="server" Usage="2" Sequence="1" />    

   <asp:PlaceHolder ID="plhTemplateOverrideErrors" runat="server">
      <div class="alert-box alert"><asp:Literal ID="ltlTemplateOverrideErrors" runat="server" /></div>
   </asp:PlaceHolder> 
    <asp:PlaceHolder ID="plhSuccessMessage" runat="server">
        <div class="alert-box success"><asp:Literal ID="ltlSuccessMessage" runat="server" /></div>
    </asp:PlaceHolder>
  <div class="row">
    <div class="column">
      <div class="panel ebiz-default-list-bu-selector">
        <div class="row">
          <div class="medium-3 columns">
              <asp:Label class="medium-only-middle" ID="lblBusinessUnit" runat="server"/>             
          </div>
          <div class="medium-9 columns">
            <asp:DropDownList ID="ddlBusinessUnit" runat="server" autoPostBack="true"  />
            
          </div>
        </div>
      </div>

      <div class="panel">
        <div class="button-group mb3">       
             <asp:PlaceHolder ID="plhOpenCreateTemplateOverride" runat="server">
               <a data-open="createTemplateOverrideModal" class="button" onclick="addTemplateOverrideClick()">
                    <i class="fa fa-plus" aria-hidden="true"></i> <asp:Literal ID="ltlAddTemplateOverride" runat="server" />
               </a> 
             </asp:PlaceHolder>           
        </div>        
        <asp:PlaceHolder ID="plhTemplateListWarning" runat="server">
           <div class="alert-box warning"><asp:Literal ID="ltlTemplateListWarning" runat="server" /></div>
        </asp:PlaceHolder>
         
        <!-- Add template overide reveal -->        
        <div class="reveal" id="createTemplateOverrideModal" data-reveal>
          <h1><asp:Literal ID="ltlTemplateOverrideHeader" runat="server" /></h1> 
            <asp:PlaceHolder ID="plhCreateTemplateOverrideErrors" runat="server">
                <div class="alert-box alert"><asp:Literal ID="ltlCreateTemplateOverrideErrors" runat="server" /></div>
            </asp:PlaceHolder>   
            <div ID="createTemplateOverrideErrors">
                <div class="alert-box alert"><asp:Label ID="lblCreateTemplateOverrideError"/></div>
            </div>
            <div class="row">
               <div class="column">              
                <asp:Label ID="lblTemplateOverrideDescription" runat="server" AssociatedControlID="txtTemplateOverrideDescription"  />
                <asp:TextBox ID="txtTemplateOverrideDescription" runat="server" ClientIDMode="Static" />              
               </div>
            </div>
            <asp:Label ID="lblProductPackageSpecific" runat="server" />          
            <div class="switch">          
              <input class="switch-input" id="yes-no" type="checkbox" name="Switch">
              <label class="switch-paddle" for="yes-no">
                <span class="show-for-sr"><%=ProductSpecificText%></span>
                <span class="switch-active" aria-hidden="true"><%=ProductPackageSpecificYesText%></span>
                <span class="switch-inactive" aria-hidden="true"><%=ProductPackageSpecificNoText%></span>
              </label>                         
            </div>
            <div class="js-product-specific-override" style="display: none;">
              <div class="row">
                <div class="column">
                  <asp:Label ID="lblProductGroup" runat="server" />            
                  <select class="js-select2--tags-no-creation" id="ddlProductGroup" multiple="true" runat="server" ClientIDMode="Static"></select>                              
                </div>
              </div>
              <div class="row">
                <div class="column">
                  <asp:Label ID="lblPackage" runat="server" />
                    <select class="js-select2--tags-no-creation" id="ddlPackage" multiple="true" runat="server" ClientIDMode="Static" ></select> 
                </div>
              </div>
            </div>
            <div class="js-non-product-specific-override">
              <div class="row">
                <div class="column">
                  <asp:Label ID="lblProductSubType" runat="server" /> 
                  <select class="js-select2--tags-no-creation" id="ddlProductSubType" multiple="true" runat="server" ClientIDMode="Static" ></select>                    
                </div>
              </div>
              <div class="row">
                <div class="column">                  
                    <asp:Label ID="lblProductType" runat="server" /> 
                    <select class="js-select2--tags-no-creation" id="ddlProductType" multiple="true" runat="server" ClientIDMode="Static" ></select>                
                </div>
              </div>
              <div class="row">
                <div class="column">
                    <asp:Label ID="lblStadium" runat="server" />                
                    <select class="js-select2--tags-no-creation" id="ddlStadium" multiple="true" runat="server" ClientIDMode="Static" ></select>                
                </div>
              </div>
            </div>
            <div class="row">
            <div class="column">
                <asp:Label ID="lblEmailConfirmation" runat="server" />                              
                <asp:DropDownList ID="ddlEmailConfirmation" class="js-select2--default" runat="server" ClientIDMode="Static"/>                    
            </div>
          </div>
            <div class="row">
            <div class="column">
                <asp:Label ID="lblQAndATemplate" runat="server" /> 
                <asp:DropDownList ID="ddlQAndATemplate" runat="server" class="js-select2--default" ClientIDMode="Static"/>
            </div>
          </div>
            <div class="row">
            <div class="column">
                <asp:Label ID="lblDataCaptureTemplateLabel" runat="server" />
                <asp:DropDownList ID="ddlDataCaptureTemplate" runat="server" class="js-select2--default" ClientIDMode="Static"/>
            </div>
          </div>
            <div class="row">
            <div class="columns small-6">
              <div class="button-group">
                 <button class="button ebiz-muted-action" data-close aria-label="Close modal" type="button" OnClick="clearControls();"><%=CancelButtonText%></button>
                 <asp:Button ID="btnAdd" runat="server" class="button ebiz-primary-action" OnClientClick="return validateCreateTemplateOverride();" />
              </div>
            </div>
          </div> 
            <button class="close-button" data-close aria-label="Close modal" type="button" OnClick="clearControls();">
             <i class="fa fa-times" aria-hidden="true"></i>
          </button>
        </div>

          <!-- Edit template overide reveal example -->
         <asp:PlaceHolder ID="plhEditTemplateOverride" runat="server" Visible="true">
             <div class="reveal" id="editTemplateOverrideModal" data-reveal>              
                 <h1><asp:Literal ID="ltlTemplateOverrideForEdit" runat="server"/></h1> 
                 <div ID="updateTemplateOverrideErrors">
                    <div class="alert-box alert"><asp:Label ID="lblUpdateTemplateOverrideError"/></div>
                 </div>
                 <div class="row">
                     <div class="column">              
                          <asp:Label ID="lblTemplateDescriptionForEdit" runat="server" AssociatedControlID="txtTemplateDescriptionForEdit"/>
                          <asp:TextBox ID="txtTemplateDescriptionForEdit" runat="server" ClientIDMode="Static" />              
                     </div>
                 </div>
            
                 <asp:Label ID="lblProductPackageSpecificForEdit" runat="server"/>          
                 <div class="switch">          
                      <input class="switch-input" id="yes-no-for-update" type="checkbox" name="Switch">
                      <label class="switch-paddle" for="yes-no-for-update">
                            <span class="show-for-sr"><%=ProductSpecificText%></span>
                            <span class="switch-active" aria-hidden="true"><%=ProductPackageSpecificYesText%></span>
                            <span class="switch-inactive" aria-hidden="true"><%=ProductPackageSpecificNoText%></span>
                      </label>                         
                </div>
            
                <div class="js-product-specific-override" style="display: none;">
                    <div class="row">
                        <div class="column">
                             <asp:Label ID="lblProductGroupForEdit" runat="server"/>            
                             <select class="js-select2--tags-no-creation" id="ddlProductGroupForEdit" multiple="true" runat="server" ClientIDMode="Static"></select>                              
                        </div>
                    </div>
                    <div class="row">
                         <div class="column">
                              <asp:Label ID="lblPackageForEdit" runat="server"/>
                              <select class="js-select2--tags-no-creation" id="ddlPackageForEdit" multiple="true" runat="server" ClientIDMode="Static"></select> 
                         </div>
                    </div>
                </div>
                <div class="js-non-product-specific-override">
                     <div class="row">
                         <div class="column">
                              <asp:Label ID="lblProductSubTypeForEdit" runat="server" /> 
                              <select class="js-select2--tags-no-creation" id="ddlProductSubTypeForEdit" multiple="true" runat="server" ClientIDMode="Static"></select>                    
                         </div>
                     </div>
                     <div class="row">
                          <div class="column">                  
                               <asp:Label ID="lblProductTypeForEdit" runat="server"/> 
                               <select class="js-select2--tags-no-creation" id="ddlProductTypeForEdit" multiple="true" runat="server" ClientIDMode="Static"></select>                
                          </div>
                     </div>
                     <div class="row">
                          <div class="column">
                               <asp:Label ID="lblStadiumForEdit" runat="server" />                
                               <select class="js-select2--tags-no-creation" id="ddlStadiumForEdit" multiple="true" runat="server" ClientIDMode="Static"></select>                
                          </div>
                     </div>
                </div>

                <div class="row">
                    <div class="column">
                        <asp:Label ID="lblEmailTemplateForEdit" runat="server" />                              
                        <asp:DropDownList ID="ddlEmailTemplateForEdit" runat="server" class="js-select2--default" ClientIDMode="Static"/>                    
                    </div>
                </div>
                <div class="row">
                     <div class="column">
                          <asp:Label ID="lblQandATemplateForEdit" runat="server" />
                          <asp:DropDownList ID="ddlQandATemplateForEdit" runat="server" class="js-select2--default" ClientIDMode="Static"/>
                     </div>
                </div>
                <div class="row">
                     <div class="column">
                          <asp:Label ID="lblDataCaptureTemplateForEdit" runat="server" /> 
                          <asp:DropDownList ID="ddlDataCaptureTemplateForEdit" runat="server" class="js-select2--default" ClientIDMode="Static"/>
                     </div>
                </div>
           
                <div class="row">
                     <div class="columns small-6">
                            <div class="button-group">
                                  <button class="button ebiz-muted-action" data-close aria-label="Close modal" type="button" OnClick="clearControls();"><%=CancelButtonText%></button>
                                  <asp:Button ID="btnUpdate" runat="server" ClientIDMode="Static" class="button ebiz-primary-action" OnClientClick="return validateUpdateTemplateOverride();"/>
                            </div>
                     </div>
                     <div class="columns small-6 text-right">
                          <div class="button-group">
                               <asp:Button ID="btnDelete" runat="server" ClientIDMode="Static" class="button ebiz-primary-action" />
                          </div>
                     </div>
                 </div> 
                 <button class="close-button" data-close aria-label="Close modal" type="button" OnClick="clearControls();">
                        <i class="fa fa-times" aria-hidden="true"></i>
                  </button>
            </div>
      </asp:PlaceHolder>

         <asp:PlaceHolder ID="plhTemplateoverrides" runat="server">
            <asp:repeater ID="rptTemplateOverrides" runat="server">
               <HeaderTemplate>
                  <table class="ebiz-responsive-table">
					 <thead>
						<tr>
                            <th scope="col" ><%=TemplateNameColumnHeading%></th>
					        <th scope="col"><%=TemplateCriteriaColumnHeading%></th>
					        <th scope="col"><%=TemplateColumnHeading %></th>
						</tr>
					 </thead>
					 <tbody>
                 </HeaderTemplate>
                <ItemTemplate>
                   <tr>
                       <td>                         
                           <a data-open="editTemplateOverrideModal" onClick="populateUpdateModal(<%# Eval("TemplateOverrideId")%> ,'<%#Server.HtmlEncode(Convert.ToString(DataBinder.Eval(Container.DataItem, "Description")).Replace("'", "\'")).Replace("\","\\") %>', '<%# Convert.ToBoolean(Eval("ProductPacakgeSpecific")).ToString().ToLower()%>','<%# Eval("ProductCriterias")%>','<%# Eval("PackageCriterias")%>','<%# Eval("ProductSubTypeCriterias")%>','<%# Eval("ProductTypeCriterias")%>','<%# Eval("StadiumCriterias")%>', <%# Eval("SaleConfirmationEmailId")%>, <%# Eval("QAndATemplateId")%>, <%# Eval("DataCaptureTemplateId")%>)">
                                 <%#Server.HtmlEncode(Convert.ToString(DataBinder.Eval(Container.DataItem, "Description"))) %>
                           </a>                                                      
                       </td>                       
                       <td>
                         <ul class="no-bullet">
                           <asp:repeater ID="rptOverrideCriteria" runat="server">
                              <ItemTemplate>
                                <li title="<%# Eval("CriteriaTypeDescription")%>">
                                    <%# Eval("CriteriaValues")%>
                                </li>
                              </ItemTemplate>
                           </asp:repeater>                       
                         </ul>
                       </td>
                       <td>
                          <ul class="no-bullet">
                             <asp:PlaceHolder ID="plhEmailTemplate" runat="server">
                                 <li><asp:Label id="lblEmailTemplate" runat="server" ><i class="fa fa-envelope-o fa-fw" aria-hidden="true"></i><%# Eval("SaleConfirmationEmailDescription")%></asp:Label> </li>
                             </asp:PlaceHolder>
                              <asp:PlaceHolder ID="plhQandATemplate" runat="server">
                                  <li><asp:Label id="lblQandATemplate" runat="server"><i class="fa fa-question fa-fw" aria-hidden="true"></i><%# Eval("QAndATemplateDescription")%></asp:Label></li>
                              </asp:PlaceHolder>
                              <asp:PlaceHolder ID="plhDataCaptureTemplate" runat="server">
                                  <li><asp:Label id="lblDataCaptureTemplate" runat="server"><i class="fa fa-check-square-o fa-fw" aria-hidden="true"></i><%# Eval("DataCaptureTemplateDescription")%></asp:Label> </li>               
                              </asp:PlaceHolder>                          
                          </ul>
                       </td>                       
                   </tr>
                </ItemTemplate>
                <FooterTemplate>
                   </tbody>
                 </table>
                </FooterTemplate>
             </asp:repeater>
         </asp:PlaceHolder>      
      </div>
    </div>
  </div>
  <asp:HiddenField ID="hdfTemplateOverrideType" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfBusinessUnit" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfCreateTemplateOverrideErrorMessage" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfUpdateTemplateOverrideErrorMessage" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfTemplateNameRequiredErrorMessage" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfCreateTemplateOverrideHasError" runat="server" ClientIDMode="Static" />
  <asp:HiddenField ID="hdfTemplateOverrideId" runat="server" ClientIDMode="Static" />
</asp:content>
