using LubeLogMCP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace LubeLogMCP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string instance { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IConfiguration _config, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            instance = _config["LUBELOG_INSTANCE"] ?? string.Empty;
            username = _config["LUBELOG_USER"] ?? string.Empty;
            password = _config["LUBELOG_PASS"] ?? string.Empty;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            //perform health check
            var viewModel = new HealthVM();
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                viewModel.UserName = username;
            }
            else
            {
                viewModel.UserName = "Not Configured";
            }
            if (!string.IsNullOrWhiteSpace(instance))
            {
                viewModel.Instance = instance;
                string endpoint = $"{instance}/api/version";
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    var authenticationString = $"{username}:{password}";
                    var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                    request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
                }
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var serverResponse = await httpClient.SendAsync(request).Result.Content.ReadFromJsonAsync<ServerVersion>();
                    if (!string.IsNullOrWhiteSpace(serverResponse?.CurrentVersion))
                    {
                        viewModel.IsConnected = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    viewModel.IsConnected = false;
                }
            }
            else
            {
                viewModel.Instance = "Not Configured";
            }
            return Json(viewModel);
        }
    }
}
