
namespace TalentBusinessLogic.Models
{
    public class HospitalityPackageListInputModel : BaseInputModel
    {
        /// <summary>
        /// Product code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Package id
        /// </summary>
        public long PackageId { get; set; }

        /// <summary>
        /// Product group code
        /// </summary>
        public string ProductGroupCode { get; set; }
    }
}
