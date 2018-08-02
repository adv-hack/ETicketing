using System;
using TalentBusinessLogic.Models;
using Talent.Common;
using System.Collections.Generic;
using AutoMapper;
using System.Data;
using TalentBusinessLogic.DataTransferObjects;
using Microsoft.VisualBasic;

namespace TalentBusinessLogic.ModelBuilders
{
    public class TicketExchangeSelectionBuilder : BaseModelBuilder
    {

        public TicketExchangeSelectionViewModel TicketingExchangeSelectionGetSeats(TicketExchangeSelectionInputModel inputModel)
        {
            TicketExchangeSelectionViewModel viewModel = new TicketExchangeSelectionViewModel(true);

            TalentTicketExchange ticketExchange = new TalentTicketExchange();
            ticketExchange.Dep.Customer = inputModel.CustomerNumber;
            ticketExchange.Dep.ProductCode = inputModel.ProductCode;

            ticketExchange.Settings = Environment.Settings.DESettings;
            ticketExchange.Settings.Cacheing = false;
            ErrorObj err = ticketExchange.GetTicketExchangeSeatSelectionForProduct();

            viewModel.Error = Data.PopulateErrorObject(err, ticketExchange.ResultDataSet, ticketExchange.Settings, null);
            if (!viewModel.Error.HasError)
            {
                if (ticketExchange.ResultDataSet.Tables["TicketExchangeCustomerSeats"].Rows.Count > 0 && ticketExchange.ResultDataSet.Tables["TicketExchangeProductInfomation"].Rows.Count>0 )
                {
                 // Product setting Values from TicketExchangeProductInfomation 
                viewModel = Data.PopulateObjectFromRow<TicketExchangeSelectionViewModel>(ticketExchange.ResultDataSet.Tables["TicketExchangeProductInfomation"].Rows[0], viewModel);

                // List of seats from TicketExchangeCustomerSeats 
                viewModel.TicketExchangeSeatList = Data.PopulateObjectListFromTable<TicketExchangeItem>(ticketExchange.ResultDataSet.Tables["TicketExchangeCustomerSeats"]);

                // Initialise original status from call. 
                viewModel.setOriginalWorkProperties();
                }
            }
            return viewModel;
        }

        public TicketExchangeViewModel TicketingExchangeSelectionConfirm(TicketExchangeInputModel inputModel)
        {
            TicketExchangeViewModel viewModel = new TicketExchangeViewModel(true);
            TalentTicketExchange ticketExchange = new TalentTicketExchange();
            ticketExchange.Dep.ListingCustomer = inputModel.ListingCustomerNumber;
            ticketExchange.Settings = Environment.Settings.DESettings;
            ticketExchange.Settings.Cacheing = false;
        
            int ticketItemCount = 0;
            foreach (TicketExchangeItem ticketExchangeSeat in inputModel.Tickets)
            {
                // Map ticket details
                DETicketExchangeItem deTicketExchangeItem = new DETicketExchangeItem();
                deTicketExchangeItem.ProductCode = ticketExchangeSeat.ProductCode;
                deTicketExchangeItem.SeatedCustomerNo = ticketExchangeSeat.SeatedCustomerNo;
                deTicketExchangeItem.PaymentOwnerCustomerNo = ticketExchangeSeat.PaymentOwnerCustomerNo;
                deTicketExchangeItem.FaceValuePrice = ticketExchangeSeat.FaceValuePrice;
                deTicketExchangeItem.OriginalPrice = ticketExchangeSeat.OriginalSalePrice;
                deTicketExchangeItem.RequestedPrice = ticketExchangeSeat.RequestedPrice;
                deTicketExchangeItem.Fee = ticketExchangeSeat.Fee;
                deTicketExchangeItem.FeeType = ticketExchangeSeat.FeeType;
                deTicketExchangeItem.PotentialEarning = ticketExchangeSeat.PotentialEarning;
                deTicketExchangeItem.Status = ticketExchangeSeat.Status;

                string[] comments = new string[2];
                comments[0] = ticketExchangeSeat.Comment1;
                comments[1] = ticketExchangeSeat.Comment2;
                deTicketExchangeItem.Comments = comments;

                // Map seat details
                DESeatDetails seat = new DESeatDetails();
                seat.Stand = ticketExchangeSeat.StandCode;
                seat.Area = ticketExchangeSeat.AreaCode;
                seat.Row = ticketExchangeSeat.RowNo;
                seat.Seat = ticketExchangeSeat.SeatNo;
                seat.AlphaSuffix = ticketExchangeSeat.AlphaSuffix;
                deTicketExchangeItem.SeatDetails = seat;

                ticketExchange.Dep.TicketExchangeItems.Add(deTicketExchangeItem);
                ticketItemCount = ticketItemCount + 1;
            }

            ErrorObj err = ticketExchange.SubmitTicketExchangeAction();
            viewModel.Error = Data.PopulateErrorObject(err, ticketExchange.ResultDataSet, ticketExchange.Settings, null, true);
            if (!viewModel.Error.HasError)
            {
                if (ticketExchange.ResultDataSet.Tables["TicketExchange"].Rows.Count > 0)
                {
                    viewModel = Data.PopulateObjectFromRow<TicketExchangeViewModel>(ticketExchange.ResultDataSet.Tables["TicketExchange"].Rows[0], viewModel);
                }
                return viewModel;
            }
            return viewModel;
        }
    }
}
