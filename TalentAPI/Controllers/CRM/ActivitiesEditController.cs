using System;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.ModelBuilders.CRM;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;

namespace TalentAPI.Controllers.CRM
{
    public class ActivitiesEditController : TalentAPIBaseController
    {
        [HttpPost]
        public HttpResponseMessage CreateComment([FromBody] ActivitiesEditInputModel inputModel)
        {
            inputModel.Source = "W";
            HttpResponseMessage response = null;
            ActivitiesEditViewModel viewModel = new ActivitiesModelBuilder().CreateActivityComment(inputModel);
            if (viewModel.Error.HasError)
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.Content = new StringContent(viewModel.Error.ErrorMessage);
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.OK);
                string result = ViewRenderer.RenderView("~/Views/PartialViews/_ActivitiesComment.cshtml", viewModel, null);
                response.Content = new StringContent(result);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            }
			return response;
        }

        [HttpPost]
        public ActivitiesEditViewModel UpdateComment([FromBody] ActivitiesEditInputModel inputModel)
        {
            inputModel.Source = "W";
            return new ActivitiesModelBuilder().UpdateActivityComment(inputModel);
        }

        [HttpDelete]
        public ErrorModel DeleteComment([FromUri] ActivitiesEditInputModel inputModel)
        {
            inputModel.Source = "W";
            return new ActivitiesModelBuilder().DeleteActivityComment(inputModel);
        }
    }
}
