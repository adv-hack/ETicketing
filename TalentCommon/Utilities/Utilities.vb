Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports System.Net.Mail
Imports System.Security.Cryptography
Imports System.Security.Cryptography.X509Certificates
Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Xml
Imports CookComputing.XmlRpc
Imports System.Globalization

'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    Methods relating to utilities
'
'       Date                        8th Nov 2006
'
'       Author                      Ben Ford  
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACUTUT- 
'                                   
'--------------------------------------------------------------------------------------------------
'   Modification Summary
'
'   dd/mm/yy    ID      By      Description
'   --------    -----   ---     -----------
'   13/02/07    /001    Ben     Fix time function
'   29/03/07    /002    Ben     Fix Validation on Expiry Date (Month=12 Fails!)
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class Utilities

    Private Const CLASSNAME As String = "Utilities"

    Public Shared Function GetNextTempOrderNumber(ByVal business_unit As String, ByVal partner As String, ByVal SQL_DB_Connection_String As String) As Long
        Dim number As Long
        number = GetECommerceDefaultsNumber(business_unit, partner, "TEMP_ORDER_NUMBER", SQL_DB_Connection_String)
        If number = -1 Then
            number = GetECommerceDefaultsNumber(business_unit, GetAllString, "TEMP_ORDER_NUMBER", SQL_DB_Connection_String)
        End If
        Return number
    End Function

    Public Shared Function GetNextOrderNumber(ByVal business_unit As String, ByVal partner As String, ByVal SQL_DB_Connection_String As String) As Long
        Dim number As Long
        number = GetECommerceDefaultsNumber(business_unit, partner, "ORDER_NUMBER", SQL_DB_Connection_String)
        If number = -1 Then
            number = GetECommerceDefaultsNumber(business_unit, GetAllString, "ORDER_NUMBER", SQL_DB_Connection_String)
        End If
        Return number
    End Function

    Public Shared Function GetNextPartnerNumber(ByVal business_unit As String, ByVal partner As String, ByVal SQL_DB_Connection_String As String) As Long
        Dim number As Long
        number = GetECommerceDefaultsNumber(business_unit, partner, "PARTNER_NUMBER", SQL_DB_Connection_String)
        If number = -1 Then
            number = GetECommerceDefaultsNumber(business_unit, GetAllString, "PARTNER_NUMBER", SQL_DB_Connection_String)
        End If
        Return number
    End Function

    Public Shared Function GetNextUserNumber(ByVal business_unit As String, ByVal partner As String, ByVal SQL_DB_Connection_String As String) As Long
        Dim number As Long
        number = GetECommerceDefaultsNumber(business_unit, partner, "USER_NUMBER", SQL_DB_Connection_String)
        If number = -1 Then
            number = GetECommerceDefaultsNumber(business_unit, GetAllString, "USER_NUMBER", SQL_DB_Connection_String)
        End If
        Return number
    End Function

    Public Shared Function GetNextTradingPortalTicket(ByVal business_unit As String, ByVal partner As String, ByVal SQL_DB_Connection_String As String) As Long
        Dim number As Long
        number = GetECommerceDefaultsNumber(business_unit, partner, "TRADINGPORTALTICKET", SQL_DB_Connection_String)
        If number = -1 Then
            number = GetECommerceDefaultsNumber(business_unit, GetAllString, "TRADINGPORTALTICKET", SQL_DB_Connection_String)
        End If
        Return number
    End Function

    Protected Shared Function GetECommerceDefaultsNumber(ByVal business_unit As String, ByVal partner As String, ByVal fieldName As String, ByVal SQL_DB_Connection_String As String) As Long

        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim val As Long = -1
        Dim cmd As New Data.SqlClient.SqlCommand
        cmd.Connection = New Data.SqlClient.SqlConnection(SQL_DB_Connection_String) '"Data Source=NWAB02;Initial Catalog=TalentEBusinessDB;User ID=sa; password=princess;"

        '' ''Try
        '' ''    cmd.Connection.Open()
        '' ''    Dim trans As System.Data.SqlClient.SqlTransaction = cmd.Connection.BeginTransaction(IsolationLevel.ReadCommitted)
        '' ''    cmd.Transaction = trans

        '' ''    cmd.CommandText = "SELECT * FROM tbl_ecommerce_module_defaults_bu " & _
        '' ''                        " WHERE BUSINESS_UNIT = @business_unit " & _
        '' ''                        " AND PARTNER = @partner " & _
        '' ''                        " AND DEFAULT_NAME = @default_name "

        '' ''    With cmd.Parameters
        '' ''        .Clear()
        '' ''        .Add("@business_unit", SqlDbType.NVarChar).Value = business_unit
        '' ''        .Add("@partner", SqlDbType.NVarChar).Value = partner
        '' ''        .Add("@default_name", SqlDbType.NVarChar).Value = fieldName
        '' ''    End With

        '' ''    Dim dr As Data.SqlClient.SqlDataReader = cmd.ExecuteReader
        '' ''    While dr.Read
        '' ''        val = CType(dr("VALUE"), Long)
        '' ''    End While
        '' ''    dr.Close()

        '' ''    If Not val = -1 Then
        '' ''        cmd.CommandText = " UPDATE tbl_ecommerce_module_defaults_bu " & _
        '' ''                             " SET VALUE = (VALUE + 1) " & _
        '' ''                             " WHERE BUSINESS_UNIT = @business_unit " & _
        '' ''                             " AND PARTNER = @partner " & _
        '' ''                             " AND DEFAULT_NAME = @default_name "
        '' ''        cmd.ExecuteNonQuery()
        '' ''    End If
        '' ''    cmd.Transaction.Commit()
        '' ''Catch ex As Exception
        '' ''    cmd.Transaction.Rollback()
        '' ''Finally
        '' ''    cmd.Connection.Close()
        '' ''End Try



        Try
            cmd.Connection.Open()
        Catch ex As Exception
        End Try

        Try
            If cmd.Connection.State = ConnectionState.Open Then
                cmd.CommandText = " BEGIN TRANSACTION " & _
                                    " " & _
                                    " UPDATE tbl_ecommerce_module_defaults_bu WITH (ROWLOCK) " & _
                                    "   SET VALUE = (VALUE + 1) " & _
                                    "   WHERE BUSINESS_UNIT = @business_unit " & _
                                    "   AND PARTNER = @partner " & _
                                    "   AND DEFAULT_NAME = @default_name " & _
                                    " " & _
                                    " SELECT * FROM tbl_ecommerce_module_defaults_bu " & _
                                    "   WHERE BUSINESS_UNIT = @business_unit " & _
                                    "   AND PARTNER = @partner " & _
                                    "   AND DEFAULT_NAME = @default_name " & _
                                    " " & _
                                  " COMMIT TRANSACTION "

                With cmd.Parameters
                    .Clear()
                    .Add("@business_unit", SqlDbType.NVarChar).Value = business_unit
                    .Add("@partner", SqlDbType.NVarChar).Value = partner
                    .Add("@default_name", SqlDbType.NVarChar).Value = fieldName
                End With


                Dim counter As Integer = 0
                While counter < 10
                    counter += 1
                    Dim dr As Data.SqlClient.SqlDataReader = cmd.ExecuteReader
                    If dr.HasRows Then
                        While dr.Read
                            val = CType(dr("VALUE"), Long)
                        End While
                    End If
                    dr.Close()
                    If val > -1 Then counter = 10
                End While

            End If
        Catch ex As Exception

        End Try

        Try
            If cmd.Connection.State = ConnectionState.Open Then
                cmd.Connection.Close()
            End If
        Catch ex As Exception
        End Try

        Dim logging As New TalentLogging()
        logging.LoadTestLog("Utilities.vb", "GetECommerceDefaultsNumber", timeSpan)

        Return val
    End Function

    Public Shared Function GetAllString() As String
        Dim allString As String = "*ALL"

        Return allString
    End Function

    Public Shared ReadOnly Property ConfigFile(ByVal settingName As String) As String
        Get
            Dim reader As System.Configuration.AppSettingsReader = New System.Configuration.AppSettingsReader
            Dim resultAsObject As Object = reader.GetValue(settingName, GetType(String))
            Return resultAsObject.ToString
        End Get
    End Property

    Private Shared strSMTP As String = ""
    Public Shared Property SMTP() As String
        Get
            Return strSMTP
        End Get
        Set(ByVal value As String)
            strSMTP = value
        End Set
    End Property

    Private Shared _SMTPPortNumber As Integer = 25
    Public Shared Property SMTPPortNumber() As Integer
        Get
            Return _SMTPPortNumber
        End Get
        Set(ByVal value As Integer)
            _SMTPPortNumber = value
        End Set
    End Property

    Public Shared ReadOnly Property SimpleXmlString(ByVal WebService As String, _
                                     ByVal SenderID As String, _
                                     ByVal ReceiverID As String, _
                                     ByVal LoginID As String, _
                                     ByVal Password As String, _
                                     ByVal Company As String, _
                                     ByVal TransactionID As String) As String
        Get
            '-------------------------------------------------------------------------------------
            ''<?xml version="1.0" encoding="utf-8" ?>
            ''<InvoiceRequest>
            ''  <Version>1.0</Version>
            ''  <TransactionHeader>
            ''    <SenderID>123456789</SenderID>
            ''    <ReceiverID>987654321</ReceiverID>
            ''    <CountryCode>UK</CountryCode>
            ''    <LoginID>UK3833HHD </LoginID>
            ''    <Password>Re887Jky52</Password>
            ''    <Company>CSG</Company>
            ''    <TransactionID>54321</TransactionID>
            ''  </TransactionHeader>
            ''</InvoiceRequest>
            '-------------------------------------------------------------------------------------
            Dim strWork As New StringBuilder("<?xml version=""1.0"" encoding=""utf-8"" ?>" & vbCrLf)
            Try
                With strWork
                    .Append("<" & WebService & ">" & vbCrLf)
                    .Append("   <Version>1.0</Version>")
                    .Append("   <TransactionHeader>" & vbCrLf)
                    .Append("       <SenderID>" & SenderID & "</SenderID>" & vbCrLf)
                    .Append("       <ReceiverID>" & ReceiverID & "</ReceiverID>" & vbCrLf)
                    .Append("       <CountryCode>UK</CountryCode>" & vbCrLf)
                    .Append("       <LoginID>" & LoginID & "</LoginID>" & vbCrLf)
                    .Append("       <Password>" & Password & "</Password>" & vbCrLf)
                    .Append("       <Company>" & Company & "</Company>" & vbCrLf)
                    .Append("       <TransactionID>" & TransactionID & "</TransactionID>" & vbCrLf)
                    .Append("   </TransactionHeader>")
                    .Append("</" & WebService & ">" & vbCrLf)
                    Return .ToString
                End With
            Catch ex As Exception
                Throw ex
            End Try
        End Get
    End Property

    Private Shared Function CreateTextAttachment(ByVal value As String, ByVal fileName As String) As Attachment
        '----------------------------------------------------------------------
        '   Convert string contents into a MemoryStream object
        '
        Dim attchAscii As New System.Text.ASCIIEncoding
        Dim atchBytes() As Byte = attchAscii.GetBytes(value)
        Dim ms As New System.IO.MemoryStream(atchBytes)
        '----------------------------------------------------------------------
        '   Embed the stream to attachment object
        '
        Dim att As New Attachment(ms, fileName)
        '----------------------------------------------------------------------
        '   Avoid leaving any stream opened
        '
        ms.Close()
        Return att
    End Function

    Public Shared Function Email_Send(ByVal strFrom As String, _
                                                ByVal strTo() As String, _
                                                ByVal strCC() As String, _
                                                ByVal strBcc() As String, _
                                                ByVal strSubject As String, _
                                                ByVal strMessage As String, _
                                                ByVal strFileList() As String, _
                                                Optional ByVal _IsAttachmentText As Boolean = False, _
                                                Optional ByVal _IsBodyHtml As Boolean = False) As ErrorObj
        '
        Const ModuleName As String = "Email_Send 1 "
        Dim err As New ErrorObj

        ' AG, 09.03.2007 - strSMTP is now set via public property 'Utilities.SMTP'
        'Dim strSMTP As String = ConfigFile("EmailSMTP")

        '---------------------------------------------------------------------------
        '   Accept an array of strTo() and an array of fileList()
        '
        '   _IsAttachmentText means the attached file contents are held within strfile 
        '   and will be put to the email as a puedo disk file by streaming, thus the 
        '   application does not need to write to disk.
        '
        Try
            For Each item As String In strTo
                '-------------------------------------------------------------------
                '   For each To address create a mail message
                '
                If ValidateEmail(item) Then
                    Dim MailMsg As New MailMessage(New MailAddress(strFrom.Trim), New MailAddress(item))
                    With MailMsg
                        .Subject = strSubject.Trim()
                        .Body = StripHTML(strMessage)
                        .BodyEncoding = Encoding.UTF8
                        .SubjectEncoding = Encoding.UTF8
                        .Priority = MailPriority.Normal

                        If _IsBodyHtml Then

                            Dim mimeType As New System.Net.Mime.ContentType("text/html")
                            Dim alternate As AlternateView = AlternateView.CreateAlternateViewFromString(strMessage, mimeType)
                            MailMsg.AlternateViews.Add(alternate)

                        End If


                        '---------------------------------------------------------------
                        If Not strCC Is Nothing Then
                            For Each cc As String In strCC
                                If ValidateEmail(cc) Then _
                                    .CC.Add(New MailAddress(cc))
                            Next
                        End If
                        '---------------------------------------------------------------
                        If Not strBcc Is Nothing Then
                            For Each Bcc As String In strBcc
                                If ValidateEmail(Bcc) Then _
                                    .Bcc.Add(New MailAddress(Bcc))
                            Next
                        End If
                        '---------------------------------------------------------------
                        If Not strFileList Is Nothing Then
                            For Each FileList As String In strFileList
                                If FileList.Length > 0 Then
                                    Select Case _IsAttachmentText
                                        Case Is = True
                                            Dim sFilename As String = Now.ToString("yymmdd-hhmmss.xml")
                                            .Attachments.Add(CreateTextAttachment(FileList, sFilename))
                                        Case Is = False
                                            If File.Exists(FileList) Then
                                                .Attachments.Add(New Attachment(FileList))
                                            End If
                                    End Select
                                End If
                            Next
                        End If
                    End With
                    '--------------------------------------------------------------------------
                    If Not err.HasError Then
                        Dim _smtpServer As String = "127.0.0.1"
                        If strSMTP.Length > 3 Then _
                            _smtpServer = strSMTP
                        Dim c As New SmtpClient(_smtpServer)
                        c.UseDefaultCredentials = True
                        c.Port = _SMTPPortNumber

                        '----------------------------------------------------------------
                        ' Only set if using local host - if delivering across the network
                        ' then leave as default (otherwise it will always use localhost)
                        '----------------------------------------------------------------
                        If _smtpServer = "127.0.0.1" OrElse _smtpServer = "localhost" Then
                            c.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis
                        End If
                        c.Send(MailMsg)
                        MailMsg.Dispose()
                        c.Dispose()
                    End If
                End If
            Next
        Catch ex As Exception
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = ModuleName & " Error "
                .ErrorNumber = "TACUTUT-13c"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                             ByVal strTo() As String, _
                             ByVal strSubject As String, _
                             ByVal strMessage As String, _
                             ByVal strfile As String, _
                             Optional ByVal _IsAttachmentText As Boolean = False, _
                             Optional ByVal _IsBodyHtml As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strCc() As String = Nothing
        Dim strBcc() As String = Nothing
        Dim stringSeparators() As String = {";"}
        Dim strFileResult() As String = strfile.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        err = Email_Send(strFrom, strTo, strCc, strBcc, strSubject, strMessage, strFileResult, _IsAttachmentText, _IsBodyHtml)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                            ByVal strTo As String, _
                            ByVal strSubject As String, _
                            ByVal strMessage As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strCc() As String = Nothing
        Dim strBcc() As String = Nothing
        Dim strfile() As String = Nothing
        Dim stringSeparators() As String = {";"}
        Dim strToResult() As String = strTo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        err = Email_Send(strFrom, strToResult, strCc, strBcc, strSubject, strMessage, strfile, False, False)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                                ByVal strTo() As String, _
                                ByVal strSubject As String, _
                                ByVal strMessage As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strCc() As String = Nothing
        Dim strBcc() As String = Nothing
        Dim strfile() As String = Nothing
        err = Email_Send(strFrom, strTo, strCc, strBcc, strSubject, strMessage, strfile, False, False)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                                ByVal strTo() As String, _
                                ByVal strCc() As String, _
                                ByVal strSubject As String, _
                                ByVal strMessage As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strBcc() As String = Nothing
        Dim strfile() As String = Nothing
        err = Email_Send(strFrom, strTo, strCc, strBcc, strSubject, strMessage, strfile, False, False)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                            ByVal strTo() As String, _
                            ByVal strCc() As String, _
                            ByVal strBCc() As String, _
                            ByVal strSubject As String, _
                            ByVal strMessage As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strfile() As String = Nothing
        err = Email_Send(strFrom, strTo, strCc, strBCc, strSubject, strMessage, strfile, False, False)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                            ByVal strTo As String, _
                            ByVal strCc As String, _
                            ByVal strBCc As String, _
                            ByVal strSubject As String, _
                            ByVal strMessage As String) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        '   Deals with several addresses delimited by semi colon : as1@aa.com; as2@aa.com; as3@aa.com; as4@aa.com;  
        '   and the attachments in ana array
        '
        Dim strfile() As String = Nothing
        Dim stringSeparators() As String = {";"}
        Dim strToResult() As String = strTo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        Dim strCCResult() As String = strCc.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        Dim strbCCResult() As String = strBCc.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        err = Email_Send(strFrom, strToResult, strCCResult, strbCCResult, strSubject, strMessage, strfile, False, False)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                            ByVal strTo As String, _
                            ByVal strSubject As String, _
                            ByVal strMessage As String, _
                            ByVal strFile() As String, _
                            Optional ByVal _IsAttachmentText As Boolean = False, _
                            Optional ByVal _IsBodyHtml As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        Dim strCc() As String = Nothing
        Dim strBcc() As String = Nothing
        Dim stringSeparators() As String = {";"}
        Dim strToResult() As String = strTo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        err = Email_Send(strFrom, strToResult, strCc, strBcc, strSubject, strMessage, strFile, _IsAttachmentText, _IsBodyHtml)
        Return err

    End Function
    Public Shared Function Email_Send(ByVal strFrom As String, _
                            ByVal strTo As String, _
                            ByVal strSubject As String, _
                            ByVal strMessage As String, _
                            ByVal strFile As String, _
                            Optional ByVal _IsAttachmentText As Boolean = False, _
                            Optional ByVal _IsBodyHtml As Boolean = False) As ErrorObj
        Dim err As New ErrorObj
        '----------------------------------------------------------------------------------------
        Dim strCc() As String = Nothing
        Dim strBcc() As String = Nothing
        Dim stringSeparators() As String = {";"}
        Dim strToResult() As String = strTo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        Dim strFileResult() As String = strFile.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        err = Email_Send(strFrom, strToResult, strCc, strBcc, strSubject, strMessage, strFileResult, _IsAttachmentText, _IsBodyHtml)
        Return err

    End Function

    Public Shared Function Split_Email_Send(ByVal strFrom As String, _
                                        ByVal strTo As String, _
                                        ByVal strSubject As String, _
                                        ByVal strXMl As String, _
                                        ByVal strSeparator As String, _
                                        ByVal strFooter As String) As ErrorObj
        Dim err As New ErrorObj
        If strFrom.Length > 6 And strTo.Length > 6 And strXMl.Length > 10 Then
            '--------------------------------------------------------------------------------------
            '   Need to split Xml Document by invoice 
            '
            Dim strBody As String = String.Empty
            '
            '----------------------------------------------------------------------------------------
            '   Returns a String array containing the substrings that are delimited by elements 
            '   of a specified String array. 
            '
            Dim stringSeparators() As String = {strSeparator}
            Dim xmlresult() As String = strXMl.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
            Dim strHeader As String = xmlresult(0) & strSeparator
            Dim iCounter As Integer
            '----------------------------------------------------------------------------------------
            For iCounter = 1 To xmlresult.GetUpperBound(0) - 1
                err = Email_Send(strFrom, _
                                        strTo, _
                                        strSubject, _
                                        "See attached xmlfile", _
                                        strHeader & xmlresult(iCounter) & strFooter, _
                                        True, False)

            Next
            '----------------------------------------------------------------------------------------
            '   Because we come out of array before the last one we still have the last one to send
            '
            err = Email_Send(strFrom, _
                                        strTo, _
                                        strSubject, _
                                        "See attached xmlfile", _
                                        strHeader & xmlresult(xmlresult.GetUpperBound(0)), _
                                        True, False)
            '
        End If
        Return err
    End Function
    Public Shared Function Split_Email_Send(ByVal strFrom As String, _
                                        ByVal strXMl As String, _
                                        ByVal strSeparator As String, _
                                        ByVal strFooter As String) As ErrorObj
        Dim err As New ErrorObj
        If strFrom.Length > 6 And strXMl.Length > 0 Then
            '--------------------------------------------------------------------------------------
            '   Need to split Xml Document by email address, this is
            '   why we have used [OrderBy Email_Address] 
            '
            Dim strBody As String = String.Empty
            '----------------------------------------------------------------------------------------
            '   Returns a String array containing the substrings that are delimited by elements 
            '   of a specified String array. 
            '
            Dim stringSeparators() As String = {strSeparator}
            Dim xmlresult() As String = strXMl.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
            Dim strHeader As String = xmlresult(0).ToString & strSeparator
            Dim iPos3, iPos4, iCounter As Integer
            '----------------------------------------------------------------------------------------

            Const ea1 As String = "<EMAIL_ADDRESS>"                 ' Length = 15
            Const ea2 As String = "</EMAIL_ADDRESS>"                ' Length = 16
            Dim strToHold As String = String.Empty
            Dim strTo As String = String.Empty
            Dim strMessage As String = String.Empty
            '  
            strBody = String.Empty
            '
            For iCounter = 1 To xmlresult.GetUpperBound(0)
                '------------------------------------------------------------------------------------
                '   <ProductAlert>
                '       <PRODUCT_CODE>BH770153</PRODUCT_CODE>
                '       <DESCRIPTION>Brake Hose BH770153</DESCRIPTION>
                '       <COMPANY>CSG</COMPANY>
                '       <EMAIL_ADDRESS>marksmith@domain.com</EMAIL_ADDRESS>
                '       <FIRST_NAME>Mark</FIRST_NAME>
                '       <LAST_NAME>Smith</LAST_NAME>
                '       <QUANTITY>8</QUANTITY>
                '   </ProductAlert>
                '------------------------------------------------------------------------------------
                '   Extract next email address 
                '
                iPos3 = InStr(xmlresult(iCounter), ea1, CompareMethod.Text) + 14            ' <EMAIL_ADDRESS>       Length = 15
                iPos4 = InStr(xmlresult(iCounter), ea2, CompareMethod.Text) - 1             ' </EMAIL_ADDRESS>      Length = 16
                strToHold = xmlresult(iCounter).Substring(iPos3, iPos4 - iPos3)             ' aaaa@aaa.com
                '
                If strToHold <> strTo And strTo.Length > 0 Then
                    strMessage = strHeader & strBody & strFooter
                    err = Utilities.Email_Send(strFrom, _
                                                strTo, _
                                                strSeparator, _
                                                "See attached xmlfile", _
                                                strMessage, _
                                                True, False)
                    strBody = strSeparator & xmlresult(iCounter)
                Else
                    strBody &= strSeparator & xmlresult(iCounter)
                End If
                strTo = strToHold
            Next
            '----------------------------------------------------------------------------------------
            '   Because we come out of array on the last one we still have the last to send
            '
            err = Utilities.Email_Send(strFrom, _
                                        strTo, _
                                        strSeparator, _
                                        "See attached xmlfile", _
                                        strHeader & strBody, _
                                        True, False)
            '
        End If
        Return err
    End Function

    Public Shared Function FixStringLength(ByVal strString As String, ByVal intLength As Int32) As String
        '------------------------------------------------------------------------------------------
        ' Return a string padded to the correct length
        '
        If strString Is Nothing Then
            strString = String.Empty
        End If
        If strString.Length > intLength Then _
            strString = strString.Substring(0, intLength)
        Return strString.PadRight(intLength)
    End Function
    Public Shared Function Quote(ByVal sValue As String) As String
        '----------------------------------------------------------------
        '  Deal with the old O'Rielly problem when writing to a database
        '
        Const a1 As String = "'"
        Const a2 As String = "''"
        If sValue.Length = 0 Then Return a2

        Dim str As New StringBuilder(a1)
        With str
            .Append(sValue.Replace(a1, a2))
            .Append(a1)
            Return .ToString
        End With
    End Function
    Public Shared Function ReadXML(ByVal XMLFIle As String) As String
        '-----------------------------------------------------------------
        '  Read xml file from disk to a string
        '
        If Not File.Exists(XMLFIle) Then
            Return XMLFIle & "does not exist."
        Else
            Using sr As StreamReader = File.OpenText(XMLFIle)
                Dim sWork As String
                Dim InputXML As New StringBuilder(String.Empty)
                sWork = sr.ReadLine()
                With InputXML
                    .Append(sWork)
                    While Not sWork Is Nothing
                        sWork = sr.ReadLine()
                        .Append(sWork)
                    End While
                    sr.Close()
                    Return .ToString
                End With
            End Using
        End If
    End Function

    Public Shared Function CheckCardNumber(ByVal cardNumber As String) As Boolean
        Try
            Dim sum As Long = 0
            Dim character As Char
            Dim i, value As Long
            Dim length As Long
            Dim parity As Long

            length = cardNumber.Length
            parity = length Mod 2

            For i = 0 To length - 1
                character = cardNumber.Chars(i)
                value = Char.GetNumericValue(character)
                If i Mod 2 = parity Then value = value * 2
                If value > 9 Then value = value - 9
                sum = sum + value
            Next
            If sum Mod 10 = 0 Then Return True
        Catch ex As Exception
            Return False
        End Try
        Return False
    End Function

    Public Shared Function ValidateCardNumber(ByVal sValue As String) As Boolean
        '------------------------------------------------------------------------------------------
        '   Luhn algorithm 
        '
        '   Credit cards numbers use check digits to guard against mistakes and to check for 
        '   validity. In the section the credit cards MasterCard, VISA, American Express, 
        '   and Discover will be considered. All of these cards use the same format for 
        '   determining and verifying check digits.
        '
        '   Why have a check digit in a credit card number?:
        '
        '   It can determine if a person keys in a number incorrectly 
        '   If  a credit card is scanned it can determine of the scanner made a mistake 
        '   When dealing with credit cards it is important to realize that some have different 
        '   lengths and have different prefixs.

        '   MasterCard is of length 16 and has a prefix of 51, 52, 53, 54, 55 
        '   VISA is of length 13 or 16 and has a prefix of 4 
        '   American Express is of length 15 and has a prefix of 34 or 37 
        '   Discover is of length 16 and has a prefix of 6011 
        '   All of the above credits use (mod 10) to determine a check digit, and in all 
        '   cases the check digit is the right-most digit in the number.
        '------------------------------------------------------------------------------------------
        Dim Result As Integer = 0
        Dim nParity, nLength, iCounter, nDigit As Long

        Try
            nLength = sValue.Length
            nParity = nLength Mod 2
            For iCounter = 0 To nLength - 1
                nDigit = Val(sValue.Chars(iCounter))
                If iCounter Mod 2 = nParity Then nDigit = nDigit * 2
                If nDigit > 9 Then nDigit = nDigit - 9
                Result = Result + nDigit
            Next
            If Result Mod 10 = 0 Then Return True
        Catch ex As Exception

        End Try
        Return False
    End Function
    Public Shared Function ValidateCVV(ByVal sValue As String) As Boolean
        '------------------------------------------------------------------------------------------
        '   What is a CVV code? 
        '   CVV stands for Credit card Validation (or Verification) Value. 
        '   The CVV is a 3 or 4 digit code embossed or imprinted on the reverse side of 
        '   Visa, MasterCard and Discover cards and on the front of American Express cards. 
        '   This is an extra security measure to ensure that you have access or physical 
        '   possession of the credit card itself in order to use the CVV code. 
        '------------------------------------------------------------------------------------------
        Dim validCVV As Boolean = False
        Try
            Select Case sValue.Trim.Length
                Case Is = 3, 4
                    validCVV = True
            End Select
        Catch ex As Exception
        End Try
        Return validCVV
    End Function
    Public Shared Function ValidateEmail(ByVal sValue As String) As Boolean
        '    Const a1 As String = "@"
        '    Const a2 As String = "."
        '    Const a3 As String = "_"
        '    '
        '    If sValue.Length < 7 Then Return False
        '    If InStr(sValue, a1) = 0 Then Return False ' must have a @ symbol
        '    If InStr(InStr(sValue, a1) + 1, sValue, a1) > 0 Then Return False ' can have only one  @ symbol
        '    If InStr(InStr(sValue, a1) + 1, sValue, a3) > 0 Then Return False ' can have underscrore after @ symbol
        '    If InStr(InStr(sValue, a1) + 1, sValue, a2) = 0 Then Return False ' can have a dot after @ symbol
        '    If InStrRev(sValue, a2) < 3 Then Return False ' must have at least 2 characters after the dot .Com
        '    '------------------------------------------------------------------------------------------
        If sValue.Equals(String.Empty) Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Shared Function ValidateExpiryDate(ByVal sExpiryDate As String) As Boolean
        '------------------------------------------------------------------------------------------
        ' Validate credit card expiry date:
        ' - Non blank
        ' - Month from 01 - 12
        ' - Date > todays date
        '------------------------------------------------------------------------------------------
        Dim validExpireDate As Boolean = False
        Dim ExpireMonth As Int16 = 0
        Dim ExpireYear As Int16 = 0
        '
        Dim todayMonth As Int16 = Now.Month
        Dim todayYear As Int16 = Now.Year
        '------------------------------------------------------------------------------------------
        Try
            Select Case sExpiryDate.Trim.Length
                Case Is = 4
                    ExpireMonth = CInt(sExpiryDate.Substring(0, 2))
                    ExpireYear = CInt(sExpiryDate.Substring(2, 2)) + 2000
                    ' /002 Fix expiry month failure
                    ' If ExpireMonth > 0 And ExpireMonth < 12 Then
                    If ExpireMonth >= 1 And ExpireMonth <= 12 Then
                        If (ExpireYear > todayYear) Then
                            validExpireDate = True
                        ElseIf (ExpireYear = todayYear And ExpireMonth >= todayMonth) Then
                            validExpireDate = True
                        End If
                    End If
            End Select
        Catch ex As Exception
        End Try
        Return validExpireDate
    End Function
    Public Shared Function ValidateStartDate(ByVal sStartDate As String) As Boolean
        '------------------------------------------------------------------------------------------
        ' Validate credit card start date:
        ' - blank
        ' - Month from 01 - 12
        ' - Date < todays date
        '------------------------------------------------------------------------------------------
        Dim validStartDate As Boolean = False
        Dim StartMonth As Int16 = 0
        Dim StartYear As Int16 = 0
        '
        Dim validDate As Boolean = False
        Dim todayMonth As Int16 = Now.Month
        Dim todayYear As Int16 = Now.Year
        '------------------------------------------------------------------------------------------
        Try
            Select Case sStartDate.Trim.Length
                Case Is = 0
                    validStartDate = True
                Case Is = 4
                    StartMonth = CInt(sStartDate.Substring(0, 2))
                    StartYear = CInt(sStartDate.Substring(2, 2)) + 2000
                    If StartMonth > 0 And StartMonth < 12 Then
                        If (StartYear <= todayYear) Then
                            validStartDate = True
                        ElseIf (StartYear = todayYear And StartMonth <= todayMonth) Then
                            validStartDate = True
                        End If
                    End If
            End Select
        Catch ex As Exception
        End Try
        Return validStartDate
    End Function

    Public Shared Function xmlExtract(ByVal xmlString As String, ByVal xmlValue As String) As String
        Dim xr As New XmlTextReader(New StringReader(xmlString))
        xr.MoveToContent()
        While xr.Read()
            Select Case xr.Name
                Case Is = xmlValue
                    Return xr.ReadString()
            End Select
        End While
        Return String.Empty
    End Function
    Public Shared Function XmlWriteFile(ByVal strXMl As String, _
                                     ByVal strPath As String, _
                                     ByVal strSeparator As String, _
                                     ByVal strFooter As String) As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------------------
        '   Need to split Xml Document by invoice 
        '   Returns a String array containing the substrings that are delimited by elements 
        '   of a specified String array. 
        '
        Dim strBody As String = String.Empty
        Dim stringSeparators() As String = {strSeparator}
        Dim xmlresult() As String = strXMl.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries)
        Dim strHeader As String = xmlresult(0).ToString & strSeparator
        Dim iCounter As Integer
        '   WriteToLog("Results: ")
        '   For iCounter = 0 To xmlresult.GetUpperBound(0)
        'WriteToLog(xmlresult(iCounter))
        '  Next
        '   WriteToLog("EndResults")

        Dim iUpperBound As Integer
        '----------------------------------------------------------------------------------------
        Try
            iUpperBound = xmlresult.GetUpperBound(0)
            For iCounter = 1 To iUpperBound - 1
                err = Utilities.XmlWriteFile(strHeader & xmlresult(iCounter) & strFooter, String.Empty, strPath)
            Next
            '----------------------------------------------------------------------------------------
            '   Because we come out of array before the last one we still have the last one to send
            '
            err = Utilities.XmlWriteFile(strHeader & xmlresult(iUpperBound), String.Empty, strPath)
            '
        Catch ex As Exception
            Const strError As String = "Failed to Write Xml File"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACUTUT-19"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Public Shared Function XmlWriteFile(ByVal xmlString As String, _
                                      Optional ByVal strFileName As String = "", _
                                      Optional ByVal strPath As String = "") As ErrorObj
        Dim err As New ErrorObj
        '--------------------------------------------------------------------------
        Try
            Dim strDocPath As String = String.Empty
            '-------------------------------------------------------------------------------
            If strPath.Length > 0 Then
                strDocPath = strPath & "\"
            Else
                strDocPath = "C:\XmlTemp\"
            End If
            strDocPath = strDocPath.Replace("\\", "\").Replace("\\", "\").Replace("\\", "\")
            Dim strDocPathSave As String = strDocPath

            '-------------------------------------------------------------------------------
            '   Check directory exists and write the Xml file out to disk
            '
            If Not Directory.Exists(strDocPath) Then _
                Directory.CreateDirectory(strDocPath)
            '
            If strFileName.Length > 0 Then
                strDocPath &= strFileName
            Else
                strDocPath &= Now.ToString("yyyy-MMM-dd-HH-mm-ss-ffffff") & ".Xml"
            End If

            Dim counter As Integer = 0
            Do While (File.Exists(strDocPath))
                counter += 1
                strDocPath = strDocPath.Insert(strDocPath.LastIndexOf("."), counter.ToString)
                'If strFileName.Length > 0 Then
                '    strDocPath &= strFileName & counter.ToString
                'Else
                '    strDocPath = strDocPathSave & sb.ToString & counter.ToString & ".xml"
                'End If

            Loop
            '    WriteToLog("Writing File " & strDocPath)
            '-------------------------------------------------------------------------------
            '   Write out XML file, but use string methods to save having to convert
            '   the string to XML
            '
            Dim outFile As StreamWriter
            outFile = New System.IO.StreamWriter(strDocPath, False)
            outFile.Write(xmlString)
            outFile.Close()
            '
        Catch ex As Exception
            Const strError As String = "Failed to Write Xml File"
            With err
                .ErrorMessage = ex.Message
                .ErrorStatus = strError
                .ErrorNumber = "TACUTUT-191"
                .HasError = True
            End With
        End Try
        Return err
    End Function
    Public Sub DataSet_To_XmlFile(ByVal aDataSet As Data.DataSet, ByVal filepath As String)
        '----------------------------------------------------------------------
        '   Convert from a Dataset to an XML disk file 
        '
        Dim sWriter As New System.IO.StreamWriter(filepath)
        Dim xmlSerial As New Xml.Serialization.XmlSerializer(aDataSet.GetType())
        xmlSerial.Serialize(sWriter, aDataSet)
        sWriter.Close()
        aDataSet.Clear()

    End Sub
    Public Function XmlFile_To_Dataset(ByVal filepath As String) As Data.DataSet
        '----------------------------------------------------------------------
        '   Convert from an XML disk file to a Dataset
        '
        Dim aDataSet As New Data.DataSet
        Dim mySerializer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(aDataSet.GetType)
        Dim fStream As FileStream = New FileStream(filepath, FileMode.Open)
        aDataSet = CType(mySerializer.Deserialize(fStream), Data.DataSet)
        fStream.Close()
        Return aDataSet

    End Function

    Public Shared Function ISeriesDate(ByVal DB2Date As String) As Date
        '----------------------------------------------------------------------------------
        '   iSeries holds dates as [CYYMMDD] so we need to convert
        '   
        Dim sDate As New StringBuilder
        Dim formattedDate As Date = Nothing
        Const sMonths As String = "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec"
        Dim sMonth() As String = sMonths.Split(",")
        Const s As String = " "
        Try
            If InStr(DB2Date, "/") > 0 Then      ' dd/mm/yyyy hh:mm:ss
                With sDate
                    .Append(DB2Date.Substring(0, 2))
                    .Append(s)
                    .Append(sMonth(DB2Date.Substring(3, 2) - 1))
                    .Append(s)
                    .Append(DB2Date.Substring(6, 4))
                End With
            Else
                ' Check for zero date.
                If DB2Date = "0" Or DB2Date = "0000000" Then
                    '' return date.
                Else
                    DB2Date = PadLeadingZeros(DB2Date, 7)
                    With sDate                      ' cyymmdd
                        .Append(DB2Date.Substring(5, 2))
                        .Append(s)
                        .Append(sMonth(DB2Date.Substring(3, 2) - 1))
                        .Append(s)
                        If DB2Date.Substring(0, 1) = 0 Then
                            .Append("19")
                            .Append(DB2Date.Substring(1, 2))
                        ElseIf DB2Date.Substring(0, 1) = 1 Then
                            .Append("20")
                            .Append(DB2Date.Substring(1, 2))
                        Else
                            .Append(DB2Date.Substring(1, 2))
                        End If
                    End With
                End If
            End If
            formattedDate = Date.Parse(sDate.ToString)
        Catch ex As Exception
            formattedDate = New Date()
        End Try
        Return formattedDate
    End Function

    ''' <summary>
    ''' Format the 8 character iSeries Date YYYYMMDD to Date object
    ''' </summary>
    ''' <param name="DB2Date">iSeries date string</param>
    ''' <returns>Date object</returns>
    ''' <remarks></remarks>
    Public Shared Function ISeries8CharacterDate(ByVal DB2Date As String) As Date
        Dim sDate As New StringBuilder
        Dim formattedDate As Date = Now.Date
        Try
            If DB2Date.Length = 8 Then
                Dim yyyy As String = DB2Date.Substring(0, 4)
                Dim MM As String = DB2Date.Substring(4, 2)
                Dim dd As String = DB2Date.Substring(6, 2)
                formattedDate = New Date(yyyy, MM, dd)
            End If
        Catch ex As Exception
            formattedDate = New Date()
        End Try
        Return formattedDate
    End Function

    Public Shared Function DateToIseriesFormat(ByVal fromDate As Date) As String
        '----------------------------------------------------------------------------------
        '   iSeries holds dates as [CYYMMDD] so we need to convert
        '   
        Dim toDate As New StringBuilder
        Try

            'Century
            If fromDate.Year > 2000 Then
                toDate.Append("1")
            Else
                toDate.Append("0")
            End If

            'Year
            toDate.Append(fromDate.Year.ToString.Substring(2, 2))

            'Month
            toDate.Append(Utilities.PadLeadingZeros(fromDate.Month, 2))

            'Day
            toDate.Append(Utilities.PadLeadingZeros(fromDate.Day, 2))

        Catch ex As Exception
        End Try
        Return toDate.ToString
    End Function

    Public Shared Function TimeToIseriesFormat(ByVal fromTime As DateTime) As String
        '----------------------------------------------------------------------------------
        '   iSeries holds time as [HHMMSS] so we need to convert into iSeries Time format
        '   
        Dim toTime As New StringBuilder
        Try

            'Hour
            toTime.Append(Utilities.PadLeadingZeros(fromTime.Hour, 2))

            'Minute
            toTime.Append(Utilities.PadLeadingZeros(fromTime.Minute, 2))

            'Second
            toTime.Append(Utilities.PadLeadingZeros(fromTime.Second, 2))

        Catch ex As Exception
        End Try
        Return toTime.ToString
    End Function

    Public Shared Function DateToIseries8Format(ByVal fromDate As Date) As String
        '----------------------------------------------------------------------------------
        '   New iSeries holds dates as [YYYYMMDD] so we need to convert date into iSeries
        '   8 charater (new way) string
        Dim toDate As New StringBuilder
        Try
            'Year
            toDate.Append(fromDate.Year)

            'Month
            toDate.Append(Utilities.PadLeadingZeros(fromDate.Month, 2))

            'Day
            toDate.Append(Utilities.PadLeadingZeros(fromDate.Day, 2))

        Catch ex As Exception
        End Try
        Return toDate.ToString
    End Function

    Public Shared Function ISeriesTime(ByVal DB2Time As String) As String
        '----------------------------------------------------------------------------------
        '   iSeries holds dates as [HHMMSS] so we need to convert
        '   
        While DB2Time.Length < 6
            DB2Time = "0" & DB2Time
        End While
        Dim sTime As New StringBuilder
        Const s As String = ":"
        Try
            With sTime
                .Append(DB2Time.Substring(0, 2))
                .Append(s)
                .Append(DB2Time.Substring(2, 2))
                .Append(s)
                .Append(DB2Time.Substring(4, 2))
            End With
        Catch ex As Exception
        End Try
        Return sTime.ToString
    End Function
    Public Shared Function GetLCID() As Integer
        Dim LCID As Integer = 0

        Return LCID
    End Function
    Public Shared Function GetDefaultLanguage() As String
        Dim language As String = "ENG"

        Return language
    End Function
    Public Shared Function CheckForDBNull_BigInt(ByVal obj As Object) As Long
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CType(obj, Long)
    End Function
    Public Shared Function CheckForDBNull_Int(ByVal obj As Object) As Integer
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CInt(obj)
    End Function
    Public Shared Function CheckForDBNull_String(ByVal obj As Object) As String
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return String.Empty Else Return CStr(obj)
    End Function
    Public Shared Function CheckForDBNull_DateTime(ByVal obj As Object) As DateTime
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return New DateTime Else Return CDate(obj)
    End Function
    Public Shared Function CheckForDBNull_Decimal(ByVal obj As Object) As Decimal
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return 0 Else Return CDec(obj)
    End Function
    Public Shared Function CheckForDBNull_Boolean_DefaultFalse(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return False Else Return CBool(obj)
    End Function
    Public Shared Function CheckForDBNull_Boolean_DefaultTrue(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return True Else Return CBool(obj)
    End Function
    Public Shared Function CheckIsDBNull(ByVal obj As Object) As Boolean
        If obj.Equals(DBNull.Value) OrElse obj.Equals(String.Empty) Then Return True Else Return False
    End Function
    Public Shared Function PadLeadingZeros(ByVal strString As String, ByVal intLength As Int32) As String
        '------------------------------------------------------------------------------------------
        ' Return a string padded with leading zeros to the correct length
        '
        If strString Is Nothing Then
            strString = String.Empty
        End If

        If strString.Length > intLength Then _
            strString = strString.Substring(0, intLength)
        Return strString.PadLeft(intLength, "0")
    End Function
    Public Shared Function PadLeadingZerosDec(ByVal dec As Decimal, ByVal intLength As Int32) As String

        Dim str As String = String.Format("{0:0.00}", dec)
        str = str.Replace(".", "")
        Return PadLeadingZeros(str, intLength)
    End Function
    Public Shared Sub WriteToLog(ByVal entry As String)
        ' BF - Put in for debugging
        Try

            Dim strLogFile As New StringBuilder("C:\Temp")
            '
            Const bslash As String = "\"
            Const bslashs As String = "\\"
            Const dash As String = "-"
            Const colon As String = ":"
            Const txt As String = ".txt"
            Dim w As System.IO.StreamWriter
            Try
                With strLogFile

                    .Append(bslash)
                    .Replace(bslashs, bslash)
                    If Not Directory.Exists(.ToString) Then _
                        Directory.CreateDirectory(.ToString)
                    '
                    .Append(DateTime.Now.Year)
                    .Append(dash)
                    .Append(DateTime.Now.Month)
                    .Append(dash)
                    .Append(DateTime.Now.Day)
                    .Append(txt)
                    w = New System.IO.StreamWriter(.ToString, True)
                    With w
                        .AutoFlush = True
                        .WriteLine(DateTime.Now.ToLongTimeString() & "." & DateTime.Now.Millisecond & colon & entry)
                        .Close()
                    End With
                End With
            Catch ex As Exception
                ' throw away the exception otherwise we may get into a loop.
            End Try
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function GetOrderStatus(ByVal purpose As String) As Integer
        Select Case UCase(purpose)
            Case Is = "DELIVERY"
                'Delivery has been created
                Return 10
            Case Is = "SUMMARY"
                'Order summary has been passed
                Return 20
            Case Is = "PAYMENT ATTEMPTED"
                'Payment attempt has been made
                Return 30

                'PAYMENT INTERMEDIATE STARTS HERE
                'Payment attempted and intermediate status received from gateway 
                'like token or any reference number
                'This status indicates still the payment is in process 
                'So website doesn't not know whether it is accepted or rejected
            Case Is = "PAYMENT INTERMEDIATE 1"
                Return 31
            Case Is = "PAYMENT INTERMEDIATE 2"
                Return 32
            Case Is = "PAYMENT INTERMEDIATE 3"
                Return 33
            Case Is = "PAYMENT INTERMEDIATE 4"
                Return 34
            Case Is = "PAYMENT INTERMEDIATE 5"
                Return 35
                'PAYMENT INTERMEDIATE ENDS HERE

            Case Is = "PAYMENT REJECTED"
                'Payment has been rejected
                Return 40
            Case Is = "PAYMENT ACCEPTED"
                'Payment was accepted
                Return 50
            Case Is = "ORDER FAILED"
                'Payment successfully taken, be order failed to be created properly
                Return 60
            Case Is = "ORDER COMPLETE"
                'User has hit confirmation page
                Return 70
            Case Is = "PROCESS ORDER"
                'System about to create backend order
                Return 80
            Case Is = "PROCESS FAILED"
                'Backend order creation failed or if google checkout order sending email confirmation failed
                Return 90
            Case Is = "ORDER PENDING"
                'Order in a completed state, waiting to be actioned
                Return 95
            Case Is = "ORDER PROCESSED"
                'Backend order creation succeeded or if google checkout order send email confirmation success
                Return 100
            Case Is = "NEEDS REVIEW"
                'Backend order creation succeeded but order needs review
                Return 110
        End Select
    End Function
    Public Shared Function GetZeroIfEmpty(ByVal valueToVerify As String) As String
        Dim tempReturnValue As String = String.Empty
        If IsNumeric(valueToVerify) Then
            tempReturnValue = valueToVerify
        Else
            tempReturnValue = "0"
        End If
        Return tempReturnValue
    End Function
    Public Shared Function FormatPrice(ByVal strString As String) As String
        If String.IsNullOrWhiteSpace(strString) Then
            strString = "0.00"
        Else
            Dim d As Decimal
            d = CDec(strString) / 100
            strString = Format(d, "0.00")
        End If
        Return strString
    End Function

    Public Shared Function RoundToValue(ByVal nValue As Object, ByVal nCeiling As Double, Optional ByVal RoundUp As Boolean = True) As Double

        Dim tmp As Integer
        Dim tmpVal
        If Not IsNumeric(nValue) Then Exit Function
        If CDec(nValue) = Decimal.Round(CDec(nValue), 2) Then Return CDbl(nValue)
        nValue = CDbl(nValue)

        'Round up to a whole integer -
        'Any decimal value will force a round to the next integer.
        'i.e. 0.01 = 1 or 0.8 = 1

        tmpVal = ((nValue / nCeiling) + (-0.5 + (RoundUp And 1)))
        tmp = Fix(tmpVal)
        tmpVal = CInt((tmpVal - tmp) * 10 ^ 0)
        nValue = tmp + tmpVal / 10 ^ 0

        'Multiply by ceiling value to set RoundtoValue
        RoundToValue = nValue * nCeiling

    End Function

    Public Shared Function getErrorDescription(ByVal ucr As Talent.Common.UserControlResource, _
                                ByVal languageCode As String, _
                                ByVal ErrorCode As String, _
                                Optional ByVal listHtmlTags As Boolean = False) As String

        Dim s As String
        Dim ContentName As String

        'Retrieve the error code description
        ContentName = "ErrorCode" + ErrorCode
        s = ucr.Content(ContentName, languageCode, True)

        'Return a default message if the above fails
        If s.Trim = "" Then
            ContentName = "DefaultErrorCode"
            s = ucr.Content(ContentName, languageCode, True) + ErrorCode
        End If

        'Attach the html tags for the un-ordered list
        If listHtmlTags = True Then
            s = "<ul><li> " + s + "</ul>"
        End If

        Return s
    End Function
    '--------------------------------------------------------------------------
    ' GenerateAccountNumber - Generate account number according to rules set in 
    '                         tbl_ecommerce_module_defaults
    '--------------------------------------------------------------------------
    Public Shared Function GenerateAccountNumber(ByVal business_unit As String, ByVal partner As String, _
                        ByVal SQL_DB_Connection_String As String, _
                        ByVal accountPrefix As String, ByVal accountLength As String, ByVal nextNumberField As String) As String
        Dim generatedAccountNumber As String = String.Empty
        Dim number As Long = 0
        '-------------------------------------------------------------------
        ' Only retreive number if the prefix doesn't cover the whole account 
        ' (i.e. account is not always the same)
        '-------------------------------------------------------------------
        If accountPrefix.Length >= accountLength Then
            generatedAccountNumber = accountPrefix
            If generatedAccountNumber.Length > accountLength Then
                generatedAccountNumber = generatedAccountNumber.Substring(0, accountLength)
            End If
        Else
            number = GetECommerceDefaultsNumber(business_unit, partner, nextNumberField, SQL_DB_Connection_String)
            If number = -1 Then
                number = GetECommerceDefaultsNumber(business_unit, GetAllString, nextNumberField, SQL_DB_Connection_String)
            End If
            '------------------------------------------
            ' Concatenate with prefix (if there is one)
            '------------------------------------------
            If accountPrefix <> String.Empty Then
                generatedAccountNumber = accountPrefix & _
                        PadLeadingZeros(number.ToString, accountLength - accountPrefix.Length)
            Else
                generatedAccountNumber = PadLeadingZeros(number.ToString, accountLength)
            End If
        End If

        Return generatedAccountNumber
    End Function

    Public Shared Sub TalentCommonLog(ByVal sModule As String, ByVal sKey As String, ByVal sValue As String)
        CreateTalentCommonLogEntry(sModule, sKey, sValue)
    End Sub

    Public Shared Sub TalentCommonLog(ByVal sModule As String, ByVal sKey As String, ByVal ds As DataSet, ByVal err As Talent.Common.ErrorObj)

        Dim sValue As String = String.Empty
        Dim intCount As Integer = 0
        Dim dt As DataTable

        sValue = "Talent.Common Response = "
        ' Number of tables in dataset
        If ds Is Nothing Then
            sValue = sValue.Trim & " ResultDataSet is Nothing!"
        Else
            sValue = sValue.Trim & " ResultDataSet.Table.count=" & ds.Tables.Count
            ' Number of rows per table
            For Each dt In ds.Tables
                sValue = sValue.Trim & ": ResultDataSet.Tables(" & intCount & ").Rows.Count=" & dt.Rows.Count
                intCount = intCount + 1
            Next
        End If

        ' Error object
        If err Is Nothing Then
            sValue = sValue.Trim & ": Err object is Nothing!"
        Else
            sValue = sValue.Trim & ": ErrorNumber=" & err.ErrorNumber & "ErrorMessage=" & err.ErrorMessage & ", ErrorStatus=" & err.ErrorStatus
        End If

        CreateTalentCommonLogEntry(sModule, sKey, sValue)

    End Sub

    Private Shared Sub CreateTalentCommonLogEntry(ByVal sModule As String, ByVal sCode As String, ByVal sValue As String)

        If Utilities.IsCacheActive Then

            Const loggingOnOffCacheKey As String = "TalentCommonLoggingOnOff"
            Const logPathCacheKey As String = "TalentCommonLogPath"

            ' Catch when nothing's are passed
            If sModule = Nothing Then
                sModule = ""
            End If
            If sCode = Nothing Then
                sCode = ""
            End If
            If sValue = Nothing Then
                sValue = ""
            End If

            ' Log entry to logfile
            Dim sLogFile As String = HttpContext.Current.Cache.Item(logPathCacheKey)
            If CType(HttpContext.Current.Cache.Item(loggingOnOffCacheKey), Boolean) And Not sLogFile = Nothing Then

                Try

                    'Construct the log record
                    Dim sRecord As New StringBuilder
                    Dim sQuote As String = """"
                    sRecord.Append(sQuote & Now.ToString & "." & Now.Millisecond & sQuote & "," & sQuote)
                    If Not HttpContext.Current.Session Is Nothing Then
                        sRecord.Append(HttpContext.Current.Session.SessionID & sQuote & ",")
                    Else
                        sRecord.Append(sQuote & ",")
                    End If
                    sRecord.Append(sQuote & sModule.Trim & sQuote & ",")
                    sRecord.Append(sQuote & sCode.Trim & sQuote & ",")
                    sRecord.Append(sQuote & sValue.Trim & sQuote)

                    'Write the record to the log file
                    sLogFile = sLogFile & Now.Year & "-" & Now.Month & "-" & Now.Day & ".log"
                    Dim s As StreamWriter
                    s = File.AppendText(sLogFile)
                    s.WriteLine(sRecord.ToString)
                    s.Close()

                Catch ex As Exception
                End Try

            End If

        End If
    End Sub

    'Private Shared Sub CreateTalentCommonLogEntry(ByVal sModule As String, ByVal sCode As String, ByVal sValue As String)

    '    Const Param1 As String = "@Param1"
    '    Const Param2 As String = "@Param2"
    '    Const Param3 As String = "@Param3"
    '    Const Param4 As String = "@Param4"
    '    Const Param5 As String = "@Param5"

    '    ' Get the Talent.Common logging config
    '    '        GetTalentCommonLoggingConfig()

    '    ' Catch when nothing's are passed
    '    If sModule = Nothing Then
    '        sModule = ""
    '    End If
    '    If sCode = Nothing Then
    '        sCode = ""
    '    End If
    '    If sValue = Nothing Then
    '        sValue = ""
    '    End If

    '    ' Log entry to database
    '    If CType(HttpContext.Current.Cache.Item(loggingOnOffCacheKey), Boolean) Then

    '        Dim conSqlLog2005 As SqlConnection = Nothing
    '        Try

    '            conSqlLog2005 = New SqlConnection(HttpContext.Current.Cache.Item(loggingConnectionStringCacheKey))
    '            conSqlLog2005.Open()

    '            Dim cmdSelect As SqlCommand = Nothing
    '            Const SQLString As String = " INSERT INTO tbl_talentcommon_log(DATETIME, SESSIONID, MODULE, CODE, VALUE) " & _
    '                                                " VALUES ( @Param1, @Param2, @Param3, @Param4, @Param5) "
    '            cmdSelect = New SqlCommand(SQLString, conSqlLog2005)
    '            With cmdSelect
    '                .Parameters.Add(New SqlParameter(Param1, SqlDbType.Char)).Value = DateTime.Now.ToString & "." & Now.Millisecond
    '                ' if called from webservice this will be empty
    '                If Not HttpContext.Current.Session Is Nothing Then
    '                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = HttpContext.Current.Session.SessionID
    '                Else
    '                    .Parameters.Add(New SqlParameter(Param2, SqlDbType.Char)).Value = String.Empty
    '                End If
    '                .Parameters.Add(New SqlParameter(Param3, SqlDbType.Char)).Value = sModule.Trim
    '                .Parameters.Add(New SqlParameter(Param4, SqlDbType.Char)).Value = sCode.Trim
    '                .Parameters.Add(New SqlParameter(Param5, SqlDbType.Char)).Value = sValue.Trim
    '                .ExecuteNonQuery()
    '            End With

    '        Catch ex As Exception
    '        Finally
    '            conSqlLog2005.Close()
    '        End Try

    '    End If
    'End Sub

    'Private Shared Sub GetTalentCommonLoggingConfig()

    '    Dim strTalentCommonLoggingConfig As String = HttpContext.Current.Server.MapPath("") & "\web.config"
    '    Const aaa1 As String = "<add name=""SqlServer2005"" connectionString="""
    '    Const aaa2 As String = """ providerName=""System.Data.SqlClient""/>"
    '    Const bbb1 As String = "<add key=""TalentCommonLoggingOnOff"" value="""
    '    Const bbb2 As String = "/>"

    '    Dim _talentCommonLoggingOnOff As Boolean = False
    '    Dim _talentCommonLoggingConnectionString As String = String.Empty

    '    Dim sString As String = String.Empty
    '    Dim intPos As Integer = 0

    '    ' If the cache item does not exists then, check that config file exists then, 
    '    ' if exists, read contents of config file.
    '    ' Load the cache items.
    '    Try
    '        ' Load logging configuration from flat config file once only and cache the settings.
    '        If HttpContext.Current.Cache.Item(loggingOnOffCacheKey) Is Nothing Then

    '            If File.Exists(strTalentCommonLoggingConfig) Then

    '                Dim sr As StreamReader = File.OpenText(strTalentCommonLoggingConfig)

    '                sString = sr.ReadLine()
    '                While Not sString Is Nothing
    '                    sString = sString.Trim
    '                    If InStr(sString, aaa1) > 0 Then
    '                        intPos = InStr(sString, aaa2)
    '                        If intPos > 0 And intPos > 44 Then
    '                            _talentCommonLoggingConnectionString = sString.Substring(44, intPos - 46)
    '                        End If
    '                    End If
    '                    If InStr(sString, bbb1) > 0 Then
    '                        intPos = InStr(sString, bbb2)
    '                        If intPos > 0 And intPos > 43 Then
    '                            If sString.Substring(43, intPos - 45).ToUpper = "TRUE" Then
    '                                _talentCommonLoggingOnOff = True
    '                            End If
    '                        End If
    '                    End If
    '                    sString = sr.ReadLine()
    '                End While
    '                sr.Close()

    '            End If
    '            HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, _talentCommonLoggingOnOff, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
    '            HttpContext.Current.Cache.Insert(loggingConnectionStringCacheKey, _talentCommonLoggingConnectionString, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
    '        End If
    '    Catch ex As Exception
    '        ' Any problem then logging is off
    '        HttpContext.Current.Cache.Insert(loggingOnOffCacheKey, False, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
    '        HttpContext.Current.Cache.Insert(loggingConnectionStringCacheKey, "", Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
    '    End Try


    'End Sub

    'Private Shared Function SqlLog2005Open() As Boolean

    'Dim iCounter As Int16 = 0

    ' Attempt to open database
    '   Try
    '       While iCounter < 10
    '            If conSqlLog2005 Is Nothing Then
    '                conSqlLog2005 = New SqlConnection(HttpContext.Current.Cache.Item(loggingConnectionStringCacheKey))
    '                conSqlLog2005.Open()
    '            ElseIf conSqlLog2005.State <> ConnectionState.Open Then
    '                conSqlLog2005 = New SqlConnection(HttpContext.Current.Cache.Item(loggingConnectionStringCacheKey))
    '                conSqlLog2005.Open()
    '            End If

    '   Possible the server needs to wake up so 
    '            If conSqlLog2005.State = ConnectionState.Open Then
    '                Exit While
    '            Else
    '                iCounter += 1
    '                System.Threading.Thread.Sleep(2500)
    '            End If

    '        End While

    ' Did it work?
    '        If conSqlLog2005.State = ConnectionState.Open Then
    '            Return True
    '        Else
    '            Return False
    '        End If

    '    Catch ex As Exception
    '        Return False
    '    End Try

    'End Function

    'Private Shared Function SqlLog2005Close() As Boolean

    '        Try
    '           If Not (conSqlLog2005 Is Nothing) Then
    '              If (conSqlLog2005.State = ConnectionState.Open) Then
    '                 conSqlLog2005.Close()
    '            End If
    '        End If

    '   Catch ex As Exception
    '       Return False
    '   End Try

    '    Return True

    'End Function


    Shared FindXmlNode_nodeFound As Boolean = False
    Shared FindXmlNode_nodeFoundCount As Integer = 0

    Public Shared Function FindXmlNode(ByVal myXmlDoc As XmlDocument, ByVal nodeName As String, ByVal nodeCount As Integer) As XmlNode

        FindXmlNode_nodeFound = False
        FindXmlNode_nodeFoundCount = 0

        Return FindMyNode(myXmlDoc.ChildNodes, nodeName, nodeCount)

    End Function


    Protected Shared Function FindMyNode(ByVal myNodes As XmlNodeList, ByVal nodeName As String, ByVal nodeCount As Integer) As XmlNode

        For Each xNode As XmlNode In myNodes
            'If nodeFound Then Exit For
            If xNode.Name = nodeName Then
                FindXmlNode_nodeFoundCount += 1
                If FindXmlNode_nodeFoundCount = nodeCount Then
                    FindXmlNode_nodeFound = True
                    Return xNode
                End If
            Else
                If xNode.ChildNodes.Count > 0 Then
                    Dim myNode As XmlNode = FindMyNode(xNode.ChildNodes, nodeName, nodeCount)
                    If Not myNode Is Nothing Then
                        Return myNode
                    End If
                End If
            End If
        Next

        Return Nothing
    End Function

    Shared FindXmlNodeHasInnerText_nodeHasError As Boolean = False

    Public Shared Function FindXmlNodeHasInnerTextError(ByVal myXmlDoc As XmlDocument, ByVal nodeName As String) As Boolean

        FindXmlNodeHasInnerText_nodeHasError = False

        Return FindMyNodeHasInnerTextError(myXmlDoc.ChildNodes, nodeName)

    End Function


    Protected Shared Function FindMyNodeHasInnerTextError(ByVal myNodes As XmlNodeList, ByVal nodeName As String) As Boolean

        For Each xNode As XmlNode In myNodes
            'If nodeFound Then Exit For
            If xNode.Name = nodeName Then
                If String.IsNullOrEmpty(xNode.InnerText) Then
                    FindXmlNodeHasInnerText_nodeHasError = True
                    Return True
                End If
            Else
                If xNode.ChildNodes.Count > 0 Then
                    If FindMyNodeHasInnerTextError(xNode.ChildNodes, nodeName) Then
                        Return True
                    End If
                End If
            End If
        Next

        Return False
    End Function


    Public Shared Function GetPropertyNames(ByVal obj As Object) As ArrayList
        Dim al As New ArrayList
        Dim inf() As System.Reflection.PropertyInfo = obj.GetType.GetProperties
        For Each info As System.Reflection.PropertyInfo In inf
            al.Add(info.Name)
        Next
        Return al

    End Function
    Public Shared Function PopulateProperties(ByVal propertiesList As ArrayList, ByVal dt As System.Data.DataTable, ByVal objectToPopulate As Object, ByVal rowIndex As Integer) As Object
        If dt.Rows.Count > 0 Then
            For i As Integer = 0 To propertiesList.Count - 1
                If dt.Columns.Contains(propertiesList(i)) Then
                    CallByName(objectToPopulate, propertiesList(i), CallType.Set, CheckDBNull(dt.Rows(rowIndex)(propertiesList(i))))
                Else
                    'If the column does not exist, handle any properties on the class that we know of
                    Select Case propertiesList(i).ToString
                        'Case Is = "IsDirty"
                        '    CallByName(objectToPopulate, propertiesList(i), CallType.Set, False)
                        Case Is = "MODULE_"
                            'Module is a KEYWORD in vb.net so the property name could not be called the same as the DB field name.
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("MODULE")))
                        Case Is = "IS_DEFAULT"
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(dt.Rows(rowIndex)("DEFAULT")))
                        Case Else
                            'Handle all other occurances
                    End Select
                End If
            Next
        End If

        Return objectToPopulate
    End Function
    Public Shared Function PopulateProperties(ByVal propertiesList As ArrayList, ByVal myRow As Data.DataRow, ByVal objectToPopulate As Object) As Object
        For i As Integer = 0 To propertiesList.Count - 1
            If myRow.Table.Columns.Contains(propertiesList(i)) Then
                CallByName(objectToPopulate, propertiesList(i), CallType.Set, CheckDBNull(myRow(propertiesList(i))))
            Else
                'If the column does not exist, handle any properties on the class that we know of
                Select Case propertiesList(i).ToString
                    Case Is = "MODULE_"
                        'Module is a KEYWORD in vb.net so the property name could not be called the same as the DB field name.
                        If myRow.Table.Columns.Contains("MODULE") Then
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_String(myRow("MODULE")))
                        End If
                    Case Is = "IS_DEFAULT"
                        If myRow.Table.Columns.Contains("DEFAULT") Then
                            CallByName(objectToPopulate, propertiesList(i), CallType.Set, Utilities.CheckForDBNull_Boolean_DefaultFalse(myRow("DEFAULT")))
                        End If
                    Case Else
                        'Handle all other occurances
                End Select
            End If
        Next

        Return objectToPopulate
    End Function

    Public Shared Function CheckDBNull(ByVal value As Object) As Object
        If value.Equals(DBNull.Value) Then
            Return Nothing
        Else
            Return value
        End If
    End Function

    Public Shared Function CheckDBNull(ByVal value As Object, ByVal defaultReturnObject As Object) As Object
        If value.Equals(DBNull.Value) Then
            Return defaultReturnObject
        Else
            Return value
        End If
    End Function

    Private callback As New System.Net.Security.RemoteCertificateValidationCallback(AddressOf RemoteCertificateValidationCallback)

    Private Function RemoteCertificateValidationCallback(ByVal sender As Object, ByVal cert As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean

        Return True

    End Function

    <XmlRpcUrl("https://bacsactiveip.accountis.net/remote")> _
    Public Interface AccountisItf
        Inherits IXmlRpcProxy
        <XmlRpcMethod("GetBankName")> _
        Function GetBankName(ByVal sortCode As String, _
                                  ByVal key As String) _
                                  As String
        <XmlRpcMethod("IsAccountNoValid")> _
        Function IsAccountNoValid(ByVal sortCode As String, _
                                  ByVal accountNumber As String, _
                                  ByVal key As String) _
                                  As Integer
    End Interface

    Public Function ValidateBankAccountWithAccountis(ByVal sortCode As String, _
                                                            ByVal accountNumber As String, _
                                                            ByVal accountisKey As String, _
                                                            ByVal urlString As String, _
                                                            ByRef bankName As String) As String
        Dim result As String = "0"


        If String.IsNullOrEmpty(sortCode) Or String.IsNullOrEmpty(accountNumber) Or String.IsNullOrEmpty(accountisKey) Or String.IsNullOrEmpty(urlString) Then
            result = "1"
        Else

            Try

                'Set ip the proxy server
                System.Net.ServicePointManager.ServerCertificateValidationCallback = callback
                Dim proxy As AccountisItf
                proxy = CType(XmlRpcProxyGen.Create(GetType(AccountisItf)), AccountisItf)
                If Not String.IsNullOrEmpty(urlString) Then
                    proxy.Url = urlString
                End If

                'Is the sort code valid
                Dim ret As String
                ret = proxy.GetBankName(sortCode, accountisKey)
                If ret.Equals("NO MATCH") Then
                    result = "1"
                Else
                    If ret.Equals("NO LICENCE") Then
                        result = "3"
                    Else
                        bankName = ret
                        'Is the account code valid
                        Dim ret2 As Integer
                        ret2 = proxy.IsAccountNoValid(sortCode, accountNumber, accountisKey)
                        If ret2.Equals(0) Then
                            result = "2"
                        End If

                    End If
                End If

            Catch ex As Exception
                result = "4"
            End Try
        End If

        Return result
    End Function

    Public Function ValidateBankAccountWithPremiumCredit(ByVal url As String, _
                                                            ByVal userName As String, _
                                                            ByVal password As String, _
                                                            ByVal sortCode As String, _
                                                            ByVal accountNumber As String) As String
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim result As String = "0"

        If String.IsNullOrEmpty(userName) Or _
            String.IsNullOrEmpty(password) Or _
            String.IsNullOrEmpty(sortCode) Or _
            String.IsNullOrEmpty(accountNumber) Then
            result = "1"
        Else

            Try
                'Call WebService
                Dim transferEngine As com.pclplstest.www.XMLTransferEngine = New com.pclplstest.www.XMLTransferEngine()
                transferEngine.Url = url
                Dim response As com.pclplstest.www.TransactionResult = transferEngine.ValidateBankDetails(userName, password, sortCode, accountNumber.PadLeft(8, "0"))
                If response.Success Then
                    'Validation Successfull
                Else
                    'Return Error
                    Dim responseError As com.pclplstest.www.eErrorCode = response.ErrorCode
                    result = CType(responseError, String)
                End If

            Catch ex As Exception
                result = "999"
            End Try
        End If

        Dim logging As New TalentLogging()
        logging.LoadTestLog("Utilities.vb", "ValidateBankAccountWithPremiumCredit", timeSpan)
        Return result
    End Function


    Public Function ValidateBankAccountWithFundTech(ByVal sortCode As String, _
                                                        ByVal accountNumber As String, _
                                                        ByVal fundTechKey As String, _
                                                        ByVal fundTechUrl As String, _
                                                        ByVal methodName As String) As String
        Dim errorCode As String = String.Empty
        Try
            fundTechUrl = fundTechUrl & methodName
            Dim postData As String = "sort_code=" & sortCode & "&account_no=" & accountNumber & "&authentication=" & fundTechKey
            Dim request As WebRequest = WebRequest.Create(fundTechUrl)
            request.Method = "POST"
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(postData)
            request.ContentType = "application/x-www-form-urlencoded"
            request.ContentLength = byteArray.Length
            Dim dataStream As Stream = request.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)
            dataStream.Close()
            Dim response As WebResponse = request.GetResponse()
            dataStream = response.GetResponseStream()
            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()
            '{"data": "", "feedback": "E-10101 Missing required fields", "success": false}
            If responseFromServer.Contains("""success"": true") Then
                errorCode = String.Empty
            Else
                'log the response
                Dim logging As New TalentLogging()
                logging.GeneralLog("ValidateBankAccountWithFundTech", "FUNDTECH-ERR", responseFromServer, "FundTechAccountValidationLog")
                errorCode = "E-ERROR"
                Dim arrOuput() As String = responseFromServer.Split(",")
                For arrIndex As Integer = 0 To arrOuput.Length - 1
                    If arrOuput(arrIndex).Contains("feedback") Then
                        ' "feedback": "E-10101 Missing required fields"
                        Dim errMessage As String = arrOuput(arrIndex).Trim.Replace("""feedback"":", "").Trim
                        '"E-10101 Missing required fields"
                        errorCode = errMessage.Split(" ")(0).Replace("""", "")
                    End If
                Next
            End If
        Catch ex As Exception
            errorCode = "E-EXCEPTION"
            Throw
        End Try
        Return errorCode
    End Function

    ''' <summary>
    ''' Get the card type for the given card number using tbl_creditcard_type_control  ''' 
    ''' </summary>
    ''' <param name="card"></param>
    ''' <param name="dbConnectionString"></param>
    ''' <returns>card type code</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCardType(ByVal card As String, ByVal dbConnectionString As String) As String
        Dim cardType As String = String.Empty

        'Parameters passed in must be valid
        If card.Length > 6 Then
            'Retrieve the card type from the first 6 digits
            Dim len As Integer = 6
            Do While len > 0
                'Try to retrieve the card type from the first x digits
                cardType = RetrieveCardType(card.Substring(0, len), dbConnectionString, 0)
                If Not String.IsNullOrWhiteSpace(cardType) Then
                    Exit Do
                Else
                    len -= 1
                End If
            Loop
            If String.IsNullOrWhiteSpace(cardType) Then cardType = String.Empty
        End If

        Return cardType
    End Function

    ''' <summary>
    ''' Check the selected card string against the card number in tbl_creditcard_type_control
    ''' </summary>
    ''' <param name="card">The card number</param>
    ''' <param name="selectedType">The selected card type</param>
    ''' <param name="dbConnectionString">The local wed db connection string</param>
    ''' <param name="maxInstallments">The max installments number for this card</param>
    ''' <param name="cardType">The actual card type of the given card number</param>
    ''' <returns>True if the card type selected matches what is in the database against the card number</returns>
    ''' <remarks></remarks>
    Public Shared Function CheckCardType(ByVal card As String, ByVal selectedType As String, ByVal dbConnectionString As String, ByRef maxInstallments As Integer, Optional ByRef cardType As String = "") As Boolean
        Dim validType As Boolean = False

        'Parameters passed in must be valid
        If card.Length > 6 AndAlso Not String.IsNullOrEmpty(selectedType) Then

            'Retrieve the card type from the first 6 digits
            Dim len As Integer = 6
            Do While len > 0

                'Try to retrieve the card type from the first x digits
                cardType = RetrieveCardType(card.Substring(0, len), dbConnectionString, maxInstallments)

                'Is the card valid?
                Select Case cardType
                    Case Is = selectedType.Trim
                        validType = True
                        Exit Do
                    Case Is = String.Empty
                        len -= 1
                    Case Else
                        Exit Do
                End Select
            Loop
        End If

        Return validType
    End Function

    Public Shared Function RetrieveCardType(ByVal card As String, _
                                            ByVal dbConnectionString As String, ByRef maxInstallments As Integer) As String

        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay
        maxInstallments = 0
        RetrieveCardType = String.Empty

        Dim cacheKey As String = "RetrieveCardType" & card

        If Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            RetrieveCardType = CType(HttpContext.Current.Cache.Item(cacheKey), String).Substring(0, 50)
            maxInstallments = CInt(CType(HttpContext.Current.Cache.Item(cacheKey), String).Substring(50, 5))
        Else

            Dim selectStr As String = "select * from tbl_creditcard_type_control " & _
                                                " where @card >= card_from_range " & _
                                                " and @card <= card_to_range " & _
                                                " order by card_from_range desc "

            Dim cmd As New SqlCommand(selectStr, New SqlConnection(dbConnectionString))


            Try


                cmd.Connection.Open()

                With cmd.Parameters
                    .Clear()
                    .Add("@card", SqlDbType.NVarChar).Value = card
                End With

                Dim dtr As SqlDataReader = Nothing
                dtr = cmd.ExecuteReader()

                If dtr.HasRows Then
                    dtr.Read()
                    RetrieveCardType = dtr("CARD_CODE").ToString.Trim
                    Try
                        maxInstallments = CInt(dtr("MAX_INSTALLMENTS").ToString)
                    Catch ex As Exception
                    End Try
                    dtr.Close()
                End If

            Catch ex As Exception
            Finally
                cmd.Connection.Close()
                cmd.Dispose()
            End Try

            HttpContext.Current.Cache.Insert(cacheKey, RetrieveCardType.PadRight(50, " ") & maxInstallments.ToString.PadLeft(5, "0"), Nothing, Now.AddMinutes(30), Caching.Cache.NoSlidingExpiration)
            Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
        End If

        Dim logging As New TalentLogging()
        logging.LoadTestLog("Utilities.vb", "RetrieveCardType", timeSpan)
        Return RetrieveCardType.Trim

    End Function

    Public Shared Function aMD5Hash(ByVal value As String) As String

        Dim objMD5 As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim arrData() As Byte
        Dim arrHash() As Byte

        arrData = Text.Encoding.UTF8.GetBytes(value)
        arrHash = objMD5.ComputeHash(arrData)
        objMD5 = Nothing

        Return ByteArrayToString(arrHash)

    End Function

    Private Shared Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim strOutput As New System.Text.StringBuilder(arrInput.Length)
        For i As Integer = 0 To arrInput.Length - 1
            strOutput.Append(arrInput(i).ToString("X2"))
        Next
        Return strOutput.ToString().ToLower
    End Function

    Public Shared Function TripleDESEncode(ByVal value As String, ByVal key As String) As String
        Try
            Dim des As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            des.IV = New Byte(7) {}
            Dim pdb As New System.Security.Cryptography.PasswordDeriveBytes(key, New Byte(-1) {})
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, New Byte(7) {})
            Dim ms As New IO.MemoryStream((value.Length * 2) - 1)
            Dim encStream As New System.Security.Cryptography.CryptoStream(ms, des.CreateEncryptor(), _
                                                                                System.Security.Cryptography.CryptoStreamMode.Write)
            Dim plainBytes As Byte() = Text.Encoding.UTF8.GetBytes(value)
            encStream.Write(plainBytes, 0, plainBytes.Length)
            encStream.FlushFinalBlock()
            Dim encryptedBytes(CInt(ms.Length - 1)) As Byte
            ms.Position = 0
            ms.Read(encryptedBytes, 0, CInt(ms.Length))
            encStream.Close()
            Return Convert.ToBase64String(encryptedBytes)
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Public Shared Function TripleDESDecode(ByVal value As String, ByVal key As String) As String
        Try
            Dim des As New System.Security.Cryptography.TripleDESCryptoServiceProvider
            des.IV = New Byte(7) {}
            Dim pdb As New System.Security.Cryptography.PasswordDeriveBytes(key, New Byte(-1) {})
            des.Key = pdb.CryptDeriveKey("RC2", "MD5", 128, New Byte(7) {})
            Dim encryptedBytes As Byte() = Convert.FromBase64String(value)
            Dim ms As New IO.MemoryStream(value.Length)
            Dim decStream As New System.Security.Cryptography.CryptoStream(ms, des.CreateDecryptor(), _
                                                                                System.Security.Cryptography.CryptoStreamMode.Write)
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length)
            decStream.FlushFinalBlock()
            Dim plainBytes(CInt(ms.Length - 1)) As Byte
            ms.Position = 0
            ms.Read(plainBytes, 0, CInt(ms.Length))
            decStream.Close()
            Return Text.Encoding.UTF8.GetString(plainBytes)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    ''' <summary>
    ''' Encrypts the string based on keyusing SHA1 and Triple DES algorithm
    ''' </summary>
    ''' <remarks>
    ''' first converts original Expression to bytesarry (be careful: use Unicode little-endian encoding)
    ''' gives Byte_Expression
    ''' encrypt the Byte_Expression with SHA1 gives Hashcode
    ''' Fillup Hashcode with padding chars (0): Fillfactor is 8 (fill up to 8Bytes) gives Hashcode8
    ''' now converts the KEY from String to Base64encoded ByteArray gives ByteKEY
    ''' crypted it with DES3
    ''' Ciphermode: ECB (Electronic Code Book) 
    ''' Padding: with Zeros
    ''' DEFAULTINITIALVECTOR: = {0×00, 0×00, 0×00, 0×00, 0×00, 0×00, 0×00, 0×00} 
    ''' Key = ByteKEY (see 4)
    ''' gives DES3ecryptedByteArray
    ''' Convert the DES3ecryptedByteArray with Base64Encryption to String 
    ''' </remarks>
    ''' <param name="valueToEncrypt">The value to encrypt.</param>
    ''' <param name="key">The key.</param>
    ''' <returns>Encrypted string</returns>
    Public Shared Function SHA1TripleDESEncode(ByVal valueToEncrypt As String, ByVal key As String) As String
        ' converts the message to Sixteen-bit Unicode Transformation Format
        ' first paramter = little-endian; 2nd parameter byte order unmarked
        Dim unicode As New UnicodeEncoding(False, False)
        Dim messagebytes As Byte() = unicode.GetBytes(valueToEncrypt)


        ' encrypt to SHA1
        Dim sha As SHA1 = New SHA1CryptoServiceProvider()
        Dim digest As Byte() = sha.ComputeHash(messagebytes)


        ' fill to 8 (needed for the CObyte[] M object)
        Dim [rem] As Integer = digest.Length Mod 8
        If [rem] <> 0 Then
            Dim paddingStr As Byte() = New Byte(digest.Length + 8 - [rem] - 1) {}
            Array.Copy(digest, paddingStr, digest.Length)
            digest = paddingStr
        End If

        ' encrypt
        Dim keyProvider As SymmetricAlgorithm = New TripleDESCryptoServiceProvider()
        keyProvider.Mode = CipherMode.ECB
        keyProvider.Padding = PaddingMode.Zeros


        key = key.Trim()
        Dim keysizes() As KeySizes = keyProvider.LegalKeySizes

        keyProvider.Key = Convert.FromBase64String(key)
        keyProvider.IV = New Byte() {&H0, &H0, &H0, &H0, &H0, &H0, _
        &H0, &H0}

        Dim cryptoTransform As ICryptoTransform = keyProvider.CreateEncryptor()
        'Encrypt the data
        Dim msEncrypt As New MemoryStream()
        Dim csEncrypt As New CryptoStream(msEncrypt, cryptoTransform, CryptoStreamMode.Write)

        'Write all data to the crypto stream and flush it
        csEncrypt.Write(digest, 0, digest.Length)
        csEncrypt.FlushFinalBlock()

        'Get encrypted array of bytes
        Dim encryptedData As Byte() = msEncrypt.ToArray()

        'toString
        Return Convert.ToBase64String(encryptedData)
    End Function
    ''' <summary>
    ''' Decrypts the given value based on the given key.
    ''' </summary>
    ''' <param name="valueToDecrypt">Value to decrypt</param>
    ''' <param name="key">The key.</param>
    ''' <returns>Decrypted value</returns>
    Public Shared Function SHA1TripleDESDecode(ByVal valueToDecrypt As String, ByVal key As String) As String
        Dim msgBase64 As Byte() = Convert.FromBase64String(valueToDecrypt)
        Dim keyBase64 As Byte() = Convert.FromBase64String(key)

        Dim keyProvider As SymmetricAlgorithm = New TripleDESCryptoServiceProvider()
        keyProvider.Mode = CipherMode.ECB
        keyProvider.Padding = PaddingMode.Zeros
        keyProvider.Key = keyBase64
        keyProvider.IV = New Byte() {&H0, &H0, &H0, &H0, &H0, &H0, _
        &H0, &H0}

        Dim cryptoTransform As ICryptoTransform = keyProvider.CreateDecryptor()

        Dim memoryStream As New MemoryStream()
        Dim cryptoStream As New CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write)

        cryptoStream.Write(msgBase64, 0, msgBase64.Length)
        cryptoStream.Close()

        Dim transformedBytes As Byte() = memoryStream.ToArray()
        memoryStream.Close()

        'remove padding
        Dim retVal As Byte() = transformedBytes

        Dim i As Integer = transformedBytes.Length
        While transformedBytes(i - 1) = 0
            i -= 1
        End While
        If (i Mod 2) <> 0 Then
            i += 1
        End If

        If i <> transformedBytes.Length Then
            Dim padless As Byte() = New Byte(i - 1) {}
            Array.Copy(transformedBytes, padless, i)
            retVal = padless
        End If

        Return Encoding.Unicode.GetString(retVal)
    End Function
    Public Shared Function ExternalDecryption(ByVal s As String, ByVal externalKey As String, ByVal encryptionUrl As String) As String
        Try

            ' Is the string encrypted by an external encryption key
            If Not s.Equals("") AndAlso Not externalKey.Equals("") AndAlso Not encryptionUrl.Equals("") Then

                'Is the string in the cache.
                Dim cacheKey As String = "ExternalDecryption-" & s & externalKey & encryptionUrl
                If Not System.Web.HttpContext.Current Is Nothing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    s = CType(HttpContext.Current.Cache.Item(cacheKey), String)
                Else

                    'Retrieve the enryption key
                    Dim enc As New Encryption.Encryption
                    enc.Url = encryptionUrl
                    Dim key As String = enc.ReturnEncryptionKey(externalKey)

                    'Decode the string
                    s = TripleDESDecode(s, key)

                    'Store in the cache if avavilable
                    If Not System.Web.HttpContext.Current Is Nothing Then
                        HttpContext.Current.Cache.Insert(cacheKey, s, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End If

            Return s
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Shared Function ExternalEncryption(ByVal s As String, ByVal externalKey As String, ByVal encryptionUrl As String) As String
        Try

            ' Is the string encrypted by an external encryption key
            If Not s.Equals("") AndAlso Not externalKey.Equals("") AndAlso Not encryptionUrl.Equals("") Then

                'Is the string in the cache.
                Dim cacheKey As String = "ExternalEncryption-" & s & externalKey & encryptionUrl
                If Not System.Web.HttpContext.Current Is Nothing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                    s = CType(HttpContext.Current.Cache.Item(cacheKey), String)
                Else

                    'Retrieve the enryption key
                    Dim enc As New Encryption.Encryption
                    enc.Url = encryptionUrl
                    Dim key As String = enc.ReturnEncryptionKey(externalKey)

                    'Decode the string
                    s = TripleDESEncode(s, key)

                    'Store in the cache if avavilable
                    If Not System.Web.HttpContext.Current Is Nothing Then
                        HttpContext.Current.Cache.Insert(cacheKey, s, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
                        Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                    End If
                End If
            End If

            Return s
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Shared Function RetrieveCommonDefault(ByVal connectionString As String, ByVal defaultName As String) As String
        ' Declare this first! Used for Logging function duration
        Dim timeSpan As TimeSpan = Now.TimeOfDay

        Dim defaultValue As String = ""
        Dim cmd As New Data.SqlClient.SqlCommand

        Try

            'Is it in cache
            Dim cacheKey As String = "RetrieveCommonDefault-" & defaultName
            If Not System.Web.HttpContext.Current Is Nothing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
                defaultValue = CType(HttpContext.Current.Cache.Item(cacheKey), String)
            Else

                'Set up the connection 
                cmd.Connection = New Data.SqlClient.SqlConnection(connectionString)
                cmd.Connection.Open()
                cmd.CommandText = " SELECT * FROM tbl_common_defaults " & _
                                    " WHERE DEFAULT_NAME = @default_name "

                'Add the default parameter
                With cmd.Parameters
                    .Clear()
                    .Add("@default_name", SqlDbType.NVarChar).Value = defaultName
                End With

                'Read the default value
                Dim dr As Data.SqlClient.SqlDataReader = cmd.ExecuteReader
                If dr.HasRows Then
                    dr.Read()
                    defaultValue = dr("VALUE")
                End If

                'Store the default in the cache, if the cache is avavilable
                If Not System.Web.HttpContext.Current Is Nothing Then
                    HttpContext.Current.Cache.Insert(cacheKey, defaultValue, Nothing, System.DateTime.Now.AddMinutes(60), Caching.Cache.NoSlidingExpiration)
                    Talent.Common.TalentThreadSafe.RemoveCacheQueueRecord(cacheKey)
                End If
            End If

        Catch ex As Exception
        Finally
            If Not cmd.Connection Is Nothing AndAlso cmd.Connection.State = ConnectionState.Open Then cmd.Connection.Close()
        End Try

        Dim logging As New TalentLogging()
        logging.LoadTestLog("Utilities.vb", "RetrieveCommonDefault", timeSpan)
        Return defaultValue
    End Function

    Public Shared Function convertToBool(ByVal str As String) As Boolean
        Dim returnVal As Boolean
        If str = "1" OrElse str = "Y" OrElse str.ToLower = "true" Then
            returnVal = True
        Else
            returnVal = False
        End If

        Return returnVal

    End Function

    ''' <summary>
    ''' Convert a given boolean value to Y or N string
    ''' </summary>
    ''' <param name="bool">The boolean value to convert</param>
    ''' <param name="emptyStringWhenFalse">If true, return and empty string when given boolean set to false</param>
    ''' <returns>Y or N string</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertToYN(ByVal bool As Boolean, Optional ByVal emptyStringWhenFalse As Boolean = False) As String
        Dim returnValue As String = "N"
        If emptyStringWhenFalse Then returnValue = String.Empty
        Try
            If bool Then returnValue = "Y"
        Catch ex As Exception
            If emptyStringWhenFalse Then returnValue = String.Empty
        End Try
        Return returnValue
    End Function

    Public Shared Function ConvertStringToDecimal(ByVal value As String) As Decimal
        Dim retval As Decimal
        Dim res As Boolean = Decimal.TryParse(value, retval)

        If res Then
            Return retval
        Else
            Return 0D
        End If


    End Function

    ''' <summary>
    ''' Convert a given boolean value to U or A string. 
    ''' </summary>
    ''' <param name="bool">The boolean value to convert</param>
    ''' <param name="emptyStringWhenFalse">If true, return and empty string when given boolean set to false</param>
    ''' <returns>U or A string. If True then A else U.</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertToAddUpdate(ByVal bool As Boolean, Optional ByVal emptyStringWhenFalse As Boolean = False) As String
        Dim returnValue As String = "U"
        If emptyStringWhenFalse Then returnValue = String.Empty
        Try
            If bool Then returnValue = "A"
        Catch ex As Exception
            If emptyStringWhenFalse Then returnValue = String.Empty
        End Try
        Return returnValue
    End Function

    Public Shared Function convertAddUpdateToBool(ByVal str As String) As Boolean
        Dim returnVal As Boolean
        If str = "1" OrElse str = "A" OrElse str.ToLower = "true" OrElse str = "a" Then
            returnVal = True
        Else
            returnVal = False
        End If

        Return returnVal

    End Function


    ''' <summary>
    ''' Convert a given boolean value to G or E string.  
    ''' </summary>
    ''' <param name="value">The value to convert</param>
    ''' <param name="emptyStringWhenFalse">If true, return and empty string when given boolean set to false</param>
    ''' <returns>G or E string. If True then G else E.</returns>
    ''' <remarks></remarks>
    Public Shared Function ConvertToGiftExperience(ByVal value As String, Optional ByVal emptyStringWhenFalse As Boolean = False) As String
        Dim returnValue As String = "E"
        If emptyStringWhenFalse Then returnValue = String.Empty
        Try
            If value = "Gift" Then returnValue = "G"
        Catch ex As Exception
            If emptyStringWhenFalse Then returnValue = String.Empty
        End Try
        Return returnValue
    End Function

    Public Shared Function convertGETOGiftExperience(ByVal str As String) As String
        Dim returnVal As String
        If str = "G" OrElse str = "g" Then
            returnVal = "Gift"
        Else
            returnVal = "Experience"
        End If

        Return returnVal

    End Function

    Public Shared Function ConvertRedeemModeToString(ByVal RMode As RedeemMode) As String
        Dim returnValue As String = "R"
        If (RMode = RedeemMode.Convert) Then
            returnValue = "C"
        ElseIf (RMode = RedeemMode.External) Then
            returnValue = "E"
        ElseIf (RMode = RedeemMode.Delete) Then
            returnValue = "D"
        End If
        Return returnValue
    End Function

    Public Shared Function ConvertBoolToInt(ByVal boolValue As Boolean) As Integer
        Dim intValue As Integer = 0
        If boolValue Then
            intValue = 1
        End If
        Return intValue
    End Function

    Public Shared Function GetXmlDocAsString(ByVal xmlDoc As XmlDocument) As String
        Dim s As String = ""
        Try
            'Takes an XML Document object and returns its string representation
            Dim sw As StringWriter = New StringWriter
            Dim xw As XmlTextWriter = New XmlTextWriter(sw)
            xmlDoc.WriteTo(xw)
            s = sw.ToString
        Catch ex As Exception
        End Try
        Return s
    End Function

    Public Shared Function IsCacheActive() As Boolean
        IsCacheActive = False
        Try
            If Not System.Web.HttpContext.Current Is Nothing Then
                IsCacheActive = True
            End If
        Catch ex As Exception
        End Try
        Return IsCacheActive
    End Function

    ''' <summary>
    ''' Validates if session is active in this environment
    ''' </summary>
    ''' <returns>Boolean</returns>
    Public Shared Function IsSessionActive() As Boolean
        IsSessionActive = False
        Try
            If System.Web.HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Session IsNot Nothing Then
                IsSessionActive = True
            End If
        Catch ex As Exception
        End Try
        Return IsSessionActive
    End Function

    ''' <summary>
    ''' Converts Hexadecimal String to binary string.
    ''' </summary>
    ''' <param name="hexadecimalString">The hexadecimal string.</param>
    ''' <returns>Binary</returns>
    Public Shared Function HexToBinary(ByVal hexadecimalString As String) As String
        Dim binaryString As String = String.Empty
        Dim byteArray As Byte() = Nothing
        Dim byteCount As Integer = 0
        If hexadecimalString IsNot Nothing AndAlso hexadecimalString.Length > 0 Then
            If hexadecimalString.Length Mod 2 <> 0 Then
                'make sure we have an even number of characters
                hexadecimalString = "0" & hexadecimalString
            End If
            byteCount = hexadecimalString.Length / 2
            byteArray = New Byte(byteCount - 1) {}
            Dim sTemp As String
            For i As Integer = 0 To byteCount - 1
                sTemp = hexadecimalString.Substring(i * 2, 2)
                byteArray(i) = Convert.ToByte(sTemp, 16)
            Next
        End If

        Dim currByte As Byte, mask As Byte
        Dim _bitArray As Boolean()
        _bitArray = New Boolean(byteArray.Length * 8 - 1) {}

        Dim bitIndex As Integer = 0
        For i As Integer = 0 To byteArray.Length - 1
            currByte = byteArray(i)
            mask = &H80
            For maskIndex As Integer = 7 To 0 Step -1
                If (currByte And mask) > 0 Then
                    _bitArray(bitIndex) = True
                Else
                    _bitArray(bitIndex) = False
                End If
                bitIndex += 1
                mask >>= 1
            Next
        Next

        Dim sb As New StringBuilder
        If _bitArray.Length > 0 Then
            For i As Integer = 0 To _bitArray.Length - 1
                If _bitArray(i) = True Then
                    sb.Append("1"c)
                Else
                    sb.Append("0"c)
                End If
            Next
        End If
        binaryString = sb.ToString()
        Return binaryString
    End Function

    Public Shared Function RandomString(ByVal iLength As Integer) As String

        Dim iZero, iNine, iA, iZ, iCount, iRandNum As Integer
        Dim sRandomString As String

        ' we'll need random characters, so a Random object 
        ' should probably be created...
        Dim rRandom As New Random()

        ' convert characters into their integer equivalents (their ASCII values)
        iZero = Asc("0")
        iNine = Asc("9")
        iA = Asc("A")
        iZ = Asc("Z")

        ' initialize our return string for use in the following loop
        sRandomString = ""

        ' now we loop as many times as is necessary to build the string 
        ' length we want
        While (iCount < iLength)
            ' we fetch a random number between our high and low values
            iRandNum = rRandom.Next(iZero, iZ)

            ' here's the cool part: we inspect the value of the random number, 
            ' and if it matches one of the legal values that we've decided upon,  
            ' we convert the number to a character and add it to our string
            If (((iRandNum >= iZero) And (iRandNum <= iNine) _
                 Or (iRandNum >= iA) And (iRandNum <= iZ))) Then
                sRandomString = sRandomString + Chr(iRandNum)
                iCount = iCount + 1
            End If

        End While
        ' finally, our random character string should be built, so we return it
        RandomString = sRandomString

    End Function

    Public Shared Function IsTalentRandomString(ByVal randomStrToValidate As String) As Boolean
        Dim isTalentRandom As Boolean = False
        Try

            Dim iZero, iNine, iA, iZ, iLength, iRandNum As Integer

            ' convert characters into their integer equivalents (their ASCII values)
            iZero = Asc("0")
            iNine = Asc("9")
            iA = Asc("A")
            iZ = Asc("Z")

            iLength = randomStrToValidate.Length

            If iLength > 0 Then
                For iCount As Integer = 0 To iLength - 1
                    iRandNum = Asc(randomStrToValidate.Substring(iCount, 1))
                    If (((iRandNum >= iZero) And (iRandNum <= iNine) _
                     Or (iRandNum >= iA) And (iRandNum <= iZ)
                     )) Then
                        isTalentRandom = True
                    Else
                        isTalentRandom = False
                        Exit For
                    End If
                Next
            End If

        Catch ex As Exception
            isTalentRandom = False
        End Try

        Return isTalentRandom
    End Function

    Private Shared Function StripHTML(ByVal HTML As String) As String
        Dim result As String = ""
        Try

            '
            ' Replace line breaks with space because browsers inserts space
            '
            result = HTML.Replace("\r", " ")
            result = result.Replace("\n", " ")
            '
            ' Remove step-formatting
            '
            result = result.Replace("\t", String.Empty)
            '
            ' Remove repeating spaces because browsers ignore them
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "( )+", " ")
            '
            ' Remove the header (prepare first by clearing attributes)
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*head([^>])*>", "<head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*head( )*>)", "</head>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<head>).*(</head>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' remove all scripts (prepare first by clearing attributes)
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*script([^>])*>", "<script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*script( )*>)", "</script>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            'result = System.Text.RegularExpressions.Regex.Replace(result, "(<script>)([^(<script>\.</script>)])*(</script>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<script>).*(</script>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' remove all styles (prepare first by clearing attributes)
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*style([^>])*>", "<style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<( )*(/)( )*style( )*>)", "</style>", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(<style>).*(</style>)", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' insert tabs in place of <td> tags
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*td([^>])*>", "\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' insert line breaks in place of <BR> and <LI> tags
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*br( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*li( )*>", "\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' insert line paragraphs (double line breaks) in place of <P>, <DIV> and <TR> tags
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*div([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*tr([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "<( )*p([^>])*>", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' Remove remaining tags like <a>, links, images, comments etc - anything that's enclosed inside < >
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "<[^>]*>", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' replace special characters:
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, " ", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&bull;", " * ", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&lsaquo;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&rsaquo;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&trade;", "(tm)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&frasl;", "/", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&lt;", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&gt;", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&copy;", "(c)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "&reg;", "(r)", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' Remove all others. More can be added, see http://hotwired.lycos.com/webmonkey/reference/special_characters/
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "&(.{2,6});", String.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' make line breaking consistent
            '
            result = result.Replace("\n", "\r")
            '
            ' Remove extra line breaks and tabs:
            ' replace more than 2 line breaks with 2 line breaks and more than 4 tabs with 4 tabs.
            ' first remove any whitespaces in between the escaped characters and remove redundant tabs in between line breaks
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\t)", "\t\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\t)( )+(\r)", "\t\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)( )+(\t)", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+(\r)", "\r\r", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' Remove multiple tabs following a line break with just one tab
            '
            result = System.Text.RegularExpressions.Regex.Replace(result, "(\r)(\t)+", "\r\t", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
            '
            ' Initial replacement target string for line breaks & tabs
            '
            Dim breaks As String = "\r\r\r"
            Dim tabs As String = "\t\t\t\t\t"

            For i As Integer = 0 To result.Length - 1
                result = result.Replace(breaks, "\r\r")
                result = result.Replace(tabs, "\t\t\t\t")
                breaks = breaks & "\r"
                tabs = tabs & "\t"
            Next

        Catch ex As Exception

        End Try
        '
        ' That's it.
        '
        Return result

    End Function

    ''' <summary>
    ''' Perform a HTMLEncode style function over specified characters using string.replace
    ''' </summary>
    ''' <param name="value">The value to be encoded</param>
    ''' <returns>The encoded value</returns>
    ''' <remarks></remarks>
    Public Shared Function EncodeSpecialCharacters(ByVal value As String) As String
        Dim formattedValue As String = value
        Try
            If value.Length > 0 Then
                formattedValue = formattedValue.Replace("&", "&amp;")
                formattedValue = formattedValue.Replace("£", "&pound;")
                formattedValue = formattedValue.Replace("<", "&lt;")
                formattedValue = formattedValue.Replace(">", "&gt;")
            End If
        Catch ex As Exception
            formattedValue = value
        End Try
        Return formattedValue
    End Function

    Public Shared Function ConvertASCIIHexValue(ByVal toConvertString As String, ByVal businessUnit As String, ByVal partner As String, ByVal languageCode As String, ByVal settings As DESettings) As String
        Dim TDataObjects As Talent.Common.TalentDataObjects = New Talent.Common.TalentDataObjects()
        TDataObjects.Settings = settings
        Dim result As String = String.Empty
        Dim stringCar As Char
        Dim conversionList As Dictionary(Of String, String) = TDataObjects.AppVariableSettings.TblDefaults.GetHexConversionValues(businessUnit, partner, languageCode, 30)
        For Each convertChar As Char In toConvertString
            'convert convertChar to ASCII Hex
            Dim convertCharASCIIHex As String = Hex(Asc(convertChar))
            'only execute on extended ASCII
            stringCar = convertChar
            For Each item As KeyValuePair(Of String, String) In conversionList
                Dim convertFromASCIIHex As String = item.Key
                Dim convertToASCIIHex As String = item.Value
                If convertCharASCIIHex = convertFromASCIIHex Then
                    stringCar = System.Convert.ToChar(System.Convert.ToUInt32(convertToASCIIHex, 16))
                    Exit For
                End If
            Next
            result = result & stringCar
        Next
        Return result
    End Function

    Public Shared Function IsTicketingFee(ByVal cashbackFeeCode As String, ByVal module1 As String, ByVal product As String, ByVal feeCategory As String) As Boolean
        Dim isFee As Boolean = False
        If UCase(module1) = GlobalConstants.BASKETMODULETICKETING.ToUpper Then
            If feeCategory IsNot Nothing AndAlso feeCategory.Trim.Length > 0 Then
                isFee = True
            ElseIf product.ToUpper = cashbackFeeCode.ToUpper Then
                isFee = True
            End If
        End If
        Return isFee
    End Function

    Public Shared Function IsBookingFeesPercentageBased(ByVal settingsEntity As DESettings, ByVal CardTypeFeeCategory As Dictionary(Of String, String), ByVal FulfilmentFeeCategory As Dictionary(Of String, String)) As Boolean
        Dim TalFees As New TalentFees
        TalFees.Settings = settingsEntity
        Dim TDataObjects As Talent.Common.TalentDataObjects = New Talent.Common.TalentDataObjects()
        TDataObjects.Settings = settingsEntity
        If CardTypeFeeCategory Is Nothing OrElse CardTypeFeeCategory.Count <= 0 Then
            CardTypeFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetCardTypeFeeCategoryList(settingsEntity.BusinessUnit)
        End If
        If FulfilmentFeeCategory Is Nothing OrElse FulfilmentFeeCategory.Count <= 0 Then
            FulfilmentFeeCategory = TDataObjects.FeesSettings.TblFeesRelations.GetFulfilmentFeeCategoryList(settingsEntity.BusinessUnit)
        End If
        TalFees.CardTypeFeeCategory = CardTypeFeeCategory
        TalFees.FulfilmentFeeCategory = FulfilmentFeeCategory
        Return TalFees.IsBookingFeesPercentageBased()
    End Function

    ''' <summary>
    ''' Format the number based on the area code(7-digit prefix field) and the full number
    ''' </summary>
    ''' <param name="telephone">The number to work with</param>
    ''' <param name="formattedTel">The 15-digit second part of the number</param>
    ''' <param name="formattedStd">The 7-digit first part of the number</param>
    ''' <remarks></remarks>
    Public Shared Sub FormatTelephone(ByVal telephone As String, ByRef formattedTel As String, ByRef formattedStd As String)
        Dim spacePos As Integer
        'Has a telephone number been entered
        If telephone.Trim <> "" Then
            'Is there a gap between the std and the tel
            spacePos = telephone.IndexOf(" ")
            If spacePos = -1 Then
                'No blank so we must split the telephone string after position 5
                If telephone.Length >= 5 Then
                    formattedStd = telephone.Substring(0, 5)
                    formattedTel = telephone.Substring(5)
                Else
                    formattedTel = telephone
                End If
            Else
                'Extract the telephone information
                formattedStd = telephone.Substring(0, spacePos)
                formattedTel = telephone.Substring(spacePos + 1)
            End If
        End If
    End Sub

    Public Shared Function RetrievePartPayments(ByVal settingsEntity As DESettings, ByVal basketHeaderID As String) As DataTable
        Dim dtPartPayments As New DataTable
        Dim err As New Talent.Common.ErrorObj
        Dim talPayment As New Talent.Common.TalentPayment
        Dim paymentEntity As New Talent.Common.DEPayments
        talPayment.Settings = settingsEntity
        With paymentEntity
            .SessionId = basketHeaderID
            .CustomerNumber = settingsEntity.LoginId
        End With
        talPayment.De = paymentEntity
        err = talPayment.RetrievePartPayments

        If Not err.HasError AndAlso _
            Not talPayment.ResultDataSet Is Nothing AndAlso _
            talPayment.ResultDataSet.Tables.Count = 2 AndAlso _
            talPayment.ResultDataSet.Tables(0).Rows(0).Item("ErrorOccurred") <> GlobalConstants.ERRORFLAG Then
            dtPartPayments = talPayment.ResultDataSet.Tables("PartPayments")
        End If
        Return dtPartPayments
    End Function

    Public Shared Function ClearCacheDependencyOnAllServers(ByVal BusinessUnit As String, ByVal CacheDependencyName As String, ByVal connectionString As String) As ErrorObj
        Dim methodName As String = "ClearCacheDependencyOnAllServers"
        Dim talDataObjects As New TalentDataObjects
        Dim talAdminDataObjects As New TalentAdminDataObjects
        Dim TalentLogger As New TalentLogging
        Dim err As New ErrorObj
        Dim cacheDepFTP As FTPclient = Nothing
        Dim ftpUser As String = String.Empty
        Dim ftpPassword As String = String.Empty
        Dim remoteFTPPath As String = String.Empty
        Dim hostedServer As String = String.Empty
        Dim fileToUpload As String = String.Empty
        Dim machineName As String = String.Empty
        Dim TEBSettings As New DESettings
        Dim TalAdminSettings As New DESettings
        TEBSettings.BusinessUnit = BusinessUnit
        TEBSettings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE
        TEBSettings.FrontEndConnectionString = connectionString
        talDataObjects.Settings = TEBSettings

        connectionString = talDataObjects.AppVariableSettings.TblDatabaseVersion.TalentAdminDatabaseConnectionString()
        TalAdminSettings.DestinationDatabase = GlobalConstants.TALENTADMINDESTINATIONDATABASE
        TalAdminSettings.BackOfficeConnectionString = connectionString
        talAdminDataObjects.Settings = TalAdminSettings

        If Not String.IsNullOrEmpty(connectionString) Then
            If Not String.IsNullOrWhiteSpace(CacheDependencyName) Then
                Try
                    If Not String.IsNullOrWhiteSpace(System.Environment.MachineName) Then
                        machineName = System.Environment.MachineName.ToUpper()
                    End If
                    Dim cacheDependencyPath As String = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(BusinessUnit, "CACHE_DEPENDENCY_PATH")
                    Dim clientName As String = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(BusinessUnit, "TALENT_ADMIN_CLIENT_NAME")
                    Dim TestOrLive As String = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(BusinessUnit, "ISTESTORLIVE")

                    If cacheDependencyPath.Length > 0 AndAlso clientName.Length > 0 AndAlso TestOrLive.Length > 0 Then
                        If cacheDependencyPath.EndsWith("\") Then
                            fileToUpload = cacheDependencyPath & CacheDependencyName
                        Else
                            fileToUpload = cacheDependencyPath & "\" & CacheDependencyName
                        End If

                        'local server file update
                        If System.IO.File.Exists(fileToUpload) Then
                            System.IO.File.Delete(fileToUpload)
                            System.IO.File.Create(fileToUpload).Dispose()
                        Else
                            System.IO.File.Create(fileToUpload).Dispose()
                        End If

                        'local server file has been created now copy this to other servers
                        If System.IO.File.Exists(fileToUpload) Then
                            Dim dtServerDetails As DataTable = talAdminDataObjects.TalentAdminSettings.GetHostServerDetailsByClientName(clientName, TestOrLive)
                            If dtServerDetails IsNot Nothing AndAlso dtServerDetails.Rows.Count > 0 Then
                                cacheDependencyPath = cacheDependencyPath.TrimEnd("\")
                                cacheDependencyPath = cacheDependencyPath.TrimEnd("/")
                                If cacheDependencyPath.LastIndexOf("\") > 0 Then
                                    cacheDependencyPath = cacheDependencyPath.Substring(cacheDependencyPath.LastIndexOf("\") + 1)
                                End If
                                For rowIndex As Integer = 0 To dtServerDetails.Rows.Count - 1
                                    If Utilities.CheckForDBNull_String(dtServerDetails.Rows(rowIndex)("MACHINE_NAME")).ToUpper() <> machineName Then
                                        cacheDepFTP = Nothing
                                        ftpUser = Utilities.CheckForDBNull_String(dtServerDetails.Rows(rowIndex)("USER_NAME"))
                                        ftpPassword = Utilities.CheckForDBNull_String(dtServerDetails.Rows(rowIndex)("PASSWORD"))
                                        remoteFTPPath = Utilities.CheckForDBNull_String(dtServerDetails.Rows(rowIndex)("REMOTE_FTP_PATH"))
                                        hostedServer = Utilities.CheckForDBNull_String(dtServerDetails.Rows(rowIndex)("IP_ADDRESS"))
                                        cacheDepFTP = New FTPclient(hostedServer, ftpUser, ftpPassword)
                                        cacheDepFTP.CurrentDirectory = "/" & cacheDependencyPath & "/"
                                        If Not cacheDepFTP.Upload(fileToUpload) Then
                                            err.ErrorMessage = "Error in uploading the file"
                                            err.HasError = True
                                            Dim logMessage As String = ftpUser & ";" & ftpPassword & ";" & remoteFTPPath & ";" & hostedServer & ";" & machineName
                                            TalentLogger.Logging(CLASSNAME, methodName, "Error while uploading the cache dependency file: " & logMessage, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit, filter4:=CacheDependencyName)
                                            Exit For
                                        End If
                                    End If
                                Next
                            Else
                                err.ErrorMessage = "No hosted server details found"
                                err.HasError = True
                                TalentLogger.Logging(CLASSNAME, methodName, "Error while uploading the cache dependency file: " & machineName, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit, filter4:=CacheDependencyName)
                            End If
                        Else
                            err.ErrorMessage = "file is missing in the executing server"
                            err.HasError = True
                            TalentLogger.Logging(CLASSNAME, methodName, "Error while uploading the cache dependency file: " & machineName, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit, filter4:=CacheDependencyName)
                        End If
                    Else
                        'Don't do any processing if these values are empty
                        If CacheDependencyName.Length = 0 Then
                            err.ErrorMessage = "'CacheDependencyName' in tbl_ecommerce_module_defaults_bu not set correctly or can't be found. BU=" & BusinessUnit
                            err.HasError = True
                            TalentLogger.Logging(CLASSNAME, methodName, "CacheDependencyName in tbl_ecommerce_module_defaults_bu default setting misconfiguration: " & machineName, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit)
                        End If
                        If clientName.Length = 0 Then
                            err.ErrorMessage = "'clientName' in tbl_ecommerce_module_defaults_bu not set correctly or can't be found. BU=" & BusinessUnit
                            err.HasError = True
                            TalentLogger.Logging(CLASSNAME, methodName, "clientName in tbl_ecommerce_module_defaults_bu default setting misconfiguration: " & machineName, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit)
                        End If
                        If TestOrLive.Length = 0 Then
                            err.ErrorMessage = "'TestOrLive' in tbl_ecommerce_module_defaults_bu not set correctly or can't be found. BU=" & BusinessUnit
                            err.HasError = True
                            TalentLogger.Logging(CLASSNAME, methodName, "TestOrLive in tbl_ecommerce_module_defaults_bu default setting misconfiguration: " & machineName, err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit)
                        End If
                    End If

                Catch exc As System.IO.IOException
                    Dim logMessage As String = ftpUser & ";" & ftpPassword & ";" & remoteFTPPath & ";" & hostedServer
                    err.ErrorMessage = "Error while uploading the cache dependency file: " & logMessage
                    err.HasError = True
                    TalentLogger.Logging(CLASSNAME, methodName, "Error while uploading the cache dependency file: " & logMessage, err, exc, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit, filter4:=CacheDependencyName)
                Catch ex As Exception
                    Dim logMessage As String = ftpUser & ";" & ftpPassword & ";" & remoteFTPPath & ";" & hostedServer
                    err.ErrorMessage = "Error while uploading the cache dependency file: " & logMessage
                    err.HasError = True
                    TalentLogger.Logging(CLASSNAME, methodName, "Error while uploading the cache dependency file: " & logMessage, err, ex, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit, filter4:=CacheDependencyName)
                Finally
                    cacheDepFTP = Nothing
                End Try
            Else
                err.ErrorMessage = "CacheDependencyName is empty"
                err.HasError = True
                TalentLogger.Logging(CLASSNAME, methodName, "CacheDependencyName is empty", err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit)
            End If
        Else
            err.ErrorMessage = "Connection string is empty"
            err.HasError = True
            TalentLogger.Logging(CLASSNAME, methodName, "Connection string is empty", err, LogTypeConstants.TCUCACHEFILEUPLOG, BusinessUnit)
        End If

        Return err
    End Function
    ''' <summary>
    ''' Use to convert column names from an SQL stored procedure to mixed case and converts iseries dates to date fields
    ''' it changes CUSTOMER_NUMBER to CustomerNumber
    ''' Only column bames that are all upper case are changed so should only change those pased back from SQL stored procedure       
    ''' </summary>
    ''' <param name="dataset"></param>
    ''' <remarks></remarks>
    Public Shared Sub ConvertISeriesTables(ByRef dataset As DataSet)
        Dim ti As TextInfo = New CultureInfo("en-GB", False).TextInfo
        Dim iseriesDate As Date = Nothing
        Dim iseriesDateFieldName(50) As String
        Dim newDateFieldName(50) As String
        Dim ix As Integer = 0
        Dim iy As Integer = 0
        For Each Dt As DataTable In dataset.Tables
            ix = 0
            For Each dc As DataColumn In Dt.Columns
                Dim colName As String = dc.ColumnName.Replace("_", " ")
                If colName = colName.ToUpper Then
                    colName = colName.ToLower
                    colName = ti.ToTitleCase(colName.ToString)
                    colName = colName.Replace(" ", "")
                    dc.ColumnName = colName
                End If
                'If an iseries date store name and name of the new field   
                Dim pos As Integer = InStr(colName.ToUpper, "ISERIESDATE")
                If pos > 0 Then
                    Dim newColName As String = dc.ColumnName.Substring(0, pos - 1)
                    ix += 1
                    iseriesDateFieldName(ix) = colName
                    newDateFieldName(ix) = newColName
                End If
            Next ' Next column 
            'If iseries dates found, add a real date field for each, read all records and populate real dates from iseriesdates, finally remove iseriesdates 
            If ix > 0 Then
                For iy = 1 To ix
                    Dt.Columns.Add(newDateFieldName(iy))
                Next
                For Each row As DataRow In Dt.Rows
                    For iy = 1 To ix
                        iseriesDate = Utilities.ISeries8CharacterDate(row.Item(iseriesDateFieldName(iy)) + 19000000)
                        row.Item(newDateFieldName(iy)) = iseriesDate
                    Next
                Next
                For iy = 1 To ix
                    Dt.Columns.Remove(iseriesDateFieldName(iy))
                Next

            End If
        Next ' next table
    End Sub
    ''' <summary>
    ''' Takes a string of comma separated and sorted alphanumerics and returns them in the format
    ''' 1,2,3,4,5A,5,6,7,15,16,18 will become:
    ''' 1-4,5A,5-7,15,16,18
    ''' </summary>
    ''' <param name="seatNums">Sorted string of comma-seperated alphanumerics</param>
    ''' <returns>Formatted string</returns>
    ''' <remarks></remarks>
    Public Shared Function FormatBulkSeats(ByVal seatNums As String)
        Dim formattedSeats As New StringBuilder
        Dim listSeatsBefore As List(Of String) = New List(Of String)
        Dim listSeatsAfter As List(Of String) = New List(Of String)
        Dim formattedSeatList As List(Of String) = New List(Of String)
        Dim seatsArray() As String
        Dim x As Integer = 0

        'No point going through all the logic if seats are unreserved
        If seatNums.Trim <> "Unreserved" Then
            seatsArray = seatNums.Trim.Split(",")
            For Each seatNum As String In seatsArray
                listSeatsBefore.Add(seatNum)
                listSeatsAfter.Add(seatNum)
            Next

            ' Set all consecutive seats to blank so 1,2,3,4 in in listSeatsBef  will become (1 ' '  ' '  4) in listSeatsAfter
            ' Logic is if seat is 1 higher than previous and one below next blank inlistSeatsAfter 
            ' but if any of 3 seats is alpha dont blank   
            For i As Integer = 0 To (listSeatsBefore.Count - 2) Step 1
                If i > 0 Then
                    If Not alphaSeat(listSeatsBefore(i - 1)) AndAlso Not alphaSeat(listSeatsBefore(i)) AndAlso Not alphaSeat(listSeatsBefore(i + 1)) AndAlso CInt(listSeatsBefore(i + 1)) - CInt(listSeatsBefore(i)) = 1 AndAlso CInt(listSeatsBefore(i)) - CInt(listSeatsBefore(i - 1)) = 1 Then
                        listSeatsAfter(i) = String.Empty
                    End If
                End If
            Next

            ' put the '-' between ranges of seats which we know because there are blanks in listSeatsAfter  
            For i As Integer = 0 To (listSeatsAfter.Count - 1) Step 1
                If listSeatsAfter(i) = String.Empty Then
                    If formattedSeatList(formattedSeatList.Count - 1) IsNot Nothing AndAlso formattedSeatList(formattedSeatList.Count - 1) <> "-" Then
                        formattedSeatList.Add("-")
                        x += 1
                    End If
                Else
                    formattedSeatList.Add(listSeatsAfter(i))
                    x += 1
                End If
            Next
            For i As Integer = 0 To (formattedSeatList.Count - 1) Step 1

            Next

            ' Finally put commas in between individual seats
            For x = 0 To formattedSeatList.Count - 1 Step 1
                If x > 0 AndAlso formattedSeats.ToString.Substring(formattedSeats.Length - 1, 1) <> "-" AndAlso formattedSeatList(x) <> "-" Then
                    formattedSeats.Append(", ")
                End If
                formattedSeats.Append(formattedSeatList(x))
            Next
        Else
            formattedSeats.Append(seatNums)
        End If
        Return formattedSeats.ToString()
    End Function
    Public Shared Function alphaSeat(ByVal seat As String) As Boolean
        Dim helperNum As Integer
        If Not Int32.TryParse(seat, helperNum) Then
            Return True
        Else
            Return False
        End If

    End Function


    ''' <summary>
    ''' Create a randomised Querystring substitution.
    ''' </summary>
    ''' <param name="length">The legnth of the randomized querystring.</param>
    ''' <returns>A alphanumeric string.</returns>
    Public Shared Function QueryStringRandomize(length As Integer) As String
        Dim obfuscatedString As String = Talent.Common.Utilities.RandomString(length)
        Return (GlobalConstants.ENCRYPTED_QUERYSTRING_PARAMETER_NAME + obfuscatedString)
    End Function

    ''' <summary>
    ''' Encrypts any string using the Rijndael algorithm.
    ''' </summary>
    ''' <param name="business_unit">The business_unit.</param>
    ''' <param name="inputText">The string to encrypt.</param>
    ''' <returns>A Base64 encrypted string.</returns>
    Public Shared Function QueryStringEncrypt(settings As DESettings, business_unit As String, inputText As String) As String
        Dim rijndaelCipher As New RijndaelManaged()
        Dim plainText As Byte() = Encoding.Unicode.GetBytes(inputText)

        
        Dim talDataObjects = New TalentDataObjects()
        talDataObjects.Settings = settings
        talDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE

        Dim clientSalt As String = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(business_unit, "CLEINT_SALT")
        Dim SecretKey As New PasswordDeriveBytes(clientSalt, Encoding.ASCII.GetBytes(clientSalt.Length.ToString()))

        Using encryptor As ICryptoTransform = rijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16))
            Using memoryStream As New MemoryStream()
                Using cryptoStream As New CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
                    cryptoStream.Write(plainText, 0, plainText.Length)
                    cryptoStream.FlushFinalBlock()
                    Return (GlobalConstants.ENCRYPTED_QUERYSTRING_PARAMETER_NAME) + Convert.ToBase64String(memoryStream.ToArray())
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Decrypts a previously encrypted string.
    ''' </summary>
    ''' <param name="business_unit">The business_unit.</param>
    ''' <param name="inputText">The encrypted string to decrypt.</param>
    ''' <returns>A decrypted string.</returns>
    Public Shared Function QueryStringDecrypt(settings As DESettings, business_unit As String, inputText As String) As String
        Dim rijndaelCipher As New RijndaelManaged()
        Dim encryptedData As Byte() = Convert.FromBase64String(inputText)

        Dim talDataObjects = New TalentDataObjects()
        talDataObjects.Settings = settings
        talDataObjects.Settings.DestinationDatabase = GlobalConstants.SQL2005DESTINATIONDATABASE

        Dim clientSalt As String = talDataObjects.AppVariableSettings.TblEcommerceModuleDefaultsBu.GetValueByDefaultNameAndBU(business_unit, "CLEINT_SALT")
        Dim SecretKey As New PasswordDeriveBytes(clientSalt, Encoding.ASCII.GetBytes(clientSalt.Length.ToString()))

        Using decryptor As ICryptoTransform = rijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16))
            Using memoryStream As New MemoryStream(encryptedData)
                Using cryptoStream As New CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)
                    Dim plainText As Byte() = New Byte(encryptedData.Length - 1) {}
                    Dim decryptedCount As Integer = cryptoStream.Read(plainText, 0, plainText.Length)
                    Return Encoding.Unicode.GetString(plainText, 0, decryptedCount)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Parses the current URL and extracts the virtual path without query string.
    ''' </summary>
    ''' <returns>The virtual path of the current URL.</returns>
    Public Shared Function GetURLVirtualPath(Optional url As String = "") As String
        If url = String.Empty Then
            url = HttpContext.Current.Request.RawUrl
        End If

        url = url.Substring(0, url.IndexOf("?"))
        url = url.Substring(url.LastIndexOf("/") + 1)
        Return url
    End Function

    ''' <summary>
    ''' Parses a URL and returns the query string.
    ''' </summary>
    ''' <param name="url">The URL to parse.</param>
    ''' <returns>The query string without the question mark.</returns>
    Public Shared Function ExtractURLQuery(url As String) As String
        Dim index As Integer = url.IndexOf("?") + 1
        Return url.Substring(index)
    End Function

End Class
