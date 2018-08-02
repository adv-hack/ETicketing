'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to import generic xml data
'
'       Date                        Mar 2007
'
'       Author                      Andy White
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      TACDEXML- 
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
'   9.	Set the [LAST_LOGIN_DATE] to today�s date.
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
<Serializable()> _
Public Class DEXmlLoad
    '--------------------------------------------------------------------------------------------------
    Private _ecrminputId As String = String.Empty
    Private _xmlId As String = String.Empty
    Private _xmlDescription As String = String.Empty
    Private _xmlValue As String = String.Empty

    Private _Active As Boolean = False
    Private _sqlTable As String = String.Empty
    Private _sqlField As String = String.Empty
    Private _sqlDescription As String = String.Empty
    Private _sqlLength As String = String.Empty
    Private _sqlType As String = String.Empty
    Private _sqlComments As String = String.Empty
    Private _sqlMandatory As String = String.Empty
    Private _sqlDefault As String = String.Empty

    Public Property EcrminputId() As String
        Get
            Return _ecrminputId
        End Get
        Set(ByVal value As String)
            _ecrminputId = value
        End Set
    End Property
    Public Property XmlId() As String
        Get
            Return _xmlId
        End Get
        Set(ByVal value As String)
            _xmlId = value
        End Set
    End Property
    Public Property XmlDescription() As String
        Get
            Return _xmlDescription
        End Get
        Set(ByVal value As String)
            _xmlDescription = value
        End Set
    End Property
    Public Property XmlValue() As String
        Get
            Return _xmlValue
        End Get
        Set(ByVal value As String)
            _xmlValue = value
        End Set
    End Property

    Public Property Active() As Boolean
        Get
            Return _Active
        End Get
        Set(ByVal value As Boolean)
            _Active = value
        End Set
    End Property
    Public Property SqlTable() As String
        Get
            Return _sqlTable
        End Get
        Set(ByVal value As String)
            _sqlTable = value
        End Set
    End Property
    Public Property SqlField() As String
        Get
            Return _sqlField
        End Get
        Set(ByVal value As String)
            _sqlField = value
        End Set
    End Property
    Public Property SqlDescription() As String
        Get
            Return _sqlDescription
        End Get
        Set(ByVal value As String)
            _sqlDescription = value
        End Set
    End Property
    Public Property Sqllength() As String
        Get
            Return _sqlLength
        End Get
        Set(ByVal value As String)
            _sqlLength = value
        End Set
    End Property
    Public Property SqlType() As String
        Get
            Return _sqlType
        End Get
        Set(ByVal value As String)
            _sqlType = value
        End Set
    End Property
    Public Property SqlComments() As String
        Get
            Return _sqlComments
        End Get
        Set(ByVal value As String)
            _sqlComments = value
        End Set
    End Property
    Public Property SqlMandatory() As String
        Get
            Return _sqlMandatory
        End Get
        Set(ByVal value As String)
            _sqlMandatory = value
        End Set
    End Property
    Public Property SqlDefault() As String
        Get
            Return _sqlDefault
        End Get
        Set(ByVal value As String)
            _sqlDefault = value
        End Set
    End Property

End Class
