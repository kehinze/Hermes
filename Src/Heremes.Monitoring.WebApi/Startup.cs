using System;
using System.IO;
using System.Web.Http;
using Hermes.Monitoring.WebApi;
using Hermes.Monitoring.WebApi.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace Hermes.Monitoring.WebApi
{
    public static class RouteNames
    {
        public static string DefaultApi = "defaultApi";
    }

    public class Startup
    {
        private HermesPerformenceTimer hermesPerformenceTimer;
        private BackgroundServerTimeTimer backgroundServerTimeTimer;
        public void Configuration(IAppBuilder app)
        { 
            hermesPerformenceTimer = new HermesPerformenceTimer();
            backgroundServerTimeTimer = new BackgroundServerTimeTimer();
            app.UseCors(CorsOptions.AllowAll);

            string contentPath = Path.Combine(Environment.CurrentDirectory, @"/Web");

            app.UseStaticFiles(contentPath);

            var config = ConfigureWebApi();

            app.UseWebApi(config);
            app.MapSignalR();
        }

        private static HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: RouteNames.DefaultApi,
                routeTemplate: "api/{controller}/{id}",
                defaults: new
                {
                    id = RouteParameter.Optional
                });
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            return config;
        }
    }
}