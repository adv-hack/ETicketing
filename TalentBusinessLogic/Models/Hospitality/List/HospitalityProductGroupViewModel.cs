using System.Collections.Generic;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using Talent.Common;

namespace TalentBusinessLogic.Models
{
    public class HospitalityProductGroupViewModel : BaseViewModel
    {
        /// <summary>
        /// parametered constructor
        /// </summary>
        /// <param name="getContentAndAttributes"></param>
        public HospitalityProductGroupViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        /// <summary>
        /// Default constructor
        /// </summary>
        public HospitalityProductGroupViewModel() { }

        /// <summary>
        /// Hospitality product group list
        /// </summary>
        public List<ProductGroupDetails> ProductGroupDetailsList { get; set; }

        /// <summary>
        /// Product group fixtures list
        /// </summary>
        public List<ProductDetails> ProductGroupFixturesList { get; set; }

    }
}
