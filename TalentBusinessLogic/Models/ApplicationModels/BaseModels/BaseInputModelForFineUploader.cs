using System.Web;
namespace TalentBusinessLogic.Models
{
    public class BaseInputModelForFineUploader : BaseInputModel
    {
        public string qquuid  { get; set; }
        public string qqfilename  { get; set; }
        public string qqtotalfilesize  { get; set; }
        public string qqfile { get; set; }
    }
}