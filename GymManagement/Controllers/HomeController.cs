using System.Diagnostics;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.AnalyticsViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAnalyticsService _analyticsService;

        public HomeController(ILogger<HomeController> logger,IAnalyticsService analyticsService)
        {
            _logger = logger;
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var AnalyticsDetails = await _analyticsService.GetDataAsync(ct);
            

            return View(AnalyticsDetails.Value);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
