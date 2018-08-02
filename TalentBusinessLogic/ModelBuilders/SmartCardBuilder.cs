using System;
using TalentBusinessLogic.Models;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.ModelBuilders
{
    public class SmartCardBuilder : BaseModelBuilder
    {
        public SmartCardAPIViewModel PopulateTeamCardAPIViewModelObject(SmartCardAPIInputModel inputModel)
        {
            SmartCardAPIViewModel viewModel = new SmartCardAPIViewModel();
            TalentSmartcard talentSmartCard = new TalentSmartcard();

            Mapper.CreateMap<SmartCardAPIInputModel, DESmartcard>();
            talentSmartCard.DE =  Mapper.Map<DESmartcard>(inputModel);
            talentSmartCard.Settings = Environment.Settings.DESettings;

            ErrorObj err = talentSmartCard.RequestPrintCard();

            viewModel.Error = Data.PopulateErrorObject(err, talentSmartCard.ResultDataSet, talentSmartCard.Settings, null);

            if (!viewModel.Error.HasError) 
            {
                viewModel = Data.PopulateObjectFromRow<SmartCardAPIViewModel>(talentSmartCard.ResultDataSet.Tables["TeamCardResults"].Rows[0]);
            }

            return viewModel;
        }    
    }
}
