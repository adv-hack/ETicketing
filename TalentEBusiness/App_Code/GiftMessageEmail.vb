Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Net.Mail

Namespace Talent.eCommerce

    Public Class GiftMessageEmail
        Private __FromAddress As String = String.Empty
        Private __ToAddress As String = String.Empty
        Private __Subject As String = String.Empty
        Private __Body As String = String.Empty
        Private __WebOrderNumber As String = String.Empty
        Private __TempOrderNumber As String = String.Empty

        Public Property FromAddress() As String
            Get
                Return __FromAddress
            End Get
            Set(ByVal value As String)
                __FromAddress = value
            End Set
        End Property

        Public Property ToAddress() As String
            Get
                Return __ToAddress
            End Get
            Set(ByVal value As String)
                __ToAddress = value
            End Set
        End Property

        Public Property Subject() As String
            Get
                Return __Subject
            End Get
            Set(ByVal value As String)
                __Subject = value
            End Set
        End Property

        Public Property Body() As String
            Get
                Return __Body
            End Get
            Set(ByVal value As String)
                __Body = value
            End Set
        End Property

        Public Property WebOrderID() As String
            Get
                Return __WebOrderNumber
            End Get
            Set(ByVal value As String)
                __WebOrderNumber = value
            End Set
        End Property

        Public Property TempOrderID() As String
            Get
                Return __TempOrderNumber
            End Get
            Set(ByVal value As String)
                __TempOrderNumber = value
            End Set
        End Property

        Public Sub New(ByVal _FromAddress As String, _
                        ByVal _ToAddress As String, _
                        ByVal _Subject As String, _
                        ByVal _Body As String, _
                        ByVal _WebOrderID As String, _
                        ByVal _TempOrderID As String)

            MyBase.New()

            Me.FromAddress = _FromAddress
            Me.ToAddress = _ToAddress
            Me.Subject = _Subject
            Me.Body = _Body
            Me.WebOrderID = _WebOrderID
            Me.TempOrderID = _TempOrderID

        End Sub

        Public Function SendMail() As Boolean

            Dim defaults As New Talent.eCommerce.ECommerceModuleDefaults
            Dim def As New Talent.eCommerce.ECommerceModuleDefaults.DefaultValues
            def = defaults.GetDefaults

            ' --------------------------------------------------
            ' Send the email.
            ' --------------------------------------------------

            Body = Body.Replace("<OrderNumber>", WebOrderID)

            Const SelectStr As String = " SELECT * " & _
                                        " FROM tbl_order_header WITH (NOLOCK)  " & _
                                        " WHERE PROCESSED_ORDER_ID = @WebOrderID " & _
                                        " AND BUSINESS_UNIT = @BUSINESS_UNIT " & _
                                        " AND PARTNER = @PARTNER "


            Dim cmd As New SqlCommand(SelectStr, New SqlConnection(ConfigurationManager.ConnectionStrings("SqlServer2005").ToString))
            Dim dr As SqlDataReader
            Try
                cmd.Connection.Open()

                With cmd.Parameters
                    .Clear()
                    .Add("@WebOrderID", SqlDbType.NVarChar).Value = WebOrderID
                    .Add("@BUSINESS_UNIT", SqlDbType.NVarChar).Value = TalentCache.GetBusinessUnit
                    .Add("@PARTNER", SqlDbType.NVarChar).Value = TalentCache.GetPartner(HttpContext.Current.Profile)
                End With

                dr = cmd.ExecuteReader()
                If dr.HasRows Then
                    dr.Read()
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("ADDRESS_LINE_1"))) Then
                        Body = Body.Replace("<address_1>", String.Empty)
                    Else
                        Body = Body.Replace("<address_1>", dr("ADDRESS_LINE_1").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("ADDRESS_LINE_2"))) Then
                        Body = Body.Replace("<address_2>", String.Empty)
                    Else
                        Body = Body.Replace("<address_2>", dr("ADDRESS_LINE_2").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("ADDRESS_LINE_3"))) Then
                        Body = Body.Replace("<address_3>", String.Empty)
                    Else
                        Body = Body.Replace("<address_3>", dr("ADDRESS_LINE_3").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("ADDRESS_LINE_4"))) Then
                        Body = Body.Replace("<address_4>", String.Empty)
                    Else
                        Body = Body.Replace("<address_4>", dr("ADDRESS_LINE_4").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("ADDRESS_LINE_5"))) Then
                        Body = Body.Replace("<address_5>", String.Empty)
                    Else
                        Body = Body.Replace("<address_5>", dr("ADDRESS_LINE_5").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("POSTCODE"))) Then
                        Body = Body.Replace("<address_pcode>", String.Empty)
                    Else
                        Body = Body.Replace("<address_pcode>", dr("POSTCODE").ToString)
                    End If
                    If String.IsNullOrEmpty(Utilities.CheckForDBNull_String(dr("COUNTRY"))) Then
                        Body = Body.Replace("<address_country>", String.Empty)
                    Else
                        Body = Body.Replace("<address_country>", dr("COUNTRY").ToString)
                    End If
                End If

                Body = Body.Replace("<br/>", Environment.NewLine)
                Body = Body.Replace("<br>", Environment.NewLine)

                Dim path As String = "~/Assets/"
                If Not path.EndsWith("/") Then
                    path += "/"
                End If
                path += WebOrderID & ".html"
                path = "http://" & HttpContext.Current.Request.Url.Authority & HttpContext.Current.Request.ApplicationPath & path

                path = path.Replace("~", "")

                Body += Environment.NewLine
                Body += path
                Body += Environment.NewLine

                Talent.Common.Utilities.Email_Send(Me.FromAddress, Me.ToAddress, Me.Subject, Me.Body)

                'Dim mm As MailMessage
                'Dim smtp As New SmtpClient
                'mm = New MailMessage(Me.FromAddress, Me.ToAddress, Me.Subject, Me.Body)
                'smtp.UseDefaultCredentials = True
                'smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis
                'smtp.Send(mm)

                dr.Close()

            Catch ex As Exception
            Finally
                If cmd.Connection.State = ConnectionState.Open Then
                    cmd.Connection.Close()
                End If
            End Try
            Return True
        End Function
    End Class
End Namespace
