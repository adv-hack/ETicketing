Namespace TalentBasketFees
    <Serializable()> _
    Friend Class TalentBasketFeesVariable
        Inherits TalentBasketFees

        Private Const SOURCECLASS As String = "TalentBasketFeesVariable"

        Private _validFeesVariableForBasket As New List(Of DEFees)

        Public Property VariableFeesList As List(Of DEFees) = Nothing

        Public Function ProcessVariableFeesForBasket() As ErrorObj
            Dim errObj As New ErrorObj
            Try
                PopulateValidFeesVariable()
                ProcessValidVariableFeesForBasket()
            Catch ex As Exception
                errObj.HasError = True
                errObj.ErrorNumber = "TCTBFVAR-001"
                errObj.ErrorStatus = "While processing variable fees for basket"
                LogError(SOURCECLASS, "", errObj.ErrorNumber, errObj.ErrorStatus, ex)
            End Try
            Return errObj
        End Function

        Private Sub PopulateValidFeesVariable()

            'Department
            For itemIndex As Integer = 0 To VariableFeesList.Count - 1
                'If Settings.AgentEntity.Department = VariableFeesList(itemIndex).FeeDepartment Then
                '    AddValidFeesVariableForBasket(VariableFeesList(itemIndex))
                'End If
                If Me.Department = VariableFeesList(itemIndex).FeeDepartment Then
                    AddValidFeesVariableForBasket(VariableFeesList(itemIndex))
                End If
            Next

            'General
            For itemIndex As Integer = 0 To VariableFeesList.Count - 1
                If VariableFeesList(itemIndex).FeeDepartment.Trim = "" Then
                    AddValidFeesVariableForBasket(VariableFeesList(itemIndex))
                End If
            Next

        End Sub

        Private Sub AddValidFeesVariableForBasket(ByVal feeEntity As DEFees)
            Dim canAddThisFeeEntity As Boolean = True
            If _validFeesVariableForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesVariableForBasket.Count - 1
                    If feeEntity.Equal(_validFeesVariableForBasket(itemIndex)) Then
                        canAddThisFeeEntity = False
                        Exit For
                    End If
                Next
            End If
            If canAddThisFeeEntity Then
                _validFeesVariableForBasket.Add(feeEntity)
            End If
        End Sub

        Private Sub ProcessValidVariableFeesForBasket()
            If _validFeesVariableForBasket IsNot Nothing AndAlso _validFeesVariableForBasket.Count > 0 Then
                For itemIndex As Integer = 0 To _validFeesVariableForBasket.Count - 1
                    Dim basketFeesEntity As DEBasketFees = CastDEFeesToDEBasketFees(_validFeesVariableForBasket(itemIndex))
                    'now override required properties
                    basketFeesEntity.BasketHeaderID = BasketEntity.Basket_Header_ID
                    basketFeesEntity.IsTransactional = True
                    BasketFeesEntityList.Add(basketFeesEntity)
                Next
            End If
        End Sub

    End Class
End Namespace

