using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class ProductGroupDetails
    {
        /// <summary>
        /// Hospitality product group code
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Hospitality product group stadium
        /// </summary>
        public string StadiumId { get; set; }

        /// <summary>
        ///  Hospitality product group description
        /// </summary>
        public string GroupDescription { get; set; }

        /// <summary>
        ///  Hospitality product group fixtures list
        /// </summary>
        public List<ProductDetails> ProductGroupFixturesList { get; set; }

        /// <summary>
        ///  Minimum fixtures required in Product Group
        /// </summary>
        public decimal MinimumFixturesRequired { get; set; }
    }
}
