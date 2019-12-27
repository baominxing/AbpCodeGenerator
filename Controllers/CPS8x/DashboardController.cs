using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ABPCodeGenerator.Controllers.CPS8x
{
    public class DashboardController : BaseController
    {
        public DashboardController(ILogger<BaseController> logger) : base(logger)
        {
        }

        public IActionResult Index()
        {
            _logger.LogInformation("ABPCodeGenerator.Controllers->DashboardController->Index");

            return View("~/Views/CPS8x/Dashboard/Index.cshtml");
        }
    }
}