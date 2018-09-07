# Airtasker challenge

### I've created a web API that returns a Chuck Norris random joke on every API request.
### For my hosted solution, the user is allowed to do 5 calls per 20 secnonds[Link](http://airtaskerchallengeratelimiter-wolfenfeld.us-east-2.elasticbeanstalk.com/)

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

