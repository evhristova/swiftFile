using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using AspNetCoreDemo.Services.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace AspNetCoreDemo.Controllers.Web
{
    public class HomeController : Controller
    {
        private readonly ISwiftService swiftService;
        private readonly IWebHostEnvironment environment;
        private readonly ILoggerManager logger;

        public HomeController(
            ISwiftService swiftService, 
            IWebHostEnvironment environment,
            ILoggerManager logger
            )
        {
            this.swiftService = swiftService;
            this.environment = environment;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            logger.LogInfo(Constants.VisitedSwiftIndexViewLog);
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadTextFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                logger.LogInfo(Constants.NotFileSelectedErrorLog);
				return View(Constants.Index, Constants.FileSelectionError);
            }

            if (Path.GetExtension(file.FileName).ToLower() != Constants.FormatFile)
            {
                logger.LogInfo(Constants.FileSelectionErrorLog);
                return View(Constants.Index, Constants.FileSelectionError);
            }

            try
            {
                var uploadsFolder = Path.Combine(environment.WebRootPath, "files");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = DateTime.Now.ToString(Constants.FormatDate) + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var result = await this.swiftService.CreateAsync(filePath);
                if (!result.IsSuccessful)
                {
                    logger.LogError(result.Message);
                    return this.View(Constants.Index, result.Message);
                }
                logger.LogInfo(Constants.FileUploadSuccessful);
                return this.View(Constants.Index, Constants.FileUploadSuccessful);
			}
            catch (Exception ex)
            {
                logger.LogError(Constants.FileUploadError + ex.Message);
                return View(Constants.Index, Constants.FileUploadError + ex.Message);
            }
        }
    }
}

