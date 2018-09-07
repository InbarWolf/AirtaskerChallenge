using AirtaskerChallengeRateLimiter.Middleware.RateLimitRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace AirtaskerChallengeRateLimiter.Middleware
{
	public class RateLimitHandler : DelegatingHandler
	{
		IRateLimitRepository repository;

		/// <summary>
		/// The RateLimiter middleware requires a repository to save requests.
		/// Upon initialization will be injected with the wanted repository 
		/// </summary>
		/// <param name="rateLimiterRepositry"></param>
		public RateLimitHandler(IRateLimitRepository rateLimiterRepositry)
		{
			repository = rateLimiterRepositry;
		}

		/// <summary>
		/// Called with any request to the API
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var identifier = HttpContext.Current.Request.UserHostAddress;
			repository.ProcessRequest(identifier);
			var secondsTillNextTime = repository.GetRequestTimeout(identifier);

			if (secondsTillNextTime != 0)
				return CreateRateLimitReachedReponse(identifier, secondsTillNextTime);

			return base.SendAsync(request, cancellationToken);

		}

		private Task<HttpResponseMessage> CreateRateLimitReachedReponse(string identifier, double secondsTillNextTime)
		{
			var response = new HttpResponseMessage
			{
				Content = new StringContent($"Rate limit exceeded. Try again in #{secondsTillNextTime} seconds"),
				StatusCode = (HttpStatusCode)429
			};

			var taskComplete = new TaskCompletionSource<HttpResponseMessage>();
			taskComplete.SetResult(response);
			return taskComplete.Task;
		}

	}
}