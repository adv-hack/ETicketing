Imports System.IO
Imports System.Xml

Partial Class PagesAgent_Smartcard_SmartCardWebServiceCall
    Inherits TalentBase01

    Private _versionNumber As String = "1.0"
    Private _loginId As String = "IRIS"
    Private _password As String = "IRIS"
    Private _partner As String = "INTERNET"
    Private _oldNamespace As String = "http://localhost/TradingPortal"
    Private _redListNamespace As String = "http://tempuri.org/RedListSchema.xsd"
    Private _whiteListNamespace As String = "http://tempuri.org/WhiteListSchema.xsd"

    Protected Sub btnProcess_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnProcess.Click
        Dim xmlRedListResponse As String = String.Empty
        Dim xmlWhiteListResponse As String = String.Empty
        If createAndSendXmlRequest(xmlRedListResponse, xmlWhiteListResponse) Then
            saveTheXmlResponse(xmlRedListResponse, xmlWhiteListResponse)
            plhResponse.Visible = False
        Else
            plhResponse.Visible = True
            plhFileSaveOption.Visible = False
        End If
    End Sub

    Private Function createAndSendXmlRequest(ByRef xmlRedListResponse As String, ByRef xmlWhiteListResponse As String) As Boolean
        Dim success As Boolean = True
        Dim xmlDEXRedListRequest As New XmlDocument
        Dim xmlDEXWhiteListRequest As New XmlDocument
        Dim accessControlWebService As New AccessControl.AccessControl
        Dim ndDEXRedListRequest, ndDEXWhiteListRequest As XmlNode
        Dim ndVersion1, ndTransactionHeader1, ndProductCode1 As XmlNode
        Dim ndVersion2, ndTransactionHeader2, ndProductCode2 As XmlNode
        Dim ndSenderID1, ndReceiverID1, ndCountryCode1, ndLoginID1, ndPassword1, ndCompany1, ndTransactionID1 As XmlNode
        Dim ndSenderID2, ndReceiverID2, ndCountryCode2, ndLoginID2, ndPassword2, ndCompany2, ndTransactionID2 As XmlNode

        With xmlDEXRedListRequest
            ndDEXRedListRequest = .CreateElement("DEXRedListRequest")
            ndVersion1 = .CreateElement("Version")
            ndTransactionHeader1 = .CreateElement("TransactionHeader")
            ndSenderID1 = .CreateElement("SenderID")
            ndReceiverID1 = .CreateElement("ReceiverID")
            ndCountryCode1 = .CreateElement("CountryCode")
            ndLoginID1 = .CreateElement("LoginID")
            ndPassword1 = .CreateElement("Password")
            ndCompany1 = .CreateElement("Company")
            ndTransactionID1 = .CreateElement("TransactionID")
            ndProductCode1 = .CreateElement("ProductCode")
        End With
        With xmlDEXWhiteListRequest
            ndDEXWhiteListRequest = .CreateElement("DEXWhiteListRequest")
            ndVersion2 = .CreateElement("Version")
            ndTransactionHeader2 = .CreateElement("TransactionHeader")
            ndSenderID2 = .CreateElement("SenderID")
            ndReceiverID2 = .CreateElement("ReceiverID")
            ndCountryCode2 = .CreateElement("CountryCode")
            ndLoginID2 = .CreateElement("LoginID")
            ndPassword2 = .CreateElement("Password")
            ndCompany2 = .CreateElement("Company")
            ndTransactionID2 = .CreateElement("TransactionID")
            ndProductCode2 = .CreateElement("ProductCode")
        End With

        ndTransactionHeader1.AppendChild(ndSenderID1)
        ndTransactionHeader1.AppendChild(ndReceiverID1)
        ndTransactionHeader1.AppendChild(ndCountryCode1)
        ndTransactionHeader1.AppendChild(ndLoginID1)
        ndTransactionHeader1.AppendChild(ndPassword1)
        ndTransactionHeader1.AppendChild(ndCompany1)
        ndTransactionHeader1.AppendChild(ndTransactionID1)
        ndTransactionHeader2.AppendChild(ndSenderID2)
        ndTransactionHeader2.AppendChild(ndReceiverID2)
        ndTransactionHeader2.AppendChild(ndCountryCode2)
        ndTransactionHeader2.AppendChild(ndLoginID2)
        ndTransactionHeader2.AppendChild(ndPassword2)
        ndTransactionHeader2.AppendChild(ndCompany2)
        ndTransactionHeader2.AppendChild(ndTransactionID2)

        ndDEXRedListRequest.AppendChild(ndVersion1)
        ndDEXRedListRequest.AppendChild(ndTransactionHeader1)
        ndDEXRedListRequest.AppendChild(ndProductCode1)
        ndDEXWhiteListRequest.AppendChild(ndVersion2)
        ndDEXWhiteListRequest.AppendChild(ndTransactionHeader2)
        ndDEXWhiteListRequest.AppendChild(ndProductCode2)
        ndVersion1.InnerText = _versionNumber
        ndVersion2.InnerText = _versionNumber
        ndProductCode1.InnerText = txtProductCode.Text.Trim().ToUpper()
        ndProductCode2.InnerText = txtProductCode.Text.Trim().ToUpper()

        xmlDEXRedListRequest.AppendChild(ndDEXRedListRequest)
        xmlDEXWhiteListRequest.AppendChild(ndDEXWhiteListRequest)

        Try
            xmlRedListResponse = accessControlWebService.DEXRedListRequest(_loginId, _password, _partner, xmlDEXRedListRequest.InnerXml)
            xmlWhiteListResponse = accessControlWebService.DEXWhiteListRequest(_loginId, _password, _partner, xmlDEXWhiteListRequest.InnerXml)
            xmlRedListResponse = xmlRedListResponse.Replace(_oldNamespace, _redListNamespace)
            xmlWhiteListResponse = xmlWhiteListResponse.Replace(_oldNamespace, _whiteListNamespace)
        Catch ex As Exception
            lblResponseMessage.Text = ex.Message & " | Stack Trace: " & ex.StackTrace
            success = False
        End Try

        If success Then
            Dim xmlRedListDoc As New XmlDocument
            Dim xmlWhiteListDoc As New XmlDocument
            xmlRedListDoc.LoadXml(xmlRedListResponse)
            xmlWhiteListDoc.LoadXml(xmlWhiteListResponse)

            Dim redListNameSpaceManager As New XmlNamespaceManager(xmlRedListDoc.NameTable)
            redListNameSpaceManager.AddNamespace("ns", _redListNamespace)
            Dim xmlNode As XmlNode = xmlRedListDoc.SelectSingleNode("//ns:ErrorOccurred", redListNameSpaceManager)
            If xmlNode IsNot Nothing Then
                lblResponseMessage.Text = "Product either not found or could not generate extract lists successfully."
                lblResponseMessage.ToolTip = xmlNode.InnerText
                success = False
            End If

            Dim whiteListNameSpaceManager As New XmlNamespaceManager(xmlWhiteListDoc.NameTable)
            whiteListNameSpaceManager.AddNamespace("ns", _whiteListNamespace)
            xmlNode = xmlWhiteListDoc.SelectSingleNode("//ns:ErrorOccurred", whiteListNameSpaceManager)
            If xmlNode IsNot Nothing Then
                lblResponseMessage.Text = "Product either not found or could not generate extract lists successfully."
                lblResponseMessage.ToolTip = xmlNode.InnerText
                success = False
            End If
        End If

        Return success
    End Function

    Private Sub saveTheXmlResponse(ByVal xmlRedListResponse As String, ByVal xmlWhiteListResponse As String)
        Dim backSlash As String = "\"
        Dim redListFileName As New StringBuilder
        redListFileName.Append(backSlash)
        redListFileName.Append("rl")
        redListFileName.Append(Now.Year.ToString())
        redListFileName.Append(Now.Month.ToString())
        redListFileName.Append(Now.Day.ToString())
        redListFileName.Append(Now.Hour.ToString())
        redListFileName.Append(Now.Minute.ToString())
        redListFileName.Append(".xml")

        Dim whiteListFileName As New StringBuilder
        whiteListFileName.Append(backSlash)
        whiteListFileName.Append("wl")
        whiteListFileName.Append(Now.Year.ToString())
        whiteListFileName.Append(Now.Month.ToString())
        whiteListFileName.Append(Now.Day.ToString())
        whiteListFileName.Append(Now.Hour.ToString())
        whiteListFileName.Append(Now.Minute.ToString())
        whiteListFileName.Append(".xml")

        Using outfile As New StreamWriter(ConfigurationManager.AppSettings("AccessControlResponseLocalDirectory") & redListFileName.ToString())
            outfile.Write(xmlRedListResponse)
            hplRedList.NavigateUrl = ConfigurationManager.AppSettings("AccessControlResponseVirtualDirectory") & redListFileName.ToString()
        End Using
        Using outfile As New StreamWriter(ConfigurationManager.AppSettings("AccessControlResponseLocalDirectory") & whiteListFileName.ToString())
            outfile.Write(xmlWhiteListResponse)
            hplWhiteList.NavigateUrl = ConfigurationManager.AppSettings("AccessControlResponseVirtualDirectory") & whiteListFileName.ToString()
        End Using
        plhFileSaveOption.Visible = True
    End Sub

End Class