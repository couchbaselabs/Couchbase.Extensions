using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestApp.Buckets;
using TestApp.Models;

namespace TestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITravelSampleBucketProvider _bucketProvider;

        public HomeController(ITravelSampleBucketProvider bucketProvider)
        {
            _bucketProvider = bucketProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Airlines()
        {
            var bucket = await _bucketProvider.GetBucketAsync();

            var result =
                await bucket.Cluster.QueryAsync<Airline>(
                    "SELECT Extent.* FROM `travel-sample` AS Extent WHERE type = 'airline' ORDER BY name");

            return View(await result.Rows.ToListAsync());
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
