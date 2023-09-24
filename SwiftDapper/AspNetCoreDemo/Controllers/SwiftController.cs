using AspNetCoreDemo.Models;
using AspNetCoreDemo.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Controllers
{
    public class SwiftController : Controller
    {
        private readonly ISwiftService swiftService;
        private readonly ILoggerManager logger;

        public SwiftController(
            ISwiftService swiftService,
            ILoggerManager logger)
        {
            this.swiftService = swiftService;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = await this.swiftService.GetAllAsync();
            if (!result.IsSuccessful)
            {
                logger.LogWarn(Constants.NoRecordsErrorLog);
                this.ViewData[Constants.ErrorMessage] = result.Message;
                return View(result.Data);
            }
            logger.LogInfo(Constants.VisitedSwiftIndexViewLog);
            return View(result.Data);
        }
    }
}
