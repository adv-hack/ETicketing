using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalentBusinessLogic.DataAnnotation;

namespace TalentBusinessLogic.Models
{
    /// <summary>
    /// Sample input model to test data annotation
    /// </summary>
    public class DataAnnotationInputModel : BaseInputModel
    {
        [TalentDisplay(Name: "First Name", Model: typeof(DataAnnotationInputModel))]
        [TalentRequired]
        [TalentRegularExpression(Pattern: @"^[A-Z\s]*$")]
        public String FirstName { get; set; }

        [TalentDisplay(Name: "Last Name", Model: typeof(DataAnnotationInputModel))]
        [TalentRequired]
        [TalentLengthAttribute(MaxLength: 10)]
        public String LastName { get; set; }

        [TalentDisplay(Name: "Ticket Quantity", Model: typeof(DataAnnotationInputModel))]
        [TalentRange(Minimum: 2, Maximum: 10)]
        public String TicketQuantity { get; set; }
    }
}
