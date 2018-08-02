
using System.Collections.Generic;
namespace TalentBusinessLogic.Models
{
    public class ErrorModel : List<string>
    {
        public string ReturnCode { get; set; }
        public string ReturnReference { get; set; }
        public string ErrorMessage { get; set; }
        public string TechnicalErrorMessage { get; set; }
        public string ErrorOccurred { get; set; }
        public bool HasError { get; set; }
    }
}
