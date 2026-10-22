using System.Web.Mvc;
using System.Web.Routing;

namespace POOI_EF_SalgadoAmarillo
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Consumidor", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
