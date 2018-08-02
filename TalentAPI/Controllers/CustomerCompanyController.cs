using System.Web.Http;
using TalentBusinessLogic.ModelBuilders.CRM;
using TalentBusinessLogic.Models;

namespace TalentAPI.Controllers
{
	public class CustomerCompanyController : TalentAPIBaseController
	{
		[HttpDelete]
		public ErrorModel DeleteContact([FromUri] CustomerCompanyInputModel input)
		{
			return new CompanyModelBuilders().RemoveCustomerCompanyAssociation(input);
		}

		[HttpPost]
		public ErrorModel AddContact([FromBody] CustomerCompanyInputModel input)
		{
			return new CompanyModelBuilders().AddCustomerCompanyAssociation(input);
		}


	}
}
