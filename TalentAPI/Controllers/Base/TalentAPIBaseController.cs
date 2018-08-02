using System.Web.Http;
using TalentAPI.Filters;

namespace TalentAPI.Controllers
{
	[AgentAuthorization]
	public class TalentAPIBaseController : ApiController
	{

	}
}
