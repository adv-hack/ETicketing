using System.Web.Http;
using TalentAPI.Filters;
using TalentBusinessLogic.ModelBuilders;
using TalentBusinessLogic.Models;
namespace TalentAPI.Controllers
{
	public class VerifyRetrieveCustomerController : ApiController
	{
		[HttpPost]
		[AgentAuthorization]
		public VerifyAndRetrieveCustomerViewModel Post([FromBody] VerifyAndRetrieveCustomerInputModel input)
		{
			VerifyAndRetrieveCustomerBuilder service = new VerifyAndRetrieveCustomerBuilder();
			VerifyAndRetrieveCustomerViewModel view = service.VerifyAndRetrieveCustomer(input);
			return view;
		}
	}
}
