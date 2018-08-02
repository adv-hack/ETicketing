using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
namespace SystemDefaultsAPI
{
	public class ViewRenderer
	{
		protected ControllerContext Context { get; set; }
		public ViewRenderer(ControllerContext controllerContext)
		{
			if (controllerContext == null)
			{
				if (HttpContext.Current != null)
				{
					controllerContext = CreateController<SystemDefaultsController>().ControllerContext;
				}
				else
				{
					throw (new InvalidOperationException(
						"ViewRenderer must run in the context of an ASP.NET " +
						"Application and requires HttpContext.Current to be present."));
				}
			}
			Context = controllerContext;
		}
		public string RenderView(string viewPath, object model)
		{
			return RenderViewToStringInternal(viewPath, model, false);
		}
		public string RenderPartialView(string viewPath, object model)
		{
			return RenderViewToStringInternal(viewPath, model, true);
		}
		public static string RenderView(string viewPath, object model, ControllerContext controllerContext)
		{
			var renderer = new ViewRenderer(controllerContext);
			return renderer.RenderView(viewPath, model);
		}
		protected string RenderViewToStringInternal(string viewPath, object model, bool partialView = false)
		{
			ViewEngineResult viewEngineResult = null;
			if (partialView)
			{
				viewEngineResult = ViewEngines.Engines.FindPartialView(Context, viewPath);
			}
			else
			{
				viewEngineResult = ViewEngines.Engines.FindView(Context, viewPath, null);
			}
			if (viewEngineResult == null)
			{
				throw (new FileNotFoundException("View not found"));
			}
			var view = viewEngineResult.View;
			Context.Controller.ViewData.Model = model;
			object result = null;
			using (StringWriter sw = new StringWriter())
			{
				var ctx = new ViewContext(Context, view,
					Context.Controller.ViewData,
					Context.Controller.TempData,
					sw);
				view.Render(ctx, sw);
				result = sw.ToString();
			}
			return result.ToString();
		}
		public static T CreateController<T>(RouteData routeData = null) where T : Controller, new()
		{
			T controller = new T();
			HttpContextBase wrapper = null;
			if (HttpContext.Current != null)
			{
				wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
			}
			if (routeData == null)
			{
				routeData = new RouteData();
			}
			if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
			{
				routeData.Values.Add("controller", controller.GetType().Name.ToLower().Replace("controller", ""));
			}
			controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
			return controller;
		}
	}
}
