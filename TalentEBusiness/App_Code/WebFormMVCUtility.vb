Imports Microsoft.VisualBasic
Imports System.IO
Imports System.Web.HttpContext
Imports System.Web.Routing
Imports System.Web.Mvc
Imports TalentBusinessLogic.Models

''' <summary>
''' Webform controller class inheriting from MVC Controller
''' </summary>
''' <remarks></remarks>
Public Class WebFormController
    Inherits Controller

End Class

''' <summary>
''' Webform MVC Utility class to handle view rendering using the controller context
''' </summary>
''' <remarks></remarks>
Public Class WebFormMVCUtil

#Region "Public Functions"

    ''' <summary>
    ''' Render a partial view as a stringwriter based on the given parameters using the HTTPContext and the Controller Context inherited from the MVC namespace
    ''' </summary>
    ''' <param name="partialViewPath">The local partial view path string</param>
    ''' <param name="model">The view model to use to render from</param>
    ''' <returns>The rendered view as a string writer</returns>
    ''' <remarks></remarks>
    Public Function RenderPartial(ByVal partialViewPath As String, ByVal model As BaseViewModel) As StringWriter
        Dim httpContext As HttpContextWrapper = New HttpContextWrapper(Current)
        Dim route As RouteData = Nothing
        Dim cContext As ControllerContext = Nothing
        Dim vContext As ViewContext = Nothing
        Dim view As IView = Nothing
        Dim viewDictionary As New ViewDataDictionary
        Dim writer As New StringWriter

        route = New RouteData()
        route.Values.Add("controller", "WebFormController")
        cContext = New ControllerContext(New RequestContext(httpContext, route), New WebFormController())
        view = ViewEngines.Engines.FindPartialView(cContext, partialViewPath).View
        viewDictionary.Model = model
        vContext = New ViewContext(cContext, view, viewDictionary, New TempDataDictionary, New StringWriter)
        view.Render(vContext, writer)
        Return writer
    End Function

#End Region

End Class
