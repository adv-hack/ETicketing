Imports Microsoft.VisualBasic
'--------------------------------------------------------------------------------------------------
'       Project                     Trading E-Commerce
'
'       Function                    Talent Membership User 
'
'       Date                        Feb 2007
'
'       Author                       
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      ACTMSU- 
'                                    
'
'       Modification Summary
'
'       dd/mm/yy    ID      By      Description
'       --------    -----   ---     -----------
'
'--------------------------------------------------------------------------------------------------
Public Class TalentMembershipUser
    Inherits System.Web.Security.MembershipUser

    Private _auto_process_default_user As Boolean
    Private _businessUnit As String
    Private _partner As String
    Private _password As String

    Public Property AutoProcessDefaultUser() As Boolean
        Get
            Return _auto_process_default_user
        End Get
        Set(ByVal value As Boolean)
            _auto_process_default_user = value
        End Set
    End Property
    Public Property BusinessUnit() As String
        Get
            Return _businessUnit
        End Get
        Set(ByVal value As String)
            _businessUnit = value
        End Set
    End Property
    Public Property Partner() As String
        Get
            Return _partner
        End Get
        Set(ByVal value As String)
            _partner = value
        End Set
    End Property
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property

    Public Sub New(ByVal providername As String, _
                            ByVal businessUnit As String, _
                            ByVal partner As String, _
                            ByVal loginID As String, _
                            ByVal email As String, _
                            ByVal auto_process_default_user As Boolean, _
                            ByVal ProviderUserKey As Object, _
                            ByVal PwQuestion As String, _
                            ByVal Comment As String, _
                            ByVal IsApproved As Boolean, _
                            ByVal IsLockedOut As Boolean, _
                            ByVal CreationDate As Date, _
                            ByVal LastLoginDate As Date, _
                            ByVal LastActivityDate As Date, _
                            ByVal LastPasswordChangedDate As Date, _
                            ByVal lastLockedOutDate As Date, _
                            ByVal password As String)

        MyBase.New(providername, _
                   loginID, _
                   ProviderUserKey, _
                   email, _
                   PwQuestion, _
                   Comment, _
                   IsApproved, _
                   IsLockedOut, _
                   CreationDate, _
                   LastLoginDate, _
                   LastActivityDate, _
                   LastPasswordChangedDate, _
                   lastLockedOutDate)

        With Me
            .AutoProcessDefaultUser = auto_process_default_user
            .BusinessUnit = businessUnit
            .Partner = partner
            .Password = password
        End With

    End Sub

End Class
