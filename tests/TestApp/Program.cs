using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestApp.Buckets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Register Couchbase with configuration section
builder.Services
    .AddCouchbase(options =>
    {
        builder.Configuration.GetSection("Couchbase").Bind(options);
    })
    .AddCouchbaseBucket<ITravelSampleBucketProvider>("travel-sample");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();

app.Services.GetRequiredService<ICouchbaseLifetimeService>().Close();
