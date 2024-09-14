using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebAppProcessingExample.Models;
using WebAppProcessingExample.Services;

namespace WebAppProcessingExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var dbService = new DBServices();
            var result = await dbService.GetAnalysisAsync();
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var storageService = new StorageService();
            var fileName=Guid.NewGuid().ToString()+Path.GetExtension(file.FileName);
            var url= await storageService.UploadFile("pictures", fileName, file.OpenReadStream());

            //Add the Data to the database
            var dbService = new DBServices();
            var insert_data = new AnalysisModel
            {
                PKId = Guid.NewGuid().ToString(),
                Name = file.FileName,
                ImageUrl = url,
                IsProcessed = false,
                Result = "UNPROCCESSED"
            };
            await dbService.InsertAnalysisAsync(insert_data);


            var queueService = new QueueService();
            await queueService.SendMessageAsync(JsonConvert.SerializeObject(insert_data));

            

            return RedirectToAction("Index");
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
