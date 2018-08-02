using System.Web.Http;
using TalentBusinessLogic.ModelBuilders;
using TalentBusinessLogic.Models;

namespace TalentAPI.Controllers
{

	public class SmartCardController : ApiController
	{
		[HttpPost]
		public SmartCardAPIViewModel Post([FromBody]SmartCardAPIInputModel input)
		{
			SmartCardBuilder service = new SmartCardBuilder();
			SmartCardAPIViewModel view = service.PopulateTeamCardAPIViewModelObject(input);
			return view;
		}

		[HttpGet]
		public SmartCardAPIViewModel Get([FromUri] SmartCardAPIInputModel input)
		{
			SmartCardBuilder service = new SmartCardBuilder();
			SmartCardAPIViewModel view = service.PopulateTeamCardAPIViewModelObject(input);
			return view;
		}
	}
}
