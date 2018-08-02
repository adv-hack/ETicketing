using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.BusinessObjects.Definitions;
using TalentBusinessLogic.DataTransferObjects.JQuery.DataTables.List;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.ModelBuilders.CRM
{
    public class ActivitiesModelBuilder : BaseModelBuilder
    {
        /// <summary>
        /// Return the activities list view model based on the input model. 
        /// Construct and call the Talent.Common layer.
        /// </summary>
        /// <param name="inputModel">Activities List input model</param>
        /// <returns>The activities list view model formatted correctly</returns>
        public ActivitiesListViewModel GetActivitiesListSearchResults(ActivitiesListInputModel inputModel)
        {
            ActivitiesListViewModel viewModel = new ActivitiesListViewModel();
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesListInputModel, DEActivities>();
            ErrorObj err = new ErrorObj();
            List<ActivitiesList> results = new List<ActivitiesList>();
            Agent agentDefinition = new Agent();

            viewModel.Draw = inputModel.Draw;
            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.De.ActivityOrderDirection = inputModel.Order[0].Dir.ToString();
            talActivites.Settings = Environment.Settings.DESettings;
            err = talActivites.CustomerActivitiesSearch();
            
            viewModel.Error = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            if (!viewModel.Error.HasError)
            {
                foreach (DataRow row in talActivites.ResultDataSet.Tables["CustomerActivitiesHeader"].Rows)
                {
                    row["DescriptiveUserName"] = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(row["ActivityUserName"].ToString());
                }
                results = Data.PopulateObjectListFromTable<ActivitiesList>(talActivites.ResultDataSet.Tables["CustomerActivitiesHeader"]);
                viewModel.DataTableList = results;
                viewModel.RecordsFiltered = (int)talActivites.ResultDataSet.Tables["StatusResults"].Rows[0]["RecordsReturned"];
            }
            return viewModel;
        }

        /// <summary>
        /// Perform an activity delete and return the error model based on the input model. 
        /// Construct and call the Talent.Common layer.
        /// </summary>
        /// <param name="inputModel">Activities List input model</param>
        /// <returns>The generic error model</returns>
        public ErrorModel DeleteActivityByID(ActivitiesListInputModel inputModel)
        {
            ErrorModel errModel = new ErrorModel();
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesListInputModel, DEActivities>();
            ErrorObj err = new ErrorObj();

            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            err = talActivites.DeleteCustomerActivity();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            return errModel;
        }

        /// <summary>
        /// Set the activities list view model for display purposes based on the selected input on the input model.
        /// </summary>
        /// <param name="inputModel">The activities list input model</param>
        /// <returns>The activities list view model formatted correctly</returns>
        public ActivitiesListViewModel ActivitiesList(ActivitiesListInputModel inputModel)
        {
            ActivitiesListViewModel viewModel = new ActivitiesListViewModel(true);
            viewModel.TemplateID = inputModel.TemplateID;
            viewModel.ActivityDate = inputModel.ActivityDate;
            viewModel.ActivitySubject = inputModel.ActivitySubject;
            viewModel.ActivityUserName = inputModel.ActivityUserName;
            viewModel.ActivityStatus = inputModel.ActivityStatus;
            viewModel.AgentList = new Agent().retrieveAgents();
            viewModel.TemplatesList = new ActivitiesDefinition().RetrieveActivityTemplates();
            viewModel.StatusList = new ActivitiesDefinition().RetrieveActivityStatus();
            return viewModel;
        }

        /// <summary>
        /// Create a new activity comment for the existing customer activity record
        /// </summary>
        /// <param name="inputModel">The input model for editing an activity</param>
        /// <returns>View model of created comment data</returns>
        public ActivitiesEditViewModel CreateActivityComment(ActivitiesEditInputModel inputModel)
        {
            ErrorModel errModel = new ErrorModel();
            ActivitiesEditViewModel viewModel = new ActivitiesEditViewModel(true, "EditProfileActivity.ascx");
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesEditInputModel, DEActivities>();
            DataTable dtActivityComments = new DataTable();
            ErrorObj err = new ErrorObj();
            
            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            talActivites.Settings.AgentEntity.AgentUsername = Environment.Agent.GetAgentUserNameBySessionId(inputModel.SessionID);
            err = talActivites.CreateActivityComment();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            viewModel.Error = errModel;
            if (!errModel.HasError)
            {
                dtActivityComments = talActivites.ResultDataSet.Tables["CustomerActivitiesComments"];
                foreach (DataRow row in dtActivityComments.Rows)
                {
                    if (row["CommentID"].ToString() == talActivites.De.ActivityCommentID.ToString())
                    {
                        Agent agentDefinition = new Agent();
                        DateTime dateUpdated = new DateTime();
                        TimeSpan timeUpdated = new TimeSpan();
                        string unformattedTimeUpdated = string.Empty;
                        CultureInfo culture = new CultureInfo(Environment.Settings.DefaultValues.Culture);

                        dateUpdated = Utilities.ISeries8CharacterDate(row["UpdatedDate"].ToString());
                        unformattedTimeUpdated = row["UpdatedTime"].ToString().PadLeft(6, '0');
                        timeUpdated = TimeSpan.ParseExact(unformattedTimeUpdated, "hhmmss", culture);
                        viewModel.ActivityCommentUpdatedDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                        viewModel.ActivityCommentUpdatedTime = timeUpdated.ToString();
                        viewModel.ActivityDescriptiveUserName = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(row["AgentName"].ToString());
                        viewModel.ActivityCommentItemIndex = inputModel.ActivityCommentItemIndex;
                        viewModel.ActivityCommentID = talActivites.De.ActivityCommentID.ToString();
                        viewModel.CustomerActivitiesHeaderID = inputModel.CustomerActivitiesHeaderID;
                        viewModel.ActivityCommentText = inputModel.ActivityCommentText;
                        viewModel.ActivityCommentBlurb = viewModel.GetControlText("CommentAddedText");
                        break;
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Update an existing activity comment for an existing activity record
        /// </summary>
        /// <param name="inputModel">The input model for editing an activity</param>
        /// <returns>view model of updated comment data</returns>
        public ActivitiesEditViewModel UpdateActivityComment(ActivitiesEditInputModel inputModel)
        {
            ErrorModel errModel = new ErrorModel();
            ActivitiesEditViewModel viewModel = new ActivitiesEditViewModel(true, "EditProfileActivity.ascx");
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesEditInputModel, DEActivities>();
            DataTable dtActivityComments = new DataTable();
            ErrorObj err = new ErrorObj();

            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            talActivites.Settings.AgentEntity.AgentUsername = Environment.Agent.GetAgentUserNameBySessionId(inputModel.SessionID);
            err = talActivites.UpdateActivityComment();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            if (!errModel.HasError)
            {
                dtActivityComments = talActivites.ResultDataSet.Tables["CustomerActivitiesComments"];
                foreach (DataRow row in dtActivityComments.Rows)
                {
                    if (row["CommentID"].ToString() == inputModel.ActivityCommentID)
                    {
                        Agent agentDefinition = new Agent();
                        DateTime dateUpdated = new DateTime();
                        TimeSpan timeUpdated = new TimeSpan();
                        string unformattedTimeUpdated = string.Empty;
                        CultureInfo culture = new CultureInfo(Environment.Settings.DefaultValues.Culture);

                        dateUpdated = Utilities.ISeries8CharacterDate(row["UpdatedDate"].ToString());
                        unformattedTimeUpdated = row["UpdatedTime"].ToString().PadLeft(6, '0');
                        timeUpdated = TimeSpan.ParseExact(unformattedTimeUpdated, "hhmmss", culture);
                        viewModel.ActivityCommentUpdatedDate = dateUpdated.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                        viewModel.ActivityCommentUpdatedTime = timeUpdated.ToString();
                        viewModel.ActivityDescriptiveUserName = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(row["AgentName"].ToString());
                        viewModel.ActivityCommentBlurb = viewModel.GetControlText("CommentEditedText");
                        break;
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Delete an existing activity comment for an existing activity record
        /// </summary>
        /// <param name="inputModel">The input model for editing an activity</param>
        /// <returns>Error model of results</returns>
        public ErrorModel DeleteActivityComment(ActivitiesEditInputModel inputModel)
        {
            ErrorModel errModel = new ErrorModel();
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesEditInputModel, DEActivities>();
            ErrorObj err = new ErrorObj();

            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            talActivites.Settings.AgentEntity.AgentUsername = Environment.Agent.GetAgentUserNameBySessionId(inputModel.SessionID);
            err = talActivites.DeleteActivityComment();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            return errModel;
        }

        /// <summary>
        /// Create the file attachment record and save the file locally. Handle any errors and set the error on the fineuploader error JSON response
        /// </summary>
        /// <param name="inputModel">The fine uploader based input model</param>
        /// <param name="postedFile">The file being posted</param>
        /// <returns>The view model with error and fine uploader JSON response</returns>
        public ActivitiesFileViewModel CreateActivityFileAttachment(ActivitiesFileInputModel inputModel, HttpPostedFile postedFile)
        {
            ActivitiesFileViewModel viewModel = new ActivitiesFileViewModel(true, "EditProfileActivity.ascx");
            ErrorModel errModel = new ErrorModel();
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesFileInputModel, DEActivities>();
            ErrorObj err = new ErrorObj();
            DataTable dtActivityTemplate = new DataTable();
            DataTable dtActivityFileAttachments = new DataTable();
            string fileName = string.Empty;
            string fileSavePath = string.Empty;
            string remotePath = string.Empty;

            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            talActivites.Settings.AgentEntity.AgentUsername = Environment.Agent.GetAgentUserNameBySessionId(inputModel.SessionID);
            err = talActivites.CreateActivityFileAttachment();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);

            if (!errModel.HasError)
            {
                dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByBUTemplateID(Environment.Settings.BusinessUnit, inputModel.TemplateID);
                if (dtActivityTemplate.Rows.Count > 0)
                {
                    fileSavePath = dtActivityTemplate.Rows[0]["LOCAL_ROOT_DIRECTORY"].ToString();
                    remotePath = dtActivityTemplate.Rows[0]["REMOTE_ROOT_DIRECTORY"].ToString();
                    /** Activities is a BUI function and needs the BU on the output URL*/
                    remotePath = @"/" + Environment.Settings.BusinessUnit.Trim() + remotePath;
                    if (fileSavePath.Length > 0 && remotePath.Length > 0)
                    {
                        if (!fileSavePath.EndsWith(@"\")) fileSavePath = fileSavePath + @"\";
                        if (!remotePath.EndsWith(@"\")) remotePath = remotePath + @"/";
                        fileName = talActivites.De.ActivityFileAttachmentID.ToString() + "_" + inputModel.ActivityFileName;
                        if (!File.Exists(fileSavePath + fileName))
                        {
                            try
                            {
                                postedFile.SaveAs(fileSavePath + fileName);
                            }
                            catch (Exception ex)
                            {
                                errModel.HasError = true;
                                errModel.ReturnCode = ex.Source;
                                errModel.ErrorMessage = viewModel.GetControlText("ProblemUploadingErrorText");
                            }
                        }
                        else
                        {
                            errModel.HasError = true;
                            viewModel.GetControlText("FileAlreadyExistsErrorText");
                        }
                    }
                    else
                    {
                        errModel.HasError = true;
                        viewModel.GetControlText("ProblemWithSetupErrorText");
                    }
                }
                else
                {
                    errModel.HasError = true;
                    viewModel.GetControlText("ProblemWithSetupErrorText");
                }
            }

            viewModel.Error = errModel;
            if (errModel.HasError)
            {
                viewModel.error = errModel.ErrorMessage;
                viewModel.success = false;
            }
            else
            {
                dtActivityFileAttachments = talActivites.ResultDataSet.Tables["CustomerActivitiesFileAttachments"];
                foreach (DataRow row in dtActivityFileAttachments.Rows)
                {
                    if (row["AttachmentID"].ToString() == talActivites.De.ActivityFileAttachmentID.ToString())
                    {
                        Agent agentDefinition = new Agent();
                        DateTime dateAdded = new DateTime();
                        TimeSpan timeAdded = new TimeSpan();
                        string unformattedTimeAdded = string.Empty;
                        CultureInfo culture = new CultureInfo(Environment.Settings.DefaultValues.Culture);

                        dateAdded = Utilities.ISeries8CharacterDate(row["DateAdded"].ToString());
                        unformattedTimeAdded = row["TimeAdded"].ToString().PadLeft(6, '0');
                        timeAdded = TimeSpan.ParseExact(unformattedTimeAdded, "hhmmss", culture);
                        viewModel.ActivityFileDateAdded = dateAdded.ToString(Environment.Settings.DefaultValues.GlobalDateFormat);
                        viewModel.ActivityFileTimeAdded = timeAdded.ToString();
                        viewModel.ActivityDescriptiveUserName = agentDefinition.GetAgentDescriptiveNameByAgentUserCode(row["AgentName"].ToString());
                        viewModel.ActivityFileItemIndex = inputModel.ActivityFileItemIndex;
                        viewModel.ActivityFileAttachmentID = talActivites.De.ActivityFileAttachmentID.ToString();
                        viewModel.ActivityFileDescription = inputModel.ActivityFileDescription;
                        viewModel.ActivityFileHasDescription = (inputModel.ActivityFileDescription.Trim().Length > 0);
                        viewModel.ActivityFileName = inputModel.ActivityFileName;
                        viewModel.ActivityFileLink = remotePath + fileName;
                        viewModel.TemplateID = inputModel.TemplateID.ToString();
                        viewModel.ActivityFileAttachmentBlurb = viewModel.GetControlText("FileAttachmentBlurbText");
                        viewModel.success = true;
                        break;
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Delete an existing activity file for an existing activity record, if that succeeds then delete the local file
        /// </summary>
        /// <param name="inputModel">The input model for editing an activity</param>
        /// <returns>Error model of results</returns>
        public ErrorModel DeleteActivityFileAttachment(ActivitiesFileInputModel inputModel)
        {
            ErrorModel errModel = new ErrorModel();
            TalentActivities talActivites = new TalentActivities();
            Mapper.CreateMap<ActivitiesFileInputModel, DEActivities>();
            ErrorObj err = new ErrorObj();
            DataTable dtActivityTemplate = new DataTable();
            string fileSavePath = string.Empty;

            talActivites.De = Mapper.Map<DEActivities>(inputModel);
            talActivites.Settings = Environment.Settings.DESettings;
            talActivites.Settings.AgentEntity.AgentUsername = Environment.Agent.GetAgentUserNameBySessionId(inputModel.SessionID);
            err = talActivites.DeleteActivityFileAttachment();
            errModel = Data.PopulateErrorObject(err, talActivites.ResultDataSet, talActivites.Settings, 2);
            if (!errModel.HasError)
            {
                dtActivityTemplate = TDataObjects.ActivitiesSettings.GetActivityTemplatesTypeByBUTemplateID(Environment.Settings.BusinessUnit, inputModel.TemplateID);
                if (dtActivityTemplate.Rows.Count > 0)
                {
                    fileSavePath = dtActivityTemplate.Rows[0]["LOCAL_ROOT_DIRECTORY"].ToString();
                    if (fileSavePath.Length > 0){
                        if (!fileSavePath.EndsWith(@"\"))
                        {
                            fileSavePath = fileSavePath + @"\";
                        }
                        if (File.Exists(fileSavePath + inputModel.ActivityFileName))
                        {
                            try
                            {
                                File.Delete(fileSavePath + inputModel.ActivityFileAttachmentID + '_' + inputModel.ActivityFileName);
                            }
                            catch(Exception ex)
                            {
                                errModel.HasError = true;
                                errModel.ReturnCode = ex.Source;
                                errModel.ErrorMessage = ex.Message;
                            }
                        }
                    }
                }
            }
            return errModel;
        }

        /// <summary>
        /// Sets activities session TemplateID on TalentBase01
        /// </summary>
        /// <param name="inputModel"></param>
        public void SetActivitiesSession(ActivitiesTemplateInputModel inputModel) 
        {
            Data.Session.Remove("TemplateIDs");
            List<DataTransferObjects.ActivityTemplateQA> templatesList =  Activities.SetActivitiesTemplates(inputModel.basket, inputModel.BasketHeaderID, inputModel.CacheDependencyPath, inputModel.Username, inputModel.Fullname);
            Data.Session.Add("TemplateIDs", templatesList);
        }

    }
}