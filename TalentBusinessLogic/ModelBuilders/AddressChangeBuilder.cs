using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using System.Data;
using Talent;
using Talent.Common;
using AutoMapper;


namespace TalentBusinessLogic.ModelBuilders.CRM
{
    public class AddressChangeBuilder : BaseModelBuilder 
    {
        public AddressChangeViewModel PopulateAddressChangeList(AddressChangeInputModel inputModel) 
        {
            AddressChangeViewModel viewModel = new AddressChangeViewModel(true);
            TalentCustomer talentCustomer = new TalentCustomer();

           Mapper.CreateMap<AddressChangeInputModel, DECustomer>();

           talentCustomer.Settings = Environment.Settings.DESettings;
           
           DECustomer deCust = new DECustomer();
           deCust = Mapper.Map<DECustomer>(inputModel);
           DECustomerV11 deCustV11 = new DECustomerV11();
           talentCustomer.DeV11.DECustomersV1.Add(deCust);
           
           ErrorObj err = talentCustomer.RetrieveCustomersAtAddress();

           viewModel.Error = Data.PopulateErrorObject(err, talentCustomer.ResultDataSet, talentCustomer.Settings, null);

            if (!viewModel.Error.HasError)
            {
                viewModel.AddressChange = Data.PopulateObjectListFromTable<AddressChangeModel>(talentCustomer.ResultDataSet.Tables["CustomersAtAddress"]);
            }

            return viewModel;
        }

        public AddressChangeSyncViewModel SyncAddress(AddressChangeSyncInputModel inputModel)
        {
            AddressChangeSyncViewModel viewModel = new AddressChangeSyncViewModel(inputModel);
            TalentCustomer talentCustomer = new TalentCustomer();

            Mapper.CreateMap<AddressChangeSyncInputModel, DECustomer>();

            talentCustomer.Settings = Environment.Settings.DESettings;

            DECustomer deCust = new DECustomer();
            deCust = Mapper.Map<DECustomer>(inputModel);


            DECustomerV11 deCustV11 = new DECustomerV11();
            talentCustomer.DeV11.DECustomersV1.Add(deCust);

            ErrorObj err = talentCustomer.UpdateCustomerAddresses();
            viewModel.Error = Data.PopulateErrorObject(err, talentCustomer.ResultDataSet, talentCustomer.Settings, null);

            //if (!viewModel.Error.HasError)
            //{
            //    viewModel.AddressChange = Data.PopulateObjectListFromTable<AddressChangeModel>(talentCustomer.ResultDataSet.Tables["CustomersAtAddress"]);
            //}

            return viewModel;
        }

    }
}
