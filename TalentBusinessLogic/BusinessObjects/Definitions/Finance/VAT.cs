using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.BusinessObjects.Definitions;
using TalentBusinessLogic.DataTransferObjects;
using TalentBusinessLogic.BusinessObjects;
using Talent;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.BusinessObjects.Definitions
{
    public class VAT : BusinessObjects
    {
        public List<TalentBusinessLogic.DataTransferObjects.VAT> retrieveVatCodes()
        {
            List<TalentBusinessLogic.DataTransferObjects.VAT> VatCodeList = new List<TalentBusinessLogic.DataTransferObjects.VAT>();

            TalentVatCodes talentVatCodes = new TalentVatCodes();
            talentVatCodes.Settings = Environment.Settings.DESettings;

            ErrorObj err = talentVatCodes.RetrieveVatCodes();
            ErrorModel errModel = Data.PopulateErrorObject(err, talentVatCodes.ResultDataSet, talentVatCodes.Settings, null);

            if (!errModel.HasError)
            {
                VatCodeList = Data.PopulateObjectListFromTable<TalentBusinessLogic.DataTransferObjects.VAT>(talentVatCodes.ResultDataSet.Tables["VATCodeListResults"]);
            }

            return VatCodeList;
        }

    }
}
