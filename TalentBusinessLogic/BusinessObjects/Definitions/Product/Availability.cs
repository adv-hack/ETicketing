using System;
using System.Data;


namespace TalentBusinessLogic.BusinessObjects.Definitions
{
    public class Availability : BusinessObjects
    {

        public void GetAvailabilityProperties(string businessUnit, ref string availabilityText, ref string availabilityCSS, ref string availabilityColor, int percent, string stadiumCode)
        {
            DataTable dtStadiumAvailability = TDataObjects.StadiumSettings.TblStadiumAreaColours.GetStadiumAvailabilityColoursAndText(businessUnit, stadiumCode);
            if (dtStadiumAvailability.Rows.Count > 0)
            {
                foreach (DataRow dr in dtStadiumAvailability.Rows)
                {
                    if (percent >= Convert.ToInt32(dr["MIN"]) & percent <= Convert.ToInt32(dr["MAX"]))
                    {
                        availabilityText = dr["TEXT"].ToString().Trim();
                        availabilityCSS = dr["CSS_CLASS"].ToString().Trim();
                        availabilityColor = dr["COLOUR"].ToString().Trim();
                        break;
                    }
                }
            }
        }

    }

}
