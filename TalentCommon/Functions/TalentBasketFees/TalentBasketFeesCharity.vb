Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesCharity
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesCharity"

        Private _validFeesCharityForBasket As New List(Of DEFees)

        Public Property CharityFeesList As List(Of DEFees) = Nothing

        Public Function ProcessCharityFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesCharity()
                ProcessValidCharityFeesForBasket()
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFCHA-001"
                errObj.ErrorStatus = "While processing charity fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function


        Private Sub PopulateValidFeesCharity()

            'Department
            For itemIndex As Integer = 0 To CharityFeesList.Count - 1
                'If Settings.AgentEntity.Department = CharityFeesList(itemIndex).FeeDepartment Then
                '    AddValidFeesCharityForBasket(CharityFeesList(itemIndex))
                'End If
                If Me.Department = CharityFeesList(itemIndex).FeeDepartment Then
                    AddValidFeesCharityForBasket(CharityFeesList(itemIndex))
                End If
            Next

            'General
            For itemIndex As Integer = 0 To CharityFeesList.Count - 1
                If CharityFeesList(itemIndex).FeeDepartment.Trim = "" Then
                    AddValidFeesCharityForBasket(CharityFeesList(itemIndex))
                End If
            Next

        End Sub

        Private Sub AddValidFeesCharityForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesCharityForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesCharityForBasket.Count - 1
                    If feeEntity.Equal(_validFeesCharityForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesCharityForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidCharityFeesForBasket()
            If _validFeesCharityForBasket IsNot Nothing AndAlso _validFeesCharityForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesCharityForBasket.Count - 1
                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesCharityForBasket(itemIndex))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.IsTransactional = True
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

    End Class
End Namespace
