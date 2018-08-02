Imports Microsoft.VisualBasic
Imports System.data
Imports System.Web
Imports Talent.Common
<Serializable()> _
Public Class TalentDeliveryCharge
    Inherits TalentBase

    Private _deliveryCharges As DEDeliveryCharges
    Public Property DeliveryCharges() As DEDeliveryCharges
        Get
            Return _deliveryCharges
        End Get
        Set(ByVal value As DEDeliveryCharges)
            _deliveryCharges = value
        End Set
    End Property


    Private _TotalValueForComparison As Decimal
    Public Property TotalValueForComparison() As Decimal
        Get
            Return _TotalValueForComparison
        End Get
        Set(ByVal value As Decimal)
            _TotalValueForComparison = value
        End Set
    End Property


    Private _freeDefaultDel As Boolean
    Public Property QualifiesForFreeDefaultDelivery() As Boolean
        Get
            Return _freeDefaultDel
        End Get
        Set(ByVal value As Boolean)
            _freeDefaultDel = value
        End Set
    End Property

    Public Property TotalWeightForComparison() As Decimal = 0
    Public Property DeliveryCountryCode() As String = String.Empty
    Public Property DeliveryCountryName() As String = String.Empty

    Public Sub New(ByVal valueForComparison As Decimal, ByVal freeDefaultDelivery As Boolean)
        MyBase.New()
        _deliveryCharges = New DEDeliveryCharges
        Me.TotalValueForComparison = valueForComparison
        Me.QualifiesForFreeDefaultDelivery = freeDefaultDelivery
    End Sub

    Public Sub New(ByVal valueForComparison As Decimal, ByVal freeDefaultDelivery As Boolean, ByVal weightForComparison As Decimal, ByVal countryCodeToDeliver As String, ByVal countryNameToDeliver As String)
        MyBase.New()
        _deliveryCharges = New DEDeliveryCharges
        Me.TotalValueForComparison = valueForComparison
        Me.QualifiesForFreeDefaultDelivery = freeDefaultDelivery
        Me.TotalWeightForComparison = weightForComparison
        Me.DeliveryCountryCode = countryCodeToDeliver
        Me.DeliveryCountryName = countryNameToDeliver
    End Sub

    Public Function GetDeliveryOptions() As ErrorObj
        Const ModuleName As String = "GetDeliveryOptions"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj

        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & Settings.Partner & Settings.Company & Settings.Language
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Me.DeliveryCharges = CType(HttpContext.Current.Cache.Item(cacheKey), DEDeliveryCharges)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbDel As New DBDeliveryCharge
            With dbDel
                .Settings = Me.Settings
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    If .ResultDataSet.Tables.Count > 0 Then
                        Dim delOptions As New DataTable
                        delOptions = .ResultDataSet.Tables(0)
                        If delOptions.Rows.Count > 0 Then
                            Me.DeliveryCharges = PopulateDeliveryCharges(delOptions)
                            Me.AddItemToCache(cacheKey, Me.DeliveryCharges, Settings)
                        End If
                    End If
                End If
            End With
        End If

        If Me.DeliveryCharges.DeliveryCharges.Count > 0 Then
            FilterOutUnwantedValues(Me.DeliveryCharges.DeliveryCharges)
        End If

        Return err
    End Function

    Public Function GetDeliveryOptionsByWeight() As ErrorObj
        Const ModuleName As String = "GetDeliveryOptionsByWeight"
        Settings.ModuleName = ModuleName
        Dim err As New ErrorObj

        '--------------------------------------------------------------------------
        Dim cacheKey As String = ModuleName & Settings.BusinessUnit & Settings.Partner & Settings.Company & Settings.Language
        If Settings.Cacheing AndAlso Talent.Common.TalentThreadSafe.ItemIsInCache(cacheKey) Then
            Me.DeliveryCharges = CType(HttpContext.Current.Cache.Item(cacheKey), DEDeliveryCharges)
        Else
            Me.GetConnectionDetails(Settings.BusinessUnit, "", ModuleName)
            Dim dbDel As New DBDeliveryCharge
            With dbDel
                .Settings = Me.Settings
                err = .AccessDatabase
                If Not err.HasError And Not .ResultDataSet Is Nothing Then
                    If .ResultDataSet.Tables.Count > 0 Then
                        Dim delOptions As New DataTable
                        delOptions = .ResultDataSet.Tables(0)
                        If delOptions.Rows.Count > 0 Then
                            Me.DeliveryCharges = PopulateDeliveryCharges(delOptions)
                            Me.AddItemToCache(cacheKey, Me.DeliveryCharges, Settings)
                        End If
                    End If
                End If
            End With
        End If

        'get only the delivery charges related to country
        Dim countryBasedDeliveryCharges As New DEDeliveryCharges
        For chargeIndex As Integer = 0 To Me.DeliveryCharges.DeliveryCharges.Count - 1
            If Me.DeliveryCharges.DeliveryCharges.Item(chargeIndex).COUNTRY_CODE = DeliveryCountryCode OrElse Me.DeliveryCharges.DeliveryCharges.Item(chargeIndex).COUNTRY_DESCRIPTION = DeliveryCountryName Then
                countryBasedDeliveryCharges.DeliveryCharges.Add(Me.DeliveryCharges.DeliveryCharges.Item(chargeIndex))
            End If
        Next
        Me.DeliveryCharges = countryBasedDeliveryCharges
        If Me.DeliveryCharges.DeliveryCharges.Count > 0 Then
            FilterOutUnwantedValuesByWeight(Me.DeliveryCharges.DeliveryCharges)
        End If

        Return err
    End Function

    Protected Function FilterOutUnwantedValuesByWeight_first(ByVal charges As Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)) As Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)

        'Get a list of all current level charges that do not have child nodes
        Dim currentLevelCharges As New Generic.List(Of String)
        'For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
        '    If Not dc.HasChildNodes AndAlso Not currentLevelCharges.Contains(dc.DELIVERY_TYPE) Then
        '        currentLevelCharges.Add(dc.DELIVERY_TYPE)
        '    End If
        'Next

        ''Filter the top level types that don't have child nodes
        'Dim validCharges As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        'For Each type As String In currentLevelCharges
        '    Dim validCharge As New DEDeliveryCharges.DEDeliveryCharge
        '    Dim valueSelected As Boolean = False
        '    For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
        '        If type = dc.DELIVERY_TYPE Then

        '            If dc.UPPER_BREAK_WEIGHT = Me.TotalWeightForComparison Then
        '                validCharge = dc
        '                valueSelected = True
        '                Exit For 'If the upper break is equal to the value then we do not need to search further

        '            ElseIf dc.UPPER_BREAK_WEIGHT > Me.TotalWeightForComparison Then

        '                If valueSelected Then 'If there is a value to compare then choose the lowest
        '                    If dc.UPPER_BREAK_WEIGHT < validCharge.UPPER_BREAK_WEIGHT Then
        '                        validCharge = dc
        '                        valueSelected = True
        '                    End If
        '                Else 'Otherwise make this the value to compare
        '                    validCharge = dc
        '                    valueSelected = True
        '                End If

        '            Else 'The upper break is lower than the value for comparison... check for the greater flag

        '                If dc.GREATER Then
        '                    validCharge = dc
        '                    valueSelected = True
        '                End If

        '            End If
        '        End If
        '    Next

        '    'If a valid charge was found, add it to the list
        '    If valueSelected Then
        '        validCharges.Add(validCharge)
        '    End If
        'Next

        ''Remove the invalid charges
        'Dim chargesToRemove As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        'If validCharges.Count > 0 Then
        '    For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
        '        'If the charge does not have child nodes and is not in the list of 
        '        'valid charges, add it to the remove list
        '        If Not dc.HasChildNodes AndAlso Not validCharges.Contains(dc) Then
        '            chargesToRemove.Add(dc)
        '        End If
        '    Next

        '    'Remove each charge in the remove list
        '    For Each ctr As DEDeliveryCharges.DEDeliveryCharge In chargesToRemove
        '        charges.Remove(ctr)
        '    Next
        'End If


        ''Re-Call self for any charges that have child nodes
        'For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
        '    If dc.HasChildNodes Then
        '        FilterOutUnwantedValuesByWeight(dc.ChildNodes)
        '    End If
        'Next

        Return charges

    End Function

    Protected Sub FilterOutUnwantedValuesByWeight(ByVal charges As Generic.List(Of DEDeliveryCharges.DEDeliveryCharge))

        'Get a list of all current level charges that do not have child nodes
        Dim currentLevelCharges As New Generic.List(Of String)
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
            If Not dc.HasChildNodes AndAlso Not currentLevelCharges.Contains(dc.DELIVERY_TYPE) Then
                currentLevelCharges.Add(dc.DELIVERY_TYPE)
            End If
        Next

        'Filter the top level types that don't have child nodes for upper break mode 2 (weight)
        Dim validCharges As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        For Each type As String In currentLevelCharges
            Dim validCharge As New DEDeliveryCharges.DEDeliveryCharge
            Dim valueSelected As Boolean = False
            For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
                If (type = dc.DELIVERY_TYPE AndAlso dc.UPPER_BREAK_MODE = 1) Then

                    If dc.UPPER_BREAK = Me.TotalWeightForComparison Then
                        validCharge = dc
                        valueSelected = True
                        Exit For 'If the upper break is equal to the value then we do not need to search further

                    ElseIf dc.UPPER_BREAK > Me.TotalWeightForComparison Then

                        If valueSelected Then 'If there is a value to compare then choose the lowest
                            If dc.UPPER_BREAK < validCharge.UPPER_BREAK Then
                                validCharge = dc
                                valueSelected = True
                            End If
                        Else 'Otherwise make this the value to compare
                            validCharge = dc
                            valueSelected = True
                        End If

                    Else 'The upper break is lower than the value for comparison... check for the greater flag

                        If dc.GREATER Then
                            validCharge = dc
                            valueSelected = True
                        End If

                    End If
                End If
            Next
            'Filter the top level types that don't have child nodes for upper break mode 0 (price/unit)
            For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
                If (type = dc.DELIVERY_TYPE AndAlso dc.UPPER_BREAK_MODE = 0) Then
                    If dc.GREATER Then
                        If Me.TotalValueForComparison >= dc.UPPER_BREAK Then
                            validCharge = dc
                            valueSelected = True
                            Exit For
                        End If
                    Else
                        If Me.TotalValueForComparison <= dc.UPPER_BREAK Then
                            validCharge = dc
                            valueSelected = True
                            Exit For
                        End If
                    End If
                End If
            Next

            'If a valid charge was found, add it to the list
            If valueSelected Then
                validCharges.Add(validCharge)
            End If
        Next

        'Remove the invalid charges
        Dim chargesToRemove As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        If validCharges.Count > 0 Then
            For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
                'If the charge does not have child nodes and is not in the list of 
                'valid charges, add it to the remove list
                If Not dc.HasChildNodes AndAlso Not validCharges.Contains(dc) Then
                    chargesToRemove.Add(dc)
                End If
            Next

            'Remove each charge in the remove list
            For Each ctr As DEDeliveryCharges.DEDeliveryCharge In chargesToRemove
                'make sure object remove is matching with validcharges
                For Each validcharge As DEDeliveryCharges.DEDeliveryCharge In validCharges
                    If Not validcharge.Equals(ctr) Then
                        charges.Remove(ctr)
                    End If
                Next
            Next
        End If


        'Re-Call self for any charges that have child nodes
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
            If dc.HasChildNodes Then
                FilterOutUnwantedValuesByWeight(dc.ChildNodes)
            End If
        Next

    End Sub

    Protected Sub FilterOutUnwantedValues(ByVal charges As Generic.List(Of DEDeliveryCharges.DEDeliveryCharge))

        'Get a list of all current level charges that do not have child nodes
        Dim currentLevelCharges As New Generic.List(Of String)
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
            If Not dc.HasChildNodes AndAlso Not currentLevelCharges.Contains(dc.DELIVERY_TYPE) Then
                currentLevelCharges.Add(dc.DELIVERY_TYPE)
            End If
        Next

        'Filter the top level types that don't have child nodes
        Dim validCharges As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        For Each type As String In currentLevelCharges
            Dim validCharge As New DEDeliveryCharges.DEDeliveryCharge
            Dim valueSelected As Boolean = False
            For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
                If type = dc.DELIVERY_TYPE Then

                    If dc.UPPER_BREAK = Me.TotalValueForComparison Then
                        validCharge = dc
                        valueSelected = True
                        Exit For 'If the upper break is equal to the value then we do not need to search further

                    ElseIf dc.UPPER_BREAK > Me.TotalValueForComparison Then

                        If valueSelected Then 'If there is a value to compare then choose the lowest
                            If dc.UPPER_BREAK < validCharge.UPPER_BREAK Then
                                validCharge = dc
                                valueSelected = True
                            End If
                        Else 'Otherwise make this the value to compare
                            validCharge = dc
                            valueSelected = True
                        End If

                    Else 'The upper break is lower than the value for comparison... check for the greater flag

                        If dc.GREATER Then
                            validCharge = dc
                            valueSelected = True
                        End If

                    End If
                End If
            Next

            'If a valid charge was found, add it to the list
            If valueSelected Then
                validCharges.Add(validCharge)
            End If
        Next

        'Remove the invalid charges
        Dim chargesToRemove As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        If validCharges.Count > 0 Then
            For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
                'If the charge does not have child nodes and is not in the list of 
                'valid charges, add it to the remove list
                If Not dc.HasChildNodes AndAlso Not validCharges.Contains(dc) Then
                    chargesToRemove.Add(dc)
                End If
            Next

            'Remove each charge in the remove list
            For Each ctr As DEDeliveryCharges.DEDeliveryCharge In chargesToRemove
                charges.Remove(ctr)
            Next
        End If


        'Re-Call self for any charges that have child nodes
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In charges
            If dc.HasChildNodes Then
                FilterOutUnwantedValues(dc.ChildNodes)
            End If
        Next

    End Sub

    Protected Function PopulateDeliveryCharges(ByVal dbCharges As DataTable) As DEDeliveryCharges
        Dim charges As New DEDeliveryCharges
        Dim properties As New ArrayList
        Dim found As Boolean = False
        Dim notAdded As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)
        Dim myTempCharges As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)

        properties = Utilities.GetPropertyNames(New DEDeliveryCharges.DEDeliveryCharge)

        'Populate a collection with all items
        For Each rw As DataRow In dbCharges.Rows
            Dim dc As DEDeliveryCharges.DEDeliveryCharge
            dc = New DEDeliveryCharges.DEDeliveryCharge
            dc = Utilities.PopulateProperties(properties, rw, dc)
            If dc.IS_DEFAULT AndAlso Me.QualifiesForFreeDefaultDelivery Then
                dc.GROSS_VALUE = 0
                dc.NET_VALUE = 0
                dc.TAX_VALUE = 0
            End If
            myTempCharges.Add(dc)
        Next

        'Add top level nodes
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In myTempCharges
            If String.IsNullOrEmpty(dc.DELIVERY_PARENT) Then
                charges.DeliveryCharges.Add(dc)
            End If
        Next

        'Attempt to add all other items
        For Each dc As DEDeliveryCharges.DEDeliveryCharge In myTempCharges
            If String.IsNullOrEmpty(dc.DELIVERY_PARENT) Then
                'Do Nothing
            Else
                found = False
                For Each parentDc As DEDeliveryCharges.DEDeliveryCharge In myTempCharges
                    If TraverseCharges_AddNodeToParent(dc, parentDc) Then
                        found = True
                        Exit For
                    End If
                Next
                If Not found Then
                    'Only add the item if it has not been added previously
                    If Not notAdded.Contains(dc) Then
                        notAdded.Add(dc)
                    End If
                End If
            End If
        Next

        ' AG  - 2013.10.22 - Temp commented out......TODO
        ''If some items were not added because their parent was not in place
        ''attempt to add them now
        'If notAdded.Count > 0 Then
        '    charges = ReTryNotAddedItems(notAdded, charges)
        'End If

        Return charges
    End Function

    Protected Function ReTryNotAddedItems(ByVal notAdded As Generic.List(Of DEDeliveryCharges.DEDeliveryCharge), ByVal charges As DEDeliveryCharges) As DEDeliveryCharges

        Dim itemsToRemove As New Generic.List(Of DEDeliveryCharges.DEDeliveryCharge)

        For Each dc As DEDeliveryCharges.DEDeliveryCharge In notAdded
            If String.IsNullOrEmpty(dc.DELIVERY_PARENT) Then
                charges.DeliveryCharges.Add(dc)
                notAdded.Remove(dc)
            Else
                For Each parentDc As DEDeliveryCharges.DEDeliveryCharge In charges.DeliveryCharges
                    If TraverseCharges_AddNodeToParent(dc, parentDc) Then
                        itemsToRemove.Add(dc)
                        Exit For
                    End If
                Next
            End If
        Next

        For Each dc As DEDeliveryCharges.DEDeliveryCharge In itemsToRemove
            notAdded.Remove(dc)
        Next

        If notAdded.Count > 0 Then
            charges = ReTryNotAddedItems(notAdded, charges)
        End If

        Return charges
    End Function

    Protected Function TraverseCharges_AddNodeToParent(ByVal dc As DEDeliveryCharges.DEDeliveryCharge, ByVal parentDc As DEDeliveryCharges.DEDeliveryCharge) As Boolean

        If parentDc.DELIVERY_TYPE = dc.DELIVERY_PARENT Then
            parentDc.ChildNodes.Add(dc)
            Return True
        Else
            If parentDc.HasChildNodes Then
                For Each childDc As DEDeliveryCharges.DEDeliveryCharge In parentDc.ChildNodes
                    If TraverseCharges_AddNodeToParent(dc, childDc) Then 'NOTE: it is the CHILD node that is passed
                        Return True
                    End If
                Next
            Else
                Return False
            End If
        End If
    End Function


End Class
