using System.Web.Http;
using AirtaskerChallengeRateLimiter.Middleware;
using AirtaskerChallengeRateLimiter.Middleware.RateLimitRepositories;
using System;
using System.Web.Configuration;

namespace AirtaskerChallengeRateLimiter
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
			//Set responses to JSON format
			var settings = config.Formatters.JsonFormatter.SerializerSettings;
			config.Formatters.Remove(config.Formatters.XmlFormatter);

			//Add rate limit middleware
			int rateLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["RateLimit"]);
			int timeLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["TimeLimit"]);

			config.MessageHandlers.Add(new RateLimitHandler(new RateLimitCacheRepository(rateLimit, timeLimit)));

			// Web API routes
			config.MapHttpAttributeRoutes();

			//Set Default route to Home controller
			config.Routes.MapHttpRoute(
				name: "Index",
				routeTemplate: "",
				defaults: new { controller = "Home", action = "Index" }
			);

			config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
