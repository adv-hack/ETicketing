Imports Microsoft.VisualBasic
Imports Talent.eCommerce

''' <summary>
''' The base class for talent user controls, which contains common code
''' for all user controls.
''' 
''' TODO - Jason Courcoux - 13/05/09 - We should pull up all common code
''' across the talent user controls into this class.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class AbstractTalentUserControl
    Inherits ControlBase

#Region "VARIABLES"
    Protected _businessUnit As String
    Protected _partner As String
    Protected _currentPage As String
    Protected ucr As New Talent.Common.UserControlResource
#End Region

    ''' <summary>
    ''' This should be called from all sub classes as in the following:-
    '''   MyBase.Page_Load( sender, e )
    ''' 
    ''' This sets the common variables and creates the module defaults, before
    ''' setting the user control resources for the control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _businessUnit = TalentCache.GetBusinessUnit
        _partner = TalentCache.GetPartner(HttpContext.Current.Profile)
        _currentPage = Talent.eCommerce.Utilities.GetCurrentPageName()
        setupUCR()
    End Sub

    ''' <summary>
    ''' This method should always be overidden to set the user control resources for
    ''' the specific sub class.
    ''' </summary>
    ''' <remarks></remarks>
    Protected MustOverride Sub setupUCR()

    Protected Sub ToggleControl(ByVal control As Control, ByVal enabled As Boolean)
        If TypeOf control Is WebControl Then
            CType(control, WebControl).Enabled = enabled
          End If
    End Sub

    ''' <summary>
    ''' Recursively adds controls of a page to a list. Useful when we want to make operations on all control on a page rather than just 
    ''' manually listing it in a non reusable way as it has been done all over the system.
    ''' </summary>
    ''' <param name="page">The root level page where we want to get controls for. Normally "Me.Controls"</param>
    ''' <param name="controls">The list of controls</param>
    ''' <remarks></remarks>
    Public Shared Sub AddControls(ByVal page As System.Web.UI.ControlCollection, ByVal controls As ArrayList)
        For Each c As System.Web.UI.Control In page
            controls.Add(c)
            If c.HasControls() Then
                AddControls(c.Controls, controls)
            End If
        Next
    End Sub
End Class
