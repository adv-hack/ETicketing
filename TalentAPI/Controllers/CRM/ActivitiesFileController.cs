using System;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.ModelBuilders.CRM;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace TalentAPI.Controllers.CRM
{
    public class ActivitiesFileController : TalentAPIBaseController
    {
        [HttpPost]
        public ActivitiesFileViewModel UploadFileAttachment()
        {
            //inputModel needs creating here rather than passed in as a paramter to prevent HTTP 415 errors caused by multipart/form-data requests
            ActivitiesFileInputModel inputModel = new ActivitiesFileInputModel();
            ActivitiesFileViewModel viewModel = new ActivitiesFileViewModel();
            var file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
            if (file != null && file.ContentLength > 0)
            {
                inputModel.Source = "W";
                inputModel.qqfile = HttpContext.Current.Request["qqfile"];
                inputModel.qqfilename = HttpContext.Current.Request["qqfilename"];
                inputModel.qqtotalfilesize = HttpContext.Current.Request["qqtotalfilesize"];
                inputModel.qquuid = HttpContext.Current.Request["qquuid"];

                inputModel.ActivityFileName = file.FileName;
                inputModel.SessionID = HttpContext.Current.Request["SessionID"];
                inputModel.TemplateID = Convert.ToInt16(HttpContext.Current.Request["TemplateID"]);
                inputModel.CustomerActivitiesHeaderID = HttpContext.Current.Request["CustomerActivitiesHeaderID"];
                inputModel.ActivityFileDescription = HttpContext.Current.Request["ActivityFileDescription"];
                inputModel.ActivityFileItemIndex = HttpContext.Current.Request["ActivityFileItemIndex"];
                viewModel = new ActivitiesModelBuilder().CreateActivityFileAttachment(inputModel, file);
                if (!viewModel.Error.HasError)
                {
                    viewModel.HTMLContent = ViewRenderer.RenderView("~/Views/PartialViews/_ActivitiesFileAttachment.cshtml", viewModel, null);
                }
            }
            else
            {
                viewModel.success = false;
            }
            return viewModel;
        }

        [HttpDelete]
        public ErrorModel DeleteFileAttachment([FromUri] ActivitiesFileInputModel inputModel)
        {
            inputModel.Source = "W";
            return new ActivitiesModelBuilder().DeleteActivityFileAttachment(inputModel);
        }
    }
}
