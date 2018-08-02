using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.UI.WebControls;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.BusinessObjects.Definitions;
using System.Data;
using Talent;
using Talent.Common;
using AutoMapper;
using System.Web.UI.WebControls;
//using TalentBusinessLogic.IoC;

namespace TalentBusinessLogic.ModelBuilders.CRM
{
    public class CompanyModelBuilders : BaseModelBuilder
    {
    #region Customer Contacts Page Builders

        public CompanyContactsViewModel PopulateCompanyContacts(CompanyContactsInputModel inputModel) 
        {
            CompanyContactsViewModel viewModel = new CompanyContactsViewModel(true);
            TalentCompany talentCompany = new TalentCompany();

            Mapper.CreateMap<CompanyContactsInputModel, DECompany>();
            talentCompany.Company = Mapper.Map<DECompany>(inputModel);
            
            talentCompany.Settings = Environment.Settings.DESettings;

            ErrorObj err =  talentCompany.RetrieveCustomersByCompanyId();

            viewModel.Error =  Data.PopulateErrorObject(err, talentCompany.ResultDataSet, talentCompany.Settings, null);

            if (!viewModel.Error.HasError)
            {
                viewModel.CompanyContacts = Data.PopulateObjectListFromTable<CompanyContactModel>(talentCompany.ResultDataSet.Tables["CompanyContactsResults"]);
            }

            return viewModel;
        }
    #endregion

    #region Company Update Page Builders
        public CompanyUpdateViewModel CompanyUpdate(CompanyUpdateInputModel inputModel)
        {
            // Create your view model with the basic settings
            CompanyUpdateViewModel viewModel = new CompanyUpdateViewModel(true);
            viewModel.CompanyOperationMode = inputModel.CompanyOperationMode;
            viewModel.AddParent = inputModel.AddParent;

            // Mode 'none', will just return the information to display the screen for create mode
            if (inputModel.CompanyOperationMode != GlobalConstants.CRUDOperationMode.None)
            {

                // Call Talent Common to perform the relevant action
                TalentCompany talentCompany = new TalentCompany();
                Mapper.CreateMap<CompanyUpdateInputModel, DECompany>();
                talentCompany.Company = Mapper.Map<DECompany>(inputModel);
                talentCompany.Settings = Environment.Settings.DESettings;

                // Removing parent has a different route
                if (inputModel.CompanyOperationMode == GlobalConstants.CRUDOperationMode.Delete)
                {
                    ErrorObj err = talentCompany.ParentCompanyOperations();
                    viewModel.Error = Data.PopulateErrorObject(err, talentCompany.ResultDataSet, talentCompany.Settings, null);
                }
                else
                {
                    ErrorObj err = talentCompany.CompanyOperations();
                    viewModel.Error = Data.PopulateErrorObject(err, talentCompany.ResultDataSet, talentCompany.Settings, null);

                    // We want to populate the company details when no error exists, or a Talent thrown error as we still need the company details
                    // at that point
                    if ((!viewModel.Error.HasError) || String.IsNullOrWhiteSpace(viewModel.Error.ReturnCode))
                    {
                        viewModel.Company = Data.PopulateObjectListFromTable<CompanyModel>(talentCompany.ResultDataSet.Tables["CompanySearchResults"]).FirstOrDefault();
                        if (viewModel.Company != null)
                        {
                            viewModel.Company.CompanyNumber = talentCompany.ResultDataSet.Tables["CompanyDetails"].Rows[0]["CompanyNumber"].ToString();
                        }
                    }
                
                }
            }

            // Return the list of valid agents and vat codes in the view to the caller.
            viewModel.VatCodeList = new VAT().retrieveVatCodes();
            TalentBusinessLogic.DataTransferObjects.VAT vatLstItem = new TalentBusinessLogic.DataTransferObjects.VAT();
            vatLstItem.VATCode = viewModel.GetPageText("NoVATCodeText");
            vatLstItem.VATUniqueID = 0;
            viewModel.VatCodeList.Insert(0, vatLstItem);

            // Return the list of valid agents and vat codes in the view to the caller.
            viewModel.AgentList = new Agent().retrieveAgents();
            TalentBusinessLogic.DataTransferObjects.Agent agentLstItem = new TalentBusinessLogic.DataTransferObjects.Agent();
            agentLstItem.Usercode = viewModel.GetPageText("NoOwningAgentText");
            viewModel.AgentList.Insert(0,agentLstItem);

            return viewModel;
        }

    #endregion

    #region Customer Company Page Builders
        /// <summary>
        /// Gets company from EM002SS if company is not in session
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        public CustomerCompanyViewModel ReadCompanyOperations(CustomerCompanyInputModel inputModel)
        {
            ErrorModel err = new ErrorModel();
            CustomerCompanyViewModel viewModel = new CustomerCompanyViewModel(true, inputModel.ControlCode);
            KeyValuePair<string, string> companyInSession = new KeyValuePair<string, string>();
            // get company from session
            if (Data.Session.Get("Company") != null)
            {
                companyInSession = (KeyValuePair<string, string>)Data.Session.Get("Company");
            }
            if (string.IsNullOrEmpty(inputModel.CompanyNumber))
            {
                //error if no company number from input model
                err.HasError = true;
                err.ErrorMessage = "Missing company number.";
                viewModel.Error = err;
            }
            else if (inputModel.CompanyNumber == companyInSession.Key)
            {
                //is company id in session same as input company id
                viewModel.Company = new CompanyModel();
                viewModel.Company.CRMCompanyName = companyInSession.Value;
                viewModel.Company.CompanyNumber = companyInSession.Key;
            }
            else
            {
                // get company if company is not found in session
                TalentCompany talentCompany = new TalentCompany();
                Mapper.CreateMap<CustomerCompanyInputModel, DECompany>();
                talentCompany.Company = Mapper.Map<DECompany>(inputModel);
                talentCompany.Settings = Environment.Settings.DESettings;
                talentCompany.Company.OwningAgent = talentCompany.Settings.AgentEntity.AgentUsername;

                ErrorObj errorOb = talentCompany.CompanyOperations();

                viewModel.Error = Data.PopulateErrorObject(errorOb, talentCompany.ResultDataSet, talentCompany.Settings, null);

                if (!viewModel.Error.HasError)
                {
                    viewModel.Company = Data.PopulateObjectListFromTable<CompanyModel>(talentCompany.ResultDataSet.Tables["CompanySearchResults"]).FirstOrDefault();
                    if (!string.IsNullOrEmpty(talentCompany.ResultDataSet.Tables["CompanyDetails"].Rows[0][0].ToString())) 
                    {
                        viewModel.Company.CompanyNumber = talentCompany.ResultDataSet.Tables["CompanyDetails"].Rows[0][0].ToString();
                    }
                    Data.Session.Add("Company", new KeyValuePair<string, string>(viewModel.Company.CompanyNumber, viewModel.Company.CRMCompanyName));
                }
            }

            return viewModel;
        }
        public ErrorModel RemoveCustomerCompanyAssociation(CustomerCompanyInputModel inputModel)
        {
            TalentCompany talentCompany = new TalentCompany();
            Mapper.CreateMap<CustomerCompanyInputModel, DECompany>();
            talentCompany.Company = Mapper.Map<DECompany>(inputModel);
            talentCompany.Company.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Delete;
            
            talentCompany.Settings = Environment.Settings.DESettings;
            ErrorObj err = talentCompany.ProcessCompanyContacts();

            return Data.PopulateErrorObject(err, talentCompany.ResultDataSet, talentCompany.Settings, null);
        }
        public ErrorModel AddCustomerCompanyAssociation(CustomerCompanyInputModel inputModel)
        {
            TalentCompany talentCompany = new TalentCompany();
            Mapper.CreateMap<CustomerCompanyInputModel, DECompany>();
            talentCompany.Company = Mapper.Map<DECompany>(inputModel);
            talentCompany.Company.CompanyOperationMode = GlobalConstants.CRUDOperationMode.Create;

            talentCompany.Settings = Environment.Settings.DESettings;
            ErrorObj err = talentCompany.ProcessCompanyContacts();

            return Data.PopulateErrorObject(err, talentCompany.ResultDataSet, talentCompany.Settings, null);
        }
    #endregion
    }
}
