using System;
using System.Collections.Generic;
using System.Linq;
using TalentBusinessLogic.Models;
using System.Data;
using Talent.Common;
using AutoMapper;

namespace TalentBusinessLogic.ModelBuilders.Profile
{
    public class PasswordModelBuilders : BaseModelBuilder
    {
        private DataTable _dtCustomerResults;
        private TalentCustomer _talCust = new TalentCustomer();
        private bool _doTokenHashing;

        #region Forgotten Password Page Builders

        /// <summary>
        /// Perform the forgotten password process based on the given input model
        /// </summary>
        /// <param name="inputModel">The forgotten password input model object</param>
        /// <returns>Forgotten password view model correctly populated for display, regardless of mode</returns>
        public ForgottenPasswordViewModel ForgottenPassword(ForgottenPasswordInputModel inputModel)
        {
            ForgottenPasswordViewModel viewModel;
            switch (inputModel.Mode)
            {
                case GlobalConstants.PASSWORD_ENC_MODE_INITIAL:
                    viewModel = new ForgottenPasswordViewModel(true);
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                    break;
                case GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN:
                    viewModel = new ForgottenPasswordViewModel(true);
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN;
                    break;
                case GlobalConstants.PASSWORD_ENC_MODE_RESPONSE:
                    viewModel = getCustomerDetailsFromBackEnd(inputModel);
                    break;
                default:
                    viewModel = new ForgottenPasswordViewModel(true);
                    break;
            }
            return viewModel;
        }

        /// <summary>
        /// Perform the customer details retrieval process and return a view model with error handling
        /// </summary>
        /// <param name="inputModel">The forgotten password input model object</param>
        /// <returns>Forgotten password view model correctly populated for display</returns>
        private ForgottenPasswordViewModel getCustomerDetailsFromBackEnd(ForgottenPasswordInputModel inputModel)
        {
            ErrorObj talentErrObj = new ErrorObj();
            DECustomer deCust = new DECustomer();
            Mapper.CreateMap<ForgottenPasswordInputModel, DECustomer>();
            DECustomerV11 deCustV11 = new DECustomerV11();
            ForgottenPasswordViewModel viewModel = new ForgottenPasswordViewModel(true);

            deCust = Mapper.Map<DECustomer>(inputModel);
            deCust.Source = GlobalConstants.SOURCE;
            deCustV11.DECustomersV1.Add(deCust);
            _talCust.DeV11 = deCustV11;
            _talCust.Settings = Environment.Settings.DESettings;
            talentErrObj = _talCust.CustomerRetrieval();
            viewModel.Error = Data.PopulateErrorObject(talentErrObj, _talCust.ResultDataSet, _talCust.Settings, 2);

            if (!viewModel.Error.HasError)
            {
                //Add the tokens to the result Set
                _dtCustomerResults = _talCust.ResultDataSet.Tables["CustomerResults"];
                if (_dtCustomerResults.Rows.Count == 1)
                {
                    _doTokenHashing = inputModel.DoTokenHashing;
                    checkCustomerTokens();
                    if (generateResetTokens())
                    {
                        if (generateTableRecords())
                        {
                            generateEmail();
                            viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_RESPONSE;
                        }
                        else
                        {
                            viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                            viewModel.Error.HasError = true;
                            viewModel.Error.ErrorMessage = viewModel.GetPageText("UnspecifiedError");
                            _talCust.Settings.Logging.ErrorObjectLog("ForgottenPassword.aspx - Reset Password email", "FPW", "Cannot create reset password table records", "PasswordEncryptionLog");
                        }
                    }
                    else
                    {
                        viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                        viewModel.Error.HasError = true;
                        viewModel.Error.ErrorMessage = viewModel.GetPageText("UnspecifiedError");
                        _talCust.Settings.Logging.ErrorObjectLog("ForgottenPassword.aspx - Reset Password email", "FPW", "Cannot create reset password tokens", "PasswordEncryptionLog");
                    }
                }
                else
                {
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                    viewModel.Error.HasError = true;
                    viewModel.Error.ErrorMessage = viewModel.GetPageText("UnspecifiedError");
                    _talCust.Settings.Logging.ErrorObjectLog("ForgottenPassword.aspx - Reset Password email", "FPW", "No customer records have been returned from WS009R", "PasswordEncryptionLog");
                }
            }
            else
            {
                viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                _talCust.Settings.Logging.ErrorObjectLog("ForgottenPassword.aspx - Reset Password email", "FPW", viewModel.Error.ErrorMessage, "PasswordEncryptionLog");
            }
            return viewModel;
        }

        /// <summary>
        /// Check the customer token and set them as used if there are any
        /// </summary>
        private void checkCustomerTokens()
        {
            if (_doTokenHashing)
            {
                string customerNumber = string.Empty;
                int affectedRows = 0;
                customerNumber = _dtCustomerResults.Rows[0]["CustomerNumber"].ToString();
                affectedRows = TDataObjects.ProfileSettings.tblForgottenPassword.SetCustomerTokensAsUsed(customerNumber, "EXPIRED");
            }
        }

        /// <summary>
        /// Generate the password reset tokens to be used when resetting the password
        /// </summary>
        /// <returns>True if the tokenisation has worked, otherwise false</returns>
        private bool generateResetTokens()
        {
            if (_doTokenHashing)
            {
                PasswordHash pH = new PasswordHash();
                List<String> tokenList = new List<String>();

                //Add the columns to store the tokens
                _dtCustomerResults.Columns.Add("ResetToken");
                _dtCustomerResults.Columns.Add("ResetHash");

                //Assign the values to the columns
                tokenList = pH.ResetPasswordToken();
                if (tokenList.Count() > 0)
                {
                    _dtCustomerResults.Rows[0]["ResetToken"] = tokenList[0];
                    _dtCustomerResults.Rows[0]["ResetHash"] = tokenList[1];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Generate the table records in tbl_forgotten_password based on the tokens created
        /// </summary>
        /// <returns>True if the records have been created otherwise false</returns>
        private bool generateTableRecords()
        {
            if (_doTokenHashing)
            {
                string hashedToken = String.Empty;
                string customerNumber = String.Empty;
                string email = String.Empty;
                int affectedRows;
                hashedToken = _dtCustomerResults.Rows[0]["ResetHash"].ToString();
                customerNumber = _dtCustomerResults.Rows[0]["CustomerNumber"].ToString();
                email = _dtCustomerResults.Rows[0]["EmailAddress"].ToString().Trim();
                affectedRows = TDataObjects.ProfileSettings.tblForgottenPassword.Insert(Environment.Settings.BusinessUnit, Environment.Settings.Partner, hashedToken, "VALID", customerNumber, email, Environment.Settings.DefaultValues.ForgottenPasswordExpiryTime);
                if (!(affectedRows > 0))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Generate the forgotten password E-Mail to be sent by the monitor based on the current customer request
        /// </summary>
        private void generateEmail()
        {
            TalentEmail talEmail = new TalentEmail();
            string xmlDoc = string.Empty;
            string email = string.Empty;
            string customerNumber = string.Empty;
            string loginURL = string.Empty;
            string token = string.Empty;

            //Assign the variables necessary
            if (_doTokenHashing)
            {
                token = _dtCustomerResults.Rows[0]["ResetToken"].ToString().Trim();
            }
            email = _dtCustomerResults.Rows[0]["EmailAddress"].ToString().Trim();
            customerNumber = _dtCustomerResults.Rows[0]["CustomerNumber"].ToString();
            xmlDoc = talEmail.CreateForgottenPasswordXmlDocument(Environment.Settings.DefaultValues.ForgottenPasswordFromEmail, email, string.Empty, string.Empty, Environment.Settings.Partner, customerNumber, token);
            //Add the record to tbl_offline_processing
            TDataObjects.AppVariableSettings.TblOfflineProcessing.Insert(Environment.Settings.BusinessUnit, "*ALL", "Pending", 0, "", "EmailMonitor", "ForgottenPassword", xmlDoc, "");
        }

        #endregion

        #region Reset  Password Model Builders

        /// <summary>
        /// Perform the reset password process based on the given input model
        /// </summary>
        /// <param name="inputModel">The reset password input model object</param>
        /// <returns>Reset password view model correctly populated for display, regardless of mode</returns>
        public ResetPasswordViewModel ResetPassword(ResetPasswordInputModel inputModel)
        {
            ResetPasswordViewModel viewModel;
            switch (inputModel.Mode)
            {
                case GlobalConstants.PASSWORD_ENC_MODE_INITIAL:
                    viewModel = new ResetPasswordViewModel(true);
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                    break;
                case GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN:
                    viewModel = new ResetPasswordViewModel(true);
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_USER_SIGNED_IN;
                    break;
                case GlobalConstants.PASSWORD_ENC_MODE_RESPONSE:
                    viewModel = resetUserPassword(inputModel);
                    break;
                default:
                    viewModel = new ResetPasswordViewModel(true);
                    break;
            }
            return viewModel;
        }

        /// <summary>
        /// Does the main work for resetting a user's password. Validates their token, then sends the details to the backend for update.
        /// </summary>
        /// <param name="inputModel">The reset password input model</param>
        /// <returns>viewModel for handling</returns>
        private ResetPasswordViewModel resetUserPassword(ResetPasswordInputModel inputModel)
        {
            //Initialise everything we need.
            ResetPasswordViewModel viewModel = new ResetPasswordViewModel(true);
            ErrorObj talentErrObj = new ErrorObj();
            DECustomer deCust = new DECustomer();
            Mapper.CreateMap<ResetPasswordInputModel, DECustomer>();
            DECustomerV11 deCustV11 = new DECustomerV11();
            PasswordHash pH = new PasswordHash();

            //Make sure token is still valid
            inputModel = validateToken(inputModel);
            if (inputModel.IsValid)
            {
                //Hash the user's new password.
                inputModel.NewHashedPassword = pH.HashSalt(inputModel.NewPassword, Environment.Settings.DefaultValues.SaltString);

                //Map the inputModel parameters to a DECustomer object.
                deCust = Mapper.Map<DECustomer>(inputModel);
                deCust.Source = GlobalConstants.SOURCE;
                deCustV11.DECustomersV1.Add(deCust);
                _talCust.DeV11 = deCustV11;
                _talCust.Settings = Environment.Settings.DESettings;

                //Backend call, and error checking
                talentErrObj = _talCust.ResetPassword();
                viewModel.Error = Data.PopulateErrorObject(talentErrObj, _talCust.ResultDataSet, _talCust.Settings, null);
                if (!viewModel.Error.HasError)
                {
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_RESPONSE;
                    expireUsedToken(inputModel);
                }
                else
                {
                    viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                    if (viewModel.Error == null)
                    {
                        viewModel.Error = new ErrorModel();
                        viewModel.Error.HasError = true;
                        viewModel.Error.ErrorMessage = viewModel.GetPageText("UnspecifiedError");
                    }
                }
            }
            else
            {
                viewModel.Mode = GlobalConstants.PASSWORD_ENC_MODE_INITIAL;
                if (viewModel.Error == null)
                {
                    viewModel.Error = new ErrorModel();
                    viewModel.Error.HasError = true;
                    viewModel.Error.ErrorMessage = viewModel.GetPageText("GenericError");
                }
            }
            return viewModel;
        }

        /// <summary>
        /// Validates the user's token in tbl_forgotten_password
        /// </summary>
        private ResetPasswordInputModel validateToken(ResetPasswordInputModel inputModel)
        {
            string customerNumber = string.Empty;
            string token = string.Empty;
            string hashedToken = string.Empty;
            PasswordHash passHash = new PasswordHash();
            DataTable tokenRows = new DataTable();
            DateTime timeNow;

            customerNumber = inputModel.CustomerNumber;
            token = inputModel.Token;
            timeNow = inputModel.DateNow;
            hashedToken = passHash.HashTokenWithCurrentAlgorithm(token);
            //Get row from table
            tokenRows = TDataObjects.ProfileSettings.tblForgottenPassword.GetByHashedToken(hashedToken);
            if (tokenRows.Rows.Count > 0)
            {
                //Check everything individually. Since certain patterns indicate hacking attempts and should be logged
                if (tokenRows.Rows.Count > 1)
                {
                    TDataObjects.ProfileSettings.tblForgottenPassword.SetTokenAsUsed(hashedToken);
                    _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Reset Password validation fail", "RPW-001", "More than 1 token found in tbl_forgotten_password. Customer number:" + inputModel.CustomerNumber, "PasswordEncryptionLog");
                }
                else
                {
                    //Check customer number
                    if (tokenRows.Rows[0]["CUSTOMER_NUMBER"].ToString().Trim() == customerNumber)
                    {
                        inputModel.IsCustomerValid = true;
                        inputModel.EmailAddress = tokenRows.Rows[0]["EMAIL_ADDRESS"].ToString().Trim();
                        inputModel.UserName = customerNumber;
                    }
                    else
                    {
                        inputModel.IsCustomerValid = false;
                        _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Reset Password validation fail", "RPW-002", "customer number in tbl_forgotten_password doesn't match the requested token. A user is attempting to reset a password suspiciously with the customer number:" + inputModel.CustomerNumber, "PasswordEncryptionLog");
                    }
                    //Check token
                    if (tokenRows.Rows[0]["HASHED_TOKEN"].ToString().Trim() == hashedToken)
                    {
                        inputModel.IsTokenValid = true;
                        inputModel.HashedToken = hashedToken;
                    }
                    else
                    {
                        inputModel.IsTokenValid = false;
                        _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Reset Password validation fail", "RPW-003", "hashed token in tbl_forgotten_password doesn't match the requested token. A user is attempting to reset a password suspiciously with the customer number:" + inputModel.CustomerNumber, "PasswordEncryptionLog");
                    }
                    //Check date
                    if (Convert.ToDateTime(tokenRows.Rows[0]["EXPIRE_TIMESTAMP"]) >= timeNow)
                    {
                        inputModel.IsDateValid = true;
                    }
                    else
                    {
                        inputModel.IsDateValid = false;
                        _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Reset Password validation fail", "RPW-004", "The date in tbl_forgotten_password doesn't match the requested token. Token has probably expired customer number:" + inputModel.CustomerNumber, "PasswordEncryptionLog");
                    }
                    //If everything is valid then the input model is valid
                    if (inputModel.IsTokenValid && inputModel.IsDateValid && inputModel.IsCustomerValid)
                    {
                        inputModel.IsValid = true;
                    }
                    else
                    {
                        inputModel.IsValid = false;
                    }
                }
            }
            else
            {
                //Token not valid, error.
                inputModel.IsValid = false;
                _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Reset Password validation fail", "RPW-000", "No token found in tbl_forgotten_password. A user is attempting to reset a password suspiciously with the customer number:" + inputModel.CustomerNumber, "PasswordEncryptionLog");
            }
            return inputModel;
        }

        /// <summary>
        /// Expire the used tokens based on the current token
        /// </summary>
        /// <param name="inputModel">The reset password input model</param>
        public void expireUsedToken(ResetPasswordInputModel inputModel)
        {
            int affectedRows = 0;
            affectedRows = TDataObjects.ProfileSettings.tblForgottenPassword.SetTokenAsUsed(inputModel.HashedToken);
            if (affectedRows != 1)
            {
                _talCust.Settings.Logging.ErrorObjectLog("ResetPassword.aspx - Token expiry failure.", "RPW-005", "Could not set tokens to expired for customer: " + inputModel.CustomerNumber, "PasswordEncryptionLog");
            }
        }

        #endregion
    }
}
