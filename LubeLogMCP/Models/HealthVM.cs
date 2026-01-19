namespace LubeLogMCP.Models
{
    public class HealthVM
    {
        public string Instance { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
    }
}
