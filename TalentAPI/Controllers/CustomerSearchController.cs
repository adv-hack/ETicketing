using System.Web.Http;
using TalentAPI.Filters;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using TalentBusinessLogic.ModelBuilders;
using TalentBusinessLogic.Models;

namespace TalentAPI.Controllers
{
	public class CustomerSearchController : ApiController
	{

		[HttpPost]
		[AgentAuthorization]
		public JQueryDataTable<CustomerSearchLists> Post(CustomerSearchInputModel input)
		{
			CustomerSearchViewModel view = new CustomerSearchBuilder().CustomerSearch(input);
			return view.GetJQueryDataTable();
		}
	}
}
