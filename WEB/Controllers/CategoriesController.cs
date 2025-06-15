using DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using WEB.ViewModels;

namespace WEB.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFactory<string> _servicesEndpointFactory;
        private readonly string ServicesEndpoint;

        public CategoriesViewModel CategoriesViewModel = new();
        public CategoriesController(IWebHostEnvironment webHostEnvironment, IFactory<string> servicesEndpointFactory)
        {
            _webHostEnvironment = webHostEnvironment;
            _servicesEndpointFactory = servicesEndpointFactory;

            ServicesEndpoint = _servicesEndpointFactory.Get();

            CategoriesViewModel.ServicesEndpoint = ServicesEndpoint;
        }
        public IActionResult Categories()
        {
            return View(CategoriesViewModel);
        }
        public IActionResult Error(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
