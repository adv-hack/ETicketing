Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports Talent.TradingPortal
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
'       Error Number Code base      TACDBXML- 
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
<WebService(Namespace:="http://localhost/TradingPortal")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class XmlLoad
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function XmlLoadRequest(ByVal loginId As String, _
                                    ByVal password As String, _
                                    ByVal company As String, _
                                    ByVal XMLString As String) As String
        Dim csgWS As New TalentWebService
        Const a1 As String = "XmlLoadRequest"
        Const a2 As String = "XmlLoadResponse"
        '
        csgWS.WebServiceName = a1               'Set the Web Service Name
        csgWS.ResponseName = a2

        XMLString = Replace(XMLString, "<csg>", "<XmlLoadRequest> <Version>1.0</Version>")
        XMLString = Replace(XMLString, "</csg>", "</XmlLoadRequest>")

        'And Response Name
        'Do the work!
        Dim xmlResponseString As String = csgWS.InvokeWebService(loginId, password, company, XMLString)
        Return xmlResponseString
        '
    End Function
   

End Class
