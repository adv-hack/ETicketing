using System.Web.Http;
using TalentAPI.Filters;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using TalentBusinessLogic.ModelBuilders;
using TalentBusinessLogic.Models;

namespace TalentAPI.Controllers
{
	public class CompanySearchController : ApiController
	{
		[HttpPost]
		[AgentAuthorization]
		public JQueryDataTable<CompanySearchLists> Post(CompanySearchInputModel input)
		{
			CompanySearchViewModel view = new CompanySearchBuilder().CompanySearch(input);
			return view.GetJQueryDataTable();
		}
	}
}
