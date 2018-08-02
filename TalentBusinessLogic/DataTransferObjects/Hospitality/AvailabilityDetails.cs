using System.Collections.Generic;

namespace TalentBusinessLogic.DataTransferObjects.Hospitality
{
    public class AvailabilityDetails
    {
        public int Sold { get; set; }
        public int Reserved { get; set; }
        public int Cancelled { get; set; }
        public string SoldOutComponents { get; set; }
        public long AvailabilityComponentID { get; set; }        
        public int AvailableUnits { get; set; }
        public int OriginalUnits { get; set; }
        public int PercentageRemaining { get; set; }
        public List<ComponentDetails> ComponentListDetails { get; set; }
        public string Availability { get; set; }
        public string AvailabilityCSS { get; set; }
        public string AvailabilityColour { get; set; }
        public string AvailabilityText { get; set; }               
        public bool HaveSoldOutComponents { get; set; }
    }
}
