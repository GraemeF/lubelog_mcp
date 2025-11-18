using LubeLogMCP.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace LubeLogMCP.MCP
{
    [McpServerToolType]
    public class LubeLoggerMCP
    {
        private string instance { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        public LubeLoggerMCP()
        {
            instance = "path/to/your/lubelogger/instance";
            username = "username";
            password = "password";
        }
        [McpServerTool, Description("Gets vehicles in garage")]
        public async Task<string> GetVehicles()
        {
            string endpoint = $"{instance}/api/vehicles";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var authenticationString = $"{username}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
            try
            {
                var httpClient = new HttpClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadFromJsonAsync<List<Vehicle>>();
                var resultString = string.Empty;
                foreach(Vehicle vehicle in result)
                {
                    resultString += $"Id: {vehicle.Id} - {vehicle.Year} {vehicle.Make} {vehicle.Model}({vehicle.Identifier})";
                }
                return resultString;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds a fuel record.")]
        public async Task<string> AddFuelRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date of fuel up")] DateTime date,
            [Description("Odometer at time of fuel up")] int odometer,
            [Description("Volume of gas pumped")] decimal volume,
            [Description("Total cost of fuel up")] decimal cost,
            [Description("Is fueled up completely")] bool fillToFull,
            [Description("Any missed fuel ups")] bool missedFuelUp)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("odometer", odometer.ToString()),
             new KeyValuePair<string, string>("fuelConsumed", volume.ToString()),
             new KeyValuePair<string, string>("cost", cost.ToString()),
             new KeyValuePair<string, string>("isFillToFull", fillToFull.ToString()),
             new KeyValuePair<string, string>("missedFuelUp", missedFuelUp.ToString()),
        };

            string endpoint = $"{instance}/api/vehicle/gasrecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var authenticationString = $"{username}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
            try
            {
                var httpClient = new HttpClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
