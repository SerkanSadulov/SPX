using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WEB.ViewModels;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFactory<string> _servicesEndpointFactory;
        private readonly string ServicesEndpoint;

        public HomeViewModel HomeViewModel = new();

        public HomeController(IWebHostEnvironment webHostEnvironment, IFactory<string> servicesEndpointFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _servicesEndpointFactory = servicesEndpointFactory;

            ServicesEndpoint = _servicesEndpointFactory.Get();

            HomeViewModel.ServicesEndpoint = ServicesEndpoint;
        }

        public IActionResult Home()
        {
            return View(HomeViewModel);
        }
        public IActionResult LogIn()
        {
            return View(HomeViewModel);
        }

        public IActionResult Register()
        {
            return View(HomeViewModel);
        }

        public IActionResult Chat()
        {
            return View(HomeViewModel);
        }
        public IActionResult HighestRated()
        {
            return View(HomeViewModel);
        }

        public IActionResult Profile(string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                ViewBag.ProfilePicturePath = filePath;
            }

            return View(HomeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot", "profilePictures");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var profilePictureName = profilePicture.FileName;
                var filePath = Path.Combine(uploadsFolder, profilePictureName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await profilePicture.CopyToAsync(fileStream);
                    }

                    return Ok(new { filePath = "/profilePics/" + profilePictureName });
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Error uploading profile picture: {ex.Message}" });
                }
            }

            return BadRequest(new { message = "No file uploaded or file is empty" });
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
