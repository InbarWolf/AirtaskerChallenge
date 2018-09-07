using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using NUnit.Framework;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Web.Http;

namespace AirtaskerChallengeRateLimiter.Tests
{
	[TestFixture]
	public class RateLimitMiddlewareTests
	{
		HttpClient client;
		string localURI = "http://localhost:29792/";
		int rateLimit = 100;
		TimeSpan timeLimit = TimeSpan.FromSeconds(20);


		[SetUp]
		public void SetupTests()
		{
			client = new HttpClient();
			
			//Clear previous calls
			Thread.Sleep(timeLimit);
		}

		[Test]
		public void RateLimitMiddleware_FirstRequestApptoved_ResponseStatusOK()
		{
			var response = CallAPIMulitpuleTime(1);

			Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
		}

		[Test]
		public void RateLimitMiddleware_ReachRateLimit_ResponseStatusOK()
		{
			var response = CallAPIMulitpuleTime(rateLimit);

			Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
		}

		[Test]
		public void RateLimitMiddleware_ExceedRateLimit_ResponseStatus429()
		{
			var response = CallAPIMulitpuleTime(rateLimit + 1);

			Assert.AreEqual((HttpStatusCode)429, response.Result.StatusCode);
		}

		[Test]
		public void RateLimitMiddleware_ExceedRateLimitWaitTimeLimitAndRequestAgain_ResponseStatusOK()
		{
			var response = CallAPIMulitpuleTime(rateLimit + 1);

			Thread.Sleep(timeLimit);

			response = CallAPIMulitpuleTime(1);

			Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
		}


		private async Task<HttpResponseMessage> CallAPIMulitpuleTime(int numberOfCalls)
		{
			HttpResponseMessage response = null;

			for (int i = 0; i < numberOfCalls; i++)
			{
				var request = new HttpRequestMessage
				{
					RequestUri = new Uri(localURI),
					Method = HttpMethod.Get
				};
				response = await client.SendAsync(request);//.Result;
			}

			return response;
		}
	}
}