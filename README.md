# Couchbase Extensions #

[![Build status](https://ci.appveyor.com/api/projects/status/5jlr7hh5ay540guk?svg=true)](https://ci.appveyor.com/project/couchbaselabs/couchbase-extensions/branch/master)



A collection of extensions for the [Couchbase .Net SDK](https://github.com/couchbase/couchbase-net-client) designed to simplify working with Couchbase within .Net Core applications.

## Developer Guide ##
- [Couchbase.Extensions.Caching](docs/caching.md): A distributed Cache [Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware) for [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) and [Couchbase](http://couchbase.com) server
- [Couchbase.Extensions.Session:](docs/session.md) A distributed Session [Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware) for [ASP.NET Cor](https://docs.microsoft.com/en-us/aspnet/core/)e and [Couchbase](http://couchbase.com) Server
- [Couchbase.Extensions.DnsDiscovery](docs/dns-srv.md):
A .Net Core compatible DNS SRV mechanism for discovering a Couchbase cluster dynamically.
- [Couchbase.Extensions.DependencyInjection](docs/dependency-injection.md): A .Net Core style dependency injection framework for a Couchbase cluster and buckets.  It simplifies cluster configuration, lifetime management, and bucket injection.

## Using Couchbase 5.0 Role Based Authentication ##

New to Couchbase Server 5.0 is [Role Based Authentication](https://developer.couchbase.com/documentation/server/5.0/security/security-authorization.html) or RBAC, which requires a cluster level user with roles to be created on the server. Alternatively, you can use the cluster user and password to authenticate with any bucket on the server. To get started, we suggest you create a config file with your Username and Password fields matching the RBAC user you have created on the server (or the cluster admin in dev environments):

    {
      "Couchbase": {
        "Username": "Administrator",
        "Password" : "password",
        "Servers": [
          "http://localhost:8091/"
        ],
        "Buckets": [
          {
            "Name": "travel-sample"
          }
        ]
      }
    }
    
For examples checkout the [TestApp](tests/TestApp) and [Couchbase.Extensions.DependencyInjection.IntegrationTests](tests/Couchbase.Extensions.DependencyInjection.IntegrationTests) projects in this repo.

If you are using a Couchbase Server version that does not support RBAC (all Pre-5.0 servers), then you will use the traditional Bucket name and password to open the bucket.

## Licensing ##
Open source under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0) license. Please contribute :)
