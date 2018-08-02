using System;
using TalentBusinessLogic.Models;
using Talent.Common;
using AutoMapper;
using System.Data;
using System.Linq;


namespace TalentBusinessLogic.ModelBuilders
{
    public class TicketExchangeProductsBuilder : BaseModelBuilder
    {

        public TicketExchangeProductsViewModel GetTicketExchangeProductsListForCustomer(TicketExchangeProductsInputModel inputModel)
        {
            TicketExchangeProductsViewModel viewModel = new TicketExchangeProductsViewModel(true);

            TalentTicketExchange ticketExchange = new TalentTicketExchange();
            ticketExchange.Dep.Customer = inputModel.CustomerNumber;
            ticketExchange.Dep.FromDate = inputModel.FromDate;

            ticketExchange.Settings = Environment.Settings.DESettings;
            ErrorObj err = ticketExchange.GetTicketExchangeProductsListForCustomer();

            viewModel.Error = Data.PopulateErrorObject(err, ticketExchange.ResultDataSet, ticketExchange.Settings, 3);
            if (!viewModel.Error.HasError)
            {
                // List of product values from TicketExchangeProductSummary 
                viewModel.TicketExchangeProductSummaryList = Data.PopulateObjectListFromTable<TicketExchangeProductSummary>(ticketExchange.ResultDataSet.Tables["TicketExchangeProductSummary"]);
                // Total for this customer from TicketExchangeSummary
                if (ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows.Count > 0)
                {
                    viewModel.TotalExpiredOnTicketExchange = Utilities.CheckForDBNull_Int(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalExpiredOnTicketExchange"]);
                    viewModel.TotalPendingOnTicketExchange = Utilities.CheckForDBNull_Int(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalPendingOnTicketExchange"]);
                    viewModel.TotalSoldOnTicketExchange = Utilities.CheckForDBNull_Int(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalSoldOnTicketExchange"]);
                    viewModel.TotalReSalePricePending = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalReSalePricePending"]);
                    viewModel.TotalReSalePriceExpired = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalReSalePriceExpired"]);
                    viewModel.TotalReSalePriceSold = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalReSalePriceSold"]);
                    viewModel.TotalPotentialEarningPricePending = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalPotentialEarningPricePending"]);
                    viewModel.TotalPotentialEarningPriceExpired = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalPotentialEarningPriceExpired"]);
                    viewModel.TotalPotentialEarningPriceSold = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalPotentialEarningPriceSold"]);
                    viewModel.TotalHandlingFeePending = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalHandlingFeePending"]);
                    viewModel.TotalHandlingFeeExpired = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalHandlingFeeExpired"]);
                    viewModel.TotalHandlingFeeSold = Utilities.CheckForDBNull_Decimal(ticketExchange.ResultDataSet.Tables["TicketExchangeSummary"].Rows[0]["TotalHandlingFeeSold"]);
                }

            }
            return viewModel;
        }

    }
}
