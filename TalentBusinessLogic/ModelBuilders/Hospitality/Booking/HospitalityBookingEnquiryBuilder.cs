using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Talent.Common;
using TalentBusinessLogic.BusinessObjects.Definitions;
using TalentBusinessLogic.DataTransferObjects.Hospitality;
using TalentBusinessLogic.Models;
using TalentBusinessLogic.Models.Hospitality.Booking;

namespace TalentBusinessLogic.ModelBuilders.Hospitality.Booking
{
    public class HospitalityBookingEnquiryBuilder : BaseModelBuilder
    {
        #region Public Functions

        /// <summary>
        /// Retrieve the hospitality bookings
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrieveHospitalityBookings(HospitalityBookingEnquiryInputModel inputModel)
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            DataSet dsBookingList = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<HospitalityBookingEnquiryDetails> hospitalityBookingList = new List<HospitalityBookingEnquiryDetails>();
            List<HospitalityBookingPrintDetails> hospitalityBookingPrintDetails = new List<HospitalityBookingPrintDetails>();
            CultureInfo culture = new CultureInfo(Environment.Settings.DefaultValues.Culture);

            dsBookingList = retrieveBookings(inputModel);
            viewModel.Error = Data.PopulateErrorObject(err, dsBookingList, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 3);
            hospitalityBookingList = Data.PopulateObjectListFromTable<HospitalityBookingEnquiryDetails>(dsBookingList.Tables["HospitalityBookings"]);
            hospitalityBookingPrintDetails = Data.PopulateObjectListFromTable<HospitalityBookingPrintDetails>(dsBookingList.Tables["HospitalityPrintDetails"]);

            //Set view model
            viewModel.HospitalityBookingEnquiryList = hospitalityBookingList.OrderByDescending(s => s.BookingRef).ToList();
            if (hospitalityBookingPrintDetails.Count>0)
            {
                viewModel.NumberOfBookings = hospitalityBookingPrintDetails.FirstOrDefault().NumberOfBookings;
                viewModel.NumberOfTicketsToPrint = hospitalityBookingPrintDetails.FirstOrDefault().NumberOfTicketsToPrint;
            }

            foreach (HospitalityBookingEnquiryDetails booking in viewModel.HospitalityBookingEnquiryList)
            {
                booking.StatusDescription = viewModel.GetPageText("BookingStatus-" + booking.Status);
                booking.ProcessedByLabel = viewModel.GetPageText("AgentMask");
                booking.ProcessedByLabel = booking.ProcessedByLabel.Replace("<<AgentCode>>", booking.ProcessedBy);
                booking.ProcessedByLabel = booking.ProcessedByLabel.Replace("<<AgentDescription>>", new Agent().GetAgentDescriptiveNameByAgentUserCode(booking.ProcessedBy));
                booking.ProductLabel = viewModel.GetPageText("ProductMask");
                booking.ProductLabel = booking.ProductLabel.Replace("<<ProductCode>>", booking.ProductCode);
                booking.ProductLabel = booking.ProductLabel.Replace("<<ProductDescription>>", booking.ProductDescription);
                booking.PackageLabel = viewModel.GetPageText("PackageMask");
                booking.PackageLabel = booking.PackageLabel.Replace("<<PackageCode>>", booking.PackageId);
                booking.PackageLabel = booking.PackageLabel.Replace("<<PackageDescription>>", booking.PackageDescription);
                booking.FormattedNettValueExclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.NettValueExclDiscount, settings.BusinessUnit, settings.Partner);
                booking.FormattedNettValueInclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.NettValueInclDiscount, settings.BusinessUnit, settings.Partner);
                booking.FormattedGrossValueExclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.GrossValueExclDiscount, settings.BusinessUnit, settings.Partner);
                booking.FormattedGrossValueInclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.GrossValueInclDiscount, settings.BusinessUnit, settings.Partner);
                booking.FormattedDiscountExclVat = TDataObjects.PaymentSettings.FormatCurrency(booking.DiscountExclVat, settings.BusinessUnit, settings.Partner);
                booking.FormattedDiscountInclVat = TDataObjects.PaymentSettings.FormatCurrency(booking.DiscountInclVat, settings.BusinessUnit, settings.Partner);
                booking.FormattedVatValueExclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.VatValueExclDiscount, settings.BusinessUnit, settings.Partner);
                booking.FormattedVatValueInclDiscount = TDataObjects.PaymentSettings.FormatCurrency(booking.VatValueInclDiscount, settings.BusinessUnit, settings.Partner);
                booking.QAndAStatusDescription = viewModel.GetPageText("QandAStatus-" + booking.QAndAStatus);
                booking.PrintStatus = getPrintStatusDescription(booking.PrintStatus);
                if (booking.Status == GlobalConstants.SOLD_BOOKING_STATUS && booking.QAndAStatus == GlobalConstants.QANDA_STATUS_INCOMPLETE)
                {
                    booking.RequiredQAndAReminder = true;
                }
                else
                {
                    booking.RequiredQAndAReminder = false;
                }

                StringBuilder bookingUrl = new StringBuilder();
                bookingUrl.Append("~/PagesPublic/Hospitality/HospitalityBooking.aspx?product=").Append(booking.ProductCode);
                bookingUrl.Append("&packageId=").Append(booking.PackageDefId);
                bookingUrl.Append("&callid=").Append(booking.BookingRef);
                bookingUrl.Append("&status=").Append(booking.Status);

                if (inputModel.LoggedInCustomerNumber.ToString().PadLeft(12, '0') == booking.TicketingCustomerMember.ToString().PadLeft(12, '0'))
                {
                    booking.RequiresLogin = false;
                    booking.BookingForwardingUrl = bookingUrl.ToString();
                    booking.PDFForwardingUrl = string.Empty;
                }
                else
                {
                    booking.RequiresLogin = true;
                    StringBuilder forwardingUrl = new StringBuilder();
                    forwardingUrl.Append("~/PagesPublic/Profile/CustomerSelection.aspx");
                    forwardingUrl.Append("?Type=Submit").Append("&FancardBox=").Append(booking.TicketingCustomerMember);
                    forwardingUrl.Append("&ReturnUrl=").Append(HttpUtility.UrlEncode(bookingUrl.ToString()));
                    booking.BookingForwardingUrl = forwardingUrl.ToString();

                    StringBuilder pdfForwardingUrl = new StringBuilder();
                    string enquiryUrl = "~/PagesPublic/Hospitality/HospitalityBookingEnquiry.aspx";
                    pdfForwardingUrl.Append("~/PagesPublic/Profile/CustomerSelection.aspx");
                    pdfForwardingUrl.Append("?Type=Submit").Append("&FancardBox=").Append(booking.TicketingCustomerMember);
                    pdfForwardingUrl.Append("&ReturnUrl=");
                    pdfForwardingUrl.Append(HttpUtility.UrlEncode(enquiryUrl));
                    booking.PDFForwardingUrl = pdfForwardingUrl.ToString();
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Retrieve Agent List
        /// </summary>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrieveAgentList()
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            viewModel.AgentList = new Agent().retrieveAgents();
            return viewModel;
        }

        /// <summary>
        /// Retrieve BookingStatus List
        /// </summary>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrieveBookingStatusList()
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            Dictionary<string, string> bookingStatusList = new Dictionary<string, string>();
            const string bookingStatus = "BookingStatus-";
            bookingStatusList.Add(String.Empty, viewModel.GetPageText("AllBookingStatusText"));
            bookingStatusList.Add(GlobalConstants.ENQUIRY_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.ENQUIRY_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.SOLD_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.SOLD_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.QUOTE_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.QUOTE_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.RESERVATION_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.RESERVATION_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.CANCELLED_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.CANCELLED_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.CREDIT_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.CREDIT_BOOKING_STATUS));
            bookingStatusList.Add(GlobalConstants.EXPIRED_BOOKING_STATUS, viewModel.GetPageText(bookingStatus + GlobalConstants.EXPIRED_BOOKING_STATUS));
            viewModel.StatusList = bookingStatusList;
            return viewModel;
        }

        /// <summary>
        /// Retrieve mark order for list
        /// </summary>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrieveMarkOrderForList()
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            Dictionary<string, string> markOrderForList = new Dictionary<string, string>();

            markOrderForList.Add(String.Empty, viewModel.GetPageText("MarkOrderForAll"));
            markOrderForList.Add(GlobalConstants.MARK_FOR_PERSONAL, viewModel.GetPageText("MarkOrderForPersonal"));
            markOrderForList.Add(GlobalConstants.MARK_FOR_BUISINESS, viewModel.GetPageText("MarkOrderForBusiness"));
            viewModel.MarkForOrderList = markOrderForList;
            return viewModel;
        }

        /// <summary>
        /// Retrieve Q&A Status List
        /// </summary>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrieveQandAStatusList()
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            Dictionary<string, string> qandAStatusList = new Dictionary<string, string>();
            const string qAndAStatus = "QandAStatus-";
            qandAStatusList.Add(String.Empty, viewModel.GetPageText("AllQandABookingStatusText"));
            qandAStatusList.Add(GlobalConstants.QANDA_STATUS_COMPLETE, viewModel.GetPageText(qAndAStatus + GlobalConstants.QANDA_STATUS_COMPLETE));
            qandAStatusList.Add(GlobalConstants.QANDA_STATUS_INCOMPLETE, viewModel.GetPageText(qAndAStatus + GlobalConstants.QANDA_STATUS_INCOMPLETE));
            viewModel.QandAStatusList = qandAStatusList;
            return viewModel;
        }

        /// <summary>
        /// Retrieve PrintStatus List
        /// </summary>
        /// <returns>The formed hospitality booking enquiry view model</returns>
        public HospitalityBookingEnquiryViewModel RetrievePrintStatusList()
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            Dictionary<string, string> printStatusList = new Dictionary<string, string>();
            const string printStatus = "PrintStatus-";
            printStatusList.Add(String.Empty, viewModel.GetPageText("AllPrintStatusText"));            
            printStatusList.Add(GlobalConstants.NA_STATUS, viewModel.GetPageText(printStatus + GlobalConstants.NA_STATUS));
            printStatusList.Add(GlobalConstants.NOT_PRINTED_STATUS, viewModel.GetPageText(printStatus + GlobalConstants.NOT_PRINTED_STATUS));
            printStatusList.Add(GlobalConstants.PARTIALLY_PRINTED_STATUS, viewModel.GetPageText(printStatus + GlobalConstants.PARTIALLY_PRINTED_STATUS));
            printStatusList.Add(GlobalConstants.FULLY_PRINTED_STATUS, viewModel.GetPageText(printStatus + GlobalConstants.FULLY_PRINTED_STATUS));            
            viewModel.PrintStatusList = printStatusList;            
            return viewModel;
        }

        /// <summary>
        /// Send Question & Answer reminder email
        /// </summary>
        /// <param name="inputModel">Object of HospitalityBookingEnquiryInputModel</param>
        /// <returns>Object of HospitalityBookingEnquiryViewModel</returns>
        public HospitalityBookingEnquiryViewModel SendQAndAReminder(HospitalityBookingEnquiryInputModel inputModel)
        {
            HospitalityBookingEnquiryViewModel viewModel = RetrieveHospitalityBookings(inputModel);
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;
            List<HospitalityBookingEnquiryDetails> hospitalityBookingList = viewModel.HospitalityBookingEnquiryList;
            int affectedRow = 0;
            for (int i = 0; i < inputModel.CallIdList.Count; i++)
            {
                HospitalityBookingEnquiryDetails hospitalityBookingEnquiry = new HospitalityBookingEnquiryDetails();
                hospitalityBookingEnquiry = hospitalityBookingList.FirstOrDefault(h => h.BookingRef.ToString() == inputModel.CallIdList[i]);
                if (hospitalityBookingEnquiry.Status == GlobalConstants.SOLD_BOOKING_STATUS && hospitalityBookingEnquiry.QAndAStatus == GlobalConstants.QANDA_STATUS_INCOMPLETE)
                {
                    TalentEmail talEmail = new TalentEmail();
                    string xmlDoc = string.Empty;
                    string email = string.Empty;
                    string customerNumber = hospitalityBookingEnquiry.TicketingCustomerMember;
                    string bookingURL = string.Empty;
                    string callId = inputModel.CallIdList[i];
                    string templateId = string.Empty;
                    inputModel.CallID = Convert.ToDecimal(inputModel.CallIdList[i]);
                    inputModel.CustomerNumber = customerNumber;
                    DataSet dsCustomerDetails = new DataSet();
                    dsCustomerDetails = retrieveCustomerDetails(inputModel);
                    viewModel.Error = Data.PopulateErrorObject(err, dsCustomerDetails, settings, 2);
                    if (!viewModel.Error.HasError)
                    {
                        DataTable dt = dsCustomerDetails.Tables["CustomerResults"];
                        DataRow drUser = dt.Rows[0];
                        email = drUser["EmailAddress"].ToString().Trim();
                        bookingURL = String.Concat("/PagesPublic/Hospitality/HospitalityBooking.aspx", "?callid=", callId, "&status=", GlobalConstants.SOLD_BOOKING_STATUS);
                        //Get the data from tbl_email_templates
                        DataTable dtemailTemplatesDefinition = TDataObjects.EmailTemplateSettings.TblEmailTemplates.GetAll();
                        if (dtemailTemplatesDefinition != null)
                        {
                            DataRow emailTemplateRow = dtemailTemplatesDefinition.AsEnumerable().FirstOrDefault(r => r.Field<string>("TEMPLATE_TYPE") == GlobalConstants.EMAIL_HOSPITALITY_Q_AND_A_REMINDER);
                            templateId = emailTemplateRow["MASTER"].ToString();
                        }
                        else
                        {
                            templateId = string.Empty;
                        }
                        //Below XML creation will be part of next user story - so comment it temporary
                        xmlDoc = talEmail.CreateHospitalityQAReminderXmlDocument(Environment.Settings.DefaultValues.DefaultTicketingEmailAddress, email, string.Empty, string.Empty, Environment.Settings.Partner, customerNumber, bookingURL, callId, templateId);
                        //Add the record to tbl_offline_processing
                        affectedRow = TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(Environment.Settings.BusinessUnit, "*ALL", "Pending", 0, "", "EmailMonitor", "HospitalityQ&AReminder", xmlDoc, "");
                        if (affectedRow > 0)
                        {
                            viewModel.SuccessfullySentQAndAReminder = true;
                        }
                        else
                        {
                            viewModel.Error.HasError = true;
                            viewModel.SuccessfullySentQAndAReminder = false;
                        }
                    }
                    else
                    {
                        viewModel.SuccessfullySentQAndAReminder = false;
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Hospitality bookings with print request status.
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>Status of Print Request</returns>
        public HospitalityBookingEnquiryViewModel PrintHospitalityBookings(HospitalityBookingEnquiryInputModel inputModel)
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            DataSet dsPrintBookings = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;

            viewModel = RetrieveHospitalityBookings(inputModel);

            if (!viewModel.Error.HasError)
            {
                dsPrintBookings = printBookings(inputModel);
                viewModel.Error = Data.PopulateErrorObject(err, dsPrintBookings, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 1);
                if (!viewModel.Error.HasError)
                {
                    DataTable dtStatusResult = dsPrintBookings.Tables[GlobalConstants.STATUS_RESULTS_TABLE_NAME];
                    if (dtStatusResult.Rows.Count > 0 && !string.IsNullOrEmpty(dtStatusResult.Rows[0]["ReturnCode"].ToString()))
                    {
                        viewModel.PrintRequestSuccess = false;                        
                    }
                    else
                    {
                        viewModel.PrintRequestSuccess = true;                        
                    }
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Generate the word document for the given call id
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>Status of document creation request</returns>
        public HospitalityBookingEnquiryViewModel GenerateDocumentForBooking(HospitalityBookingEnquiryInputModel inputModel)
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            DataSet dsDocumentBooking = new DataSet();
            ErrorObj err = new ErrorObj();
            DESettings settings = Environment.Settings.DESettings;

            viewModel = RetrieveHospitalityBookings(inputModel);

            if (!viewModel.Error.HasError)
            {
                dsDocumentBooking = createDocumentForBooking(inputModel);
                viewModel.Error = Data.PopulateErrorObject(err, dsDocumentBooking, settings, GlobalConstants.STATUS_RESULTS_TABLE_NAME, 2);
                if (!viewModel.Error.HasError)
                {
                    DataTable dtStatusResult = dsDocumentBooking.Tables[GlobalConstants.STATUS_RESULTS_TABLE_NAME];
                    DataTable dtDocumentInformation = dsDocumentBooking.Tables["DocumentInformation"];
                    if (dtStatusResult.Rows.Count > 0 && !string.IsNullOrEmpty(dtStatusResult.Rows[0]["ReturnCode"].ToString()))
                    {
                        viewModel.GenerateDocumentRequestSuccess = false;
                    }
                    else
                    {
                        viewModel.GenerateDocumentRequestSuccess = true;
                        viewModel.CallIdForDocumentProduction = inputModel.CallIdForDocumentProduction;                        
                        viewModel.MergedWordDocument = dtDocumentInformation.Rows[0].ItemArray[1].ToString();
                    }
                }
            }
            return viewModel;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Get the Hospitality Bookings from the C#xx files based on the given parameters
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>A data set of results for all the hospitality information</returns>
        private DataSet retrieveBookings(HospitalityBookingEnquiryInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();

            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();
            talPackage.DePackages.HospitalityBookingFilters.Agent = inputModel.BoxOfficeUser;
            talPackage.DePackages.HospitalityBookingFilters.CallId = inputModel.CallID;
            talPackage.DePackages.HospitalityBookingFilters.Fromdate = inputModel.FromDate;
            talPackage.DePackages.HospitalityBookingFilters.ToDate = inputModel.ToDate;
            talPackage.DePackages.HospitalityBookingFilters.Status = inputModel.Status;
            talPackage.DePackages.HospitalityBookingFilters.Customer = inputModel.CustomerNumber;
            talPackage.DePackages.HospitalityBookingFilters.PackageDescription = inputModel.Package;
            talPackage.DePackages.HospitalityBookingFilters.ProductDescription = inputModel.ProductCode;
            talPackage.DePackages.HospitalityBookingFilters.MaxRecords = inputModel.MaxRecords;
            talPackage.DePackages.HospitalityBookingFilters.MarkOrderFor = inputModel.MarkOrderFor;
            talPackage.DePackages.HospitalityBookingFilters.QandAStatus = inputModel.QandAStatus;
            talPackage.DePackages.HospitalityBookingFilters.PrintStatus = inputModel.PrintStatus;
            err = talPackage.GetHospitalityBookings();
            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Retrieve customer details
        /// </summary>
        /// <param name="inputModel">Object of HospitalityBookingEnquiryInputModel</param>
        /// <returns>Customer details</returns>
        private DataSet retrieveCustomerDetails(HospitalityBookingEnquiryInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
           
            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();
            talPackage.DePackages.HospitalityBookingFilters.Agent = inputModel.BoxOfficeUser;
            talPackage.DePackages.HospitalityBookingFilters.CallId = inputModel.CallID;
            talPackage.DePackages.HospitalityBookingFilters.Fromdate = inputModel.FromDate;
            talPackage.DePackages.HospitalityBookingFilters.ToDate = inputModel.ToDate;
            talPackage.DePackages.HospitalityBookingFilters.Status = inputModel.Status;
            talPackage.DePackages.HospitalityBookingFilters.Customer = inputModel.CustomerNumber;
            talPackage.DePackages.HospitalityBookingFilters.PackageDescription = inputModel.Package;
            talPackage.DePackages.HospitalityBookingFilters.ProductDescription = inputModel.ProductCode;
            talPackage.DePackages.HospitalityBookingFilters.MaxRecords = inputModel.MaxRecords;
            talPackage.DePackages.HospitalityBookingFilters.MarkOrderFor = inputModel.MarkOrderFor;
            talPackage.DePackages.HospitalityBookingFilters.QandAStatus = inputModel.QandAStatus;

            DECustomer deCust = new DECustomer();
            deCust.CustomerNumber = inputModel.CustomerNumber;
            deCust.CorporateSaleID = inputModel.CallID.ToString();

            DECustomerV11 deCustV11 = new DECustomerV11();
            deCustV11.DECustomersV1.Add(deCust);

            TalentCustomer talentCustomer = new TalentCustomer();
            talentCustomer.DeV11 = deCustV11;
            talentCustomer.Settings = settings;
            err = talentCustomer.CustomerRetrieval();

            dsResults = talentCustomer.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Get print status description from status code.
        /// </summary>
        /// <param name="printStatusCode">Print status code returned from CS001S</param>
        /// <returns>Print Status Description</returns>
        private string getPrintStatusDescription(string printStatusCode)
        {
            HospitalityBookingEnquiryViewModel viewModel = new HospitalityBookingEnquiryViewModel(true);
            String returnVal = viewModel.GetPageText("PrintStatus-" + printStatusCode);
            return returnVal;
        }

        /// <summary>
        ///Print Request status for filtered Hospitality Bookings.
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>A data set of result with  print request status</returns>
        private DataSet printBookings(HospitalityBookingEnquiryInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();
            talPackage.DePackages.HospitalityBookingFilters.Agent = inputModel.BoxOfficeUser;
            talPackage.DePackages.HospitalityBookingFilters.CallId = inputModel.CallID;
            talPackage.DePackages.HospitalityBookingFilters.Fromdate = inputModel.FromDate;
            talPackage.DePackages.HospitalityBookingFilters.ToDate = inputModel.ToDate;
            talPackage.DePackages.HospitalityBookingFilters.Status = inputModel.Status;
            talPackage.DePackages.HospitalityBookingFilters.Customer = inputModel.CustomerNumber;
            talPackage.DePackages.HospitalityBookingFilters.PackageDescription = inputModel.Package;
            talPackage.DePackages.HospitalityBookingFilters.ProductDescription = inputModel.ProductCode;
            talPackage.DePackages.HospitalityBookingFilters.MaxRecords = inputModel.MaxRecords;
            talPackage.DePackages.HospitalityBookingFilters.MarkOrderFor = inputModel.MarkOrderFor;
            talPackage.DePackages.HospitalityBookingFilters.QandAStatus = inputModel.QandAStatus;
            talPackage.DePackages.HospitalityBookingFilters.PrintStatus = inputModel.PrintStatus;
            talPackage.DePackages.BoxOfficeUser = inputModel.LoggedInBoxOfficeUser;
            talPackage.DePackages.CallId = inputModel.CallIdToBePrinted;
            err = talPackage.PrintHospitalityBookings();
            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }

        /// <summary>
        /// Create document for booking
        /// </summary>
        /// <param name="inputModel">The given hospitality booking enquiry input model</param>
        /// <returns>A data set of result with document creation status and path</returns>
        private DataSet createDocumentForBooking(HospitalityBookingEnquiryInputModel inputModel)
        {
            DataSet dsResults = new DataSet();
            TalentPackage talPackage = new TalentPackage();
            DESettings settings = Environment.Settings.DESettings;
            ErrorObj err = new ErrorObj();
            talPackage.Settings = settings;
            talPackage.DePackages.HospitalityBookingFilters = new HospitalityBookingFilters();            
            talPackage.DePackages.BoxOfficeUser = inputModel.LoggedInBoxOfficeUser;
            talPackage.DePackages.CallId = inputModel.CallIdForDocumentProduction;
            talPackage.DePackages.CustomerNumber = inputModel.CustomerNumber;
            err = talPackage.CreateHospitalityBookingDocument();
            dsResults = talPackage.ResultDataSet;
            return dsResults;
        }
        #endregion
    }
}
