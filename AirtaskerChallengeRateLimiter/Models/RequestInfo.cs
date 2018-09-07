using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirtaskerChallengeRateLimiter.Models
{
	public struct RequestInfo
	{
		public int RequestCounter { get; set; }
		public DateTime RequestTime { get; set; }
	}
}