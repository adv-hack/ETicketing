Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Text
Imports System.Web
Imports Talent.Common.Utilities
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with various requests
'                                   such as checking connections

'       Date                        10/07/08
'
'       Author                      Ben Ford
'
'       @ CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACTAUT - 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class TalentStadium
    Inherits TalentBase

    Public Function GetSeating(ByVal productCode As String, ByVal standCode As String, ByVal areaCode As String) As String
        Dim seating As New StringBuilder
        Dim product As New TalentProduct
        Dim err As New ErrorObj
        product.Settings = Settings
        product.De.ProductCode = productCode
        product.De.StandCode = standCode.ToUpper()
        product.De.AreaCode = areaCode.ToUpper()
        product.De.Src = Settings.OriginatingSourceCode
        err = product.ProductSeatAvailability()

        product.ResultDataSet = Nothing

        err = product.ProductSeatNumbers()

        seating.Append("<seats>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0001' a='A' x='30' y='25' sc='1' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0002' a='A' x='55' y='25' sc='2' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0003' a='A' x='80' y='25' sc='3' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0004' a='A' x='105' y='25' sc='4' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0005' a='A' x='130' y='25' sc='5' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0006' a='A' x='155' y='25' sc='6' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0007' a='A' x='180' y='25' sc='7' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='D' n='0008' a='A' x='205' y='25' sc='8' slt='0'/>")

        seating.Append("<s v='0' vDesc='0' r='C' n='0001' a='A' x='30' y='50' sc='1' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0002' a='A' x='55' y='50' sc='2' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0003' a='A' x='80' y='50' sc='3' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0004' a='A' x='105' y='50' sc='4' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0005' a='A' x='130' y='50' sc='5' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0006' a='A' x='155' y='50' sc='6' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0007' a='A' x='180' y='50' sc='7' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='C' n='0008' a='A' x='205' y='50' sc='8' slt='0'/>")

        seating.Append("<s v='0' vDesc='0' r='B' n='0001' a='A' x='30' y='75' sc='1' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0002' a='A' x='55' y='75' sc='2' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0003' a='A' x='80' y='75' sc='3' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0004' a='A' x='105' y='75' sc='4' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0005' a='A' x='130' y='75' sc='5' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0006' a='A' x='155' y='75' sc='6' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0007' a='A' x='180' y='75' sc='7' slt='0'/>")
        seating.Append("<s v='0' vDesc='0' r='B' n='0008' a='A' x='205' y='75' sc='8' slt='0'/>")
        seating.Append("</seats>")

        Return seating.ToString()
    End Function

End Class


