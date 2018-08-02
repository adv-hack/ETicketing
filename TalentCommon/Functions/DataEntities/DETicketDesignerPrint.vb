''' <summary>
''' Class holds all the ticket designer mapped data with fields in ticket layout 
''' and functionalities to retrieve those details
''' </summary>
<Serializable()> _
Public Class DETicketDesignerPrint

#Region "Class Level Fields"
    Private _varText As New Generic.Dictionary(Of String, String)
    Private _barcode As String = ""
    Private _orderDetails As Data.DataTable
    Private _despatchItems As Data.DataTable
    Private _despatchItemsSummary As Data.DataTable
    Private _despatchMode As String
#End Region

#Region "Properties"

    Public Property CurrentDate() As String
        Get
            Return GetValue("DATE")
        End Get
        Set(ByVal value As String)
            SetValue("DATE", value)
        End Set
    End Property

    Public Property CurrentTime() As String
        Get
            Return GetValue("TIME")
        End Get
        Set(ByVal value As String)
            SetValue("TIME", value)
        End Set
    End Property

    Public Property ProductCode() As String
        Get
            Return GetValue("MTCD08")
        End Get
        Set(ByVal value As String)
            SetValue("MTCD08", value)
        End Set
    End Property

    Public Property ProductDescription() As String
        Get
            Return GetValue("MTDS08")
        End Get
        Set(ByVal value As String)
            SetValue("MTDS08", value)
        End Set
    End Property

    Public Property ProductDate() As String
        Get
            Return GetValue("MDTE08")
        End Get
        Set(ByVal value As String)
            SetValue("MDTE08", value)
        End Set
    End Property

    Public Property ProductDateInWords() As String
        Get
            Return GetValue("MDTEWD")
        End Get
        Set(ByVal value As String)
            SetValue("MDTEWD", value)
        End Set
    End Property

    Public Property TicketText() As String
        Get
            Return GetValue("TXT108")
        End Get
        Set(ByVal value As String)
            SetValue("TXT108", value)
        End Set
    End Property

    Public Property ProductTime() As String
        Get
            Return GetValue("MTKO08")
        End Get
        Set(ByVal value As String)
            SetValue("MTKO08", value)
        End Set
    End Property

    Public Property PayReference() As String
        Get
            Return GetValue("PYRF15")
        End Get
        Set(ByVal value As String)
            SetValue("PYRF15", value)
        End Set
    End Property

    Public Property PayType() As String
        Get
            Return GetValue("PYTP15")
        End Get
        Set(ByVal value As String)
            SetValue("PYTP15", value)
        End Set
    End Property

    Public Property PayTypeDescription() As String
        Get
            Return GetValue("PYTP")
        End Get
        Set(ByVal value As String)
            SetValue("PYTP", value)
        End Set
    End Property

    Public Property PayAmount() As String
        Get
            Return GetValue("PYMT15")
        End Get
        Set(ByVal value As String)
            SetValue("PYMT15", value)
        End Set
    End Property

    Public Property Stand() As String
        Get
            Return GetValue("STND09")
        End Get
        Set(ByVal value As String)
            SetValue("STND09", value)
        End Set
    End Property

    Public Property StandDesc() As String
        Get
            Return GetValue("STTX02")
        End Get
        Set(ByVal value As String)
            SetValue("STTX02", value)
        End Set
    End Property

    Public Property Area() As String
        Get
            Return GetValue("AREA09")
        End Get
        Set(ByVal value As String)
            SetValue("AREA09", value)
        End Set
    End Property

    Public Property AreaText() As String
        Get
            Return GetValue("ARTX21")
        End Get
        Set(ByVal value As String)
            SetValue("ARTX21", value)
        End Set
    End Property

    Public Property RowN() As String
        Get
            Return GetValue("ROWN09")
        End Get
        Set(ByVal value As String)
            SetValue("ROWN09", value)
        End Set
    End Property

    Public Property SeatN() As String
        Get
            Return GetValue("SNUM09")
        End Get
        Set(ByVal value As String)
            SetValue("SNUM09", value)
        End Set
    End Property

    Public Property SeatSuffix() As String
        Get
            Return GetValue("ASFX09")
        End Get
        Set(ByVal value As String)
            SetValue("ASFX09", value)
        End Set
    End Property

    Public Property Seat() As String
        Get
            Return GetValue("SEAT1")
        End Get
        Set(ByVal value As String)
            SetValue("SEAT1", value)
        End Set
    End Property

    Public Property SalePrice() As String
        Get
            Return GetValue("SALP09")
        End Get
        Set(ByVal value As String)
            SetValue("SALP09", value)
        End Set
    End Property

    Public Property Turnstile() As String
        Get
            Return GetValue("TURN")
        End Get
        Set(ByVal value As String)
            SetValue("TURN", value)
        End Set
    End Property

    Public Property Gate() As String
        Get
            Return GetValue("GATE")
        End Get
        Set(ByVal value As String)
            SetValue("GATE", value)
        End Set
    End Property

    Public Property SeatText() As String
        Get
            Return GetValue("TXTD09")
        End Get
        Set(ByVal value As String)
            SetValue("TXTD09", value)
        End Set
    End Property

    Public Property RestText() As String
        Get
            Return GetValue("SRSC")
        End Get
        Set(ByVal value As String)
            SetValue("SRSC", value)
        End Set
    End Property

    Public Property PriceCode() As String
        Get
            Return GetValue("PRCD09")
        End Get
        Set(ByVal value As String)
            SetValue("PRCD09", value)
        End Set
    End Property

    Public Property PriceCodeDesc() As String
        Get
            Return GetValue("STXT06")
        End Get
        Set(ByVal value As String)
            SetValue("STXT06", value)
        End Set
    End Property

    Public Property PriceBand() As String
        Get
            Return GetValue("PRCE09")
        End Get
        Set(ByVal value As String)
            SetValue("PRCE09", value)
        End Set
    End Property

    Public Property PriceBandDesc() As String
        Get
            Return GetValue("STXT12")
        End Get
        Set(ByVal value As String)
            SetValue("STXT12", value)
        End Set
    End Property

    Public Property TicketID() As String
        Get
            Return GetValue("SATKID")
        End Get
        Set(ByVal value As String)
            SetValue("SATKID", value)
        End Set
    End Property

    Public Property CustomerNumber() As String
        Get
            Return GetValue("MEMB20")
        End Get
        Set(ByVal value As String)
            SetValue("MEMB20", value)
        End Set
    End Property

    Public Property Title() As String
        Get
            Return GetValue("TITL20")
        End Get
        Set(ByVal value As String)
            SetValue("TITL20", value)
        End Set
    End Property

    Public Property FirstName() As String
        Get
            Return GetValue("FNAM20")
        End Get
        Set(ByVal value As String)
            SetValue("FNAM20", value)
        End Set
    End Property

    Public Property Surname() As String
        Get
            Return GetValue("SNAM20")
        End Get
        Set(ByVal value As String)
            SetValue("SNAM20", value)
        End Set
    End Property

    Public Property Barcode() As String
        Get
            Return _barcode
        End Get
        Set(ByVal value As String)
            _barcode = value
        End Set
    End Property

    Public Property AddressLine1() As String
        Get
            Return GetValue("ADD120")
        End Get
        Set(ByVal value As String)
            SetValue("ADD120", value)
        End Set
    End Property

    Public Property AddressLine2() As String
        Get
            Return GetValue("ADD220")
        End Get
        Set(ByVal value As String)
            SetValue("ADD220", value)
        End Set
    End Property

    Public Property AddressLine3() As String
        Get
            Return GetValue("ADD320")
        End Get
        Set(ByVal value As String)
            SetValue("ADD320", value)
        End Set
    End Property

    Public Property AddressLine4() As String
        Get
            Return GetValue("ADD420")
        End Get
        Set(ByVal value As String)
            SetValue("ADD420", value)
        End Set
    End Property

    Public Property AddressLine5() As String
        Get
            Return GetValue("ADD520")
        End Get
        Set(ByVal value As String)
            SetValue("ADD520", value)
        End Set
    End Property

    Public Property PostCode() As String
        Get
            Return GetValue("POST20")
        End Get
        Set(ByVal value As String)
            SetValue("POST20", value)
        End Set
    End Property

    Public Property ClientReferenceName() As String
        Get
            Return GetValue("EMREFR")
        End Get
        Set(ByVal value As String)
            SetValue("EMREFR", value)
        End Set
    End Property

    Public Property CallReference() As String
        Get
            Return GetValue("AHMCID")
        End Get
        Set(ByVal value As String)
            SetValue("AHMCID", value)
        End Set
    End Property

    Public Property PackageDescription() As String
        Get
            Return GetValue("PHDESC")
        End Get
        Set(ByVal value As String)
            SetValue("PHDESC", value)
        End Set
    End Property

    Public Property NumberOfUnits() As String
        Get
            Return GetValue("ADNUNI")
        End Get
        Set(ByVal value As String)
            SetValue("ADNUNI", value)
        End Set
    End Property

    Public Property GoodsValue() As String
        Get
            Return GetValue("AHTOTV")
        End Get
        Set(ByVal value As String)
            SetValue("AHTOTV", value)
        End Set
    End Property

    Public Property TotalVATValue() As String
        Get
            Return GetValue("AHVATV")
        End Get
        Set(ByVal value As String)
            SetValue("AHVATV", value)
        End Set
    End Property

    Public Property CostPerPackage() As String
        Get
            Return GetValue("PKGCST")
        End Get
        Set(ByVal value As String)
            SetValue("PKGCST", value)
        End Set
    End Property

    Public Property VoucherCode() As String
        Get
            Return GetValue("VOUC02")
        End Get
        Set(ByVal value As String)
            SetValue("VOUC02", value)
        End Set
    End Property

    Public Property ComponentDescription() As String
        Get
            Return GetValue("CHDESC")
        End Get
        Set(ByVal value As String)
            SetValue("CHDESC", value)
        End Set
    End Property

    Public Property PaymentReference() As String
        Get
            Return GetValue("STPYRF")
        End Get
        Set(ByVal value As String)
            SetValue("STPYRF", value)
        End Set
    End Property
    Public Property TotalPrice() As String
        Get
            Return GetValue("STTOTP")
        End Get
        Set(ByVal value As String)
            SetValue("STTOTP", value)
        End Set
    End Property
    Public Property PaymentMethod() As String
        Get
            Return GetValue("STPAYM")
        End Get
        Set(ByVal value As String)
            SetValue("STPAYM", value)
        End Set
    End Property
    Public Property RefundAmount() As String
        Get
            Return GetValue("STRAMT")
        End Get
        Set(ByVal value As String)
            SetValue("STRAMT", value)
        End Set
    End Property
    Public Property OnAccountAmount() As String
        Get
            Return GetValue("STAAMT")
        End Get
        Set(ByVal value As String)
            SetValue("STAAMT", value)
        End Set
    End Property
    Public Property onAccountRefunded() As String
        Get
            Return GetValue("STAREF")
        End Get
        Set(ByVal value As String)
            SetValue("STAREF", value)
        End Set
    End Property
    Public Property deliveryContactName() As String
        Get
            Return GetValue("STDLCN")
        End Get
        Set(ByVal value As String)
            SetValue("STDLCN", value)
        End Set
    End Property
    Public Property deliveryAddress1() As String
        Get
            Return GetValue("STDLA1")
        End Get
        Set(ByVal value As String)
            SetValue("STDLA1", value)
        End Set
    End Property
    Public Property deliveryAddress2() As String
        Get
            Return GetValue("STDLA2")
        End Get
        Set(ByVal value As String)
            SetValue("STDLA2", value)
        End Set
    End Property
    Public Property deliveryAddress3() As String
        Get
            Return GetValue("STDLA3")
        End Get
        Set(ByVal value As String)
            SetValue("STDLA3", value)
        End Set
    End Property
    Public Property deliveryAddress4() As String
        Get
            Return GetValue("STDLA4")
        End Get
        Set(ByVal value As String)
            SetValue("STDLA4", value)
        End Set
    End Property
    Public Property deliveryAddress5() As String
        Get
            Return GetValue("STDLA5")
        End Get
        Set(ByVal value As String)
            SetValue("STDLA5", value)
        End Set
    End Property
    Public Property deliveryPostCode() As String
        Get
            Return GetValue("STDLPC")
        End Get
        Set(ByVal value As String)
            SetValue("STDLPC", value)
        End Set
    End Property
    Public Property orderDetails() As Data.DataTable
        Get
            Return _orderDetails
        End Get
        Set(ByVal value As Data.DataTable)
            _orderDetails = value
        End Set
    End Property

    Public Property despatchItems() As Data.DataTable
        Get
            Return _despatchItems
        End Get
        Set(ByVal value As Data.DataTable)
            _despatchItems = value
        End Set
    End Property
    Public Property despatchItemsSummary() As Data.DataTable
        Get
            Return _despatchItemsSummary
        End Get
        Set(ByVal value As Data.DataTable)
            _despatchItemsSummary = value
        End Set
    End Property

    Public Property despatchPaymentReference() As String
        Get
            Return GetValue("PREF07")
        End Get
        Set(ByVal value As String)
            SetValue("PREF07", value)
        End Set
    End Property
    Public Property despatchCustomerNumber() As String
        Get
            Return GetValue("MEMB07")
        End Get
        Set(ByVal value As String)
            SetValue("MEMB07", value)
        End Set
    End Property
    Public Property despatchDeliveryContactName() As String
        Get
            Return GetValue("NAME07")
        End Get
        Set(ByVal value As String)
            SetValue("NAME07", value)
        End Set
    End Property
    Public Property despatchDeliveryAddress1() As String
        Get
            Return GetValue("ADD107")
        End Get
        Set(ByVal value As String)
            SetValue("ADD107", value)
        End Set
    End Property
    Public Property despatchDeliveryAddress2() As String
        Get
            Return GetValue("ADD207")
        End Get
        Set(ByVal value As String)
            SetValue("ADD207", value)
        End Set
    End Property
    Public Property despatchDeliveryAddress3() As String
        Get
            Return GetValue("ADD307")
        End Get
        Set(ByVal value As String)
            SetValue("ADD307", value)
        End Set
    End Property
    Public Property despatchDeliveryAddress4() As String
        Get
            Return GetValue("ADD407")
        End Get
        Set(ByVal value As String)
            SetValue("ADD407", value)
        End Set
    End Property
    Public Property despatchDeliveryAddress5() As String
        Get
            Return GetValue("ADD507")
        End Get
        Set(ByVal value As String)
            SetValue("ADD507", value)
        End Set
    End Property
    Public Property despatchDeliveryPostCode() As String
        Get
            Return GetValue("POST07")
        End Get
        Set(ByVal value As String)
            SetValue("POST07", value)
        End Set
    End Property
    Public Property despatchSaleDate() As String
        Get
            Return GetValue("SLDT07")
        End Get
        Set(ByVal value As String)
            SetValue("SLDT07", value)
        End Set
    End Property

    Public Property despatchMode As String
        Get
            Return _despatchMode
        End Get
        Set(ByVal value As String)
            _despatchMode = value
        End Set
    End Property

    Public Property despatchCustomerAttribute() As String
        Get
            Return GetValue("CH0407")
        End Get
        Set(ByVal value As String)
            SetValue("CH0407", value)
        End Set
    End Property

    Public Property despatchCustomerCompanyName() As String
        Get
            Return GetValue("CH1007")
        End Get
        Set(ByVal value As String)
            SetValue("CH1007", value)
        End Set
    End Property

    Public Property VoucherExpiryDate() As String
        Get
            Return GetValue("EXDT02")
        End Get
        Set(ByVal value As String)
            SetValue("EXDT02", value)
        End Set
    End Property

#End Region

#Region "Public Methods"

    Public Function GetValueByKey(ByVal key As String) As String
        GetValueByKey = ""
        If Not _varText.TryGetValue(key, GetValueByKey) Then
            GetValueByKey = "NA"
        End If
        Return GetValueByKey
    End Function

#End Region

#Region "Private Methods"

    Private Sub SetValue(ByVal key As String, ByVal value As String)
        If _varText.ContainsKey(key) Then
            _varText(key) = value
        Else
            _varText.Add(key, value)
        End If
    End Sub

    Private Function GetValue(ByVal key As String) As String
        GetValue = ""
        If _varText.ContainsKey(key) Then
            GetValue = _varText.Item(key)
        End If
        Return GetValue
    End Function

#End Region

End Class
