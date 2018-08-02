using System;
using System.Collections.Generic;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using System.Data;
using Talent.Common;
using AutoMapper;
//using TalentBusinessLogic.IoC;

namespace TalentBusinessLogic.ModelBuilders
{
    public class CustomerSearchBuilder : BaseModelBuilder
    {
        // PopulateCustomerSearchViewModelObjectLists - CustomerSearch
        public CustomerSearchViewModel CustomerSearch(CustomerSearchInputModel inputModel)
        {
            CustomerSearchViewModel viewModel = new CustomerSearchViewModel();

            // Draw parameter needs to be returned to the datatable so we can determin if we need
            // to store the record count.
            inputModel.Source = "W";
            viewModel.Draw = inputModel.Draw;

            TalentSearch talentSearch = new TalentSearch();

            Mapper.CreateMap<CustomerSearchInputModel, DECustomerSearch>();
            talentSearch.CustomerSearch = Mapper.Map<DECustomerSearch>(inputModel);

            Mapper.CreateMap<CustomerSearchInputModel, DECustomer>();
            talentSearch.CustomerSearch.Customer = Mapper.Map<DECustomer>(inputModel);

            talentSearch.Settings = Environment.Settings.DESettings;
            ErrorObj err = talentSearch.PerformCustomerSearch();

            CustomerSearchLists SearchResults = new CustomerSearchLists();

            SearchResults.Errors = Data.PopulateErrorObject(err, talentSearch.ResultDataSet, talentSearch.Settings, null);

            if (!SearchResults.Errors.HasError)
            {
                
                SearchResults.Customers = Data.PopulateObjectListFromTable<CustomerSearch.Customers>(talentSearch.ResultDataSet.Tables["SearchResults"]);

                if (SearchResults.Customers != null && SearchResults.Customers.Count > 0)
                {
                    if (!string.IsNullOrEmpty(inputModel.AgentType) && !string.IsNullOrEmpty(inputModel.AgentLoginID)) 
                    {
                        SearchResults.Clubs = GetClubDetails(inputModel);
                    }
                }

                // Populate the datatablelist in the base view model with our lists
                viewModel.DataTableList = new List<CustomerSearchLists>();
                viewModel.DataTableList.Add(SearchResults);
                viewModel.RecordsFiltered = (int)talentSearch.ResultDataSet.Tables["StatusResults"].Rows[0]["RecordsReturned"];
                viewModel.RecordsTotal = SearchResults.Customers.Count;

            }

            return viewModel;
        }

        private List<CustomerSearch.Clubs> GetClubDetails(CustomerSearchInputModel inputModel)
        {
            TalentDataObjects dataObjects = new TalentDataObjects();
            dataObjects.Settings = Environment.Settings.DESettings;
            List<CustomerSearch.Clubs> clubs = new List<CustomerSearch.Clubs>();

            DataTable partnerClub = dataObjects.ClubSettings.GetPartnerUserClubByAgentTypeLoginID(inputModel.AgentType, inputModel.AgentLoginID);
            if (partnerClub.Rows.Count > 0)
            {
                clubs = Data.PopulateObjectListFromTable<CustomerSearch.Clubs>(dataObjects.ClubSettings.GetTblClubDetailsByAgentType(inputModel.AgentType));
            }
            return clubs;
        }


    }
}
