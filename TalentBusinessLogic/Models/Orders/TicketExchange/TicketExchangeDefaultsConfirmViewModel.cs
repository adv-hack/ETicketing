using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
using TalentBusinessLogic.Models;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects;

namespace TalentBusinessLogic.Models
{
    public class TicketExchangeConfirmViewModel : BaseViewModel
    {
             
        public TicketExchangeConfirmViewModel (bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public TicketExchangeConfirmViewModel() { }
       
    }
  
}
