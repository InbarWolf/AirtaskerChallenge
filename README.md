# Airtasker challenge

The task is to produce a rate-limiting module that stops a particular requestor from making too many http requests within a particular period of time.

The module should expose a method that keeps track of requests and limits it such that a requester can only make 100 requests per hour. After the limit has been reached, return a 429 with the text "Rate limit exceeded. Try again in #{n} seconds".

Although you are only required to implement the strategy described above, it should be easy to extend the rate limiting module to take on different rate-limiting strategies.

How you do this is up to you. Think about how easy your rate limiter will be to maintain and control. Write what you consider to be production-quality code, with comments and tests if and when you consider them necessary.

## About The code
I've created a web API that returns a Chuck Norris random joke on every API request.
For my hosted solution, the user is allowed to do 5 calls per hour.
[Link](http://airtaskerchallengeratelimiter-wolfenfeld.us-east-2.elasticbeanstalk.com/)

In order to keep track of all the requests, I implemented a middleware called **RateLimitHandler**.
The middleware is registered in the pipeline to be called before reaching the requested route. 
RateLimitHandler is injected with an **IRateLimitRepository** object that manages the number of request per IP with their timestamp.

In my current solution, my concrete repository, **RateLimitCacheRepository**, is using the HttpContext.Current.Cache.

Both the rate and time limit are configurable in the WebConfigurationManager.AppSettings.

The initialization of the concrete repository, rate and time limits, and the middleware itself is done and added to the pipeline in the WebApiConfig.cs

```csharp
//Add rate limit middleware
int rateLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["RateLimit"]);
int timeLimit = Convert.ToInt32(WebConfigurationManager.AppSettings["TimeLimit"]);

config.MessageHandlers.Add(new RateLimitHandler(new RateLimitCacheRepository(rateLimit, timeLimit)));

```

