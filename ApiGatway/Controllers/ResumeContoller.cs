using Abstractions;
using Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiGatway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResumeContoller : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IEmailService _emailService;

        public ResumeContoller(IStorageService storageService, IEmailService emailService)
        {
            _storageService = storageService;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadResume(IFormFile formfile)
        {
            // Copy our request body into a memory stream
            Request.EnableBuffering();
            using var stream = new MemoryStream();

            //await Request.Body.CopyToAsync(stream);
            await formfile.CopyToAsync(stream);

            // Upload the stream contents to cloud storage
            var storedFileUrl = await _storageService.Upload(stream);
            
            // Email recruitment team with a link to the file
             await _emailService.Send("santosh3737@gmail.com",
                $"Somebody has uploaded a resume! Read it here: {storedFileUrl}");

            return Ok();
        }
    }
}
