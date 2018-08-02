using System;
using TalentBusinessLogic.Models;
using Talent.Common;

namespace TalentBusinessLogic.ModelBuilders
{
    public class AddressLabelPrintBuilder : BaseModelBuilder
    {
        public AddressLabelViewModel PrintAddressLabel(AddressLabelInputModel inputModel) 
        { 
            AddressLabelViewModel viewModel = new AddressLabelViewModel();
            TalentAddressing talentAddressing = new TalentAddressing();

            talentAddressing.De.CustomerNumber = Utilities.PadLeadingZeros(inputModel.CustomerNumber, 12);  
            talentAddressing.Settings = Environment.Settings.DESettings;
            ErrorObj err = talentAddressing.PrintAddressLabel();

            viewModel.Status = Data.PopulateErrorObject(err, talentAddressing.ResultDataSet, talentAddressing.Settings, null);
            
            return viewModel;

        }
    }
}
