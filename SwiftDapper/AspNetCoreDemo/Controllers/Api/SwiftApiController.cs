using System;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreDemo.Services.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.Controllers
{
    [ApiController]
    [Route("api/swift")]
    public class SwiftApiController : ControllerBase
    {
        private readonly ISwiftService swiftService;
        private readonly IWebHostEnvironment environment;
        private readonly ILoggerManager logger;

        public SwiftApiController(
            ISwiftService swiftService,
            IWebHostEnvironment environment,
            ILoggerManager logger)
        {
            this.swiftService = swiftService;
            this.environment = environment;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await this.swiftService.GetAllAsync();
            if (!result.IsSuccessful)
            {
                logger.LogWarn(Constants.NoRecordsErrorLog);
                return this.StatusCode(StatusCodes.Status404NotFound, result.Message);
            }
            logger.LogInfo(Constants.GetAllSuccessful);
            return this.StatusCode(StatusCodes.Status200OK, result.Data);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                logger.LogInfo(Constants.NotFileSelectedErrorLog);
                return BadRequest(Constants.FileSelectionError);
            }

            if (Path.GetExtension(file.FileName).ToLower() != Constants.FormatFile)
            {
                logger.LogInfo(Constants.FileSelectionErrorLog);
                return BadRequest(Constants.FileSelectionError);
            }

            try
            {
                var uploadsFolder = Path.Combine(environment.WebRootPath, "files");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var result = await this.swiftService.CreateAsync(filePath);
                if (!result.IsSuccessful)
                {
                    logger.LogError(result.Message);
                    return BadRequest(result.Message);
                }
                logger.LogInfo(Constants.FileUploadSuccessful);
                return Ok(Constants.FileUploadSuccessful);
            }
            catch (Exception ex)
            {
                logger.LogError(Constants.FileUploadError + ex.Message);
                return BadRequest(Constants.FileUploadError + ex.Message);
            }
        }
    }
}
