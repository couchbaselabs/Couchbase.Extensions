# Couchbase Distributed Cache for ASP.NET Core #

A custom ASP.NET Core Middleware plugin for a distributed cache using Couchbase server as the backing store. Supports both Ephemeral (in-memory) and Couchbase (persistent) buckets.

## Getting Started ##

Assuming you have an [installation of Couchbase Server](https://docs.couchbase.com/server/current/introduction/intro.html) and Visual Studio (examples with VSCODE forthcoming), do the following:

### Couchbase .NET Core Distributed Cache ###

- Create a .NET Core Web Application using Visual Studio or VsCodeor CIL
- Install the package from [NuGet](https://www.nuget.org/packages/Couchbase.Extensions.Caching/) or build from source and add reference

### Setup ###

In Setup.cs add the following to the ConfigureServices method:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    services.AddCouchbase(opt =>
    {
        opt.ConnectionString = "couchbase://localhost";
        opt.UserName = "Administrator";
        opt.Password = "password";
    });

    services.AddDistributedCouchbaseCache(opt => { 
        opt.BucketName = "cache";
        opt.ScopeName = "my_service";
        opt.CollectionName = "my_collection";
    });
}
```

You can change the `localhost` hostname to wherever you are hosting your Couchbase cluster.

### Using Caching in your Controllers ###

In your controller add a parameter for `ICouchbaseCache` or `IDistributedCache` to the constructor.
Using `IDistributedCache` will allow persistence of `byte[]` to the cache. `ICouchbaseCache` extends
this functionality with additional methods for storing and retrieving typed objects.

```csharp
public class HomeController : Controller
{
    private ICouchbaseCache _cache;

    public HomeController(ICouchbaseCache cache)
    {
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        await _cache.SetAsync("CacheTime", DateTimeOffset.Now);
        return View();
    }

    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page. "
                    + (await _cache.GetAsync<DateTimeOffset>("CacheTime"));
        return View();
    }
}
```

For performance reasons, we strongly recommend using the Async overloads and not the synchronous methods on IDistributeCache.

## Serialization and Transcoding ##

Any type you persist in the cache will be transcoded and serialized using the transcoder and serializer
configured in the call to `AddCouchbase`. By default, this is the `JsonTranscoder` and the `DefaultSerializer`
which uses Newtonsoft.Json. However, setting and getting `byte[]` is always transcoded as raw binary regardless
of the configured transcoder.
