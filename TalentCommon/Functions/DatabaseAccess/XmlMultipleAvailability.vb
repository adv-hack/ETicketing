Imports System.Net
'   Error Code  -   TACXMLMA- (TAC -Talent Common, XMLMA - class name XmlMultipleAvailability)
'   Next Error Code Starting is TACXMLMA-05

''' <summary>
''' This class provides the functionality to check stock availability using web services or any xml mode checking
''' </summary>
Public Class XmlMultipleAvailability
    Inherits XmlAccess

#Region "Properties"
    Private _productsPriceAndAvailability As New DePNA
    ''' <summary>
    ''' Gets or sets the DePNA object
    ''' </summary>
    ''' <value>The dep.</value>
    Public Property DePriceNAvailability() As DePNA
        Get
            Return _productsPriceAndAvailability
        End Get
        Set(ByVal value As DePNA)
            _productsPriceAndAvailability = value
        End Set
    End Property

#End Region

#Region "Public Methods"
    ''' <summary>
    ''' functionality provides the way to call the respective service based on the xmlsettings
    ''' and returns the errorobj and assigns result dataset
    ''' </summary>
    ''' <returns>Talent Error Object</returns>
    Public Function GetMultipleStock() As ErrorObj
        Dim errObj As New ErrorObj

        If Not _productsPriceAndAvailability Is Nothing Then
            'Check is it BEKO SAP STOCK CHECK WS
            If (Me.Settings.XmlSettings.DestinationType.ToUpper().Equals("SAPSTOCKCHECK")) Then
                If (Me.Settings.XmlSettings.XmlVersion.Trim().Equals("1.0")) Then
                    errObj = BEKOSAPWSStockCheck()
                End If
            Else

            End If
        Else
            With errObj
                .ErrorMessage = "DE PNA object contains no data"
                .ErrorNumber = "TACXMLMA-01"
                .HasError = True
            End With
        End If

        Return errObj
    End Function
#End Region

#Region "Private Methods"
    ''' <summary>
    ''' BEKO SAP Webservice which uses BAPI Material Availability service to check the stock availability
    ''' Any error returns errorobj or if found assigns result dataset
    ''' </summary>
    ''' <returns></returns>
    Private Function BEKOSAPWSStockCheck() As ErrorObj

        Dim errObj As New ErrorObj
        Dim moduleName As String = "BEKOSAPWSStockCheck"
        Const PLANT As String = "1111"
        Const UNIT As String = "PC"
        Const CHECK_RULE As String = "A"
        Const WSSUCCESSCODE As String = "S"
        Const LOGTYPE As String = "BekoStockCheckLog"

        'webservice returs are stored in this datatable
        Dim returnData As New DataTable
        With returnData.Columns
            .Add("LineNo", GetType(String))         'PLANT
            .Add("ProductNumber", GetType(String))  'MATERIAL
            .Add("WareHouse", GetType(String))      'STGE_LOC
            .Add("Quantity", GetType(Double))       'AV_QTY_PLT
            .Add("ReturnType", GetType(String))     'RETURN.TYPE
            .Add("Description", GetType(String))    'RETURN.MESSAGE
            .Add("ErrorCode", GetType(String))      'CODE
        End With
        Try
            'unpack _productsPriceAndAvailability
            Dim productNumbers() As String = DePriceNAvailability.AlternateSKU.Split(",")
            Dim storageLocations() As String = DePriceNAvailability.Warehouse.Split(",")
            Dim productSKUs() As String = DePriceNAvailability.SKU.Split(",")

            Dim productNumbersUpperBound As Integer = productNumbers.GetUpperBound(0)
            Dim productNumber As String = String.Empty
            Dim storageLocation As String = String.Empty
            Dim productSKU As String = String.Empty

            'webservice settings and output holders to avoid object creation inside loop
            Dim stockCheckWSResponse As BEKOSAPStockCheck.BAPI_MATERIAL_AVAILABILITYResponse = Nothing
            Dim stockCheckWSReturnValues As BEKOSAPStockCheck.BAPIRETURN = Nothing
            Dim BEKOSAPStockCheckWS As New BEKOSAPStockCheck.ZW61V
            Dim MaterialDetails As New BEKOSAPStockCheck.BAPI_MATERIAL_AVAILABILITY
            With BEKOSAPStockCheckWS
                .Url = Me.Settings.XmlSettings.PostXmlUrl
                .Credentials = New NetworkCredential(Me.Settings.XmlSettings.UserName, _
                                                        Me.Settings.XmlSettings.Password, _
                                                            Me.Settings.XmlSettings.DomainName)
                .SoapVersion = Web.Services.Protocols.SoapProtocolVersion.Default
            End With

            'looping the product items to check stock
            For currentProductItem As Integer = 0 To productNumbersUpperBound
                productNumber = productNumbers(currentProductItem)
                storageLocation = storageLocations(currentProductItem)
                productSKU = productSKUs(currentProductItem)
                Try
                    'accessing webservice
                    MaterialDetails.PLANT = PLANT
                    MaterialDetails.UNIT = UNIT
                    MaterialDetails.MATERIAL = productNumber
                    MaterialDetails.STGE_LOC = storageLocation
                    MaterialDetails.CHECK_RULE = CHECK_RULE
                    Dim WMDVEMaterialDetails(0) As BEKOSAPStockCheck.BAPIWMDVE
                    WMDVEMaterialDetails(0) = New BEKOSAPStockCheck.BAPIWMDVE
                    MaterialDetails.WMDVEX = WMDVEMaterialDetails
                    Dim WMDVSMaterialDetails(0) As BEKOSAPStockCheck.BAPIWMDVS
                    WMDVSMaterialDetails(0) = New BEKOSAPStockCheck.BAPIWMDVS
                    MaterialDetails.WMDVSX = WMDVSMaterialDetails

                    'calling webservice method
                    stockCheckWSResponse = BEKOSAPStockCheckWS.BAPI_MATERIAL_AVAILABILITY(MaterialDetails)
                    stockCheckWSReturnValues = stockCheckWSResponse.RETURN
                Catch ex As Exception
                    With errObj
                        .ErrorMessage = "Failed while accessing BEKO SAP Stock Check Web Service" & " : " & ex.Message
                        .ErrorNumber = "TACXMLMA-03"
                        .HasError = True
                    End With
                    'Logging exceptions in log file
                    Dim inputToWS As String = "AccessedTime:" & DateTime.Now.ToString()
                    inputToWS = inputToWS & ",LoginID:" & Settings.LoginId
                    inputToWS = inputToWS & ",AccountNo1:" & Settings.AccountNo1
                    inputToWS = inputToWS & ",ProductNumber:" & productNumber
                    inputToWS = inputToWS & ",storageLocation:" & storageLocation
                    inputToWS = inputToWS & ",SKU:" & productSKU
                    Dim outputFromWS As String = errObj.ErrorNumber & ":" & errObj.ErrorMessage
                    Settings.Logging.GeneralLog(moduleName, inputToWS, outputFromWS, LOGTYPE)
                End Try
                If Not errObj.HasError Then
                    Try
                        'if the returntype is otherthan S and empty string log it but dont raise error
                        If ((stockCheckWSReturnValues.TYPE.Trim() <> WSSUCCESSCODE) _
                        And (stockCheckWSReturnValues.TYPE.Trim() <> "")) Then
                            Dim inputToWS As String = "AccessedTime:" & DateTime.Now.ToString()
                            inputToWS = inputToWS & ",LoginID:" & Settings.LoginId
                            inputToWS = inputToWS & ",AccountNo1:" & Settings.AccountNo1
                            inputToWS = inputToWS & ",ProductNumber:" & productNumber
                            inputToWS = inputToWS & ",storageLocation:" & storageLocation
                            inputToWS = inputToWS & ",SKU:" & productSKU
                            Dim outputFromWS As String = String.Empty
                            outputFromWS = outputFromWS & "AV_QTY_PLT:" & stockCheckWSResponse.AV_QTY_PLT
                            outputFromWS = outputFromWS & ", TYPE:" & stockCheckWSReturnValues.TYPE
                            outputFromWS = outputFromWS & ", MESSAGE:" & stockCheckWSReturnValues.MESSAGE
                            outputFromWS = outputFromWS & ", CODE:" & stockCheckWSReturnValues.CODE
                            Settings.Logging.GeneralLog(moduleName, inputToWS, outputFromWS, LOGTYPE)
                        End If
                        'get the return and move to returnData
                        Dim dr As DataRow
                        dr = returnData.NewRow()
                        dr("LineNo") = PLANT
                        dr("ProductNumber") = productSKU
                        dr("WareHouse") = storageLocation
                        dr("Quantity") = stockCheckWSResponse.AV_QTY_PLT
                        dr("ReturnType") = stockCheckWSReturnValues.TYPE
                        dr("Description") = stockCheckWSReturnValues.MESSAGE
                        dr("ErrorCode") = stockCheckWSReturnValues.CODE
                        returnData.Rows.Add(dr)
                        dr = Nothing
                    Catch ex As Exception
                        With errObj
                            .ErrorMessage = "Failed while assigning BEKO SAP Stock Check Web Service output to data table" & " : " & ex.Message
                            .ErrorNumber = "TACXMLMA-04"
                            .HasError = True
                        End With
                        Dim inputToWS As String = "AccessedTime:" & DateTime.Now.ToString()
                        inputToWS = inputToWS & ",LoginID:" & Settings.LoginId
                        inputToWS = inputToWS & ",AccountNo1:" & Settings.AccountNo1
                        inputToWS = inputToWS & ",ProductNumber:" & productNumber
                        inputToWS = inputToWS & ",storageLocation:" & storageLocation
                        inputToWS = inputToWS & ",SKU:" & productSKU
                        Dim outputFromWS As String = errObj.ErrorNumber & ":" & errObj.ErrorMessage
                        Settings.Logging.GeneralLog(moduleName, inputToWS, outputFromWS, LOGTYPE)
                    End Try
                End If
                'clears the error object assign the quantity above order quantity
                'as these exceptions are web service access errors
                If errObj.HasError Then
                    Dim dr As DataRow
                    dr = returnData.NewRow()
                    dr("LineNo") = PLANT
                    dr("ProductNumber") = productSKU
                    dr("WareHouse") = storageLocation
                    dr("Quantity") = 0
                    dr("ReturnType") = "E"
                    dr("Description") = errObj.ErrorNumber & ":" & errObj.ErrorMessage
                    dr("ErrorCode") = "UNAVAILABLE"
                    returnData.Rows.Add(dr)
                    dr = Nothing
                    'clears the error object
                    With errObj
                        .ErrorMessage = ""
                        .ErrorNumber = ""
                        .HasError = False
                    End With
                End If
                If errObj.HasError Then
                    Exit For
                End If
            Next

        Catch ex As Exception
            With errObj
                .ErrorMessage = "Failed while unpacking products from DePNA" & " : " & ex.Message
                .ErrorNumber = "TACXMLMA-02"
                .HasError = True
            End With
        End Try

        If Not errObj.HasError Then
            ResultDataSet = New DataSet
            ResultDataSet.Tables.Add(returnData)
        End If

        Return errObj
    End Function

#End Region

End Class
