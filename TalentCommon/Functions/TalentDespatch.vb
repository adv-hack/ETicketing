Imports System.Collections.Generic
Imports System.Web

<Serializable()> _
Public Class TalentDespatch
    Inherits TalentBase

#Region "Public Properties"
    Public Property DeDespatch As New DEDespatch
    Public Property DespatchCollection As New List(Of DEDespatchItem)
    Public Property ResultDataSet() As DataSet
#End Region

#Region "Public Functions"

    ''' <summary>
    ''' Retrive the Despatch Transaction Items
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveDespatchTransactionItems() As ErrorObj
        Const ModuleName As String = "RetrieveDespatchTransactionItems"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName

        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    Public Function RetrieveDespatchAddressLabelItems() As ErrorObj
        Const ModuleName As String = "RetrieveDespatchAddressLabelItems"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName

        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the Despatch Order information based on the given payment reference or despatch note ID
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveDespatchProcessItems() As ErrorObj
        Const ModuleName As String = "RetrieveDespatchProcessItems"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the payment reference for a given ticket number
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrievePaymentReferenceFromTicketNumber() As ErrorObj
        Const ModuleName As String = "RetrievePaymentReferenceFromTicketNumber"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Retrieve the Despatch Note Items
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function RetrieveDespatchNoteItems() As ErrorObj
        Const ModuleName As String = "RetrieveDespatchNoteItems"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

    ''' <summary>
    ''' Send the all the despatch details to the completion routine
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function DespatchCompletion() As ErrorObj
        Const ModuleName As String = "DespatchCompletion"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            .DespatchCollection = DespatchCollection
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' Perform check against TC043TBL.CH0543 to ensure correct processing occured.
    ''' </summary>
    ''' <returns>Error object</returns>
    ''' <remarks></remarks>
    Public Function DespatchOrderTokenCheck() As ErrorObj
        Const ModuleName As String = "DespatchOrderTokenCheck"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            .DespatchCollection = DespatchCollection
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function
    ''' <summary>
    ''' SOME ONE NEEDS TO SORT THIS MESS OUT PLEASE
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GenerateDespatchNotes() As ErrorObj

        Const ModuleName As String = "GenerateDespatchNotes"
        Dim err As New ErrorObj

        '----------------------------------------------------
        'Create the PDF
        '----------------------------------------------------
        Dim ticketDesignDespatchNote As New TicketDesignerDespatchNote(TicketType.PDF)
        ticketDesignDespatchNote.DeDespatch = DeDespatch
        ticketDesignDespatchNote.Settings = Settings
        ticketDesignDespatchNote.Settings.ModuleName = ModuleName
        err = ticketDesignDespatchNote.CreateDespatchNote()

        '        Me.DeDespatch.GeneratedDespatchPDFDocument = ticketDesignDespatchNote.DespatchNotePath + ticketDesignDespatchNote.DespatchNoteFileName
        Me.DeDespatch.GeneratedDespatchPDFDocument = ticketDesignDespatchNote.DespatchNoteFileName
        Me.DeDespatch.GeneratedDespatchPDFDocumentPageCount = ticketDesignDespatchNote.PageCount
        Return err

    End Function

    ''' <summary>
    ''' Create the tracking reference numbers against a payment reference
    ''' </summary>
    ''' <returns>Error Object</returns>
    ''' <remarks></remarks>
    Public Function CreateTrackingReferences()
        Const ModuleName As String = "CreateTrackingReferences"
        Dim err As New ErrorObj
        Dim despatch As New DBDespatch
        GetConnectionDetails(Settings.BusinessUnit, String.Empty, ModuleName)
        DeDespatch.Mode = "C"
        Settings.ModuleName = ModuleName
        With despatch
            .Settings = Settings
            .DeDespatch = DeDespatch
            If Not err.HasError Then
                err = .AccessDatabase()
            End If
            If Not err.HasError And Not .ResultDataSet Is Nothing Then
                ResultDataSet = .ResultDataSet
            End If
        End With
        Return err
    End Function

#End Region
End Class
