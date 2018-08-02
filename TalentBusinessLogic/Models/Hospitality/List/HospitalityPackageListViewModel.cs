
using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class HospitalityPackageListViewModel : BaseViewModel
    {
        /// <summary>
        /// If a product code is passed into the inputmodel then the list of packages returned
        /// will be in PackageListDetailsByProductCode.
        /// If a product group code is passed into the input model then the list of packages returned will be in PackageListDetailsByProductGroup 
        /// Otherwise a complete list of PackageListDetailsHome and PackageListDetailsSeason
        /// will be populated
        /// </summary>
        public HospitalityPackageListViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public HospitalityPackageListViewModel() { }
        public List<PackageDetails> PackageListDetailsHome { get; set; }
        public List<PackageDetails> PackageListDetailsSeason { get; set; }
        public List<PackageDetails> PackageListDetailsByProductCode { get; set; }

        /// <summary>
        /// If a product group code is passed into the input model then the list of packages returned will be in PackageListDetailsByProductGroup 
        /// </summary>
        public List<PackageDetails> PackageListDetailsByProductGroup { get; set; }
        public int MinGroupSizeHome { get; set; }
        public int MinGroupSizeSeason { get; set; }
        public int MaxGroupSizeHome { get; set; }
        public int MaxGroupSizeSeason { get; set; }
        public decimal MinBudgetHome { get; set; }
        public decimal MinBudgetSeason { get; set; }
        public decimal MaxBudgetHome { get; set; }
        public decimal MaxBudgetSeason { get; set; }
        public string FormattedPackagePrice { get; set; }
        public string FormattedPackageGroupSizeDescription { get; set; }

    }
}
