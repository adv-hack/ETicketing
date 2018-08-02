Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net
Imports System.Xml
Imports Talent.eCommerce
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    CheckPayment.aspx. Check eith HSBC or COMMIDEA payment
'                                   gateways can be seen OK
'
'       Date                        17/12/08
'
'       Author                      Ben Ford
'
'       @ CS Group 2007             All rights reserved.
'
'       Error Number Code base      
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'       
'--------------------------------------------------------------------------------------------------
Partial Class PagesPublic_Misc_CheckPayment
    Inherits Base01

    Private _failureReason As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim success As Boolean = True
        _failureReason = String.Empty
        Dim myDefaults As New ECommerceModuleDefaults
        Dim defs As ECommerceModuleDefaults.DefaultValues = myDefaults.GetDefaults()

        '-------------------------------------------
        ' Check which payment gateway is in use.
        ' Only check website if it is HSBC or 
        ' Commidea. Otherwise just count as success
        '-------------------------------------------
        Select Case defs.PaymentGatewayType
            Case Is = "HSBC"
                success = CheckHsbcIsOK(defs.PaymentURL1)
            Case Is = "COMMIDEA"
                success = CheckCommideaIsOK(defs.PaymentURL1)
        End Select

        '---------------------
        ' Write response label
        '---------------------
        If success = True Then
            lblResult.Text = "GATEWAY CHECKED OK"
        Else
            lblResult.Text = "GATEWAY CHECK FAILED - " & _failureReason
        End If

    End Sub

    Private Function CheckHsbcIsOK(ByVal url As String) As Boolean
        Dim strPost As String = System.Web.HttpUtility.UrlEncode("")
        Dim success As Boolean = True
        Dim result As String = String.Empty

        Dim myWriter As StreamWriter = Nothing
        Dim objRequest As HttpWebRequest
        Dim transactionStatus As String = String.Empty
        Dim arrRequest As Byte()
        Dim strmRequest As Stream

        arrRequest = Encoding.UTF8.GetBytes(strPost)
        objRequest = CType(WebRequest.Create(url), HttpWebRequest)
        objRequest.Method = "POST"
        objRequest.ContentLength = arrRequest.Length

        objRequest.ContentType = "text/XML"

        Try
            strmRequest = objRequest.GetRequestStream()
            strmRequest.Write(arrRequest, 0, arrRequest.Length)
            strmRequest.Close()
        Catch ex As Exception
            _failureReason = "Failed to create streamwriter - " & ex.Message
            success = False
        End Try

        Dim objResponse As HttpWebResponse

        If success Then
            '----------------------------------------
            ' Get response and check it's a valid doc
            '----------------------------------------
            Try
                objResponse = CType(objRequest.GetResponse(), HttpWebResponse) '

                Using sr As StreamReader = New StreamReader(objResponse.GetResponseStream())
                    result = sr.ReadToEnd()

                    sr.Close()
                End Using
            Catch ex As Exception
                _failureReason = "Failed to read stream - " & ex.Message
                success = False
            End Try

            '-----------------------------
            ' Check valid XML doc returned
            '-----------------------------
            If success Then
                Dim _xmlResponseDoc As New XmlDocument
                Try
                    _xmlResponseDoc.LoadXml(result)
                Catch ex As Exception
                    _failureReason = "Invalid XML doc returned" & ex.Message
                    success = False
                End Try
            End If
        End If

        Return success
    End Function
    Private Function CheckCommideaIsOK(ByVal url As String) As Boolean
        Dim success As Boolean = True
        Dim result As String = String.Empty
        Try
            '----------------------------------------------------------
            ' Just read contents of the page to make sure we can hit it
            '----------------------------------------------------------
            Dim Request As HttpWebRequest = WebRequest.Create(url)
            Dim Response As HttpWebResponse = Request.GetResponse
            Dim SR As StreamReader
            SR = New StreamReader(Response.GetResponseStream)

            result = SR.ReadToEnd
            SR.Close()
        Catch ex As Exception
            _failureReason = "Failed to create streamwriter - " & ex.Message
            success = False
        End Try
        Return success
    End Function
End Class
