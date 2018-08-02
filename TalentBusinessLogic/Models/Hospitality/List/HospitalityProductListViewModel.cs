
using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class HospitalityProductListViewModel : BaseViewModel
    {
        public HospitalityProductListViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public HospitalityProductListViewModel() { }
        public List<ProductDetails> ProductListDetailsHome { get; set; }
        public List<ProductDetails> ProductListDetailsSeason { get; set; }
        public List<ProductDetails> ProductListDetailByPackageId { get; set; }
        public string MinDateHome { get; set; }
        public string MaxDateHome { get; set; }
        public string MinDateSeason { get; set; }
        public string MaxDateSeason { get; set; }

    }
}
