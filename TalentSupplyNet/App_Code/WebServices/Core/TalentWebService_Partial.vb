Imports Microsoft.VisualBasic
Imports System.Xml
Imports System.IO
Imports Talent.Common
Imports System.Data
Imports System.Data.SqlClient
Namespace Talent.TradingPortal

    ''' <summary>
    ''' The centralised class where the respective webservice request and respeonse are managed
    ''' </summary>
    ''' <remarks>
    '''This is a partial class. Hence implementaion is shared between the files
    '''1. TalentWebService.vb
    '''2. TalentWebService_Partial.vb (this file)
    ''' </remarks>
    Partial Public Class TalentWebService

#Region "Class Level Fields"

        Private Const DSUploadTDDataRequest As String = "DSUploadTDDataRequest"

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' Invokes the web service.
        ''' </summary>
        ''' <remarks>
        ''' LogWriter has been commented. In future to log the progress status of web service remove those comments 
        ''' </remarks>
        ''' <param name="loginId">The login id.</param>
        ''' <param name="password">The password.</param>
        ''' <param name="company">The company.</param>
        ''' <param name="dataSetInput">The data set input.</param>
        ''' <returns>xml string</returns>
        Public Function InvokeWebService(ByVal loginId As String, _
                                      ByVal password As String, _
                                      ByVal company As String, _
                                      ByVal dataSetInput As DataSet) As String
            Dim businessUnit As String = String.Empty
            businessUnit = GetBusinessUnitFromURL(HttpContext.Current.Request.Url.ToString)
            Dim LoggingOn As Boolean = True
            Dim errObj As New ErrorObj
            Dim Defaults As New Defaults
            Dim currentDocumentVersion As String = "1.0"

            'Get Transaction ID
            With Defaults
                .BusinessUnit = businessUnit
                If TransactionID = String.Empty Then
                    errObj = .GetTransactionNumber()
                    TransactionID = .TransactionNumber.ToString
                End If
            End With
            ''LOG: web service has been invoked
            'LogWriter.WriteDBLog(LoggingOn, TransactionID, "InvokeWebService", company, loginId, WebServiceName)

            Dim xmlResp As XmlResponse = GetRelevantXmlResponseObject()
            ''LOG: Response has been created
            'LogWriter.WriteDBLog(LoggingOn, TransactionID, "LDS0005", resp)

            With xmlResp
                .TransactionID = TransactionID
                .RootElement = ResponseName()
                .LoginId = loginId
                .WebServiceName = WebServiceName
                .Company = company
            End With

            If errObj.HasError Then
                xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                ''LOG: Error in getting transaction ID
                'LogWriter.WriteDBLog(LoggingOn, TransactionID, "LDS0100", resp)
            Else
                ''LOG: Attempting to create profile
                'LogWriter.WriteDBLog(LoggingOn, TransactionID, "LDS0115", reqe)
                'valid transaction id 
                'Now authenticate the request
                Dim pr As New Profile
                pr.BusinessUnit = businessUnit
                pr.Company = company
                errObj = pr.CreateProfile(loginId, password, company, WebServiceName())

                If errObj.HasError Then
                    ''LOG: Authentication Error
                    'LogWriter.WriteDBLog(LoggingOn, TransactionID, errObj.ErrorMessage, errObj.ErrorStatus, errObj.ErrorNumber)
                    xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                Else
                    ''LOG: Profile created... Checking User Validity
                    'LogWriter.WriteDBLog(LoggingOn, TransactionID, "LDS0120", reqe)

                    'Set properties upon the xmlResponse object
                    'Response version - will default to 1.0 (within Profile object) if not set
                    currentDocumentVersion = pr.ResponseVersion()

                    'Not a valid user or not authorised to service - return response containing error.
                    If Not pr.ValidUser Then
                        xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                        ''LOG: User not valid... Response sent
                        'LogWriter.WriteDBLog(pr.WriteLog, TransactionID, "LDS0125", resp)
                    Else

                        ' Validate that the transaction ID is unique
                        errObj = validateTransactionNumber(TransactionID)
                        If errObj.HasError Then
                            xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                        Else
                            'valid transaction id and valid user

                            ''LOG: user valid... Defaults are loading                        
                            'LogWriter.WriteDBLog(pr.WriteLog, TransactionID, "LDS0130", reqe)

                            With Defaults
                                .BusinessUnit = businessUnit
                                .WebServiceName = WebServiceName
                                .Company = company
                                ' Override Defaults with profile settings
                                .DestinationDatabase = pr.WebServiceDestinationDatabase()
                                .Xsd = pr.Xsd
                                .CacheTimeMinutes = pr.CacheTimeMinutes()
                                .WriteLog = pr.WriteLog
                                .StoreXml = pr.StoreXml
                                .DatabaseType1 = pr.DatabaseType1
                                errObj = .GetDefaults()
                            End With

                            With xmlResp
                                'path to store the untransformed doc if store xml = true
                                .UntransformedDocPath = Defaults.XmlResponseDocPathU
                                'path to store the transformed doc if store xml = true
                                .TransformedDocPath = Defaults.XmlResponseDocPathT
                                .WriteLog = Defaults.WriteLog
                                .StoreXml = Defaults.StoreXml
                                .EmailFrom = Defaults.EmailFrom & String.Empty
                                .EmailCompany = pr.EmailCompany & String.Empty
                                .EmailUser = pr.EmailUser & String.Empty
                                .EmailXmlResponse = pr.EmailXmlResponse
                            End With
                            If errObj.HasError Then
                                xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                                ''LOG: error loading Defaults... response sent
                                'LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "LDS0135", resp)
                            Else
                                ''LOG: Defaults loaded sucessfully... generating request
                                'LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "LDS0140", reqe)

                                Dim DataSetReq As DataSetRequest = GetRelevantDSRequestObject()
                                With DataSetReq
                                    .DataSetInput = dataSetInput
                                    .DocumentVersion = Defaults.DefaultCurrentVersion
                                    With .Settings
                                        .AccountNo1 = Defaults.AccountNo1                           ' account number part 1
                                        .AccountNo2 = Defaults.AccountNo2                           ' account number part 2
                                        .AccountNo3 = Defaults.AccountNo3                           ' account number part 3
                                        .AccountNo4 = Defaults.AccountNo4                           ' account number part 4
                                        .AccountNo5 = Defaults.AccountNo5                           ' account number part 5
                                        .BusinessUnit = businessUnit
                                        .BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("SYSTEM21").ToString
                                        .Cacheing = Defaults.Cacheing()                             ' Cacheing?
                                        .CacheTimeMinutes = pr.CacheTimeMinutes()                   ' Cache Time
                                        .Company = company                                          ' Company
                                        .DatabaseType1 = Defaults.DatabaseType1()                   ' Database type
                                        .DestinationDatabase = Defaults.DestinationDatabase()       ' Destination Database
                                        .FrontEndConnectionString = ConfigurationManager.ConnectionStrings("SqlServer2005").ToString

                                        .WebServiceName = WebServiceName                            ' service instance name
                                        .RetryFailures = Defaults.RetryFailures                     ' Retry Failures on/off
                                        .RetryAttempts = Defaults.RetryAttempts                     ' Retry attempts
                                        .RetryWaitTime = Defaults.RetryWaitTime                     ' Retry wait time
                                        .RetryErrorNumbers = Defaults.RetryErrorNumbers             ' Retry error numbers
                                        ' Set Stored Procedure Group from DB, or if not found, from WebConfig
                                        If Not String.IsNullOrEmpty(pr.WebServiceStoredProcedureGroup) Then
                                            .StoredProcedureGroup = pr.WebServiceStoredProcedureGroup
                                        Else
                                            .StoredProcedureGroup = ConfigurationManager.AppSettings("DefaultStoredProcedureGroup")
                                        End If
                                        .ResponseDirectory = pr.ResponseDirectory
                                        .LogRequests = pr.LogRequests
                                        .TransactionID = TransactionID
                                        .LoginId = loginId
                                        .Partner = company
                                    End With
                                End With

                                errObj = DataSetReq.ValidateDataSet()

                                If errObj.HasError Then
                                    xmlResp = CreateXmlResponseError(xmlResp, errObj, currentDocumentVersion)
                                    ''LOG: invalid DataSet
                                    'LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "LDS0245", reqe)
                                Else
                                    ''LOG: valid DataSet... accessing DB
                                    'LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "LDS0255", reqe)
                                    'finally going to access DB
                                    xmlResp = DataSetReq.AccessDatabase(xmlResp)
                                    If Not xmlResp.Err.HasError Then
                                        'DB accessed successfully
                                        'Apply Outgoing XSLT Style Sheet - to convert standard XML doc to 3rd party schema
                                        With DataSetReq
                                            If pr.ApplyOutgoingStyleSheet Then
                                            Else
                                            End If
                                        End With
                                        'Web Service specific elelements, after response is created
                                        errObj = PostResponseSpecifics(xmlResp)
                                    End If
                                End If

                            End If
                        End If
                        ''LOG: Web Service Complete
                        'LogWriter.WriteDBLog(Defaults.WriteLog, TransactionID, "LDS0300", resp)
                    End If
                End If
            End If
            Return xmlResp.GetXmlResponseAsString()
        End Function
#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Gets the relevant XML response object.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetRelevantXmlResponseObject() As XmlResponse
            Dim xmlResp As XmlResponse = Nothing
            Select Case WebServiceName()
                Case Is = DSUploadTDDataRequest
                    Dim xmlWork As New XmlUploadTDDataResponse
                    xmlResp = CType(xmlWork, XmlResponse)
            End Select
            Return xmlResp
        End Function

        ''' <summary>
        ''' Gets the relevant DS request object.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetRelevantDSRequestObject() As DataSetRequest
            Dim DataSetReq As DataSetRequest = Nothing
            Select Case WebServiceName()
                Case Is = DSUploadTDDataRequest
                    Dim dataSetWork As New DSUploadTDDataRequest
                    DataSetReq = CType(dataSetWork, DSUploadTDDataRequest)
            End Select
            Return DataSetReq
        End Function

        ''' <summary>
        ''' Creates the XML response error.
        ''' </summary>
        ''' <param name="xmlResp">The XML resp.</param>
        ''' <param name="errorObject">The error object.</param>
        ''' <param name="documentVersion">The document version.</param>
        ''' <returns>XmlResponse Object</returns>
        Private Function CreateXmlResponseError(ByVal xmlResp As XmlResponse, ByVal errorObject As ErrorObj, ByVal documentVersion As String) As XmlResponse
            If (errorObject.ErrorStatus.Length <= 0) Then
                errorObject.ErrorStatus = errorObject.ErrorMessage
            End If
            xmlResp.Err = errorObject
            xmlResp.DocumentVersion = documentVersion
            xmlResp.CreateResponse()
            Return xmlResp
        End Function

#End Region

    End Class
End Namespace
