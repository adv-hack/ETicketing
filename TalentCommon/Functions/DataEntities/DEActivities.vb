Imports System.Text

<Serializable()> _
Public Class DEActivities

    Public Property CustomerActivitiesHeaderMode() As String
    Public Property CustomerActivitiesCommentsMode() As String
    Public Property CustomerActivitiesAttachmentsMode() As String
    Public Property CustomerActivitiesHeaderID() As Integer
    Public Property CustomerNumber() As String
    Public Property TemplateID() As Integer
    Public Property ActivityUserName() As String
    Public Property ActivityDate() As String
    Public Property ActivitySubject() As String
    Public Property ActivityStatus() As String
    Public Property ActivityQuestionIDArray() As String
    Public Property ActivityQuestionTextArray() As String
    Public Property ActivityAnswerTextArray() As String
    Public Property ActivityTextDelimiter() As String
    Public Property SortOrder() As String
    Public Property ActivityOrderDirection() As String
    Public Property Length() As Integer
    Public Property Start() As Integer
    Public Property Draw() As String
    Public Property ActivityCommentID() As Integer
    Public Property ActivityCommentText() As String
    Public Property ActivityFileAttachmentID() As Integer
    Public Property ActivityFileName() As String
    Public Property ActivityFileDescription() As String
    Public Property Source() As String
    Public Property ActivityCallId() As Integer

    Public Function LogString() As String
        Dim logDetails As New StringBuilder
        Dim comma As String = ","
        logDetails.Append(CustomerActivitiesHeaderMode).Append(comma)
        logDetails.Append(CustomerActivitiesHeaderID).Append(comma)
        logDetails.Append(CustomerNumber).Append(comma)
        logDetails.Append(TemplateID).Append(comma)
        logDetails.Append(ActivityUserName).Append(comma)
        logDetails.Append(ActivityDate).Append(comma)
        logDetails.Append(ActivitySubject).Append(comma)
        logDetails.Append(Source)
        Return logDetails.ToString()
    End Function

End Class
