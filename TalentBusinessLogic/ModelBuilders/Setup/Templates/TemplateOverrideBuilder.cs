using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Talent.Common;
using TalentBusinessLogic.DataTransferObjects.Setup.Template;
using TalentBusinessLogic.Models.Setup.Template;
using Talent.Common.DataObjects.TableObjects;

namespace TalentBusinessLogic.ModelBuilders.Setup.Templates
{
    public class TemplateOverrideBuilder : BaseModelBuilder
    {
        #region Public Functions
        /// <summary>
        /// This method retrieves template override list
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>The formed template override view model</returns>
        public TemplateOverrideViewModel RetrieveTemplateOverrideList(TemplateOverrideInputModel inputModel)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DataSet dsTemplateOverrideList = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<TemplateOverrideHeader> templateOverrideList = new List<TemplateOverrideHeader>();
            List<TemplateOverrideCriteria> templateOverrideCriterias = new List<TemplateOverrideCriteria>();

            inputModel.Mode = GlobalConstants.TEMPLATE_OVERRIDE_READ_MODE;
            dsTemplateOverrideList = retrieveTemplateOverrides(inputModel);
            viewModel.Error = Data.PopulateErrorObject(err, dsTemplateOverrideList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);

            if (!viewModel.Error.HasError)
            {
                templateOverrideList = Data.PopulateObjectListFromTable<TemplateOverrideHeader>(dsTemplateOverrideList.Tables["TemplateOverrideHeader"]);
                templateOverrideCriterias = Data.PopulateObjectListFromTable<TemplateOverrideCriteria>(dsTemplateOverrideList.Tables["TemplateOverrideCriterias"]);
                
                //Set view model
                viewModel.TemplateOverrideList = templateOverrideList.OrderByDescending(s => s.TemplateOverrideId).ToList();

                foreach (TemplateOverrideCriteria criteria in templateOverrideCriterias)
                {
                    criteria.CriteriaTypeDescription = viewModel.GetPageText("TemplateCriteria-" + criteria.CriteriaType);
                }

                foreach (TemplateOverrideHeader template in viewModel.TemplateOverrideList)
                {
                    template.TemplateOverrideCriterias = templateOverrideCriterias.Where(s => s.TemplateUseId == template.TemplateOverrideId).ToList();
                    template.TemplateOverrideCriteriasFormatted = template.TemplateOverrideCriterias.GroupBy(g => g.CriteriaTypeDescription).Select(s => new TemplateOverrideCriteriaFormatted { CriteriaTypeDescription = s.Key, CriteriaValues = string.Join(",", s.Select(e => e.CriteriaValue).ToList()) }).ToList();
                    if (template.TemplateOverrideCriterias[0].CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTCODE || template.TemplateOverrideCriterias[0].CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PACKAGE)
                    {
                        template.ProductPacakgeSpecific = true;
                    }
                    else
                    {
                        template.ProductPacakgeSpecific = false;
                    }

                    foreach (TemplateOverrideCriteria templateOverrideCriteria in template.TemplateOverrideCriterias)
                    {
                        if(templateOverrideCriteria.CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTCODE)
                        {
                           if (template.ProductCriterias == null)
                              {
                                 template.ProductCriterias = template.ProductCriterias + templateOverrideCriteria.CriteriaValue;
                              }
                           else
                              {
                                 template.ProductCriterias = template.ProductCriterias + "," + templateOverrideCriteria.CriteriaValue;
                              }
                        }
                        if(templateOverrideCriteria.CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PACKAGE)
                        {
                            if (template.PackageCriterias == null)
                            {
                                template.PackageCriterias = template.PackageCriterias + templateOverrideCriteria.CriteriaValue;
                            }
                            else
                            {
                                template.PackageCriterias = template.PackageCriterias + "," + templateOverrideCriteria.CriteriaValue;
                            }
                        }
                        if (templateOverrideCriteria.CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_SUBTYPE)
                        {
                            if (template.ProductSubTypeCriterias == null)
                            {
                                template.ProductSubTypeCriterias = template.ProductSubTypeCriterias + templateOverrideCriteria.CriteriaValue;
                            }
                            else
                            {
                                template.ProductSubTypeCriterias = template.ProductSubTypeCriterias + "," + templateOverrideCriteria.CriteriaValue;
                            }
                        }
                        if (templateOverrideCriteria.CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_PRODUCTTYPE)
                        {
                            if (template.ProductTypeCriterias == null)
                            {
                                template.ProductTypeCriterias = template.ProductTypeCriterias + templateOverrideCriteria.CriteriaValue;
                            }
                            else
                            {
                                template.ProductTypeCriterias = template.ProductTypeCriterias + "," + templateOverrideCriteria.CriteriaValue;
                            }
                        }
                        if (templateOverrideCriteria.CriteriaType == GlobalConstants.TEMPLATE_OVERRIDE_CRITERIA_STADIUM)
                        {
                            if (template.StadiumCriterias == null)
                            {
                                template.StadiumCriterias = template.StadiumCriterias + templateOverrideCriteria.CriteriaValue;
                            }
                            else
                            {
                                template.StadiumCriterias = template.StadiumCriterias + "," + templateOverrideCriteria.CriteriaValue;
                            }
                        }

                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// This method creates the template override
        /// </summary>
        /// <returns>The formed template override view model</returns>
        public TemplateOverrideViewModel CreateTemplateOverride(TemplateOverrideInputModel inputModel)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DataSet dsTemplateOverrideList = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<TemplateOverrideHeader> templateOverrideList = new List<TemplateOverrideHeader>();
            List<TemplateOverrideCriteria> templateOverrideCriterias = new List<TemplateOverrideCriteria>();

            inputModel.Mode = GlobalConstants.TEMPLATE_OVERRIDE_CREATE_MODE;
            dsTemplateOverrideList = createAndRetrieveTemplateOverrides(inputModel);
            viewModel.Error = Data.PopulateErrorObject(err, dsTemplateOverrideList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);

            if (!viewModel.Error.HasError)
            {
                templateOverrideList = Data.PopulateObjectListFromTable<TemplateOverrideHeader>(dsTemplateOverrideList.Tables["TemplateOverrideHeader"]);
                templateOverrideCriterias = Data.PopulateObjectListFromTable<TemplateOverrideCriteria>(dsTemplateOverrideList.Tables["TemplateOverrideCriterias"]);

                //Set view model
                viewModel.TemplateOverrideList = templateOverrideList.OrderByDescending(s => s.TemplateOverrideId).ToList();

                foreach (TemplateOverrideCriteria criteria in templateOverrideCriterias)
                {
                    criteria.CriteriaTypeDescription = viewModel.GetPageText("TemplateCriteria-" + criteria.CriteriaType);
                }

                foreach (TemplateOverrideHeader template in viewModel.TemplateOverrideList)
                {
                    template.TemplateOverrideCriterias = templateOverrideCriterias.Where(s => s.TemplateUseId == template.TemplateOverrideId).ToList();
                    template.TemplateOverrideCriteriasFormatted = template.TemplateOverrideCriterias.GroupBy(g => g.CriteriaTypeDescription).Select(s => new TemplateOverrideCriteriaFormatted { CriteriaTypeDescription = s.Key, CriteriaValues = string.Join(",", s.Select(e => e.CriteriaValue).ToList()) }).ToList();
                }
            }


            return viewModel;
        }

        /// <summary>
        /// This method deletes the template
        /// </summary>
        /// <returns>The formed template override view model</returns>
        public TemplateOverrideViewModel DeleteTemplateOverride(TemplateOverrideInputModel inputModel)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes:true);
            DataSet dsTemplateOverrideList = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<TemplateOverrideHeader> templateOverrideList = new List<TemplateOverrideHeader>();
            List<TemplateOverrideCriteria> templateOverrideCriterias = new List<TemplateOverrideCriteria>();

            inputModel.Mode = GlobalConstants.TEMPLATE_OVERRIDE_DELETE_MODE;
            dsTemplateOverrideList = deleteAndRetrieveTemplateOverrides(inputModel);
            viewModel.Error = Data.PopulateErrorObject(err, dsTemplateOverrideList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);

            if (!viewModel.Error.HasError)
            {
                templateOverrideList = Data.PopulateObjectListFromTable<TemplateOverrideHeader>(dsTemplateOverrideList.Tables["TemplateOverrideHeader"]);
                templateOverrideCriterias = Data.PopulateObjectListFromTable<TemplateOverrideCriteria>(dsTemplateOverrideList.Tables["TemplateOverrideCriterias"]);

                //Set view model
                viewModel.TemplateOverrideList = templateOverrideList.OrderByDescending(s => s.TemplateOverrideId).ToList();

                foreach (TemplateOverrideCriteria criteria in templateOverrideCriterias)
                {
                    criteria.CriteriaTypeDescription = viewModel.GetPageText("TemplateCriteria-" + criteria.CriteriaType);
                }

                foreach (TemplateOverrideHeader template in viewModel.TemplateOverrideList)
                {
                    template.TemplateOverrideCriterias = templateOverrideCriterias.Where(s => s.TemplateUseId == template.TemplateOverrideId).ToList();
                    template.TemplateOverrideCriteriasFormatted = template.TemplateOverrideCriterias.GroupBy(g => g.CriteriaTypeDescription).Select(s => new TemplateOverrideCriteriaFormatted { CriteriaTypeDescription = s.Key, CriteriaValues = string.Join(",", s.Select(e => e.CriteriaValue).ToList()) }).ToList();
                }
            }

            return viewModel;
        }

        /// <summary>
        /// This method retrieves template override criterias
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>The formed template override view model</returns>
        public TemplateOverrideViewModel RetriveOverrideCriterias()
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DataSet dsOverrideCriteria = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<TicketingOverrideCriteria> ticketingOverrideCriterias = new List<TicketingOverrideCriteria>();
            List<PackageOverrideCriteria> packageOverrideCriterias = new List<PackageOverrideCriteria>();

            dsOverrideCriteria = retriveOverrideCriterias();
            viewModel.Error = Data.PopulateErrorObject(err, dsOverrideCriteria, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);

            if (!viewModel.Error.HasError)
            {
                ticketingOverrideCriterias = Data.PopulateObjectListFromTable<TicketingOverrideCriteria>(dsOverrideCriteria.Tables["TicketingOverrideCriteria"]);
                packageOverrideCriterias = Data.PopulateObjectListFromTable<PackageOverrideCriteria>(dsOverrideCriteria.Tables["PackageOverrideCriteria"]);

                //Set view model
                viewModel.TicketingOverrideCriterias = ticketingOverrideCriterias.ToList();
                viewModel.PackageOverrideCriterias = packageOverrideCriterias.ToList();

                foreach (TicketingOverrideCriteria criteria in ticketingOverrideCriterias)
                {
                    criteria.ProductTypeDecsription = viewModel.GetPageText("ProductType-" + criteria.ProductType);
                }
            }

            return viewModel;
        }

        /// <summary>
        /// This methid retrieves Businessunits
        /// </summary>
        /// <returns>The formed template override view model</returns>
        public TemplateOverrideViewModel RetrieveBusinessUnitList()
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DESettings settings = Environment.Settings.DESettings;            

            tbl_url_bu tblUrlBu = new tbl_url_bu(settings);
            DataTable dtBUs = tblUrlBu.GetDistinctEBusinessBUs();
           
            List<string> businessUnits = dtBUs.AsEnumerable().Select(r => r.Field<string>("BUSINESS_UNIT")).ToList();
            Dictionary<string, string> businessUnitsHash = businessUnits.ToDictionary(businessUnit => businessUnit);

            viewModel.BusinessUnitList = businessUnitsHash;
            return viewModel;
        }

        /// <summary>
        /// Retrieve Email Confirmation Templates
        /// </summary>
        /// <returns>The email confirmation templates</returns>
        public TemplateOverrideViewModel RetrieveEmailConfirmationList(TemplateOverrideInputModel inputModelTemplateOverride)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DESettings settings = Environment.Settings.DESettings;            

            tbl_email_templates tblEmailTemplates = new tbl_email_templates(settings);
            DataTable dtEmailTemplates = tblEmailTemplates.GetAll();
            DataView dvEmailTemplates = new DataView(dtEmailTemplates);
            dvEmailTemplates.RowFilter = "BUSINESS_UNIT = '" + inputModelTemplateOverride.BusinessUnit + "' AND TEMPLATE_TYPE = '" + GlobalConstants.EMAIL_ORDER_CONFIRMATION + "' AND ACTIVE = 1";
            dtEmailTemplates = dvEmailTemplates.ToTable();
            viewModel.EmailConfirmationList = (from DataRow dr in dtEmailTemplates.Rows
                                               select new EmailConfirmationItem()
                                               {
                                                   SaleConfirmationEmailId = Utilities.ConvertStringToDecimal(dr["EMAILTEMPLATE_ID"].ToString()),
                                                   SaleConfirmationEmailDescription = dr["NAME"].ToString()
                                               }).OrderBy(s => s.SaleConfirmationEmailDescription).ThenBy(s => s.SaleConfirmationEmailId).ToList();
            return viewModel;
        }

        /// <summary>
        /// Retrieve Q and A Templates
        /// </summary>
        /// <returns>The Q and A templates</returns>
        public TemplateOverrideViewModel RetrieveQandATemplateList(TemplateOverrideInputModel inputModelTemplateOverride)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DESettings settings = Environment.Settings.DESettings;            

            tbl_activity_templates tblActivityTemplates = new tbl_activity_templates(settings);
            DataTable dtActivityTemplates = tblActivityTemplates.GetByBU(inputModelTemplateOverride.BusinessUnit, Utilities.CheckForDBNull_Int(GlobalConstants.ACTIVITY_TEMPLATE_TYPE_ID_HOSPITALITY));
            viewModel.QandAList = (from DataRow dr in dtActivityTemplates.Rows
                                   select new QandAItem()
                                   {
                                       QAndATemplateId = Utilities.ConvertStringToDecimal(dr["TEMPLATE_ID"].ToString()),
                                       QAndATemplateDescription = dr["NAME"].ToString()
                                   }).OrderBy(s => s.QAndATemplateDescription).ThenBy(s => s.QAndATemplateId).ToList();
            return viewModel;
        }

        /// <summary>
        /// Retrieve Data Capture Templates
        /// </summary>
        /// <returns>The Data Capture templates</returns>
        public TemplateOverrideViewModel RetrieveDataCaptureTemplateList(TemplateOverrideInputModel inputModelTemplateOverride)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DESettings settings = Environment.Settings.DESettings;            

            tbl_activity_templates tblActivityTemplates = new tbl_activity_templates(settings);
            DataTable dtActivityTemplates = tblActivityTemplates.GetByBU(inputModelTemplateOverride.BusinessUnit, Utilities.CheckForDBNull_Int(GlobalConstants.ACTIVITY_TEMPLATE_TYPE_HOSPITALITY_DATA_CAPTURE));

            viewModel.DataCaptureList = (from DataRow dr in dtActivityTemplates.Rows
                                         select new DataCaptureItem()
                                         {
                                             DataCaptureTemplateId = Utilities.ConvertStringToDecimal(dr["TEMPLATE_ID"].ToString()),
                                             DataCaptureTemplateDescription = dr["NAME"].ToString()
                                         }).OrderBy(s => s.DataCaptureTemplateDescription).ThenBy(s => s.DataCaptureTemplateId).ToList();
            return viewModel;
        }

        /// <summary>
        /// This method update the TemplateOverride criterias
        /// </summary>
        /// <param name = "inputModel" > The given TemplateOverride input model</param>
        /// <returns>The formed TemplateOverride view model</returns>
        public TemplateOverrideViewModel UpdateTemplateOverride(TemplateOverrideInputModel inputModel)
        {
            TemplateOverrideViewModel viewModel = new TemplateOverrideViewModel(getContentAndAttributes: true);
            DataSet dsTemplateOverrideList = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<TemplateOverrideHeader> templateOverrideList = new List<TemplateOverrideHeader>();
            List<TemplateOverrideCriteria> templateOverrideCriterias = new List<TemplateOverrideCriteria>();

            inputModel.Mode = GlobalConstants.TEMPLATE_OVERRIDE_UPDATE_MODE;
            dsTemplateOverrideList = updateAndRetrieveTemplateOverrides(inputModel);
            viewModel.Error = Data.PopulateErrorObject(err, dsTemplateOverrideList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);

            if (!viewModel.Error.HasError)
            {
                templateOverrideList = Data.PopulateObjectListFromTable<TemplateOverrideHeader>(dsTemplateOverrideList.Tables["TemplateOverrideHeader"]);
                templateOverrideCriterias = Data.PopulateObjectListFromTable<TemplateOverrideCriteria>(dsTemplateOverrideList.Tables["TemplateOverrideCriterias"]);

                //Set view model
                viewModel.TemplateOverrideList = templateOverrideList.OrderByDescending(s => s.TemplateOverrideId).ToList();

                foreach (TemplateOverrideCriteria criteria in templateOverrideCriterias)
                {
                    criteria.CriteriaTypeDescription = viewModel.GetPageText("TemplateCriteria-" + criteria.CriteriaType);
                }

                foreach (TemplateOverrideHeader template in viewModel.TemplateOverrideList)
                {
                    template.TemplateOverrideCriterias = templateOverrideCriterias.Where(s => s.TemplateUseId == template.TemplateOverrideId).ToList();
                    template.TemplateOverrideCriteriasFormatted = template.TemplateOverrideCriterias.GroupBy(g => g.CriteriaTypeDescription).Select(s => new TemplateOverrideCriteriaFormatted { CriteriaTypeDescription = s.Key, CriteriaValues = string.Join(",", s.Select(e => e.CriteriaValue).ToList()) }).ToList();
                }
            }
            return viewModel;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Get template overrides from the TM001 and TM002 files based on the given parameters
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>A data set of results for all the template overrides</returns>
        private DataSet retrieveTemplateOverrides(TemplateOverrideInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentTemplateOverride talTemplateOverride = new TalentTemplateOverride();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talTemplateOverride.Settings = settings;
            talTemplateOverride.DeTemplate.BusinessUnit = inputModel.BusinessUnit;
            talTemplateOverride.DeTemplate.Mode = inputModel.Mode;
            talTemplateOverride.DeTemplate.Source = GlobalConstants.SOURCE;
            err = talTemplateOverride.GetTemplateOverrideList();
            dsResults = talTemplateOverride.ResultDataSet;
            return dsResults;
        }
   
        /// <summary>
        ///  Create template override and get new template override list from the TM001 and TM002 files based on the given parameters
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>A data set of results for all the template overrides</returns>
        private DataSet createAndRetrieveTemplateOverrides(TemplateOverrideInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentTemplateOverride talTemplateOverride = new TalentTemplateOverride();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            List<TemplateOverrideCriteria> overrideCriterias = new List<TemplateOverrideCriteria>();


            talTemplateOverride.Settings = settings;
            talTemplateOverride.DeTemplate.BusinessUnit = inputModel.BusinessUnit;
            talTemplateOverride.DeTemplate.Mode = inputModel.Mode;

            foreach(TemplateOverrideCriteria overridecriteria in inputModel.TemplateOverrideCriterias)
            {
                TemplateOverrideCriteria criteria = new TemplateOverrideCriteria();
                criteria.CriteriaType = overridecriteria.CriteriaType;
                criteria.CriteriaValue = overridecriteria.CriteriaValue;
                overrideCriterias.Add(criteria);
            }
            talTemplateOverride.DeTemplate.Source = GlobalConstants.SOURCE;
            talTemplateOverride.DeTemplate.TemplateOverrideId = inputModel.TemplateOverrideID;
            talTemplateOverride.DeTemplate.Description = inputModel.Description;
            talTemplateOverride.DeTemplate.BoxOfficeUser = inputModel.BoxOfficeUser;
            talTemplateOverride.DeTemplate.SaleConfirmationEmailId = inputModel.SaleConfirmationEmailID;
            talTemplateOverride.DeTemplate.SaleConfirmationEmailDescription = inputModel.SaleConfirmationEmailDescription;
            talTemplateOverride.DeTemplate.QAndATemplateID = inputModel.QAndATemplateID;
            talTemplateOverride.DeTemplate.QAndATemplateDescription = inputModel.QAndATemplateDescription;
            talTemplateOverride.DeTemplate.DataCaptureTemplateId = inputModel.DataCaptureTemplateID;
            talTemplateOverride.DeTemplate.DataCaptureTemplateDescription = inputModel.DataCaptureTemplateDescription;
            talTemplateOverride.DeTemplate.AutoExpandQAndA = inputModel.AutoExpandQAndA;
            talTemplateOverride.DeTemplate.TemplateOverrideCriterias = overrideCriterias;
            err = talTemplateOverride.CreateTemplateOverride();
            dsResults = talTemplateOverride.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// This method update template override and get updated template override list from the TM001 and TM002 files based on the given parameters
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>A data set of results for all the template overrides</returns>
        private DataSet updateAndRetrieveTemplateOverrides(TemplateOverrideInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentTemplateOverride talTemplateOverride = new TalentTemplateOverride();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            List<TemplateOverrideCriteria> overrideCriterias = new List<TemplateOverrideCriteria>();


            talTemplateOverride.Settings = settings;
            talTemplateOverride.DeTemplate.BusinessUnit = inputModel.BusinessUnit;
            talTemplateOverride.DeTemplate.Mode = inputModel.Mode;

            foreach (TemplateOverrideCriteria overridecriteria in inputModel.TemplateOverrideCriterias)
            {
                TemplateOverrideCriteria criteria = new TemplateOverrideCriteria();
                criteria.CriteriaType = overridecriteria.CriteriaType;
                criteria.CriteriaValue = overridecriteria.CriteriaValue;
                overrideCriterias.Add(criteria);
            }
            talTemplateOverride.DeTemplate.Source = GlobalConstants.SOURCE;
            talTemplateOverride.DeTemplate.TemplateOverrideId = inputModel.TemplateOverrideID;
            talTemplateOverride.DeTemplate.Description = inputModel.Description;
            talTemplateOverride.DeTemplate.BoxOfficeUser = inputModel.BoxOfficeUser;
            talTemplateOverride.DeTemplate.SaleConfirmationEmailId = inputModel.SaleConfirmationEmailID;
            talTemplateOverride.DeTemplate.SaleConfirmationEmailDescription = inputModel.SaleConfirmationEmailDescription;
            talTemplateOverride.DeTemplate.QAndATemplateID = inputModel.QAndATemplateID;
            talTemplateOverride.DeTemplate.QAndATemplateDescription = inputModel.QAndATemplateDescription;
            talTemplateOverride.DeTemplate.DataCaptureTemplateId = inputModel.DataCaptureTemplateID;
            talTemplateOverride.DeTemplate.DataCaptureTemplateDescription = inputModel.DataCaptureTemplateDescription;
            talTemplateOverride.DeTemplate.AutoExpandQAndA = inputModel.AutoExpandQAndA;
            talTemplateOverride.DeTemplate.TemplateOverrideCriterias = overrideCriterias;
            err = talTemplateOverride.UpdateTemplateOverride();
            dsResults = talTemplateOverride.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        ///  This method deletes the template and get new template override list from the TM001 and TM002 files based on the given parameters
        /// </summary>
        /// <param name="inputModel">The given template override input model</param>
        /// <returns>A data set of results for all the template overrides</returns>
        private DataSet deleteAndRetrieveTemplateOverrides(TemplateOverrideInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentTemplateOverride talTemplateOverride = new TalentTemplateOverride();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talTemplateOverride.Settings = settings;
            talTemplateOverride.DeTemplate.Source = GlobalConstants.SOURCE;
            talTemplateOverride.DeTemplate.BusinessUnit = inputModel.BusinessUnit;
            talTemplateOverride.DeTemplate.Mode = inputModel.Mode;
            talTemplateOverride.DeTemplate.TemplateOverrideId = inputModel.TemplateOverrideID;
            talTemplateOverride.DeTemplate.BoxOfficeUser = inputModel.BoxOfficeUser;

            err = talTemplateOverride.DeleteTemplateOverride();
            dsResults = talTemplateOverride.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        ///  Get template override criterias
        /// </summary>
        /// <returns>A data set of results for all the template overrides</returns>
        private DataSet retriveOverrideCriterias()
        {
            DataSet dsResults = new DataSet();
            TalentTemplateOverride talTemplateOverride = new TalentTemplateOverride();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talTemplateOverride.Settings = settings;
            talTemplateOverride.DeTemplate.Source = GlobalConstants.SOURCE;
            err = talTemplateOverride.GetTemplateOverrideCriterias();
            dsResults = talTemplateOverride.ResultDataSet;
            return dsResults;
        }
        #endregion
    }
}
