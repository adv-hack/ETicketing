using Talent.Common;
using TalentBusinessLogic.BusinessObjects.Environment;

namespace TalentBusinessLogic.BusinessObjects
{
    public class Fees : BusinessObjects
    {
        /// <summary>
        /// Checks if basket item a Fee TODO: Needs to be moved to a Fees class
        /// </summary>
        /// <param name="module1"></param>
        /// <param name="product"></param>
        /// <param name="feeCategory"></param>
        /// <returns></returns>
        public bool IsTicketingFee(string module1, string product, string feeCategory)
        {
            bool success = false;
            if (module1.ToUpper() == GlobalConstants.BASKETMODULETICKETING.ToString().ToUpper())
            {
               
                if (feeCategory != null && feeCategory.Trim().Length > 0)
                {
                    success = true;
                }
                else if (product.ToUpper() == Environment.Settings.DefaultValues.CashBackFeeCode.ToUpper())
                {
                    success = true;
                }
            }
            if (module1.ToUpper() == "TICKETING" && feeCategory != null && feeCategory.Trim().Length > 0)
            {
                success = true;
            }
            return success;
        }
    }
}
