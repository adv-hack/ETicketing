Imports Microsoft.VisualBasic
Imports System
Imports System.Xml
Imports Talent.Common
'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to import generic xml data
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       © CS Group 2007             All rights reserved.
'
'       Error Number Code base      TTPRXML- 
'                                    
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'-------------------------------------------------------------------------------------------------
'   Process
'   -------
'   1.	Check logon, password being used, this is the same as all the web services.
'   2.	Load the new table [tbl_xml_field_xref] in to the program.
'   3.	Put the XML data to a data entity [DEXmlLoad].
'   4.	Validate the XML data using the [tbl_xml_field_xref] constraints; also populate the extra fields 
'       with the data entity [DEXmlLoad].
'   5.	Accept or reject the XML document.
'   6.	If the xml document is rejected, create a response xml document and pass back to the user.
'   7.	If the xml document is accepted, sequentially pass through the data entity creating dynamic sequel 
'       insert statements and post each to the front end database. 
'       a.	The email address given is used as the Login identifier for all user table
'       b.	If a record is posted to the user table the process automatically creates an [authorized_users 
'           record] with a password of password.
'       c.	The [LAST_LOGIN_DATE] field on the [authorized_users record] is set to [191111] 
'           (that is 11th November 1911). This is so that the new users can be extracted and posted to the 
'           back end database.
'   8.	Extract all new users and post one by one to the backend database using the process from the 
'       registration form [TalentCustomer.SetCustomer] 
'   9.	Set the [LAST_LOGIN_DATE] to today’s date.
'   10.	Create a response xml document and pass back to the user, only include data lines that have 
'       Active fields, as per the [ACTIVE] flag on the [tbl_xml_field_xref] table.
'-------------------------------------------------------------------------------------------------
'   General XML Details     These are ID's 1-99
'   Customer Details        These are ID's 100-199
'   Contact Details         These are ID's 200-299
'   Attributes              These are ID's 500-599.
'                               Attributes are matched against TALENT by name and associated 
'                               against the Client/Contact as defined in T#AT
'                               Attributes can only be Added or Deleted.
'   Actions                 These are ID's 600-699
'                               Actions can only be ADDED
'   Notes                   Note are not currently supported
'                               Notes can only be ADDED
'   Brochure Requests       These are catered for by using the Contact Caption fields in 
'                               the Contact Section
'
'-------------------------------------------------------------------------------------------------
Namespace Talent.TradingPortal

    Public Class XmlLoadRequest
        Inherits XmlRequest

        Private _xml As Collection

        Public Property Xml() As Collection
            Get
                Return _xml
            End Get
            Set(ByVal value As Collection)
                _xml = value
            End Set
        End Property
        Public Overrides Function AccessDatabase(ByVal xmlResp As XmlResponse) As XmlResponse
            '-------------------------------------------------------------------------------------
            '   Dim xmlAction As XmlInvoiceResponse = CType(xmlResp, XmlInvoiceResponse)
            Dim xmlAction As XmlLoadResponse = CType(xmlResp, XmlLoadResponse)
            Dim xmlLoad As New TalentXmlLoad()
            Dim err As ErrorObj = Nothing
            '
            Select Case MyBase.DocumentVersion
                Case Is = "1.0"
                    err = LoadXmlV1()

            End Select
            '-------------------------------------------------------------------------------------
            '   Place the Request
            '
            If Not err.HasError Then
                Settings.BackOfficeConnectionString = ConfigurationManager.ConnectionStrings("TALENTCRM").ToString
                With xmlLoad
                    '.BusinessUnit = ConfigurationManager.AppSettings("DefaultBusinessUnit")
                    .BusinessUnit = Settings.BusinessUnit
                    .PartnerCode = Settings.Company
                    .Xml = Xml
                    .Settings = Settings
                    err = .LoadData
                End With
            End If
            '-------------------------------------------------------------------------------------
            With xmlAction
                .Err = err
                .ResultDataSet = xmlLoad.ResultDataSet
                .CreateResponse()
            End With
            Return CType(xmlAction, XmlResponse)
        End Function
        Private Function LoadXmlV1() As ErrorObj
            Const ModuleName As String = "LoadXmlV1"
            Dim err As New ErrorObj
            '-------------------------------------------------------------------------------------
            '   We have the full XMl document held in xmlDoc. Putting all the data found into Data 
            '   Entities 
            '
            Dim Node1 As XmlNode
            Dim Node2 As XmlNode
            Dim de As New DEXmlLoad
            Xml = New Collection
            '-------------------------------------------------------------------------------------
            Try
                For Each Node1 In xmlDoc.SelectSingleNode("//XmlLoadRequest").ChildNodes
                    Select Case Node1.Name
                        Case Is = "ecrminput"
                            For Each Node2 In Node1.ChildNodes
                                de = New DEXmlLoad
                                With de
                                    .EcrminputId = Node1.Attributes("id").Value
                                    .XmlId = Node2.Attributes("id").Value
                                    .XmlDescription = Node2.Attributes("title").Value
                                    .XmlValue = Node2.InnerText
                                End With
                                Xml.Add(de)
                            Next Node2
                    End Select
                Next Node1
            Catch ex As Exception
                With err
                    .ErrorMessage = ex.Message
                    .ErrorStatus = ModuleName & " Error"
                    .ErrorNumber = "TTPRXML-01"
                    .HasError = True
                End With
            End Try
            Return err
        End Function

    End Class

End Namespace