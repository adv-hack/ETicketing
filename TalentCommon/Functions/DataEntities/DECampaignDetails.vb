'--------------------------------------------------------------------------------------------------
'       Project                     Trading Portal Project.
'
'       Function                    This class is used to deal with Product Details
'
'       Date                        8th Jan 2008
'
'       Author                      Ben Ford
'
'       � CS Group 2007             All rights reserved.
'
'       Error Number Code base      - 
'                                   
'--------------------------------------------------------------------------------------------------
<Serializable()> _
Public Class DECampaignDetails
    '
    Private _sessionId As String = ""
    Private _campaignCode As String = ""
  
    '
    Public Property SessionId() As String
        Get
            Return _sessionId
        End Get
        Set(ByVal value As String)
            _sessionId = value
        End Set
    End Property
    Public Property CampaignCode() As String
        Get
            Return _campaignCode
        End Get
        Set(ByVal value As String)
            _campaignCode = value
        End Set
    End Property
   
End Class
