using System;
using System.Collections.Generic;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using System.Data;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.ModelBuilders
{
    public class CompanySearchBuilder : BaseModelBuilder
    {

        public CompanySearchViewModel CompanySearch(CompanySearchInputModel inputModel)
        {
            CompanySearchViewModel viewModel = new CompanySearchViewModel();

            // Draw parameter needs to be returned to the datatable so we can determin if we need
            // to store the record count.
            viewModel.Draw = inputModel.Draw;

            TalentSearch talentSearch = new TalentSearch();

            Mapper.CreateMap<CompanySearchInputModel, DECompanySearch>();
            talentSearch.CompanySearch = Mapper.Map<DECompanySearch>(inputModel);

            Mapper.CreateMap<CompanySearchInputModel, DECompany>();
            talentSearch.CompanySearch.Company = Mapper.Map<DECompany>(inputModel);

            talentSearch.Settings = Environment.Settings.DESettings;
            ErrorObj err = talentSearch.PerformCompanySearch();

            CompanySearchLists SearchResults = new CompanySearchLists();

            SearchResults.Errors = Data.PopulateErrorObject(err, talentSearch.ResultDataSet, talentSearch.Settings, null);

            if (!SearchResults.Errors.HasError)
            {
                
                SearchResults.Companies = Data.PopulateObjectListFromTable<CompanySearch.Company>(talentSearch.ResultDataSet.Tables["SearchResults"]);

                // Populate the datatablelist in the base view model with our lists
                viewModel.DataTableList = new List<CompanySearchLists>();
                viewModel.DataTableList.Add(SearchResults);
                viewModel.RecordsFiltered = (int)talentSearch.ResultDataSet.Tables["StatusResults"].Rows[0]["RecordsReturned"];
                viewModel.RecordsTotal = SearchResults.Companies.Count;
            }
            return viewModel;
        }

    }
}
