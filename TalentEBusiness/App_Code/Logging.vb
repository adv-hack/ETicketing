Imports Microsoft.VisualBasic

Namespace Talent.eCommerce
    Public Class Logging
        Public Shared Sub WriteLog(ByVal loginID As String, _
                                        ByVal errorNumber As String, _
                                        ByVal errorMsg As String, _
                                        ByVal errorstatus As String, _
                                        Optional ByVal business_unit As String = "", _
                                        Optional ByVal partner As String = "", _
                                        Optional ByVal pagename As String = "", _
                                        Optional ByVal controlcode As String = "")

            Dim errObj As New Talent.Common.ErrorObj
            errObj.ErrorNumber = errorNumber
            errObj.ErrorStatus = errorstatus
            errObj.ErrorMessage = errorMsg

            WriteLog(loginID, errObj, business_unit, partner, pagename, controlcode)
        End Sub

        Public Shared Sub WriteLog(ByVal loginID As String, ByVal errObj As Talent.Common.ErrorObj, Optional ByVal business_unit As String = "", Optional ByVal partner As String = "", Optional ByVal pagename As String = "", Optional ByVal controlcode As String = "")
            'Test for null values
            '------------------------------------
            If String.IsNullOrEmpty(business_unit) Then
                Try
                    business_unit = TalentCache.GetBusinessUnit
                Catch ex As Exception

                End Try
            End If
            If String.IsNullOrEmpty(partner) Then
                Try
                    partner = TalentCache.GetPartner(HttpContext.Current.Profile)
                Catch
                End Try
            End If
            If String.IsNullOrEmpty(errObj.ErrorNumber) Then errObj.ErrorNumber = String.Empty
            If String.IsNullOrEmpty(errObj.ErrorMessage) Then errObj.ErrorMessage = String.Empty
            If String.IsNullOrEmpty(errObj.ErrorStatus) Then errObj.ErrorStatus = String.Empty
            '-----------------------------------
            WriteDBLog(loginID, errObj, business_unit, partner, pagename, controlcode)
        End Sub
        Private Shared Sub WriteDBLog(ByVal loginID As String, ByVal errObj As Talent.Common.ErrorObj, ByVal business_unit As String, ByVal partner As String, ByVal pagename As String, ByVal controlcode As String)
            Try
                If (New ECommerceModuleDefaults).GetDefaults.EcommerceLoggingInUse Then
                    Dim Logs As New LoggingDataSetTableAdapters.tbl_ecommerce_error_logTableAdapter
                    Logs.AddNewLogEntry(business_unit, _
                                        partner, _
                                        loginID, _
                                        errObj.ErrorNumber, _
                                        errObj.ErrorMessage, _
                                        errObj.ErrorStatus, _
                                        pagename, _
                                        controlcode, _
                                        Now)
                End If
            Catch ex As Exception
                WriteTextLog(loginID, errObj, business_unit, partner, pagename, controlcode)
            End Try
        End Sub


        Private Shared Sub WriteTextLog(ByVal loginID As String, ByVal errObj As Talent.Common.ErrorObj, ByVal business_unit As String, ByVal partner As String, ByVal pagename As String, ByVal controlcode As String)
            Const dash As String = "-", pipe As String = "|"
            Dim filename As String = Now.Year & dash & Now.Month & dash & Now.Day & ".txt"
            Dim path As String = ConfigurationManager.AppSettings("LogPath").ToString
            If Not path.EndsWith("\") Then path += "\"
            path += filename
            Try
                Dim writer As New System.IO.StreamWriter(path, True)
                writer.AutoFlush = True
                writer.WriteLine(Now.ToString & pipe & _
                                    business_unit & pipe & _
                                    partner & pipe & _
                                    loginID & pipe & _
                                    errObj.ErrorNumber & pipe & _
                                    errObj.ErrorMessage & pipe & _
                                    errObj.ErrorStatus & pipe & _
                                    pagename & pipe & _
                                    controlcode)
                writer.Close()
                writer.Dispose()
            Catch ex As Exception

            End Try
        End Sub

    End Class
End Namespace
