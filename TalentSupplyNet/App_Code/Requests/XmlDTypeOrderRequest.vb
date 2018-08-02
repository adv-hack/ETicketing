Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with DType Order Requests
'
'       Date                        9th Nov 2006
'
'       Author                      Andy White  
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRQOR- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlDTypeOrderRequest
        Inherits XmlRequest

        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            '------------------------------------------------------------------------
            '       XmlDTypeOrderResponse is identical to XmlOrderResponse
            '
            '       So Order.DTypeOrderRequest passes control to XmlOrderResponse.vb
            '
            '       Thus save duplication
            '------------------------------------------------------------------------
            Dim xmlAction As XmlOrderResponse = CType(xmlResp, XmlOrderResponse)
            Dim err As ErrorObj = Nothing
            '
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '--------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                Dim order As New TalentDTypeOrder
                With order
                    .Dep = Dep
                    .Settings = Settings
                    err = .Create
                End With
            End If
            '--------------------------------------------------------------------
            With xmlAction
                .Err = err
                .CreateResponse()
            End With

            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '--------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1, Node2, Node3, Node4 As XmlNode
            Dim deos As New DeOrders                ' DeOrderHeader, DEOrderInfo  as collections
            Dim deoh As New DeOrderHeader           ' Items and DEAddress as Collection
            Dim dead1 As New DeAddress              ' Single Line Item
            Dim dead2 As New DeAddress              ' Single Line Item
            Dim dead3 As New DeAddress              ' Single Line Item
            Dim deoi As New DEOrderInfo             ' DEProductLines, DECommentLines  as collections
            Dim depr As DeProductLines              ' Multiple Line Item
            Dim decl As DeCommentLines              ' Multiple Line Item
            Dep = New DEOrder
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//DTypeOrderRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "TransactionHeader"
                            Dep.CollDETrans.Add(Extract_TransactionHeader(Node1))
                        Case Is = "OrderHeaderInformation"
                            ''Dep.CollDEOrders.Add(Extract_OrderHeader(Node1))
                            With deoh
                                For Each Node2 In Node1.ChildNodes
                                    Select Case Node2.Name
                                        Case Is = "BillToSuffix"
                                            .BillToSuffix = Node2.InnerText
                                        Case Is = "AddressingInformation"
                                            '-----------------------------------------------------------------------
                                            '       <AddressingInformation>
                                            '           <CustomerPO>CustomerPO_1</CustomerPO>
                                            '           <ShipToAttention>Mrs Jones</ShipToAttention>
                                            '           <EndUserPO>EndUserPO_1</EndUserPO>
                                            '-----------------------------------------------------------------------
                                            For Each Node3 In Node2.ChildNodes
                                                '
                                                Select Case Node3.Name
                                                    Case Is = "CustomerPO" : .CustomerPO = Node3.InnerText
                                                    Case Is = "ShipToAttention" : dead1.ContactName = Node3.InnerText
                                                    Case Is = "EndUserPO" : .EndUserPO = Node3.InnerText
                                                    Case Is = "ShipTo"
                                                        '----------------------------------------------------------------------
                                                        '       <ShipTo>
                                                        '           <Address>
                                                        '               <ShipToAddress1>Red House</ShipToAddress1>
                                                        '               <ShipToAddress2>55 The Street</ShipToAddress2>
                                                        '               <ShipToAddress3>Gosford</ShipToAddress3>
                                                        '               <ShipToCity>London</ShipToCity>
                                                        '               <ShipToProvince>UK</ShipToProvince>
                                                        '               <ShipToPostalCode>SW1 3IM</ShipToPostalCode>
                                                        '           </Address>
                                                        '       </ShipTo>
                                                        '----------------------------------------------------------------------
                                                        With dead1
                                                            .Category = Node3.Name
                                                            For Each Node4 In Node3.SelectSingleNode("Address").ChildNodes
                                                                Select Case Node4.Name
                                                                    '-----------------------------------------------------------
                                                                    '   Shipping Address info
                                                                    '
                                                                    Case Is = "ShipToAttention" : .ContactName = Node4.InnerText
                                                                    Case Is = "ShipToAddress1" : .Line1 = Node4.InnerText
                                                                    Case Is = "ShipToAddress2" : .Line2 = Node4.InnerText
                                                                    Case Is = "ShipToAddress3" : .Line3 = Node4.InnerText
                                                                    Case Is = "ShipToCity" : .City = Node4.InnerText
                                                                    Case Is = "ShipToProvince" : .Province = Node4.InnerText
                                                                    Case Is = "ShipToPostalCode" : .PostalCode = Node4.InnerText
                                                                End Select
                                                            Next Node4
                                                        End With
                                                        deoh.CollDEAddress.Add(dead1)
                                                End Select
                                            Next Node3
                                        Case Is = "ProcessingOptions"
                                            '-------------------------------------------------------------------------------
                                            '   <ProcessingOptions>
                                            '       <CarrierCode>CT</CarrierCode>
                                            '       <ShipmentOptions>
                                            '           <BackOrderFlag>Y</BackOrderFlag>
                                            '           <ShipFromBranches>20</ShipFromBranches>
                                            '       </ShipmentOptions>
                                            '   </ProcessingOptions>
                                            '-------------------------------------------------------------------------------
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CarrierCode" : .CarrierCode = Node3.InnerText
                                                    Case Is = "AutoRelease" : .AutoRelease = Node3.InnerText
                                                    Case Is = "BackOrderFlag" : .BackOrderFlag = Node3.InnerText
                                                    Case Is = "ShipmentOptions"
                                                        For Each Node4 In Node3.ChildNodes
                                                            Select Case Node4.Name
                                                                Case Is = "BackOrderFlag" : .BackOrderFlag = Node3.InnerText
                                                                Case Is = "ShipFromBranches" : .ShipFromBranches = Node3.InnerText
                                                            End Select
                                                        Next Node4
                                                End Select
                                            Next Node3
                                        Case Is = "EndUserInformation"
                                            '------------------------------------------------------------------------------
                                            '   <EndUserInformation>
                                            '       <ContactName>Jacob Lyons</ContactName>
                                            '       <PhoneNumber>01604575550</PhoneNumber>
                                            '       <ExtensionNumber>33228</ExtensionNumber>
                                            '       <FaxNumber>01604575555</FaxNumber>
                                            '       <Address1>192 Linford Street</Address1>
                                            '       <Address2>Linslow</Address2>
                                            '       <City>Milton Keynes</City>
                                            '       <Province>UK</Province>
                                            '       <PostalCode>MK9 8GG</PostalCode>
                                            '       <EndUserCountryCode>UK</EndUserCountryCode>
                                            '       <CompanyName>Electron Micro</CompanyName>
                                            '       <VATNumber>123456</VATNumber>
                                            '       <AuthorizationNumber>A-123456</AuthorizationNumber>
                                            '       <PricingLevel>P</PricingLevel>
                                            '       <EmailAddress>jlyons@Emicro.com</EmailAddress>
                                            '   </EndUserInformation>
                                            '------------------------------------------------------------------------------
                                            With dead2
                                                .Category = Node2.Name
                                                For Each Node3 In Node2.ChildNodes
                                                    Select Case Node3.Name

                                                        Case Is = "ContactName" : .ContactName = Node3.InnerText
                                                        Case Is = "PhoneNumber" : .PhoneNumber = Node3.InnerText
                                                        Case Is = "ExtensionNumber" : .ExtensionNumber = Node3.InnerText
                                                        Case Is = "FaxNumber" : .FaxNumber = Node3.InnerText
                                                        Case Is = "Address1" : .Line1 = Node3.InnerText
                                                        Case Is = "Address2" : .Line2 = Node3.InnerText
                                                        Case Is = "City" : .City = Node3.InnerText
                                                        Case Is = "Province" : .Province = Node3.InnerText
                                                        Case Is = "PostalCode" : .PostalCode = Node3.InnerText
                                                        Case Is = "EndUserCountryCode" : .CountryCode = Node3.InnerText
                                                        Case Is = "CompanyName" : .CompanyName = Node3.InnerText
                                                        Case Is = "VATNumber" : .VATNumber = Node3.InnerText
                                                        Case Is = "AuthorizationNumber" : .AuthorizationNumber = Node3.InnerText
                                                        Case Is = "PricingLevel" : .PricingLevel = Node3.InnerText
                                                        Case Is = "EmailAddress" : .Email = Node3.InnerText

                                                    End Select
                                                Next Node3
                                            End With
                                            deoh.CollDEAddress.Add(dead2)
                                        Case Is = "ResellerInformation"
                                            '------------------------------------------------------------------------------
                                            '   <ResellerInformation>
                                            '       <SalesRepName>Frank Fletcher</SalesRepName>
                                            '       <ResellerContactName>Geraldine Duberry</ResellerContactName>
                                            '       <ResellerPhoneNumber>01922 533388</ResellerPhoneNumber>
                                            '       <ResellerFaxNumber>01922 533389</ResellerFaxNumber>
                                            '       <ResellerAddress1>Bank House</ResellerAddress1>
                                            '       <ResellerAddress2>Brompton</ResellerAddress2>
                                            '       <ResellerCity>Birmingham</ResellerCity>
                                            '       <ResellerProvince>UK</ResellerProvince>
                                            '       <ResellerPostalCode>BR2 4TT</ResellerPostalCode>
                                            '       <ResellerCompanyName>RightOnIT</ResellerCompanyName>
                                            '       <ResellerEmailAddress>gdub@rightonit.com</ResellerEmailAddress>
                                            '   </ResellerInformation>
                                            '------------------------------------------------------------------------------
                                            With dead3
                                                .Category = Node2.Name
                                                For Each Node3 In Node2.ChildNodes
                                                    Select Case Node3.Name
                                                        Case Is = "SalesRepName" : .RepName = Node3.InnerText
                                                        Case Is = "ResellerContactName" : .ContactName = Node3.InnerText
                                                        Case Is = "ResellerPhoneNumber" : .PhoneNumber = Node3.InnerText
                                                        Case Is = "ResellerFaxNumber" : .FaxNumber = Node3.InnerText
                                                        Case Is = "ResellerAddress1" : .Line1 = Node3.InnerText
                                                        Case Is = "ResellerAddress2" : .Line2 = Node3.InnerText
                                                        Case Is = "ResellerCity" : .City = Node3.InnerText
                                                        Case Is = "ResellerProvince" : .Province = Node3.InnerText
                                                        Case Is = "ResellerPostalCod" : .PostalCode = Node3.InnerText
                                                        Case Is = "ResellerCompanyName" : .CompanyName = Node3.InnerText
                                                        Case Is = "ResellerEmailAddress" : .Email = Node3.InnerText
                                                    End Select
                                                Next Node3
                                            End With
                                            deoh.CollDEAddress.Add(dead3)
                                    End Select
                                Next Node2
                            End With
                            deos.DEOrderHeader = deoh
                            '----------------------------------------------------------------------------------------------

                        Case Is = "OrderLineInformation"
                            For Each Node2 In Node1.ChildNodes
                                Select Case Node2.Name
                                    Case Is = "ProductLine"
                                        '----------------------------------------------------------------------------------
                                        '   <OrderLineInformation>
                                        '       <ProductLine>
                                        '           <SKU>Product Line 1</SKU>
                                        '           <Quantity>100</Quantity>
                                        '           <CustomerLineNumber />
                                        '       </ProductLine>
                                        '---------------------------------------------------------------------------------
                                        depr = New DeProductLines
                                        With depr
                                            For Each Node3 In Node2.ChildNodes
                                                .Category = "Insert"
                                                Select Case Node3.Name
                                                    Case Is = "SKU" : .SKU = Node3.InnerText
                                                    Case Is = "Quantity" : .Quantity = Node3.InnerText
                                                    Case Is = "CustomerLineNumber" : .CustomerLineNumber = Node3.InnerText
                                                End Select
                                            Next Node3
                                        End With
                                        deoi.CollDEProductLines.Add(depr)
                                        '---------------------------------------------------------------------------------
                                    Case Is = "CommentLine"
                                        decl = New DeCommentLines
                                        '---------------------------------------------------------------------------------
                                        '   <CommentLine>
                                        '       <CommentText>comment 1</CommentText>
                                        '   </CommentLine>
                                        '---------------------------------------------------------------------------------
                                        With decl
                                            For Each Node3 In Node2.ChildNodes
                                                Select Case Node3.Name
                                                    Case Is = "CommentText" : .CommentText = Node3.InnerText
                                                End Select
                                            Next Node3
                                        End With
                                        deoi.CollDECommentLines.Add(decl)
                                End Select

                            Next Node2
                            deos.DEOrderInfo = deoi
                            '----------------------------------------------------------------------------------------------
                        Case Is = "ShowDetail"
                            '
                            '  <ShowDetail>21</ShowDetail>
                            '----------------------------------------------------------------------------------------------
                            deos.ShowDetail = Node1.InnerText
                            '
                    End Select
                Next Node1
                Dep.CollDEOrders.Add(deos)
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRQOR-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace