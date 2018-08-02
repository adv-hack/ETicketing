using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using TalentBusinessLogic.Models.ApplicationModels.BaseModels;

namespace TalentBusinessLogic.Models
{
    public class CompanySearchViewModel : BaseViewModelForDataTables<CompanySearchLists>
    {
        public CompanySearchViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public CompanySearchViewModel() { }

    }
}
