using LubeLogMCP.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace LubeLogMCP.MCP
{
    [McpServerToolType]
    public class LubeLoggerMCP
    {
        private string instance { get; set; }
        private string username { get; set; }
        private string password { get; set; }
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LubeLoggerMCP(IConfiguration _config, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            instance = _config["LUBELOG_INSTANCE"] ?? string.Empty;
            username = _config["LUBELOG_USER"] ?? string.Empty;
            password = _config["LUBELOG_PASS"] ?? string.Empty;
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        [McpServerTool, Description("Gets status of LubeLogger MCP.")]
        public async Task<string> GetLubeLoggerMCPStatus()
        {
            string result = string.Empty;
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                result += $"Auth Configured for {username}";
            } 
            else
            {
                result += "Auth Not Configured";
            }
            result += Environment.NewLine;
            if (!string.IsNullOrWhiteSpace(instance))
            {
                result += $"MCP Server Configured for {instance}";
                string endpoint = $"{instance}/api/version";
                var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                AddAuthHeaders(request);
                result += Environment.NewLine;
                try
                {
                    var httpClient = _httpClientFactory.CreateClient();
                    var serverResponse = await httpClient.SendAsync(request).Result.Content.ReadFromJsonAsync<ServerVersion>();
                    if (!string.IsNullOrWhiteSpace(serverResponse.CurrentVersion))
                    {
                        result += $"LubeLogger Version: {serverResponse.CurrentVersion}";
                    }
                }
                catch (Exception ex)
                {
                    result += $"Failed to connect to LubeLogger instance: {ex.Message}";
                }
            }
            else
            {
                result += "LubeLogger Instance not Configured";
            }
            return result;
        }
        [McpServerTool, Description("Gets vehicles in garage.")]
        public async Task<string> GetVehicles()
        {
            string endpoint = $"{instance}/api/vehicles";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadFromJsonAsync<List<Vehicle>>();
                var resultString = string.Empty;
                foreach(Vehicle vehicle in result)
                {
                    resultString += $"Id: {vehicle.Id} - {vehicle.Year} {vehicle.Make} {vehicle.Model}({vehicle.Identifier})";
                    resultString += Environment.NewLine;
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
            [Description("Any missed fuel ups")] bool missedFuelUp,
            [Description("Any extra fields configured for gasrecord")] List<ExtraField> extraFields)
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

            for(int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/gasrecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds a service record.")]
        public async Task<string> AddServiceRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date serviced")] DateTime date,
            [Description("Odometer at time of service")] int odometer,
            [Description("Description of items serviced")] string description,
            [Description("Total cost of the service")] decimal cost,
            [Description("Any extra fields configured for servicerecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("odometer", odometer.ToString()),
             new KeyValuePair<string, string>("description", description),
             new KeyValuePair<string, string>("cost", cost.ToString())
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/servicerecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds a repair record.")]
        public async Task<string> AddRepairRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date repaired")] DateTime date,
            [Description("Odometer at time of repair")] int odometer,
            [Description("Description of items repaired")] string description,
            [Description("Total cost of the repair")] decimal cost,
            [Description("Any extra fields configured for repairrecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("odometer", odometer.ToString()),
             new KeyValuePair<string, string>("description", description),
             new KeyValuePair<string, string>("cost", cost.ToString())
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/repairrecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds an upgrade record.")]
        public async Task<string> AddUpgradeRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date upgraded")] DateTime date,
            [Description("Odometer at time of upgrade")] int odometer,
            [Description("Description of items upgraded")] string description,
            [Description("Total cost of the upgrade")] decimal cost,
            [Description("Any extra fields configured for upgraderecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("odometer", odometer.ToString()),
             new KeyValuePair<string, string>("description", description),
             new KeyValuePair<string, string>("cost", cost.ToString())
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/upgraderecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds an odometer record.")]
        public async Task<string> AddOdometerRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date recorded")] DateTime date,
            [Description("Odometer recorded")] int odometer,
            [Description("Any extra fields configured for odometerrecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("odometer", odometer.ToString())
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/odometerrecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds a supply record.")]
        public async Task<string> AddSupplyRecord(
            [Description("id of the vehicle")] int vehicleId,
            [Description("Date purchased")] DateTime date,
            [Description("Description of the supply")] string description,
            [Description("Quantity purchased")] decimal quantity,
            [Description("Cost of the supply")] decimal cost,
            [Description("Part number")] string partNumber,
            [Description("Part supplier")] string partSupplier,
            [Description("Any extra fields configured for supplyrecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("description", description),
             new KeyValuePair<string, string>("partQuantity", quantity.ToString()),
             new KeyValuePair<string, string>("cost", cost.ToString()),
             new KeyValuePair<string, string>("partNumber", partNumber),
             new KeyValuePair<string, string>("partSupplier", partSupplier)
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/supplyrecords/add?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Adds a shop supply record.")]
        public async Task<string> AddShopSupplyRecord(
            [Description("Date purchased")] DateTime date,
            [Description("Description of the supply")] string description,
            [Description("Quantity purchased")] decimal quantity,
            [Description("Cost of the supply")] decimal cost,
            [Description("Part number")] string partNumber,
            [Description("Part supplier")] string partSupplier,
            [Description("Any extra fields configured for supplyrecord")] List<ExtraField> extraFields)
        {
            var dataParams = new List<KeyValuePair<string, string>>
        {
             new KeyValuePair<string, string>("date", date.ToString("yyyy-MM-dd")),
             new KeyValuePair<string, string>("description", description),
             new KeyValuePair<string, string>("partQuantity", quantity.ToString()),
             new KeyValuePair<string, string>("cost", cost.ToString()),
             new KeyValuePair<string, string>("partNumber", partNumber),
             new KeyValuePair<string, string>("partSupplier", partSupplier)
        };

            for (int i = 0; i < extraFields.Count; i++)
            {
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][name]", extraFields[i].Name));
                dataParams.Add(new KeyValuePair<string, string>($"extraFields[{i}][value]", extraFields[i].Value));
            }

            string endpoint = $"{instance}/api/vehicle/supplyrecords/add?vehicleId=0";

            var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new FormUrlEncodedContent(dataParams)
            };
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Gets latest odometer reading for a vehicle.")]
        public async Task<string> GetLatestOdometer(
            [Description("id of the vehicle")] int vehicleId
            )
        {

            string endpoint = $"{instance}/api/vehicle/odometerrecords/latest?vehicleId={vehicleId}";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadAsStringAsync();

                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [McpServerTool, Description("Get Extra Fields for a Record Type")]
        public async Task<string> GetExtraFields(
            [Description("Record type")] ImportMode importMode
            )
        {

            string endpoint = $"{instance}/api/extrafields";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            AddAuthHeaders(request);
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var result = await httpClient.SendAsync(request).Result.Content.ReadFromJsonAsync<List<ExtraFieldsVM>>();
                result.RemoveAll(x => x.RecordType != importMode.ToString());
                var serializedResult = JsonSerializer.Serialize(result);
                return serializedResult;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        private void AddAuthHeaders(HttpRequestMessage request)
        {
            //check if we have headers
            if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out var authHeader) ?? false)
            {
                request.Headers.Add("Authorization", authHeader.FirstOrDefault());
            }
            else if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                var authenticationString = $"{username}:{password}";
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
                request.Headers.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }
    }
}
