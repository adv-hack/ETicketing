Imports System.Data
Imports System.Xml
Imports Talent.Common
Imports TalentBusinessLogic.Models
Imports TalentBusinessLogic.ModelBuilders.Profile

Namespace Talent.TradingPortal

    Public Class XmlRetrievePasswordResponse
        Inherits XmlResponse

        Private _ndHeaderRoot, _ndHeader As XmlNode
        Private _ndVerifyPassword, _ndResponse, _ndReturnCode As XmlNode
        Private _ndUserName, _ndPassword, _ndEmailSentToUser, _ndEncryptedPassword As XmlNode
        Private _dtRetrievePasswordResults, _dtStatusResults As DataTable
        Private _doTokenHashing As Boolean

        Public SendForgottenPasswordEmail As Boolean
        Public EmailAddress As String

        Protected Overrides Sub InsertBodyV1()
            Try
                With MyBase.xmlDoc
                    _ndVerifyPassword = .CreateElement("RetrievePassword")
                    _ndResponse = .CreateElement("Response")
                End With
                Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
                _doTokenHashing = Utilities.CheckForDBNull_Boolean_DefaultFalse(def.GetDefault("HASH_PASSWORD"))
                createResponseSection()

                'Populate the xml document
                With _ndVerifyPassword
                    .AppendChild(_ndResponse)
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                _ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                _ndHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                _ndHeader.InsertAfter(_ndVerifyPassword, _ndHeaderRoot)

                'Here we call a model builder to generate the forgotten password for the user.
                If SendForgottenPasswordEmail Then
                    Dim inputModel As New ForgottenPasswordInputModel
                    Dim viewModel As New ForgottenPasswordViewModel
                    Dim forgottenPasswordBuilder As New PasswordModelBuilders

                    inputModel.Source = "S"
                    inputModel.CustomerNumber = ResultDataSet.Tables("CustomerResults").Rows(0)("CustomerNumber").ToString().Trim()
                    inputModel.EmailAddress = ResultDataSet.Tables("CustomerResults").Rows(0)("EmailAddress").ToString().Trim()
                    inputModel.Mode = "Response"
                    inputModel.DoTokenHashing = _doTokenHashing
                    viewModel = forgottenPasswordBuilder.ForgottenPassword(inputModel)
                    If viewModel.Error IsNot Nothing AndAlso viewModel.Error.HasError Then
                        Throw New Exception(viewModel.Error.ErrorMessage)
                    End If
                End If
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRP-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Private Sub createResponseSection()
            Dim dr, drStatus As DataRow

            'Read the values for the response section
            _dtStatusResults = ResultDataSet.Tables("StatusResults")
            _dtRetrievePasswordResults = ResultDataSet.Tables("CustomerResults")
            drStatus = _dtStatusResults.Rows(0)
            dr = _dtRetrievePasswordResults.Rows(0)

            'Create the response xml nodes
            With MyBase.xmlDoc
                _ndReturnCode = .CreateElement("ReturnCode")
                _ndPassword = .CreateElement("Password")
                _ndUserName = .CreateElement("UserName")
                _ndEmailSentToUser = .CreateElement("EmailSentToUser")
            End With

            'Populate the nodes
            _ndReturnCode.InnerText = drStatus("ReturnCode").ToString().Trim()
            If _doTokenHashing Then
                _ndPassword.InnerText = "**ENCRYPTED**"
            Else
                _ndPassword.InnerText = dr("PasswordHint").ToString().Trim()
            End If
            _ndUserName.InnerText = dr("CustomerNumber").ToString().Trim()
            _ndEmailSentToUser.InnerText = SendForgottenPasswordEmail.ToString()

            'Set the xml nodes
            With _ndResponse
                .AppendChild(_ndReturnCode)
                .AppendChild(_ndPassword)
                .AppendChild(_ndUserName)
                .AppendChild(_ndEmailSentToUser)
            End With
        End Sub

        Protected Overrides Sub InsertBodyV1_1()
            Try
                With MyBase.xmlDoc
                    _ndVerifyPassword = .CreateElement("RetrievePassword")
                    _ndResponse = .CreateElement("Response")
                End With
                Dim def As New SupplynetDefaults(Settings.BusinessUnit, Settings.Company)
                _doTokenHashing = Utilities.CheckForDBNull_Boolean_DefaultFalse(def.GetDefault("HASH_PASSWORD"))
                createResponseSection11()

                'Populate the xml document
                With _ndVerifyPassword
                    .AppendChild(_ndResponse)
                End With

                Const c1 As String = "//"
                Const c2 As String = "/TransactionHeader"
                _ndHeader = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement())
                _ndHeaderRoot = MyBase.xmlDoc.SelectSingleNode(c1 & RootElement() & c2)
                _ndHeader.InsertAfter(_ndVerifyPassword, _ndHeaderRoot)

                'Here we call a model builder to generate the forgotten password for the user.
                If SendForgottenPasswordEmail Then
                    Dim inputModel As New ForgottenPasswordInputModel
                    Dim viewModel As New ForgottenPasswordViewModel
                    Dim forgottenPasswordBuilder As New PasswordModelBuilders

                    inputModel.Source = "S"
                    inputModel.CustomerNumber = ResultDataSet.Tables("CustomerResults").Rows(0)("CustomerNumber").ToString().Trim()
                    inputModel.EmailAddress = ResultDataSet.Tables("CustomerResults").Rows(0)("EmailAddress").ToString().Trim()
                    inputModel.Mode = "Response"
                    inputModel.DoTokenHashing = _doTokenHashing
                    viewModel = forgottenPasswordBuilder.ForgottenPassword(inputModel)
                    If viewModel.Error IsNot Nothing AndAlso viewModel.Error.HasError Then
                        Throw New Exception(viewModel.Error.ErrorMessage)
                    End If
                End If
            Catch ex As Exception
                Const strError As String = "Failed to create the response xml"
                With Err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = strError
                    .ErrorNumber = "TTPRSRP-01"
                    .HasError = True
                End With
            End Try
        End Sub

        Private Sub createResponseSection11()
            Dim dr, drStatus As DataRow

            'Read the values for the response section
            _dtStatusResults = ResultDataSet.Tables("StatusResults")
            _dtRetrievePasswordResults = ResultDataSet.Tables("CustomerResults")
            drStatus = _dtStatusResults.Rows(0)
            dr = _dtRetrievePasswordResults.Rows(0)

            'Create the response xml nodes
            With MyBase.xmlDoc
                _ndReturnCode = .CreateElement("ReturnCode")
                _ndPassword = .CreateElement("Password")
                _ndUserName = .CreateElement("UserName")
                _ndEmailSentToUser = .CreateElement("EmailSentToUser")
                _ndEncryptedPassword = .CreateElement("EncryptedPassword")
            End With

            'Populate the nodes
            _ndReturnCode.InnerText = drStatus("ReturnCode").ToString().Trim()
            _ndPassword.InnerText = "**ENCRYPTED**"
            _ndUserName.InnerText = dr("CustomerNumber").ToString().Trim()
            _ndEmailSentToUser.InnerText = SendForgottenPasswordEmail.ToString()
            _ndEncryptedPassword.InnerText = dr("EncryptedPassword").ToString().Trim()

            'Set the xml nodes
            With _ndResponse
                .AppendChild(_ndReturnCode)
                .AppendChild(_ndPassword)
                .AppendChild(_ndUserName)
                .AppendChild(_ndEmailSentToUser)
                .AppendChild(_ndEncryptedPassword)
            End With
        End Sub
    End Class

End Namespace