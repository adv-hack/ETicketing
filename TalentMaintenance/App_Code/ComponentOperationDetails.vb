Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Data
Imports Talent.Common
Imports System.Web
Imports Talent.Common.UtilityExtension
Imports System.Linq

Public Class ComponentOperationDetails
    Implements IComponentOperationDetails
    Public Property Id() As Long = 0 Implements IComponentOperationDetails.Id
    Public Property Operation() As New OperationMode Implements IComponentOperationDetails.Operation
    'can be converted to System.EventArgs to support the other databound controls but most probably the repeater would be used all the times.
    Public Property RepCommandEventArgs() As System.Web.UI.WebControls.RepeaterCommandEventArgs = Nothing Implements IComponentOperationDetails.RepCommandEventArgs
    Public Property TalPackage() As New TalentPackage Implements IComponentOperationDetails.TalPackage
    Public Property SequenceMode As SequenceMode Implements IComponentOperationDetails.SequenceMode  ' = Nothing
End Class

Public Interface IComponentOperationDetails
    Property Id() As Long
    Property Operation() As OperationMode
    Property RepCommandEventArgs() As System.Web.UI.WebControls.RepeaterCommandEventArgs
    Property TalPackage() As TalentPackage
    Property SequenceMode As SequenceMode
End Interface
