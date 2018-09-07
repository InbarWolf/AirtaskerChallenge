using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtaskerChallengeRateLimiter.Middleware.RateLimitRepositories
{
	public interface IRateLimitRepository
	{
		void ProcessRequest(string id);
		double GetRequestTimeout(string id);
	}
}
