using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WEB.ViewModels;

namespace WEB.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFactory<string> _servicesEndpointFactory;
        private readonly string ServicesEndpoint;

        public ServicesViewModel ServicesViewModel = new();

        public ServicesController(IWebHostEnvironment webHostEnvironment, IFactory<string> servicesEndpointFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _servicesEndpointFactory = servicesEndpointFactory;

            ServicesEndpoint = _servicesEndpointFactory.Get();

            ServicesViewModel.ServicesEndpoint = ServicesEndpoint;
        }

        public IActionResult AddService()
        {
            return View(ServicesViewModel);
        }

        public IActionResult Service()
        {
            return View(ServicesViewModel);
        }

        public IActionResult ServicesByCategory()
        {
            return View(ServicesViewModel);
        }

        public async Task<IActionResult> UploadServiceImage(IFormFile serviceImage)
        {
            if (serviceImage != null && serviceImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "serviceImages");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var serviceImageName = serviceImage.FileName;
                var filePath = Path.Combine(uploadsFolder, serviceImageName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await serviceImage.CopyToAsync(fileStream);
                    }

                    return Ok(new { filePath = "/serviceImages/" + serviceImageName });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error uploading service image: {ex.Message}" });
                }
            }

            return BadRequest(new { message = "No file uploaded or file is empty" });
        }

        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
