using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentBusinessLogic.DataTransferObjects.Setup.Template
{
    public class EmailConfirmationItem
    {
        /// <summary>
        ///Unique ticketing confirmation email template id 
        ///</summary>
        public decimal SaleConfirmationEmailId { get; set; }

        /// <summary>
        ///Ticketing confirmation email template description
        ///</summary>
        public string SaleConfirmationEmailDescription { get; set; }
    }
}
