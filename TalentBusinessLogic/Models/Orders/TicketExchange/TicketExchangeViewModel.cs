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
    public class TicketExchangeViewModel : BaseViewModel
    {

        public string TicketExchangeReference { get; set; }
  
        public TicketExchangeViewModel(bool getContentAndAttributes) : base(new WebFormResource(), getContentAndAttributes) { }
        public TicketExchangeViewModel() { }
       
    }
  
}
