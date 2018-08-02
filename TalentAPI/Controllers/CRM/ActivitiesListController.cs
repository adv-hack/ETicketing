using System.Web.Http;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using TalentBusinessLogic.ModelBuilders.CRM;

namespace TalentAPI.Controllers
{
	public class ActivitiesListController : TalentAPIBaseController
	{
		[HttpPost]
		public JQueryDataTable<ActivitiesList> Post(ActivitiesListInputModel input)
		{
			ActivitiesListViewModel view = new ActivitiesListViewModel();
			input.Source = "W";
			ActivitiesModelBuilder builder = new ActivitiesModelBuilder();
			view = builder.GetActivitiesListSearchResults(input);
			return view.GetJQueryDataTable();
		}

		[HttpDelete]
		public ErrorModel Delete([FromUri] ActivitiesListInputModel input)
		{
			input.Source = "W";
			return new ActivitiesModelBuilder().DeleteActivityByID(input);
		}
	}
}