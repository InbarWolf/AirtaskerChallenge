using AirtaskerChallengeRateLimiter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;

namespace AirtaskerChallengeRateLimiter.Middleware.RateLimitRepositories
{
	public class RateLimitCacheRepository : IRateLimitRepository
	{
		private static readonly object cacheLock = new object();
		private int rateLimit;
		private TimeSpan timeLimit;

		public RateLimitCacheRepository(int rateLimit, int timeLimitInSeconds)
		{
			this.rateLimit = rateLimit;
			this.timeLimit = TimeSpan.FromSeconds(timeLimitInSeconds);
		}
		/// <summary>
		/// Saves the requests counter according to the request IP
		/// </summary>
		/// <param name="id"></param>
		public void ProcessRequest(string id)
		{
			string identification = id;

			var request = new RequestInfo
			{
				RequestTime = DateTime.Now,
				RequestCounter = 1
			};

			lock (cacheLock)
			{
				RequestInfo? currentRequest = (RequestInfo?)HttpContext.Current.Cache[identification];

				if (currentRequest.HasValue)
				{
					//if request from current IP is still within the last time limit, increment request counter
					if (currentRequest.Value.RequestTime + timeLimit >= DateTime.Now)
					{
						request = new RequestInfo
						{
							RequestCounter = currentRequest.Value.RequestCounter + 1,
							RequestTime = currentRequest.Value.RequestTime
						};

						HttpContext.Current.Cache[identification] = request;
						return;
					}
					//If previous calls time limit expired - set value to 1
					HttpContext.Current.Cache[identification] = request;
				}

				//New IP request
				HttpContext.Current.Cache.Add(identification, request, null,
						Cache.NoAbsoluteExpiration, timeLimit, CacheItemPriority.Low, null);
			}
		}

		/// <summary>
		/// Checks the timeout for the user according to the configured time limit
		/// If rate limit was not reached, returns 0
		/// </summary>
		/// <param name="id"></param>
		/// <returns>Timeout</returns>
		public double GetRequestTimeout(string id)
		{
			lock (cacheLock)
			{
				RequestInfo? currentRequest = (RequestInfo?)HttpContext.Current.Cache[id];

				//if request from current IP has exceeded rate count, return seconds till a new request is allowed
				if (currentRequest.HasValue && currentRequest.Value.RequestCounter > rateLimit)
					return timeLimit.TotalSeconds - (DateTime.Now - currentRequest.Value.RequestTime).Seconds;

				return 0;
			}
		}
	}
}