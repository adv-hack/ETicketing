Imports System
Imports TalentBusinessLogic.ModelBuilders
Imports TalentBusinessLogic.Models

Partial Class Examples_DataAnnotation
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim model As New DataAnnotationInputModel()
        model.FirstName = txtFirstName.Text
        model.LastName = txtLastName.Text
        model.TicketQuantity = txtQuantity.Text

        Dim modelBuilder As New ModelBuilder()
        blErrorMessages.Items.Clear()
        Dim viewModel As DataAnnotationViewModel = modelBuilder.Call(Of DataAnnotationInputModel, DataAnnotationViewModel)(model, New ExampleModelBuilder.DITestAction())
        If viewModel.Error IsNot Nothing Then
            For Each errMessage As String In viewModel.Error
                blErrorMessages.Items.Add(errMessage)
            Next
        End If
    End Sub
End Class
