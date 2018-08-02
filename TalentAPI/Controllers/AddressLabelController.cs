using System.Web.Http;
using TalentBusinessLogic.ModelBuilders;
using TalentBusinessLogic.Models;

namespace TalentAPI.Controllers
{
	public class AddressLabelController : TalentAPIBaseController
	{
		[HttpPost]
		public AddressLabelViewModel Post([FromBody] AddressLabelInputModel input)
		{
			AddressLabelPrintBuilder builder = new AddressLabelPrintBuilder();
			AddressLabelViewModel view = builder.PrintAddressLabel(input);
			return view;
		}
	}
}
