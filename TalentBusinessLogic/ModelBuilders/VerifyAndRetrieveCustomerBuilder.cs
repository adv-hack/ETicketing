using TalentBusinessLogic.Models;
using Talent.Common;
using AutoMapper;
using System.Collections.Generic;

namespace TalentBusinessLogic.ModelBuilders
{
    public class VerifyAndRetrieveCustomerBuilder : BaseModelBuilder
    {
        public VerifyAndRetrieveCustomerViewModel VerifyAndRetrieveCustomer(VerifyAndRetrieveCustomerInputModel inputModel)
        {
            VerifyAndRetrieveCustomerViewModel viewModel = new VerifyAndRetrieveCustomerViewModel();
            TalentCustomer talentCustomer = new TalentCustomer();
        
            Mapper.CreateMap<VerifyAndRetrieveCustomerInputModel, DECustomer>();
            DECustomer customer = Mapper.Map<DECustomer>(inputModel);
            customer.Password = Talent.Common.Utilities.TripleDESEncode(Talent.Common.Utilities.RandomString(10), inputModel.NoiseEncryptionKey);
            customer.Source = "W";
            DECustomerV11 V11 = new DECustomerV11();
            V11.DECustomersV1.Add(customer);
            talentCustomer.DeV11 = V11;

            talentCustomer.Settings = Environment.Settings.DESettings;
            talentCustomer.Settings.TicketingKioskMode = true;
            talentCustomer.Settings.IsAgent = Environment.Settings.DESettings.IsAgent;

            ErrorObj err = talentCustomer.VerifyAndRetrieveCustomerDetails();
            viewModel.Error = Data.PopulateErrorObject(err, talentCustomer.ResultDataSet, talentCustomer.Settings,null);

            if (!viewModel.Error.HasError)
            {
                viewModel = Data.PopulateObjectFromRow<VerifyAndRetrieveCustomerViewModel>(talentCustomer.ResultDataSet.Tables["CustomerResults"].Rows[0]);
                // set session value for company
                if (!string.IsNullOrEmpty(viewModel.CompanyName) && !string.IsNullOrEmpty(viewModel.CompanyNumber))
                {
                    Data.Session.Add("Company", new KeyValuePair<string, string>(viewModel.CompanyNumber, viewModel.CRMCompanyName));
                }
                viewModel.Valid = true;
            }
            else 
            {
                viewModel.Valid = false;
            }

            return viewModel;
        }
    }
}
