
<Serializable()> _
Public Class DBStatementEnquiry
    Inherits DBAccess


    Private _stmt As DEStatement
    Public Property Statement() As DEStatement
        Get
            Return _stmt
        End Get
        Set(ByVal value As DEStatement)
            _stmt = value
        End Set
    End Property

    Private _dtHeader As New DataTable
    Private _dtItem As New DataTable
    Private _parmTRAN As String

    Private Property ParmTRAN() As String
        Get
            Return _parmTRAN
        End Get
        Set(ByVal value As String)
            _parmTRAN = value
        End Set
    End Property

    Public Property DtHeader() As DataTable
        Get
            Return _dtHeader
        End Get
        Set(ByVal value As DataTable)
            _dtHeader = value
        End Set
    End Property
    Public Property DtItem() As DataTable
        Get
            Return _dtItem
        End Get
        Set(ByVal value As DataTable)
            _dtItem = value
        End Set
    End Property


    Protected Overrides Function AccessDataBaseSQL2005() As ErrorObj
        Dim err As New ErrorObj

        'Get the invoices
        '**************************************
        Dim invoice As New TalentInvoicing
        Dim deI As New Talent.Common.DEInvoice(Statement.BusinessUnit, Statement.Username)
        Try
            deI.FromDate = Statement.FromDate
        Catch ex As Exception
        End Try
        Try
            deI.ToDate = Statement.ToDate
        Catch ex As Exception
        End Try
        deI.OrderNumber = Statement.OrderNumber
        deI.Finalised = Statement.Finalised

        invoice.Dep = deI
        invoice.Settings = settings
        invoice.GetConnectionDetails(Statement.BusinessUnit)

        err = invoice.InvoiceEnquiryHeader

        If Not err.HasError Then

            Dim dtInv As Data.DataTable = invoice.ResultDataSet.Tables("InvoiceEnquiryHeader")
            '************************************

            'Get the credit notes
            '************************************
            Dim creditnote As New TalentCreditNote
            Dim deC As New Talent.Common.DECreditNote(Statement.BusinessUnit, Statement.Username)
            Try
                deC.FromDate = Statement.FromDate
            Catch ex As Exception
            End Try
            Try
                deC.ToDate = Statement.ToDate
            Catch ex As Exception
            End Try
            deC.OrderNumber = Statement.OrderNumber
            deC.Finalised = Statement.Finalised

            creditnote.Dep = deC
            creditnote.Settings = Settings
            creditnote.GetConnectionDetails(Statement.BusinessUnit)

            err = creditnote.GetCreditNoteHeader

            If Not err.HasError Then

                Dim dtCred As Data.DataTable = creditnote.ResultDataSet.Tables("CreditNoteEnquiryHeader")
                '***********************************

                err = CombineInvoicesAndCreditNotes(dtInv, dtCred)
            End If
        End If

        Return err
    End Function

    Protected Function CombineInvoicesAndCreditNotes(ByVal dtInv As DataTable, ByVal dtCred As DataTable) As ErrorObj
        Dim err As New ErrorObj

        Const creditNoteStr As String = "CreditNote"
        Const invoiceStr As String = "Invoice"

        Try
            ' Create a new result set and populate 
            ' with both sets of results
            '***********************************
            Me.ResultDataSet = New DataSet("Statements")
            DtHeader = New DataTable("StatementsHeader")
            AddColumnsToDataTables()

            If dtInv.Rows.Count > 0 Then
                For Each rInv As DataRow In dtInv.Rows
                    Dim dRow As DataRow = DtHeader.NewRow
                    dRow = Nothing
                    dRow = DtHeader.NewRow()
                    dRow("OrderNumber") = Utilities.CheckForDBNull_String(rInv("OrderNumber"))
                    dRow("CustomerCompanyCode") = "" 'Utilities.CheckForDBNull_String(rinv("CustomerCompanyCode"))
                    dRow("BusinessUnit") = Utilities.CheckForDBNull_String(rInv("BusinessUnit"))
                    dRow("Partner") = Utilities.CheckForDBNull_String(rInv("Partner"))
                    dRow("PartnerNumber") = Utilities.CheckForDBNull_String(rInv("PartnerNumber"))
                    dRow("LoginID") = Utilities.CheckForDBNull_String(rInv("LoginID"))
                    dRow("CustomerNumber") = Utilities.CheckForDBNull_String(rInv("CustomerNumber"))
                    dRow("Finalised") = Utilities.CheckForDBNull_Boolean_DefaultFalse(rInv("Finalised"))
                    dRow("ItemNumber") = Utilities.CheckForDBNull_String(rInv("InvoiceNumber"))
                    dRow("ItemDateTime") = Utilities.CheckForDBNull_DateTime(rInv("InvoiceDateTime"))
                    dRow("ItemAmount") = Utilities.CheckForDBNull_Decimal(rInv("InvoiceAmount"))
                    dRow("VatAmount") = Utilities.CheckForDBNull_Decimal(rInv("VatAmount"))
                    dRow("ChargesAmount") = 0 'Utilities.CheckForDBNull_String(rinv("ChargesAmount"))
                    dRow("OutstandingAmount") = Utilities.CheckForDBNull_String(rInv("OutstandingAmount"))
                    dRow("ItemProcessed") = Utilities.CheckForDBNull_String(rInv("InvoiceProcessed"))
                    dRow("CurrencyCode") = "" 'Utilities.CheckForDBNull_String(rinv("CurrencyCode"))
                    dRow("CustomerPO") = Utilities.CheckForDBNull_String(rInv("CustomerPO"))
                    dRow("OriginalOrderNo") = Utilities.CheckForDBNull_String(rInv("OriginalOrderNo"))
                    dRow("OriginalOrderDate") = Utilities.CheckForDBNull_DateTime(rInv("OriginalOrderDate"))
                    dRow("DispatchSequence") = Utilities.CheckForDBNull_String(rInv("DispatchSequence"))
                    dRow("AccountNumber") = Utilities.CheckForDBNull_String(rInv("AccountNumber"))
                    dRow("CustomerName") = Utilities.CheckForDBNull_String(rInv("CustomerName"))
                    dRow("CustomerAttention") = Utilities.CheckForDBNull_String(rInv("CustomerAttention"))
                    dRow("CustomerAddress1") = Utilities.CheckForDBNull_String(rInv("CustomerAddress1"))
                    dRow("CustomerAddress2") = Utilities.CheckForDBNull_String(rInv("CustomerAddress2"))
                    dRow("CustomerAddress3") = Utilities.CheckForDBNull_String(rInv("CustomerAddress3"))
                    dRow("CustomerAddress4") = Utilities.CheckForDBNull_String(rInv("CustomerAddress4"))
                    dRow("CustomerAddress5") = Utilities.CheckForDBNull_String(rInv("CustomerAddress5"))
                    dRow("CustomerAddress6") = Utilities.CheckForDBNull_String(rInv("CustomerAddress6"))
                    dRow("CustomerAddress7") = Utilities.CheckForDBNull_String(rInv("CustomerAddress7"))
                    dRow("ShipToName") = Utilities.CheckForDBNull_String(rInv("ShipToName"))
                    dRow("ShipToAttention") = Utilities.CheckForDBNull_String(rInv("ShipToAttention"))
                    dRow("ShipToAddress1") = Utilities.CheckForDBNull_String(rInv("ShipToAddress1"))
                    dRow("ShipToAddress2") = Utilities.CheckForDBNull_String(rInv("ShipToAddress2"))
                    dRow("ShipToAddress3") = Utilities.CheckForDBNull_String(rInv("ShipToAddress3"))
                    dRow("ShipToAddress4") = Utilities.CheckForDBNull_String(rInv("ShipToAddress4"))
                    dRow("ShipToAddress5") = Utilities.CheckForDBNull_String(rInv("ShipToAddress5"))
                    dRow("ShipToAddress6") = Utilities.CheckForDBNull_String(rInv("ShipToAddress6"))
                    dRow("ShipToAddress7") = Utilities.CheckForDBNull_String(rInv("ShipToAddress7"))
                    dRow("ShipFromName") = Utilities.CheckForDBNull_String(rInv("ShipFromName"))
                    dRow("ShipFromAttention") = Utilities.CheckForDBNull_String(rInv("ShipFromAttention"))
                    dRow("ShipFromAddress1") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress1"))
                    dRow("ShipFromAddress2") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress2"))
                    dRow("ShipFromAddress3") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress3"))
                    dRow("ShipFromAddress4") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress4"))
                    dRow("ShipFromAddress5") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress5"))
                    dRow("ShipFromAddress6") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress6"))
                    dRow("ShipFromAddress7") = Utilities.CheckForDBNull_String(rInv("ShipFromAddress7"))
                    dRow("VATNumber") = Utilities.CheckForDBNull_String(rInv("VATNumber"))
                    dRow("PaymentTermsType") = Utilities.CheckForDBNull_String(rInv("PaymentTermsType"))
                    dRow("PaymentTermsPeriod") = Utilities.CheckForDBNull_Int(rInv("PaymentTermsPeriod"))
                    dRow("PaymentTermsDays") = Utilities.CheckForDBNull_Int(rInv("PaymentTermsDays"))
                    dRow("Type") = invoiceStr
                    DtHeader.Rows.Add(dRow)
                Next
            End If

            If dtCred.Rows.Count > 0 Then
                For Each rCred As DataRow In dtCred.Rows
                    Dim dRow As DataRow = DtHeader.NewRow
                    dRow = Nothing
                    dRow = DtHeader.NewRow()
                    dRow("OrderNumber") = Utilities.CheckForDBNull_String(rCred("OrderNumber"))
                    dRow("CustomerCompanyCode") = "" 'Utilities.CheckForDBNull_String(rCred("CustomerCompanyCode"))
                    dRow("BusinessUnit") = Utilities.CheckForDBNull_String(rCred("BusinessUnit"))
                    dRow("Partner") = Utilities.CheckForDBNull_String(rCred("Partner"))
                    dRow("PartnerNumber") = Utilities.CheckForDBNull_String(rCred("PartnerNumber"))
                    dRow("LoginID") = Utilities.CheckForDBNull_String(rCred("LoginID"))
                    dRow("CustomerNumber") = Utilities.CheckForDBNull_String(rCred("CustomerNumber"))
                    dRow("Finalised") = Utilities.CheckForDBNull_Boolean_DefaultFalse(rCred("Finalised"))
                    dRow("ItemNumber") = Utilities.CheckForDBNull_String(rCred("CreditNoteNumber"))
                    dRow("ItemDateTime") = Utilities.CheckForDBNull_DateTime(rCred("CreditNoteDateTime"))
                    dRow("ItemAmount") = Utilities.CheckForDBNull_Decimal(rCred("CreditNoteAmount"))
                    dRow("VatAmount") = Utilities.CheckForDBNull_Decimal(rCred("VatAmount"))
                    dRow("ChargesAmount") = 0 'Utilities.CheckForDBNull_String(rCred("ChargesAmount"))
                    dRow("OutstandingAmount") = Utilities.CheckForDBNull_String(rCred("OutstandingAmount"))
                    dRow("ItemProcessed") = Utilities.CheckForDBNull_String(rCred("CreditNoteProcessed"))
                    dRow("CurrencyCode") = "" 'Utilities.CheckForDBNull_String(rCred("CurrencyCode"))
                    dRow("CustomerPO") = Utilities.CheckForDBNull_String(rCred("CustomerPO"))
                    dRow("OriginalOrderNo") = Utilities.CheckForDBNull_String(rCred("OriginalOrderNo"))
                    dRow("OriginalOrderDate") = Utilities.CheckForDBNull_DateTime(rCred("OriginalOrderDate"))
                    dRow("DispatchSequence") = Utilities.CheckForDBNull_String(rCred("DispatchSequence"))
                    dRow("AccountNumber") = Utilities.CheckForDBNull_String(rCred("AccountNumber"))
                    dRow("CustomerName") = Utilities.CheckForDBNull_String(rCred("CustomerName"))
                    dRow("CustomerAttention") = Utilities.CheckForDBNull_String(rCred("CustomerAttention"))
                    dRow("CustomerAddress1") = Utilities.CheckForDBNull_String(rCred("CustomerAddress1"))
                    dRow("CustomerAddress2") = Utilities.CheckForDBNull_String(rCred("CustomerAddress2"))
                    dRow("CustomerAddress3") = Utilities.CheckForDBNull_String(rCred("CustomerAddress3"))
                    dRow("CustomerAddress4") = Utilities.CheckForDBNull_String(rCred("CustomerAddress4"))
                    dRow("CustomerAddress5") = Utilities.CheckForDBNull_String(rCred("CustomerAddress5"))
                    dRow("CustomerAddress6") = Utilities.CheckForDBNull_String(rCred("CustomerAddress6"))
                    dRow("CustomerAddress7") = Utilities.CheckForDBNull_String(rCred("CustomerAddress7"))
                    dRow("ShipToName") = Utilities.CheckForDBNull_String(rCred("ShipToName"))
                    dRow("ShipToAttention") = Utilities.CheckForDBNull_String(rCred("ShipToAttention"))
                    dRow("ShipToAddress1") = Utilities.CheckForDBNull_String(rCred("ShipToAddress1"))
                    dRow("ShipToAddress2") = Utilities.CheckForDBNull_String(rCred("ShipToAddress2"))
                    dRow("ShipToAddress3") = Utilities.CheckForDBNull_String(rCred("ShipToAddress3"))
                    dRow("ShipToAddress4") = Utilities.CheckForDBNull_String(rCred("ShipToAddress4"))
                    dRow("ShipToAddress5") = Utilities.CheckForDBNull_String(rCred("ShipToAddress5"))
                    dRow("ShipToAddress6") = Utilities.CheckForDBNull_String(rCred("ShipToAddress6"))
                    dRow("ShipToAddress7") = Utilities.CheckForDBNull_String(rCred("ShipToAddress7"))
                    dRow("ShipFromName") = Utilities.CheckForDBNull_String(rCred("ShipFromName"))
                    dRow("ShipFromAttention") = Utilities.CheckForDBNull_String(rCred("ShipFromAttention"))
                    dRow("ShipFromAddress1") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress1"))
                    dRow("ShipFromAddress2") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress2"))
                    dRow("ShipFromAddress3") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress3"))
                    dRow("ShipFromAddress4") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress4"))
                    dRow("ShipFromAddress5") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress5"))
                    dRow("ShipFromAddress6") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress6"))
                    dRow("ShipFromAddress7") = Utilities.CheckForDBNull_String(rCred("ShipFromAddress7"))
                    dRow("VATNumber") = Utilities.CheckForDBNull_String(rCred("VATNumber"))
                    dRow("PaymentTermsType") = Utilities.CheckForDBNull_String(rCred("PaymentTermsType"))
                    dRow("PaymentTermsPeriod") = Utilities.CheckForDBNull_Int(rCred("PaymentTermsPeriod"))
                    dRow("PaymentTermsDays") = Utilities.CheckForDBNull_Int(rCred("PaymentTermsDays"))
                    dRow("Type") = creditNoteStr
                    DtHeader.Rows.Add(dRow)
                Next
            End If
            '*************************************
        Catch ex As Exception
            err.HasError = True
            err.ErrorMessage = ex.Message
            err.ErrorNumber = ""
        End Try

        ResultDataSet.Tables.Add(DtHeader)

        Return err
    End Function

    Private Sub AddColumnsToDataTables()
        With DtHeader.Columns
            .Add("OrderNumber", GetType(String))
            .Add("CustomerCompanyCode", GetType(String))
            .Add("BusinessUnit", GetType(String))
            .Add("Partner", GetType(String))
            .Add("PartnerNumber", GetType(String))
            .Add("LoginID", GetType(String))
            .Add("CustomerNumber", GetType(String))
            .Add("ItemNumber", GetType(String))
            .Add("Finalised", GetType(Boolean))
            .Add("ItemDateTime", GetType(Date))
            .Add("ItemAmount", GetType(Decimal))
            .Add("VatAmount", GetType(Decimal))
            .Add("ChargesAmount", GetType(Decimal))
            .Add("OutstandingAmount", GetType(Decimal))
            .Add("ItemProcessed", GetType(String))
            .Add("CurrencyCode", GetType(String))
            .Add("CustomerPO", GetType(String))
            .Add("OriginalOrderNo", GetType(String))
            .Add("OriginalOrderDate", GetType(Date))
            .Add("DispatchSequence", GetType(String))
            .Add("AccountNumber", GetType(String))
            .Add("CustomerName", GetType(String))
            .Add("CustomerAttention", GetType(String))
            .Add("CustomerAddress1", GetType(String))
            .Add("CustomerAddress2", GetType(String))
            .Add("CustomerAddress3", GetType(String))
            .Add("CustomerAddress4", GetType(String))
            .Add("CustomerAddress5", GetType(String))
            .Add("CustomerAddress6", GetType(String))
            .Add("CustomerAddress7", GetType(String))
            .Add("ShipToName", GetType(String))
            .Add("ShipToAttention", GetType(String))
            .Add("ShipToAddress1", GetType(String))
            .Add("ShipToAddress2", GetType(String))
            .Add("ShipToAddress3", GetType(String))
            .Add("ShipToAddress4", GetType(String))
            .Add("ShipToAddress5", GetType(String))
            .Add("ShipToAddress6", GetType(String))
            .Add("ShipToAddress7", GetType(String))
            .Add("ShipFromName", GetType(String))
            .Add("ShipFromAttention", GetType(String))
            .Add("ShipFromAddress1", GetType(String))
            .Add("ShipFromAddress2", GetType(String))
            .Add("ShipFromAddress3", GetType(String))
            .Add("ShipFromAddress4", GetType(String))
            .Add("ShipFromAddress5", GetType(String))
            .Add("ShipFromAddress6", GetType(String))
            .Add("ShipFromAddress7", GetType(String))
            .Add("VATNumber", GetType(String))
            .Add("PaymentTermsType", GetType(String))
            .Add("PaymentTermsPeriod", GetType(Integer))
            .Add("PaymentTermsDays", GetType(Integer))
            .Add("Type", GetType(String))
        End With
    End Sub


End Class
