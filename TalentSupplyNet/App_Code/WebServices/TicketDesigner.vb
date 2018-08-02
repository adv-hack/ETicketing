Imports System.Data
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal

''' <summary>
''' Web service to ticket designer data
''' </summary>
<WebService(Namespace:="http://localhost/TradingPortal")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class TicketDesigner
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Uploads the ticket designer data.
    ''' </summary>
    ''' <param name="loginId">The login id.</param>
    ''' <param name="password">The password.</param>
    ''' <param name="company">The company.</param>
    ''' <param name="dataSetInput">The data set input.</param>
    ''' <returns></returns>
    <WebMethod()> _
    Public Function UploadData(ByVal loginId As String, _
                                      ByVal password As String, _
                                      ByVal company As String, _
                                      ByVal dataSetInput As DataSet) As String

        Dim csgWS As New TalentWebService
        Const wsRequest As String = "DSUploadTDDataRequest"
        Const wsResponse As String = "XmlUploadTDDataResponse"
        csgWS.WebServiceName = wsRequest
        csgWS.ResponseName = wsResponse

        'invoke web service and get response
        Dim responseString As String = csgWS.InvokeWebService(loginId, password, company, dataSetInput)

        csgWS = Nothing
        Return responseString
    End Function
End Class
