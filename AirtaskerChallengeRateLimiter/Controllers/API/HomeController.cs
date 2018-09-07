using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AirtaskerChallengeRateLimiter.Controllers.API
{
    public class HomeController : ApiController
    {
		[HttpGet]
		public HttpResponseMessage Index()
		{
			string joke = GetAChuckNorrisJokeFromAPI();

			return Request.CreateResponse(HttpStatusCode.OK, joke);
		}

		/// <summary>
		/// Calling an api that returns random Chuck Norris jokes
		/// </summary>
		/// <returns></returns>
		private string GetAChuckNorrisJokeFromAPI()
		{
			string apiUrl = "http://api.icndb.com/jokes/random";

			try
			{
				using (HttpClient client = new HttpClient())
				{
					client.BaseAddress = new Uri(apiUrl);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

					var response = client.GetAsync(apiUrl).Result;

					dynamic result = JObject.Parse(response.Content.ReadAsStringAsync().Result);
					return result.value.joke;
				}
			}
			catch { }

			return string.Empty;
		}
	}
}
